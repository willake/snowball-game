using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace Game.UI
{
    public class TransitionPanel : UIPanel
    {
        public override AvailableUI Type { get => AvailableUI.TransitionPanel; }

        private CanvasGroup _canvasGroup;

        [Header("Settings")]
        public float fadeDurationInSeconds = 0.1f;
        public Ease fadeEase = Ease.Linear;
        public override WDButton[] GetSelectableButtons()
        {
            return new WDButton[0];
        }

        public override void PerformCancelAction()
        {

        }

        public override void Open()
        {
            GetCanvasGroup().alpha = 1f;
            gameObject.SetActive(true);
        }

        public override async UniTask OpenAsync()
        {
            GetCanvasGroup().alpha = 0f;
            gameObject.SetActive(true);
            await GetCanvasGroup()
                .DOFade(1f, fadeDurationInSeconds)
                .SetEase(fadeEase).SetUpdate(true).AsyncWaitForCompletion();
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }

        public override async UniTask CloseAsync()
        {
            GetCanvasGroup().alpha = 1f;
            await GetCanvasGroup()
                .DOFade(0f, fadeDurationInSeconds)
                .SetEase(fadeEase).SetUpdate(true).AsyncWaitForCompletion();
            gameObject.SetActive(false);
        }

        private CanvasGroup GetCanvasGroup()
        {
            if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
            return _canvasGroup;
        }
    }
}