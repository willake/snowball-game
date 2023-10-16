using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class AmmoBar : MonoBehaviour
    {
        public AmmoBarSlot[] slots;

        public void SetAmmo(int ammo)
        {
            if (ammo > slots.Length)
            {
                ammo = slots.Length;
            }

            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].SetIsFilled(i < ammo);
            }
        }
    }
}