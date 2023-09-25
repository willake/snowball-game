using UnityEngine;

namespace Game.UI
{
    public interface IWDAnimator<T> where T : System.Enum
    {
        void SetState(T state, bool playAnimation);
    }

    public abstract class WDAnimator<T> : MonoBehaviour, IWDAnimator<T> where T : System.Enum
    {
        private T _state { get; set; }

        public T GetState() { return _state; }
        public abstract void SetState(T state, bool playAnimation = true);
    }
}