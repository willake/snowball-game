using UnityEngine;
using Game.Gameplay.CharacterStates;

namespace Game.Gameplay
{
    public class PlayerCharacter : Character
    {
        public override Vector3 Velocity => GetRigidbody() ? GetRigidbody().velocity : Vector3.zero;
        public void Aim()
        {
            if (State.canThrow == false) return;
            weaponHolder.Aim();
            SetCharacterState(CharacterState.AimState);
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