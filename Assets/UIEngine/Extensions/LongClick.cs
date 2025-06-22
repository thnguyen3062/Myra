using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace UIEngine.Extensions
{
    [DisallowMultipleComponent, ExecuteInEditMode]
    public class LongClick : MonoBehaviour
    {
        //Fields
        [SerializeField] private RectTransform m_Target;
        [SerializeField] private bool m_CheckInside = true;
        [SerializeField] private float m_ClickDuration = 2f;
        [SerializeField] private UnityEvent m_OnLongClick;

        // Values
        private bool clicking = false;
        private float totalDownTime = 0f;

        // Methods
        public void AddOnLongClickEvent(UnityAction ua)
        {
            if (m_OnLongClick == null)
                m_OnLongClick = new UnityEvent();
            m_OnLongClick.AddListener(ua);
        }

        void Awake()
        {
            if (m_CheckInside && m_Target == null)
                m_Target = GetComponent<RectTransform>();
        }
        // Start is called before the first frame update
        //void Start() { }

        // Update is called once per frame
        void Update()
        {
            // Detect the first click
            if (Input.GetMouseButtonDown(0))
            {                
                totalDownTime = 0;
                if (m_CheckInside && m_Target != null)
                {
                    Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 localPoint = m_Target.InverseTransformPoint(worldPoint);
                    if (m_Target.rect.Contains(localPoint))
                        clicking = true;                    
                }
                else clicking = true;                               
            }

            // If a first click detected, and still clicking,
            // measure the total click time, and fire an event
            // if we exceed the duration specified
            if (clicking && Input.GetMouseButton(0))
            {
                totalDownTime += Time.deltaTime;
                if (totalDownTime >= m_ClickDuration)
                {
                    clicking = false;
                    if (m_OnLongClick != null)
                        m_OnLongClick.Invoke();                    
                }
            }

            // If a first click detected, and we release before the
            // duraction, do nothing, just cancel the click
            if (clicking && Input.GetMouseButtonUp(0))
            {
                clicking = false;
            }
        }
    }
}

