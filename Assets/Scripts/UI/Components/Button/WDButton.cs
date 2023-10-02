using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using System;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

namespace Game.UI
{
    public class WDButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// Whether play animation when being cliked  
        /// </summary>
        public bool playAnimation = true;
        public WDButtonAnimator animator;

        [SerializeField]
        private bool _isInteractable = true;
        public bool IsInteractable { get => _isInteractable; }
        private bool _isHovered;

        /// <summary>
        /// A unity event invoke when the button is clicked  
        /// </summary>
        public OnClickEvent onClick = new OnClickEvent();

        /// <summary>
        /// A unity event invoke when the button is clicked. Works with UniRx
        /// </summary>
        public IObservable<Unit> OnClickObservable
        {
            get => onClick
            .AsObservable().ThrottleFirst(Consts.THROTTTLE_IN_SCEOND);
        }

        private void Start()
        {
            animator.SetBody(this);
        }

        public void StopAnimation()
        {
            animator.Stop();
        }

        public void SetIsInteractable(bool interactable)
        {
            _isInteractable = interactable;
        }

        public void OnPointerEnter(PointerEventData e)
        {
            if (_isInteractable == false) return;
            _isHovered = true;

            if (playAnimation)
            {
                animator.SetState(WDButtonState.Hover);
            }
        }

        public void OnPointerExit(PointerEventData e)
        {
            if (_isInteractable == false) return;
            _isHovered = false;

            if (playAnimation)
            {
                animator.SetState(WDButtonState.Idle);
            }
        }

        public void OnPointerUp(PointerEventData e)
        {
            if (_isInteractable == false) return;

            if (playAnimation)
            {
                if (_isHovered)
                {
                    animator.SetState(WDButtonState.Hover);
                }
                else
                {
                    animator.SetState(WDButtonState.Idle);
                }
            }

            onClick.Invoke();
        }

        public void OnPointerDown(PointerEventData e)
        {
            if (_isInteractable == false) return;
            animator.SetState(WDButtonState.Click);
        }

        public void Deselect()
        {
            animator.SetState(WDButtonState.Idle);
        }

        public void Select()
        {
            animator.SetState(WDButtonState.Hover);
        }

        public void Click()
        {
            onClick.Invoke();
        }

        private void OnDestroy()
        {
            animator.Stop();
        }

        public class OnClickEvent : UnityEvent { }
    }

    public static class WDButtonExtension
    {
        /// <summary>
        /// A UniTask version of on click event. It can be used in tutorial scripts while making tutorials.  
        /// </summary>
        public static UniTask OnClickAsync(this WDButton button)
        {
            return new AsyncUnityEventHandler(button.onClick, button.GetCancellationTokenOnDestroy(), true).OnInvokeAsync();
        }
    }
}