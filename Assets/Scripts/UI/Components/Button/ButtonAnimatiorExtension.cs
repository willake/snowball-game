using DG.Tweening;
using UnityEngine;

namespace Game.UI
{
    public abstract class ButtonAnimatorExtension : MonoBehaviour
    {
        public abstract Tween OnIdle(float animationTime);
        public abstract Tween OnHover(float animationTime);
        public abstract Tween OnClick(float animationTime);
    }
}