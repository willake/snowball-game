using UnityEngine;

namespace Game.Gameplay
{
    public class PlayerCharacter : Character
    {
        public void Hold()
        {
            weaponHolder.Hold();
        }

        public void Throw()
        {
            weaponHolder.Throw();
        }
    }
}