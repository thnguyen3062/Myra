using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UIEngine.Extensions
{
    [DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(RectTransform))]
    public class UIDragDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerClickHandler
    {
        public delegate void Callback(GameObject data);

        public enum SiblingEvent { None, PointerDown, BeginDrag }

        // Fields
        [SerializeField] RectTransform m_Rect;
        [SerializeField] SiblingEvent m_SiblingEvent = SiblingEvent.BeginDrag;
        [SerializeField] ScrollRect m_ScrollRect;

        // Values
        public bool allowDrag { get; set; } = true;
        public bool onDrag { get; private set; } = false;
        private Vector3 offsetDrag = Vector3.zero;       
        private Callback onBeginDragCB, onDragCB, onEndDragCB;
        private Callback onPointerEnterCB, onPointerExitCB, onPointerDownCB, onPointerClickCB;

        // Methods
        public UIDragDrop SetScrollRect(ScrollRect sr)
        {
            m_ScrollRect = sr;
            return this;
        }

        /// <summary>Callback(GameObject data)</summary>     
        public UIDragDrop SetOnBeginDragCallback(Callback func) { onBeginDragCB = func; return this; }
        /// <summary>Callback(GameObject data)</summary>
        public UIDragDrop SetOnDragCallback(Callback func) { onDragCB = func; return this; }
        /// <summary>Drop Event<br>Callback(GameObject data)</br></summary>
        public UIDragDrop SetOnEndDragCallback(Callback func) { onEndDragCB = func; return this; }

        /// <summary>Callback(GameObject data)</summary>
        public UIDragDrop SetOnPointerEnterCallback(Callback func) { onPointerEnterCB = func; return this; }
        /// <summary>Callback(GameObject data)</summary>
        public UIDragDrop SetOnPointerExitCallback(Callback func) { onPointerExitCB = func; return this; }
        /// <summary>Callback(GameObject data)</summary>
        public UIDragDrop SetOnPointerDownCallback(Callback func) { onPointerDownCB = func; return this; }
        /// <summary>Callback(GameObject data)</summary>
        public UIDragDrop SetOnPointerClickCallback(Callback func) { onPointerClickCB = func; return this; }

        public UIDragDrop DoReset()
        {
            allowDrag = onDrag = false;
            offsetDrag = Vector3.zero;
            onBeginDragCB = onDragCB = onEndDragCB = null;
            onPointerEnterCB = onPointerExitCB = onPointerDownCB = onPointerClickCB = null;
            return this;
        }

        public RectTransform rectTransform { 
            get { return m_Rect; }
            set { m_Rect = value; }
        }
        public Vector2 anchoredPosition
        {
            get { return rectTransform.anchoredPosition; }
            set { rectTransform.anchoredPosition = value; }
        }
        public void SetActive(bool active) { rectTransform.gameObject.SetActive(active); }        

        // Events Drag
        public void OnBeginDrag(PointerEventData eventData)
        {            
            if (allowDrag)
            {
                //if (m_SiblingEvent == SiblingEvent.BeginDrag)
                //    rectTransform.SetAsLastSibling();
                onDrag = true;
                Vector3 globalMousePos;
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out globalMousePos))
                {
                    offsetDrag.x = globalMousePos.x - rectTransform.position.x;
                    offsetDrag.y = globalMousePos.y - rectTransform.position.y;

                    //Debug.Log(globalMousePos.x + "-" + rectTransform.position.x + " offset:" + offsetDrag.x);
                    if (onBeginDragCB != null) onBeginDragCB(rectTransform.gameObject);
                }
            }

            m_ScrollRect?.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (allowDrag)
            {
                Vector3 globalMousePos;
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out globalMousePos))
                {
                    rectTransform.position = new Vector3(globalMousePos.x - offsetDrag.x, globalMousePos.y - offsetDrag.y, 0f);
                    if (onDragCB != null) onDragCB(rectTransform.gameObject);
                }
            }

            m_ScrollRect?.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (allowDrag)
            {
                onDrag = false;
                if (onEndDragCB != null) onEndDragCB(rectTransform.gameObject);
            }
            m_ScrollRect?.OnEndDrag(eventData);
        }

        // Event Pointer
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (allowDrag && onPointerEnterCB != null)
                onPointerEnterCB(rectTransform.gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (allowDrag && onPointerExitCB != null)
                onPointerExitCB(rectTransform.gameObject);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (onPointerDownCB != null)
            {
                onPointerDownCB(rectTransform.gameObject);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!onDrag)
            {
                if (onPointerClickCB != null) onPointerClickCB(rectTransform.gameObject);
            }
        }

        void Awake()
        {
            if (m_Rect == null) m_Rect = GetComponent<RectTransform>();
        }
    }
}