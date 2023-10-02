using UnityEngine.Events;
namespace Game.Gameplay
{
    public class CharacterStatus
    {
        public int health;
        public int ammo;
        public int maxAmmo;
        public HealthUpdateEvent healthUpdateEvent;
        public AmmoUpdateEvent ammoUpdateEvent;
        public DieEvent dieEvent;

        public CharacterStatus(int id)
        {
            health = 100;
            ammo = 0;
            maxAmmo = 0;
            healthUpdateEvent = new HealthUpdateEvent();
            ammoUpdateEvent = new AmmoUpdateEvent();
            dieEvent = new DieEvent();
        }

        [System.Serializable]
        public class HealthUpdateEvent : UnityEvent<int> { }
        [System.Serializable]
        // ammo / maxAmmo
        public class AmmoUpdateEvent : UnityEvent<int, int> { }
        [System.Serializable]
        public class DieEvent : UnityEvent<CharacterStatus> { }
    }
}