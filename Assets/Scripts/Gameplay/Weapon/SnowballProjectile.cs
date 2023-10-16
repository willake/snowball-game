using System.Collections;
using Game.Audios;
using UnityEngine;
using UnityEngine.Events;
using Game.Events;
using System;
using Game.Saves;

namespace Game.Gameplay
{
    public class SnowballProjectile : MonoBehaviour
    {
        private Lazy<EventManager> _eventManager = new Lazy<EventManager>(
            () => DIContainer.instance.GetObject<EventManager>(),
            true
        );
        protected EventManager EventManager { get => _eventManager.Value; }

        [Header("References")]
        public GameObject trail;
        [Header("Settings")]
        public float damage = 50f;
        public float criticalDamage = 100f;
        public float autoDisabledInSeconds = 2f;
        public Camp OwnerCamp { get; private set; }
        public OnHitEvent onHitEvent = new();

        private Rigidbody _rig;
        private Collider _collider;
        private bool _isCritical;
        private Vector3 _throwPosition;
        private float _energyInPercentage;

        public void SetEnergy(float energy)
        {
            _energyInPercentage = energy / 100f;
        }

        public void SetThrowPosition(Vector3 position)
        {
            _throwPosition = position;
            _throwPosition.y = 0;
        }

        public void SetIsCritical(bool isCritical)
        {
            _isCritical = isCritical;
        }

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
            bool isHit = false;
            bool isDamaged = false;
            bool isKill = false;
            Character character = null;
            if (
                OwnerCamp == Camp.Player
                && other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                character = other.gameObject.GetComponent<Character>();
                if (character != null && character.State.isDead == false)
                {
                    isDamaged = character.TakeDamage(
                        _isCritical ? criticalDamage : damage, GetRigidbody().velocity);
                    isKill = isDamaged && character.State.isDead;
                    isHit = true;
                }
            }

            if (OwnerCamp == Camp.Enemy
                && other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                character = other.gameObject.GetComponent<Character>();
                if (character != null && character.State.isDead == false)
                {
                    isDamaged = character.TakeDamage(
                        _isCritical ? criticalDamage : damage, GetRigidbody().velocity);
                    isKill = isDamaged && character.State.isDead;
                    isHit = true;
                }
            }

            if (!isHit && (other.gameObject.layer == LayerMask.NameToLayer("Enemy") ||
                other.gameObject.layer == LayerMask.NameToLayer("Player")))
            {
                return;
            }

            if (OwnerCamp == Camp.Player)
            {
                Vector3 hitPosition = transform.position;
                hitPosition.y = 0;
                float distance = (_throwPosition - hitPosition).magnitude;
                EventManager.Publish(
                    EventNames.onPlayerBallHit,
                    new Payload()
                    {
                        args = new object[] { new GameStatisticsDataV1.ThrownBall
                            {
                                hitDistance = distance,
                                energy = _energyInPercentage,
                                isCritical = _isCritical,
                                isHitEnemy = isHit,
                                isKillEnemy = isKill
                            }
                        }
                    }
                );
            }

            if (isHit)
            {
                WrappedAudioClip audioClip = ResourceManager.instance.audioResources.gameplayAudios.snowballHit;
                AudioManager.instance?.PlaySFX(
                    audioClip.clip,
                    audioClip.volume,
                    UnityEngine.Random.Range(0.6f, 1.2f)
                );
            }
            else
            {
                WrappedAudioClip audioClip = ResourceManager.instance.audioResources.gameplayAudios.snowballNotHit;
                AudioManager.instance?.PlaySFX(
                    audioClip.clip,
                    audioClip.volume,
                    UnityEngine.Random.Range(0.6f, 1.2f)
                );
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