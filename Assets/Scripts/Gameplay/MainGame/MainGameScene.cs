using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.UI;
using Cysharp.Threading.Tasks;
using Game.Gameplay.Cameras;

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
        private HashSet<int> _enemyList = new HashSet<int>();

        private void Start()
        {
            if (GameManager.instance)
            {
                levelLoader.LoadLevel(GameManager.instance.levelToLoad).Forget();
            }
            else
            {
                levelLoader.LoadLevel(AvailableLevel.Test).Forget();
            }

            // Character player = characterFactory.GeneratePlayer("Player");
            // player.GetComponent<PlayerController>().BindCamera(playerCamera);

            if (UIManager.instance)
            {
                _gameHUDPanel = UIManager.instance.OpenUI(AvailableUI.GameHUDPanel) as GameHUDPanel;
            }
        }

        public void RegisterPlayer(PlayerController playerController)
        {
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
                EndGamePanel panel = UIManager.instance.OpenUI(AvailableUI.EndGamePanel) as EndGamePanel;
                panel.SetEndGameState(EndGamePanel.EndGameState.Win);
            }
        }
    }
}
