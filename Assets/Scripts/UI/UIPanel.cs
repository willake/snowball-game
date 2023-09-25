using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Game.UI
{
    public abstract class UIPanel : MonoBehaviour
    {
        /// <summary>
        /// The id of UI  
        /// </summary>
        public abstract AvailableUI Type { get; }
        /// <summary>
        /// Open the panel, use this when you don't care where the animation is played
        /// </summary>
        public abstract void Open();
        /// <summary>
        /// Open the panel, the function would be waitable  
        /// </summary>
        public abstract UniTask OpenAsync();
        /// <summary>
        /// Close the panel, use this when you don't care where the animation is played
        /// </summary>
        public abstract void Close();
        /// <summary>
        /// Close the panel, the function would be waitable  
        /// </summary>
        public abstract UniTask CloseAsync();
        /// <summary>
        /// List all the selectable button for keyboard use
        /// </summary>
        public abstract WDButton[] GetSelectableButtons();
        /// <summary>
        /// The function will trigger when cancel button is triggered.   
        /// </summary>
        public abstract void PerformCancelAction();
    }
}