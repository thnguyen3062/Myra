using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using GIKCore.DB;
using GIKCore.Bundle;
using GIKCore.Utilities;

namespace GIKCore.Lang
{
    public class LangData
    {
        public enum To { None, Lower, Upper, UpperFistLetter }

        public const int TYPE_NONE = -1;
        ///// <summary> Viernamese </summary>
        //public const int TYPE_VI = 0;
        /// <summary> English </summary>
        public const int TYPE_EN = 1;
        /// <summary> Japanese </summary>
        public const int TYPE_JP = 2;
        /// <summary> Korean </summary>
        public const int TYPE_KR = 3;
        /// <summary> Traditional Chinese </summary>
        public const int TYPE_TC = 4;
        /// <summary> Simplify Chinese </summary>
        public const int TYPE_SC = 5;

        public LangData(string id) { this.id = id; }

        public string id { get; private set; }
        private Dictionary<int, string> map = new Dictionary<int, string>();

        public void Set(int type, string content)
        {
            if (string.IsNullOrEmpty(content)) content = "";
            if (map.ContainsKey(type)) map[type] = content;
            else map.Add(type, content);
        }
        public string Get(int type)
        {
            string ret = map.ContainsKey(type) ? map[type] : "";
            return ret;
        }
        public int Get(string s)
        {
            if (map.ContainsKey(TYPE_EN) && s.Equals(map[TYPE_EN]))
                return TYPE_EN;
            else if (map.ContainsKey(TYPE_JP) && s.Equals(map[TYPE_JP]))
                return TYPE_JP;
            else if (map.ContainsKey(TYPE_KR) && s.Equals(map[TYPE_KR]))
                return TYPE_KR;
            else if (map.ContainsKey(TYPE_TC) && s.Equals(map[TYPE_TC]))
                return TYPE_TC;
            else if (map.ContainsKey(TYPE_SC) && s.Equals(map[TYPE_SC]))
                return TYPE_SC;
            else return TYPE_NONE;
        }
    }

    public class LangHandler
    {
        // Values
        private static Dictionary<string, LangData> map = new Dictionary<string, LangData>();
        public static int lastType = LangData.TYPE_NONE;

        public static bool CheckType(int target) { return lastType == target; }
        public static string Get(string id, string defaultValue = "", string prefix = "", string suffix = "", int type = -1, LangData.To to = LangData.To.None)
        {
            if (type == -1) type = lastType;
            LangData ld = GetMap(id);
            string content = (ld != null) ? ld.Get(type) : "";
            string final = (content.IndexOf("#blank") == 0) ? "" : (prefix + (!string.IsNullOrEmpty(content) ? content : defaultValue) + suffix);
            return FormatLabel(final, to);
        }
        public static LangData GetMap(string id)
        {
            if (!string.IsNullOrEmpty(id) && map.ContainsKey(id))
                return map[id];
            return null;
        }
        public static void Set(int type)
        {
            GamePrefs.LastLang = lastType = type;
        }
        public static void Init()
        {
            if (GamePrefs.LastLang == LangData.TYPE_NONE)
            {
                GamePrefs.LastLang = LangData.TYPE_EN;
            }
            lastType = GamePrefs.LastLang;
            Parse();
        }
        public static void Parse()
        {
            string aJSON = BundleHandler.LoadDatabase("db_lang_new", "GIKCore/DB/");
            Debug.Log("hiennt " + string.IsNullOrEmpty(aJSON));
            TextAsset t = Resources.Load<TextAsset>("GIKCore/DB/db_lang_new");
            Debug.Log("hien " + string.IsNullOrEmpty(t.ToString()));
            JSONNode N = JSON.Parse(aJSON);
            JSONArray jArray = N.AsArray;

            map.Clear();
            for (int i = 0; i < jArray.Count; i++)
            {
                JSONObject o = jArray[i].AsObject;
                LangData data = new LangData(o["KEY"].Value);
                data.Set(LangData.TYPE_EN, o["EN"].Value);
                data.Set(LangData.TYPE_JP, o["JP"].Value);
                data.Set(LangData.TYPE_KR, o["KR"].Value);
                data.Set(LangData.TYPE_TC, o["CN"].Value);
                data.Set(LangData.TYPE_SC, o["TW"].Value);

                if (map.ContainsKey(data.id)) map[data.id] = data;
                else map.Add(data.id, data);
            }
        }

        public static string GameLabel(string code, int type = -1, LangData.To to = LangData.To.None)
        {
            if (type == -1) type = lastType;
            string ret = "";
            return FormatLabel(ret, to);
        }

        public static string FormatLabel(string ret, LangData.To to)
        {
            if (string.IsNullOrEmpty(ret)) return "";
            if (to == LangData.To.Lower)
                ret = ret.ToLower();
            else if (to == LangData.To.Upper)
                ret = ret.ToUpper();
            else if (to == LangData.To.UpperFistLetter)
                ret = IUtil.StringToUpperFirstLetter(ret);
            return ret;
        }

        public static string GetLangCode(int type = -1)
        {
            if (type == -1) type = lastType;
            switch(type)
            {
                case LangData.TYPE_EN: return "en";
                case LangData.TYPE_JP: return "jp";
                case LangData.TYPE_KR: return "kr";
                case LangData.TYPE_TC: return "tc";
                case LangData.TYPE_SC: return "sc";
            }
            return "";
        }
    }
}