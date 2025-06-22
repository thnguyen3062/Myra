using System.Collections;
using UnityEngine;

public class CamShakeScript_UI : MonoBehaviour
{
    public IEnumerator Shake(float duration, float magnitude)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        
        if (rectTransform == null)
        {
            Debug.LogError("CamShakeScript requires a RectTransform component.");
            yield break;
        }

        Vector3 originalPos = rectTransform.anchoredPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-0.2f, 0.2f) * magnitude;
            float y = Random.Range(-0.3f, 0.3f) * magnitude;
            float z = Random.Range(0f, 0f) * magnitude;

            float a = Mathf.Lerp(0 , duration , (duration - elapsed));

            rectTransform.anchoredPosition = new Vector3(originalPos.x + x * a, originalPos.y + y * a, originalPos.z + z * a);

            elapsed += Time.deltaTime;

            yield return null;
        }

        rectTransform.anchoredPosition = originalPos;
    }
}