using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SC_Wiggle : MonoBehaviour
{
    [SerializeField] private GameObject MainObject;
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
    public Sequence wiggleSeq;
    


    // Start is called before the first frame update
    void Start()
    {
        image = MainObject.GetComponent<Image>();
        
        PlayAnim();
    }

    void PlayAnim()
    {
        Sequence wiggleSeqTemp = DOTween.Sequence();

        wiggleSeq = wiggleSeqTemp;

        wiggleSeq.Append(MainObject.transform.DOScaleY( smallScale , duration));
        wiggleSeq.Join(MainObject.transform.DOScaleX( bigScale , duration));
        wiggleSeq.Join(MainObject.transform.DOLocalMoveY(move1 , duration ).SetRelative(true));
        wiggleSeq.Join(image.DOColor( Color1 , 0.01f));

        wiggleSeq.Append(MainObject.transform.DOLocalJump( transform.localPosition , move2 , 1 , 0.3f , true));
        wiggleSeq.Join(MainObject.transform.DOScaleY( 1f , duration).SetEase(Ease.OutBack));
        wiggleSeq.Join(MainObject.transform.DOScaleX( 1f , duration).SetEase(Ease.OutBack));
        wiggleSeq.Join(image.DOColor( Color2 , duration - 0.05f).SetLoops(2 , LoopType.Yoyo));

        wiggleSeq.AppendInterval(0.5f);

        wiggleSeq.OnComplete(() => wiggleSeq.Restart());

        wiggleSeq.Play();
    }

}
