using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using WillakeD.ScenePropertyDrawler;
using System;
using UnityEngine.Events;
using Game.Gameplay;

namespace Game
{
    public class LevelManager : MonoBehaviour
    {
        private string _currentLevel = string.Empty;

        public OnLoadLevelEvent onLoadLevel = new OnLoadLevelEvent();
        public IObservable<string> OnLoadLevelObservable
        {
            get => onLoadLevel.AsObservable();
        }

        public OnLoadLevelEvent onLevelLoaded = new OnLoadLevelEvent();
        public IObservable<string> OnLevelLoadedObservable
        {
            get => onLevelLoaded.AsObservable();
        }

        public async UniTask LoadLevel(AvailableLevel level)
        {
            string levelName = GetLevelName(level);

            onLoadLevel.Invoke(levelName);

            await LoadLevel(levelName);

            onLevelLoaded.Invoke(levelName);
        }

        private string GetLevelName(AvailableLevel scene)
        {
            switch (scene)
            {
                case AvailableLevel.Test:
                default:
                    return ResourceManager.instance.levelResources.test.GetSceneNameByPath();
                case AvailableLevel.Level1:
                    return ResourceManager.instance.levelResources.level1.GetSceneNameByPath();
                case AvailableLevel.Level2:
                    return ResourceManager.instance.levelResources.level2.GetSceneNameByPath();
                case AvailableLevel.Level3:
                    return ResourceManager.instance.levelResources.level3.GetSceneNameByPath();
            }
        }

        private async UniTask LoadLevel(string levelName)
        {
            AsyncOperation loadSceneOperation =
                SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);

            await loadSceneOperation;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName));

            loadSceneOperation.allowSceneActivation = false;

            if (_currentLevel != string.Empty)
            {
                AsyncOperation unloadSceneOperation =
                    SceneManager.UnloadSceneAsync(_currentLevel);

                await unloadSceneOperation;
            }

            loadSceneOperation.allowSceneActivation = true;

            _currentLevel = levelName;
        }

        public class OnLoadLevelEvent : UnityEvent<string> { }
    }
}