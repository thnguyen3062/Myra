using UnityEngine;
using DG.Tweening;

public class TweenAnchoredPosition : ITween
{
    // Fields
    [SerializeField] private RectTransform m_RectTarget;
    [SerializeField] private Vector2 from = Vector2.zero;
    [SerializeField] private Vector2 to = Vector2.zero;

    // Methods
    public void SetFrom(Vector2 f) { from = f; }
    public void SetTo(Vector2 t) { to = t; }
    public void DoReset(Vector2 v2) { m_RectTarget.anchoredPosition = v2; }

    public override void DoKill()
    {
        m_RectTarget.DOKill();
    }
    protected override void PlayTween()
    {
        base.PlayTween();
        m_RectTarget.anchoredPosition = from;

        if (m_Loop == 0)
        {
            m_RectTarget.DOAnchorPos(to, duration)
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
            seq.Append(m_RectTarget.DOAnchorPos(to, duration))
               .Append(m_RectTarget.DOAnchorPos(from, reverseDuration))
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
            m_RectTarget.DOAnchorPos(to, duration)
                        .OnPlay(() => { onTween = true; })
                        .OnComplete(() =>
                        {
                            Sequence seq = DOTween.Sequence();
                            seq.Append(m_RectTarget.DOAnchorPos(from, reverseDuration))
                               .Append(m_RectTarget.DOAnchorPos(to, duration))
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
