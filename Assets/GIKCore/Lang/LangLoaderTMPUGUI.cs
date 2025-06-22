using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace GIKCore.Lang
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LangLoaderTMPUGUI : LangLoader
    {
        // Fields
        [SerializeField] protected TextMeshProUGUI m_TxtTarget;
        [SerializeField] protected bool m_AllowChangeFont = true;
        [SerializeField] protected LangFontProps m_FontDefault;
        [SerializeField][Tooltip("English")] private LangFontProps m_FontEN;
        [SerializeField][Tooltip("Japanese")] private LangFontProps m_FontJP;
        [SerializeField][Tooltip("Korean")] private LangFontProps m_FontKR;
        [SerializeField][Tooltip("Traditional Chinese")] private LangFontProps m_FontTC;
        [SerializeField][Tooltip("Simplify Chinese")] private LangFontProps m_FontSC;
        [SerializeField] protected string m_LastType = "";

        // Methods
        protected override void OnFont()
        {
            if (m_AllowChangeFont)
            {
                LangFontProps collector = (Game.main.fontCollector != null) ? Game.main.fontCollector.GetTMP() : null;
                TMP_FontAsset fontAsset = collector != null ? collector.fontAsset : null;
                Material fontMaterial = collector != null ? collector.fontMaterial : null;
                m_LastType = lastType + "";
                FontDataInfo fontData = FontData.Instance.GetFontDataInfo(lastType);
                if (fontData != null)
                {
                    if (fontData.fontAsset != null)
                    {
                        fontAsset = fontData.fontAsset;
                    }
                }

                LangFontProps custom = null;
                switch (lastType)
                {
                    case LangData.TYPE_EN:
                        {
                            if (m_FontEN != null && m_FontEN.fontAsset != null) custom = m_FontEN;
                            break;
                        }
                    case LangData.TYPE_JP:
                        {
                            if (m_FontJP != null && m_FontJP.fontAsset != null) custom = m_FontJP;
                            break;
                        }
                    case LangData.TYPE_KR:
                        {
                            if (m_FontKR != null && m_FontKR.fontAsset != null) custom = m_FontKR;
                            break;
                        }
                    case LangData.TYPE_TC:
                        {
                            if (m_FontTC != null && m_FontTC.fontAsset != null) custom = m_FontTC;
                            break;
                        }
                    case LangData.TYPE_SC:
                        {
                            if (m_FontSC != null && m_FontSC.fontAsset != null) custom = m_FontSC;
                            break;
                        }
                }

                if (custom != null)
                {//uu tien su dung custom
                    fontAsset = custom.fontAsset;
                    fontMaterial = custom.fontMaterial;
                }

                if (!CheckNULL())
                {
                    if (fontAsset != null)
                        m_TxtTarget.font = fontAsset;
                    if (fontMaterial != null)
                        m_TxtTarget.fontMaterial = fontMaterial;
                }
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
            if (CheckNULL()) m_TxtTarget = GetComponent<TextMeshProUGUI>();
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
