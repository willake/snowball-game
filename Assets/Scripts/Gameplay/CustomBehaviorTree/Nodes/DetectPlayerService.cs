using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;
using Game.RuntimeStates;

namespace Game.Gameplay.CustomBehaviorTree
{
    [AddComponentMenu("")]
    [MBTNode("Custom Services/Detect Player Service")]
    public class DetectPlayerService : Service
    {
        public LayerMask enemyLayer;
        public LayerMask playerLayer;
        public Vector3State statePlayerPos;
        [Tooltip("Sphere radius")]
        public float sightRange = 10;
        public BoolReference isPlayerInView = new BoolReference(VarRefMode.DisableConstant);
        public Vector3Reference playerPos = new Vector3Reference(VarRefMode.DisableConstant);

        public override void Task()
        {
            // Find target in radius and feed blackboard variable with results
            float distance = Vector3.Distance(transform.position, statePlayerPos.value);

            RaycastHit hit;
            bool canSeePlayer = false;
            if (Physics.Raycast(
                transform.position, statePlayerPos.value - transform.position,
                out hit, Mathf.Infinity, ~enemyLayer))
            {
                if (1 << hit.collider.gameObject.layer == playerLayer)
                {
                    canSeePlayer = true;
                }
            }

            if (canSeePlayer && distance < sightRange)
            {
                isPlayerInView.Value = true;
            }
            else
            {
                isPlayerInView.Value = false;
            }

            playerPos.Value = statePlayerPos.value;
        }

        public override bool IsValid()
        {
            return !(statePlayerPos == null);
        }
    }
}
