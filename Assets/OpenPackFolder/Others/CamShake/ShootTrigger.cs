using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTrigger : MonoBehaviour
{
    public CamShakeScript camShake;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)){
            StartCoroutine(camShake.Shake(1.0f, 0.1f));
        }
    }
}
