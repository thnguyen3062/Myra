using UnityEngine;
using DG.Tweening;

public class TweenScale : ITween
{
    // Fields
    [SerializeField] private Transform m_Target;
    [SerializeField] private Vector3 from = Vector3.one;
    [SerializeField] private Vector3 to = Vector3.one;

    // Methods
    public void SetFrom(Vector3 f) { from = f; }
    public void SetTo(Vector3 t) { to = t; }
    public void DoReset(Vector3 scale) { m_Target.transform.localScale = scale; }

    public override void DoKill()
    {
        m_Target.DOKill();
    }
    protected override void PlayTween()
    {
        base.PlayTween();
        m_Target.localScale = from;

        if (m_Loop == 0)
        {
            m_Target.DOScale(to, duration)
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
            seq.Append(m_Target.DOScale(to, duration))
               .Append(m_Target.DOScale(from, reverseDuration))
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
            m_Target.DOScale(to, duration)
                    .OnPlay(() => { onTween = true; })
                    .OnComplete(() =>
                    {
                        Sequence seq = DOTween.Sequence();
                        seq.Append(m_Target.DOScale(from, reverseDuration))
                           .Append(m_Target.DOScale(to, duration))
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
