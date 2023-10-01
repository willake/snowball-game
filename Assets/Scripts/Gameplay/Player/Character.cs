using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Game.Gameplay
{
    public class Character : MonoBehaviour
    {
        private NavMeshAgent _navMeshAgent;
        private Rigidbody _rigibody;

        [Header("Status")]
        public float health = 100;
        public HealthUpdateEvent healthUpdateEvent = new();
        public DieEvent dieEvent = new();
        private float _maxHealth = 100;

        [Header("Settings")]
        public float acc = 5f;
        public float maxSpeed = 5f;
        public Vector3 currentVelocity { get { return GetRigidbody().velocity; } }

        [Header("Weapons")]
        public WeaponHolder weaponHolder;

        private void Start()
        {
            _maxHealth = health;
        }

        private Rigidbody GetRigidbody()
        {
            if (_rigibody == null) _rigibody = GetComponent<Rigidbody>();

            return _rigibody;
        }

        public NavMeshAgent GetNavMeshAgent()
        {
            if (_navMeshAgent == null) _navMeshAgent = GetComponent<NavMeshAgent>();

            return _navMeshAgent;
        }

        public void Move(float horizontal, float vertical)
        {
            if (GetRigidbody().velocity.magnitude < maxSpeed)
            {
                GetRigidbody().AddForce(
                 new Vector3(horizontal, 0, vertical) * acc, ForceMode.Force);
            }
        }

        public void MoveTo(Vector3 position)
        {
            GetNavMeshAgent().SetDestination(position);
        }

        public void TakeDamage(float damage)
        {
            health -= damage;

            if (health < float.Epsilon)
            {
                health = 0f;
                dieEvent.Invoke();
            }

            healthUpdateEvent.Invoke(health, _maxHealth);
        }

        public void Reload()
        {
            weaponHolder.Reload();
        }

        public void Aim(Vector3 direction, bool useFoward = true)
        {
            float angle = (float)Math.Atan2(direction.x, direction.y);
            transform.rotation = Quaternion.Euler(
                new Vector3(0, angle * Mathf.Rad2Deg, 0));

            weaponHolder.UpdateAimDirection(useFoward ? transform.forward : direction);
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

        private void OnDestroy()
        {
            healthUpdateEvent.RemoveAllListeners();
            dieEvent.RemoveAllListeners();
        }

        public class HealthUpdateEvent : UnityEvent<float, float> { }
        public class DieEvent : UnityEvent { }
    }
}