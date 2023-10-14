using System.Collections;
using Game.Audios;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Gameplay
{
    public class SnowballProjectile : MonoBehaviour
    {
        [Header("References")]
        public GameObject trail;
        [Header("Settings")]
        public float damage = 50f;
        public float autoDisabledInSeconds = 2f;
        public Camp OwnerCamp { get; private set; }
        public OnHitEvent onHitEvent = new();

        private Rigidbody _rig;
        private Collider _collider;

        public void SetOwnerCamp(Camp camp)
        {
            OwnerCamp = camp;
        }

        public void EnableTrail(bool enabled)
        {
            trail.SetActive(enabled);
        }

        private void OnTriggerEnter(Collider other)
        {
            bool hit = false;
            if (
                OwnerCamp == Camp.Player
                && other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                other.gameObject.GetComponent<Character>().TakeDamage(
                    damage, GetRigidbody().velocity);
                hit = true;
            }

            if (OwnerCamp == Camp.Enemy
                && other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                other.gameObject.GetComponent<Character>().TakeDamage(
                    damage, GetRigidbody().velocity);
                hit = true;
            }

            if (!hit && (other.gameObject.layer == LayerMask.NameToLayer("Enemy") ||
                other.gameObject.layer == LayerMask.NameToLayer("Player")))
            {
                return;
            }

            WrappedAudioClip audioClip = ResourceManager.instance.audioResources.gameplayAudios.snowballHit;
            AudioManager.instance?.PlaySFX(
                audioClip.clip,
                audioClip.volume,
                Random.Range(0.8f, 1.2f)
            );
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

        public Rigidbody GetRigidbody()
        {
            if (_rig == null) _rig = GetComponent<Rigidbody>();

            return _rig;
        }

        public Collider GetCollider()
        {
            if (_collider == null) _collider = GetComponent<Collider>();
            return _collider;
        }


        public class OnHitEvent : UnityEvent<Vector3, Vector3> { }
    }
}