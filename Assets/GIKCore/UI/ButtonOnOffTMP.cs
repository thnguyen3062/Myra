using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GIKCore.UI
{
    public class ButtonOnOffTMP : ButtonOnOff
    {
        // Fields
        [Space]
        [SerializeField] private TextMeshProUGUI m_TextCommon;
        [SerializeField] private TextMeshProUGUI m_TextOn, m_TextOff;

        // Methods
        public TextMeshProUGUI textCommon { get { return m_TextCommon; } }
        public TextMeshProUGUI textOn { get { return m_TextOn; } }
        public TextMeshProUGUI textOff { get { return m_TextOff; } }

        public override string labelCommon
        {
            get { return ""; }
            set { textCommon.text = value; }
        }
        public override string labelOn
        {
            get { return textOn.text; }
            set { textOn.text = value; }
        }
        public override string labelOff
        {
            get { return textOff.text; }
            set { textOff.text = value; }
        }

        // Start is called before the first frame update
        //void Start() { }

        // Update is called once per frame
        //void Update() { }
    }
}
