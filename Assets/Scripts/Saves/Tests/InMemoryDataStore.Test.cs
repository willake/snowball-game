using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Game.Saves.Tests
{
    public class InMemoryDataStoreTest
    {
        const string EXISTING_FILE_NAME = "0";
        const string EXISTING_FILE_CONTENT = "hello1";
        const string EXISTING_FILE_NAME_2 = "1";
        const string EXISTING_FILE_CONTENT_2 = "hello2";
        const string EXISTING_FILE_NAME_3 = "2";
        const string EXISTING_FILE_CONTENT_3 = "hello3";

        InMemoryDataStore<string> sut;

        [UnitySetUp]
        public IEnumerator SetUp() =>
        UniTask.ToCoroutine(async () =>
        {
            sut = new InMemoryDataStore<string>("", "FOLDER");
            await sut.Save(EXISTING_FILE_NAME, EXISTING_FILE_CONTENT);
            await sut.Save(EXISTING_FILE_NAME_2, EXISTING_FILE_CONTENT_2);
            await sut.Save(EXISTING_FILE_NAME_3, EXISTING_FILE_CONTENT_3);
        });

        [UnityTearDown]
        public IEnumerator TearDown() =>
        UniTask.ToCoroutine(async () =>
        {
            await sut.ClearAll();
        });

        [UnityTest]
        public IEnumerator A_InMemoryDataStore_LoadWithExistingFile_WillGetExpectValue() =>
        UniTask.ToCoroutine(async () =>
        {
            string result = await sut.Load(EXISTING_FILE_NAME);

            Assert.AreEqual(EXISTING_FILE_CONTENT, result);
        });

        [UnityTest]
        public IEnumerator B_InMemoryDataStore_SaveWithData_WillGetExpectValue() =>
        UniTask.ToCoroutine(async () =>
        {
            string fileName = "save_data_test";
            string expectData = "file saving manager test value";

            await sut.Save(fileName, expectData);
            string result = await sut.Load(fileName);

            Assert.AreEqual(expectData, result);
        });

        [UnityTest]
        public IEnumerator C_InMemoryDataStore_DeleteExistingData_WillNotExist() =>
        UniTask.ToCoroutine(async () =>
        {
            await sut.Delete(EXISTING_FILE_NAME);
            string result = await sut.Load(EXISTING_FILE_NAME);

            Assert.AreNotEqual(EXISTING_FILE_CONTENT, result);
        });

        [UnityTest]
        public IEnumerator D_InMemoryDataStore_DataSeparation_ShouldNotEqual() =>
        UniTask.ToCoroutine(async () =>
        {
            var dataStore = new InMemoryDataStore<string>("", "NEW_FOLDER");
            string content = await dataStore.Load(EXISTING_FILE_NAME);
            Assert.AreNotEqual(content, EXISTING_FILE_CONTENT);
        });

        [Test]
        public void E_InMemoryDataStore_GetDataCount_WillGetExpectValue()
        {
            int count = sut.GetDataCount();

            Assert.AreEqual(3, count);
        }

        [UnityTest]
        public IEnumerator F_InMemoryDataStore_LoadMultipleExistingFiles_WillGetExpectValue() =>
        UniTask.ToCoroutine(async () =>
        {
            string[] result = (await sut.LoadAll(0, 3));

            Assert.AreEqual(EXISTING_FILE_CONTENT, result[0]);
            Assert.AreEqual(EXISTING_FILE_CONTENT_2, result[1]);
            Assert.AreEqual(EXISTING_FILE_CONTENT_3, result[2]);
        });

        [UnityTest]
        public IEnumerator G_InMemoryDataStore_LoadMultipleExistingFilesWithOffset_WillGetExpectValue() =>
        UniTask.ToCoroutine(async () =>
        {
            string[] result = (await sut.LoadAll(1, 2));

            Assert.AreEqual(EXISTING_FILE_CONTENT_2, result[0]);
            Assert.AreEqual(EXISTING_FILE_CONTENT_3, result[1]);
        });

        [UnityTest]
        public IEnumerator H_InMemoryDataStore_LoadMultipleExistingFilesWithLargeOffset_WillGetNull() =>
        UniTask.ToCoroutine(async () =>
        {
            string[] result = (await sut.LoadAll(100, 2));

            Assert.AreEqual(null, result);
        });
    }
}