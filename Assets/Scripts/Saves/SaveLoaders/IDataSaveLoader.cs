using Cysharp.Threading.Tasks;

namespace Game.Saves
{
    interface IDataSaveLoader<T> where T : IPersistentData
    {
        int GetDataCount();
        UniTask<T> Load(long key);
        UniTask<T[]> LoadAll(int offset, int count, bool descending = false);
        UniTask Save(T entity);
        UniTask DeleteSaveData(T entity);
        UniTask DeleteAllSaveData();
    }
}