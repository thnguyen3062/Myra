using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GIKCore.Utilities;
using DG.Tweening;

namespace GIKCore.Tween
{
    public class TweenWorldPosition : ITween
    {
        // Fields
        [SerializeField] private Transform m_Target;
        [SerializeField] private Vector3 m_From = Vector3.zero;
        [SerializeField] private Vector3 m_To = Vector3.zero;
        [SerializeField] private bool m_IgnoreZ = true;
        [Space]
        [Header("Priority")]
        [SerializeField] private Transform m_TransformFrom;
        [SerializeField] private Transform m_TransformTo;

        // Methods
        public ITween SetFrom(Vector3 f) { m_From = f; return this; }
        public ITween SetTo(Vector3 t) { m_To = t; return this; }
        public ITween SetTransformFrom(Transform t) { m_TransformFrom = t; return this; }
        public ITween SetTransformTo(Transform t) { m_TransformTo = t; return this; }

        private Vector3 from
        {
            get
            {
                Vector3 v3 = (m_TransformFrom != null ? m_TransformFrom.position : m_From);
                if (m_IgnoreZ) v3.z = 0;
                return v3;
            }
        }
        private Vector3 to
        {
            get
            {
                Vector3 v3 = (m_TransformTo != null ? m_TransformTo.position : m_To);
                if (m_IgnoreZ) v3.z = 0;
                return v3;
            }
        }

        public void DoReset(Vector3 v3) { m_Target.position = v3; }

        public override void DoKill() { m_Target.DOKill(); }
        protected override void InitData()
        {
            base.InitData();
            DoReset(from);
        }
        protected override void RepeatZero()
        {
            base.RepeatZero();
            m_Target.DOMove(to, duration)
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
            seq.Append(m_Target.DOMove(to, duration))
               .Append(m_Target.DOMove(from, reverseDuration))
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
            m_Target.DOMove(to, duration)
                    .SetEase(m_Ease)
                    .OnComplete(() =>
                    {
                        Sequence seq = DOTween.Sequence(m_Target);
                        seq.Append(m_Target.DOMove(from, reverseDuration))
                           .Append(m_Target.DOMove(to, duration))
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
