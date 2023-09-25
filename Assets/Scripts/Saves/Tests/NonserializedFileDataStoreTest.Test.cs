using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Game.Saves.Tests
{
    public class NonserializedFileDataStoreTest
    {
        const string EXISTING_FILE_NAME = "test";
        const string EXISTING_FILE_CONTENT = "hello";
        const string EXISTING_FILE_NAME_2 = "test2";
        const string EXISTING_FILE_CONTENT_2 = "hello2";
        const string EXISTING_FILE_NAME_3 = "test3";
        const string EXISTING_FILE_CONTENT_3 = "hello3";
        NonSerializedFileDataStore sut;

        public string saveFolderPath
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
            sut = new NonSerializedFileDataStore(saveFolderPath);
            await sut.Save(EXISTING_FILE_NAME, EXISTING_FILE_CONTENT);
            await sut.Save(EXISTING_FILE_NAME_2, EXISTING_FILE_CONTENT_2);
            await sut.Save(EXISTING_FILE_NAME_3, EXISTING_FILE_CONTENT_3);
        });

        [UnityTearDown]
        public IEnumerator TearDown() => UniTask.ToCoroutine(async () =>
        {
            await sut.ClearAll();
        });

        [UnityTest]
        public IEnumerator A_NonSerializedFileDataStore_LoadWithExistingFile_WillGetExpectValue() =>
        UniTask.ToCoroutine(async () =>
        {
            string result = await sut.Load(EXISTING_FILE_NAME);

            Assert.AreEqual(EXISTING_FILE_CONTENT, result);
        });

        [UnityTest]
        public IEnumerator B_NonSerializedFileDataStore_SaveWithData_WillGetExpectValue() =>
        UniTask.ToCoroutine(async () =>
        {
            string fileName = "save_data_test";
            string expectData = "file saving manager test value";

            await sut.Save(fileName, expectData);
            string result = await sut.Load(fileName);

            Assert.AreEqual(expectData, result);
        });

        [UnityTest]
        public IEnumerator C_NonSerializedFileDataStore_DeleteExistingData_WillNotExist() =>
        UniTask.ToCoroutine(async () =>
        {
            await sut.Delete(EXISTING_FILE_NAME);

            Assert.IsFalse(File.Exists($"{saveFolderPath}/{EXISTING_FILE_NAME}.json"));
        });

        [UnityTest]
        public IEnumerator D_NonSerializedFileDataStore_Load_FileExists_ReturnNotEmptyString() =>
        UniTask.ToCoroutine(async () =>
        {
            string result = await sut.Load(EXISTING_FILE_NAME);

            Assert.AreNotEqual(result, "");
        });

        [UnityTest]
        public IEnumerator E_NonSerializedFileDataStore_Load_FileNotExists_ReturnDefaultEmptyString() =>
        UniTask.ToCoroutine(async () =>
        {
            string result = await sut.Load("FILE_NOT_EXITST");

            Assert.AreEqual(result, "");
        });

        [UnityTest]
        public IEnumerator F_NonSerializedFileDataStore_DataSeparation_ShouldNotEqual() =>
        UniTask.ToCoroutine(async () =>
        {
            var dataStore = new NonSerializedFileDataStore("NEW_FOLDER");
            string content = await dataStore.Load(EXISTING_FILE_NAME);
            Assert.AreNotEqual(content, EXISTING_FILE_CONTENT);
        });

        [UnityTest]
        public IEnumerator G_NonSerializedFileDataStore_Reentrance_ShouldNotThrow() =>
        UniTask.ToCoroutine(async () =>
        {
            var dataStore = new NonSerializedFileDataStore("NEW_FOLDER");

            var (result1, result2) = await UniTask.WhenAll(
                dataStore.Load(EXISTING_FILE_NAME), dataStore.Load(EXISTING_FILE_NAME));
            Assert.AreEqual(
                result1, result2);
        });

        [Test]
        public void H_NonSerializedFileDataStore_GetDataCount_WillGetExpectValue()
        {
            int count = sut.GetDataCount();

            Assert.AreEqual(3, count);
        }

        [UnityTest]
        public IEnumerator I_NonSerializedFileDataStore_LoadMultipleExistingFiles_WillGetExpectValue() =>
        UniTask.ToCoroutine(async () =>
        {
            string[] result = (await sut.LoadAll(0, 3));

            Assert.AreEqual(EXISTING_FILE_CONTENT, result[0]);
            Assert.AreEqual(EXISTING_FILE_CONTENT_2, result[1]);
            Assert.AreEqual(EXISTING_FILE_CONTENT_3, result[2]);
        });

        [UnityTest]
        public IEnumerator J_NonSerializedFileDataStore_LoadMultipleExistingFilesWithOffset_WillGetExpectValue() =>
        UniTask.ToCoroutine(async () =>
        {
            string[] result = (await sut.LoadAll(1, 2));

            Assert.AreEqual(EXISTING_FILE_CONTENT_2, result[0]);
            Assert.AreEqual(EXISTING_FILE_CONTENT_3, result[1]);
        });

        [UnityTest]
        public IEnumerator K_NonSerializedFileDataStore_LoadMultipleExistingFilesWithLargeOffset_WillGetNull() =>
        UniTask.ToCoroutine(async () =>
        {
            string[] result = (await sut.LoadAll(100, 2));

            Assert.AreEqual(null, result);
        });
    }
}