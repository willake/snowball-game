using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.UI
{
    public class WDText : MonoBehaviour
    {
        public TextMeshProUGUI textMesh;
        public string text
        {
            set
            {
                textMesh.text = value;
            }
            get
            {
                return textMesh.text;
            }
        }

        public Color32 color
        {
            set
            {
                textMesh.color = value;
            }
            get
            {
                return textMesh.color;
            }
        }
    }
}