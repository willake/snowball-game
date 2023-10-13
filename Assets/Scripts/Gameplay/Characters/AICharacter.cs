using UnityEngine;
using UnityEngine.AI;
using Game.Gameplay.CharacterStates;

namespace Game.Gameplay
{
    public class AICharacter : Character
    {
        public override Vector3 Velocity => GetNavMeshAgent() ? GetNavMeshAgent().velocity : Vector3.zero;

        private NavMeshAgent _navMeshAgent;

        private void Start()
        {
            dieEvent.AddListener(() => SetMovementEnabled(false));
            weaponHolder.throwEvent.AddListener(() => SetMovementEnabled(false));
            GetCharacterAnimatior()?.thorwEndedEvent.AddListener(
                () => SetMovementEnabled(true));
            weaponHolder.reloadStartEvent.AddListener(() => SetMovementEnabled(false));
            weaponHolder.reloadEndEvent.AddListener(() => SetMovementEnabled(true));
        }

        public void SetMovementEnabled(bool isEnabled)
        {
            GetNavMeshAgent().isStopped = !isEnabled;
        }

        public void Aim()
        {
            if (isGrounded == false || State.canThrow == false) return;
            weaponHolder.AimWithoutCharging();
            SetCharacterState(CharacterState.AimState);
        }

        public void Reload()
        {
            if (State.canReload == false || State.isReloading) return;
            weaponHolder.Reload();
        }

        public float EstimateEnergyToPosition(Vector3 target)
        {
            float gravity = Physics.gravity.magnitude;
            // Selected angle in radians
            float angle = weaponHolder.throwingPitch * Mathf.Deg2Rad;
            Vector3 weaponPos = weaponHolder.transform.position;

            // Positions of this object and the target on the same plane
            Vector3 planarTarget = new Vector3(target.x, 0, target.z);
            Vector3 planarPostion = new Vector3(weaponPos.x, 0, weaponPos.z);

            // Planar distance between objects
            float distance = Vector3.Distance(planarTarget, planarPostion);
            // Distance along the y axis between objects
            float yOffset = transform.position.y - target.y;

            float initialVelocity = (1 / Mathf.Cos(angle)) *
                Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) /
                (distance * Mathf.Tan(angle) + yOffset));

            Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

            // Rotate our velocity to match the direction between the two objects
            float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion);
            Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

            return finalVelocity.magnitude;
        }

        public void MoveTo(Vector3 position)
        {
            if (State.canMove == false) return;
            GetNavMeshAgent().SetDestination(position);
        }

        public NavMeshAgent GetNavMeshAgent()
        {
            if (_navMeshAgent == null) _navMeshAgent = GetComponent<NavMeshAgent>();

            return _navMeshAgent;
        }
    }
}