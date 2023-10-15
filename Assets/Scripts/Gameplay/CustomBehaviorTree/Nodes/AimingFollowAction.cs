using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;
using Game.RuntimeStates;

namespace Game.Gameplay.CustomBehaviorTree
{
    [AddComponentMenu("")]
    [MBTNode("Custom Actions/Aiming Follow Action")]
    public class AimingFollowAction : Leaf
    {
        public Vector3State playerPos;
        public AICharacter character;
        public float stopDistance = 2f;
        [Tooltip("How often target position should be updated")]
        public float updateInterval = 1f;
        public float minAimingTime = 0.6f;
        public float maxAimingTime = 1.2f;
        private float _time = 0;
        private float _aimingTime = 0;
        private float _determinedAimingTime = 0;

        public override void OnEnter()
        {
            _time = 0;
            _aimingTime = 0;
            _determinedAimingTime =
                Random.Range(minAimingTime, maxAimingTime);
            character.MoveTo(playerPos.value);
        }

        public override NodeResult Execute()
        {
            _time += Time.deltaTime;
            _aimingTime += Time.deltaTime;
            // Update destination every given interval

            Vector3 direction = playerPos.value - transform.position;
            character.UpdateAimDirection(direction.normalized, false);

            if (_time > updateInterval)
            {
                // Reset time and update destination
                _time = 0;
                character.MoveTo(playerPos.value);
            }
            if (_aimingTime < _determinedAimingTime)
            {
                return NodeResult.running;
            }
            // Check if path is ready
            if (character.PathPending)
            {
                return NodeResult.running;
            }
            return NodeResult.success;
        }

        public override void OnExit()
        {
            character.Idle();
            // agent.ResetPath();
        }

        public override bool IsValid()
        {
            return !(character == null) && !(playerPos == null);
        }
    }
}