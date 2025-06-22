using EZCameraShake;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCamera : MonoBehaviour
{
    public float mad=1;
    public float rough=4;
    public float fadeIn=0.1f;
    public float fadeOut=1;

    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Shake()
    {
        CameraShaker.Instance.ShakeOnce(mad, rough, fadeIn, fadeOut);
    }
}
