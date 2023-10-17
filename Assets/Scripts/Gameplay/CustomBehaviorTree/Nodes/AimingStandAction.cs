using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;
using Game.RuntimeStates;

namespace Game.Gameplay.CustomBehaviorTree
{
    [AddComponentMenu("")]
    [MBTNode("Custom Actions/Aiming Stand Action")]
    public class AimingStandAction : Leaf
    {
        public Vector3State playerPos;
        public AICharacter character;
        public float minAimingTime = 0.6f;
        public float maxAimingTime = 1.2f;
        private float _aimingTime = 0;
        private float _determinedAimingTime = 0;

        public override void OnEnter()
        {
            _aimingTime = 0;
            _determinedAimingTime =
                Random.Range(minAimingTime, maxAimingTime);
        }

        public override NodeResult Execute()
        {
            _aimingTime += Time.deltaTime;
            // Update destination every given interval

            Vector3 direction = playerPos.value - transform.position;
            character.UpdateAimDirection(direction.normalized, false);

            if (character.State.isDead)
            {
                return NodeResult.failure;
            }

            if (_aimingTime < _determinedAimingTime)
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