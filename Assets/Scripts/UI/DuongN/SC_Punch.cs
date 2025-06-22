using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SC_Punch : MonoBehaviour
{
    // [SerializeField] private GameObject MainObject;
    [SerializeField] private float move1 = -10f;
    [SerializeField] private float smallScale = 0.5f;
    [SerializeField] private float bigScale = 1.3f;
    [SerializeField] private float move2 = 10;
    [SerializeField] private int vibrato;
    [SerializeField] private float elasticity = 0.5f;
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private Color Color1;
    [SerializeField] private Color Color2;
    private Material material;
    private Image image;
    public Sequence jumpSeq;
    [SerializeField] private SC_Button_Upgrade button;
    

    // Start is called before the first frame update
    void Start()
    {
        image = transform.GetComponent<Image>();
        
        button = transform.GetComponent<SC_Button_Upgrade>();

        if(button != null)
        {
            button.OnButtonStatusOff += StopAnim;
            button.OnButtonStatusOn += PlayAnim;
        }
        else
        {
            Debug.Log("No button in wiggle.");
        }

        PlayAnim();
    }

    void PlayAnim()
    {
        Sequence jumpSeqTemp = DOTween.Sequence();

        jumpSeq = jumpSeqTemp;
        
        jumpSeq.Append(transform.DOPunchScale( Vector3.one * bigScale , duration , vibrato ,  elasticity ));

        jumpSeq.OnComplete(() => jumpSeq.Restart());

        jumpSeq.Play();
    }

    void StopAnim()
    {
        jumpSeq.OnComplete(null);
        jumpSeq.Complete();
        jumpSeq.Pause();
    }

    void OnDestroy()
    {
        if(button != null)
        {
            button.OnButtonStatusOn -= PlayAnim;
            button.OnButtonStatusOff -= StopAnim;
        }
    }
}
