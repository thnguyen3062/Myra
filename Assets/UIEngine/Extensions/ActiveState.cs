using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace UIEngine.Extensions
{
    [DisallowMultipleComponent, ExecuteInEditMode]
    public class ActiveState : MonoBehaviour
    {
        // Fields
        [SerializeField] private GameObject m_Target;
        [SerializeField] private bool m_SetAsLastSibling = true;
        [SerializeField] private UnityEvent m_OnAwake, m_OnStart, m_OnEnable, m_OnFalseCallback;

        // Values
        public delegate void CallFunc();

        // Methods        
        public void SetAsLastSibling() { m_Target.transform.SetAsLastSibling(); }
        public void True() { m_Target.SetActive(true); }
        public void False(float seconds = 0f)
        {
            CallFunc func = () =>
            {
                m_Target.SetActive(false);
                if (m_OnFalseCallback != null) m_OnFalseCallback.Invoke();
            };
            StopAllCoroutines();
            if (seconds <= 0) func();
            else StartCoroutine(Delay(seconds, () => { func(); }));
        }
        public void DoDestroy(float seconds = 0f)
        {
            StopAllCoroutines();
            if (seconds <= 0) Destroy(m_Target);
            else StartCoroutine(Delay(seconds, () => { Destroy(m_Target); }));
        }

        private IEnumerator Delay(float seconds, CallFunc callback)
        {
            yield return new WaitForSeconds(seconds);
            if (callback != null) callback();
        }

        void Awake()
        {
            if (m_Target == null) m_Target = gameObject;
            if (m_OnAwake != null) m_OnAwake.Invoke();
        }

        // Start is called before the first frame update
        void Start()
        {
            if (m_OnStart != null) m_OnStart.Invoke();
        }

        // Update is called once per frame
        //void Update() { }

        void OnEnable()
        {
            if (m_SetAsLastSibling) SetAsLastSibling();
            if (m_OnEnable != null) m_OnEnable.Invoke();
        }
    }
}