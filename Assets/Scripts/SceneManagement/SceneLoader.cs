using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using WillakeD.ScenePropertyDrawler;
using System;
using UnityEngine.Events;

namespace Game
{
    public class SceneLoader : MonoBehaviour
    {
        private string _currentScene = string.Empty;

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

            await LoadScene(sceneName);

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