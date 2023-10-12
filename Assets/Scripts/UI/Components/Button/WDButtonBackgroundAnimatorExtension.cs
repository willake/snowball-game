using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Game.UI
{
    public class WDButtonBackgroundAnimatiorExtension : ButtonAnimatorExtension
    {
        [Header("Settings")]
        public Image background;
        public Color idleColor = Color.white;
        public Color hoverColor = Color.white;

        public override Tween OnIdle(float animationTime)
        {
            return background.DOColor(idleColor, animationTime);
        }

        public override Tween OnHover(float animationTime)
        {
            return background.DOColor(hoverColor, animationTime);
        }

        public override Tween OnClick(float animationTime)
        {
            return background.DOColor(idleColor, animationTime);
        }
    }
}