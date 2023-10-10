namespace Game.Gameplay.CharacterStates
{
    public class CharacterState
    {

    }

    public class Idle : CharacterState
    {
        // From: Move, Damaged, Throw
        // To: Move, Damaged, Dead, Hold
    }

    public class Move : CharacterState
    {
        // From: Idle
        // To: Idle, Damaged, Dead, Hold 
    }

    public class HoldMove : CharacterState
    {
        // From: Move, Hold
        // To: Throw, Damaged, Dead
    }

    public class Hold : CharacterState
    {
        // From: Idle, Move, HoldMove
        // To: HoldMove, Damaged, Dead, Throw
    }

    public class Throw : CharacterState
    {
        // From: Hold, HoldMove
        // To: Idle, Dead
    }

    public class Damage : CharacterState
    {
        // From: any
        // To: Idle
    }

    public class Dead : CharacterState
    {
        // From: any
        // To: NaN
    }
}