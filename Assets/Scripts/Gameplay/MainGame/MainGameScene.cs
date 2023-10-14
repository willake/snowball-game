using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.UI;
using Cysharp.Threading.Tasks;
using Game.Gameplay.Cameras;
using Game.Audios;

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

        private GameHUDPanel _gameHUDPanel;
        private PlayerController _player;
        private HashSet<int> _enemyList = new HashSet<int>();

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

            WrappedAudioClip audioStart = ResourceManager.instance?.audioResources.gameplayAudios.levelStart;
            AudioManager.instance?.PlaySFX(
                audioStart.clip,
                audioStart.volume
            );
        }

        public void RegisterPlayer(PlayerController playerController)
        {
            _player = playerController;
            if (UIManager.instance)
            {
                playerCamera = playerController.bindedCamera.GetCamera();
                _gameHUDPanel.BindCharacter(playerController.bindedCharacter);
            }
        }

        public void EliminatePlayer(PlayerController playerController)
        {
            Debug.Log("Player lose!");
            if (UIManager.instance)
            {
                EndGamePanel panel = UIManager.instance.OpenUI(AvailableUI.EndGamePanel) as EndGamePanel;
                panel.SetEndGameState(EndGamePanel.EndGameState.Lose);
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

            if (_enemyList.Count <= 0)
            {
                Debug.Log("Player wins");
                _player.isControllable = false;
                EndGamePanel panel = UIManager.instance.OpenUI(AvailableUI.EndGamePanel) as EndGamePanel;
                panel.SetEndGameState(EndGamePanel.EndGameState.Win);
            }
        }
    }
}
