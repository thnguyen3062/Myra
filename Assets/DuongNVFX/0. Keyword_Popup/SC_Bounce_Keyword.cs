using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;


public class SC_Bounce_Keyword : MonoBehaviour
{
    [SerializeField] private Image m_BlackBG;
    public float fadeInDuration = 0.2f;
    public float fadeOutDuration = 0.2f;
    public float duration = 2f;
    [SerializeField] private Vector3 scaleFactor = Vector3.one;
    private Image image;
    private TextMeshPro textMeshPro;
    private Sequence bounceTween;


    private void Awake()
    {
        image = m_BlackBG;
        // textMeshPro = GetComponentInChildren<TextMeshPro>();

        // if (spriteRenderer == null)
        // {
        //     Debug.LogWarning(gameObject.name + " khong co spriteRenderer component de bounce.");
        // }

        // if (textMeshPro == null)
        // {
        //     Debug.LogWarning(gameObject.name + " child khong co textMeshPro child de bounce.");
        // }        

    }

    private void OnEnable()
    {
        BounceInThenOut();
    }

    void BounceInThenOut()
    {
            bounceTween = DOTween.Sequence();
            bounceTween.Append(transform.DOScale(Vector3.zero, 0.01f));
            bounceTween.Append(image.DOFade(0.8f, fadeInDuration).SetEase(Ease.OutBack));
            bounceTween.Join(transform.DOScale(scaleFactor, fadeInDuration).SetEase(Ease.OutBack));
            bounceTween.AppendInterval(duration);
            bounceTween.Append(image.DOFade(0f, fadeOutDuration).SetEase(Ease.InBack));
            bounceTween.Join(transform.DOScale(Vector3.zero, fadeOutDuration).SetEase(Ease.InBack)).OnComplete(() => gameObject.transform.parent.gameObject.SetActive(false));
    }

        
    

    private void OnDestroy()
    {
        if (bounceTween != null)
        {
            bounceTween.Kill();
        }
    }
}