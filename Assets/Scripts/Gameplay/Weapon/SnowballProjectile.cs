using System.Collections;
using UnityEngine;

namespace Game.Gameplay
{
    public class SnowballProjectile : MonoBehaviour
    {
        [Header("Settings")]
        public float damage = 50f;
        public float autoDisabledInSeconds = 5f;
        public Camp OwnerCamp { get; private set; }

        public void SetOwnerCamp(Camp camp)
        {
            OwnerCamp = camp;
        }

        public void Start()
        {
            Debug.Log("F");
        }

        public void Awake()
        {
            Debug.Log("A");
        }
        public void Shot()
        {
            Awake();
            StartCoroutine(AutoDisabled());
        }

        private void OnCollisionEnter(Collision other)
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

            gameObject.SetActive(false);

            // play hit effect
        }

        IEnumerator AutoDisabled()
        {
            yield return new WaitForSeconds(autoDisabledInSeconds);

            gameObject.SetActive(false);
        }
    }
}