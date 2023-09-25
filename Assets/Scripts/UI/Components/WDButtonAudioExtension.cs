using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Game.Audios;

namespace Game.UI
{
    public class WDButtonAudioExtension : MonoBehaviour
    {
        private WDButton button;

        private void Start()
        {
            if (button == null)
            {
                button = GetComponent<WDButton>();
            }

            button
                .OnClickObservable
                .ObserveOnMainThread()
                .Subscribe(_ => PlayAudio())
                .AddTo(this);
        }

        protected void PlayAudio()
        {
            WrappedAudioClip wrappedClip = ResourceManager.instance
                .audioResources.uiAudios.buttonClick;

            AudioManager.instance.PlaySfx(
                wrappedClip.clip,
                wrappedClip.volume
            );
        }
    }
}