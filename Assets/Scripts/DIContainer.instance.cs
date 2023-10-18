using Game.Events;
using WillakeD.CommonPatterns;
using Game.Screens;
using Game.Saves;

namespace Game
{
    public partial class DIContainer : Singleton<DIContainer>
    {
        private void Awake()
        {
            SetUp();
        }

        protected void SetUp()
        {
            Register<EventManager>(() => new EventManager());
            Register<JsonRepository<GameStatisticsDataV1>>(() =>
            {
#if UNITY_EDITOR
                return new JsonRepository<GameStatisticsDataV1>(SaveMode.NonSerializedFile, "D:\\Projects\\INFOMAIGT\\Data");
#elif UNITY_WEBGL
                return new JsonRepository<GameStatisticsDataV1>(SaveMode.PlayerPrefs);
#else
                return new JsonRepository<GameStatisticsDataV1>(SaveMode.NonSerializedFile, Consts.GAME_FOLDER_PATH());
#endif
            });
            Register<ScreenAspectManager>(() => new ScreenAspectManager());
        }
    }
}