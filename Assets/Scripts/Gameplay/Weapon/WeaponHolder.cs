using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using Game.Gameplay.WeaponHolderStates;
using Game.Audios;

namespace Game.Gameplay
{
    public class WeaponHolder : MonoBehaviour
    {
        [Header("References")]
        public Transform socket;
        public ParticleSystem particalCriticalHit;

        [Header("Settings")]
        public bool isDebugLogEnabled = false;
        [Range(0f, 30f)]
        public float throwingPitch = 10f;
        public float minEnergy = 0f;
        public float maxEnergy = 10f;
        public float criticalThreshold = 0.95f;
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

        public void Reset()
        {
            holdingWeapon.Reset();
            ammoUpdateEvent.Invoke(Ammo);
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

        public bool AimWithoutCharging()
        {
            if (State.shouldReload) return false;

            Energy = 0;
            holdingWeapon.Load();
            loadEvent.Invoke();
            SetWeaponHolderState(WeaponHolderState.AimState);
            return true;
        }

        public bool Throw()
        {
            return Throw(Energy);
        }

        // for enemy AI
        public bool Throw(float energy)
        {
            if (State.isAiming == false) return false;

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

            energy = Mathf.Clamp(energy, minEnergy, maxEnergy);

            bool isCritical = false;

            // critical hit
            if (ownerCamp == Camp.Player && energy / maxEnergy > criticalThreshold)
            {
                WrappedAudioClip audioClip =
                    ResourceManager.instance.audioResources.gameplayAudios.criticalCharge;
                AudioManager.instance.PlaySFX(
                    audioClip.clip,
                    audioClip.volume
                );
                particalCriticalHit.Play();
                isCritical = true;
            }

            holdingWeapon.Attack(shootDirection.normalized, energy, isCritical);

            throwEvent.Invoke();
            ammoUpdateEvent.Invoke(Ammo);

            if (Ammo <= 0) SetWeaponHolderState(WeaponHolderState.NeedReloadState);
            else SetWeaponHolderState(WeaponHolderState.IdleState);

            return true;
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
