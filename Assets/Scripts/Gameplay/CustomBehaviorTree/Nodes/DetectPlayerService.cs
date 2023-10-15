using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;
using Game.RuntimeStates;

namespace Game.Gameplay.CustomBehaviorTree
{
    [AddComponentMenu("")]
    [MBTNode("Custom/Detect Player Service")]
    public class DetectPlayerService : Service
    {
        public Vector3State statePlayerPos;
        [Tooltip("Sphere radius")]
        public float sightRange = 10;
        public BoolReference isPlayerInView = new BoolReference(VarRefMode.DisableConstant);
        public Vector3Reference playerPos = new Vector3Reference(VarRefMode.DisableConstant);

        public override void Task()
        {
            // Find target in radius and feed blackboard variable with results
            float distance = Vector3.Distance(transform.position, statePlayerPos.value);

            if (distance < sightRange)
            {
                isPlayerInView.Value = true;
                playerPos.Value = statePlayerPos.value;
            }
            else
            {
                isPlayerInView.Value = false;
            }
        }
    }
}
