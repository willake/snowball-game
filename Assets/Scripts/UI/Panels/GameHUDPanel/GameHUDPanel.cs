using UniRx;
using System;
using Cysharp.Threading.Tasks;
using Game.Events;
using UnityEngine;
using Game.Gameplay;

namespace Game.UI
{
    public class GameHUDPanel : UIPanel
    {
        public override AvailableUI Type => AvailableUI.GameHUDPanel;

        [Header("References")]
        public WDButton btnMenu;

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
            character.weaponHolder.energyUpdateEvent.AddListener(UpdateEnergy);
            character.weaponHolder.holdEvent.AddListener(ShowChargeBar);
            character.weaponHolder.throwEvent.AddListener(CloseChargeBar);
        }

        public void UnbindCharacter()
        {
            _bindedCharacter.healthUpdateEvent.RemoveListener(UpdateHealth);
            _bindedCharacter.weaponHolder.ammoUpdateEvent.RemoveListener(UpdateAmmo);
            _bindedCharacter.weaponHolder.energyUpdateEvent.RemoveListener(UpdateEnergy);
            _bindedCharacter.weaponHolder.holdEvent.RemoveListener(ShowChargeBar);
            _bindedCharacter.weaponHolder.throwEvent.RemoveListener(CloseChargeBar);
            _bindedCharacter = null;
        }

        private void UpdateHealth(float health)
        {
            // TODO: update health UI
        }

        private void UpdateAmmo(int ammo)
        {
            // TODO: update ammo UI
        }

        private void UpdateEnergy(float energyInPercentage)
        {
            // TOD: update energy bar if it shows up
        }

        private void ShowChargeBar()
        {

        }

        private void CloseChargeBar()
        {

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