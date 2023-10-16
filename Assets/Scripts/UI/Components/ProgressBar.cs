using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

// reference: https://www.youtube.com/watch?v=Qw8odLHv38Q&t=0s&ab_channel=LlamAcademy

namespace Game.UI
{
    public class ProgressBar : MonoBehaviour
    {
        [Header("References")]
        public Image progressImage;
        public UnityEvent onProgress = new UnityEvent();
        public UnityEvent onComplated = new UnityEvent();

        [Header("Settings")]
        public float defaultDuration;
        public Ease ease;
        public Gradient gradient;
        private Tween _tween;


        private void Start()
        {
            if (progressImage.type != Image.Type.Filled)
            {
                Debug.LogWarning($"{name}'s ProgressImage is not of type \"Filled\" so it connot be used" +
                    $"as a progress bar. Disabling this progress bar");
                this.enabled = false;
            }
        }

        public void SetProgress(float progress)
        {
            if (_tween != null && _tween.IsPlaying())
            {
                _tween.Kill();
            }

            onProgress.Invoke();
            progressImage.fillAmount = progress;
            progressImage.color = gradient.Evaluate(progress);
        }

        public void SetProgress(float progress, float duration)
        {
            if (progress < 0 || progress > 1)
            {
                Debug.LogWarning(
                    $"Invalid progress passed, excepted value is between 0 and 1. Got {progress}. Clamping");
                progress = Mathf.Clamp01(progress);
            }

            if (Mathf.Abs(progressImage.fillAmount - progress) > float.Epsilon)
            {
                if (_tween != null && _tween.IsActive())
                {
                    _tween.Kill();
                }

                _tween = progressImage
                    .DOFillAmount(progress, duration)
                    .SetEase(ease)
                    .OnUpdate(() =>
                    {
                        onProgress.Invoke();
                        progressImage.color = gradient.Evaluate(progress);
                    })
                    .OnComplete(() => onComplated.Invoke());
            }
        }

        private void OnDestroy()
        {
            if (_tween != null && _tween.IsActive())
            {
                _tween.Kill();
            }
        }
    }
}