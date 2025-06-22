using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class TweenAlphaCanvasGroup : ITween
{
    // Fields
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [Range(0f, 1f)]
    [SerializeField] private float from = 1f, to = 0f;

    // Methods
    public void SetFrom(float f) { from = FormatAlpha(f); }
    public void SetTo(float t) { to = FormatAlpha(t); }
    public void DoReset(float alpha) { m_CanvasGroup.alpha = FormatAlpha(alpha); }    

    public override void DoKill()
    {
        m_CanvasGroup.DOKill();
    }
    protected override void PlayTween()
    {
        base.PlayTween();
        m_CanvasGroup.alpha = from;

        if (m_Loop == 0)
        {
            m_CanvasGroup.DOFade(to, duration)
                         .OnPlay(() => { onTween = true; })
                         .OnComplete(() =>
                         {
                             onTween = false;
                             InvokeTweenCompleteEvent();
                         });
        }
        else if (m_Loop == -1)
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(m_CanvasGroup.DOFade(to, duration))
               .Append(m_CanvasGroup.DOFade(from, reverseDuration))
               .SetLoops(-1)
               .OnPlay(() => { onTween = true; })
               .OnComplete(() =>
               {
                   onTween = false;
                   InvokeTweenCompleteEvent();
               });
        }
        else if (m_Loop >= 1)
        {
            m_CanvasGroup.DOFade(to, duration)
                         .OnPlay(() => { onTween = true; })
                         .OnComplete(() =>
                         {
                             Sequence seq = DOTween.Sequence();
                             seq.Append(m_CanvasGroup.DOFade(from, reverseDuration))
                                .Append(m_CanvasGroup.DOFade(to, duration))
                                .SetLoops(m_Loop)
                                .OnComplete(() =>
                                {
                                    onTween = false;
                                    InvokeTweenCompleteEvent();
                                });
                         });
        }
    }    

    // Use this for initialization
    //void Start() { }

    // Update is called once per frame
    //void Update() { }    
}
