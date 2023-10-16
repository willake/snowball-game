using UniRx;
using System;
using Cysharp.Threading.Tasks;
using Game.Events;
using UnityEngine;
using Game.Gameplay;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameHUDPanel : UIPanel
    {
        public override AvailableUI Type => AvailableUI.GameHUDPanel;

        [Header("References")]
        public WDButton btnMenu;

        public ProgressBar healthBar;
        public AmmoBar ammoBar;

        private Character _bindedCharacter = null;

        private void Start()
        {
            btnMenu
                .OnClickObservable
                .ObserveOnMainThread()
                .Subscribe(_ => SwitchToMainGame())
                .AddTo(this);
        }

        public override WDButton[] GetSelectableButtons()
        {
            return new WDButton[] { btnMenu };
        }

        public override void PerformCancelAction()
        {

        }

        public override void Open()
        {
        }

        public override async UniTask OpenAsync()
        {
            await UniTask.CompletedTask;
        }

        public override void Close()
        {
        }

        public override async UniTask CloseAsync()
        {
            await UniTask.CompletedTask;
        }

        public void BindCharacter(Character character)
        {
            _bindedCharacter = character;
            character.healthUpdateEvent.AddListener(UpdateHealth);
            character.weaponHolder.ammoUpdateEvent.AddListener(UpdateAmmo);
            healthBar.SetProgress(character.health / character.MaxHealth);
            ammoBar.SetAmmo(10);
        }

        public void UnbindCharacter()
        {
            _bindedCharacter.healthUpdateEvent.RemoveListener(UpdateHealth);
            _bindedCharacter.weaponHolder.ammoUpdateEvent.RemoveListener(UpdateAmmo);
            _bindedCharacter = null;
        }

        private void UpdateHealth(float health, float maxHealth)
        {
            healthBar.SetProgress(health / maxHealth, 0.4f);
        }

        private void UpdateAmmo(int ammo)
        {
            ammoBar.SetAmmo(ammo);
        }

        private void SwitchToMainGame()
        {
            GameManager.instance.SwitchScene(AvailableScene.Menu);
        }

        private void OnDestroy()
        {
            btnMenu.StopAnimation();
            UnbindCharacter();
        }
    }
}