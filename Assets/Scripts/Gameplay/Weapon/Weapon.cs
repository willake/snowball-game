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
        public Camp ownerCamp;

        public void SetOwnerCamp(Camp camp)
        {
            ownerCamp = camp;
        }

        public int GetOwnerCampLayer()
        {
            switch (ownerCamp)
            {
                case Camp.Player:
                default:
                    return LayerMask.NameToLayer("Player");
                case Camp.Enemy:
                    return LayerMask.NameToLayer("Enemy");
            }
        }
        public abstract bool Attack(Vector3 direction, float energy, bool isCritical);
        public abstract void Reload();
    }
}