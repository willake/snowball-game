using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;
using Game.RuntimeStates;

namespace Game.Gameplay.CustomBehaviorTree
{
    [AddComponentMenu("")]
    [MBTNode(name = "Custom Conditions/Is Player Alive Condition")]
    public class isPlayerAliveCondition : Condition
    {
        public BoolState isPlayerDead;

        public override bool Check()
        {
            return !isPlayerDead.value;
        }

        public override bool IsValid()
        {
            return !(isPlayerDead == null);
        }
    }
}
