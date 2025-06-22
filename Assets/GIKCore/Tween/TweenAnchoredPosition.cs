using UnityEngine;
using GIKCore.Utilities;
using GIKCore.Attribute;
using DG.Tweening;

namespace GIKCore.Tween
{
    public class TweenAnchoredPosition : ITween
    {
        // Fields
        [SerializeField] private RectTransform m_RectTarget;
        [SerializeField] private Vector2 from = Vector2.zero;
        [SerializeField] private Vector2 to = Vector2.zero;
        [Space]
        [Header("Priority")]
        [SerializeField] private RectTransform m_RectFrom;
        [SerializeField] private RectTransform m_RectTo;

        // Methods
        public ITween SetFrom(Vector2 f) { from = f; return this; }
        public ITween SetTo(Vector2 t) { to = t; return this; }
        public ITween SetRectFrom(RectTransform rt) { m_RectFrom = rt; return this; }
        public ITween SetRectTo(RectTransform rt) { m_RectTo = rt; return this; }

        private Vector2 anchoredFrom
        {
            get { return (m_RectFrom != null ? m_RectFrom.anchoredPosition : from); }
        }
        private Vector2 anchoredTo
        {
            get { return (m_RectTo != null ? m_RectTo.anchoredPosition : to); }
        }

        public void DoReset(Vector2 v2) { m_RectTarget.anchoredPosition = v2; }

        public override void DoReset()
        {
            DoReset(anchoredFrom);
        }
        public override void DoKill() { m_RectTarget.DOKill(); }
        protected override void InitData()
        {
            base.InitData();
            DoReset();
        }
        protected override void RepeatZero()
        {
            base.RepeatZero();
            m_RectTarget.DOAnchorPos(anchoredTo, duration)
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
            Sequence seq = DOTween.Sequence(m_RectTarget);
            seq.Append(m_RectTarget.DOAnchorPos(anchoredTo, duration))
               .Append(m_RectTarget.DOAnchorPos(anchoredFrom, reverseDuration))
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
            m_RectTarget.DOAnchorPos(anchoredTo, duration)
                        .SetEase(m_Ease)
                        .OnComplete(() =>
                        {
                            Sequence seq = DOTween.Sequence(m_RectTarget);
                            seq.Append(m_RectTarget.DOAnchorPos(anchoredFrom, reverseDuration))
                               .Append(m_RectTarget.DOAnchorPos(anchoredTo, duration))
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
