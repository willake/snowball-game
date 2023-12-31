using UniRx;
using System;
using Cysharp.Threading.Tasks;
using Game.Events;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using Game.Gameplay;

namespace Game.UI
{
    public class PausePanel : UIPanel
    {
        public override AvailableUI Type { get => AvailableUI.PausePanel; }

        [Title("References")]
        public CanvasGroup canvasGroup;
        public WDButton btnResume;
        public WDButton btnMenu;

        [Title("Settings")]
        public float fadeDuration = 0.2f;
        public Ease fadeEase = Ease.InSine;

        private void Start()
        {
            btnResume
                .OnClickObservable
                .ObserveOnMainThread()
                .Subscribe(async _ =>
                {
                    await UIManager.instance.PrevAsync();
                    GameManager.instance.ResumeGame();
                })
                .AddTo(this);

            btnMenu
                .OnClickObservable
                .ObserveOnMainThread()
                .Subscribe(_ =>
                {
                    GameManager.instance.ResumeGame();
                    MainGameScene.instance.NavigateToMenu(false).Forget();
                })
                .AddTo(this);
        }

        public override WDButton[] GetSelectableButtons()
        {
            return new WDButton[0];
        }

        public override void PerformCancelAction()
        {

        }

        public override void Open()
        {
            canvasGroup.alpha = 1;
            gameObject.SetActive(true);
            GameManager.instance.PauseGame();
        }

        public override async UniTask OpenAsync()
        {
            gameObject.SetActive(true);
            GameManager.instance.PauseGame();
            canvasGroup.alpha = 0;
            await canvasGroup
                .DOFade(1, fadeDuration)
                .SetEase(fadeEase).SetUpdate(true).AsyncWaitForCompletion();
        }

        public override void Close()
        {
            gameObject.SetActive(false);
            GameManager.instance.ResumeGame();
        }

        public override async UniTask CloseAsync()
        {
            canvasGroup.alpha = 1;
            await canvasGroup
                .DOFade(0, fadeDuration)
                .SetEase(fadeEase).SetUpdate(true).AsyncWaitForCompletion();
            gameObject.SetActive(false);
            GameManager.instance.ResumeGame();
        }
    }
}