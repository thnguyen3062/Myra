using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using GIKCore.Utilities;
using UnityEngine.Video;

namespace GIKCore.Bundle
{
    public class BundleRequest
    {
        private static Dictionary<string, List<string>> map = new Dictionary<string, List<string>>();
        private static List<string> def = new List<string>();

        public static void Parse()
        {
            string aJSON = BundleHandler.LoadDatabase("db_bundle_request", "GIKCore/DB/");
            JSONNode N = JSON.Parse(aJSON);
            JSONArray jArray = N.AsArray;

            map.Clear();
            def.Clear();
            for (int i = 0; i < jArray.Count; i++)
            {
                JSONObject o = jArray[i].AsObject;

                JSONArray a = o["bundle"].AsArray;
                List<string> bundle = new List<string>();
                for (int j = 0; j < a.Count; j++)
                    bundle.Add(a[j].Value);

                string gamecode = o["gamecode"].Value;
                if (gamecode.Equals("all"))
                {
                    def.AddRange(bundle);
                }
                else
                {
                    if (map.ContainsKey(gamecode))
                        map[gamecode] = bundle;
                    else map.Add(gamecode, bundle);
                }
            }
        }

        public static List<string> Get(string gamecode)
        {
            if (string.IsNullOrEmpty(gamecode)) return null;

            foreach (KeyValuePair<string, List<string>> pair in map)
            {
                if (gamecode.Contains(pair.Key))
                {
                    return new List<string>(pair.Value);//clone                
                }
            }

            return new List<string>(def);
        }
    }

    public class BundleVersion
    {
        public enum Reachability { Cloud, Downloading, FinishedDownloading }

        public string name;
        public string URI;
        public Hash128 hash;
        public AssetBundle bundle = null;
        public Reachability reachability = Reachability.Cloud;
        public UnityWebRequest www = null;
    }

    public class BundleHandler
    {
        private static BundleHandler instance;
        public static BundleHandler main
        {
            get
            {
                if (instance == null) instance = new BundleHandler();
                return instance;
            }
        }

        // Values    
        // name have to same as asset bundle you are defined
        public const string Database = "database";
        public const string Sounds = "sounds";
        public const string Tutorial1 = "tutorial1";
        public const string UI = "ui";
        public const string Sheet = "sheet";

        public const string SPLIT = "_hash_";

        public List<BundleVersion> lstCloud = new List<BundleVersion>();
        private Dictionary<string, BundleVersion> dictLocal = new Dictionary<string, BundleVersion>();//map name to assetbundle
        public int countConnect = 0;

        // Methods
        public void AddToLocal(BundleVersion av)
        {
            av.reachability = BundleVersion.Reachability.FinishedDownloading;
            if (dictLocal.ContainsKey(av.name))
            {
                dictLocal[av.name].bundle = av.bundle;
                dictLocal[av.name].hash = av.hash;
                dictLocal[av.name].URI = av.URI;
            }
            else dictLocal.Add(av.name, av);
        }

        public TextAsset GetTextAsset(string key, string assetName)
        {
            if (dictLocal.ContainsKey(key))
            {
                AssetBundle bundle = dictLocal[key].bundle;
                if (bundle != null)
                {
                    TextAsset data = bundle.LoadAsset<TextAsset>(assetName);
                    return data;
                }
            }
            return null;
        }

        public Sprite GetSprite(string key, string assetName)
        {    
            if (dictLocal.ContainsKey(key))
            {
                AssetBundle bundle = dictLocal[key].bundle;
                if (bundle != null)
                {
                    Sprite data = bundle.LoadAsset<Sprite>(assetName);
                    return data;
                }
            }
            return null;
        }

        public VideoClip GetVideo(string key, string assetName)
        {
            if (dictLocal.ContainsKey(key))
            {
                AssetBundle bundle = dictLocal[key].bundle;
                if (bundle != null)
                {
                    VideoClip data = bundle.LoadAsset<VideoClip>(assetName);
                    return data;
                }
            }
            return null;
        }

