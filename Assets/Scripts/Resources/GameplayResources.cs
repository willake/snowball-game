using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace Game
{
    [CreateAssetMenu(menuName = "MyGame/Resources/GameplayResources")]
    public class GameplayResources : ScriptableObject
    {
        [Header("Player")]
        [AssetsOnly]
        public GameObject playerCharacter;

        [Header("Weapons")]
        [AssetsOnly]
        public GameObject weaponPistol;
    }
}