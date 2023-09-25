using System;
using UnityEngine;

namespace Game.Gameplay
{
    public class CharacterFactory : MonoBehaviour
    {
        public GameObject playerCharacter;
        public Character GeneratePlayerCharacter(string name)
        {
            GameObject obj = Instantiate(playerCharacter, Vector3.zero, Quaternion.identity);
            obj.name = name;
            Character character = obj.GetComponent<Character>();
            return character;
        }
    }
}