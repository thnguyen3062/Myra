using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GIKCore.Utilities;

namespace UIEngine.UIPool
{
    [DisallowMultipleComponent, ExecuteInEditMode]
    public class RecycleLayoutGroup : MonoBehaviour
    {
        // Fields
        [SerializeField] private Transform m_Group;
        [SerializeField] private List<GameObject> m_CellPrefabs;

        // Values
        private ICallback.CallFunc4<GameObject, object, int> cellDataCB = null;
        private ICallback.CallFunc2<GameObject> cellDataClearCB = null;
        private ICallback.CallFunc6<GameObject, int> cellPrefabCB = null;

        protected List<object> _adapter = new List<object>();
        public bool isInitialized { get; private set; } = false;

        // Methods
        public Transform group { get { return m_Group; } }

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
        /// <para>Notice: use function GetDeclarePrefab to get prefab you set in Unity Editor</para>
        /// </summary>
        public RecycleLayoutGroup SetCellPrefabCallback(ICallback.CallFunc6<GameObject, int> func) { cellPrefabCB = func; return this; }

        /// <summary>
        /// <para>Define the way how you work with each cell data.</para>
        /// <para></para>
        /// <para>Param: delegate (GameObject go, T data, int 'index of data in adapter')</para>
        /// </summary>
        public RecycleLayoutGroup SetCellDataCallback<T>(ICallback.CallFunc4<GameObject, T, int> func)
        {
            Init();
            cellDataCB = (GameObject go, object data, int index) =>
            {
                if (func != null) func(go, CastData<T>(data), index);
            };
            return this;
        }
        /** excecute when GameObject become inactive to clear all data of cell */
        public RecycleLayoutGroup SetCellDataClearCallback(ICallback.CallFunc2<GameObject> func) { cellDataClearCB = func; return this; }

        /// <summary>
        /// <para>Return the prefab you set in Unity Editor. Return null if not found</para>
        /// <para></para>
        /// <para>Param: Index of prefab in Unity Editor</para>
        /// </summary>
        public GameObject GetDeclarePrefab(int index)
        {
            if (m_CellPrefabs != null && index >= 0 && index < m_CellPrefabs.Count)
                return m_CellPrefabs[index];
            return null;
        }

        public RecycleLayoutGroup SetDeclarePrefab(params GameObject[] values)
        {
            m_CellPrefabs.Clear();
            foreach (GameObject go in values)
                m_CellPrefabs.Add(go);
            return this;
        }

        /// <summary>
        /// <para>Return a GameObject have data at 'index' in adapter. Return null if not found.</para>
        /// <para></para>
        /// <para>Param: Index of data in adapter.</para>
        /// </summary>
        public GameObject GetCellPrefab(int index)
        {
            if (cellPrefabCB != null)
                return cellPrefabCB(index);
            return GetDeclarePrefab(0);
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
            if (index >= 0 && index < _adapter.Count)
                return _adapter[index];
            return null;
        }
        protected void SetCellData(GameObject go, object data, int index)
        {
            if (cellDataCB != null) cellDataCB(go, data, index);
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

            int numElement = _adapter.Count;
            for (int i = 0; i < numElement; i++)
            {
                if (data.Equals(_adapter[i]))
                    return i;
            }

            return -1;
        }
        public int GetLastDataIndex()
        {
            return (_adapter.Count - 1);
        }

        private void AddCell(GameObject go, int index, string suffix)
        {
            go.name = "item_" + index + "_" + suffix;
            go.SetActive(true);
            //            
            SetCellData(go, GetCellData(index), index);
        }

        private GameObject GetPoolObject(int index)
        {
            GameObject prefab = GetCellPrefab(index);
            for (int i = 0; i < m_Group.childCount; i++)
            {
                GameObject goTmp = m_Group.GetChild(i).gameObject;
                if (!goTmp.activeSelf && goTmp.name.Contains(prefab.name))
                {
                    goTmp.name = prefab.name;
                    return goTmp;
                }
            }

            GameObject go = Instantiate(prefab, m_Group);
            go.name = prefab.name;
            return go;
        }

        /// <summary>
        /// <para>Set Adapter</para>
        /// <para>Param: List data with type T.</para>        
        /// </summary>
        public virtual RecycleLayoutGroup SetAdapter<T>(List<T> adapter)
        {
            Init();
            _adapter.Clear();
            if (adapter != null)
            {
                foreach (T data in adapter)
                    _adapter.Add(data);
            }

            int childCount = m_Group.childCount;
            for (int i = 0; i < childCount; i++)
            {
                GameObject go = m_Group.GetChild(i).gameObject;
                if (go != null)
                {
                    go.SetActive(false);
                    if (cellDataClearCB != null) cellDataClearCB(go);
                }
            }

            int numData = _adapter.Count;
            for (int i = 0; i < numData; i++)
            {
                GameObject go = GetPoolObject(i);
                AddCell(go, i, go.name);
            }
            return this;
        }
        public RecycleLayoutGroup ClearAdapter() { SetAdapter<object>(null); return this; }

        private void Init()
        {
            if (!isInitialized)
            {
                isInitialized = true;
            }
        }

        private void Awake()
        {
            if (m_Group == null) m_Group = transform;
            Init();
        }

        // Start is called before the first frame update
        //void Start() { }

        // Update is called once per frame
        //void Update() { }
    }
}
