using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay
{
    public abstract class Gun : Weapon
    {
        public float distanceLimit = 10f;
        public float shootingDelay = 0.7f;
        [Range(0f, 100f)]
        public float accuracy = 90f;
        protected float _lastShootTime = 0.0f;

        [Header("VFX")]
        public ParticleSystem muzzleFlash;
        public GameObject trailPrefab;
        public int trailPoolSize = 20;
        private Queue<TrailRenderer> _trailPoool;

        private void Start()
        {
            _trailPoool = new Queue<TrailRenderer>();

            for (int i = 0; i < trailPoolSize; i++)
            {
                GameObject obj = Instantiate(
                    trailPrefab, transform.position, Quaternion.identity, transform);
                TrailRenderer trail = obj.GetComponent<TrailRenderer>();
                obj.SetActive(false);
                _trailPoool.Enqueue(trail);
            }
        }

        public override void Attack(Vector3 direction)
        {
            if (Time.time - _lastShootTime < shootingDelay)
            {
                return;
            }

            _lastShootTime = Time.time;

            muzzleFlash.Play();

            RaycastHit hit;

            TrailRenderer trail = PopTrailInstance();
            trail.transform.position = transform.position;

            Vector3 shootDirection = DirectionConcernAccuracy(direction);

            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, shootDirection, out hit, distanceLimit))
            {
                Debug.DrawRay(transform.position, shootDirection * (hit.point - transform.position).magnitude, Color.yellow, 5);

                StartCoroutine(SpawnTrailHit(trail, hit.point, hit.normal));
            }
            else
            {
                Debug.DrawRay(transform.position, shootDirection * 100, Color.green, 5);
                Vector3 endPoint = transform.position + shootDirection * distanceLimit;
                StartCoroutine(SpawnTrailNotHit(trail, endPoint));
            }
        }

        private IEnumerator SpawnTrailNotHit(TrailRenderer trail, Vector3 endPoint)
        {
            float time = 0;
            Vector3 startPosition = trail.transform.position;

            while (time < 1)
            {
                trail.transform.position = Vector3.Lerp(startPosition, endPoint, time);
                time += Time.deltaTime / trail.time;

                yield return null;
            }

            trail.transform.position = endPoint;
            ReturnTrailInstance(trail);
        }

        private IEnumerator SpawnTrailHit(TrailRenderer trail, Vector3 hitPoint, Vector3 hotNormal)
        {
            float time = 0;
            Vector3 startPosition = trail.transform.position;

            while (time < 1)
            {
                trail.transform.position = Vector3.Lerp(startPosition, hitPoint, time);
                time += Time.deltaTime / trail.time;

                yield return null;
            }

            trail.transform.position = hitPoint;
            ReturnTrailInstance(trail);

            var emitParams = new ParticleSystem.EmitParams
            {
                position = hitPoint,
                rotation3D = Quaternion.LookRotation(hotNormal).eulerAngles
            };
            Debug.Log($"Hitnormal: {hotNormal} LookRotation: {Quaternion.LookRotation(hotNormal).eulerAngles}");
            MainGameScene.instance.vfxManager.hitEffectParticle.Emit(emitParams, 1);
        }

        private void ReturnTrailInstance(TrailRenderer trail)
        {
            _trailPoool.Enqueue(trail);
            trail.gameObject.SetActive(false);
        }

        private TrailRenderer PopTrailInstance()
        {
            if (_trailPoool.Count > 0)
            {
                TrailRenderer trail = _trailPoool.Dequeue();
                trail.gameObject.SetActive(true);
                return trail;
            }
            else
            {
                Debug.LogWarning("Trail pool inadequate");
                return null;
            }
        }

        private Vector3 DirectionConcernAccuracy(Vector3 direction)
        {
            direction += new Vector3(
                Random.Range(-0.1f, 0.1f),
                0,
                Random.Range(-0.1f, 0.1f)
            );
            return direction;
        }
    }
}