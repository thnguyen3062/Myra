using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GIKCore.Net;

namespace GIKCore.UI
{
    [DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(Canvas), typeof(CanvasScaler))]
    public class CanvasAdjust : MonoBehaviour
    {
        // Fields        
        [SerializeField] private CanvasScaler m_CanvasScaler;
        [SerializeField] private bool m_AllowAdjust = true;        

        // Values    
        private int lastScreenWidth = 0, lastScreenHeight = 0;//in pixcels    

        // Methods
        public Vector2 DefaultResolution
        {
            get { return new Vector2(m_CanvasScaler.referenceResolution.x, m_CanvasScaler.referenceResolution.y); }
        }
        public float DefaultAspect
        {
            get
            {
                Vector2 v2 = DefaultResolution;
                return v2.x / v2.y;
            }
        }        

        private void Adjust()
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;

            float aspectDef = DefaultAspect;
            float aspectNow = (float)lastScreenWidth / lastScreenHeight;           

            float match = (aspectNow <= aspectDef) ? 0f : 1f;//left is default; 0 is match width; 1 is match height        
            m_CanvasScaler.matchWidthOrHeight = match;

            HandleNetData.QueueNetData(NetData.CANVAS_ADJUST, match);
        }

        // System
        void Awake()
        {                        
            if (m_CanvasScaler == null) m_CanvasScaler = GetComponent<CanvasScaler>();
            if (m_CanvasScaler != null && m_AllowAdjust)
                Adjust();
        }

        // Use this for initialization
        //void Start() { }

        // Update is called once per frame
        void Update()
        {
            if (m_AllowAdjust && (lastScreenWidth != Screen.width || lastScreenHeight != Screen.height))
                Adjust();
        }
    }
}