namespace Game.Gameplay.CharacterStates
{
    public interface ICharacterState
    {
        public bool isAiming { get; }
        public bool canMove { get; }
        public bool canThrow { get; }
        public bool canInterrupt { get; }
    }
}