using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class TweenAnchoredPositionSequence : ITween
{
    // Fields
    [SerializeField] private RectTransform m_RectTarget;
    [SerializeField] private Vector2 from = Vector2.zero;
    [SerializeField] private List<Vector2> m_WaveStep = new List<Vector2>();
    [SerializeField]
    [Min(0)]
    [Tooltip("Time for run tween each step, if you are not set, we will use Duration intead")]
    private List<float> m_TimeStep = new List<float>();

    // Methods
    public void SetFrom(Vector3 f) { from = f; }
    public void SetWaveStep(List<Vector2> ws)
    {
        m_WaveStep.Clear();
        m_WaveStep.AddRange(ws);
    }
    public void SetTimeStep(List<float> ts)
    {
        m_TimeStep.Clear();
        foreach (float time in ts)
        {
            m_TimeStep.Add(Mathf.Max(time, 0f));
        }
    }
    public void DoReset(Vector2 v2) { m_RectTarget.anchoredPosition = v2; }
    private float GetTimeStep(int index)
    {
        if (index >= 0 && index < m_TimeStep.Count)
            return m_TimeStep[index];
        return duration;
    }

    public override void DoKill()
    {
        m_RectTarget.DOKill();
    }
    protected override void PlayTween()
    {
        base.PlayTween();
        m_RectTarget.anchoredPosition = from;

        int num = m_WaveStep.Count;
        if (num > 0)
        {
            // in DGTween, 
            // loop = -1 => run infinity 
            // loop = 0 or 1 => run 1 time; 
            // lopp >= 2 => 1 run + (loop -1) times; Ex: loop = 2 => 1 run + 1 loop = 2 times
            // => when convert to custom tween, we need add 1 to loop
            int loop = (m_Loop == -1) ? -1 : (m_Loop + 1);
            Sequence seq = DOTween.Sequence();
            for (int i = 0; i < num; i++)
            {
                Vector2 to = m_WaveStep[i];
                float duation = GetTimeStep(i);
                seq.Append(m_RectTarget.DOAnchorPos(to, duation));
            }
            seq.SetLoops(loop)
               .OnPlay(() => { onTween = true; })
               .OnComplete(() =>
               {
                   onTween = false;
                   InvokeTweenCompleteEvent();
               });
        }
    }    

    // Start is called before the first frame update
    //void Start() { }

    // Update is called once per frame
    //void Update() { }
}
