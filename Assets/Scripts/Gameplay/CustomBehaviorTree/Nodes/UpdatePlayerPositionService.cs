using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;
using Game.RuntimeStates;

namespace Game.Gameplay.CustomBehaviorTree
{
    [AddComponentMenu("")]
    [MBTNode("Custom Services/Update Player Position Service")]
    public class UpdatePlayerPositionService : Service
    {
        public Vector3State vector3State;
        public Vector3Reference vector3Reference = new Vector3Reference(VarRefMode.DisableConstant);

        public override void Task()
        {
            vector3Reference.Value = vector3State.value;
        }

        public override bool IsValid()
        {
            return !(vector3State == null);
        }
    }
}
