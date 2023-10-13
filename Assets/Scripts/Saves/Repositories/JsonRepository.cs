using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Game.Saves
{
    public class JsonRepository<T> : IRepository<T> where T : IEntity<T>
    {
        public JsonRepository()
        {
#if UNITY_EDITOR
            _saveLoader = new JsonSaveLoader<T>(SaveMode.InMemory, Consts.PERSISTANT_DATA_PATH);
#else
            _saveLoader = new JsonSaveLoader<T>(SaveMode.File, Consts.PERSISTANT_DATA_PATH);
#endif
        }

        public JsonRepository(SaveMode mode)
        {
            _saveLoader = new JsonSaveLoader<T>(mode, Consts.PERSISTANT_DATA_PATH);
        }


        public JsonRepository(SaveMode mode, string path)
        {
            _saveLoader = new JsonSaveLoader<T>(mode, path);
        }

        private readonly IDataSaveLoader<T> _saveLoader;

        public int GetDataCount()
        {
            return _saveLoader.GetDataCount();
        }

        public async UniTask Delete(T entity)
        {
            await _saveLoader.DeleteSaveData(entity);
        }

        public async UniTask<T> Get(long key)
        {
            return await _saveLoader.Load(key);
        }

        public async UniTask<T[]> GetAll(int offset, int count, bool descending = false)
        {
            return await _saveLoader.LoadAll(offset, count, descending);
        }

        public async UniTask Insert(T entity)
        {
            await _saveLoader.Save(entity);
        }

        public async UniTask Update(T entity)
        {
            T saveData = await _saveLoader.Load(entity.SaveKey);

            if (saveData == null)
            {
                await Insert(entity);
            }
            else
            {
                saveData.Update(entity);
                await _saveLoader.Save(saveData);
            }
        }

        public async UniTask Clear()
        {
            await _saveLoader.DeleteAllSaveData();
        }
    }
}