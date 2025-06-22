using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenSizeDelta : ITween
{
    // Fields
    [SerializeField] private RectTransform m_Target;
    [SerializeField] private Vector2 from = Vector2.one * 100;
    [SerializeField] private Vector2 to = Vector2.one * 100;

    // Methods
    public void SetFrom(Vector2 f) { from = f; }
    public void SetTo(Vector2 t) { to = t; }
    public void DoReset(Vector2 sizeDelta) { m_Target.sizeDelta = sizeDelta; }

    public override void DoKill()
    {
        m_Target.DOKill();
    }
    protected override void PlayTween()
    {
        base.PlayTween();
        m_Target.sizeDelta = from;
        if (m_Loop == 0)
        {
            m_Target.DOSizeDelta(to, duration)
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
            seq.Append(m_Target.DOSizeDelta(to, duration))
               .Append(m_Target.DOSizeDelta(from, reverseDuration))
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
            m_Target.DOSizeDelta(to, duration)
                    .OnPlay(() => { onTween = true; })
                    .OnComplete(() =>
                    {
                        Sequence seq = DOTween.Sequence();
                        seq.Append(m_Target.DOSizeDelta(from, reverseDuration))
                           .Append(m_Target.DOSizeDelta(to, duration))
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
