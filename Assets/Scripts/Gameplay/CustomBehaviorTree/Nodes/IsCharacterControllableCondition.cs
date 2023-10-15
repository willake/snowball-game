using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

namespace Game.Gameplay.CustomBehaviorTree
{
    [AddComponentMenu("")]
    [MBTNode(name = "Custom Conditions/Is Character Controllable Condition")]
    public class IsCharacterControllableCondition : Condition
    {
        public BoolReference isDead = new BoolReference(VarRefMode.DisableConstant);
        public BoolReference canMove = new BoolReference(VarRefMode.DisableConstant);

        public override bool Check()
        {
            return !isDead.Value && canMove.Value;
        }
    }
}
