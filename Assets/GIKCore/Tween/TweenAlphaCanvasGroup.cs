using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using GIKCore.Utilities;

namespace GIKCore.Tween
{
    public class TweenAlphaCanvasGroup : ITween
    {
        // Fields
        [SerializeField] private CanvasGroup m_CanvasGroup;
        [Range(0f, 1f)]
        [SerializeField] private float from = 1f, to = 0f;

        // Methods
        public ITween SetFrom(float f) { from = IMath.LimitAmount(f); return this; }
        public ITween SetTo(float t) { to = IMath.LimitAmount(t); return this; }
        public void DoReset(float alpha) { m_CanvasGroup.alpha = IMath.LimitAmount(alpha); }

        public override void DoReset()
        {
            DoReset(from);
        }
        public override void DoKill() { m_CanvasGroup.DOKill(); }
        protected override void InitData()
        {
            base.InitData();
            DoReset();
        }
        protected override void RepeatZero()
        {
            base.RepeatZero();
            m_CanvasGroup.DOFade(to, duration)
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
            Sequence seq = DOTween.Sequence(m_CanvasGroup);
            seq.Append(m_CanvasGroup.DOFade(to, duration))
               .Append(m_CanvasGroup.DOFade(from, reverseDuration))
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
            m_CanvasGroup.DOFade(to, duration)
                         .SetEase(m_Ease)
                         .OnComplete(() =>
                         {
                             Sequence seq = DOTween.Sequence(m_CanvasGroup);
                             seq.Append(m_CanvasGroup.DOFade(from, reverseDuration))
                                .Append(m_CanvasGroup.DOFade(to, duration))
                                .SetLoops(m_Loop)
                                .SetEase(m_Ease)
                                .OnComplete(() =>
                                {
                                    onTween = false;
                                    IUtil.InvokeEvent(m_OnTweenComplete);
                                });
                         });
        }

        // Use this for initialization
        //void Start() { }

        // Update is called once per frame
        //void Update() { }    
    }
}
