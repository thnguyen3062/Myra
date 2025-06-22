using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GIKCore.UI
{
    public class ButtonOnOffText : ButtonOnOff
    {
        // Fields
        [Space]
        [SerializeField] private Text m_TextCommon;
        [SerializeField] private Text m_TextOn, m_TextOff;

        // Methods
        public Text textCommon { get { return m_TextCommon; } }
        public Text textOn { get { return m_TextOn; } }
        public Text textOff { get { return m_TextOff; } }

        public override string labelCommon {
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
