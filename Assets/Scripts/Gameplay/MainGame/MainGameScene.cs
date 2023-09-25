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
        public PlayerController playerController;
        public CharacterFactory characterFactory;
        public WeaponFactory weaponFactory;
        public EnvironmentVFXManager vfxManager;

        private void Start()
        {
            if (UIManager.instance)
            {
                UIManager.instance.OpenUI(AvailableUI.GameHUDPanel);
            }
            Character player = characterFactory.GeneratePlayerCharacter("Dummy");
            playerController.BindCharacter(player);
            playerController.BindCamera(playerCamera);
            Weapon pistol = weaponFactory.GenerateWeapon(0);
            playerController.EquipWeapon(pistol);
            // UIManager.instance.OpenUI(AvailableUI.GameHUDPanel);
        }
    }
}
