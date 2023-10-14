using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    [CreateAssetMenu(menuName = "MyGame/Resources/AudioResources")]
    public class AudioResources : ScriptableObject
    {
        [Header("UI Audio Assets")]
        public UIAudios uiAudios;
        public BackgroundAudios backgroundAudios;
        public GameplayAudios gameplayAudios;

        [Serializable]
        public class UIAudios
        {
            [FormerlySerializedAs("ButtonClick")] public WrappedAudioClip buttonClick;
            [FormerlySerializedAs("ButtonConfirm")] public WrappedAudioClip buttonConfirm;
        }

        [Serializable]
        public class BackgroundAudios
        {
            [FormerlySerializedAs("AmbienceWind")] public WrappedAudioClip ambienceWind;
        }

        [Serializable]
        public class GameplayAudios
        {
            [FormerlySerializedAs("LevelWin")] public WrappedAudioClip levelWin;
            [FormerlySerializedAs("LevelLose")] public WrappedAudioClip levelLose;
            [FormerlySerializedAs("LevelStart")] public WrappedAudioClip levelStart;
            [FormerlySerializedAs("Footstep1")] public WrappedAudioClip footStep1;
            [FormerlySerializedAs("Footstep2")] public WrappedAudioClip footStep2;
            [FormerlySerializedAs("SnowballThrow")] public WrappedAudioClip snowballThrow;
            [FormerlySerializedAs("SnowballNotHit")] public WrappedAudioClip snowballNotHit;
            [FormerlySerializedAs("SnowballHit")] public WrappedAudioClip snowballHit;
            [FormerlySerializedAs("SnoaballHitHard")] public WrappedAudioClip snowballHitHard;
            [FormerlySerializedAs("PlayerDamaged")] public WrappedAudioClip playerDamaged;
            [FormerlySerializedAs("EnemyDamaged")] public WrappedAudioClip enemyDamaged;
            [FormerlySerializedAs("Reload")] public WrappedAudioClip reload;
            [FormerlySerializedAs("CriticalCharge")] public WrappedAudioClip criticalCharge;
        }
    }
}