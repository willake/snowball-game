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
        private SnowballProjectile _holdingProjectile;

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
            Ammo = maxAmmo;
        }

        public void Hold()
        {
            if (Ammo <= 0) return;

            // pop a snowball
            _holdingProjectile = _projectilePool.Dequeue();
            _holdingProjectile.transform.position = this.transform.position;
            // _holdingProjectile.gameObject.layer = projectileLayer;
            _holdingProjectile.GetRigidbody().velocity = Vector3.zero;
            _holdingProjectile.GetRigidbody().useGravity = false;
            _holdingProjectile.gameObject.SetActive(true);
        }

        public override void Attack(Vector3 direction, float energy)
        {
            Debug.Log("FUCK: " + energy);
            Debug.Log("DIR:" + direction);
            if (Ammo <= 0 || _holdingProjectile == null) return;

            // TODO: snowball mechanic
            _holdingProjectile.GetRigidbody().useGravity = true;
            _holdingProjectile.GetRigidbody().AddForce(
                direction * energy * energyMultiplier, ForceMode.Impulse);

            // activate auto disabled

            _projectilePool.Enqueue(_holdingProjectile);
            _holdingProjectile = null;
            Ammo -= 1;

        }

        public override bool Reload()
        {
            if (Ammo >= maxAmmo) return false;

            // TODO: add reload time and play animation

            Ammo = maxAmmo;
            return true;
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
            if (_holdingProjectile)
            {
                _holdingProjectile.transform.position = this.transform.position;
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