namespace Game.Gameplay.WeaponHolderStates
{
    public interface IWeaponHolderStates
    {
        public bool isAiming { get; }
        public bool canThrow { get; }
        public bool shouldReload { get; }
    }
}