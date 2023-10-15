using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MBT;

namespace Game.Gameplay.CustomBehaviorTree
{
    [AddComponentMenu("")]
    [MBTNode("Custom Actions/Reload Action")]
    public class ReloadAction : Leaf
    {
        public AICharacter aiCharacter;

        public override NodeResult Execute()
        {
            aiCharacter.Reload();
            return NodeResult.success;
        }

        public override bool IsValid()
        {
            return !(aiCharacter == null);
        }
    }
}