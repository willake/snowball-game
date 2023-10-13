using UnityEngine;
using Game.Gameplay.CharacterStates;

namespace Game.Gameplay
{
    public class PlayerCharacter : Character
    {
        public override Vector3 Velocity => GetRigidbody() ? GetRigidbody().velocity : Vector3.zero;
        public void Aim()
        {
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
            // haven't really throw yet, show do not set state
            if (State.isAiming)
            {
                weaponHolder.Throw();
            }
        }
    }
}