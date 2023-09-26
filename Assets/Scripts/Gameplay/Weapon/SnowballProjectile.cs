using UnityEngine;

namespace Game.Gameplay
{
    public class SnowballProjectile : MonoBehaviour
    {
        public ControllerType OwnerType { get; private set; }
        public void SetOwnerType(ControllerType ownerType)
        {
            OwnerType = ownerType;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (
                OwnerType == ControllerType.Player
                && other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {

            }

            if (OwnerType == ControllerType.AI
                && other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {

            }
        }
    }
}