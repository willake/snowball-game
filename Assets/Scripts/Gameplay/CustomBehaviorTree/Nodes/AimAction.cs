using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MBT;

namespace Game.Gameplay.CustomBehaviorTree
{
    [AddComponentMenu("")]
    [MBTNode("Custom Actions/Aim Action")]
    public class AimAction : Leaf
    {
        public AICharacter aiCharacter;

        public override NodeResult Execute()
        {
            if (aiCharacter.State.isDead)
            {
                aiCharacter.Idle();
                return NodeResult.failure;
            }
            aiCharacter.Aim();
            return NodeResult.success;
        }

        public override bool IsValid()
        {
            return !(aiCharacter == null);
        }
    }
}