using UnityEngine;

namespace Game.Gameplay
{
    public class RegularEnemy : AIController
    {
        private void Update()
        {
            if (isControllable == false) return;

            if (bindedCharacter.State.isAiming)
            {
                Vector3 direction = statePlayerPos.value - transform.position;
                bindedCharacter.UpdateAimDirection(direction.normalized, false);
            }

            //target player
            float distance = Vector3.Distance(transform.position, statePlayerPos.value);

            if (distance < attackRange)
            {
                AttackPlayer();
                return;
            }
            // player is in sight range
            if (distance < sightRange)
            {
                ChasePlayer();
                return;
            }
        }
    }
}