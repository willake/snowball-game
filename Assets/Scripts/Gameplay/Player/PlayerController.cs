using System;
using System.Collections;
using System.Collections.Generic;
using Game.Gameplay.Cameras;
using UnityEngine;

namespace Game.Gameplay
{
    public class PlayerController : Controller
    {
        public PlayerCamera bindedCamera;

        private bool _isMoving;
        private bool _isAiming = false;

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
        }

        private void Update()
        {
            if (bindedCharacter == null) return;

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
                bindedCharacter.Hold();
                _isAiming = true;
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                bindedCharacter.Throw();
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
                bindedCharacter.Aim((chaPos - Input.mousePosition).normalized);
            }
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