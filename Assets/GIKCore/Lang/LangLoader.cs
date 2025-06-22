using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using GIKCore.Utilities;

namespace GIKCore.Lang
{
    [DisallowMultipleComponent, ExecuteInEditMode]
    public class LangLoader : MonoBehaviour
    {
        public enum Mode { Lang, Font, Both}
        // Fields
        [SerializeField] protected Mode m_Mode = Mode.Both;
        [SerializeField] protected string m_IdLang;
        [SerializeField] protected LangData.To m_To = LangData.To.None;
        [SerializeField][Multiline] protected string m_Default = "";
        [SerializeField] protected string m_Prefix = "";
        [SerializeField] protected string m_Suffix = "";
        [SerializeField] protected bool m_AllowValidate = true;

        // Values
        protected int lastType = -1;
        protected string lastText = "";

        // Methods
        protected virtual void OnFont() { }
        protected virtual void OnLang() { }

        protected virtual string GetText() { return ""; }
        protected virtual bool CheckNULL() { return true; }
        protected virtual void Init() { }

        protected void Loader()
        {
            if (CheckNULL()) return;
            if (lastType != LangHandler.lastType)
            {
                lastType = LangHandler.lastType;
                switch (m_Mode)
                {
                    case Mode.Lang:
                        {
                            OnLang();
                            break;
                        }
                    case Mode.Font:
                        {
                            OnFont();
                            break;
                        }
                    case Mode.Both:
                        {
                            OnFont();
                            OnLang();
                            break;
                        }
                }
            }
        }

        void Awake()
        {
            Init();
            Loader();
        }

        // Start is called before the first frame update    
        //void Start() { }

        // Update is called once per frame
        void Update()
        {
            Loader();
#if UNITY_EDITOR
            UpdateSomething();
#endif
        }

#if UNITY_EDITOR
        private void Validate()
        {
            if (!m_AllowValidate) return;
            string target = GetText();

            if (string.IsNullOrEmpty(target))
            {
                m_IdLang = m_Prefix = m_Default = m_Suffix = "";
            }

            // check prefix
            if (!string.IsNullOrEmpty(target))
            {
                m_Prefix = "";
                List<string> prefix = new List<string>() { " " };
                foreach (string s in prefix)
                {
                    if (target.StartsWith(s))
                    {
                        target = target.Substring(s.Length, target.Length - s.Length);
                        if (!string.IsNullOrEmpty(target))
                            m_Prefix = s;
                        break;
                    }
                }
            }

            //check suffix
            if (!string.IsNullOrEmpty(target))
            {
                m_Suffix = "";
                List<string> suffix = new List<string>() { " : ", " :", ": ", ":", " ", "..." };//sort by length
                foreach (string s in suffix)
                {
                    if (target.EndsWith(s))
                    {
                        target = target.Substring(0, target.Length - s.Length);
                        if (!string.IsNullOrEmpty(target))
                            m_Suffix = s;
                        break;
                    }
                }
            }

            //check body
            if (!string.IsNullOrEmpty(target))
            {
                string aJSON = IUtil.LoadTextAsset2("GIKCore/DB/db_lang_new");
                JSONNode N = JSON.Parse(aJSON);
                JSONArray jArray = N.AsArray;

                bool found = false;
                string targetLower = target.ToLower();
                for (int i = 0; i < jArray.Count; i++)
                {
                    JSONObject o = jArray[i].AsObject;
                    string id = o["KEY"].Value;
                    string en = o["EN"].Value;
                    string jp = o["JP"].Value;
                    string kr = o["KR"].Value;
                    string tc = o["CN"].Value;
                    string sc = o["TW"].Value;

                    if (en.ToLower().Equals(targetLower) || 
                        jp.ToLower().Equals(targetLower) || 
                        kr.ToLower().Equals(targetLower) || 
                        tc.ToLower().Equals(targetLower) || 
                        sc.ToLower().Equals(targetLower))
                    {
                        found = true;
                        m_IdLang = id;
                        m_Default = en;
                        break;
                    }
                }

                if (!found)
                {
                    m_IdLang = "";
                    m_Default = target;
                }
            }
        }

        protected void UpdateSomething()
        {
            if (!CheckNULL())
            {
                string currentText = GetText();
                if (!lastText.Equals(currentText))
                {
                    lastText = currentText;
                    Validate();
                }
            }
        }

        void OnValidate()
        {
            Validate();
        }
#endif
    }
}