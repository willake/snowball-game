using System;
using System.Collections;
using System.Collections.Generic;
using Game.Gameplay.Cameras;
using Game.RuntimeStates;
using UnityEngine;

namespace Game.Gameplay
{
    public class PlayerController : Controller
    {
        [Header("References")]
        public PlayerCamera bindedCamera;
        public Vector3State statePlayerPos;

        private bool _isMoving;
        private bool _isAiming = false;
        public bool isControllable = true;

        private PlayerCharacter _playerCharacter;

        private PlayerCharacter GetPlayerCharacter()
        {
            if (_playerCharacter == null) _playerCharacter = bindedCharacter as PlayerCharacter;

            return _playerCharacter;
        }

        public void BindCamera(PlayerCamera cam)
        {
            bindedCamera = cam;
            bindedCamera.TakeOver(PlayerCamera.ControllerType.Player);
        }

        private void Start()
        {
            if (MainGameScene.instance)
            {
                MainGameScene.instance.RegisterPlayer(this);
            }

            bindedCharacter.dieEvent.AddListener(HandleDieEvent);
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

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            if (Math.Abs(horizontal) > float.Epsilon || Math.Abs(vertical) > float.Epsilon)
            {
                bindedCharacter.Move(horizontal, vertical);
                _isMoving = true;
            }
            else
            {
                _isMoving = false;
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                GetPlayerCharacter().Hold();
                _isAiming = true;
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                GetPlayerCharacter().Throw();
                _isAiming = false;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                bindedCharacter.Reload();
            }

            if (_isAiming)
            {
                Vector3 chaPos =
                    bindedCamera.GetCamera().WorldToScreenPoint(bindedCharacter.transform.position);
                bindedCharacter.Aim((Input.mousePosition - chaPos).normalized);
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
                _isMoving, bindedCharacter.transform.position, direction);
        }
    }
}