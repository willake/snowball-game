using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Cysharp.Threading.Tasks;
using System.Linq;

namespace Game.Saves
{
    public class NonSerializedFileDataStore : DataStore<string>
    {
        public NonSerializedFileDataStore(string path) : base(path) { }

        private string ComposePath(string fileName)
        {
            return $"{saveFolderPath}/{fileName}";
        }

        public override int GetDataCount()
        {
            if (Directory.Exists(saveFolderPath) == false)
            {
                return 0;
            }

            string[] fileNames = Directory.GetFiles(saveFolderPath);

            return fileNames.Length;
        }

        public override UniTask<string> Load(string fileName)
        {
            string path = ComposePath(fileName);
            if (File.Exists(path) == false)
            {
                return UniTask.FromResult(string.Empty);
            }

            return UniTask.FromResult(File.ReadAllText(path));
        }

        public override UniTask<string[]> LoadAll(int offset, int count, bool descending = false)
        {
            if (Directory.Exists(saveFolderPath) == false)
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

            string[] fileNames = Directory.GetFiles(saveFolderPath);
            if (descending)
            {
                fileNames = fileNames.OrderByDescending(x => x).ToArray();
            }
            //fileNames.OrderBy(name => name);
            string[] dataArray = new string[count];

            for (int i = offset, id = 0; i < offset + count; i++, id++)
            {
                dataArray[id] = File.ReadAllText(fileNames[i]);
            }

            return UniTask.FromResult(dataArray);
        }

        public override UniTask Save(string fileName, string data)
        {
            if (Directory.Exists(saveFolderPath) == false)
            {
                Directory.CreateDirectory(saveFolderPath);
            }

            File.WriteAllText(ComposePath(fileName), data);

            return UniTask.RunOnThreadPool(() => { });
        }

        public override UniTask Delete(string fileName)
        {
            string saveFilePath = $"{saveFolderPath}/{fileName}";

            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
            }

            return UniTask.RunOnThreadPool(() => { });
        }

        public override UniTask ClearAll()
        {
            if (Directory.Exists(saveFolderPath))
            {
                Directory.Delete(saveFolderPath, true);
            }

            return UniTask.RunOnThreadPool(() => { });
        }
    }
}