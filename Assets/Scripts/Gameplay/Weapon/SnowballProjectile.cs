using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Gameplay
{
    public class SnowballProjectile : MonoBehaviour
    {
        [Header("Settings")]
        public float damage = 50f;
        public float autoDisabledInSeconds = 2f;
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

        private void Update()
        {
            if (transform.position.y < -20)
            {
                gameObject.SetActive(false);
            }
        }

        public class OnHitEvent : UnityEvent<Vector3, Vector3> { }
    }
}