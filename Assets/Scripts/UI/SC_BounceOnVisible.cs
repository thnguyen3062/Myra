using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class SC_BounceOnVisible : MonoBehaviour
{
    public List<GameObject>  objectsToBounce = new List<GameObject>(); // List of game objects to bounce

    [SerializeField] private float delayBetweenBounces = 0.5f; // Delay between each object's bounce

    [SerializeField] private float bounceDuration = 0.5f;

    [SerializeField] private float scale = 1.2f; 

    private void Start()
    {
        
    }
    public IEnumerator BeginBounce()
    {
        foreach (GameObject obj in objectsToBounce)
        {
            //Lúc start có visible không
            if (IsObjectVisible(obj))
            {
                BounceObject(obj);
            }
            else
            {
                // Không visible -> đợi WaitUntilVisible.
                yield return WaitUntilVisible(obj);
            }

            // Đợi cho đến bounce tiếp theo
            yield return new WaitForSeconds(delayBetweenBounces);
        }
    }

    private bool IsObjectVisible(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.isVisible; //renderer
        }

        CanvasRenderer canvasRenderer = obj.GetComponent<CanvasRenderer>();
        if (canvasRenderer != null)
        {
            return canvasRenderer.cull != true; // canvasRenderer cull != true -----> currently visible.
        }
        //Không tìm được renderer nào -> invisible
        return false;
    }

    private IEnumerator WaitUntilVisible(GameObject obj)
    {
        // Wait until visible
        while (!IsObjectVisible(obj))
        {
            yield return null;
        }

        BounceObject(obj);
    }

    private void BounceObject(GameObject obj)
    {
        // có rect transform hoặc transform không
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        Transform transform = obj.GetComponent<Transform>();

        if (rectTransform != null)
        {
            rectTransform.DOScale(Vector3.zero * scale, 0.01f);
            // Bounce animation for RectTransform using DOTween
            rectTransform.DOScale(Vector3.one * scale, bounceDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    rectTransform.DOScale(Vector3.one, bounceDuration).SetEase(Ease.InQuad);
                });
        }
        else if (transform != null)
        {
            // Bounce animation for Transform using DOTween
            transform.DOScale(Vector3.one * scale, bounceDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    transform.DOScale(Vector3.one, bounceDuration).SetEase(Ease.InQuad);
                });
        }
    }



    public SC_BounceOnVisible SetScaleZero()
    {

        foreach (GameObject obj in objectsToBounce)
        {
            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            Transform transform = obj.GetComponent<Transform>();
            
            if (rectTransform != null)
            {
                rectTransform.localScale = Vector3.zero;
            }
            else if (transform != null)
            {
                transform.localScale = Vector3.zero;
            }
        }
        return this;
    }

}

