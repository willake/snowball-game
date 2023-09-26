using System;
using System.Collections;
using System.Collections.Generic;
using Game.Gameplay.Cameras;
using UnityEngine;

namespace Game.Gameplay
{
    public class PlayerController : Controller
    {
        private PlayerCamera _camera;
        public Character bindedCharacter;

        private bool _isMoving;
        private bool _isAiming = false;

        public void BindCamera(PlayerCamera cam)
        {
            _camera = cam;
            _camera.TakeOver(PlayerCamera.ControllerType.Player);
        }

        public void BindCharacter(Character character)
        {
            bindedCharacter = character;
        }

        public void EquipWeapon(Weapon weapon)
        {
            bindedCharacter.EquipWeapon(weapon);
        }

        public void Aim()
        {
            bindedCharacter.Aim(Vector2.zero);
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

            if (_isAiming)
            {
                Vector3 chaPos =
                    _camera.GetCamera().WorldToScreenPoint(bindedCharacter.transform.position);
                bindedCharacter.Aim((chaPos - Input.mousePosition).normalized);
            }
        }

        private void FixedUpdate()
        {
            if (bindedCharacter == null || _camera == null) return;

            Vector3 direction = bindedCharacter.currentVelocity;
            direction.y = 0;
            direction.Normalize();

            _camera.FollowCharacter(
                _isMoving, bindedCharacter.transform.position, direction);
        }
    }
}