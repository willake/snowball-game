using UnityEngine;
using DG.Tweening;

namespace Game.UI
{
    public enum WDButtonState
    {
        Idle,
        Hover,
        Click
    }
    public class WDButtonAnimator : DoTweenAnimator<WDButtonState, WDButton>
    {
        private ButtonAnimatorExtension[] _extensions;
        [Header("Settings")]
        public float scale = 1.1f;

        private void Start()
        {
            _extensions = GetComponents<ButtonAnimatorExtension>();
        }
        protected override Tween GetAnimationTween(WDButtonState state, bool playAnimation = true)
        {
            float animationTime = playAnimation ? 0.2f : 0f;
            Sequence sequence = DOTween.Sequence();
            switch (state)
            {
                case WDButtonState.Idle:
                    sequence.Append(
                        GetBody().transform.DOScale(1f, animationTime)
                    );
                    foreach (var ext in _extensions)
                    {
                        sequence.Join(ext.OnIdle(animationTime));
                    }
                    break;
                case WDButtonState.Hover:
                    sequence.Append(
                        GetBody().transform.DOScale(scale, animationTime)
                    );
                    foreach (var ext in _extensions)
                    {
                        sequence.Join(ext.OnHover(animationTime));
                    }
                    break;
                case WDButtonState.Click:
                    sequence.Append(
                        GetBody().transform.DOScale(1f, animationTime)
                    );
                    foreach (var ext in _extensions)
                    {
                        sequence.Join(ext.OnClick(animationTime));
                    }
                    break;
                default:
                    break;
            }

            sequence.SetUpdate(true);

            return sequence;
        }
    }
}