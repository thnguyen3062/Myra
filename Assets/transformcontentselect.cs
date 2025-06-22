using GIKCore.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class transformcontentselect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChancePosition()
    {
        SoundHandler.main.PlaySFX("900_slide_1", "sounds");
        transform.position = new Vector2(transform.position.x + (-20f) * Time.deltaTime, transform.position.y);
    }
    public void ChancePositionleft()
    {
        SoundHandler.main.PlaySFX("900_slide_1", "sounds");
        transform.position = new Vector2(transform.position.x + (20f) * Time.deltaTime, transform.position.y);
    }
}
