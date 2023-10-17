using UnityEngine;
using Game.Gameplay.CharacterStates;

namespace Game.Gameplay
{
    public class PlayerCharacter : Character
    {
        [Header("Settings")]
        public float moveSpeed = 8f;
        public float aimSpeed = 5f;
        public Vector3 currentVelocity { get { return GetRigidbody().velocity; } }
        public override Vector3 Velocity => GetRigidbody() ? GetRigidbody().velocity : Vector3.zero;
        public void Aim()
        {
            if (State.isDead) return;
            if (isGrounded == false || State.canThrow == false) return;
            if (weaponHolder.Aim()) SetCharacterState(CharacterState.AimState);
        }

        public void Reload()
        {
            if (State.canReload == false || State.isReloading) return;
            GetRigidbody().velocity = Vector3.zero;
            weaponHolder.Reload();
        }

        public void TryThrow()
        {
            if (State.isDead) return;
            // haven't really throw yet, show do not set state
            if (State.isAiming)
            {
                weaponHolder.Throw();
            }
        }

        public void Idle()
        {
            GetRigidbody().velocity = new Vector3(0, GetRigidbody().velocity.y, 0);
        }

        public void Move(float horizontal, float vertical)
        {
            if (State.canMove == false) return;

            Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
            if (State.isAiming)
            {
                GetRigidbody().velocity = direction * aimSpeed;
                transform.rotation =
                    Quaternion.LookRotation(direction);
            }
            else
            {
                GetRigidbody().velocity = direction * moveSpeed;
            }
        }
    }
}