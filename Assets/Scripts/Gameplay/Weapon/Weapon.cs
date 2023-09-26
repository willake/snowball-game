using UnityEngine;

namespace Game.Gameplay
{
    public enum WeaponType
    {
        Melee,
        Snowball
    }

    public abstract class Weapon : MonoBehaviour
    {
        public int id;
        public WeaponType weaponType;
        public bool canHold = false;

        public abstract void SetOwnerType(ControllerType type);
        public abstract int GetOwnerLayer();
        public abstract void Attack(Vector3 direction, float energy);
        public abstract void Reload();
    }
}