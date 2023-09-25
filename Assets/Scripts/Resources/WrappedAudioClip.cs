using UnityEngine;
using Sirenix.OdinInspector;

namespace Game
{
    [System.Serializable]
    public class WrappedAudioClip
    {
        public AudioClip clip;
        [PropertyRange(0f, 1f)]
        public float volume = 1;
    }
}