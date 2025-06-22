using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

[DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(RectTransform))]
public class Transition : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{    
    [SerializeField]
    protected Button m_Button = null;
    [SerializeField]
    protected RectTransform m_Target = null;
    [SerializeField]
    [Tooltip("How long until the target color/scale transitions to a new color/scale")]
    [Min(0)]
    protected float m_Duration = 0.1f;

    // Values
    private bool lastEnabled = true;
    private ScrollRect scrollRect = null;

    // Methods
    public bool interactable
    {
        get
        {            
            return m_Button.gameObject.activeSelf && m_Button.interactable;
        }
    }

    public virtual void Down() { }
    public virtual void Up() { }
    public virtual void Leave() { }

    public void OnPointerDown(PointerEventData eventData)
    {
        lastEnabled = m_Button.enabled;
        Down();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_Button.enabled = lastEnabled;
        Leave();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_Button.enabled = lastEnabled;
        Up();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (scrollRect != null) scrollRect.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (scrollRect != null) scrollRect.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (scrollRect != null) scrollRect.OnEndDrag(eventData);
    }

    protected virtual void Awake()
    {
        if (m_Button == null) m_Button = GetComponent<Button>();
        if (m_Target == null) m_Target = GetComponent<RectTransform>();
        scrollRect = GetComponentInParent<ScrollRect>();
    }   

    // Start is called before the first frame update
    //void Start() { }

    // Update is called once per frame
    //void Update() { }
}
