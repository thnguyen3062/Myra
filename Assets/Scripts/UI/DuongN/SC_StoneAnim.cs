using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SC_StoneAnim : MonoBehaviour
{
    [SerializeField] private float bigScale = 1.3f;
    [SerializeField] private int vibrato;
    [SerializeField] private float elasticity = 0.5f;
    [SerializeField] private float duration = 0.2f;
    private Image image;
    public Sequence jumpSeq;
    [SerializeField] private Color color1 = Color.white;
    [SerializeField] private Color color2 = new Color(87f / 255f , 118f / 255f , 94f / 255f , 1f);
    [SerializeField] private float colorDuration = 0.5f;
    private Image childImage;
    

    // Start is called before the first frame update
    void Start()
    {
        childImage = null;
        image = transform.GetComponent<Image>();
        //if (transform.childCount > 0)
        //{
        //    Transform childTransform = transform.GetChild(0);
        //    childImage = childTransform.GetComponent<Image>();
        //    if (childImage == null)
        //    {
        //        Debug.LogError("Child does not have the Image component.");
        //    }
        //}
        //else
        //{
        //    Debug.Log("Stone khong co child.");
        //}
    }

    public void PlayAnim()
    {
        Sequence jumpSeqTemp = DOTween.Sequence();

        jumpSeq = jumpSeqTemp;
        
        jumpSeq.Append(transform.DOPunchScale( Vector3.one * bigScale , duration , vibrato ,  elasticity ));

        jumpSeq.OnComplete(() => jumpSeq.Restart());

        jumpSeq.Play();
    }

    public void StopAnim()
    {
        jumpSeq.Complete();
        transform.DOScale(Vector3.one, 0);
    }


    public void ChangeColor()
    {
        if(childImage != null)
        {
            // Change the color
            if(image.color == color1)
            {
                Sequence changeColor = DOTween.Sequence();
                changeColor.Append(image.DOColor(color2, colorDuration).SetEase(Ease.OutSine));
                changeColor.Join(childImage.DOColor(color2, colorDuration).SetEase(Ease.OutSine));

            }
            else
            {
                Sequence changeColor = DOTween.Sequence();
                changeColor.Append(image.DOColor(color1, colorDuration).SetEase(Ease.OutSine));
                changeColor.Join(childImage.DOColor(color1, colorDuration).SetEase(Ease.OutSine));
            }
        }
        else
        {
            // Change the color
            if(image.color == color1)
            {
                Sequence changeColor = DOTween.Sequence();
                changeColor.Append(image.DOColor(color2, colorDuration).SetEase(Ease.OutSine));
            }
            else
            {
                Sequence changeColor = DOTween.Sequence();
                changeColor.Append(image.DOColor(color1, colorDuration).SetEase(Ease.OutSine));
            }
        }
    }
}
