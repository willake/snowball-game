using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.UI;
using System;
using Game.Audios;
using Game.Events;

namespace Game.Gameplay
{
    public class MainGameScene : GameScene<MainGameScene>
    {
        [Header("References")]
        public Camera playerCamera;
        public CharacterFactory characterFactory;
        public EnvironmentVFXManager vfxManager;
        public LevelLoader levelLoader;
        public Canvas worldSpaceCanvas;
        public GameStatisticsCollector gameStatisticsCollector;

        private GameHUDPanel _gameHUDPanel;
        private PlayerController _player;
        private HashSet<int> _enemyList = new HashSet<int>();
        private int _ambienceWindLoopID = 0;

        private Lazy<EventManager> _eventManager = new Lazy<EventManager>(
            () => DIContainer.instance.GetObject<EventManager>(),
            true
        );
        protected EventManager EventManager { get => _eventManager.Value; }

        public bool IsGameRunning { get; private set; }

        private async void Start()
        {
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

            if (UIManager.instance)
            {
                _gameHUDPanel = UIManager.instance.OpenUI(AvailableUI.GameHUDPanel) as GameHUDPanel;
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
            // gameStatisticsCollector.StartRecording();
        }

        public void OnEndGame(bool isWin)
        {
            IsGameRunning = false;
            EventManager.Publish(
                EventNames.onGameEnd,
                new Payload()
                {
                    args = new object[] { isWin }
                }
            );
            // gameStatisticsCollector.StopRecording(isWin);

        }

        public void RegisterPlayer(PlayerController playerController)
        {
            _player = playerController;
            if (UIManager.instance)
            {
                playerCamera = playerController.bindedCamera.GetCamera();
                _gameHUDPanel.BindController(playerController);
            }
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
                if (UIManager.instance)
                {
                    EndGamePanel panel = UIManager.instance.OpenUI(AvailableUI.EndGamePanel) as EndGamePanel;
                    panel.SetEndGameState(EndGamePanel.EndGameState.Lose);
                }
            }
        }

        /* Register enemy to enemylist and return an ID. */
        public void RegisterEnemy(AIController aiController)
        {
            _enemyList.Add(aiController.GetInstanceID());
        }

        public void EliminateEnemy(AIController aiController)
        {
            _enemyList.Remove(aiController.GetInstanceID());

            EventManager.Publish(
                EventNames.onEnemyDead,
                new Payload()
            );

            if (_enemyList.Count <= 0)
            {
                Debug.Log("Player wins");
                OnEndGame(true);
                EndGamePanel panel = UIManager.instance.OpenUI(AvailableUI.EndGamePanel) as EndGamePanel;
                panel.SetEndGameState(EndGamePanel.EndGameState.Win);
            }
        }

        private void OnDestroy()
        {
            AudioManager.instance.StopSFXLoop(_ambienceWindLoopID);
        }
    }
}
