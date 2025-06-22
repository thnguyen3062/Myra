using UnityEngine;
using GIKCore.Utilities;
using DG.Tweening;

namespace GIKCore.Tween
{
    public class TweenScale : ITween
    {
        // Fields
        [SerializeField] private Transform m_Target;
        [SerializeField] private Vector3 from = Vector3.one;
        [SerializeField] private Vector3 to = Vector3.one;

        // Methods
        public ITween SetFrom(Vector3 f) { from = f; return this; }
        public ITween SetTo(Vector3 t) { to = t; return this; }
        public void DoReset(Vector3 scale) { m_Target.transform.localScale = scale; }

        public override void DoReset()
        {
            DoReset(from);
        }
        public override void DoKill()
        {
            m_Target.DOKill();
        }
        protected override void InitData()
        {
            base.InitData();
            DoReset();
        }
        protected override void RepeatZero()
        {
            base.RepeatZero();
            m_Target.DOScale(to, duration)
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
            seq.Append(m_Target.DOScale(to, duration))
               .Append(m_Target.DOScale(from, reverseDuration))
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
            m_Target.DOScale(to, duration)
                    .SetEase(m_Ease)
                    .OnComplete(() =>
                    {
                        Sequence seq = DOTween.Sequence(m_Target);
                        seq.Append(m_Target.DOScale(from, reverseDuration))
                           .Append(m_Target.DOScale(to, duration))
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
