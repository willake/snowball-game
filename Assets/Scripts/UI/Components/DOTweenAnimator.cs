using UnityEngine;
using DG.Tweening;

namespace Game.UI
{
    public abstract class DoTweenAnimator<S, T> : WDAnimator<S> where S : System.Enum where T : MonoBehaviour
    {
        protected T _body;
        protected Tween _tween;

        public void Stop()
        {
            if (_tween.IsActive())
            {
                _tween.Kill();
            }
        }

        protected T GetBody()
        {
            if (_body == null) _body = GetComponent<T>();

            return _body;
        }

        public void SetBody(T body)
        {
            _body = body;
        }

        public override void SetState(S state, bool playAnimation = true)
        {
            if (_tween.IsActive())
            {
                _tween.Kill();
            }
            _tween = GetAnimationTween(state);
            _tween.Play();
        }

        private void OnDestroy()
        {
            if (_tween.IsActive())
            {
                _tween.Kill();
            }
        }

        protected abstract Tween GetAnimationTween(S state, bool playAnimation = true);
    }
}