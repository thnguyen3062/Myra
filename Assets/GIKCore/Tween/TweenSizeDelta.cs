using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GIKCore.Utilities;
using DG.Tweening;

namespace GIKCore.Tween
{
    public class TweenSizeDelta : ITween
    {
        public enum Mode { None, KeepWidth, KeepHeight }
        // Fields
        [SerializeField] private RectTransform m_Target;
        [SerializeField] private Vector2 from = Vector2.one * 100;
        [SerializeField] private Vector2 to = Vector2.one * 100;
        [SerializeField] private Mode m_Mode = Mode.None;

        // Methods
        public ITween SetMode(Mode mode) { m_Mode = mode; return this; }
        public ITween SetFrom(Vector2 f) { from = f; return this; }
        public ITween SetTo(Vector2 t) { to = t; return this; }
        public void DoReset(Vector2 sizeDelta) { m_Target.sizeDelta = sizeDelta; }

        public override void DoReset()
        {
            DoReset(GetSize(from));
        }
        public override void DoKill() { m_Target.DOKill(); }
        protected override void InitData()
        {
            base.InitData();
            DoReset();
        }
        protected override void RepeatZero()
        {
            base.RepeatZero();
            m_Target.DOSizeDelta(GetSize(to), duration)
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
            seq.Append(m_Target.DOSizeDelta(GetSize(to), duration))
               .Append(m_Target.DOSizeDelta(GetSize(from), reverseDuration))
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
            m_Target.DOSizeDelta(GetSize(to), duration)
                    .SetEase(m_Ease)
                    .OnComplete(() =>
                    {
                        Sequence seq = DOTween.Sequence(m_Target);
                        seq.Append(m_Target.DOSizeDelta(GetSize(from), reverseDuration))
                           .Append(m_Target.DOSizeDelta(GetSize(to), duration))
                           .SetLoops(m_Loop)
                           .SetEase(m_Ease)
                           .OnComplete(() =>
                           {
                               onTween = false;
                               IUtil.InvokeEvent(m_OnTweenComplete);
                           });
                    });
        }

        private Vector2 GetSize(Vector2 v2)
        {
            if (m_Mode == Mode.KeepWidth)
            {
                float width = m_Target.sizeDelta.x;
                return new Vector2(width, v2.y);
            }
            else if (m_Mode == Mode.KeepHeight)
            {
                float height = m_Target.sizeDelta.y;
                return new Vector2(v2.x, height);
            }

            return v2;
        }

        // Start is called before the first frame update
        //void Start() { }

        // Update is called once per frame
        //void Update() { }
    }
}
