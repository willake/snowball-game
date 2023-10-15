using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MBT;

namespace Game.Gameplay.CustomBehaviorTree
{
    [AddComponentMenu("")]
    [MBTNode("Custom Actions/Update Aim Direction Action")]
    public class UpdateAimDirectionAction : Leaf
    {
        public Vector3Reference playerPos = new Vector3Reference(VarRefMode.DisableConstant);
        public AICharacter aiCharacter;

        public override NodeResult Execute()
        {
            Vector3 direction = playerPos.Value - transform.position;
            aiCharacter.UpdateAimDirection(direction.normalized, false);
            return NodeResult.success;
        }

        public override bool IsValid()
        {
            return !(aiCharacter == null);
        }
    }
}