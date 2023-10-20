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
    public class ModalPanel : UIPanel
    {
        public override AvailableUI Type { get => AvailableUI.ModalPanel; }

        [Title("References")]
        public CanvasGroup canvasGroup;
        public RectTransform panel;
        public WDButton btnConfirm;
        public WDText txtContent;

        [Title("Settings")]
        public float panelFadePosY = 500;
        public float fadeDuration = 0.2f;
        public Ease fadeEase = Ease.InSine;

        private void Start()
        {
            btnConfirm
                .OnClickObservable
                .ObserveOnMainThread()
                .Subscribe(_ =>
                {
                    UIManager.instance.PrevAsync().Forget();
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
        }

        public override async UniTask OpenAsync()
        {
            canvasGroup.alpha = 0;
            panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, panelFadePosY);
            gameObject.SetActive(true);
            Sequence sequence = DOTween.Sequence();
            sequence.Append(canvasGroup
                .DOFade(1, fadeDuration));
            sequence.Join(panel.DOAnchorPosY(0, fadeDuration));
            await sequence.SetEase(fadeEase).SetUpdate(true).AsyncWaitForCompletion();
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }

        public override async UniTask CloseAsync()
        {
            canvasGroup.alpha = 1;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(canvasGroup
                .DOFade(0, fadeDuration));
            sequence.Join(panel.DOAnchorPosY(panelFadePosY, fadeDuration));
            await sequence.SetEase(fadeEase).SetUpdate(true).AsyncWaitForCompletion();
            gameObject.SetActive(false);
        }

        public void SetContent(string content)
        {
            txtContent.text = content;
        }
    }
}