using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay
{
    public interface IWeapon
    {
        void Fire(Vector3 direction);
    }
}