using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class LifeBar : MonoBehaviour
    {
        public LifeBarSlot[] slots;

        public void SetLifes(int lifes)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].SetIsFilled(i < lifes + 1);
            }
        }
    }
}