using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UIEngine.Extensions.Attribute;

namespace UIEngine.Extensions
{
    [DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(ScrollRect))]    
    public class ScrollHelper : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        public delegate void Callback(Vector2 v2);
        public delegate void Callback2(int index);

        // Fields        
        [Help("ScrollHelper will normalize anchor for all child of its to (0, 1)", Type.Warning)]
        [SerializeField] private ScrollRect m_ScrollRect;
        [Help("Only available in drag mode", Type.Warning)]
        [SerializeField] private bool m_BounceBackToFitChild = false;

        // Values
        private Callback onScrollCompleteCB;
        private Callback2 onBounceBackCB;

        private List<RectTransform> lstChild = new List<RectTransform>();
        private Vector2 pointFrom = Vector2.zero, pointTo = Vector2.zero, maxAbs = Vector2.zero;
        private float mDuration = 0f, mTime = 0f;
        public bool onTween { get; private set; } = false;
        public bool onDrag { get; private set; } = false;

        // Methods
        public ScrollRect scrollRect { get { return m_ScrollRect; } }
        public RectTransform content { get { return m_ScrollRect.content; } }
        public RectTransform viewport { get { return m_ScrollRect.viewport; } }
        public int childCount { get { return lstChild.Count; } }

        /// <summary>
        /// Only available in drag mode
        /// </summary>
        /// <param name="func"></param>
        public void SetBounceBackCallback(Callback2 func) { onBounceBackCB = func; }

        /// <summary>
        /// Scroll the content to the percent position of ScrollRect in any direction.    
        /// <para>#Vector2 percent: A point which will be clamp between (0,0) and (1,1); 
        /// <br> ++ horizontal: percent(0, 0) -> content(0, 0), percent(1, 0) -> content(0 - MaxAbsX, 0);</br>
        /// <br> ++ vertical: percent(0, 0) -> content(0, 0 + MaxAbsY), percent(0, 1) -> content(0, 0)</br>
        /// <br> ++ view of bound: bottom-right(0, 0); top-right(0, 1); bottom-left(1, 0); top-left(1, 1)</br>
        /// </para>
        /// <para>#float duration: Scroll time in second, if you don't pass duration, the content will jump to the percent position of ScrollRect immediately.</para>
        /// <para>#Callback complete: the callback function will be execute when scroll complete</para>
        /// <para>#bool rebuildLayoutImmediate: force rebuild layout immediate if true; using this param in case of dynamic child (active, inactive, destroy, add) </para>
        /// </summary>  
        public void ScrollTo(Vector2 percent, float duration, Callback complete = null, bool rebuildLayoutImmediate = false)
        {
            if (rebuildLayoutImmediate) RebuildLayoutImmediate();
            if (maxAbs.x == 0 || maxAbs.y == 0) GetMaxAbs();
            if (percent.x < 0) percent.x = 0;
            if (percent.x > 1) percent.x = 1;
            if (percent.y < 0) percent.y = 0;
            if (percent.y > 1) percent.y = 1;

            onScrollCompleteCB = complete;
            pointFrom = content.anchoredPosition;
            pointTo = pointFrom;

            if (scrollRect.vertical) pointTo.y = (1 - percent.y) * maxAbs.y;
            if (scrollRect.horizontal) pointTo.x = 0 - percent.x * maxAbs.x;

            if (pointFrom.x != pointTo.x || pointFrom.y != pointTo.y)
            {
                if (duration > 0)
                {
                    mDuration = duration;
                    mTime = 0f;
                    onTween = true;
                }
                else
                {
                    content.anchoredPosition = pointTo;
                    InvokeComplete();
                }
            }
            else InvokeComplete();
        }
        public void ScrollToFirst(float duration, Callback complete = null, bool rebuildLayoutImmediate = false)
        {
            Vector2 percent = Vector2.zero;
            if (scrollRect.vertical) percent.y = 1;
            ScrollTo(percent, duration, complete, rebuildLayoutImmediate);
        }
        public void ScrollToLast(float duration, Callback complete = null, bool rebuildLayoutImmediate = false)
        {
            Vector2 percent = Vector2.zero;
            if (scrollRect.horizontal) percent.x = 1;
            ScrollTo(percent, duration, complete, rebuildLayoutImmediate);
        }
        public void ScrollToPosition(Vector2 to, float duration, Callback complete = null, bool rebuildLayoutImmediate = false)
        {
            ScrollTo(GetPercent(to, rebuildLayoutImmediate), duration, complete);
        }
        public void ScrollToPositionX(float x, float duration, Callback complete = null, bool rebuildLayoutImmediate = false)
        {
            Vector2 to = content.anchoredPosition;
            to.x = x;
            ScrollToPosition(to, duration, complete, rebuildLayoutImmediate);
        }
        public void ScrollToPositionY(float y, float duration, Callback complete = null, bool rebuildLayoutImmediate = false)
        {
            Vector2 to = content.anchoredPosition;
            to.y = y;
            ScrollToPosition(to, duration, complete, rebuildLayoutImmediate);
        }
        public void ScrollToOffset(Vector2 offset, float duration, Callback complete = null, bool rebuildLayoutImmediate = false)
        {
            Vector2 to = content.anchoredPosition + offset;
            ScrollTo(GetPercent(to, rebuildLayoutImmediate), duration, complete);
        }
        public void ScrollToOffsetX(float offsetX, float duration, Callback complete = null, bool rebuildLayoutImmediate = false)
        {
            Vector2 offset = new Vector2(offsetX, 0);
            ScrollToOffset(offset, duration, complete, rebuildLayoutImmediate);
        }
        public void ScrollToOffsetY(float offsetY, float duration, Callback complete = null, bool rebuildLayoutImmediate = false)
        {
            Vector2 offset = new Vector2(0, offsetY);
            ScrollToOffset(offset, duration, complete, rebuildLayoutImmediate);
        }
        public int ScrollToChild(int index, float duration, Callback complete = null, bool rebuildLayoutImmediate = false)
        {
            if (index < 0) index = 0;
            else if (index >= childCount) index = childCount - 1;

            Vector2 to = lstChild[index].anchoredPosition;
            to.x *= -1; to.y *= -1;

            ScrollTo(GetPercent(to), duration, complete, rebuildLayoutImmediate);
            return index;
        }
        public Vector2 GetPercent(Vector2 to, bool rebuildLayoutImmediate = false)
        {
            if (rebuildLayoutImmediate) RebuildLayoutImmediate();
            if (maxAbs.x == 0 || maxAbs.y == 0) GetMaxAbs();
            Vector2 percent = Vector2.zero;
            if (scrollRect.vertical)
            {
                percent.y = 1 - ((maxAbs.y != 0) ? (to.y / maxAbs.y) : 0);
            }
            if (scrollRect.horizontal)
            {
                percent.x = (maxAbs.x != 0) ? (-to.x / maxAbs.x) : 0;
            }

            if (percent.x < 0) percent.x = 0;
            if (percent.x > 1) percent.x = 1;
            if (percent.y < 0) percent.y = 0;
            if (percent.y > 1) percent.y = 1;

            return percent;
        }
        public void DoReset(bool resetPosition = true)
        {
            onScrollCompleteCB = null;
            onBounceBackCB = null;
            InvokeComplete();
            if (resetPosition) content.anchoredPosition = Vector2.zero;
        }

        private void BounceBackToFitChild()
        {
            if (m_BounceBackToFitChild && !onDrag && !onTween)
            {
                scrollRect.StopMovement();

                int index = 0;
                int numChild = childCount;

                if (scrollRect.vertical)
                {
                    float offsetY = content.anchoredPosition.y; //init posY is at y = 0 (local)
                    for (int i = 0; i < numChild; i++)
                    {
                        RectTransform go = lstChild[i];
                        float yTop = go.anchoredPosition.y + offsetY;
                        float yBot = yTop - go.rect.height;

                        if (yBot > 0f) continue;
                        if (yTop - go.rect.height * 0.5f < 0f)
                        {
                            index = i;
                            break;
                        }
                        if (yTop - go.rect.height * 0.5f >= 0f)
                        {
                            index = i + 1;
                            break;
                        }
                    }
                }
                if (scrollRect.horizontal)
                {
                    float offsetX = content.anchoredPosition.x;//init posX is at x = 0 (local)                    
                    for (int i = 0; i < numChild; i++)
                    {
                        RectTransform go = lstChild[i];
                        float xLeft = go.anchoredPosition.x + offsetX;
                        float xRight = xLeft + go.rect.width;
                        if (xRight < 0f) continue;
                        if (xLeft + go.rect.width * 0.5f <= 0f)
                        {
                            index = i + 1;
                            break;
                        }
                        if (xLeft + go.rect.width * 0.5f > 0f)
                        {
                            index = i;
                            break;
                        }
                    }
                }

                index = ScrollToChild(index, 0.2f);
                if (onBounceBackCB != null) onBounceBackCB(index);
            }
        }

        /// <summary>
        /// In case of dynamic child (active, inactive, destroy add), please call this function before you call any other function  
        /// </summary>
        public void RebuildLayoutImmediate()
        {
            lstChild.Clear();
            foreach (Transform child in content)
            {
                if (child.gameObject.activeSelf)
                {
                    RectTransform rt = child.GetComponent<RectTransform>();
                    Normalize(rt);
                    lstChild.Add(rt);
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            GetMaxAbs();
        }
        private void GetMaxAbs()
        {
            maxAbs.x = (content.rect.width > viewport.rect.width) ? (content.rect.width - viewport.rect.width) : 0f;
            maxAbs.y = (content.rect.height > viewport.rect.height) ? (content.rect.height - viewport.rect.height) : 0f;
        }
        private void Normalize(RectTransform rt)
        {
            rt.anchorMin = rt.anchorMax = rt.pivot = Vector2.up;
        }
        private void InvokeComplete()
        {
            onTween = false;
            mDuration = mTime = 0f;
            scrollRect.StopMovement();
            if (onScrollCompleteCB != null) onScrollCompleteCB(content.anchoredPosition);
        }

        // Start is called before the first frame update
        private void Awake()
        {
            if (m_ScrollRect == null) m_ScrollRect = GetComponent<ScrollRect>();
            Normalize(content);
        }
        void Start()
        {
            RebuildLayoutImmediate();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void FixedUpdate()
        {
            if (mDuration > 0f)
            {
                content.anchoredPosition = Vector2.Lerp(pointFrom, pointTo, mTime / mDuration);
                mTime += Time.deltaTime;
                if (mTime >= mDuration)
                {
                    content.anchoredPosition = pointTo;
                    InvokeComplete();
                }
            }
        }

        // Event Systems        
        public void OnBeginDrag(PointerEventData eventData)
        {
            onDrag = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            onDrag = false;
            BounceBackToFitChild();
        }
    }
}
