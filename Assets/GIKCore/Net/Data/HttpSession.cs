using System.Collections.Generic;
using GIKCore.DB;
using GIKCore.Bundle;
using UnityEngine;

namespace GIKCore.Net
{
    public class HttpSession
    {
        // Values
        private IHttpRequest http;

        // Methods
        public HttpSession()
        {
            http = new IHttpRequest();
        }

        // ----------------------------------------------------------------------
        // ----------------------------------------------------------------------
        // --------------------- DEFINE SEND FUNCTION BELOW ---------------------
        // ----------------------------------------------------------------------
        // ----------------------------------------------------------------------

        public void GetConfig()
        {
            Config.countConnect++;
            http.GET(HttpResponseData.GET_CONFIG, Config.GetUrlConfig());
        }
        public bool GetResource()
        {
            BundleHandler.main.countConnect++;
            string dest = "";
#if UNITY_IOS
                                dest = "res_ios.php";
#elif UNITY_ANDROID
            dest = "res_android.php";
#elif UNITY_WEBGL
                                dest = "res_webgl.php";
#elif UNITY_STANDALONE
                                dest = "res_windows.php";
#endif
            //ServerInfo sv = Config.GetServer();
            //if (!string.IsNullOrEmpty(dest) && sv != null && !string.IsNullOrEmpty(sv.resource))
            //{
            //#if UNITY_EDITOR
            //old            string url = "https://res-myth-v2.helitech-solutions.com/res_windows.php";
            //#else
              //string url = "https://res-myth-v2.helitech-solutions.com/" + dest;

            string url = "https://res.mytheria.dev/res_android.php/" + dest;

            //#endif
            http.GET(HttpResponseData.GET_RESOURCE, url); 
            return true;
        }
        //    return false;
        //}

        public void GetFacebookID(string access_token)
        {
            string url = "https://graph.facebook.com/me?access_token=" + access_token;
            http.GET(HttpResponseData.GET_FACEBOOK_ID, url);
        }
    }
}