using UnityEngine;
using DG.Tweening;
using TMPro;

namespace Game.UI
{
    public class WDButtonColorAnimatiorExtension : ButtonAnimatorExtension
    {
        [Header("Settings")]
        public TextMeshProUGUI textMesh;
        public Color idleColor = Color.white;
        public Color hoverColor = Color.white;

        public override Tween OnIdle(float animationTime)
        {
            return textMesh.DOColor(idleColor, animationTime);
        }

        public override Tween OnHover(float animationTime)
        {
            return textMesh.DOColor(hoverColor, animationTime);
        }

        public override Tween OnClick(float animationTime)
        {
            return textMesh.DOColor(idleColor, animationTime);
        }
    }
}