using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.UI
{
    public interface IWDText
    {
        public void SetText(string t);
        public void SetTextColor(Color32 color);
    }
}