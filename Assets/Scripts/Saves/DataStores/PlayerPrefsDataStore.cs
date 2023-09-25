using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Cysharp.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;

namespace Game.Saves
{
    public class PlayerPrefsDataStore : DataStore<string>
    {
        private string _keys_key;
        private HashSet<string> _keys;
        public PlayerPrefsDataStore(string path) : base(path)
        {
            _keys_key = path;

            string keysJson = PlayerPrefs.GetString(_keys_key);
            string[] keys = JsonConvert.DeserializeObject<string[]>(keysJson, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
            });

            if (keys == null)
            {
                _keys = new HashSet<string>();
            }
            else
            {
                _keys = new HashSet<string>(keys);
            }

        }

        private string ComposePath(string fileName)
        {
            return $"{saveFolderPath}/{fileName}";
        }

        public override int GetDataCount()
        {
            return _keys.Count;
        }

        public override UniTask<string> Load(string fileName)
        {
            string path = ComposePath(fileName);
            if (_keys.Contains(path) == false)
            {
                return UniTask.FromResult(string.Empty);
            }

            return UniTask.FromResult(PlayerPrefs.GetString(path, string.Empty));
        }

        public override UniTask<string[]> LoadAll(int offset, int count, bool descending = false)
        {
            if (_keys.Count == 0)
            {
                return UniTask.FromResult(default(string[]));
            }

            int totalCount = GetDataCount();

            if (offset >= totalCount)
            {
                return UniTask.FromResult(default(string[]));
            }

            if (offset + count > totalCount)
            {
                count = totalCount - offset;
            }

            string[] fileNames = _keys.OrderBy(x => x).ToArray();
            if (descending)
            {
                fileNames = _keys.OrderByDescending(x => x).ToArray();
            }
            //fileNames.OrderBy(name => name);
            string[] dataArray = new string[count];

            for (int i = offset, id = 0; i < offset + count; i++, id++)
            {
                dataArray[id] = PlayerPrefs.GetString(fileNames[i], string.Empty);
            }

            return UniTask.FromResult(dataArray);
        }

        public override UniTask Save(string fileName, string data)
        {
            PlayerPrefs.SetString(ComposePath(fileName), data);

            _keys.Add(ComposePath(fileName));

            string keysJson = JsonConvert.SerializeObject(_keys.ToArray());
            PlayerPrefs.SetString(_keys_key, keysJson);
            PlayerPrefs.Save();

            return UniTask.RunOnThreadPool(() => { });
        }

        public override UniTask Delete(string fileName)
        {
            PlayerPrefs.DeleteKey(ComposePath(fileName));

            _keys.Remove(ComposePath(fileName));

            string keysJson = JsonConvert.SerializeObject(_keys.ToArray());
            PlayerPrefs.SetString(_keys_key, keysJson);
            PlayerPrefs.Save();

            return UniTask.RunOnThreadPool(() => { });
        }

        public override UniTask ClearAll()
        {
            string[] keys = _keys.ToArray();

            for (int i = 0; i < keys.Length; i++)
            {
                PlayerPrefs.DeleteKey(keys[i]);
            }

            _keys = new HashSet<string>();

            PlayerPrefs.DeleteKey(_keys_key);
            PlayerPrefs.Save();

            return UniTask.RunOnThreadPool(() => { });
        }
    }
}