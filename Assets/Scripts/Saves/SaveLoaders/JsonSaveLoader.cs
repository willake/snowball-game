using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Saves
{
    public class JsonSaveLoader<T> : IDataSaveLoader<T> where T : IPersistentData
    {
        private readonly IDataStore<string> _dataStore;

        public JsonSaveLoader(SaveMode mode, string savePath)
        {
            string path = savePath + "/" + typeof(T);
            switch (mode)
            {
                case SaveMode.File:
                    _dataStore = new DataStoreAdaptor<byte[], string>(
                        new FileDataStore(path),
                        bytes => bytes?.AsString() ?? "",
                        str => str?.AsBytes() ?? Array.Empty<byte>()
                    );
                    return;
                case SaveMode.InMemory:
                    _dataStore = new InMemoryDataStore<string>("", path);
                    return;
                case SaveMode.NonSerializedFile:
                    _dataStore = new NonSerializedFileDataStore(path);
                    return;
                case SaveMode.PlayerPrefs:
                    _dataStore = new PlayerPrefsDataStore(path);
                    return;
            }

            throw new System.Exception("Unknow SavingMode" + mode);
        }

        private string ComposeFileName(object name)
        {
            return $"{name}.json";
        }

        public int GetDataCount()
        {
            return _dataStore.GetDataCount();
        }

        public async UniTask<T> Load(long key)
        {
            string jsonString = await _dataStore.Load(ComposeFileName(key)) ?? "";

            T saveData = JsonConvert.DeserializeObject<T>(jsonString, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
            });

            saveData?.BuildIndex();

            return saveData;
        }

        public async UniTask<T[]> LoadAll(int offset, int count, bool descending = false)
        {
            string[] jsonStrings = await _dataStore.LoadAll(offset, count, descending);

            if (jsonStrings == null) return Array.Empty<T>();

            T[] saveDataArray = new T[jsonStrings.Length];

            for (int i = 0; i < jsonStrings.Length; i++)
            {
                saveDataArray[i] = JsonConvert.DeserializeObject<T>(jsonStrings[i], new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                });
                saveDataArray[i]?.BuildIndex();
            }

            return saveDataArray;
        }

        public async UniTask Save(T entity)
        {
            string jsonString = JsonConvert.SerializeObject(entity);

            await _dataStore.Save(ComposeFileName(entity.SaveKey), jsonString);
        }

        public async UniTask DeleteSaveData(T entity)
        {
            await _dataStore.Delete(ComposeFileName(entity.SaveKey));
        }

        public async UniTask DeleteAllSaveData()
        {
            await _dataStore.ClearAll();
        }
    }

    public static class StringExtensions
    {
        public static byte[] AsBytes(this string str)
        {
            return System.Text.Encoding.Default.GetBytes(str);
        }

        public static string AsString(this byte[] arr)
        {
            return System.Text.Encoding.Default.GetString(arr);
        }
    }
}