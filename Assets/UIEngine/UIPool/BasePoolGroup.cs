using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UIEngine.Extensions.Attribute;
using GIKCore.Utilities;

namespace UIEngine.UIPool
{
    [DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(RectTransform))]
    public abstract class BasePoolGroup : MonoBehaviour
    {
        // Field
        [Help("- Pool Group will normalize anchor for all child of its to (0, 1).\n- You should set anchor top = 0, right = 0, bottom = 0, left = 0 to viewport. \n- You need generate a new GameObject in the scene with a preset template(your prefabs) and set them to field m_CellPrefabs", Type.Warning)]
        [SerializeField] private ScrollRect m_ScrollRect;
        [SerializeField] private List<GameObject> m_CellPrefabs;
        [Help("If cell size not set (x && y <= 0), it's will get default by prefabs template original size", Type.Info)]
        [SerializeField] private Vector2 m_CellSize;
        [SerializeField] protected Vector2 m_Spacing;

        // Values
        protected ICallback.CallFunc6<GameObject, int> cellPrefabCB = null;
        protected ICallback.CallFunc6<Vector2, int> cellSizeCB = null;
        protected ICallback.CallFunc4<GameObject, object, int> cellDataCB = null;
        protected ICallback.CallFunc2<Vector2> onValueChangedCB = null;
        protected ICallback.CallFunc2<Vector2> onScrollCompleteCB = null;

        protected List<PoolObject> listPool = new List<PoolObject>();
        protected List<object> adapter = new List<object>();
        private List<Vector2> listCellSize = new List<Vector2>();
        private List<Vector2> listCellPos = new List<Vector2>();

        private Vector2 cellSizeDefault = Vector2.zero;
        protected Vector2 pointFrom = Vector2.zero, pointTo = Vector2.zero;
        protected float _duration = 0f;
        private float _time = 0f;

        protected bool isInitialized = false;

        // Methods       
        public ScrollRect scrollRect { get { return m_ScrollRect; } }
        public RectTransform content { get { return m_ScrollRect.content; } }
        public RectTransform viewport { get { return m_ScrollRect.viewport; } }

        public T CastData<T>(object data)
        {
            try
            {
                return (T)System.Convert.ChangeType(data, typeof(T));
            }
            catch (System.InvalidCastException e)
            {
                return default(T);
            }
        }

        /// <summary>
        /// <para>In case of multiple prefabs, return prefab you want to work with data at 'index' in adapter.</para>
        /// <para></para>
        /// <para>Param: delegate (int 'index of data in adapter')</para>
        /// <para>Notice: use function GetCellPrefab to get prefab you set in Unity Editor</para>
        /// </summary>
        public BasePoolGroup SetCellPrefabCallback(ICallback.CallFunc6<GameObject, int> func) { cellPrefabCB = func; return this; }

        /// <summary>
        /// <para>Define the way how you work with each cell data.</para>
        /// <para></para>
        /// <para>Param: delegate (GameObject go, T data, int 'index of data in adapter')</para>
        /// </summary>
        public BasePoolGroup SetCellDataCallback<T>(ICallback.CallFunc4<GameObject, T, int> func)
        {
            Init();
            cellDataCB = (GameObject go, object data, int index) =>
            {
                if (func != null) func(go, CastData<T>(data), index);
            };
            return this;
        }

        /// <summary>
        /// <para>Callback executed when the scroll position of the slider is changed.</para>
        /// <para></para>
        /// <para>Param: delegate()</para>
        /// </summary>
        public BasePoolGroup SetOnValueChangedCallback(ICallback.CallFunc2<Vector2> func) { onValueChangedCB = func; return this; }

        /// <summary>
        /// <para>Call when <ScrollTo> function finished.</para>
        /// <para>Param: delegate()</para>
        /// </summary>
        public BasePoolGroup SetOnScrollCompleteCallback(ICallback.CallFunc2<Vector2> func) { onScrollCompleteCB = func; return this; }

        /// <summary>
        /// <para>Return the cell prefab you set in Unity Editor.</para>
        /// <para></para>
        /// <para>Param: Index of prefab in Unity Editor</para>
        /// </summary>
        public GameObject GetDeclarePrefab(int index)
        {
            if (m_CellPrefabs != null && index >= 0 && index < m_CellPrefabs.Count)
                return m_CellPrefabs[index];
            return null;
        }

        public BasePoolGroup SetDeclarePrefab(params GameObject[] values)
        {
            m_CellPrefabs.Clear();
            foreach (GameObject go in values)
                m_CellPrefabs.Add(go);
            return this;
        }

        /// <summary>
        /// <para>Return the prefab corresponding each data in adapter.</para>
        /// <para></para>
        /// <para>Param: Index of data in adapter.</para>
        /// </summary>
        protected GameObject GetCellPrefab(int index)
        {
            if (cellPrefabCB != null)
            {
                if (index != -1)
                    return cellPrefabCB(index);
                else
                    return null;
            }
                
            return GetDeclarePrefab(0);
        }

        /// <summary>
        /// <para>Return size of cell that you set in Unity Editor.</para>
        /// </summary>
        public Vector2 GetCellSize()
        {
            if (m_CellSize.x <= 0 || m_CellSize.y <= 0)
                return cellSizeDefault;
            return m_CellSize;
        }
        public BasePoolGroup SetCellSize(Vector2 v2) { m_CellSize = v2; return this; }

        /// <summary>
        /// <para>Return size of cell have data at 'index' in adapter; if not found, return size of cell that you set in Unity Editor.</para>        
        /// <para></para>
        /// <para>Param: Index of data in adapter.</para>
        /// <para>Notice: Make sure you call function SetAdapter first.</para>
        /// </summary>
        public Vector2 GetCellSize(int index)
        {
            if (index >= 0 && index < listCellSize.Count)
                return listCellSize[index];
            return GetCellSize();
        }
        protected void AddToListCellSize(Vector2 v2)
        {
            if (listCellSize != null) listCellSize.Add(v2);
            else listCellSize = new List<Vector2>() { v2 };
        }

        /// <summary>
        /// <para>Return anchored position of cell have data at 'index' in adapter.</para>
        /// <para>Return (0, 0) if index out of bounds.</para>
        /// <para></para>
        /// <para>Param: Index of data in adapter</para>
        /// </summary>    
        public Vector2 GetCellPos(int index)
        {
            if (index >= 0 && index < listCellPos.Count)
                return listCellPos[index];
            return Vector2.zero;
        }
        protected void AddToListCellPos(Vector2 v2)
        {
            if (listCellPos != null)
                listCellPos.Add(v2);
            else listCellPos = new List<Vector2>() { v2 };
        }

        /// <summary>
        /// <para>Return data at 'index' in adapter.</para>
        /// <para></para>
        /// <para>Param: Index of data in adapter</para>
        /// <para>Notice: Make sure you call function SetAdapter first.</para>
        /// </summary>
        public T GetCellDataAt<T>(int index)
        {
            object data = GetCellData(index);
            return CastData<T>(data);
        }
        private object GetCellData(int index)
        {
            if (index >= 0 && index < adapter.Count)
                return adapter[index];
            return null;
        }
        protected void SetCellData(GameObject go, object data, int index)
        {
            if (cellDataCB != null) cellDataCB(go, data, index);
        }

        /// <summary>
        /// <para>Return a GameObject have data at 'index' in adapter. Return null if not found.</para>
        /// <para></para>
        /// <para>Param: Index of data in adapter.</para>
        /// </summary>
        public GameObject GetGameObject(int index)
        {
            GameObject prefab = GetCellPrefab(index);

            if (prefab == null) return null;

            foreach (PoolObject po in listPool)
            {
                if (!po.isAvailable && po.prefabName.Equals(prefab.name) && po.index == index)
                    return po.go;
            }

            return null;
        }

        /// <summary>
        /// <para>Return index of data in adapter. Return -1 if not found</para>
        /// </summary>
        /// <returns>The index.</returns>
        /// <param name="data">Data.</param>
        public int GetDataIndex(object data)
        {
            if (data == null)
                return -1;

            int numElement = adapter.Count;
            for (int i = 0; i < numElement; i++)
            {
                if (data.Equals(adapter[i]))
                    return i;
            }

            return -1;
        }

        private void AddCell(GameObject go, int index)
        {
            go.name = "item_" + index;
            go.SetActive(true);
            //
            NormalizeCellUI(go, index);
            SetCellData(go, GetCellData(index), index);
        }
        protected void Normalize(RectTransform rect)
        {
            rect.anchorMin = Vector2.up;
            rect.anchorMax = Vector2.up;
            rect.pivot = Vector2.up;
        }
        protected void NormalizeCellUI(GameObject go, int index)
        {
            //anchor
            RectTransform rect = go.GetComponent<RectTransform>();
            Normalize(rect);

            //size
            rect.sizeDelta = GetCellSize(index);

            //pos
            rect.anchoredPosition = GetCellPos(index);
        }
        protected bool IsCellFitInMaskWidth(int index)
        {
            float offsetX = content.anchoredPosition.x;

            float xLeft = GetCellPos(index).x + offsetX;
            float xRight = xLeft + GetCellSize(index).x;

            if (xLeft >= 0f && xRight <= viewport.rect.width)
                return true;

            return false;
        }
        protected bool IsCellFitInMaskHeight(int index)
        {
            float offsetY = content.anchoredPosition.y;

            float yTop = GetCellPos(index).y + offsetY;
            float yBot = yTop - GetCellSize(index).y;

            if (yTop <= 0 && yBot >= -viewport.rect.height)
                return true;

            return false;
        }

        //#Work with pool
        protected bool IsCellVisible(int index)
        {
            foreach (PoolObject po in listPool)
            {
                if (!po.isAvailable && po.index == index)
                    return true;
            }
            return false;
        }
        protected void ResetPool()
        {
            foreach (PoolObject po in listPool)
            {
                po.RecycleObject();
            }
        }
        protected void GetPooledObject(int index)
        {
            GameObject prefab = GetCellPrefab(index);

            if (prefab == null)
            {
                Debug.LogWarning("The prefab you get with index " + index + " in adapter should not be NULL.");
                return;
            }

            foreach (PoolObject po in listPool)
            {
                if (po.isAvailable && po.prefabName.Equals(prefab.name))
                {
                    po.index = index;
                    po.isAvailable = false;
                    AddCell(po.go, index);
                    return;
                }
            }

            //no pool object inactive --> create new
            GameObject go = Instantiate(prefab, content);
            AddCell(go, index);

            //add to list pool
            PoolObject po2 = new PoolObject();
            po2.index = index;
            po2.prefabName = prefab.name;
            po2.isAvailable = false;
            po2.go = go;
            listPool.Add(po2);
            //
            return;
        }

        /// <summary>
        /// Only work with reference value as class
        /// </summary>
        /// <returns></returns>
        public BasePoolGroup ReloadDataToVisibleCell()
        {
            foreach(PoolObject po in listPool)
            {
                if (!po.isAvailable)
                {
                    SetCellData(po.go, GetCellData(po.index), po.index);
                }
            }
            return this;
        }

        //#Virtual
        /// <summary>
        /// <para>Set Adapter</para>
        /// <para>Param1: List data with type T.</para>
        /// <para>Param2: boolean - true if you want to reset position to (0, 0), false if not.</para>
        /// </summary>
        public virtual BasePoolGroup SetAdapter<T>(List<T> adapter, bool resetPosition = true)
        {           
            Init();
            scrollRect.StopMovement();
            this.adapter.Clear();
            if (adapter != null)
            {
                foreach (T data in adapter)
                    this.adapter.Add(data);
            }
            if (this.adapter.Count <= 0) ScrollToFirst(0);//in case adapter empty => force sroll to (0, 0)
            return this;
        }
        public BasePoolGroup ClearAdapter() { SetAdapter<object>(null, true); return this; }

        protected virtual void CalculateSizeDelta()
        {
            listCellPos.Clear();
            listCellSize.Clear();
            if (viewport.rect.width <= 0 || viewport.rect.height <= 0)
                Canvas.ForceUpdateCanvases();
        }

        protected virtual void UpdateData()
        {
            if (onValueChangedCB != null)
                onValueChangedCB(content.anchoredPosition);
        }

        /// <summary>
        /// Scroll the content to the percent position of ScrollRect in any direction.    
        /// <para>#Vector2 percent: A point which will be clamp between (0,0) and (1,1); 
        /// <br> ++ horizontal: percent(0, 0) -> content(0, 0); percent(1, 0) -> content(0 - MaxAbsX, 0);</br>
        /// <br> ++ vertical: percent(0, 0) -> content(0, 0 + MaxAbsY); percent(0, 1) -> content(0, 0)</br>
        /// <br> ++ view of bound: bottom-right(0, 0); top-right(0, 1); bottom-left(1, 0); top-left(1, 1)</br>
        /// </para>
        /// <para>#float duration: Scroll time in second, if you don't pass duration, the content will jump to the percent position of ScrollRect immediately.</para>
        /// <para>#Callback complete: the callback function will be execute when scroll complete</para>
        /// <para>#bool rebuildLayoutImmediate: force rebuild layout immediate if true; using this param in case of dynamic child (active, inactive, destroy, add) </para>
        /// </summary>
        public BasePoolGroup ScrollTo(Vector2 percent, float duration, ICallback.CallFunc2<Vector2> complete = null)
        {
            if (percent.x < 0) percent.x = 0;
            if (percent.x > 1) percent.x = 1;
            if (percent.y < 0) percent.y = 0;
            if (percent.y > 1) percent.y = 1;

            SetOnScrollCompleteCallback(complete);

            float maxAbsX = (content.rect.width > viewport.rect.width) ? (content.rect.width - viewport.rect.width) : 0f;
            float maxAbsY = (content.rect.height > viewport.rect.height) ? (content.rect.height - viewport.rect.height) : 0f;

            pointFrom = content.anchoredPosition;
            pointTo = pointFrom;

            if (scrollRect.vertical) pointTo.y = (1 - percent.y) * maxAbsY;
            if (scrollRect.horizontal) pointTo.x = 0 - percent.x * maxAbsX;

            if (pointFrom.x != pointTo.x || pointFrom.y != pointTo.y)
            {
                if (duration > 0)
                {
                    _duration = duration;
                    _time = 0f;
                }
                else
                {
                    content.anchoredPosition = pointTo;
                    InvokeComplete();
                }
            }
            else InvokeComplete();
            return this;
        }

        public BasePoolGroup ScrollToFirst(float duration, ICallback.CallFunc2<Vector2> complete = null)
        {
            Vector2 percent = Vector2.zero;
            if (scrollRect.vertical) percent.y = 1;
            ScrollTo(percent, duration, complete);
            return this;
        }

        public BasePoolGroup ScrollToLast(float duration, ICallback.CallFunc2<Vector2> complete = null)
        {
            Vector2 percent = Vector2.zero;
            if (scrollRect.horizontal) percent.x = 1;
            ScrollTo(percent, duration, complete);
            return this;
        }

        public BasePoolGroup ScrollToOffset(Vector2 offset, float duration, ICallback.CallFunc2<Vector2> complete = null)
        {
            Vector2 to = content.anchoredPosition + offset;
            ScrollTo(GetPercent(to), duration, complete);
            return this;
        }

        public BasePoolGroup ScrollToOffsetX(float offsetX, float duration, ICallback.CallFunc2<Vector2> complete = null)
        {
            Vector2 offset = new Vector2(offsetX, 0);
            ScrollToOffset(offset, duration, complete);
            return this;
        }

        public BasePoolGroup ScrollToOffsetY(float offsetY, float duration, ICallback.CallFunc2<Vector2> complete = null)
        {
            Vector2 offset = new Vector2(0, offsetY);
            ScrollToOffset(offset, duration, complete);
            return this;
        }

        public BasePoolGroup ScrollToChild(int index, float duration, ICallback.CallFunc2<Vector2> complete = null, Vector2? offset = null, bool cancelIfFitWidth = false, bool cancelIfFitHeight = false)
        {
            if (offset == null) offset = Vector2.zero;
            if (cancelIfFitWidth && IsCellFitInMaskWidth(index)) return this;
            if (cancelIfFitHeight && IsCellFitInMaskHeight(index)) return this;

            Vector2 to = GetCellPos(index) + offset.Value;
            to.x *= -1; to.y *= -1;

            ScrollTo(GetPercent(to), duration, complete);
            return this;
        }
        /// <summary>
        /// Calculate a point which will be clamp between (0,0) and (1,1)
        /// <br> ++ horizontal: percent(0, 0) -> content(0, 0); percent(1, 0) -> content(0 - MaxAbsX, 0);</br>
        /// <br> ++ vertical: percent(0, 0) -> content(0, 0 + MaxAbsY); percent(0, 1) -> content(0, 0)</br>
        /// <br> ++ view of bound: bottom-right(0, 0); top-right(0, 1); bottom-left(1, 0); top-left(1, 1)</br>
        /// </summary>
        /// <param name="to"></param>
        /// <returns></returns>
        public Vector2 GetPercent(Vector2 to)
        {
            float maxAbsX = (content.rect.width > viewport.rect.width) ? (content.rect.width - viewport.rect.width) : 0f;
            float maxAbsY = (content.rect.height > viewport.rect.height) ? (content.rect.height - viewport.rect.height) : 0f;
            Vector2 anchor = Vector2.zero;

            if (scrollRect.vertical)
            {
                anchor.y = 1 - ((maxAbsY != 0) ? (to.y / maxAbsY) : 0);
            }
            if (scrollRect.horizontal)
            {
                anchor.x = (maxAbsX != 0) ? (-to.x / maxAbsX) : 0;
            }

            if (anchor.x < 0) anchor.x = 0;
            if (anchor.x > 1) anchor.x = 1;
            if (anchor.y < 0) anchor.y = 0;
            if (anchor.y > 1) anchor.y = 1;

            return anchor;
        }

        protected void InvokeComplete()
        {
            _duration = _time = 0f;
            scrollRect.StopMovement();
            UpdateData();
            if (onScrollCompleteCB != null)
                onScrollCompleteCB(content.anchoredPosition);
        }

        protected void CheckError()
        {
            if (scrollRect == null)
            {
                Debug.LogError("ScrollRect field of Pool Group is null or empty!");
                return;
            }

            if (content == null)
            {
                Debug.LogError("Content field of ScrollRect is null or empty!");
                return;
            }

            if (viewport == null)
            {
                Debug.LogError("Viewport field of ScrollRect is null or empty!");
                return;
            }
        }

        private void Init()
        {
            if (!isInitialized)
            {
                isInitialized = true;

                if (m_ScrollRect == null) m_ScrollRect = GetComponentInParent<ScrollRect>();
                Normalize(content);

                scrollRect.onValueChanged.AddListener(v2 =>
                {// v2 in normalize: [0, 1]                
                    UpdateData();
                });

                if (m_CellPrefabs != null && m_CellPrefabs.Count > 0)
                {
                    Vector2 sizeDelta = m_CellPrefabs[0].GetComponent<RectTransform>().sizeDelta;
                    cellSizeDefault.x = sizeDelta.x;
                    cellSizeDefault.y = sizeDelta.y;
                }
            }
        }

        protected virtual void Awake()
        {
            Init();
            CheckError();
        }

        protected virtual void Start() { }

        // Update is called once per frame
        protected virtual void Update() { }

        protected virtual void FixedUpdate()
        {
            if (_duration > 0)
            {
                content.anchoredPosition = Vector2.Lerp(pointFrom, pointTo, _time / _duration);
                UpdateData();
                _time += Time.deltaTime;
                if (_time >= _duration)
                {
                    content.anchoredPosition = pointTo;
                    InvokeComplete();
                }
            }
        }
    }
}
