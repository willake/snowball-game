using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Audios;
using Game.Gameplay;
using Game.Saves;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace Game.UI
{
    public class EndGamePanel : UIPanel
    {
        public override AvailableUI Type => AvailableUI.EndGamePanel;

        [Header("References")]
        public EndGameTitle title;
        public WDTextButton btnMenu;
        public WDTextButton btnExport;
        public WDText txtPlayTime;
        public WDText txtDeathCount;
        public WDText txtDamagedCount;
        public WDText txtReloadCount;
        public WDText txtAverageEnergy;
        public WDText txtTotalEnemies;
        public WDText txtKilledEnemies;
        public WDText txtSnowballThrown;
        public WDText txtCriticalCharge;
        public WDText txtAvergeDistance;

        private CanvasGroup _canvasGroup;

        [Header("Fade animation settings")]
        public float fadeDuration = 0.2f;
        public Ease fadeEase = Ease.InSine;
#if UNITY_WEBGL && !UNITY_EDITOR
        //
        // WebGL
        //
        [DllImport("__Internal")]
        private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);
#endif

        private GameStatisticsDataV1 _data;
        private void Start()
        {
            btnMenu
                .OnClickObservable
                .ObserveOnMainThread()
                .Subscribe(_ => GoMainMenu())
                .AddTo(this);

#if UNITY_WEBGL && !UNITY_EDITOR
            btnExport.gameObject.SetActive(true);
            btnExport
                .OnClickObservable
                .ObserveOnMainThread()
                .Subscribe(_ =>
                {
                    string data = JsonConvert.SerializeObject(_data);
                    var bytes = Encoding.UTF8.GetBytes(data);
                    DownloadFile(gameObject.name, "OnFileDownload", $"{_data.id}.txt", bytes, bytes.Length);
                })
                .AddTo(this);
#else
            btnExport.gameObject.SetActive(true);
#endif
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

        public void UpdateStatisticsInformation(GameStatisticsDataV1 data)
        {
            _data = data;
            txtPlayTime.text = ParseTimeText(data.finishTime);
            txtDeathCount.text = data.deathCount.ToString();
            txtDamagedCount.text = data.damagedCount.ToString();
            txtReloadCount.text = data.reloadCount.ToString();
            txtTotalEnemies.text = data.totalEnemyCount.ToString();
            txtKilledEnemies.text = data.killedEnemyCount.ToString();
            txtSnowballThrown.text = data.thrownBalls.Count.ToString();
            txtCriticalCharge.text = data.thrownBalls.Where(x => x.isCritical).Count().ToString();

            float totalEnergy = 0f;
            data.thrownBalls.ForEach(x => totalEnergy += x.energy);
            txtAverageEnergy.text = (totalEnergy / data.thrownBalls.Count).ToString("0.00");

            float totalDistance = 0f;
            List<GameStatisticsDataV1.ThrownBall> ballsKilledEnemies =
                data.thrownBalls.Where(x => x.isKillEnemy).ToList();
            ballsKilledEnemies.ForEach(x => totalDistance += x.hitDistance);
            txtAvergeDistance.text = (totalDistance / ballsKilledEnemies.Count).ToString("0.00");
        }

        private string ParseTimeText(long time)
        {
            long minutes = time / 60;
            long seconds = time % 60;
            string minutesText = minutes > 9 ? $"{minutes}" : $"0{minutes}";
            string secondsText = seconds > 9 ? $"{seconds}" : $"0{seconds}";
            return $"{minutesText}:{secondsText}";
        }


        private void GoMainMenu()
        {
            MainGameScene.instance.NavigateToMenu(true).Forget();
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