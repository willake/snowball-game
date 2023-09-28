using UnityEngine;

namespace Game.RuntimeStates
{
    [CreateAssetMenu(fileName = "Vector3State", menuName = "MyGame/RuntimeStates/Vector3State", order = 0)]
    public class Vector3State : ScriptableObject
    {
        public Vector3 value;
    }
}