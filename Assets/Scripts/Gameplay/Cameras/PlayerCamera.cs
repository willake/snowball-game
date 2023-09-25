using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Gameplay.Cameras
{
    public class PlayerCamera : MonoBehaviour
    {
        private Camera _cam;
        private ControllerType _controllerType;

        [Header("Settings")]
        public float movingSmoothTime = 0.3f;
        public float idleSmoothTime = 0.5f;
        public float maxFollowSpeed = 10f;

        [Header("Prediction Settings")]
        public float predictionDistance = 1f;

        [Header("Arm Settings")]
        [Range(0f, 90f)]
        public float cameraAngle = 60f;
        public float cameraDistance = 10f;
        // public Vector3 arm = new Vector3(0, 5, -10);

        private Vector3 _velocity;

        private void Awake()
        {
            transform.rotation = Quaternion.Euler(new Vector3(90 - cameraAngle, 0, 0));
        }

        public Camera GetCamera()
        {
            if (_cam == null) _cam = GetComponent<Camera>();

            return _cam;
        }

        public void TakeOver(ControllerType controllerType)
        {
            _controllerType = controllerType;
        }

        public void FollowCharacter(bool isPushingButton, Vector3 characterPos, Vector3 direction)
        {
            if (_controllerType != ControllerType.Player) return;

            // calculate arm
            Vector3 arm = new Vector3(
                0, Mathf.Cos(cameraAngle * Mathf.Deg2Rad), -Mathf.Sin(cameraAngle * Mathf.Deg2Rad)
            ) * cameraDistance;

            Vector3 targetPos;
            if (isPushingButton)
            {
                targetPos = characterPos + arm + (direction * predictionDistance);
                targetPos = Vector3.SmoothDamp(
                    transform.position, targetPos, ref _velocity, movingSmoothTime);
            }
            else
            {
                targetPos = characterPos + arm;
                targetPos = Vector3.SmoothDamp(
                    transform.position, targetPos, ref _velocity, idleSmoothTime);
            }

            transform.position = targetPos;
        }

        public enum ControllerType
        {
            Player
        }
    }
}