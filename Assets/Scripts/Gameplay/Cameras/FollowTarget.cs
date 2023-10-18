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
            if (target == null) return;
            transform.position = target.position + offset;
        }

        public void ForceUpdate()
        {
            if (target == null) return;
            transform.position = target.position + offset;
        }
    }
}