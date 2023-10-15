using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

namespace Game.Gameplay.CustomBehaviorTree
{
    [AddComponentMenu("")]
    [MBTNode("Custom Actions/Throw Action")]
    public class ThrowAction : Leaf
    {
        public Vector3Reference playerPos = new Vector3Reference(VarRefMode.DisableConstant);
        public AICharacter aiCharacter;

        public override NodeResult Execute()
        {
            Vector3 direction = playerPos.Value - transform.position;
            aiCharacter.UpdateAimDirection(direction.normalized, false);

            float energy = aiCharacter.EstimateEnergyToPosition(playerPos.Value) + 5;
            aiCharacter.Throw(energy);
            return NodeResult.success;
        }

        public override bool IsValid()
        {
            return !(aiCharacter == null);
        }
    }
}