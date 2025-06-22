using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using GIKCore.Attribute;
using GIKCore.Utilities;
using DG.Tweening;

namespace GIKCore.Tween
{
    public abstract class ITween : MonoBehaviour
    {
        // Fields
        [SerializeField][Min(0)] protected float duration = 0.3f;
        [SerializeField][Min(0)] protected float reverseDuration = 0f;
        [SerializeField][Min(0)] protected float delayBeforePlay = 0f;

        [Help("-1: repeat infinity; 0: run only one time; 1 or higher: repeat with (loop + 1) times.\n Ex: loop = 1 => 1 run + 1 loop = 2 times", Type.Info)]
        [SerializeField]
        protected int m_Loop = 0;

        [Help("Sets the ease of the tween; check out: https://easings.net/", Type.Info)]
        [SerializeField] protected Ease m_Ease = Ease.Linear;

        [SerializeField] protected bool m_AutoplayOnEnable = true;

        [SerializeField]
        [Tooltip("TRUE => we will kill tween from the running tween list before run tween")]
        protected bool m_DoKill = true;

        [SerializeField]
        [Tooltip("Execute when GameObject become enable")]
        protected UnityEvent m_OnEnable;

        [SerializeField]
        [Tooltip("Execute when call Play function and before call PlayTween function")]
        protected UnityEvent m_OnPlay;

        [SerializeField]
        [Tooltip("Execute when call PlayTween function, right after tween start")]
        protected UnityEvent m_OnTweenStart;

        [SerializeField]
        [Tooltip("Execute right after tween complete")]
        protected UnityEvent m_OnTweenComplete;

        // Values
        public bool onTween { get; protected set; } = false;

        public ITween SetActive(bool on) { gameObject.SetActive(on); return this; }

        // Methods
        /// <summary>
        /// -1: loop infinity; 0: run only one time; 1 or higher: loop with times.
        /// <br></br>
        /// Ex: loop = 1 => 1 run + 1 loop = 2 times
        /// </summary>    
        public ITween SetLoop(int loop) { m_Loop = Mathf.Max(loop, -1); return this; }
        public ITween SetDuration(float d) { duration = Mathf.Max(d, 0); return this; }
        public ITween SetReverseDuration(float rd) { reverseDuration = Mathf.Max(rd, 0); return this; }

        /// <summary>Execute when gameobject become enable</summary>    
        public ITween AddOnEnableEvent(UnityAction ua)
        {
            if (m_OnEnable == null)
                m_OnEnable = new UnityEvent();
            m_OnEnable.AddListener(ua);
            return this;
        }
        /// <summary>Execute when Play function call and before PlayTween function call</summary> 
        public ITween AddOnPlayEvent(UnityAction ua)
        {
            if (m_OnPlay == null)
                m_OnPlay = new UnityEvent();
            m_OnPlay.AddListener(ua);
            return this;
        }
        /// <summary>Execute when PlayTween function call, right after tween start</summary>
        public ITween AddOnTweenStartEvent(UnityAction ua)
        {
            if (m_OnTweenStart == null)
                m_OnTweenStart = new UnityEvent();
            m_OnTweenStart.AddListener(ua);
            return this;
        }
        /// <summary>Execute right after tween complete</summary>
        public ITween AddOnTweenCompleteEvent(UnityAction ua)
        {
            if (m_OnTweenComplete == null)
                m_OnTweenComplete = new UnityEvent();
            m_OnTweenComplete.AddListener(ua);
            return this;
        }

        public virtual void Play(float seconds = 0f)
        {
            IUtil.InvokeEvent(m_OnPlay);

            StopAllCoroutines();
            IUtil.ScheduleOnce(this, () => { PlayTween(); }, seconds);
        }

        public virtual void DoReset() { }
        public virtual void DoKill() { }
        protected virtual void PlayTween()
        {
            InitData();

            if (m_Loop == 0) RepeatZero();//run only one time
            else if (m_Loop < 0) RepeatForever();
            else if (m_Loop > 0) RepeatTimes();

            IUtil.InvokeEvent(m_OnTweenStart);
        }

        protected virtual void InitData()
        {
            if (m_DoKill) DoKill();
            onTween = true;
        }
        protected virtual void RepeatZero() { }
        protected virtual void RepeatForever() { }
        protected virtual void RepeatTimes() { }

        // Use this for initialization
        //void Start() { }

        // Update is called once per frame
        //void Update() { }

        void OnEnable()
        {
            IUtil.InvokeEvent(m_OnEnable);
            if (m_AutoplayOnEnable) Play(delayBeforePlay);
        }

        void OnDestroy()
        {
            DoKill();
        }
    }
}
