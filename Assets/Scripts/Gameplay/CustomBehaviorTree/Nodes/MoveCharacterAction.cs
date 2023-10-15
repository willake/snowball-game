using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

namespace Game.Gameplay.CustomBehaviorTree
{
    [AddComponentMenu("")]
    [MBTNode("Custom Actions/Move Character Action")]
    public class MoveCharacterAction : Leaf
    {
        public Vector3Reference destination;
        public AICharacter character;
        public float stopDistance = 2f;
        [Tooltip("How often target position should be updated")]
        public float updateInterval = 1f;
        private float time = 0;

        public override void OnEnter()
        {
            time = 0;
            character.MoveTo(destination.Value);
        }

        public override NodeResult Execute()
        {
            time += Time.deltaTime;
            // Update destination every given interval
            if (time > updateInterval)
            {
                // Reset time and update destination
                time = 0;
                character.MoveTo(destination.Value);
            }
            // Check if path is ready
            if (character.PathPending)
            {
                return NodeResult.running;
            }
            // Check if agent is very close to destination
            if (character.RemainingDistance < stopDistance)
            {
                return NodeResult.success;
            }
            // Check if there is any path (if not pending, it should be set)
            if (character.HasPath)
            {
                return NodeResult.running;
            }
            // By default return failure
            return NodeResult.failure;
        }

        public override void OnExit()
        {
            character.Idle();
            // agent.ResetPath();
        }

        public override bool IsValid()
        {
            return !(character == null);
        }
    }
}