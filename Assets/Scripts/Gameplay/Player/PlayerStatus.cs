using UnityEngine.Events;
namespace Game.Gameplay
{
    public class PlayerStatus
    {
        public int ID { get; private set; }
        public int hp;
        public PlayerDieEvent dieEvent;

        public PlayerStatus(int id)
        {
            ID = id;
            dieEvent = new PlayerDieEvent();
        }
    }

    [System.Serializable]
    public class PlayerDieEvent : UnityEvent<PlayerStatus> { }
}