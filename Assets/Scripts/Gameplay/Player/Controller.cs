using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay
{
    public abstract class Controller : MonoBehaviour
    {
        [Header("References")]
        public Character bindedCharacter;
        public PlayerStatus Status { get; protected set; }

        public void BindCharacter(Character character)
        {
            bindedCharacter = character;
        }

        private void SetStatus(PlayerStatus status)
        {
            Status = status;
        }
    }
}