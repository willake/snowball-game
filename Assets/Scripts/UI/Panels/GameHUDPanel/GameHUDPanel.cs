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
        public ProgressBar healthBar;
        public AmmoBar ammoBar;
        public LifeBar lifeBar;
        public WDText txtTime;
        public ProgressBar chargeBar;
        private bool _isChargeBarOpen = false;

        private Controller _bindedController = null;

        public override WDButton[] GetSelectableButtons()
        {
            return new WDButton[] { };
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

        public void BindController(PlayerController controller)
        {
            chargeBar.gameObject.SetActive(false);
            _bindedController = controller;
            controller.lifesUpdateEvent.AddListener(UpdateLifes);
            controller.bindedCharacter.healthUpdateEvent.AddListener(UpdateHealth);
            controller.bindedCharacter.weaponHolder.ammoUpdateEvent.AddListener(UpdateAmmo);
            UpdateAmmo(10);
            UpdateLifes(controller.availableLifes);

            controller.bindedCharacter.weaponHolder.loadEvent.AddListener(() =>
            {
                Cursor.visible = false;
                chargeBar.SetProgress(0f);
                chargeBar.gameObject.SetActive(true);
                _isChargeBarOpen = true;
            });
            controller.bindedCharacter.weaponHolder.throwEvent.AddListener(() =>
            {
                Cursor.visible = true;
                chargeBar.gameObject.SetActive(false);
                _isChargeBarOpen = false;
            });
            controller.bindedCharacter.weaponHolder.energyUpdateEvent.AddListener(progress => chargeBar.SetProgress(progress));
            healthBar.SetProgress(
                controller.bindedCharacter.health / controller.bindedCharacter.MaxHealth);
        }

        private void Update()
        {
            if (_isChargeBarOpen)
            {
                GetChargeBarRect().transform.position = Input.mousePosition;
            }
        }

        private RectTransform _chargeBarRect;
        private RectTransform GetChargeBarRect()
        {
            if (_chargeBarRect == null) _chargeBarRect = chargeBar.GetComponent<RectTransform>();
            return _chargeBarRect;
        }

        public void UnbindCharacter()
        {
            _bindedController.bindedCharacter.healthUpdateEvent.RemoveListener(UpdateHealth);
            _bindedController.bindedCharacter.weaponHolder.ammoUpdateEvent.RemoveListener(UpdateAmmo);
            _bindedController = null;
        }

        private string ParseText(long time)
        {
            long minutes = time / 60;
            long seconds = time % 60;
            string minutesText = minutes > 9 ? $"{minutes}" : $"0{minutes}";
            string secondsText = seconds > 9 ? $"{seconds}" : $"0{seconds}";
            return $"{minutesText}:{secondsText}";
        }

        public void UpdateTimeText(long time)
        {
            txtTime.text = ParseText(time);
        }

        private void UpdateLifes(int lifes)
        {
            lifeBar.SetLifes(lifes);
        }

        private void UpdateHealth(float health, float maxHealth)
        {
            healthBar.SetProgress(health / maxHealth, 0.4f);
        }

        private void UpdateAmmo(int ammo)
        {
            ammoBar.SetAmmo(ammo);
        }

        private void OnDestroy()
        {
            UnbindCharacter();
        }
    }
}