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
        public float damageAnimationTime = 0.5f;

        public UnityEvent thorwEndedEvent = new();
        public UnityEvent damageEndedEvent = new();

        private Coroutine _throwCoroutine = null;
        private Coroutine _damageCoroutine = null;


        private Animator GetAnimator()
        {
            if (_animator == null) _animator = GetComponent<Animator>();

            return _animator;
        }

        public void SetIsAiming(bool isAiming)
        {
            GetAnimator().SetBool("isAiming", isAiming);
        }

        public void SetIsReloading(bool isReloading)
        {
            if (isReloading)
            {
                GetAnimator().SetTrigger("ReloadStart");
                GetAnimator().ResetTrigger("ReloadEnd");
            }
            else
            {
                GetAnimator().SetTrigger("ReloadEnd");
                GetAnimator().ResetTrigger("ReloadStart");
            }
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

            if (_damageCoroutine != null)
            {
                StopCoroutine(_damageCoroutine);
                _damageCoroutine = null;
            }
            _damageCoroutine = StartCoroutine(CountDownDamageAnimation());
        }

        public void TriggerDead()
        {
            GetAnimator().SetTrigger("Dead");
        }

        public void TriggerRevive()
        {
            GetAnimator().SetTrigger("Revive");
        }

        IEnumerator CountDownThrowAnimation()
        {
            yield return new WaitForSecondsRealtime(throwingAnimationTime);
            thorwEndedEvent.Invoke();
        }

        IEnumerator CountDownDamageAnimation()
        {
            yield return new WaitForSecondsRealtime(damageAnimationTime);
            damageEndedEvent.Invoke();
        }

        private void OnDestroy()
        {
            thorwEndedEvent.RemoveAllListeners();
        }
    }
}