namespace Game.Saves.Tests
{
    [System.Serializable]
    public class TestEntity : IEntity<TestEntity>
    {
        public int id;
        public int testValue;

        public long SaveKey
        {
            get
            {
                return id;
            }
        }

        public TestEntity() { }

        public TestEntity(int id)
        {
            this.id = id;
        }

        public void Update(TestEntity data)
        {
            this.testValue = data.testValue;
        }

        public void BuildIndex()
        {

        }
    }
}