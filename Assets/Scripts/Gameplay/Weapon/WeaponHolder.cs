using UnityEngine;

namespace Game.Gameplay
{
    public class WeaponHolder : MonoBehaviour
    {
        [Header("References")]
        public Transform socket;

        private Weapon _holdingWeapon;
        public Weapon HoldingWeapon { get => _holdingWeapon; }

        public void EquipWeapon(Weapon weapon)
        {
            this._holdingWeapon = weapon;
            weapon.transform.SetParent(socket);
            weapon.transform.position = socket.position;
        }

        public void DropWeapon()
        {
        }

        public void Attack(Vector3 direction)
        {
            HoldingWeapon.Attack(direction);
        }

        public void Reload()
        {

        }
    }
}
