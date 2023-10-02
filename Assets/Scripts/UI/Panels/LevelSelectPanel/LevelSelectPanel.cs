using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Game.UI
{
    public class LevelSelectPanel : UIPanel
    {
        public override AvailableUI Type => AvailableUI.LevelSelectPanel;

        [Header("References")]
        public WDTextButton btnTest;
        public WDTextButton btnLevel1;
        public WDTextButton btnLevel2;
        public WDTextButton btnLevel3;
        public WDTextButton btnBack;

        private void Start()
        {
            btnTest
                .OnClickObservable
                .ObserveOnMainThread()
                .Subscribe(_ => LoadLevel(0))
                .AddTo(this);
            btnLevel1
                .OnClickObservable
                .ObserveOnMainThread()
                .Subscribe(_ => LoadLevel(1))
                .AddTo(this);
            btnLevel2
                .OnClickObservable
                .ObserveOnMainThread()
                .Subscribe(_ => LoadLevel(2))
                .AddTo(this);
            btnLevel3
                .OnClickObservable
                .ObserveOnMainThread()
                .Subscribe(_ => LoadLevel(3))
                .AddTo(this);
            btnBack
                .OnClickObservable
                .ObserveOnMainThread()
                .Subscribe(_ => BackToMenu())
                .AddTo(this);
        }

        public override WDButton[] GetSelectableButtons()
        {
            return new WDButton[] {
                btnTest,
                btnLevel1,
                btnLevel2,
                btnLevel3,
                btnBack
            };
        }

        public override void PerformCancelAction()
        {

        }

        public override void Open()
        {
            OpenAsync().Forget();
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

        private void BackToMenu()
        {
            UIManager.instance.Prev();
        }

        private void LoadLevel(int level)
        {
            switch (level)
            {
                case 0:
                default:
                    GameManager.instance.levelToLoad = Gameplay.AvailableLevel.Test;
                    break;
                case 1:
                    GameManager.instance.levelToLoad = Gameplay.AvailableLevel.Level1;
                    break;
                case 2:
                    GameManager.instance.levelToLoad = Gameplay.AvailableLevel.Level2;
                    break;
                case 3:
                    GameManager.instance.levelToLoad = Gameplay.AvailableLevel.Level3;
                    break;
            }
            GameManager.instance.SwitchScene(AvailableScene.MainGame);
        }

        public enum EndGameState
        {
            Win,
            Lose
        }
    }
}