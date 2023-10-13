using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using System;
using Game.Events;
using WillakeD.CommonPatterns;
using Game.UI;
using Game.Saves;
using Game.Gameplay;

namespace Game
{
    public class GameManager : Singleton<GameManager>
    {
        public const string IDENTITY = "GAME_MANAGER";
        private Lazy<EventManager> _eventManager = new Lazy<EventManager>(
            () => DIContainer.instance.GetObject<EventManager>(),
            true
        );
        protected EventManager EventManager { get => _eventManager.Value; }

        [Header("References")]
        public SceneLoader sceneLoader;
        public AvailableLevel levelToLoad = AvailableLevel.Test;

        private void Start()
        {
            sceneLoader
                .OnLoadSceneObservable
                .ObserveOnMainThread()
                .Subscribe(_ => UIManager.instance.CloseAllUI())
                .AddTo(this);

            SwitchScene(AvailableScene.Menu);
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
        }

        public void SwitchScene(AvailableScene scene)
        {
            EventManager.Publish(
                EventNames.loadScene,
                new Payload()
                {
                    args = new object[] { scene }
                }
            );

            sceneLoader
                .SwitchScene(scene)
                .Forget();
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        private void OnDestroy()
        {
            EventManager.ClearSubscriptions();
        }
    }
}