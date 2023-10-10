using UnityEngine;
using Game.Gameplay.CharacterStates;

namespace Game.Gameplay
{
    public class PlayerCharacter : Character
    {
        public void Aim()
        {
            if (State.canThrow == false) return;
            weaponHolder.Hold();
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