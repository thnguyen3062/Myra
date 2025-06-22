// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;


public class SC_Bounce_Dmg : MonoBehaviour
{
   [SerializeField] private DamagePopup dmgPool;
    public float fadeInDuration = 0.2f;
    public float fadeOutDuration = 0.2f;
    public float bounceDuration = 2f;
    public Vector3 scaleFactor = Vector3.one;
    private SpriteRenderer spriteRenderer;
    private TextMeshPro textMeshPro;
    private Tween bounceTween;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        textMeshPro = GetComponentInChildren<TextMeshPro>();

        if (spriteRenderer == null)
        {
            Debug.LogWarning(gameObject.name + " khong co spriteRenderer component de bounce.");
        }

        if (textMeshPro == null)
        {
            Debug.LogWarning(gameObject.name + " child khong co textMeshPro child de bounce.");
        }        

    }

    private void OnEnable()
    {
        BounceInThenOut();
    }

    void BounceInThenOut()
    {
        if (spriteRenderer != null)
        {
            // Fade in
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
            spriteRenderer.transform.localScale = Vector3.zero;
            spriteRenderer.DOFade(1f, fadeInDuration).SetEase(Ease.OutBack);
            spriteRenderer.transform.DOScale(scaleFactor, fadeInDuration).SetEase(Ease.OutBack);
        }

        if (textMeshPro != null)
        {
            // Scale up
            textMeshPro.transform.localScale = Vector3.zero;
            textMeshPro.transform.DOScale(Vector3.one, fadeInDuration).SetEase(Ease.OutBack).OnComplete(() =>
            {
                // Wait for 2 seconds
                DOVirtual.DelayedCall(bounceDuration, () =>
                {
                    //scale down
                    spriteRenderer.transform.DOScale(Vector3.zero, fadeOutDuration).SetEase(Ease.InBack);
                    // Scale down
                    textMeshPro.transform.DOScale(Vector3.zero, fadeOutDuration).SetEase(Ease.InBack).OnComplete(() =>
                    {
                        // Disable the GameObject
                        gameObject.SetActive(false);
                        if(dmgPool != null)
                            dmgPool.onComplete?.Invoke();
                    });
                });
            });
        }
    }    

    private void OnDestroy()
    {
        if (bounceTween != null)
        {
            bounceTween.Kill();
        }
    }
}