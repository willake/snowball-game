namespace Game.Gameplay.CharacterStates
{
    public static class CharacterState
    {
        public static readonly ICharacterState IdleState = new IdleState();
        public static readonly ICharacterState AimState = new AimState();
        public static readonly ICharacterState ThrowState = new ThrowState();
        public static readonly ICharacterState ReloadState = new ReloadState();
        public static readonly ICharacterState DamagedState = new DamagedState();
        public static readonly ICharacterState DeadState = new DeadState();
    }

    public class IdleState : ICharacterState
    {
        // From: Move, Damaged, Throw
        // To: Move, Damaged, Dead, Hold
        public bool isAiming { get => false; }
        public bool isReloading { get => false; }
        public bool isDead { get => false; }
        public bool canMove { get => true; }
        public bool canThrow { get => true; }
        public bool canInterrupt { get => true; }
    }

    public class AimState : ICharacterState
    {
        // From: Idle, Move, HoldMove
        // To: HoldMove, Damaged, Dead, Throw
        public bool isAiming { get => true; }
        public bool isReloading { get => false; }
        public bool isDead { get => false; }
        public bool canMove { get => true; }
        public bool canThrow { get => false; }
        public bool canInterrupt { get => true; }
    }

    public class ThrowState : ICharacterState
    {
        // From: Hold, HoldMove
        // To: Idle, Dead
        public bool isAiming { get => true; }
        public bool isReloading { get => false; }
        public bool isDead { get => false; }
        public bool canMove { get => false; }
        public bool canThrow { get => false; }
        public bool canInterrupt { get => false; }
    }

    public class DamagedState : ICharacterState
    {
        // From: any
        // To: Idle
        public bool isAiming { get => false; }
        public bool isReloading { get => false; }
        public bool isDead { get => false; }
        public bool canMove { get => false; }
        public bool canThrow { get => false; }
        public bool canInterrupt { get => false; }
    }

    public class DeadState : ICharacterState
    {
        // From: any
        // To: NaN
        public bool isAiming { get => false; }
        public bool isReloading { get => false; }
        public bool isDead { get => true; }
        public bool canMove { get => false; }
        public bool canThrow { get => false; }
        public bool canInterrupt { get => false; }
    }

    public class ReloadState : ICharacterState
    {
        // From: any
        // To: NaN
        public bool isAiming { get => false; }
        public bool isReloading { get => true; }
        public bool isDead { get => false; }
        public bool canMove { get => false; }
        public bool canThrow { get => false; }
        public bool canInterrupt { get => true; }
    }
}