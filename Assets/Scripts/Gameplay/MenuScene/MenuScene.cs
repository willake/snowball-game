using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.UI;

namespace Game.Gameplay
{
    public class MenuScene : GameScene<MenuScene>
    {
        private void Start()
        {
            UIManager.instance.OpenUI(AvailableUI.MenuPanel);
        }
    }
}
