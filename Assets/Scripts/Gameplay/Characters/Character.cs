using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Game.Gameplay.CharacterStates;

namespace Game.Gameplay
{
    public abstract class Character : MonoBehaviour
    {
        private CharacterAnimatior _characterAnimator;
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

        public bool isGrounded { get; private set; }
        public ICharacterState State { get; private set; }
        public abstract Vector3 Velocity { get; }

        private void Start()
        {
            MaxHealth = health;
            weaponHolder.throwEvent.AddListener(() => SetCharacterState(CharacterState.ThrowState));
            GetCharacterAnimatior()?.thorwEndedEvent.AddListener(
                () => SetCharacterState(CharacterState.IdleState));
            GetCharacterAnimatior()?.damageEndedEvent.AddListener(
                () => SetCharacterState(CharacterState.IdleState));

            SetCharacterState(IdleState);
            Debug.Log(gameObject.name + "Character Start");
            Debug.Log(State);
        }

        public void Idle()
        {
            GetRigidbody().velocity = new Vector3(0, GetRigidbody().velocity.y, 0);
        }

        public void Move(float horizontal, float vertical)
        {
            if (State.canMove == false) return;
            if (GetRigidbody().velocity.magnitude < maxSpeed)
            {
                GetRigidbody().AddForce(
                 new Vector3(horizontal, 0, vertical) * acc, ForceMode.Force);
            }

            if (State.isAiming)
            {
                transform.rotation =
                    Quaternion.LookRotation(new Vector3(horizontal, 0, vertical));
            }
        }

        public void TakeDamage(float damage, Vector3 direction)
        {
            if (State.canInterrupt == false) return;

            health -= damage;
            healthUpdateEvent.Invoke(health, MaxHealth);

            if (health < float.Epsilon)
            {
                health = 0f;
                dieEvent.Invoke();
                SetCharacterState(CharacterState.DeadState);
            }
            else
            {
                SetCharacterState(CharacterState.DamagedState);
            }

            if (State.isAiming)
            {
                ThrowWithoutCharging(1f);
            }

            transform.rotation =
                    Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z) * -1);
        }

        public void Reload()
        {
            weaponHolder.Reload();
        }

        public void UpdateAimDirection(Vector3 direction, bool useFoward = true)
        {
            float angle = (float)Math.Atan2(direction.x, direction.y);
            transform.rotation = Quaternion.Euler(
                new Vector3(0, angle * Mathf.Rad2Deg, 0));

            weaponHolder.UpdateAimDirection(useFoward ? transform.forward : direction);
        }

        public void ThrowWithoutCharging(float energy)
        {
            if (State.canThrow == false) return;
            weaponHolder.ThrowWithoutCharging(energy);
        }

        protected void SetCharacterState(ICharacterState state)
        {
            State = state;

            if (state == CharacterState.DamagedState)
            {
                GetCharacterAnimatior()?.TriggerDamage();
            }

            if (state == CharacterState.DeadState)
            {
                GetCharacterAnimatior()?.TriggerDead();
            }

            if (state == CharacterState.ThrowState)
            {
                GetCharacterAnimatior()?.TriggerThrow();
            }

            GetCharacterAnimatior()?.SetIsAiming(state == CharacterState.AimState);
        }

        private void Update()
        {
            RaycastHit hit;
            isGrounded = Physics.Raycast(
                transform.position, transform.TransformDirection(Vector3.down), out hit,
                0.5f, LayerMask.NameToLayer("Floor"));

            Debug.Log("Current State: " + State);
            if (State.canMove == false) return;

            Vector3 velocity = Velocity;
            if (State.isAiming && velocity.magnitude > float.Epsilon)
            {
                // calculate angle of moving direction
                float movingAngle = (velocity.x > 0 ? 1 : -1) *
                    Vector2.Angle(
                        new Vector2(0, 1),
                        new Vector2(velocity.normalized.x, velocity.normalized.z)
                    );
                // calculate angle of facing direction
                Vector2 forward = new Vector2(transform.forward.x, transform.forward.z);
                float facingAngle = (transform.forward.x > 0 ? 1 : -1) * Vector2.Angle(new Vector2(0, 1), forward);
                // calculate realative moving angle of facing direction
                float angle = (movingAngle - facingAngle) * Mathf.Deg2Rad;
                GetCharacterAnimatior()?.SetMoveSpeed(
                    Mathf.Sin(angle),
                    Mathf.Cos(angle), 1);
            }
            else if (velocity.magnitude > float.Epsilon)
            {
                GetCharacterAnimatior()?.SetMoveSpeed(
                    velocity.x,
                    velocity.z, 1);
                transform.rotation =
                    Quaternion.LookRotation(new Vector3(velocity.normalized.x, 0, velocity.normalized.z));
            }
            else
            {
                GetCharacterAnimatior()?.SetMoveSpeed(
                    0,
                    0, 0);
            }
        }

        private void OnDestroy()
        {
            healthUpdateEvent.RemoveAllListeners();
            dieEvent.RemoveAllListeners();
        }

        public class HealthUpdateEvent : UnityEvent<float, float> { }
        public class DieEvent : UnityEvent { }

        protected CharacterAnimatior GetCharacterAnimatior()
        {
            if (_characterAnimator == null) _characterAnimator = GetComponent<CharacterAnimatior>();

            return _characterAnimator;
        }
        protected Rigidbody GetRigidbody()
        {
            if (_rigibody == null) _rigibody = GetComponent<Rigidbody>();

            return _rigibody;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.forward * 3);
            Gizmos.DrawLine(transform.position, Vector3.down);
        }

        public static readonly ICharacterState IdleState = new IdleState();
        public static readonly ICharacterState AimState = new AimState();
        public static readonly ICharacterState ThrowState = new ThrowState();
        public static readonly ICharacterState DamagedState = new DamagedState();
        public static readonly ICharacterState DeadState = new DeadState();
    }
}