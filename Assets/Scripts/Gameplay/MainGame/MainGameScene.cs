using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.UI;
using System;
using Game.Audios;
using Game.Events;
using DG.Tweening;
using Game.RuntimeStates;
using Cysharp.Threading.Tasks;
using Game.WebRequests;

namespace Game.Gameplay
{
    public class MainGameScene : GameScene<MainGameScene>
    {
        [Header("References")]
        public BoolState isGameRunningState;
        public Camera playerCamera;
        public CharacterFactory characterFactory;
        public EnvironmentVFXManager vfxManager;
        public LevelLoader levelLoader;
        public Canvas worldSpaceCanvas;
        public GameStatisticsCollector gameStatisticsCollector;

        private GameHUDPanel _gameHUDPanel;
        private EndGamePanel _endGamePanel;
        private PlayerController _player;
        private HashSet<int> _enemyList = new HashSet<int>();
        private HashSet<int> _bossList = new HashSet<int>();
        private int _ambienceWindLoopID = 0;

        private Lazy<EventManager> _eventManager = new Lazy<EventManager>(
            () => DIContainer.instance.GetObject<EventManager>(),
            true
        );
        protected EventManager EventManager { get => _eventManager.Value; }
        private Lazy<WebRequestManager> _webRequestManager = new Lazy<WebRequestManager>(
           () => DIContainer.instance.GetObject<WebRequestManager>(),
           true
       );
        protected WebRequestManager WebRequestManager { get => _webRequestManager.Value; }

        public bool IsGameRunning { get; private set; }

        private int _initalBossCount = 0;

        private float _playTimeInSeconds = 0f;

        private async void Start()
        {
            IsGameRunning = false;
            isGameRunningState.value = false;
            if (GameManager.instance)
            {
                await levelLoader.LoadLevel(GameManager.instance.levelToLoad);
            }
            else
            {
                await levelLoader.LoadLevel(AvailableLevel.Test);
            }

            // Character player = characterFactory.GeneratePlayer("Player");
            // player.GetComponent<PlayerController>().BindCamera(playerCamera);

            GameStartPanel startPanel =
                await UIManager.instance.OpenUIAsync(AvailableUI.GameStartPanel) as GameStartPanel;

            await startPanel.ShowText("- Ready -", 2f, Ease.InOutSine);
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            await startPanel.ShowText("- Start -", 1f, Ease.InOutSine);
            await UIManager.instance.PrevAsync();
            if (UIManager.instance)
            {
                _gameHUDPanel = UIManager.instance.OpenUI(AvailableUI.GameHUDPanel) as GameHUDPanel;
                playerCamera = _player.bindedCamera.GetCamera();
                _gameHUDPanel.BindController(_player);
            }

            WrappedAudioClip audioStart =
                ResourceManager.instance?.audioResources.gameplayAudios.levelStart;
            AudioManager.instance?.PlaySFX(
                audioStart.clip,
                audioStart.volume
            );

            WrappedAudioClip ambienceWind =
                ResourceManager.instance?.audioResources.backgroundAudios.ambienceWind;
            _ambienceWindLoopID = AudioManager.instance.PlaySFXLoop(
                ambienceWind.clip,
                ambienceWind.volume
            );
            OnStartGame();
        }

        public void OnStartGame()
        {
            IsGameRunning = true;
            isGameRunningState.value = true;
            _initalBossCount = _bossList.Count;

            gameStatisticsCollector.StartRecording(
                GetLevelNumer(GameManager.instance.levelToLoad), _enemyList.Count);
            _gameHUDPanel.UpdateTimeText(0);
        }

        private int GetLevelNumer(AvailableLevel availableLevel)
        {
            switch (availableLevel)
            {
                case AvailableLevel.Test:
                default:
                    return 0;
                case AvailableLevel.Level1:
                    return 1;
                case AvailableLevel.Level2:
                    return 2;
                case AvailableLevel.Level3:
                    return 3;
            }
        }

        public async void OnEndGame(bool isWin)
        {
            AudioManager.instance.StopSFXLoop(_ambienceWindLoopID);
            IsGameRunning = false;
            isGameRunningState.value = false;
            EventManager.Publish(
                EventNames.onGameEnd,
                new Payload()
                {
                    args = new object[] { isWin }
                }
            );
            gameStatisticsCollector.StopRecording(isWin, (long)_playTimeInSeconds);

            EndGamePanel panel =
                    await UIManager.instance.OpenUIAsync(AvailableUI.EndGamePanel) as EndGamePanel;
            panel.SetEndGameState(isWin ? EndGamePanel.EndGameState.Win : EndGamePanel.EndGameState.Lose);
            panel.UpdateStatisticsInformation(gameStatisticsCollector.StatisticsData);

            if (GameManager.instance.levelToLoad != AvailableLevel.Level1) return;

            var result = await
                WebRequestManager.UploadPlayData(gameStatisticsCollector.StatisticsData);

            ModalPanel modalPanel =
                UIManager.instance.OpenUI(AvailableUI.ModalPanel) as ModalPanel;

            if (result.isSuccess)
            {
                modalPanel.SetContent($"The playing data has successfully been sent. ({result.responseCode})");
            }
            else
            {
                modalPanel.SetContent($"Failed to send data. Reason: {result.message} ({result.responseCode})");
            }
        }

        public void RegisterPlayer(PlayerController playerController)
        {
            _player = playerController;
        }

        public void EliminatePlayer(PlayerController playerController)
        {
            EventManager.Publish(
                EventNames.onPlayerDead,
                new Payload()
            );

            if (playerController.Revive())
            {

            }
            else
            {
                Debug.Log("Player lose!");
                OnEndGame(false);
            }
        }

        /* Register enemy to enemylist and return an ID. */
        public void RegisterEnemy(AIController aiController, EnemyType enemyType)
        {
            _enemyList.Add(aiController.GetInstanceID());

            if (enemyType == EnemyType.Boss)
            {
                _bossList.Add(aiController.GetInstanceID());
            }
        }

        public void EliminateEnemy(AIController aiController, EnemyType enemyType)
        {
            _enemyList.Remove(aiController.GetInstanceID());

            if (enemyType == EnemyType.Boss)
            {
                _bossList.Remove(aiController.GetInstanceID());
            }

            EventManager.Publish(
                EventNames.onEnemyDead,
                new Payload()
                {
                    args = new object[] { enemyType }
                }
            );

            Destroy(aiController.gameObject);

            // if the level has no bosses, win when all enemies are dead
            // if the level has bosses, win when the bosses are dead
            if ((_initalBossCount == 0 && _enemyList.Count <= 0) ||
                (_initalBossCount > 0 && _bossList.Count <= 0))
            {
                // win when all enemies are dead
                Debug.Log("Player wins");
                OnEndGame(true);
            }
        }

        public async UniTask NavigateToMenu(bool finished)
        {
            IsGameRunning = false;
            isGameRunningState.value = false;
            if (finished == false)
            {
                gameStatisticsCollector.StopRecordingWithoutSave();
                AudioManager.instance.StopSFXLoop(_ambienceWindLoopID);
            }
            Destroy(_player.gameObject);
            await levelLoader.UnloadCurrentLevel();
            GameManager.instance.SwitchScene(AvailableScene.Menu);
        }

        private void FixedUpdate()
        {
            if (IsGameRunning == false) return;
            _playTimeInSeconds += Time.fixedDeltaTime;
            _gameHUDPanel.UpdateTimeText((long)_playTimeInSeconds);
        }
    }
}
