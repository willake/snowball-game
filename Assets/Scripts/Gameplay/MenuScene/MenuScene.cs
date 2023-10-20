using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.UI;

namespace Game.Gameplay
{
    public class MenuScene : GameScene<MenuScene>
    {
        private async void Start()
        {
            await UIManager.instance.OpenUIAsync(AvailableUI.MenuPanel);
            ModalPanel modalPanel =
                await UIManager.instance.OpenUIAsync(AvailableUI.ModalPanel) as ModalPanel;
            modalPanel.SetContent(@"By playing this game, you have confirm to share your play data with us. To Note that we do not collect any personal information.");
        }
    }
}
