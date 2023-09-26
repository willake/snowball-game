using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay
{
    public class Snowball : Weapon
    {
        [Header("Refereces")]
        public GameObject prefabSnowballBullet;

        [Header("Settings")]
        public int maxAmmo = 10;
        public int poolSize = 20;
        public float energyMultiplier = 10f;
        public int Ammo { get; private set; }
        private GameObject _snowballPoolObj;
        private Queue<GameObject> _snowballPool;
        private GameObject _holdingProjectile;

        private void Start()
        {
            _snowballPool = new Queue<GameObject>();
            _snowballPoolObj = new GameObject();
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj =
                    Instantiate(
                        prefabSnowballBullet, transform.position, Quaternion.identity, _snowballPoolObj.transform);
                obj.SetActive(false);
                obj.layer = this.gameObject.layer;
                _snowballPool.Enqueue(obj);
            }
            Ammo = maxAmmo;
        }

        public void Hold()
        {
            if (Ammo <= 0) return;

            // pop a snowball
            _holdingProjectile = _snowballPool.Dequeue();
            _holdingProjectile.SetActive(true);
            _holdingProjectile.transform.position = this.transform.position;
            _holdingProjectile.gameObject.layer = GetOwnerCampLayer();
            Rigidbody rig = _holdingProjectile.GetComponent<Rigidbody>();
            rig.useGravity = false;
        }

        public override void Attack(Vector3 direction, float energy)
        {
            if (Ammo <= 0 || _holdingProjectile == null) return;

            // TODO: snowball mechanic
            Rigidbody rig = _holdingProjectile.GetComponent<Rigidbody>();
            rig.useGravity = true;
            rig.AddForce(direction * energy * energyMultiplier, ForceMode.Impulse);

            // activate auto disabled
            _holdingProjectile.GetComponent<SnowballProjectile>().Shot();

            _snowballPool.Enqueue(_holdingProjectile);
            _holdingProjectile = null;
        }

        public override bool Reload()
        {
            if (Ammo >= maxAmmo) return false;

            // TODO: add reload time and play animation

            Ammo = maxAmmo;
            return true;
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
            _snowballPool.Clear();
            Destroy(_snowballPoolObj);
        }
    }
}