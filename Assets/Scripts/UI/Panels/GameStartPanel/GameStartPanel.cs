using UniRx;
using System;
using Cysharp.Threading.Tasks;
using Game.Events;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace Game.UI
{
    public class GameStartPanel : UIPanel
    {
        public override AvailableUI Type { get => AvailableUI.GameStartPanel; }

        [Title("References")]
        public CanvasGroup canvasGroup;
        public WDText title;

        [Title("Settings")]
        public float fadeDuration = 0.2f;
        public Ease fadeEase = Ease.InSine;

        private void Start()
        {
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

        public async UniTask ShowText(string text, float duration, Ease ease = Ease.Linear)
        {
            RectTransform titleRect = title.GetComponent<RectTransform>();
            Vector2 original = titleRect.anchoredPosition;
            title.TextMesh.alpha = 0;
            title.text = text;
            titleRect.anchoredPosition = new Vector2(original.x, original.y - 200);
            Sequence sequence = DOTween.Sequence();
            sequence.Append(titleRect.DOAnchorPosY(original.y, duration));
            sequence.Join(title.TextMesh.DOFade(1, duration));

            await sequence.SetUpdate(true).SetEase(ease).AsyncWaitForCompletion();
        }

        public override async UniTask OpenAsync()
        {
            gameObject.SetActive(true);

            canvasGroup.alpha = 0;

            await canvasGroup
                .DOFade(1, fadeDuration)
                .SetEase(fadeEase).SetUpdate(true).AsyncWaitForCompletion();
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }

        public override async UniTask CloseAsync()
        {
            canvasGroup.alpha = 1;
            await canvasGroup
                .DOFade(0, fadeDuration)
                .SetEase(fadeEase).SetUpdate(true).AsyncWaitForCompletion();
            gameObject.SetActive(false);
        }
    }
}