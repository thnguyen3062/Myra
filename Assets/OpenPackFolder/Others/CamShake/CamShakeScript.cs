using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShakeScript : MonoBehaviour
{
    public IEnumerator Shake (float duration, float magnitude){
  
    Vector3 originalPos = transform.localPosition;

    float elapsed = 0.0f;

    while (elapsed < duration) {
    float x = Random.Range(-0.2f,0.2f) * magnitude;
    float y = Random.Range(-0.3f,0.3f) * magnitude;
    float z = Random.Range(0f,0f) * magnitude;

    transform.localPosition = new Vector3 (originalPos.x + x, originalPos.y + y, originalPos.z + z);

    elapsed += Time.deltaTime;

    yield return null;
    }

    transform.localPosition = originalPos;
}
}
