using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Game.Gameplay
{
    public class CharacterAnimatior : MonoBehaviour
    {
        private Animator _animator;

        [Header("Settings")]
        public float throwingAnimationTime = 0.5f;

        public UnityEvent thorwEndedEvent = new();

        private Coroutine _throwCoroutine = null;


        private Animator GetAnimator()
        {
            if (_animator == null) _animator = GetComponent<Animator>();

            return _animator;
        }

        public void SetIsAiming(bool isAiming)
        {
            GetAnimator().SetBool("isAiming", isAiming);
        }

        public void SetMoveSpeed(float horizontal, float vertical, float speed)
        {
            GetAnimator().SetFloat("Horizontal", horizontal);
            GetAnimator().SetFloat("Vertical", vertical);
            GetAnimator().SetFloat("Speed", speed);
        }

        public void TriggerThrow()
        {
            GetAnimator().SetTrigger("Throw");

            if (_throwCoroutine != null)
            {
                StopCoroutine(_throwCoroutine);
                _throwCoroutine = null;
            }
            _throwCoroutine = StartCoroutine(CountDownThrowAnimation());
        }

        public void TriggerDamage()
        {
            GetAnimator().SetTrigger("Damage");
        }

        public void TriggerDead()
        {
            GetAnimator().SetTrigger("Dead");
        }

        IEnumerator CountDownThrowAnimation()
        {
            yield return new WaitForSecondsRealtime(throwingAnimationTime);
            thorwEndedEvent.Invoke();
        }

        private void OnDestroy()
        {
            thorwEndedEvent.RemoveAllListeners();
        }
    }
}