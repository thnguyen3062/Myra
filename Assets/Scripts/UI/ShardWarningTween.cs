using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShardWarningTween : MonoBehaviour
{
    private Vector3 startPoint =new Vector3(0.1f,0.1f,1);
    private Vector3 endPoint = new Vector3 (2,2,2);
    public float flipDuration = 0.5f;
    // private CanvasGroup canvasGroup;
    public Color originalColor;
    public Color blinkColor;
    public float blinkDuration = 0.25f;
    private SpriteRenderer spriteRenderer;
    private Material spriteMaterial;
    public float targetAlpha = 1f;
    private float originalAlpha = 0.5f;

    // private CanvasRenderer objectRenderer;
    // private Image image;
    // private Material imageMaterial;
    private void Awake()
    {
        // Get the CanvasGroup component
        // canvasGroup = GetComponent<CanvasGroup>();
        // image = GetComponent<Image>();
        // imageMaterial = Instantiate(image.material);
        // image.material = imageMaterial;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteMaterial = Instantiate(spriteRenderer.material);
        spriteRenderer.material = spriteMaterial;
    }
    private void Start()
    {
        AnimateCard();
    }
    public void AnimateCard()
    {
        // Move the card from the start point to the end point
        //transform.position = startPoint.position;
        //transform.rotation = startPoint.rotation;
        transform.localScale = startPoint;
        spriteMaterial.color = originalColor;
        spriteRenderer.DOFade(targetAlpha, 0.1f).From(originalAlpha);

        Sequence sequence2 = DOTween.Sequence();
        sequence2.Append(spriteMaterial.DOColor(blinkColor, blinkDuration));
        sequence2.Append(spriteMaterial.DOColor(originalColor, blinkDuration));
        sequence2.Append(spriteMaterial.DOColor(blinkColor, blinkDuration));
        sequence2.Append(spriteMaterial.DOColor(originalColor, blinkDuration));

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(endPoint, flipDuration).SetEase(Ease.OutElastic));
        sequence.Join(sequence2);
        sequence.AppendInterval(0.05f);


        spriteRenderer.DOFade(originalAlpha, 1.5f).From(targetAlpha).SetEase(Ease.InQuint);

        //transform.position = startPoint.position;
        //transform.rotation = startPoint.rotation;
        //transform.localScale = startPoint;
        //Anh dang de tam sau khi chay xong thi destroy object nay di. Hien dieu chinh nhe.

        sequence.Play();
        sequence.OnComplete(() => this.gameObject.SetActive(false)); ;
    }
}