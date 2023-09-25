using System.Collections.Concurrent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Cysharp.Threading.Tasks;
using System.Linq;

namespace Game.Saves
{
    public class InMemoryDataStore<T> : DataStore<T>
    {
        private ConcurrentDictionary<string, T> _inMemoryDict = new ConcurrentDictionary<string, T>();

        private T defaultValue;

        public InMemoryDataStore(T defaultValue, string name) : base(name)
        {
            this.defaultValue = defaultValue;
        }

        public override int GetDataCount()
        {
            return _inMemoryDict.Count;
        }

        private string ComposePath(string name)
        {
            return $"{saveFolderPath}/{name}";
        }

        public override UniTask<T> Load(string fileName)
        {
            if (_inMemoryDict.TryGetValue(ComposePath(fileName), out T data))
            {
                return UniTask.FromResult<T>(data);
            }

            return UniTask.FromResult<T>(defaultValue);
        }

        public override UniTask<T[]> LoadAll(int offset, int count, bool descending = false)
        {
            int totalCount = GetDataCount();

            if (offset >= totalCount)
            {
                return UniTask.FromResult(default(T[]));
            }

            if (offset + count > totalCount)
            {
                count = totalCount - offset;
            }

            string[] fileNames = _inMemoryDict.Keys.OrderBy(x => x).ToArray();

            if (descending)
            {
                fileNames = fileNames.OrderByDescending(x => x).ToArray();
            }

            T[] dataArray = new T[count];

            for (int i = offset, id = 0; i < offset + count; i++, id++)
            {
                if (_inMemoryDict.TryGetValue(fileNames[i], out T data))
                {
                    dataArray[id] = data;
                }
                else
                {
                    dataArray[id] = default(T);
                }
            }

            return UniTask.FromResult(dataArray);
        }

        public override UniTask Save(string fileName, T data)
        {
            return UniTask.RunOnThreadPool(() =>
            {
                _inMemoryDict[ComposePath(fileName)] = data;
            });
        }

        public override UniTask Delete(string fileName)
        {
            return UniTask.RunOnThreadPool(() =>
            {
                _inMemoryDict.TryRemove(ComposePath(fileName), out T value);
            });
        }

        public override UniTask ClearAll()
        {
            return UniTask.RunOnThreadPool(() =>
            {
                _inMemoryDict.Clear();
            });
        }
    }
}