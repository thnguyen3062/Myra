using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GIKCore.UI
{
    public class GoOnOff : MonoBehaviour
    {
        public enum ActionOnAwake { None, TurnOn, TurnOff };

        // Fields
        [SerializeField] private GameObject m_GoOn, m_GoOff;
        [SerializeField] private ActionOnAwake m_ActionOnAwake = ActionOnAwake.None;
        [SerializeField] private string m_Id;

        // Values
        private bool mOnline = false;

        // Methods
        public bool online { get { return mOnline; } }
        public string id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }
        public int idInt
        {
            get
            {
                int r = -1;
                int.TryParse(id, out r);
                return r;
            }
        }

        public void SetActive(bool active) { gameObject.SetActive(active); }
        public void TurnOn()
        {
            mOnline = true;
            m_GoOn.SetActive(true);
            m_GoOff.SetActive(false);
        }
        public void TurnOff()
        {
            mOnline = false;
            m_GoOn.SetActive(false);
            m_GoOff.SetActive(true);
        }
        public void Turn(bool on)
        {
            if (on) TurnOn();
            else TurnOff();
        }

        // System
        void Awake()
        {
            if (m_ActionOnAwake == ActionOnAwake.TurnOn) TurnOn();
            else if (m_ActionOnAwake == ActionOnAwake.TurnOff) TurnOff();
            else
            {
                mOnline = m_GoOn.activeSelf;
            }
        }

        // Use this for initialization
        //void Start() { }

        // Update is called once per frame
        //void Update() { }
    }
}