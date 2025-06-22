using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GIKCore.Utilities;

namespace GIKCore.UI
{
    public class NetBlock : MonoBehaviour
    {
        // Fields
        [SerializeField] private GameObject m_GoCircle;

        // Values
        private float TIMEOUT_DEFAULT = 5f;//seconds
        private float timeoutMax = 5f;//default
        private float timeoutCount = 0f;

        // Methods
        public void SetTimeout(float timeout)
        {
            timeoutMax = timeout < 0 ? TIMEOUT_DEFAULT : timeout;
        }

        public void Show(float delay = 1f)
        {
            if (delay <= 0)
            {
                Hide();//forse reset all;
                m_GoCircle.SetActive(true);
                gameObject.SetActive(true);
            }
            else
            {
                timeoutCount = 0;//reset time;
                if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                    m_GoCircle.SetActive(false);//wait for delay
                    StartCoroutine(IUtil.Delay(() => { m_GoCircle.SetActive(true); }, delay));
                }
            }
        }
        public void ShowNow() { Show(0); }
        public void Hide()
        {
            StopAllCoroutines();
            timeoutCount = 0f;
            m_GoCircle.SetActive(false);
            gameObject.SetActive(false);
        }

        // Start is called before the first frame update
        //void Start() { }

        // Update is called once per frame
        void Update()
        {
            if (timeoutMax > 0)
            {
                timeoutCount += Time.deltaTime;
                if (timeoutCount >= timeoutMax)
                    Hide();
            }
        }
    }
}