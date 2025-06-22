using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UIEngine.Extensions
{
    [DisallowMultipleComponent]
    public class DoubleClick : MonoBehaviour
    {
        // Fields
        [SerializeField] private RectTransform m_Target;
        [SerializeField] private bool m_CheckInside = true;
        [SerializeField] private float m_DoubleClickInterval = 0.5f;
        [SerializeField] private UnityEvent m_OnDoubleClick;

        // Values        
        private float secondClickTimeout = -1f;

        // Methods
        public void AddOnLongClickEvent(UnityAction ua)
        {
            if (m_OnDoubleClick == null)
                m_OnDoubleClick = new UnityEvent();
            m_OnDoubleClick.AddListener(ua);
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
            if (Input.GetMouseButtonDown(0))
            {
                bool clicking = false;
                if (m_CheckInside && m_Target != null)
                {
                    Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 localPoint = m_Target.InverseTransformPoint(worldPoint);
                    if (m_Target.rect.Contains(localPoint))
                        clicking = true;
                }
                else clicking = true;

                if (clicking)
                {
                    if (secondClickTimeout < 0)
                    {
                        // This is the first click, calculate the timeout
                        secondClickTimeout = Time.time + m_DoubleClickInterval;
                    }
                    else
                    {
                        // This is the second click, is it within the interval 
                        if (Time.time < secondClickTimeout)
                        {
                            // Invoke the event
                            if (m_OnDoubleClick != null)
                                m_OnDoubleClick.Invoke();

                            // Reset the timeout
                            secondClickTimeout = -1;
                        }
                    }
                }
            }

            // If we wait too long for a second click, just cancel the double click
            if (secondClickTimeout > 0 && Time.time >= secondClickTimeout)
            {
                secondClickTimeout = -1;
            }
        }
    }
}
