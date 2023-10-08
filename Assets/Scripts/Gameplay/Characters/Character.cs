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

        public bool isAiming { get; private set; }
        public bool isGrounded { get; private set; }

        private void Start()
        {
            MaxHealth = health;
            weaponHolder.throwEvent.AddListener(HandleThrowEvent);
        }

        public void SetIsAiming(bool isAiming)
        {
            this.isAiming = isAiming;
            GetCharacterAnimatior().SetIsAiming(isAiming);
        }

        public void Idle()
        {
            GetRigidbody().velocity = new Vector3(0, GetRigidbody().velocity.y, 0);
            GetCharacterAnimatior()?.SetMoveSpeed(0, 0, 0);
        }

        public void Move(float horizontal, float vertical)
        {
            if (GetRigidbody().velocity.magnitude < maxSpeed)
            {
                GetRigidbody().AddForce(
                 new Vector3(horizontal, 0, vertical) * acc, ForceMode.Force);

                // calculate angle of moving direction
                float movingAngle = (horizontal > 0 ? 1 : -1) *
                    Vector2.Angle(new Vector2(0, 1), new Vector2(horizontal, vertical));
                // calculate angle of facing direction
                Vector2 forward = new Vector2(transform.forward.x, transform.forward.z);
                float facingAngle = (transform.forward.x > 0 ? 1 : -1) * Vector2.Angle(new Vector2(0, 1), forward);
                float angle = (movingAngle - facingAngle) * Mathf.Deg2Rad;
                // Debug.Log($"Facing Angle: {facingAngle}, Moving Angle: {movingAngle}, Angle: {(facingAngle - movingAngle)}");
                GetCharacterAnimatior()?.SetMoveSpeed(
                    Mathf.Sin(angle),
                    Mathf.Cos(angle), 1);
            }

            if (isAiming == false)
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

        private void Update()
        {
            RaycastHit hit;
            if (Physics.Raycast(
                transform.position, transform.TransformDirection(Vector3.down), out hit, 0.5f, LayerMask.NameToLayer("Floor")))
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }

        public void HandleThrowEvent()
        {
            GetCharacterAnimatior()?.TriggerThrow();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.forward * 3);
            Gizmos.DrawLine(transform.position, Vector3.down);
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