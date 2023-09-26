using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Game.Gameplay
{
    public class WeaponHolder : MonoBehaviour
    {
        [Header("References")]
        public Transform socket;

        [Header("Settings")]
        public bool isDebugLogEnabled = false;
        [Range(0f, 30f)]
        public float throwingPitch = 10f;
        public float minEnergy = 0f;
        public float maxEnergy = 10f;
        public float chargeIntervalInSeconds = 0.01f;
        public float energyPerInterval = 0.1f;
        public float timeOutEnergy = 0.1f;

        public Vector3 AimDirection { get; private set; }
        public float Energy { get; private set; }

        [Header("Property")]
        public Camp ownerCamp;
        public Weapon holdingWeapon;

        private Coroutine _runningCorotine = null;

        public void UpdateAimDirection(Vector3 direction)
        {
            AimDirection = direction;
        }

        public void EquipWeapon(Weapon weapon)
        {
            holdingWeapon = weapon;
            weapon.transform.SetParent(socket);
            weapon.transform.position = socket.position;
            weapon.SetOwnerCamp(ownerCamp);
        }

        public void Hold()
        {
            Energy = 0;
            Snowball snowball = holdingWeapon as Snowball;
            snowball.Hold();
            _runningCorotine = StartCoroutine(ChargeEnergy());
        }

        public void Throw()
        {
            if (_runningCorotine != null)
            {
                StopCoroutine(_runningCorotine);
            }

            _runningCorotine = null;

            float pitch = throwingPitch * Mathf.Deg2Rad;

            Vector3 shootDirection = new Vector3(
                AimDirection.x * Mathf.Cos(pitch),
                Mathf.Sin(pitch),
                AimDirection.z * Mathf.Cos(pitch)
            );
            holdingWeapon.Attack(shootDirection.normalized, Energy);
        }

        public void Reload()
        {
            if (holdingWeapon.weaponType == WeaponType.Snowball)
            {
                holdingWeapon.Reload();
            }
        }

        IEnumerator ChargeEnergy()
        {
            while (Energy < maxEnergy)
            {
                yield return new WaitForSeconds(chargeIntervalInSeconds);
                Energy += energyPerInterval;
                if (isDebugLogEnabled)
                {
                    Debug.Log($"Energy increase {energyPerInterval} to {Energy}");
                }
            }

            yield return new WaitForSeconds(0.1f);

            while (Energy > minEnergy)
            {
                yield return new WaitForSeconds(chargeIntervalInSeconds);
                Energy -= energyPerInterval;
                if (isDebugLogEnabled)
                {
                    Debug.Log($"Energy decrease {energyPerInterval} to {Energy}");
                }
            }

            Energy = timeOutEnergy;

            Throw();
            if (isDebugLogEnabled)
            {
                Debug.Log($"Time out, throw the ball anyway");
            }
        }
    }
}
