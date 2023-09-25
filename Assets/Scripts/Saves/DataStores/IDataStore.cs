using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Game.Saves
{
    public interface IDataStore<T>
    {
        int GetDataCount();
        UniTask<T> Load(string fileName);

        UniTask<T[]> LoadAll(int offset, int count, bool descending = false);

        UniTask Save(string fileName, T data);

        UniTask Delete(string fileName);

        UniTask ClearAll();
    }
}