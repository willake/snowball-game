using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Game.Saves
{
    public interface IRepository<T> where T : IEntity<T>
    {
        int GetDataCount();
        UniTask<T> Get(long key);
        UniTask<T[]> GetAll(int offset, int count, bool descending = false);
        UniTask Insert(T entity);
        UniTask Update(T entity);
        UniTask Delete(T entity);
        UniTask Clear();
    }
}