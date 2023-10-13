namespace Game.Gameplay.WeaponHolderStates
{
    public static class WeaponHolderState
    {
        public static readonly IWeaponHolderStates IdleState = new IdleState();
        public static readonly IWeaponHolderStates AimState = new AimState();
        public static readonly IWeaponHolderStates ReloadState = new ReloadState();
        public static readonly IWeaponHolderStates NeedReloadState = new NeedReloadState();
    }

    public class IdleState : IWeaponHolderStates
    {
        public bool isAiming { get => false; }
        public bool canThrow { get => true; }
        public bool shouldReload { get => false; }
    }

    public class AimState : IWeaponHolderStates
    {
        public bool isAiming { get => true; }
        public bool canThrow { get => false; }
        public bool shouldReload { get => false; }
    }

    public class ReloadState : IWeaponHolderStates
    {
        public bool isAiming { get => false; }
        public bool canThrow { get => false; }
        public bool shouldReload { get => false; }
    }

    public class NeedReloadState : IWeaponHolderStates
    {
        public bool isAiming { get => false; }
        public bool canThrow { get => false; }
        public bool shouldReload { get => true; }
    }
}