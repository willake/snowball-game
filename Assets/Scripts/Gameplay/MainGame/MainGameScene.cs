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
        public PlayerCamera playerCamera;
        public CharacterFactory characterFactory;
        public EnvironmentVFXManager vfxManager;
        public LevelManager levelManager;

        private GameHUDPanel _gameHUDPanel;

        private void Start()
        {
            if (GameManager.instance)
            {
                levelManager.LoadLevel(GameManager.instance.levelToLoad).Forget();
            }
            else
            {
                levelManager.LoadLevel(AvailableLevel.Test).Forget();
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
                _gameHUDPanel.BindCharacter(playerController.bindedCharacter);
            }
        }
    }
}
