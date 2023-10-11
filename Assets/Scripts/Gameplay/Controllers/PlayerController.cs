using System;
using System.Collections;
using System.Collections.Generic;
using Game.Gameplay.Cameras;
using Game.RuntimeStates;
using Game.UI;
using UnityEngine;

namespace Game.Gameplay
{
    public class PlayerController : Controller
    {
        [Header("References")]
        public PlayerCamera bindedCamera;
        public Vector3State statePlayerPos;
        public ProgressBar reloadBar;

        private bool _isPressingMove;
        public bool isControllable;

        private PlayerCharacter _playerCharacter;

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
                MainGameScene.instance.RegisterPlayer(this);
            }

            bindedCharacter.dieEvent.AddListener(HandleDieEvent);
            bindedCharacter.weaponHolder.reloadStartEvent.AddListener(() =>
            {
                reloadBar.gameObject.SetActive(true);
                reloadBar.SetProgress(0f, 0f);
            });
            bindedCharacter.weaponHolder.reloadEndEvent.AddListener(() => reloadBar.gameObject.SetActive(false));
            bindedCharacter.weaponHolder.reloadProgressUpdateEvent.AddListener(progress => reloadBar.SetProgress(progress, 0f));

            isControllable = true;
        }

        private void SetupReloadBar()
        {
            reloadBar.SetProgress(0f, 0f);
            reloadBar.transform.SetParent(MainGameScene.instance.worldSpaceCanvas.transform);
            if (reloadBar.TryGetComponent<FaceCamera>(out FaceCamera faceCamera))
            {
                faceCamera.cam = bindedCamera.GetComponent<Camera>();
            }
        }

        private void HandleDieEvent()
        {
            isControllable = false;
            // bindedCharacter.GetNavMeshAgent().isStopped = true;
            StartCoroutine(DestoryCharacter());
        }

        IEnumerator DestoryCharacter()
        {
            yield return new WaitForSeconds(2f);
            MainGameScene.instance?.EliminatePlayer(this);
        }

        private void Update()
        {
            if (bindedCharacter == null) return;
            if (isControllable == false) return;

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            if (Math.Abs(horizontal) > float.Epsilon || Math.Abs(vertical) > float.Epsilon)
            {
                bindedCharacter.Move(horizontal, vertical);
                _isPressingMove = true;
            }
            else
            {
                bindedCharacter.Idle();
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
                bindedCharacter.Reload();
            }

            if (bindedCharacter.State.isAiming)
            {
                Vector3 chaPos =
                    bindedCamera.GetCamera().WorldToScreenPoint(bindedCharacter.transform.position);
                bindedCharacter.UpdateAimDirection((Input.mousePosition - chaPos).normalized);
            }

            statePlayerPos.value = bindedCharacter.transform.position;
        }

        private void FixedUpdate()
        {
            if (bindedCharacter == null || bindedCamera == null) return;

            Vector3 direction = bindedCharacter.currentVelocity;
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
    }
}