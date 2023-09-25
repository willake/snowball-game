using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using WillakeD.CommonPatterns;

namespace Game.Inputs
{
    public class InputManager : Singleton<InputManager>
    {
        public UIInputSet inputSetUI;
        public bool AllowInput { get; private set; }

        private void Update()
        {
            if (AllowInput)
            {
                inputSetUI.DetectInput();
            }
        }

        public void SetAllowInput(bool v)
        {
            AllowInput = v;
        }
    }
}