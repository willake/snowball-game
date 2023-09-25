using UnityEngine;

namespace Game.UI
{
    public class WDTextButton : WDButton, IWDText
    {
        public WDText text;

        public void SetText(string t)
        {
            text.SetText(t);
        }

        public void SetTextColor(Color32 color)
        {
            text.SetTextColor(color);
        }
    }
}