using System;
using UnityEngine;

namespace Game.Gameplay
{
    public class CharacterFactory : MonoBehaviour
    {
        [Header("References")]
        public GameObject playerPrefab;
        public GameObject enemyPrefab;

        public Character GeneratePlayer(string name)
        {
            GameObject obj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            obj.name = name;
            Character character = obj.GetComponent<Character>();
            return character;
        }

        public Character GenerateEnemy(string name)
        {
            GameObject obj = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);
            obj.name = name;
            Character character = obj.GetComponent<Character>();
            return character;
        }
    }
}