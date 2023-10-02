using UniRx;
using System;
using Cysharp.Threading.Tasks;
using Game.Events;
using UnityEngine;
using Unity.Plastic.Antlr3.Runtime.Tree;

namespace Game.UI
{
    public class MenuPanel : UIPanel
    {
        public override AvailableUI Type => AvailableUI.MenuPanel;

        [Header("References")]
        public WDTextButton btnPlay;
        public WDTextButton btnSettings;

        private void Start()
        {
            btnPlay
                .OnClickObservable
                .ObserveOnMainThread()
                .Subscribe(_ => GoLevelSelect())
                .AddTo(this);

            btnSettings
                .OnClickObservable
                .ObserveOnMainThread()
                .Subscribe(_ => { })
                .AddTo(this);
        }

        public override WDButton[] GetSelectableButtons()
        {
            return new WDButton[] {
                btnPlay,
                btnSettings
            };
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
            gameObject.SetActive(true);
            await UniTask.CompletedTask;
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }

        public override async UniTask CloseAsync()
        {
            gameObject.SetActive(false);
            await UniTask.CompletedTask;
        }

        private void GoLevelSelect()
        {
            UIManager.instance.OpenUI(AvailableUI.LevelSelectPanel);
        }
    }
}