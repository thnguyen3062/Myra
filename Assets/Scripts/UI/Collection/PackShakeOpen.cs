using DG.Tweening;
using GIKCore.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PackShakeOpen : MonoBehaviour
{
    // Fields
    [SerializeField] private Image m_WhiteImage;

    // Values

    // Methods
    public void DoOpen(ICallback.CallFunc complete)
    {
        
        //gameObject.SetActive(true);
        //Color newColor = new Color(1, 1, 1, 15f / 255f);
        //Sequence sequence = DOTween.Sequence();
        //sequence.Append(transform.DORotate(new Vector3(0, 0, 20), 0.5f).SetEase(Ease.InOutBounce).SetLoops(6, LoopType.Yoyo))
        //    .Insert(3, transform.DOScale(5, 1))
        //    .Insert(3, transform.GetComponent<Image>().DOColor(Color.clear, 4))
        //    .Insert(3, m_WhiteImage.DOColor(newColor, 2))
        //    .OnComplete(()=>
        //    {
        //        gameObject.transform.localScale = new Vector3(1, 1, 1);
        //        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        //        m_WhiteImage.color = new Color(1, 1, 1, 0);
                
        //        transform.DOKill(true);
        //        gameObject.SetActive(false);
        //    });
    }
}
