using UnityEngine;

namespace Game.Gameplay
{
    public enum WeaponType
    {
        Melee,
        Gun
    }
    public abstract class Weapon : MonoBehaviour
    {
        public int id;
        public WeaponType weaponType;
        public bool canHold = false;

        public abstract void Fire(Vector3 direction);
    }
}