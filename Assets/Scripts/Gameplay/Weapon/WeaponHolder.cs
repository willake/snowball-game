using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;

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

        public int Ammo
        {
            get
            {
                if (holdingWeapon)
                {
                    return holdingWeapon.Ammo;
                }

                return 0;
            }
        }
        public float Energy { get; private set; }

        public UnityEvent holdEvent = new();
        public UnityEvent throwEvent = new();
        public EnergyUpdateEvent energyUpdateEvent = new();
        public AmmoUpdateEvent ammoUpdateEvent = new();

        [Header("Property")]
        public Camp ownerCamp;
        public Snowball holdingWeapon;

        private Coroutine _runningCorotine = null;

        public void UpdateAimDirection(Vector3 direction)
        {
            AimDirection = direction;
        }

        public void Hold()
        {
            Energy = 0;
            holdingWeapon.Hold();
            _runningCorotine = StartCoroutine(ChargeEnergy());
            holdEvent.Invoke();
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

            if (holdingWeapon.Attack(shootDirection.normalized, Energy))
            {
                throwEvent.Invoke();
                ammoUpdateEvent.Invoke(Ammo);
            }
        }

        // for enemy AI
        public void ThrowWithoutCharging(float energy)
        {
            holdingWeapon.Hold();

            float pitch = throwingPitch * Mathf.Deg2Rad;

            Vector3 shootDirection = new Vector3(
                AimDirection.x * Mathf.Cos(pitch),
                Mathf.Sin(pitch),
                AimDirection.z * Mathf.Cos(pitch)
            );

            energy = Mathf.Clamp(energy, minEnergy, maxEnergy);

            holdingWeapon.Attack(shootDirection.normalized, energy);
            throwEvent.Invoke();
        }

        public void Reload()
        {
            bool success = holdingWeapon.Reload();

            if (success)
            {
                ammoUpdateEvent.Invoke(holdingWeapon.maxAmmo);
            }
        }

        IEnumerator ChargeEnergy()
        {
            while (Energy < maxEnergy)
            {
                yield return new WaitForSeconds(chargeIntervalInSeconds);
                Energy += energyPerInterval;
                energyUpdateEvent.Invoke(Energy / maxEnergy);
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
                energyUpdateEvent.Invoke(Energy / maxEnergy);
                if (isDebugLogEnabled)
                {
                    Debug.Log($"Energy decrease {energyPerInterval} to {Energy}");
                }
            }

            Energy = timeOutEnergy;
            energyUpdateEvent.Invoke(Energy / maxEnergy);

            Throw();
            if (isDebugLogEnabled)
            {
                Debug.Log($"Time out, throw the ball anyway");
            }
        }

        private void OnDestroy()
        {
            holdEvent.RemoveAllListeners();
            throwEvent.RemoveAllListeners();
            energyUpdateEvent.RemoveAllListeners();
            ammoUpdateEvent.RemoveAllListeners();
        }

        public class EnergyUpdateEvent : UnityEvent<float> { }
        // ammo / maxAmmo
        public class AmmoUpdateEvent : UnityEvent<int> { }
    }
}
