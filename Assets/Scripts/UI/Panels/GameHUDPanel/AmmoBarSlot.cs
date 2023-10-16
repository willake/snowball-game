using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class AmmoBarSlot : MonoBehaviour
    {
        public GameObject filled;

        public void SetIsFilled(bool isFilled)
        {
            if (isFilled)
            {
                filled.SetActive(true);
            }
            else
            {
                filled.SetActive(false);
            }
        }
    }
}