using UnityEngine;
using Game.RuntimeStates;
using Game.UI;
using Game.Gameplay.Cameras;
using System.Collections;

namespace Game.Gameplay
{
    public class AIController : Controller
    {
        [Header("References")]
        public Vector3State statePlayerPos;
        public ProgressBar healthBar;

        [Header("Settings")]
        public float sightRange;
        public float attackRange;
        //Patroling
        public Transform[] patrolPoints;
        public float walkPointRange;
        //Attacking
        public float minAttackIntervalInSeconds = 1f;
        public float maxAttackIntervalInSeconds = 2f;
        private float _nextAttackInterval;
        private float _lastAttackTime = 0;

        public bool isControllable { get; private set; }

        private AICharacter _aiCharacter;

        private void Start()
        {
            if (MainGameScene.instance)
            {
                SetupHealthBar(
                    MainGameScene.instance.worldSpaceCanvas,
                    MainGameScene.instance.playerCamera
                );
                MainGameScene.instance.RegisterEnemy(this);
            }

            bindedCharacter.healthUpdateEvent.AddListener(UpdateHealthBar);
            bindedCharacter.dieEvent.AddListener(HandleDieEvent);

            isControllable = true;
        }

        protected void Patroling()
        {
            if (patrolPoints.Length == 0) return;
            // if (!walkPointSet) SearchWalkPoint();

            // Vector3 distanceToWalkPoint = transform.position - walkPoint;

            //Walkpoint reached
            // if (distanceToWalkPoint.magnitude < 1f)
            // walkPointSet = false;
        }
        protected void SearchWalkPoint()
        {
            //Calculate random point in range
            // float randomZ = Random.Range(-walkPointRange, walkPointRange);
            // float randomX = Random.Range(-walkPointRange, walkPointRange);

            // walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            // if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            // walkPointSet = true;
        }

        protected void ChasePlayer()
        {
            GetAICharacter().MoveTo(statePlayerPos.value);
        }

        protected void AttackPlayer()
        {
            if (Time.time - _lastAttackTime < _nextAttackInterval) return;
            if (bindedCharacter.weaponHolder.Ammo <= 0)
            {
                bindedCharacter.Reload();
            }

            Vector3 direction = statePlayerPos.value - transform.position;
            bindedCharacter.UpdateAimDirection(direction.normalized, false);

            float energy = GetAICharacter().EstimateEnergyToPosition(statePlayerPos.value);
            GetAICharacter().ThrowWithoutCharging(energy);

            _nextAttackInterval = Random.Range(minAttackIntervalInSeconds, maxAttackIntervalInSeconds);
            _lastAttackTime = Time.time;
        }

        private void HandleDieEvent()
        {
            isControllable = false;
            bindedCharacter.GetNavMeshAgent().isStopped = true;
            StartCoroutine(DestoryCharacter());
        }

        IEnumerator DestoryCharacter()
        {
            yield return new WaitForSeconds(2f);
            MainGameScene.instance?.EliminateEnemy(this);
            Destroy(this.healthBar.gameObject);
            Destroy(this.gameObject);
        }

        private void UpdateHealthBar(float health, float maxHealth)
        {
            healthBar.SetProgress(health / maxHealth);
        }

        private void SetupHealthBar(Canvas canvas, Camera camera)
        {
            // set healthbar transform to canvas.transform
            healthBar.SetProgress(1, 0);
            healthBar.transform.SetParent(canvas.transform);
            if (healthBar.TryGetComponent<FaceCamera>(out FaceCamera faceCamera))
            {
                faceCamera.cam = camera;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, sightRange);
        }

        protected AICharacter GetAICharacter()
        {
            if (_aiCharacter == null) _aiCharacter = bindedCharacter as AICharacter;

            return _aiCharacter;
        }
    }
}