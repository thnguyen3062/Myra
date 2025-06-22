using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GIKCore.Attribute;
using GIKCore.Utilities;
using UnityEngine.Events;
using GIKCore.Tween;
using System.Security.Cryptography;

namespace GIKCore.UI
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class FollowSize : MonoBehaviour
    {
        public enum FollowTarget { Width, Height, Both }
        public enum FollowMode { Hand, Once, Always, Interval }

        // Fields
        [Help("Min Size <= Target Size + Offset <= Max Size")]
        [SerializeField] private RectTransform m_Target;
        [SerializeField] private FollowTarget m_FollowTarget = FollowTarget.Both;
        [Help("Hand: Handler by Hand; Once: run only once on Awake; Always: run once on Awake then called once per frame on Update", Type.Info)]
        [SerializeField] private FollowMode m_FollowMode = FollowMode.Always;
        [SerializeField][Min(0)] private float m_Interval = 0f;
        [SerializeField] private Size m_Min = new Size();
        [SerializeField] private Size m_Max = new Size();
        [SerializeField] private Size m_Offset = new Size();
        [SerializeField] private bool m_IgnoreScale = false;
        [SerializeField] UnityEvent m_OnValueChanged;

        // Values
        private RectTransform rt;
        private Size lastSize = new Size();
        private float countInterval;
        private bool once = false;

        // Methods
        private float targetWidth { get { return m_Target != null ? Mathf.Abs(m_Target.rect.width * (m_IgnoreScale ? 1f : m_Target.localScale.x)) : 0; } }
        private float targetHeight { get { return m_Target != null ? Mathf.Abs(m_Target.rect.height * (m_IgnoreScale ? 1f : m_Target.localScale.y)) : 0; } }

        public FollowSize ChangeTarget(RectTransform rt)
        {
            m_Target = rt;
            return this;
        }
        public FollowSize AddOnValueChangedEvent(UnityAction ua)
        {
            if (m_OnValueChanged == null)
                m_OnValueChanged = new UnityEvent();
            m_OnValueChanged.AddListener(ua);
            return this;
        }
        public FollowSize SetMin(Size s)
        {
            m_Min.Set(s);
            return this;
        }
        public FollowSize SetMax(Size s)
        {
            m_Max.Set(s);
            return this;
        }

        /// <summary>
        /// Update size imediately
        /// </summary>
        /// <param name="rebuildLayoutImmediate">true: force rebuild layout of target immediately</param>
        /// <returns></returns>
        public void Validate(bool rebuildLayoutImmediate = true)
        {
            if (m_Target != null && rt != null)
            {
                if (rebuildLayoutImmediate)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(m_Target);
                lastSize.Set(targetWidth, targetHeight);

                float width = lastSize.width + m_Offset.width;
                float height = lastSize.height + m_Offset.height;

                if (m_Min.width > 0) width = IMath.MinLimit(width, m_Min.width);
                if (m_Max.width > 0) width = IMath.MaxLimit(width, m_Max.width);

                if (m_Min.height > 0) height = IMath.MinLimit(height, m_Min.height);
                if (m_Max.height > 0) height = IMath.MaxLimit(height, m_Max.height);

                switch(m_FollowTarget)
                {
                    case FollowTarget.Width:
                        {
                            rt.sizeDelta = new Vector2(width, rt.sizeDelta.y);
                            break;
                        }
                    case FollowTarget.Height:
                        {
                            rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
                            break;
                        }
                    case FollowTarget.Both:
                        {
                            rt.sizeDelta = new Vector2(width, height);
                            break;
                        }
                }

                IUtil.InvokeEvent(m_OnValueChanged);
            }
        }
        private void CheckValidate()
        {
            if (lastSize.width != targetWidth || lastSize.height != targetHeight)
                Validate();
        }

        void Awake()
        {
            TryGetComponent(out rt);
            if (m_FollowMode != FollowMode.Hand)
                Validate();
        }

        // Start is called before the first frame update
        //void Start() { }

        void OnEnable()
        {
            once = false;
        }

        // Update is called once per frame
        void Update()
        {
            switch (m_FollowMode)
            {
                case FollowMode.Always:
                    {
                        Validate();
                        break;
                    }
                case FollowMode.Interval:
                    {
                        countInterval -= Time.deltaTime;
                        if (countInterval <= 0)
                        {
                            countInterval = m_Interval;
                            Validate();
                        }
                        break;
                    }
            }

#if UNITY_EDITOR
            if (m_FollowMode != FollowMode.Always)
                CheckValidate();
#endif
        }

        void LateUpdate()
        {
            if (!once && m_FollowMode == FollowMode.Once)
            {
                once = true;
                Validate();
            }
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            CheckValidate();
        }
#endif
    }
}
