using UnityEngine;

namespace Game.Gameplay
{
    public class RegularEnemy : AIController
    {
        private void Update()
        {
            if (isControllable == false) return;
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