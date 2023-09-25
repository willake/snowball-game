using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Game.Saves.Tests
{
    public class JsonRepositoryTest
    {
        const int EXIST_SAVE_DATA_ID = 1;
        const int EXIST_SAVE_DATA_VALUE = 24601;
        TestEntity existingData;
        JsonRepository<TestEntity> _repository;

        [UnitySetUp]
        public IEnumerator SetUp() =>
        UniTask.ToCoroutine(async () =>
        {
            _repository = new JsonRepository<TestEntity>(SaveMode.InMemory);

            existingData = new TestEntity(EXIST_SAVE_DATA_ID);

            existingData.testValue = EXIST_SAVE_DATA_VALUE;

            await _repository.Insert(existingData);
        });

        [UnityTearDown]
        public IEnumerator TearDown() =>
        UniTask.ToCoroutine(async () =>
        {
            await _repository.Clear();
        });

        [UnityTest]
        public IEnumerator A_JsonRepository_InsertWithNormalData_WillSuccess() =>
        UniTask.ToCoroutine(async () =>
        {
            await _repository.Clear();

            TestEntity newData = new TestEntity(2);
            newData.testValue = 10;

            await _repository.Insert(newData);

            TestEntity result = await _repository.Get(newData.SaveKey);

            Assert.AreEqual(newData.testValue, result.testValue);
        });

        [UnityTest]
        public IEnumerator B_JsonRepository_GetExistingData_WillMatchExpectValue() =>
        UniTask.ToCoroutine(async () =>
        {
            TestEntity result = await _repository.Get(EXIST_SAVE_DATA_ID);

            Assert.AreEqual(EXIST_SAVE_DATA_VALUE, result.testValue);
        });

        [UnityTest]
        public IEnumerator C_JsonRepository_GetNotExistingData_WillGetNull() =>
        UniTask.ToCoroutine(async () =>
        {
            await _repository.Clear();

            TestEntity result = await _repository.Get(EXIST_SAVE_DATA_ID);

            Assert.IsNull(result);
        });

        [UnityTest]
        public IEnumerator D_JsonRepository_UpdateWithNormalData_WillGetSameValue() =>
        UniTask.ToCoroutine(async () =>
        {
            int expectedValue = 24601;

            TestEntity data = await _repository.Get(EXIST_SAVE_DATA_ID);

            data.testValue = expectedValue;

            await _repository.Update(data);

            TestEntity modified = await _repository.Get(EXIST_SAVE_DATA_ID);

            Assert.AreEqual(expectedValue, modified.testValue);
        });

        [UnityTest]
        public IEnumerator E_JsonRepository_DeleteExistingData_WillGetNullAfterDelete() =>
        UniTask.ToCoroutine(async () =>
        {
            await _repository.Delete(existingData);

            TestEntity result = await _repository.Get(EXIST_SAVE_DATA_ID);

            Assert.IsNull(result);
        });

        [UnityTest]
        public IEnumerator F_JsonRepository_ClearExistingData_WillGetNullAfterClear() =>
        UniTask.ToCoroutine(async () =>
        {
            await _repository.Clear();

            TestEntity result = await _repository.Get(EXIST_SAVE_DATA_ID);

            Assert.IsNull(result);
        });
    }
}