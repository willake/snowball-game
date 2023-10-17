using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Saves
{
    // - average accuracy of throwing ball
    // - average time to kill boss
    // - average strength/distance snowball thrown
    // - average distance between player and enemy when killed
    // - percentage of enemies killed in total
    // - average distance player traveled
    // - average snow ball thrown in total
    // - average times the player reload
    [System.Serializable]
    public class GameStatisticsDataV1 : IEntity<GameStatisticsDataV1>
    {
        public long id;
        public int level;
        public int version;
        public bool isPlayerWin;
        public int deathCount;
        public int damagedCount;
        public int reloadCount;
        public int killedEnemyCount;
        public int killedRegulars;
        public int killedRangers;
        public int killedSnipers;
        public int killedBosses;
        public int totalEnemyCount;
        public List<ThrownBall> thrownBalls;
        public long finishTime;
        public long createdAt;
        public long updatedAt;

        [System.Serializable]
        public struct ThrownBall
        {
            public float hitDistance;
            public float energy;
            public bool isCritical;
            public bool isHitEnemy;
            public bool isKillEnemy;
        }

        public long SaveKey { get => id; }

        public GameStatisticsDataV1(long id)
        {
            this.id = id;
            this.level = -1;
            this.version = 1;
            this.isPlayerWin = false;
            this.deathCount = 0;
            this.damagedCount = 0;
            this.reloadCount = 0;
            this.killedEnemyCount = 0;
            this.killedRegulars = 0;
            this.killedRangers = 0;
            this.killedSnipers = 0;
            this.killedBosses = 0;
            this.totalEnemyCount = 0;
            this.thrownBalls = new();
            this.finishTime = 0;
            this.createdAt = TimeStampUtils.NowInSeconds;
            this.updatedAt = TimeStampUtils.NowInSeconds;
        }

        public void Update(GameStatisticsDataV1 data)
        {
            this.id = data.id;
            this.level = data.level;
            this.version = 1;
            this.isPlayerWin = false;
            this.deathCount = data.deathCount;
            this.damagedCount = data.damagedCount;
            this.reloadCount = data.reloadCount;
            this.killedEnemyCount = data.killedEnemyCount;
            this.killedRegulars = data.killedRegulars;
            this.killedRangers = data.killedRangers;
            this.killedSnipers = data.killedSnipers;
            this.killedBosses = data.killedBosses;
            this.totalEnemyCount = data.totalEnemyCount;
            this.thrownBalls = data.thrownBalls;
            this.finishTime = data.finishTime;
            this.updatedAt = TimeStampUtils.NowInSeconds;
        }

        public void BuildIndex()
        {

        }
    }
}