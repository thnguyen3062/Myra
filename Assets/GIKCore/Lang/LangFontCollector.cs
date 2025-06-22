using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace GIKCore.Lang
{
    [Serializable]
    public class LangFontProps
    {
        public TMP_FontAsset fontAsset;
        public Material fontMaterial;
    }

    public class LangFontCollector : MonoBehaviour
    {
        // Fields
        [Header("[Text Legacy]")]
        [SerializeField] private List<Font> m_FontDefault;
        [SerializeField] [Tooltip("English")] private List<Font> m_FontEN;
        [SerializeField] [Tooltip("Japanese")] private List<Font> m_FontJP;
        [SerializeField] [Tooltip("Korean")] private List<Font> m_FontKR;
        [SerializeField] [Tooltip("Traditional Chinese")] private List<Font> m_FontTC;
        [SerializeField] [Tooltip("Simplify Chinese")] private List<Font> m_FontSC;
        [Header("[Text Mesh Pro]")]
        [SerializeField] private List<LangFontProps> m_FontTMPDefault;
        [SerializeField] [Tooltip("English")] private List<LangFontProps> m_FontTMPEN;
        [SerializeField] [Tooltip("Japanese")] private List<LangFontProps> m_FontTMPJP;
        [SerializeField] [Tooltip("Korean")] private List<LangFontProps> m_FontTMPKR;
        [SerializeField] [Tooltip("Traditional Chinese")] private List<LangFontProps> m_FontTMPTC;
        [SerializeField] [Tooltip("Simplify Chinese")] private List<LangFontProps> m_FontTMPSC;

        // Methods
        public Font Get(int index = 0, int type = -1)
        {
            if (type == -1) type = LangHandler.lastType;
            List<Font> lst = m_FontDefault;
            switch (type)
            {
                case LangData.TYPE_EN:
                    lst = m_FontEN;
                    break;
                case LangData.TYPE_JP:
                    lst = m_FontJP;
                    break;
                case LangData.TYPE_KR:
                    lst = m_FontKR;
                    break;
                case LangData.TYPE_TC:
                    lst = m_FontTC;
                    break;
                case LangData.TYPE_SC:
                    lst = m_FontSC;
                    break;
                default:
                    lst = m_FontEN;
                    break;
            }

            if (lst != null)
            {
                if (index < 0 || index >= lst.Count)
                    index = lst.Count - 1;
                return lst[index];
            }
            return null;
        }

        public LangFontProps GetTMP(int index = 0, int type = -1)
        {
            if (type == -1) type = LangHandler.lastType;
            List<LangFontProps> lst = m_FontTMPDefault;
            switch (type)
            {
                case LangData.TYPE_EN:
                    lst = m_FontTMPEN;
                    break;
                case LangData.TYPE_JP:
                    lst = m_FontTMPJP;
                    break;
                case LangData.TYPE_KR:
                    lst = m_FontTMPKR;
                    break;
                case LangData.TYPE_TC:
                    lst = m_FontTMPTC;
                    break;
                case LangData.TYPE_SC:
                    lst = m_FontTMPSC;
                    break;
            }

            if (lst != null)
            {
                if (index < 0 || index >= lst.Count)
                    index = lst.Count - 1;
                return lst[index];
            }
            return null;
        }

        private void Awake()
        {
            Game.main.fontCollector = this;
        }

        // Start is called before the first frame update
        //void Start() { }

        // Update is called once per frame
        //void Update() { }
    }
}