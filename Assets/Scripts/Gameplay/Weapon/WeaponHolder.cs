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
        public float minEnergy = 0f;
        public float maxEnergy = 10f;
        public float chargeIntervalInSeconds = 0.01f;
        public float energyPerInterval = 0.1f;
        public float timeOutEnergy = 0.1f;

        public Vector3 AimDirection { get; private set; }
        public float Energy { get; private set; }

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

            holdingWeapon.Attack(AimDirection, Energy);
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
                Debug.Log($"Energy increase {energyPerInterval} to {Energy}");
            }

            yield return new WaitForSeconds(0.1f);

            while (Energy > minEnergy)
            {
                yield return new WaitForSeconds(chargeIntervalInSeconds);
                Energy -= energyPerInterval;
                Debug.Log($"Energy decrease {energyPerInterval} to {Energy}");
            }

            Energy = timeOutEnergy;

            Throw();
            Debug.Log($"Time out, throw the ball anyway");
        }
    }
}
