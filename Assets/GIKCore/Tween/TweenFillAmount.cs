using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GIKCore.Utilities;

namespace GIKCore.Tween
{
    public class TweenFillAmount : ITween
    {
        // Fields
        [SerializeField] private Image m_Target;
        [SerializeField][Range(0f, 1f)] private float from = 0f;
        [SerializeField][Range(0f, 1f)] private float to = 1f;

        // Methods
        public ITween SetFrom(float f) { from = IMath.LimitAmount(f); return this; }
        public ITween SetTo(float t) { to = IMath.LimitAmount(t); return this; }
        public void DoReset(float x) { m_Target.fillAmount = IMath.LimitAmount(x); }
        public override void DoKill() { m_Target.DOKill(); }
        protected override void InitData()
        {
            base.InitData();
            DoReset(from);
        }
        protected override void RepeatZero()
        {
            base.RepeatZero();
            m_Target.DOFillAmount(to, duration)
                    .SetEase(m_Ease)
                    .OnComplete(() =>
                    {
                        onTween = false;
                        IUtil.InvokeEvent(m_OnTweenComplete);
                    });
        }
        protected override void RepeatForever()
        {
            base.RepeatForever();
            Sequence seq = DOTween.Sequence(m_Target);
            seq.Append(m_Target.DOFillAmount(to, duration))
               .Append(m_Target.DOFillAmount(from, reverseDuration))
               .SetLoops(-1)
               .SetEase(m_Ease)
               .OnComplete(() =>
               {
                   onTween = false;
                   IUtil.InvokeEvent(m_OnTweenComplete);
               });
        }
        protected override void RepeatTimes()
        {
            base.RepeatTimes();
            m_Target.DOFillAmount(to, duration)
                    .SetEase(m_Ease)
                    .OnComplete(() =>
                    {
                        Sequence seq = DOTween.Sequence(m_Target);
                        seq.Append(m_Target.DOFillAmount(from, reverseDuration))
                           .Append(m_Target.DOFillAmount(to, duration))
                           .SetLoops(m_Loop)
                           .SetEase(m_Ease)
                           .OnComplete(() =>
                           {
                               onTween = false;
                               IUtil.InvokeEvent(m_OnTweenComplete);
                           });
                    });
        }

        // Start is called before the first frame update
        //void Start() { }

        // Update is called once per frame
        //void Update() { }
    }
}
