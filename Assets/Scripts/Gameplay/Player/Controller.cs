using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay
{
    public abstract class Controller : MonoBehaviour
    {
        public PlayerStatus Status { get; protected set; }

        private void SetStatus(PlayerStatus status)
        {
            Status = status;
        }
    }
}