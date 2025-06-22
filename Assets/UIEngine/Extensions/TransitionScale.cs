using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TransitionScale : Transition
{
    // Fields
    [SerializeField]
    [Tooltip("When user press the target, the target will zoom to a scale. The final scale of the target equals (target original scale * zoomScale)")]
    private float m_ZoomScale = 0.8f;

    // Values
    private Vector3 originalScale = new Vector3();
    private Vector3 finaleScale = new Vector3();

    // Methods
    public override void Down()
    {
        if (!interactable) return;
        m_Target.DOScale(finaleScale, m_Duration);
    }
    public override void Up()
    {
        if (!interactable) return;
        m_Target.DOScale(originalScale, m_Duration);
    }
    public override void Leave()
    {
        if (!interactable) return;
        m_Target.DOKill();
        m_Target.localScale = originalScale;
    }

    protected override void Awake()
    {
        base.Awake();
        originalScale = m_Target.localScale;
        finaleScale = originalScale * m_ZoomScale;
    }
    // Start is called before the first frame update
    //void Start() { }

    // Update is called once per frame
    //void Update() { }
}
