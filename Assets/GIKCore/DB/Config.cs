using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

namespace GIKCore.DB
{
    public class ServerInfo
    {
        public string name { get; private set; } = "";
        public string host { get; private set; } = "";
        public int port { get; private set; } = 0;
        public int id { get; private set; } = -1;
        public int status { get; private set; } = 0;
        public string resource { get; private set; } = "";

        public ServerInfo(string host, int port, int id, int status = 0, string name = "", string resource = "")
        {
            this.host = host;
            this.port = port;
            this.id = id;
            this.status = status;
            this.resource = resource;
        }
    }

    public class Config
    {
        public static bool allowLog = Debug.isDebugBuild;
        public static int countConnect = 0;

        public static string DOMAIN = "https://h939info.heligame.vn";

        public static bool sv1000 { get; private set; } = true;

        private static List<ServerInfo> lstServer = new List<ServerInfo>();

        private static string mPingURL = "";
        public static string pingURL
        {
            get
            {
                if (string.IsNullOrEmpty(mPingURL))
                {
#if UNITY_WEBGL
                return WebGLNative.GetLocationHrefJS();
#else
                    return DOMAIN;
#endif
                }
                return mPingURL;
            }
        }
        private static string mPingURL1 = "";
        public static string pingURL1
        {
            get
            {
                if (string.IsNullOrEmpty(mPingURL1))
                {
#if UNITY_WEBGL
                return "";
#else
                    return "https://www.google.com/";
#endif
                }
                return mPingURL1;
            }
        }
        private static string mPingURL2 = "";
        public static string pingURL2
        {
            get
            {
                if (string.IsNullOrEmpty(mPingURL2))
                {
#if UNITY_WEBGL
                return "";
#else
                    return "https://www.bing.com/";
#endif
                }
                return mPingURL2;
            }
        }

        private static string mDeviceIdURL = "";
        public static string deviceIdURL
        {
            get
            {
#if UNITY_EDITOR
                return "";
#elif (UNITY_ANDROID || UNITY_IOS)
            return string.IsNullOrEmpty(mDeviceIdURL) ? UniWebViewHelper.StreamingAssetURLForPath("UUID/index.html") : mDeviceIdURL;            
#else
            return "";
#endif
            }
        }

        // Method
        public static string GetUrlConfig()
        {
            return string.Format(DOMAIN + "/api/server?os_type={0}&version={1}&pkn={2}&mcc={3}&device_name={4}",        
                            Constants.OS_TYPE, Constants.VERSION, Constants.CP_CODE, Constants.CLIENT_MCC, SystemInfo.deviceName); ;
        }
        public static ServerInfo GetServer()
        {
            ServerInfo sv = (lstServer.Count > 0) ? lstServer[0] : null;
            
            // sv local 
            //sv = new ServerInfo("127.0.0.1", 8889, 1, resource: "");  
            // sv dev new
              sv = new ServerInfo("45.77.245.14", 8889, 1, resource: "");
            // sv test new
                //sv = new ServerInfo("139.180.153.32", 8889, 1, resource: "");
            // sv Pro new
            //sv = new ServerInfo("139.180.142.49", 8889, 1, resource: "");

            // sv Pro new 2024
                //sv = new ServerInfo("137.220.60.146", 8889, 1, resource: "");
            if (sv != null && sv.id < 1000) sv1000 = false;
            return sv;
        }
        public static void Parse(string aJSON)
        {
            JSONNode N = JSON.Parse(aJSON);
            mPingURL = (N["ping"] != null) ? N["ping"].Value : "";
            mPingURL1 = (N["ping1"] != null) ? N["ping1"].Value : "";
            mPingURL2 = (N["ping2"] != null) ? N["ping2"].Value : "";
            mDeviceIdURL = (N["di"] != null) ? N["di"].Value : "";

            if (N["server"] != null)
            {
                JSONArray aServer = N["server"].AsArray;
                lstServer.Clear();
                foreach (JSONNode node in aServer)
                {
                    string resource = node["resource"].Value;

                    int id = node["id"].AsInt;
                    string domain = node["domain"].Value;

                    string[] split = domain.Split(new string[] { "@" }, System.StringSplitOptions.RemoveEmptyEntries);
                    int port = -1; int.TryParse(split[1], out port);
                    string host = split[0];
                    lstServer.Add(new ServerInfo(host: host, port: port, id: id, resource: resource));
                }
            }
        }        
    }
}