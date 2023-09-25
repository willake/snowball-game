using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Game.Saves.Tests
{
    public class FileDataStoreTest
    {
        const string EXISTING_FILE_NAME = "test";
        const string EXISTING_FILE_CONTENT = "hello";
        const string EXISTING_FILE_NAME_2 = "test2";
        const string EXISTING_FILE_CONTENT_2 = "hello2";
        const string EXISTING_FILE_NAME_3 = "test3";
        const string EXISTING_FILE_CONTENT_3 = "hello3";
        FileDataStore sut;

        private static string SaveFolderPath
        {
#if UNITY_EDITOR
            get => Application.streamingAssetsPath + "FOLDER";
#else
            get => Application.persistentDataPath + "FOLDER";
#endif
        }

        [UnitySetUp]
        public IEnumerator SetUp() => UniTask.ToCoroutine(async () =>
        {
            sut = new FileDataStore(SaveFolderPath);
            await sut.Save(EXISTING_FILE_NAME, EXISTING_FILE_CONTENT.AsBytes());
            await sut.Save(EXISTING_FILE_NAME_2, EXISTING_FILE_CONTENT_2.AsBytes());
            await sut.Save(EXISTING_FILE_NAME_3, EXISTING_FILE_CONTENT_3.AsBytes());
        });

        [UnityTearDown]
        public IEnumerator TearDown() => UniTask.ToCoroutine(async () =>
        {
            await sut.ClearAll();
        });

        [UnityTest]
        public IEnumerator A_FileDataStore_LoadWithExistingFile_WillGetExpectValue() =>
        UniTask.ToCoroutine(async () =>
        {
            string result = (await sut.Load(EXISTING_FILE_NAME)).AsString();

            Assert.AreEqual(EXISTING_FILE_CONTENT, result);
        });

        [UnityTest]
        public IEnumerator B_FileDataStore_SaveWithData_WillGetExpectValue() =>
        UniTask.ToCoroutine(async () =>
        {
            string fileName = "save_data_test";
            string expectData = "file saving manager test value";

            await sut.Save(fileName, expectData.AsBytes());
            string result = (await sut.Load(fileName)).AsString();

            Assert.AreEqual(expectData, result);
        });

        [UnityTest]
        public IEnumerator C_FileDataStore_DeleteExistingData_WillNotExist() =>
        UniTask.ToCoroutine(async () =>
        {
            await sut.Delete(EXISTING_FILE_NAME);

            Assert.IsFalse(File.Exists($"{SaveFolderPath}/{EXISTING_FILE_NAME}.json"));
        });

        [UnityTest]
        public IEnumerator D_FileDataStore_Load_FileExists_ReturnNotNull() =>
        UniTask.ToCoroutine(async () =>
        {
            byte[] result = await sut.Load(EXISTING_FILE_NAME);

            Assert.AreNotEqual(result, default(byte[]));
        });

        [UnityTest]
        public IEnumerator E_FileDataStore_Load_FileNotExists_ReturnDefaultEmptyByteArray() =>
        UniTask.ToCoroutine(async () =>
        {
            byte[] result = await sut.Load("FILE_NOT_EXITST");

            Assert.AreEqual(result, default(byte[]));
        });

        [UnityTest]
        public IEnumerator F_FileDataStore_DataSeparation_ShouldNotEqual() =>
        UniTask.ToCoroutine(async () =>
        {
            var dataStore = new FileDataStore("NEW_FOLDER");
            byte[] content = await dataStore.Load(EXISTING_FILE_NAME);
            Assert.AreNotEqual(content, EXISTING_FILE_CONTENT);
        });

        [UnityTest]
        public IEnumerator G_FileDataStore_Reentrance_ShouldNotThrow() =>
        UniTask.ToCoroutine(async () =>
        {
            var dataStore = new FileDataStore("NEW_FOLDER");

            var (result1, result2) = await UniTask.WhenAll(
                dataStore.Load(EXISTING_FILE_NAME), dataStore.Load(EXISTING_FILE_NAME));
            Assert.AreEqual(
                result1, result2);
        });

        [Test]
        public void H_FileDataStore_GetDataCount_WillGetExpectValue()
        {
            int count = sut.GetDataCount();

            Assert.AreEqual(3, count);
        }

        [UnityTest]
        public IEnumerator I_FileDataStore_LoadMultipleExistingFiles_WillGetExpectValue() =>
        UniTask.ToCoroutine(async () =>
        {
            byte[][] result = (await sut.LoadAll(0, 3));

            Assert.AreEqual(EXISTING_FILE_CONTENT, result[0].AsString());
            Assert.AreEqual(EXISTING_FILE_CONTENT_2, result[1].AsString());
            Assert.AreEqual(EXISTING_FILE_CONTENT_3, result[2].AsString());
        });

        [UnityTest]
        public IEnumerator J_FileDataStore_LoadMultipleExistingFilesWithOffset_WillGetExpectValue() =>
        UniTask.ToCoroutine(async () =>
        {
            byte[][] result = (await sut.LoadAll(1, 2));

            Assert.AreEqual(EXISTING_FILE_CONTENT_2, result[0].AsString());
            Assert.AreEqual(EXISTING_FILE_CONTENT_3, result[1].AsString());
        });

        [UnityTest]
        public IEnumerator K_FileDataStore_LoadMultipleExistingFilesWithLargeOffset_WillGetNull() =>
        UniTask.ToCoroutine(async () =>
        {
            byte[][] result = (await sut.LoadAll(100, 2));

            Assert.AreEqual(null, result);
        });
    }
}