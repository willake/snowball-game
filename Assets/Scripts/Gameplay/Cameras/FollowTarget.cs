using UnityEngine;

namespace Game.Gameplay.Cameras
{
    public class FollowTarget : MonoBehaviour
    {
        [Header("Settings")]
        public Transform target;
        public Vector3 offset;

        private void Update()
        {
            transform.position = target.position + offset;
        }

        public void ForceUpdate()
        {
            transform.position = target.position + offset;
        }
    }
}