        public Texture GetTexture(string key, string assetName)
        {
            if (dictLocal.ContainsKey(key))
            {
                AssetBundle bundle = dictLocal[key].bundle;
                if (bundle != null)
                {
                    Texture data = bundle.LoadAsset<Texture>(assetName);
                    return data;
                }
            }
            Texture dataNext = Resources.Load<Texture>("Pack/Images/" + key + "/" + assetName);
            if (dataNext != null)
                return dataNext;
            return null;
        }
        public Texture2D GetTexture2D(string key, string assetName)
        {
            if (dictLocal.ContainsKey(key))
            {
                AssetBundle bundle = dictLocal[key].bundle;
                if (bundle != null)
                {
                    Texture2D data = bundle.LoadAsset<Texture2D>(assetName);
                    return data;
                }
            }
            Texture2D dataNext = Resources.Load<Texture2D>("Pack/Images/" + key + "/" + assetName);
            if (dataNext != null)
                return dataNext;
            return null;
        }

        public Sprite[] GetSpriteMultiple(string key, string assetName)
        {
            if (dictLocal.ContainsKey(key))
            {
                AssetBundle bundle = dictLocal[key].bundle;
                if (bundle != null)
                {
                    Sprite[] data = bundle.LoadAssetWithSubAssets<Sprite>(assetName);
                    return data;
                }
            }
            return null;
        }
        public Material GetMaterial(string key, string assetName)
        {
            if (dictLocal.ContainsKey(key))
            {
                AssetBundle bundle = dictLocal[key].bundle;
                if (bundle != null)
                {
                    Material data = bundle.LoadAsset<Material>(assetName);
                    return data;
                }
            }
            return null;
        }

        public AudioClip GetAudioClip(string key, string assetName)
        {
            if (dictLocal.ContainsKey(key))
            {
                AssetBundle bundle = dictLocal[key].bundle;
                if (bundle != null)
                {
                    AudioClip data = bundle.LoadAsset<AudioClip>(assetName);
                    return data;
                }
            }
            return null;
        }

        public Object GetObject(string key, string assetName)
        {
            if (dictLocal.ContainsKey(key))
            {
                AssetBundle bundle = dictLocal[key].bundle;
                if (bundle != null)
                {
                    Object data = bundle.LoadAsset(assetName);
                    return data;
                }
            }
            return null;
        }
        public Object[] GetAllObject(string key)
        {
            if (dictLocal.ContainsKey(key))
            {
                AssetBundle bundle = dictLocal[key].bundle;
                if (bundle != null)
                {
                    Object[] data = bundle.LoadAllAssets();
                    return data;
                }
            }
            return null;
        }
        //------- GAME -------

        public static string LoadDatabase(string assetName, string path = "DB/")
        {
            TextAsset ret = null;
//#if (UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL || UNITY_EDITOR || UNITY_STANDALONE)
//            ret = main.GetTextAsset(Database, assetName);
//#endif
            if (ret == null) ret = IUtil.LoadTextAsset(path + assetName);
            if (ret != null) return ret.ToString();
            return "";
        }
        public static AudioClip LoadSound(string assetName,string key, string path = "Pack/Sounds/")
        {
            AudioClip ret = null;
#if (UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL || UNITY_EDITOR || UNITY_STANDALONE)
            ret = main.GetAudioClip(key, assetName);
#endif
            if (ret == null) ret = IUtil.LoadAudioClip("Pack/"+key+"/" + assetName);
            return ret;
        }
        public static Sprite[] LoadSpriteFrames(string assetName, string path = "Pack/Images/Sheet/")
        {
            Sprite[] ret = null;
#if (UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL || UNITY_EDITOR || UNITY_STANDALONE)
            ret = main.GetSpriteMultiple(Sheet, assetName);
#endif
            if (ret == null || ret.Length <= 0)
                ret = IUtil.LoadSpriteMultipe(path + assetName);
            return ret;
        }
        public static Sprite LoadSprite(string bundle, string assetName, string path = "Pack/Images/")
        {
            Sprite ret = null;
#if (UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL || UNITY_EDITOR || UNITY_STANDALONE)
            ret = main.GetSprite(bundle, assetName);
#endif
            if (ret == null)
            {
                ret = IUtil.LoadSprite(path + assetName);
            }

            return ret;
        }
    }
}