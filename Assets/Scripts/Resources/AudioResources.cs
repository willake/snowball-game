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

        [Serializable]
        public class UIAudios
        {
            [FormerlySerializedAs("ButtonClick")] public WrappedAudioClip buttonClick;
        }
    }
}