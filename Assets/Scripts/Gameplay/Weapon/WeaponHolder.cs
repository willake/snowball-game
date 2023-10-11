using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using Game.Gameplay.WeaponHolderStates;

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
        public float reloadIntervalInSeconds = 0.05f;
        public float reloadTimeInSeconds = 1f;

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

        public UnityEvent loadEvent = new();
        public UnityEvent throwEvent = new();
        public EnergyUpdateEvent energyUpdateEvent = new();
        public AmmoUpdateEvent ammoUpdateEvent = new();
        public UnityEvent reloadStartEvent = new();
        public UnityEvent reloadEndEvent = new();
        public ReloadProgressUpdateEvent reloadProgressUpdateEvent = new();

        [Header("Property")]
        public Camp ownerCamp;
        public Snowball holdingWeapon;
        public IWeaponHolderStates State { get; private set; }

        private Coroutine _chargingCorotine = null;
        private Coroutine _reloadingCorotine = null;

        private void Start()
        {
            State = WeaponHolderState.IdleState;
        }

        public void UpdateAimDirection(Vector3 direction)
        {
            AimDirection = direction;
        }

        public bool Aim()
        {
            if (State.shouldReload) return false;

            Energy = 0;
            holdingWeapon.Load();
            _chargingCorotine = StartCoroutine(ChargeEnergy());
            loadEvent.Invoke();
            SetWeaponHolderState(WeaponHolderState.AimState);
            return true;
        }

        public bool Throw()
        {
            if (State.isAiming == false) return false;
            // the ball is already on the hand, so no need to check anything
            if (_chargingCorotine != null)
            {
                StopCoroutine(_chargingCorotine);
            }

            _chargingCorotine = null;

            float pitch = throwingPitch * Mathf.Deg2Rad;

            Vector3 shootDirection = new Vector3(
                AimDirection.x * Mathf.Cos(pitch),
                Mathf.Sin(pitch),
                AimDirection.z * Mathf.Cos(pitch)
            );

            holdingWeapon.Attack(shootDirection.normalized, Energy);

            throwEvent.Invoke();
            ammoUpdateEvent.Invoke(Ammo);

            if (Ammo <= 0) SetWeaponHolderState(WeaponHolderState.NeedReloadState);
            else SetWeaponHolderState(WeaponHolderState.IdleState);

            return true;
        }

        // for enemy AI
        public void ThrowWithoutCharging(float energy)
        {
            if (State.shouldReload) return;

            holdingWeapon.Load();

            float pitch = throwingPitch * Mathf.Deg2Rad;

            Vector3 shootDirection = new Vector3(
                AimDirection.x * Mathf.Cos(pitch),
                Mathf.Sin(pitch),
                AimDirection.z * Mathf.Cos(pitch)
            );

            energy = Mathf.Clamp(energy, minEnergy, maxEnergy);

            holdingWeapon.Attack(shootDirection.normalized, energy);
            throwEvent.Invoke();

            if (Ammo <= 0) SetWeaponHolderState(WeaponHolderState.NeedReloadState);
            else SetWeaponHolderState(WeaponHolderState.IdleState);
        }

        public void Reload()
        {
            if (Ammo >= holdingWeapon.maxAmmo) return;

            _reloadingCorotine = StartCoroutine(StartReload());

            SetWeaponHolderState(WeaponHolderState.ReloadState);
        }

        public void TerminateReload()
        {
            if (_reloadingCorotine != null)
            {
                StopCoroutine(_reloadingCorotine);
            }

            SetWeaponHolderState(WeaponHolderState.IdleState);
            reloadEndEvent.Invoke();
        }

        IEnumerator StartReload()
        {
            reloadStartEvent.Invoke();
            float time = 0;
            while (time < reloadTimeInSeconds)
            {
                yield return new WaitForSeconds(reloadIntervalInSeconds);
                time += reloadIntervalInSeconds;
                reloadProgressUpdateEvent.Invoke(time / reloadTimeInSeconds);
            }

            holdingWeapon.Reload();

            ammoUpdateEvent.Invoke(holdingWeapon.maxAmmo);
            SetWeaponHolderState(WeaponHolderState.IdleState);
            reloadEndEvent.Invoke();
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

        protected void SetWeaponHolderState(IWeaponHolderStates state)
        {
            State = state;
        }

        private void OnDestroy()
        {
            loadEvent.RemoveAllListeners();
            throwEvent.RemoveAllListeners();
            energyUpdateEvent.RemoveAllListeners();
            ammoUpdateEvent.RemoveAllListeners();
            reloadStartEvent.RemoveAllListeners();
            reloadEndEvent.RemoveAllListeners();
            reloadProgressUpdateEvent.RemoveAllListeners();
        }

        public class EnergyUpdateEvent : UnityEvent<float> { }
        public class ReloadProgressUpdateEvent : UnityEvent<float> { }
        // ammo / maxAmmo
        public class AmmoUpdateEvent : UnityEvent<int> { }
    }
}
