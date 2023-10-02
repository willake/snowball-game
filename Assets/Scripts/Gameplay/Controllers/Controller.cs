using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay
{
    public abstract class Controller : MonoBehaviour
    {
        [Header("References")]
        public Character bindedCharacter;

        public void BindCharacter(Character character)
        {
            bindedCharacter = character;
        }
    }
}