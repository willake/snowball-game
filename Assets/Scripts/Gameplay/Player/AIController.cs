using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.AI;
using Game.RuntimeStates;

namespace Game.Gameplay
{
    public class AIController : Controller
    {
        [Header("References")]
        public Vector3State statePlayerPos;

        [Header("Settings")]
        public LayerMask playerLayer;
        public float sightRange, attackRange;
        //Patroling
        public Transform[] patrolPoints;
        public float walkPointRange;
        //Attacking
        public float minAttackIntervalInSeconds = 1f;
        public float maxAttackIntervalInSeconds = 2f;
        private float _nextAttackInterval;
        private float _lastAttackTime = 0;

        private void Update()
        {
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

        private void Patroling()
        {
            if (patrolPoints.Length == 0) return;
            // if (!walkPointSet) SearchWalkPoint();

            // Vector3 distanceToWalkPoint = transform.position - walkPoint;

            //Walkpoint reached
            // if (distanceToWalkPoint.magnitude < 1f)
            // walkPointSet = false;
        }
        private void SearchWalkPoint()
        {
            //Calculate random point in range
            // float randomZ = Random.Range(-walkPointRange, walkPointRange);
            // float randomX = Random.Range(-walkPointRange, walkPointRange);

            // walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            // if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            // walkPointSet = true;
        }

        private void ChasePlayer()
        {
            bindedCharacter.MoveTo(statePlayerPos.value);
        }

        private void AttackPlayer()
        {
            if (Time.time - _lastAttackTime < _nextAttackInterval) return;
            if (bindedCharacter.weaponHolder.Ammo <= 0)
            {
                bindedCharacter.Reload();
            }
            //Make sure enemy doesn't move
            // agent.SetDestination(transform.position);

            Vector3 direction = statePlayerPos.value - transform.position;
            bindedCharacter.Aim(direction.normalized);

            bindedCharacter.ThrowWithoutCharging(5);

            _nextAttackInterval = UnityEngine.Random.Range(minAttackIntervalInSeconds, maxAttackIntervalInSeconds);
            _lastAttackTime = Time.time;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, sightRange);
        }
    }
}