using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.UI
{
    public class WDText : MonoBehaviour
    {
        private TextMeshProUGUI _textMesh;
        public TextMeshProUGUI TextMesh { get => GetTextMeshProUGUI(); }
        public string text
        {
            set
            {
                GetTextMeshProUGUI().text = value;
            }
            get
            {
                return GetTextMeshProUGUI().text;
            }
        }

        public Color32 color
        {
            set
            {
                GetTextMeshProUGUI().color = value;
            }
            get
            {
                return GetTextMeshProUGUI().color;
            }
        }

        private TextMeshProUGUI GetTextMeshProUGUI()
        {
            if (_textMesh == null) _textMesh = GetComponent<TextMeshProUGUI>();
            return _textMesh;
        }
    }
}