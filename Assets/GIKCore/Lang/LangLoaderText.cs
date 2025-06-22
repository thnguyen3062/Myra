using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GIKCore.Lang
{
    [RequireComponent(typeof(Text))]
    public class LangLoaderText : LangLoader
    {
        // Fields
        [SerializeField] protected Text m_TxtTarget;
        [SerializeField] protected bool m_AllowChangeFont = true;
        [SerializeField] protected Font m_FontDefault;
        [SerializeField] [Tooltip("English")] private Font m_FontEN;
        [SerializeField] [Tooltip("Japanese")] private Font m_FontJP;
        [SerializeField] [Tooltip("Korean")] private Font m_FontKR;
        [SerializeField] [Tooltip("Traditional Chinese")] private Font m_FontTC;
        [SerializeField] [Tooltip("Simplify Chinese")] private Font m_FontSC;
        // Methods
        protected override void OnFont()
        {
            if (m_AllowChangeFont)
            {
                Font font = null;
                if (Game.main.fontCollector != null)
                    font = Game.main.fontCollector.Get();
                switch (lastType)
                {
                    case LangData.TYPE_EN:
                        if (m_FontEN != null) font = m_FontEN;
                        break;
                    case LangData.TYPE_JP:
                        if (m_FontJP != null) font = m_FontJP;
                        break;
                    case LangData.TYPE_KR:
                        if (m_FontKR != null) font = m_FontKR;
                        break;
                    case LangData.TYPE_TC:
                        if (m_FontTC != null) font = m_FontTC;
                        break;
                    case LangData.TYPE_SC:
                        if (m_FontSC != null) font = m_FontSC;
                        break;
                    default:
                        if (m_FontEN != null) font = m_FontEN;
                        break;
                }

                if (!CheckNULL() && font != null) m_TxtTarget.font = font;
            }            
        }
        protected override void OnLang()
        {
            base.OnLang();
            string s = LangHandler.Get(id: m_IdLang, defaultValue: m_Default, suffix: m_Suffix, prefix: m_Prefix, to: m_To);

            if (!string.IsNullOrEmpty(s))
                m_TxtTarget.text = lastText = s;
        }
        protected override bool CheckNULL()
        {
            return (m_TxtTarget == null);
        }
        protected override void Init()
        {
            if (CheckNULL()) m_TxtTarget = GetComponent<Text>();
        }

        protected override string GetText()
        {
            return CheckNULL() ? "" : m_TxtTarget.text;
        }

        // Start is called before the first frame update
        //void Start() { }

        // Update is called once per frame
        //void Update() { }
    }
}
