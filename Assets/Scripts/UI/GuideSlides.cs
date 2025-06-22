using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideSlides : MonoBehaviour
{
    [SerializeField] List<GameObject> slides = new List<GameObject>();
    // Start is called before the first frame update
    float count = 5;
    int ind = 0;
    void Start()
    {
        ind = 0;
        foreach(GameObject go in slides)
            go.SetActive(false);
        slides[ind].SetActive(true);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        count -= Time.deltaTime;
        if(count<=0)
        {
            DoClick();
            count = 5;
        }    
    }
    public void DoClick()
    {
        slides[ind].GetComponent<SC_Fade_UI_Tutorial>().FadeOutAndDisable();

        if (ind < slides.Count - 1)
            ind++;
        else
            ind = 0;
        slides[ind].SetActive(true);
    }    
}
