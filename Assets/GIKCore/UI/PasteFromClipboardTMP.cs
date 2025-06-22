using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GIKCore.Net;
using TMPro;

namespace GIKCore.UI
{
    [DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(TMP_InputField))]
    public class PasteFromClipboardTMP : GameListener
    {
        // Fields
        [SerializeField] private TMP_InputField m_InputField;

        // Methods
        public override bool ProcessNetData(int id, object data)
        {
            if (base.ProcessNetData(id, data)) return true;
            switch (id)
            {
                case NetData.WEBGL_PASTED:
                    {
                        string pastedData = (string)data;
                        if (m_InputField != null && m_InputField.isFocused)
                            m_InputField.text = pastedData;
                        break;
                    }
            }
            return false;
        }

        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            if (m_InputField == null)
                m_InputField = GetComponent<TMP_InputField>();
        }

        //void Start() { }

        // Update is called once per frame
        //void Update() { }
    }
}