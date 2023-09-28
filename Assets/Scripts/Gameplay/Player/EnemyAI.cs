﻿
using UnityEngine;
using UnityEngine.AI;

namespace Game.Gameplay
{
    public class EnemyAI : MonoBehaviour
    {
        public NavMeshAgent agent;

        public Transform player, self;

        public LayerMask whatIsGround, whatIsPlayer;

        public float health;

        //Patroling
        public Vector3 walkPoint;
        bool walkPointSet;
        public float walkPointRange;

        //Attacking
        public float timeBetweenAttacks;
        bool alreadyAttacked;
        public WeaponHolder weaponHolder;

        //States
        public float sightRange, attackRange;
        public bool playerInSightRange, playerInAttackRange;
        private float lastAttack = 0;
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            self = GetComponent<Transform>();
        }

        private void Update()
        {
            //Check for sight and attack range
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!playerInSightRange && !playerInAttackRange) Patroling();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }

        private void Patroling()
        {
            if (!walkPointSet) SearchWalkPoint();

            if (walkPointSet)
                agent.SetDestination(walkPoint);

            Vector3 distanceToWalkPoint = transform.position - walkPoint;

            //Walkpoint reached
            if (distanceToWalkPoint.magnitude < 1f)
                walkPointSet = false;
        }
        private void SearchWalkPoint()
        {
            //Calculate random point in range
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            float randomX = Random.Range(-walkPointRange, walkPointRange);

            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
                walkPointSet = true;
        }

        private void ChasePlayer()
        {
            agent.SetDestination(player.position);
        }

        private void AttackPlayer()
        {
            if (weaponHolder.Ammo == 0)
                weaponHolder.Reload();
            //Make sure enemy doesn't move
            agent.SetDestination(transform.position);
            transform.LookAt(player);
            
            if (!alreadyAttacked)
            {
                timeBetweenAttacks = Random.Range(0.2f, 10f);
                // weaponHolder.Hold();
                alreadyAttacked = true;
            }

            if (Time.realtimeSinceStartup - lastAttack > timeBetweenAttacks)
            {
                lastAttack = Time.realtimeSinceStartup;
                weaponHolder.UpdateAimDirection(self.transform.forward);
                // weaponHolder.SetEnergy(timeBetweenAttacks);
                weaponHolder.ThrowWithoutCharging(5);
                ResetAttack();
            }
            
        }
        private void ResetAttack()
        {
            alreadyAttacked = false;
        }

        public void TakeDamage(float damage)
        {
            health -= damage;

            if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
        }
        private void DestroyEnemy()
        {
            Destroy(gameObject);
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