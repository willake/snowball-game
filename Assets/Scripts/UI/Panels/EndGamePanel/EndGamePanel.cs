using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Audios;

namespace Game.UI
{
    public class EndGamePanel : UIPanel
    {
        public override AvailableUI Type => AvailableUI.EndGamePanel;

        [Header("References")]
        public EndGameTitle title;
        public WDTextButton btnMenu;

        private CanvasGroup _canvasGroup;

        [Header("Fade animation settings")]
        public float fadeDuration = 0.2f;
        public Ease fadeEase = Ease.InSine;


        private void Start()
        {
            btnMenu
                .OnClickObservable
                .ObserveOnMainThread()
                .Subscribe(_ => GoMainMenu())
                .AddTo(this);
        }

        public override WDButton[] GetSelectableButtons()
        {
            return new WDButton[] {
                btnMenu
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
            GetCanvasGroup().alpha = 0;
            gameObject.SetActive(true);

            GetCanvasGroup().DOFade(1, fadeDuration).SetEase(fadeEase).SetUpdate(true);
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

        private void GoMainMenu()
        {
            GameManager.instance.SwitchScene(AvailableScene.Menu);
        }

        private void OnDestroy()
        {
            btnMenu.StopAnimation();
        }

        private CanvasGroup GetCanvasGroup()
        {
            if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();

            return _canvasGroup;
        }

        public void SetEndGameState(EndGameState state)
        {
            if (state == EndGameState.Win)
            {
                title.PlayWinAnimation();
                AudioManager.instance?.PlaySFX(
                    ResourceManager.instance.audioResources.gameplayAudios.levelWin.clip
                );
            }
            else
            {
                title.PlayLoseAnimation();
                AudioManager.instance?.PlaySFX(
                    ResourceManager.instance.audioResources.gameplayAudios.levelLose.clip
                );
            }
        }

        public enum EndGameState
        {
            Win,
            Lose
        }
    }
}