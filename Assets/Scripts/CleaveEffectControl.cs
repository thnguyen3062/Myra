using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleaveEffectControl : MonoBehaviour
{
    [SerializeField] private GameObject impact0, impact1, impactNearLeft,impactNearRight;
    // Start is called before the first frame update
    void Start()
    {
        impact0.SetActive(false);
        impact1.SetActive(false);
        impactNearLeft.SetActive(false);
        impactNearRight.SetActive(false);
    }
    public IEnumerator OnCleaveHit(bool isLeft)
    {
        impact0.SetActive(true);
        impact1.SetActive(true);
        yield return new WaitForSeconds(0.02f);
        if (isLeft)
        {
            impactNearLeft.SetActive(true);
        }    
        else
            impactNearRight.SetActive(true);

    }    
}
