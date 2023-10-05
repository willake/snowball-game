using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;
using UnityEngine.Events;

namespace Game.Gameplay
{
    public class Character : MonoBehaviour
    {
        private CharacterAnimatior _characterAnimator;
        private NavMeshAgent _navMeshAgent;
        private Rigidbody _rigibody;

        [Header("Status")]
        public float health = 100;
        public HealthUpdateEvent healthUpdateEvent = new();
        public DieEvent dieEvent = new();
        public float MaxHealth { get; private set; }

        [Header("Settings")]
        public float acc = 5f;
        public float maxSpeed = 5f;
        public Vector3 currentVelocity { get { return GetRigidbody().velocity; } }

        [Header("Weapons")]
        public WeaponHolder weaponHolder;

        private bool _isAiming = false;

        private void Start()
        {
            MaxHealth = health;
            weaponHolder.throwEvent.AddListener(HandleThrowEvent);
        }

        public void SetIsAiming(bool isAiming)
        {
            _isAiming = isAiming;
        }

        public void Idle()
        {
            GetRigidbody().velocity = Vector3.zero;
            GetCharacterAnimatior()?.SetMoveSpeed(0);
        }

        public void Move(float horizontal, float vertical)
        {
            if (GetRigidbody().velocity.magnitude < maxSpeed)
            {
                GetRigidbody().AddForce(
                 new Vector3(horizontal, 0, vertical) * acc, ForceMode.Force);
                GetCharacterAnimatior()?.SetMoveSpeed(1);
            }

            if (_isAiming == false)
            {
                transform.rotation =
                    Quaternion.LookRotation(new Vector3(horizontal, 0, vertical));
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

            healthUpdateEvent.Invoke(health, MaxHealth);
            GetCharacterAnimatior()?.TriggerDamage();
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

        public void HandleThrowEvent()
        {
            GetCharacterAnimatior()?.TriggerThrow();
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

        private CharacterAnimatior GetCharacterAnimatior()
        {
            if (_characterAnimator == null) _characterAnimator = GetComponent<CharacterAnimatior>();

            return _characterAnimator;
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
    }
}