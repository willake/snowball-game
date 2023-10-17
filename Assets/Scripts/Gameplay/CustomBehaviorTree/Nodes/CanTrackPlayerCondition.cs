using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;
using Game.RuntimeStates;

namespace Game.Gameplay.CustomBehaviorTree
{
    [AddComponentMenu("")]
    [MBTNode(name = "Custom Conditions/Can Track Player Condition")]
    public class CanTrackPlayerCondition : Condition
    {
        public BoolReference isDamaged = new BoolReference(VarRefMode.DisableConstant);
        public BoolReference isTargetingPlayer = new BoolReference(VarRefMode.DisableConstant);
        public BoolReference isPlayerInView = new BoolReference(VarRefMode.DisableConstant);

        public override bool Check()
        {
            return isDamaged.Value || isTargetingPlayer.Value || isPlayerInView.Value;
        }

        public override bool IsValid()
        {
            return !(isDamaged.isInvalid) || !(isTargetingPlayer.isInvalid) || !(isPlayerInView.isInvalid);
        }
    }
}
