using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Gameplay
{
    public class Character : MonoBehaviour
    {
        private Rigidbody _rigibody;

        public float acc = 5f;
        public float maxSpeed = 5f;
        public Vector3 facingDirection = Vector2.right;
        public Vector3 currentVelocity { get { return GetRigidbody().velocity; } }

        [Header("Weapons")]
        public WeaponHolder weaponHolder;

        private Rigidbody GetRigidbody()
        {
            if (_rigibody == null) _rigibody = GetComponent<Rigidbody>();

            return _rigibody;
        }

        public void Move(float horizontal, float vertical)
        {
            if (GetRigidbody().velocity.magnitude < maxSpeed)
            {
                GetRigidbody().AddForce(
                 new Vector3(horizontal, 0, vertical) * acc, ForceMode.Force);
            }
        }

        public void EquipWeapon(Weapon weapon)
        {
            weaponHolder.EquipWeapon(weapon);
        }

        public void DropWeapon()
        {
        }

        public void Reload()
        {
            weaponHolder.Reload();
        }

        public void Aim(Vector3 direction)
        {
            float angle = (float)Math.Atan2(direction.x, direction.y);
            transform.rotation = Quaternion.Euler(
                new Vector3(0, angle * Mathf.Rad2Deg + 180, 0));
            weaponHolder.UpdateAimDirection(transform.forward);
        }

        public void Hold()
        {
            weaponHolder.Hold();
        }

        public void Throw()
        {
            weaponHolder.Throw();
        }
    }
}