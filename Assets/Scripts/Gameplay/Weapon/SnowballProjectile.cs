using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Gameplay
{
    public class SnowballProjectile : MonoBehaviour
    {
        [Header("Settings")]
        public float damage = 50f;
        public float autoDisabledInSeconds = 5f;
        public Camp OwnerCamp { get; private set; }
        public OnHitEvent onHitEvent = new();

        private Rigidbody _rig;

        public Rigidbody GetRigidbody()
        {
            if (_rig == null) _rig = GetComponent<Rigidbody>();

            return _rig;
        }

        public void SetOwnerCamp(Camp camp)
        {
            OwnerCamp = camp;
        }

        public void Shot()
        {
            StartCoroutine(AutoDisabled());
        }

        private void OnTriggerEnter(Collider other)
        {
            if (
                OwnerCamp == Camp.Player
                && other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                other.gameObject.GetComponent<Character>().TakeDamage(damage);
            }

            if (OwnerCamp == Camp.Enemy
                && other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                other.gameObject.GetComponent<Character>().TakeDamage(damage);
            }

            onHitEvent.Invoke(transform.position, GetRigidbody().velocity);
            GetRigidbody().velocity = Vector3.zero;
            gameObject.SetActive(false);

            // play hit effect

        }

        IEnumerator AutoDisabled()
        {
            yield return new WaitForSeconds(autoDisabledInSeconds);
            GetRigidbody().velocity = Vector3.zero;
            gameObject.SetActive(false);
        }

        public class OnHitEvent : UnityEvent<Vector3, Vector3> { }
    }
}