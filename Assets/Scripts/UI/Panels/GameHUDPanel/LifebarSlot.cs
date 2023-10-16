using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class LifeBarSlot : MonoBehaviour
    {
        public Image image;
        public Sprite empty;
        public Sprite filled;

        public void SetIsFilled(bool isFilled)
        {
            if (isFilled)
            {
                image.sprite = filled;
            }
            else
            {
                image.sprite = empty;
            }
        }
    }
}