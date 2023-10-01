using Codice.Client.Commands.TransformerRule;
using UnityEngine;

namespace Game.Gameplay.Cameras
{
    public class FaceCamera : MonoBehaviour
    {
        public Camera cam;

        private void Update()
        {
            if (cam) transform.rotation = Quaternion.LookRotation(cam.transform.forward * -1);
        }
    }
}