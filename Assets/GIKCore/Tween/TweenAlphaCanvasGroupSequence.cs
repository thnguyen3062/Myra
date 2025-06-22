using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GIKCore.Utilities;

namespace GIKCore.Tween
{
    public class TweenAlphaCanvasGroupSequence : ITween
    {
        // Fields
        [SerializeField] private CanvasGroup m_CanvasGroup;

        [SerializeField][Range(0f, 1f)] private float from = 1f;

        [SerializeField][Range(0f, 1f)] private List<float> m_WaveStep = new List<float>();
        [SerializeField]
        [Min(0)]
        [Tooltip("Time for run tween each step, if you are not set, we will use Duration intead")]
        private List<float> m_TimeStep = new List<float>();

        // Methods
        public ITween SetFrom(float f) { from = IMath.LimitAmount(f); return this; }
        public ITween SetWaveStep(List<float> ws)
        {
            m_WaveStep.Clear();
            foreach (float f in ws)
            {
                m_WaveStep.Add(IMath.LimitAmount(f));
            }
            return this;
        }
        public ITween SetTimeStep(List<float> ts)
        {
            m_TimeStep.Clear();
            foreach (float time in ts)
            {
                m_TimeStep.Add(Mathf.Max(time, 0f));
            }
            return this;
        }
        public void DoReset(float alpha) { m_CanvasGroup.alpha = IMath.LimitAmount(alpha); }
        private float GetTimeStep(int index)
        {
            if (index >= 0 && index < m_TimeStep.Count)
                return m_TimeStep[index];
            return duration;
        }

        public override void DoKill() { m_CanvasGroup.DOKill(); }
        protected override void InitData()
        {
            if (m_DoKill) DoKill();
            DoReset(from);
        }
        protected override void PlayTween()
        {
            InitData();

            int num = m_WaveStep.Count;
            if (num > 0)
            {
                onTween = true;

                // in DGTween, 
                // loop = -1 => run infinity 
                // loop = 0 or 1 => run 1 time; 
                // lopp >= 2 => 1 run + (loop -1) times; Ex: loop = 2 => 1 run + 1 loop = 2 times
                // => when convert to custom tween, we need add 1 to loop
                int loop = (int)IMath.MinLimit(m_Loop + 1, -1);
                Sequence seq = DOTween.Sequence(m_CanvasGroup);
                for (int i = 0; i < num; i++)
                {
                    float to = m_WaveStep[i];
                    float duation = GetTimeStep(i);
                    seq.Append(m_CanvasGroup.DOFade(to, duation));
                }
                seq.SetLoops(loop)
                   .SetEase(m_Ease)
                   .OnComplete(() =>
                   {
                       onTween = false;
                       IUtil.InvokeEvent(m_OnTweenComplete);
                   });

                IUtil.InvokeEvent(m_OnTweenStart);
            }
        }

        // Start is called before the first frame update
        //void Start() { }

        // Update is called once per frame
        //void Update() { }
    }
}
