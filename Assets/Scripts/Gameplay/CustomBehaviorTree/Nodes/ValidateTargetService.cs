using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;
using Game.RuntimeStates;

namespace Game.Gameplay.CustomBehaviorTree
{
    [AddComponentMenu("")]
    [MBTNode("Custom Services/Validate Target Service")]
    public class ValidateTargetService : Service
    {
        public Vector3State statePlayerPos;
        public BoolReference isTargetingPlayer;
        public float validateDistance = 14f;

        public override void Task()
        {
            float distance = Vector3.Distance(transform.position, statePlayerPos.value);
            isTargetingPlayer.Value = distance > validateDistance ? false : isTargetingPlayer.Value;
        }

        public override bool IsValid()
        {
            return !(statePlayerPos == null) || !isTargetingPlayer.isInvalid;
        }
    }
}
