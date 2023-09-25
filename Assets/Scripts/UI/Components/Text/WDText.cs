using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.UI
{
    public class WDText : MonoBehaviour, IWDText
    {
        public TextMeshProUGUI text;
        public void SetText(string t)
        {
            text.text = t;
        }

        public void SetTextColor(Color32 color)
        {
            text.color = color;
        }
    }
}