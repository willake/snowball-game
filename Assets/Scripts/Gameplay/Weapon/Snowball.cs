using Game.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay
{
    public class Snowball : Weapon
    {
        [Header("Refereces")]
        public GameObject prefabProjectile;
        public GameObject prefabOnHitEffect;

        [Header("Settings")]
        public int maxAmmo = 10;
        public int poolSize = 15;
        public float energyMultiplier = 10f;
        public LayerMask projectileLayer;
        public int Ammo { get; private set; }
        private GameObject _poolObj;
        private Queue<SnowballProjectile> _projectilePool;
        private Queue<ParticleSystem> _onHitEffectPool;
        private SnowballProjectile _loadedProjectile;

        public bool isLoaded { get; private set; }

        private void Start()
        {
            _projectilePool = new Queue<SnowballProjectile>();
            _onHitEffectPool = new Queue<ParticleSystem>();
            _poolObj = new GameObject
            {
                name = "SnowballPool"
            };
            for (int i = 0; i < poolSize; i++)
            {
                GameObject projectileObj =
                    Instantiate(
                        prefabProjectile, transform.position, Quaternion.identity, _poolObj.transform);
                projectileObj.SetActive(false);
                projectileObj.layer = this.gameObject.layer;
                SnowballProjectile projectile =
                    projectileObj.GetComponent<SnowballProjectile>();
                projectile.SetOwnerCamp(ownerCamp);
                _projectilePool.Enqueue(projectile);

                projectile.onHitEvent.AddListener(PlayOnHitEffect);

                GameObject hitEffectObj =
                    Instantiate(
                        prefabOnHitEffect, transform.position, Quaternion.identity, _poolObj.transform);
                hitEffectObj.SetActive(false);
                hitEffectObj.layer = this.gameObject.layer;
                ParticleSystem particle = hitEffectObj.GetComponent<ParticleSystem>();
                _onHitEffectPool.Enqueue(particle);
            }
            Reset();
        }

        public void Reset()
        {
            Ammo = maxAmmo;
        }

        public void Load()
        {
            if (Ammo <= 0) return;
            if (isLoaded) return;

            // pop a snowball
            _loadedProjectile = _projectilePool.Dequeue();
            _loadedProjectile.transform.position = this.transform.position;
            // _holdingProjectile.gameObject.layer = projectileLayer;
            _loadedProjectile.GetRigidbody().velocity = Vector3.zero;
            _loadedProjectile.GetRigidbody().isKinematic = true;
            _loadedProjectile.GetCollider().enabled = false;
            _loadedProjectile.EnableTrail(false);
            _loadedProjectile.gameObject.SetActive(true);

            isLoaded = true;
            Ammo -= 1;
        }

        public override bool Attack(Vector3 direction, float energy, bool isCritical)
        {
            if (isLoaded == false) return false;

            _loadedProjectile.SetThrowPosition(transform.position);
            _loadedProjectile.SetEnergy(energy);
            _loadedProjectile.SetIsCritical(isCritical);
            _loadedProjectile.EnableTrail(true);
            _loadedProjectile.GetRigidbody().isKinematic = false;
            _loadedProjectile.GetCollider().enabled = true;
            _loadedProjectile.GetRigidbody().AddForce(
                direction * energy * energyMultiplier, ForceMode.Impulse);

            // activate auto disabled

            _projectilePool.Enqueue(_loadedProjectile);
            _loadedProjectile = null;
            isLoaded = false;

            return true;
        }

        public override void Reload()
        {
            if (Ammo >= maxAmmo) return;

            Ammo = maxAmmo;
        }

        public void PlayOnHitEffect(Vector3 position, Vector3 velocity)
        {
            ParticleSystem onHitEffect = _onHitEffectPool.Dequeue();
            onHitEffect.transform.position = position;
            onHitEffect.gameObject.SetActive(true);
            onHitEffect.Play();
            if (velocity.magnitude > float.Epsilon)
            {
                onHitEffect.transform.rotation = Quaternion.LookRotation(velocity.normalized * -1);
            }
            _onHitEffectPool.Enqueue(onHitEffect);
        }

        private void Update()
        {
            if (_loadedProjectile)
            {
                _loadedProjectile.transform.position = this.transform.position;
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < poolSize; i++)
            {
                SnowballProjectile projectile = _projectilePool.Dequeue();

                projectile.onHitEvent.RemoveAllListeners();
            }
            _projectilePool.Clear();
            Destroy(_poolObj);
        }
    }
}