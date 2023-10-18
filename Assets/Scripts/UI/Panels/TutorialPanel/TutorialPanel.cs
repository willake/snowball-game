using UniRx;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.UI;
using Game.Screens;
using Game.Audios;

namespace Game.UI
{
    public class TutorialPanel : UIPanel
    {
        public override AvailableUI Type { get => AvailableUI.TutorialPanel; }

        [Title("References")]
        public WDTextButton btnBack;

        [Header("Settings")]
        public float fadeDuration = 0.2f;
        public Ease fadeEase = Ease.OutSine;

        private void Start()
        {
            btnBack
                .OnClickObservable
                .ObserveOnMainThread()
                .Subscribe(_ =>
                {
                    UIManager.instance.Prev();
                    UIManager.instance.OpenUI(AvailableUI.MenuPanel);
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
            gameObject.SetActive(true);
        }
        public override async UniTask OpenAsync()
        {
            Open();
            await UniTask.CompletedTask;
        }
        public override void Close()
        {
            gameObject.SetActive(false);
        }
        public override async UniTask CloseAsync()
        {
            Close();
            await UniTask.CompletedTask;
        }
    }
}