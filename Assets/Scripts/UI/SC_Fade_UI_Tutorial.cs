using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using GIKCore.Utilities;

public class SC_Fade_UI_Tutorial : MonoBehaviour
{
    public float fadeInDuration = 0.3f;
    public float fadeOutDuration = 0.3f;

    private CanvasGroup canvasGroup;
    private Tween fadeTween;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            Debug.LogWarning(gameObject.name + " khong co canvasGroup component de fade.");
        }
    }

    private void OnEnable()
    {
        FadeIn();
    }

    void FadeIn()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            fadeTween = canvasGroup.DOFade(1f, fadeInDuration).SetEase(Ease.OutSine);
        }
    }

    public void FadeOutAndDisable()
    {
        if (canvasGroup != null)
        {
            fadeTween = canvasGroup.DOFade(0f, fadeOutDuration).SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);

                });
        }
    }

    private void OnDestroy()
    {
        if (fadeTween != null)
        {
            fadeTween.Kill();
        }
    }
}
