using UnityEngine;

namespace Game.Gameplay
{
    public enum WeaponType
    {
        Melee,
        Gun,
        Snowball
    }
    public abstract class Weapon : MonoBehaviour
    {
        public int id;
        public WeaponType weaponType;
        public bool canHold = false;

        public abstract void Attack(Vector3 direction);
    }
}