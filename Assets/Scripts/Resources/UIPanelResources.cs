using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace Game
{
    [CreateAssetMenu(menuName = "MyGame/Resources/UIPanelResources")]
    public class UIPanelResources : ScriptableObject
    {
        [Header("UI Panels")]
        [AssetsOnly]
        public GameObject menuPanel;
        [AssetsOnly]
        public GameObject gameHUDPanel;
        [AssetsOnly]
        public GameObject pausePanel;
        [AssetsOnly]
        public GameObject endGamePanel;
        [AssetsOnly]
        public GameObject levelSelectPanel;
        [AssetsOnly]
        public GameObject settingsPanel;
        [AssetsOnly]
        public GameObject gameStartPanel;
        [AssetsOnly]
        public GameObject tutorialPanel;
        [AssetsOnly]
        public GameObject modalPanel;
    }
}