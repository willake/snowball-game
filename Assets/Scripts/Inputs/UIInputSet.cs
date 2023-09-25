using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Game.Inputs
{
    [CreateAssetMenu(menuName = "MyGame/InputSets/UIInputSet")]
    public class UIInputSet : ScriptableObject, IInputSet
    {
        public bool IsActive { get => true; }
        public KeyCode kKeyboardUp = KeyCode.UpArrow;
        public KeyCode kKeyboardDown = KeyCode.DownArrow;
        public KeyCode kKeyboardConfirm = KeyCode.Space;
        public KeyCode kKeyboardCancel = KeyCode.Escape;
        public string kJoyStickVertical = "Y Axis";
        public KeyCode kJoyStickConfirm = KeyCode.Joystick1Button0;
        public KeyCode kJoyStickCancel = KeyCode.Joystick1Button1;
        public UnityEvent ePressUpEvent = new UnityEvent();
        public UnityEvent ePressDownEvent = new UnityEvent();
        public UnityEvent ePressConfirmEvent = new UnityEvent();
        public UnityEvent ePressCancelEvent = new UnityEvent();

        public void PressUp() { ePressUpEvent.Invoke(); }
        public void PressDown() { ePressDownEvent.Invoke(); }
        public void PressConfirm() { ePressConfirmEvent.Invoke(); }
        public void PressCancel() { ePressCancelEvent.Invoke(); }

        public void Activate() { }
        public void Deactivate() { }
        public void DetectInput()
        {
            if (Input.GetKeyDown(kKeyboardUp)) PressUp();
            if (Input.GetKeyDown(kKeyboardDown)) PressDown();
            if (Input.GetKeyDown(kKeyboardConfirm)) PressConfirm();
            if (Input.GetKeyDown(kKeyboardCancel)) PressCancel();

            // float verticalAxis = Input.GetAxis(JoyStickVertical);

            // if (verticalAxis > 0) PressUp();
            // if (verticalAxis < 0) PressDown();

            if (Input.GetKeyDown(kJoyStickConfirm)) PressConfirm();
            if (Input.GetKeyDown(kJoyStickCancel)) PressCancel();
        }
    }
}