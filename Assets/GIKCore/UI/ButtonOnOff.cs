using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GIKCore.UI
{
    public class ButtonOnOff : MonoBehaviour
    {
        public enum ActionOnAwake { None, TurnOn, TurnOff };

        // Fields
        [SerializeField] private RectTransform m_RectTransform;
        [SerializeField] private Button m_Button;
        [SerializeField] private GameObject m_GoBoxOnOff, m_GoBoxDisable, m_GoBlock;
        [SerializeField] private Image m_ImageOn, m_ImageOff;
        [SerializeField]
        [Tooltip("TRUE: set BoxOnOff to deactivate when disable button")]
        private bool m_DeactivateWhenDisable = false;
        [SerializeField] private bool m_SwitchTargetGraphic = true;
        [SerializeField] private ActionOnAwake m_ActionOnAwake = ActionOnAwake.None;
        [SerializeField] private string m_Id;

        // Values
        public delegate void Callback(string id);

        private Callback onClickCallback = null;
        public bool online { get; private set; } = false;

        // Methods
        public void OnClick()
        {
            if (onClickCallback != null)
                onClickCallback(id);
        }

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
        public virtual string labelCommon
        {
            get { return ""; }
            set { }
        }
        public virtual string labelOn
        {
            get { return ""; }
            set { }
        }
        public virtual string labelOff
        {
            get { return ""; }
            set { }
        }
        public string labelAll
        {
            set { labelCommon = labelOn = labelOff = value; }
        }

        public RectTransform rectTransform { get { return m_RectTransform; } }
        public Vector2 anchoredPosition
        {
            get { return m_RectTransform.anchoredPosition; }
            set { m_RectTransform.anchoredPosition = value; }
        }

        public ButtonOnOff SetOnClickCallback(Callback func)
        {
            onClickCallback = func;
            return this;
        }
        public ButtonOnOff SetActive(bool active)
        {
            gameObject.SetActive(active);
            return this;
        }
        public ButtonOnOff SetActiveBoxDisable(bool active)
        {
            m_GoBoxDisable.SetActive(active);
            return this;
        }
        /// <summary>
        /// TRUE: set BoxOnOff to deactivate when disable button
        /// </summary>    
        public ButtonOnOff SetDeactivateWhenDisable(bool on)
        {
            m_DeactivateWhenDisable = on;
            return this;
        }

        /// <summary> Also call SetDisable function </summary>        
        public ButtonOnOff SetInteractable(bool on)
        {
            m_Button.interactable = on;
            SetDisable(!on);
            return this;
        }
        public ButtonOnOff SetDisable(bool on)
        {
            m_GoBlock.SetActive(on);
            if (m_DeactivateWhenDisable)
                m_GoBoxOnOff.SetActive(!on);
            return this;
        }

        public ButtonOnOff TurnOn()
        {
            online = true;
            m_ImageOn.gameObject.SetActive(true);
            m_ImageOff.gameObject.SetActive(false);
            if (m_SwitchTargetGraphic) m_Button.targetGraphic = m_ImageOn;
            return this;
        }
        public ButtonOnOff TurnOff()
        {
            online = false;
            m_ImageOn.gameObject.SetActive(false);
            m_ImageOff.gameObject.SetActive(true);
            if (m_SwitchTargetGraphic) m_Button.targetGraphic = m_ImageOff;
            return this;
        }
        public ButtonOnOff Turn(bool on)
        {
            if (on) return TurnOn();
            return TurnOff();
        }

        void Awake()
        {
            if (m_ActionOnAwake == ActionOnAwake.TurnOn) TurnOn();
            else if (m_ActionOnAwake == ActionOnAwake.TurnOff) TurnOff();
            else
            {
                online = m_ImageOn.gameObject.activeSelf;
            }
            id = m_Id;
        }

        // Use this for initialization
        //void Start() { }

        // Update is called once per frame
        //void Update() { }
    }
}