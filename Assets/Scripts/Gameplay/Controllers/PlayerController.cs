using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Gameplay.Cameras;
using Game.RuntimeStates;
using Game.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Gameplay
{
    public class PlayerController : Controller
    {
        [Header("References")]
        public Transform spawnPoint;
        public PlayerCamera bindedCamera;
        public BoolState isPlayerDead;
        public Vector3State statePlayerPos;
        public ProgressBar reloadBar;
        public ProgressBar chargeBar;

        private bool _isPressingMove;
        public int availableLifes = 3;
        private PlayerCharacter _playerCharacter;
        public LifesUpdateEvent lifesUpdateEvent = new();

        public void BindCamera(PlayerCamera cam)
        {
            bindedCamera = cam;
            bindedCamera.TakeOver(PlayerCamera.ControllerType.Player);
        }

        private void Start()
        {
            if (MainGameScene.instance)
            {
                SetupReloadBar();
                SetupChargeBar();
                MainGameScene.instance.RegisterPlayer(this);
            }

            bindedCharacter.dieEvent.AddListener(HandleDieEvent);
            bindedCharacter.weaponHolder.reloadStartEvent.AddListener(() =>
            {
                GetReloadBarFollowScript().ForceUpdate();
                reloadBar.SetProgress(0f);
                reloadBar.gameObject.SetActive(true);
            });
            bindedCharacter.weaponHolder.reloadEndEvent.AddListener(() => reloadBar.gameObject.SetActive(false));
            bindedCharacter.weaponHolder.reloadProgressUpdateEvent.AddListener(progress => reloadBar.SetProgress(progress));

            bindedCharacter.weaponHolder.loadEvent.AddListener(() =>
            {
                GetChargeBarFollowScript().ForceUpdate();
                chargeBar.SetProgress(0f);
                chargeBar.gameObject.SetActive(true);
            });
            bindedCharacter.weaponHolder.throwEvent.AddListener(() => chargeBar.gameObject.SetActive(false));
            bindedCharacter.weaponHolder.energyUpdateEvent.AddListener(progress => chargeBar.SetProgress(progress));

            isPlayerDead.value = false;
        }

        public bool Revive()
        {
            if (availableLifes == 0)
            {
                return false;
            }
            Debug.Log($"AvailableLife {availableLifes}");
            transform.position = spawnPoint.position;
            availableLifes--;
            isPlayerDead.value = false;
            bindedCharacter.Revive();
            lifesUpdateEvent.Invoke(availableLifes);
            return true;
        }

        private void SetupReloadBar()
        {
            reloadBar.SetProgress(0f);
            reloadBar.transform.SetParent(MainGameScene.instance.worldSpaceCanvas.transform);
            if (reloadBar.TryGetComponent<FaceCamera>(out FaceCamera faceCamera))
            {
                faceCamera.cam = bindedCamera.GetComponent<Camera>();
            }
            reloadBar.gameObject.SetActive(false);
        }

        private void SetupChargeBar()
        {
            chargeBar.SetProgress(0f);
            chargeBar.transform.SetParent(MainGameScene.instance.worldSpaceCanvas.transform);
            if (chargeBar.TryGetComponent<FaceCamera>(out FaceCamera faceCamera))
            {
                faceCamera.cam = bindedCamera.GetComponent<Camera>();
            }
            chargeBar.gameObject.SetActive(false);
        }

        private void HandleDieEvent()
        {
            // bindedCharacter.GetNavMeshAgent().isStopped = true;
            isPlayerDead.value = true;
            StartCoroutine(DestoryCharacter());
        }

        IEnumerator DestoryCharacter()
        {
            yield return new WaitForSeconds(2f);
            MainGameScene.instance?.EliminatePlayer(this);
        }

        private void Update()
        {
            if (MainGameScene.instance.IsGameRunning == false) return;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameManager.instance.IsPaused == false)
                {
                    UIManager.instance.OpenUIAsync(AvailableUI.PausePanel).Forget();
                }
                return;
            }
            if (GameManager.instance.IsPaused) return;
            if (bindedCharacter == null || bindedCharacter.State.isDead) return;

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            if (Math.Abs(horizontal) > float.Epsilon || Math.Abs(vertical) > float.Epsilon)
            {
                GetPlayerCharacter().Move(horizontal, vertical);
                _isPressingMove = true;
            }
            else
            {
                GetPlayerCharacter().Idle();
                _isPressingMove = false;
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                GetPlayerCharacter().Aim();
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                GetPlayerCharacter().TryThrow();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                GetPlayerCharacter().Reload();
            }

            if (bindedCharacter.State.isAiming)
            {
                Vector3 chaPos =
                    bindedCamera.GetCamera().WorldToScreenPoint(bindedCharacter.transform.position);
                Vector3 dir = (Input.mousePosition - chaPos).normalized;
                dir.z = dir.y;
                bindedCharacter.UpdateAimDirection(dir);
            }

            statePlayerPos.value = bindedCharacter.transform.position;
        }

        private void FixedUpdate()
        {
            if (bindedCharacter == null || bindedCamera == null) return;

            Vector3 direction = GetPlayerCharacter().currentVelocity;
            direction.y = 0;
            direction.Normalize();

            bindedCamera.FollowCharacter(
                _isPressingMove, bindedCharacter.transform.position, direction);
        }

        private PlayerCharacter GetPlayerCharacter()
        {
            if (_playerCharacter == null) _playerCharacter = bindedCharacter as PlayerCharacter;

            return _playerCharacter;
        }

        private FollowTarget _chargebarFollow;
        private FollowTarget GetChargeBarFollowScript()
        {
            if (_chargebarFollow == null) _chargebarFollow = chargeBar.GetComponent<FollowTarget>();

            return _chargebarFollow;
        }
        private FollowTarget _reloadbarFollow;
        private FollowTarget GetReloadBarFollowScript()
        {
            if (_reloadbarFollow == null) _reloadbarFollow = reloadBar.GetComponent<FollowTarget>();

            return _reloadbarFollow;
        }

        public class LifesUpdateEvent : UnityEvent<int> { }
    }
}