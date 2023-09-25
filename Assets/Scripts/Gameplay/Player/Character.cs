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
        public Transform weaponSocket;
        public Weapon weapon;
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
            this.weapon = weapon;
            this.weapon.transform.SetParent(weaponSocket);
            this.weapon.transform.position = weaponSocket.position;
        }

        public void DropWeapon()
        {
        }

        public void Aim(Vector3 direction)
        {
            float angle = (float)Math.Atan2(direction.x, direction.y);
            transform.rotation = Quaternion.Euler(
                new Vector3(0, angle * Mathf.Rad2Deg + 180, 0));
        }

        public void Attack()
        {
            this.weapon.Fire(transform.forward);
        }
    }
}