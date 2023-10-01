using UnityEngine;

namespace Game.UI
{
    public class WDTextButton : WDButton
    {
        public WDText wdtext;

        public void SetText(string t)
        {
            wdtext.text = t;
        }

        public void SetTextColor(Color32 color)
        {
            wdtext.color = color;
        }
    }
}