using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WillakeD.CommonPatterns;

namespace Game.Gameplay
{
    public abstract class GameScene<T> : Singleton<T> where T : MonoBehaviour
    {
    }
}