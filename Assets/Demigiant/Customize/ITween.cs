using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ITween : MonoBehaviour
{
    // Fields
    [SerializeField] [Min(0)] protected float duration = 0.3f;
    [SerializeField] [Min(0)] protected float reverseDuration = 0f;
    [SerializeField] [Min(0)] protected float delayBeforePlay = 0f;

    [SerializeField]
    [Min(-1)]
    [Tooltip("-1: loop infinity; 0: run only one time; 1 or higher: loop with times.\n Ex: loop = 1 => 1 run + 1 loop = 2 times")]
    protected int m_Loop = 0;

    [SerializeField] protected bool m_AutoplayOnEnable = true;

    [SerializeField]
    [Tooltip("if set TRUE, we will kill tween from the running tween list before run tween")]
    protected bool m_DoKill = true;

    [SerializeField]
    [Tooltip("Execute when gameobject become enable")]
    private UnityEvent m_OnEnable;

    [SerializeField]
    [Tooltip("Execute when Play function call and before PlayTween function call")]
    private UnityEvent m_OnPlay;

    [SerializeField]
    [Tooltip("Execute when PlayTween function call, right after tween start")]
    private UnityEvent m_OnTweenStart;

    [SerializeField]
    [Tooltip("Execute right after tween complete")]
    private UnityEvent m_OnTweenComplete;

    // Values
    public bool onTween { get; protected set; } = false;

    // Methods
    /// <summary>
    /// -1: loop infinity; 0: run only one time; 1 or higher: loop with times.
    /// <br></br>
    /// Ex: loop = 1 => 1 run + 1 loop = 2 times
    /// </summary>    
    public void SetLoop(int loop) { m_Loop = Mathf.Max(loop, -1); }
    public void SetDuration(float d) { duration = Mathf.Max(d, 0); }
    public void SetReverseDuration(float rd) { reverseDuration = Mathf.Max(rd, 0); }

    /// <summary>Execute when gameobject become enable</summary>    
    public void AddOnEnableEvent(UnityAction ua)
    {
        if (m_OnEnable == null)
            m_OnEnable = new UnityEvent();
        m_OnEnable.AddListener(ua);
    }
    /// <summary>Execute when Play function call and before PlayTween function call</summary> 
    public void AddOnPlayEvent(UnityAction ua)
    {
        if (m_OnPlay == null)
            m_OnPlay = new UnityEvent();
        m_OnPlay.AddListener(ua);
    }
    /// <summary>Execute when PlayTween function call, right after tween start</summary>
    public void AddOnTweenStartEvent(UnityAction ua)
    {
        if (m_OnTweenStart == null)
            m_OnTweenStart = new UnityEvent();
        m_OnTweenStart.AddListener(ua);
    }
    /// <summary>Execute right after tween complete</summary>
    public void AddOnTweenCompleteEvent(UnityAction ua)
    {
        if (m_OnTweenComplete == null)
            m_OnTweenComplete = new UnityEvent();
        m_OnTweenComplete.AddListener(ua);
    }
    public void InvokeEnableEvent()
    {
        if (m_OnEnable != null) m_OnEnable.Invoke();
    }
    public void InvokePlayEvent()
    {
        if (m_OnPlay != null) m_OnPlay.Invoke();
    }
    public void InvokeTweenStartEvent()
    {
        if (m_OnTweenStart != null) m_OnTweenStart.Invoke();
    }
    public void InvokeTweenCompleteEvent()
    {
        if (m_OnTweenComplete != null) m_OnTweenComplete.Invoke();
    }

    protected float FormatAlpha(float a)
    {
        if (a > 1f) return 1f;
        if (a < 0f) return 0f;
        return a;
    }

    public virtual void DoKill() { }
    public virtual void Play(float seconds = 0f)
    {
        InvokePlayEvent();

        StopAllCoroutines();
        if (seconds <= 0f) PlayTween();
        else StartCoroutine(WaitForPlay(seconds));
    }
    private IEnumerator WaitForPlay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        PlayTween();
    }
    protected virtual void PlayTween()
    {
        if (m_DoKill) DoKill();
    }

    // Use this for initialization
    //void Start() { }

    // Update is called once per frame
    //void Update() { }

    void OnEnable()
    {
        if (m_AutoplayOnEnable) Play(delayBeforePlay);
        InvokeEnableEvent();
    }

    void OnDestroy()
    {
        DoKill();
    }
}
