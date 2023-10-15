using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;
using Game.RuntimeStates;

namespace Game.Gameplay.CustomBehaviorTree
{
    [AddComponentMenu("")]
    [MBTNode("Custom Services/Update Character State Service")]
    public class UpdateCharacterStateService : Service
    {
        public AICharacter aiCharacter;
        public BoolReference isAiming = new BoolReference(VarRefMode.DisableConstant);
        public BoolReference isReloading = new BoolReference(VarRefMode.DisableConstant);
        public BoolReference isDead = new BoolReference(VarRefMode.DisableConstant);
        public BoolReference canMove = new BoolReference(VarRefMode.DisableConstant);
        public BoolReference canThrow = new BoolReference(VarRefMode.DisableConstant);
        public BoolReference canReload = new BoolReference(VarRefMode.DisableConstant);

        public override void Task()
        {
            isAiming.Value = aiCharacter.State.isAiming;
            isReloading.Value = aiCharacter.State.isReloading;
            isDead.Value = aiCharacter.State.isDead;
            canMove.Value = aiCharacter.State.canMove;
            canThrow.Value = aiCharacter.State.canThrow;
            canReload.Value = aiCharacter.State.canReload;
        }

        public override bool IsValid()
        {
            return !(aiCharacter == null);
        }
    }
}
