using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent, RequireComponent(typeof(RectTransform))]
public class UISizeFitter : MonoBehaviour
{
    public enum FitterMode { EnvelopeParent, FitInParent, WidthControlsHeight, HeightControlsWidth };
    // Fields
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private RectTransform m_RectTarget;
    [SerializeField] private FitterMode m_FitterMode;

    // Values
    private float aspect, widthFactor, heightFactor;

    // Methods
    private void DoFitter(FitterMode mode)
    {
        switch (mode)
        {
            case FitterMode.EnvelopeParent:
                {
                    float width = widthFactor;
                    float height = (aspect != 0f) ? width / aspect : 0f;
                    if (height < heightFactor)
                    {
                        height = heightFactor;
                        width = height * aspect;
                    }
                    m_RectTarget.sizeDelta = new Vector2(width, height);
                    break;
                }
            case FitterMode.FitInParent:
                {
                    m_RectTarget.sizeDelta = new Vector2(widthFactor, heightFactor);
                    break;
                }
            case FitterMode.WidthControlsHeight:
                {
                    float width = widthFactor;
                    float height = (aspect != 0f) ? width / aspect : 0f;
                    m_RectTarget.sizeDelta = new Vector2(width, height);
                    break;
                }
            case FitterMode.HeightControlsWidth:
                {
                    float height = heightFactor;
                    float width = height * aspect;
                    m_RectTarget.sizeDelta = new Vector2(width, height);
                    break;
                }
        }
    }

    // Use this for initialization
    void Awake()
    {
        if (m_Canvas == null) m_Canvas = GetComponentInParent<Canvas>();
        if (m_RectTarget == null) m_RectTarget = GetComponent<RectTransform>();                
    }

    void Start()
    {
        aspect = (m_RectTarget.rect.height != 0) ? m_RectTarget.rect.width / m_RectTarget.rect.height : 0f;
        widthFactor = m_Canvas.pixelRect.width / m_Canvas.scaleFactor;
        heightFactor = m_Canvas.pixelRect.height / m_Canvas.scaleFactor;
        DoFitter(m_FitterMode);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

