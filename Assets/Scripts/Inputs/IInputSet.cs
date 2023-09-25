
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UniRx;

namespace Game.Inputs
{
    public interface IInputSet
    {
        bool IsActive { get; }
        void Activate();
        void Deactivate();
        void DetectInput();
    }
}