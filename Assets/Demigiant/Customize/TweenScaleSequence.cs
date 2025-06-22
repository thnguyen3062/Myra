using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenScaleSequence : ITween
{
    // Fields
    [SerializeField] private Transform m_Target;
    [SerializeField] private Vector3 from = Vector3.one;
    [SerializeField] private List<Vector3> m_WaveStep = new List<Vector3>();
    [SerializeField]
    [Min(0)]
    [Tooltip("Time for run tween each step, if you are not set, we will use Duration intead")]
    private List<float> m_TimeStep = new List<float>();

    // Methods
    public void SetFrom(Vector3 f) { from = f; }
    public void SetWaveStep(List<Vector3> ws)
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
    public void DoReset(Vector3 scale) { m_Target.transform.localScale = scale; }
    private float GetTimeStep(int index)
    {
        if (index >= 0 && index < m_TimeStep.Count)
            return m_TimeStep[index];
        return duration;
    }

    public override void DoKill()
    {
        m_Target.DOKill();
    }
    protected override void PlayTween()
    {
        base.PlayTween();
        m_Target.localScale = from;

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
            for(int i = 0; i < num; i++)
            {
                Vector3 to = m_WaveStep[i];
                float duation = GetTimeStep(i);
                seq.Append(m_Target.DOScale(to, duation));
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
