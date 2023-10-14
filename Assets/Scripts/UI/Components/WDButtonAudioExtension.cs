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

        [Header("Settings")]
        public ButtonType buttonType = ButtonType.Regular;

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
            WrappedAudioClip wrappedClip;

            switch (buttonType)
            {
                case ButtonType.Regular:
                default:
                    wrappedClip = ResourceManager.instance
                        .audioResources.uiAudios.buttonClick;
                    break;
                case ButtonType.Confirm:
                    wrappedClip = ResourceManager.instance
                        .audioResources.uiAudios.buttonConfirm;
                    break;
            }

            AudioManager.instance.PlaySFX(
                wrappedClip.clip,
                wrappedClip.volume
            );
        }

        public enum ButtonType
        {
            Regular,
            Confirm
        }
    }
}