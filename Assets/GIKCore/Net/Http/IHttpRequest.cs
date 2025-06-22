using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GIKCore.Utilities;

namespace GIKCore.Net
{
    public class IHttpRequest
    {
        public static bool allowLog = true;

        private int id = -1;
        private bool isLoading = false;

        public void GET(int id, string url, ICallback.CallFunc2<UnityWebRequest> onBeforeSend = null)
        {
            if (!isLoading)
            {
                isLoading = true;
                this.id = id;
                Game.main.netBlock.Show();
                Game.main.StartCoroutine(WaitForHttpGet(url, onBeforeSend));
            }
        }
        public void POST(int id, string url, string data, ICallback.CallFunc2<UnityWebRequest> onBeforeSend = null)
        {
            if (!isLoading)
            {
                isLoading = true;
                this.id = id;
                Game.main.netBlock.Show();
                Game.main.StartCoroutine(WaitForHttpPost(url, data, onBeforeSend));
            }
        }
        public void POST(int id, string url, WWWForm formData, ICallback.CallFunc2<UnityWebRequest> onBeforeSend = null)
        {
            if (!isLoading)
            {
                isLoading = true;
                this.id = id;
                Game.main.netBlock.Show();
                Game.main.StartCoroutine(WaitForHttpPost(url, formData, onBeforeSend));
            }
        }

        IEnumerator WaitForHttpGet(string url, ICallback.CallFunc2<UnityWebRequest> onBeforeSend)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                if (onBeforeSend != null) onBeforeSend(www);
                www.timeout = 20;
                yield return www.SendWebRequest();
                ProcessResponse(www);
            }
        }
        IEnumerator WaitForHttpPost(string url, WWWForm formData, ICallback.CallFunc2<UnityWebRequest> onBeforeSend)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(url, formData))
            {
                if (onBeforeSend != null) onBeforeSend(www);
                www.timeout = 20;
                yield return www.SendWebRequest();
                ProcessResponse(www);
            }
        }
        IEnumerator WaitForHttpPost(string url, string data, ICallback.CallFunc2<UnityWebRequest> onBeforeSend)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(url, data))
            {
                if (onBeforeSend != null) onBeforeSend(www);
                www.timeout = 20;
                yield return www.SendWebRequest();
                ProcessResponse(www);
            }
        }

        private void ProcessResponse(UnityWebRequest www)
        {
            if (www.responseCode > 0)
                Game.main.netBehavior.InternetConnected();

            isLoading = false;
            string data = "";

            if (allowLog) LogWriterHandle.WriteLog(www.url);
            if (www.result != UnityWebRequest.Result.Success)
            {
                LogWriterHandle.WriteLog("------- " + www.error + "|" + www.url);

                data = HttpResponseData.ERROR + www.error;
                if (www.error.Equals(HttpResponseData.ERROR_CONNECT_HOST))
                    data = HttpResponseData.ERROR_CONNECT_HOST;
                else if (www.error.Equals(HttpResponseData.ERROR_RESOLVE_HOST))
                    data = HttpResponseData.ERROR_RESOLVE_HOST;
            }
            else
            {//success
                data = www.downloadHandler.text;
                if (allowLog) LogWriterHandle.WriteLog("------- " + data);
            }

            HandleNetData.QueueHttpResponseData(id, data);
            id = -1;
        }
    }
}