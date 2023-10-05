using UnityEngine;

namespace Game.Gameplay
{
    public class CharacterAnimatior : MonoBehaviour
    {
        private Animator _animator;

        private Animator GetAnimator()
        {
            if (_animator == null) _animator = GetComponent<Animator>();

            return _animator;
        }

        public void SetMoveSpeed(float speed)
        {
            GetAnimator().SetFloat("Speed", speed);
        }

        public void TriggerThrow()
        {
            GetAnimator().SetTrigger("Throw");
        }

        public void TriggerDamage()
        {
            GetAnimator().SetTrigger("Damage");
        }

        public void TriggerDead()
        {
            GetAnimator().SetTrigger("Dead");
        }
    }
}