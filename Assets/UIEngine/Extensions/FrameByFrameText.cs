using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UIEngine.Extensions
{
    [DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(Text))]    
    public class FrameByFrameText : MonoBehaviour
    {
        // Fields
        [SerializeField] private Text m_TxtTarget;
        [SerializeField] private List<string> m_Frames;
        [SerializeField] [Min(1)] private int m_FPS = 24;
        [SerializeField] [Tooltip("set loop < 0 will make the animation loop infinity")] [Min(-1)] private int m_Loop = -1;
        [SerializeField] [Min(0)] private int m_FrameStart = 0;
        [SerializeField] private bool m_AutoplayOnEnable = false;
        [SerializeField] private UnityEvent m_OnEnable, m_OnComplete;

        // Values   
        private int countLoop = 0;
        private int idxNext = 0;
        private float timeRun = 0f;
        private bool onPlay = false;

        // Methods
        public Text txtTarget { get { return m_TxtTarget; } }
        public int loop { get { return m_Loop; } }
        public int numFrame { get { return m_Frames.Count; } }
        public void Play(int loop = -1, int frameStart = 0)
        {
            Stop();
            if (m_TxtTarget == null || m_Frames == null || numFrame <= 0)
                return;
            m_Loop = countLoop = Mathf.Max(loop, -1);
            m_FrameStart = idxNext = Mathf.Max(frameStart, 0);
            onPlay = true;
            timeRun = 0f;

            SetNextFrame();
        }
        public void Stop() { onPlay = false; }
        public void Stop(bool active)
        {
            onPlay = false;
            SetActive(active);
        }
        public void SetActive(bool active) { m_TxtTarget.gameObject.SetActive(active); }
        public void GotoFrame(int frameIndex)
        {
            if (frameIndex < numFrame)
                m_TxtTarget.text = m_Frames[frameIndex];
        }
        public void GotoLastFrame()
        {
            GotoFrame(numFrame - 1);
        }
        public void GotoFirstFrame() { GotoFrame(0); }
        public void SetFrames(List<string> lstFrame)
        {
            if (lstFrame == null || lstFrame.Count <= 0)
                return;
            m_Frames = new List<string>(lstFrame);
        }
        public void AddFrames(List<string> lstFrame)
        {
            if (lstFrame == null || lstFrame.Count <= 0)
                return;
            if (m_Frames == null)
                m_Frames = new List<string>(lstFrame);
            else m_Frames.AddRange(lstFrame);
        }
        public void ClearFrames()
        {
            if (m_Frames != null)
                m_Frames.Clear();
        }
        public void SetFPS(int fps)
        {
            m_FPS = Mathf.Max(fps, 1);
        }
        public void AddCompleteListener(UnityAction call)
        {
            if (m_OnComplete == null)
                m_OnComplete = new UnityEvent();
            m_OnComplete.AddListener(call);
        }

        public void InvokeCompleteListener()
        {
            if (m_OnComplete != null) m_OnComplete.Invoke();
        }

        private void SetNextFrame()
        {
            if (idxNext < numFrame)
            {
                m_TxtTarget.text = m_Frames[idxNext];
                idxNext++;
            }
            else
            {
                idxNext = 0;
                if (countLoop >= 0)
                {
                    countLoop -= 1;
                    if (countLoop < 0)
                    {
                        Stop();
                        InvokeCompleteListener();
                        return;
                    }
                }

                InvokeCompleteListener();
                SetNextFrame();
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            if (m_TxtTarget == null) m_TxtTarget = GetComponent<Text>();
        }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        void FixedUpdate()
        {
            if (onPlay)
            {
                timeRun += Time.deltaTime;
                if (timeRun * m_FPS >= 1f)
                {//next frame
                    timeRun = 0f;
                    SetNextFrame();
                }
            }
        }

        void OnEnable()
        {
            if (m_AutoplayOnEnable) Play(m_Loop, m_FrameStart);
            if (m_OnEnable != null)
                m_OnEnable.Invoke();
        }       
    }
}