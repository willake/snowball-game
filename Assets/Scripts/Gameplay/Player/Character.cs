using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Gameplay
{
    public class Character : MonoBehaviour
    {
        private Rigidbody _rigibody;

        [Header("Status")]
        public float health = 100;
        public HealthUpdateEvent healthUpdateEvent = new();
        public DieEvent dieEvent = new();

        [Header("Settings")]
        public float acc = 5f;
        public float maxSpeed = 5f;
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

        public void TakeDamage(float damage)
        {
            health -= damage;

            if (health < float.Epsilon)
            {
                health = 0f;
                dieEvent.Invoke();
            }

            healthUpdateEvent.Invoke(health);
        }

        public void Reload()
        {
            weaponHolder.Reload();
        }

        public void Aim(Vector3 direction)
        {
            float angle = (float)Math.Atan2(direction.x, direction.y);
            transform.rotation = Quaternion.Euler(
                new Vector3(0, angle * Mathf.Rad2Deg, 0));
            weaponHolder.UpdateAimDirection(transform.forward);
            Debug.DrawRay(transform.position, transform.forward, Color.green, 5);
        }

        public void Hold()
        {
            weaponHolder.Hold();
        }

        public void Throw()
        {
            weaponHolder.Throw();
        }

        public void ThrowWithoutCharging(float energy)
        {
            weaponHolder.ThrowWithoutCharging(energy);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.forward * 3);
        }

        public class HealthUpdateEvent : UnityEvent<float> { }
        public class DieEvent : UnityEvent { }
    }
}