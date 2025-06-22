using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TweenFillAmount : ITween
{
    // Fields
    [SerializeField] private Image m_Target;
    [SerializeField] [Range(0f, 1f)] private float from = 0f;
    [SerializeField] [Range(0f, 1f)] private float to = 1f;

    // Methods
    public void SetFrom(float f)
    {
        from = f;
        if (from < 0f) from = 0f;
        if (from > 1f) from = 1f;
    }
    public void SetTo(float t)
    {
        to = t;
        if (to < 0f) to = 0f;
        if (to > 1f) to = 1f;
    }
    public void DoReset(float x)
    {
        if (x < 0f) x = 0f;
        if (x > 1f) x = 1f;
        m_Target.fillAmount = x;
    }
    public override void DoKill()
    {
        m_Target.DOKill();
    }
    protected override void PlayTween()
    {
        base.PlayTween();
        m_Target.fillAmount = from;

        if (m_Loop == 0)
        {
            m_Target.DOFillAmount(to, duration)
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
            seq.Append(m_Target.DOFillAmount(to, duration))
               .Append(m_Target.DOFillAmount(from, reverseDuration))
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
            m_Target.DOFillAmount(to, duration)
                    .OnPlay(() => { onTween = true; })
                    .OnComplete(() =>
                    {
                        Sequence seq = DOTween.Sequence();
                        seq.Append(m_Target.DOFillAmount(from, reverseDuration))
                           .Append(m_Target.DOFillAmount(to, duration))
                           .SetLoops(m_Loop)
                           .OnComplete(() =>
                           {
                               onTween = false;
                               InvokeTweenCompleteEvent();
                           });
                    });
        }
    }

    // Start is called before the first frame update
    //void Start() { }

    // Update is called once per frame
    //void Update() { }
}
