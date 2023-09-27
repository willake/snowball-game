using UniRx;
using System;
using Cysharp.Threading.Tasks;
using Game.Events;
using UnityEngine;

namespace Game.UI
{
    public class MenuPanel : UIPanel
    {
        public override AvailableUI Type => AvailableUI.MenuPanel;

        [Header("References")]
        public WDTextButton btnPlayTest;
        public WDTextButton btnPlayLevel1;

        private void Start()
        {
            btnPlayTest
                .OnClickObservable
                .ObserveOnMainThread()
                .Subscribe(_ => PlayTest())
                .AddTo(this);

            btnPlayLevel1
                .OnClickObservable
                .ObserveOnMainThread()
                .Subscribe(_ => PlayLevel1())
                .AddTo(this);
        }

        public override WDButton[] GetSelectableButtons()
        {
            return new WDButton[] {
                btnPlayTest,
                btnPlayLevel1
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

        private void PlayTest()
        {
            GameManager.instance.levelToLoad = Gameplay.AvailableLevel.Test;
            GameManager.instance.SwitchScene(AvailableScene.MainGame);
        }

        private void PlayLevel1()
        {
            GameManager.instance.levelToLoad = Gameplay.AvailableLevel.Level1;
            GameManager.instance.SwitchScene(AvailableScene.MainGame);
        }

        private void OnDestroy()
        {
            btnPlayTest.StopAnimation();
            btnPlayLevel1.StopAnimation();
        }
    }
}