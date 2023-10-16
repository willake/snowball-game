using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;
using Game.RuntimeStates;

namespace Game.Gameplay.CustomBehaviorTree
{
    [AddComponentMenu("")]
    [MBTNode(name = "Custom Conditions/Is Game Running Condition")]
    public class isGameRunningCondition : Condition
    {
        public BoolState isGameRunning;

        public override bool Check()
        {
            return isGameRunning.value;
        }

        public override bool IsValid()
        {
            return !(isGameRunning == null);
        }
    }
}
