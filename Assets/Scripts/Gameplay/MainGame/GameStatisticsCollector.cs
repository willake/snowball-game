using System.Collections;
using System.Collections.Generic;
using System;
using Game.Events;
using UnityEngine;
using Game.Saves;
using Cysharp.Threading.Tasks;

namespace Game.Gameplay
{
    public class GameStatisticsCollector : MonoBehaviour
    {
        public const string IDENTITY = "GAME_DATA_COLLECTOR";
        private Lazy<EventManager> _eventManager = new Lazy<EventManager>(
            () => DIContainer.instance.GetObject<EventManager>(),
            true
        );
        protected EventManager EventManager { get => _eventManager.Value; }
        private Lazy<JsonRepository<GameStatisticsDataV1>> _statisticsDataRepository =
            new Lazy<JsonRepository<GameStatisticsDataV1>>(
                () => DIContainer.instance.GetObject<JsonRepository<GameStatisticsDataV1>>(),
                true
            );
        protected JsonRepository<GameStatisticsDataV1> StatisticsDataRepository
        { get => _statisticsDataRepository.Value; }

        private GameStatisticsDataV1 _statisticsData;
        public GameStatisticsDataV1 StatisticsData { get => _statisticsData; }

        Subscription _onGameEndSubscription;
        Subscription _onPlayerDeadSubscription;
        Subscription _onPlayerDamagedSubscription;
        Subscription _onPlayerReloadSubscription;
        Subscription _onEnemyDeadSubscription;
        Subscription _onPlayerBallHitSubscription;

        public void StartRecording(int level)
        {
            _statisticsData = new GameStatisticsDataV1(TimeStampUtils.NowInMilliseconds);

            _statisticsData.level = level;

            _onGameEndSubscription =
                EventManager.Subscribe(IDENTITY, EventNames.onGameEnd,
                (payload) =>
                {
                    _statisticsData.isPlayerWin = true;
                });

            _onPlayerDeadSubscription =
                EventManager.Subscribe(IDENTITY, EventNames.onPlayerDead,
                (payload) =>
                {
                    _statisticsData.deathCount += 1;
                });

            _onPlayerDamagedSubscription =
                EventManager.Subscribe(IDENTITY, EventNames.onPlayerDamaged,
                (payload) =>
                {
                    _statisticsData.damagedCount += 1;
                });

            _onPlayerReloadSubscription =
                EventManager.Subscribe(IDENTITY, EventNames.onPlayerReload,
                (Payload) =>
                {
                    _statisticsData.reloadCount += 1;
                });

            _onEnemyDeadSubscription =
                EventManager.Subscribe(IDENTITY, EventNames.onEnemyDead,
                (payload) =>
                {
                    _statisticsData.killedEnemyCount += 1;
                });

            _onPlayerBallHitSubscription =
                EventManager.Subscribe(IDENTITY, EventNames.onPlayerBallHit,
                (payload) =>
                {
                    GameStatisticsDataV1.ThrownBall thrownBall =
                        (GameStatisticsDataV1.ThrownBall)payload.args[0];
                    _statisticsData.thrownBalls.Add(thrownBall);
                });
        }

        public void StopRecording(bool isWin)
        {
            // save data in background
            _statisticsData.isPlayerWin = isWin;
            StatisticsDataRepository.Insert(_statisticsData).Forget();
            Debug.Log($"Save Statistics Data to {Consts.GAME_FOLDER_PATH() + "Saves"} with id: {_statisticsData.id}");
            EventManager.CancelSubscription(
                EventNames.onGameEnd,
                _onGameEndSubscription);
            EventManager.CancelSubscription(
                EventNames.onPlayerDead,
                _onPlayerDeadSubscription);
            EventManager.CancelSubscription(
                EventNames.onPlayerDamaged,
                _onPlayerDamagedSubscription);
            EventManager.CancelSubscription(
                EventNames.onPlayerReload,
                _onPlayerReloadSubscription
            );
            EventManager.CancelSubscription(
                EventNames.onEnemyDead,
                _onEnemyDeadSubscription);
            EventManager.CancelSubscription(
                EventNames.onPlayerBallHit,
                _onPlayerBallHitSubscription);
        }
    }
}