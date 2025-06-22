using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;


public class SC_Bounce_Stat : MonoBehaviour
{
    public float fadeInDuration = 0.2f;
    public float fadeOutDuration = 0.2f;
    public float duration = 2f;
    [SerializeField] private Vector3 scaleFactor = Vector3.one;
    private Image image;
    private TextMeshPro textMeshPro;
    private Sequence bounceTween;


    private void Awake()
    {
        image = GetComponent<Image>();
        textMeshPro = GetComponentInChildren<TextMeshPro>();

        // if (spriteRenderer == null)
        // {
        //     Debug.LogWarning(gameObject.name + " khong co spriteRenderer component de bounce.");
        // }

        if (textMeshPro == null)
        {
            Debug.LogWarning(gameObject.name + " child khong co textMeshPro child de bounce.");
        }

    }
    private void OnEnable()
    {
        BounceInThenOut();
    }
    private void OnDisable()
    {
        DoKillAllTween();
    }

    private void BounceInThenOut()
    {
        bounceTween = DOTween.Sequence();
        bounceTween.Append(transform.DOScale(Vector3.zero, 0.01f));
        bounceTween.Join(image.DOFade(0f, 0.01f));
        bounceTween.Join(textMeshPro.DOFade(0f, 0.01f));

        bounceTween.Append(image.DOFade(1f, fadeInDuration).SetEase(Ease.OutBack));
        bounceTween.Join(textMeshPro.DOFade(1f, fadeInDuration).SetEase(Ease.OutBack));
        bounceTween.Join(transform.DOScale(scaleFactor, fadeInDuration).SetEase(Ease.OutBack));

        bounceTween.AppendInterval(duration);

        bounceTween.Append(image.DOFade(0f, fadeOutDuration).SetEase(Ease.InBack));
        bounceTween.Join(textMeshPro.DOFade(0f, fadeOutDuration).SetEase(Ease.InBack));
        bounceTween.Join(transform.DOScale(Vector3.zero, fadeOutDuration).SetEase(Ease.InBack));
        // .OnComplete(() => gameObject.SetActive(false));

        bounceTween.AppendInterval(duration);

        bounceTween.SetLoops(-1);
    }
    private void DoKillAllTween()
    {
        if (bounceTween != null)
        {
            bounceTween.Kill();
        }
    }
}