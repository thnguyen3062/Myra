using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GIKCore.UI
{
    [DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(Canvas), typeof(CanvasScaler))]
    public class ICanvas : MonoBehaviour
    {
        // Fields
        [SerializeField] private Canvas m_Canvas;
        [SerializeField] private RectTransform m_PanelUI, m_PanelPopup;

        // Methods
        public Canvas root { get { return m_Canvas; } }
        public RectTransform panelUI { get { return m_PanelUI; } }
        public RectTransform panelPopup { get { return m_PanelPopup; } }
        public Rect rect { get { return panelUI.rect; } }

        void Awake()
        {
            Game.main.canvas = this;
            if (m_Canvas == null) m_Canvas = GetComponent<Canvas>();
        }

        // Start is called before the first frame update
        //void Start() { }

        // Update is called once per frame
        //void Update() { }
    }
}
