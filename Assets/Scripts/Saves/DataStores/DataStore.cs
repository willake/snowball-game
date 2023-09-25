using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Game.Saves
{
    public abstract class DataStore<T> : IDataStore<T>
    {
        internal readonly string saveFolderPath;

        protected DataStore(string path) => (saveFolderPath) = (path);

        public abstract int GetDataCount();

        public abstract UniTask<T> Load(string fileName);

        public abstract UniTask<T[]> LoadAll(int offset, int count, bool descending = false);

        public abstract UniTask Save(string fileName, T data);

        public abstract UniTask Delete(string fileName);

        public abstract UniTask ClearAll();
    }
}
