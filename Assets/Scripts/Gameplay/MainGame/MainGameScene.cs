using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.UI;
using Game.Gameplay.Cameras;

namespace Game.Gameplay
{
    public class MainGameScene : GameScene<MainGameScene>
    {
        public PlayerCamera playerCamera;
        public CharacterFactory characterFactory;
        public EnvironmentVFXManager vfxManager;

        private void Start()
        {
            Character player = characterFactory.GeneratePlayer("Player");
            player.GetComponent<PlayerController>().BindCamera(playerCamera);

            if (UIManager.instance)
            {
                GameHUDPanel hud = UIManager.instance.OpenUI(AvailableUI.GameHUDPanel) as GameHUDPanel;
                hud.BindCharacter(player);
            }
        }
    }
}
