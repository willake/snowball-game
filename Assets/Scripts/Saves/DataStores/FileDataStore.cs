using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Cysharp.Threading.Tasks;
using System.Linq;

namespace Game.Saves
{
    public class FileDataStore : DataStore<byte[]>
    {
        public FileDataStore(string path) : base(path) { }

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

        public override UniTask<byte[]> Load(string fileName)
        {
            string path = ComposePath(fileName);
            if (File.Exists(path) == false)
            {
                return UniTask.FromResult(default(byte[]));
            }

            using (FileStream fs = new FileStream(
                    path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                int length = (int)fs.Length;
                byte[] allBytes = new byte[length];
                fs.Read(allBytes, 0, length);
                fs.Close();
                return UniTask.FromResult(allBytes);
            }
        }

        public override UniTask<byte[][]> LoadAll(int offset, int count, bool descending = false)
        {
            if (Directory.Exists(saveFolderPath) == false)
            {
                return UniTask.FromResult(default(byte[][]));
            }

            int totalCount = GetDataCount();

            if (offset >= totalCount)
            {
                return UniTask.FromResult(default(byte[][]));
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
            byte[][] dataArray = new byte[count][];

            for (int i = offset, id = 0; i < offset + count; i++, id++)
            {
                using (FileStream fs = new FileStream(
                    fileNames[i], FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    int length = (int)fs.Length;
                    byte[] allBytes = new byte[length];
                    fs.Read(allBytes, 0, length);
                    fs.Close();
                    dataArray[id] = allBytes;
                }
            }

            return UniTask.FromResult(dataArray);
        }

        public override UniTask Save(string fileName, byte[] data)
        {
            if (Directory.Exists(saveFolderPath) == false)
            {
                Directory.CreateDirectory(saveFolderPath);
            }

            File.WriteAllBytes(ComposePath(fileName), data);

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