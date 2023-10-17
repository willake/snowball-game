namespace Game.Events
{
    public class EventNames
    {
        /*
        args
        {
            AvaliableScene scene
        }
        */
        public static EventName loadScene = new EventName("LOAD_SCENE", false);
        /*
        args
        {
            bool isWin
        }
        */
        public static EventName onGameEnd = new EventName("ON_GAME_END", false);
        public static EventName onPlayerDead = new EventName("ON_PLAYER_DEAD", false);
        public static EventName onPlayerDamaged = new EventName("ON_PLAYER_DAMAGED", false);
        public static EventName onPlayerReload = new EventName("ON_PLAYER_RELOAD", false);
        /*
        args
        {
            EnemyType enemyType
        }
        */
        public static EventName onEnemyDead = new EventName("ON_ENEMY_DEAD", false);

        /*
        args
        {
            ThrowedBall throwedBall
        }
        */
        public static EventName onPlayerBallHit = new EventName("ON_PLAYER_BALL_HIT", false);
    }
}