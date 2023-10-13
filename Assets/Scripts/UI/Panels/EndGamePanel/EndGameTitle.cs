using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
namespace Game.UI
{
    public class EndGameTitle : MonoBehaviour
    {
        [Header("References")]
        public WDText txtTitle;
        [Header("Win Animattion Settings")]
        public Color winColor;
        public float winDuration;
        public Ease winEase;
        [Header("Lose Animattion Settings")]
        public Color loseColor;
        public float loseDuration;
        public Ease loseEase;

        private RectTransform _rectTransform;

        public void PlayWinAnimation()
        {
            txtTitle.text = "You win!";
            txtTitle.color = winColor;
            GetRectTransform().localScale = Vector3.zero;
            GetRectTransform()
                .DOScale(new Vector3(1, 1, 1), winDuration)
                .SetEase(winEase).SetUpdate(true);
        }

        public void PlayLoseAnimation()
        {
            txtTitle.text = "Sorry, you lose :(";
            txtTitle.color = loseColor;
            Vector2 originalPos = GetRectTransform().anchoredPosition;
            GetRectTransform().anchoredPosition = new Vector2(originalPos.x, originalPos.y + 200);
            GetRectTransform()
                .DOAnchorPosY(originalPos.y, loseDuration)
                .SetEase(loseEase).SetUpdate(true);
        }

        private RectTransform GetRectTransform()
        {
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();

            return _rectTransform;
        }
    }
}