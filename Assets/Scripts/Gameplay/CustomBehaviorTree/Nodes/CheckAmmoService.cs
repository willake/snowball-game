using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

namespace Game.Gameplay.CustomBehaviorTree
{
    [AddComponentMenu("")]
    [MBTNode(name = "Custom Services/Check Ammo Service")]
    public class CheckAmmoService : Service
    {
        public AICharacter character;

        public BoolReference shouldReload = new BoolReference(VarRefMode.DisableConstant);
        public override void Task()
        {
            shouldReload.Value = character.weaponHolder.State.shouldReload;
        }

        public override bool IsValid()
        {
            return !(character == null);
        }
    }
}
