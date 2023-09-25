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
                    break;
                case WDButtonState.Hover:
                    sequence.Append(
                        GetBody().transform.DOScale(1.2f, animationTime)
                    );
                    break;
                case WDButtonState.Click:
                    sequence.Append(
                        GetBody().transform.DOScale(1f, animationTime)
                    );
                    break;
                default:
                    break;
            }

            sequence.SetUpdate(true);

            return sequence;
        }
    }
}