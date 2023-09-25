using Cysharp.Threading.Tasks;
using System;
namespace Game.Saves
{
    public sealed class DataStoreAdaptor<T, S> : IDataStore<S>
    {
        private IDataStore<T> store;
        private Func<T, S> convertFrom;
        private Func<S, T> convertTo;

        public DataStoreAdaptor(IDataStore<T> store, Func<T, S> convertFrom, Func<S, T> convertTo)
        {
            this.store = store;
            this.convertFrom = convertFrom;
            this.convertTo = convertTo;
        }

        public int GetDataCount()
        {
            return store.GetDataCount();
        }

        public async UniTask<S> Load(string fileName)
        {
            return convertFrom.Invoke(await store.Load(fileName));
        }

        public async UniTask<S[]> LoadAll(int offset, int count, bool descending = false)
        {
            T[] dataArray = await store.LoadAll(offset, count, descending);
            S[] convertedDataArray = new S[dataArray.Length];

            for (int i = 0; i < dataArray.Length; i++)
            {
                convertedDataArray[i] = convertFrom.Invoke(dataArray[i]);
            }

            return convertedDataArray;
        }

        public UniTask Save(string fileName, S data)
        {
            return store.Save(fileName, convertTo(data));
        }

        public UniTask Delete(string fileName)
        {
            return store.Delete(fileName);
        }

        public UniTask ClearAll()
        {
            return store.ClearAll();
        }
    }
}