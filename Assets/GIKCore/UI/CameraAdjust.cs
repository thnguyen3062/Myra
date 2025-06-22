using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GIKCore.UI
{
    [DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(Camera))]
    public class CameraAdjust : MonoBehaviour
    {
        // Fields
        [SerializeField] private Camera m_Camera;
        [SerializeField] private CanvasAdjust m_CanvasAdjust;
        [SerializeField] private bool m_Block;

        // Values    
        public float aspectDef { get; private set; }
        public float aspectNow { get; private set; }
        public float orthosizeDef { get; private set; }
        public float orthosizeNow { get; private set; }
        public float scale { get; private set; }
        private int lastScreenWidth = 0, lastScreenHeight = 0;

        // Methods
        public Camera main { get { return m_Camera; } }

        public Vector3 ScreenToWorldPoint(Vector3 screenPoint, bool ignoreZ = true)
        {
            Vector3 worldPoint = m_Camera.ScreenToWorldPoint(screenPoint);
            if (ignoreZ) worldPoint.z = 0;
            return worldPoint;
        }

        public Vector3 ScreenToWorldPoint(float x, float y, bool ignoreZ = true)
        {
            Vector3 worldPoint = m_Camera.ScreenToWorldPoint(new Vector3(x, y));
            if (ignoreZ) worldPoint.z = 0;
            return worldPoint;
        }

        public Vector3 ScreenToWorldPoint(float x, float y, float z)
        {
            Vector3 worldPoint = m_Camera.ScreenToWorldPoint(new Vector3(x, y, z));
            return worldPoint;
        }

        public bool RectContainsScreenPoint(RectTransform target, Vector3 screenPoint)
        {
            if (target == null) return false;
            return RectContainsWorldPoint(target, ScreenToWorldPoint(screenPoint));
        }

        public bool RectContainsWorldPoint(RectTransform target, Vector3 worldPoint)
        {
            if (target == null) return false;
            Vector2 localPoint = target.InverseTransformPoint(worldPoint);
            return target.rect.Contains(localPoint);
        }

        private void Adjust()
        {
            if (m_Block) return;

            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;

            aspectDef = m_CanvasAdjust.DefaultAspect;
            aspectNow = (float)lastScreenWidth / lastScreenHeight;
            scale = aspectDef / aspectNow;

            orthosizeDef = m_CanvasAdjust.DefaultResolution.y / 200f;
            orthosizeNow = (aspectNow <= aspectDef) ? orthosizeDef * scale : orthosizeDef;
            m_Camera.orthographicSize = orthosizeNow;

            Vector3 to = transform.position;
            to.z = -2f * orthosizeNow;
            transform.position = to;
        }

        void Awake()
        {
            Game.main.cam = this;
            if (m_CanvasAdjust == null)
                m_CanvasAdjust = FindObjectOfType<CanvasAdjust>();
            if (m_CanvasAdjust != null)
                Adjust();
        }

        // Start is called before the first frame update
        //void Start() { }

        // Update is called once per frame
        void Update()
        {
            if (lastScreenWidth != Screen.width || lastScreenHeight != Screen.height)
                Adjust();
        }
    }
}
