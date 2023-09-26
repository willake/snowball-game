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
        private Character _character;

        public bool isControllingMovement;

        private bool _isAiming = false;
        private bool _isHoldingFire = false;

        public void BindCamera(PlayerCamera cam)
        {
            _camera = cam;
            _camera.TakeOver(PlayerCamera.ControllerType.Player);
        }

        public void BindCharacter(Character character)
        {
            _character = character;
            Debug.Log($"Character has been set to: {character.name}");
        }

        public void EquipWeapon(Weapon weapon)
        {
            _character.EquipWeapon(weapon);
        }

        public void Fire()
        {
            _character.Attack();
        }

        public void Aim()
        {
            _character.Aim(Vector2.zero);
        }

        private void Update()
        {
            if (_character == null) return;

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            if (Math.Abs(horizontal) > float.Epsilon || Math.Abs(vertical) > float.Epsilon)
            {
                _character.Move(horizontal, vertical);
                isControllingMovement = true;
            }
            else
            {
                isControllingMovement = false;
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                _isAiming = true;
            }

            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                _isAiming = false;
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _character.Attack();
                _isHoldingFire = true;
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                _isHoldingFire = false;
            }

            if (_isAiming)
            {
                Vector3 chaPos =
                    _camera.GetCamera().WorldToScreenPoint(_character.transform.position);
                _character.Aim((chaPos - Input.mousePosition).normalized);
            }

            if (_isHoldingFire && _character.weaponHolder.HoldingWeapon.canHold)
            {
                _character.Attack();
            }
        }

        private void FixedUpdate()
        {
            if (_character == null || _camera == null) return;

            Vector3 direction = _character.currentVelocity;
            direction.y = 0;
            direction.Normalize();

            _camera.FollowCharacter(
                isControllingMovement, _character.transform.position, direction);
        }
    }
}