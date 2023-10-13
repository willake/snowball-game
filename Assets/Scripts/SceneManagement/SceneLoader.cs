using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using WillakeD.ScenePropertyDrawler;
using System;
using UnityEngine.Events;
using Game.UI;
using System.Threading;

namespace Game
{
    public class SceneLoader : MonoBehaviour
    {
        [Header("References")]
        public Camera crossSceneCamera;
        public TransitionPanel transitionPanel;
        private string _currentScene = string.Empty;

        [Header("Settings")]
        public float transitionInSeconds = 0.5f;

        public OnLoadSceneEvent onLoadScene = new OnLoadSceneEvent();
        public IObservable<string> OnLoadSceneObservable
        {
            get => onLoadScene.AsObservable();
        }

        public OnLoadSceneEvent onSceneLoaded = new OnLoadSceneEvent();
        public IObservable<string> OnSceneLoadedObservable
        {
            get => onSceneLoaded.AsObservable();
        }

        public async UniTask SwitchScene(AvailableScene scene)
        {
            string sceneName = GetSceneName(scene);

            onLoadScene.Invoke(sceneName);
            await transitionPanel.OpenAsync();
            crossSceneCamera.enabled = true;

            await LoadScene(sceneName);

            await UniTask.Delay(TimeSpan.FromSeconds(transitionInSeconds));

            await transitionPanel.CloseAsync();
            crossSceneCamera.enabled = false;
            onSceneLoaded.Invoke(sceneName);
        }

        private string GetSceneName(AvailableScene scene)
        {
            switch (scene)
            {
                case AvailableScene.Menu:
                default:
                    return ResourceManager.instance.sceneResources.menu.GetSceneNameByPath();
                case AvailableScene.MainGame:
                    return ResourceManager.instance.sceneResources.game.GetSceneNameByPath();
            }
        }

        private async UniTask LoadScene(string sceneName)
        {
            AsyncOperation loadSceneOperation =
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            await loadSceneOperation;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

            loadSceneOperation.allowSceneActivation = false;

            if (_currentScene != string.Empty)
            {
                AsyncOperation unloadSceneOperation =
                    SceneManager.UnloadSceneAsync(_currentScene);

                await unloadSceneOperation;
            }

            loadSceneOperation.allowSceneActivation = true;

            _currentScene = sceneName;
        }

        public class OnLoadSceneEvent : UnityEvent<string> { }
    }
}