using UnityEngine;

namespace Game.Gameplay
{
    public class PlayerCharacter : Character
    {
        public void Hold()
        {
            if (isThrowing || isDamaging) return;
            weaponHolder.Hold();
        }

        public void Throw()
        {
            weaponHolder.Throw();
        }
    }
}