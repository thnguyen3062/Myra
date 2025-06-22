using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
using GIKCore.Net;
using GIKCore.Lang;

namespace GIKCore.Bundle
{
    public class BundleDownloader : GameListener
    {
        // Fields
        [SerializeField] private GameObject m_GoContent;
        [SerializeField] private Image m_ImgFill;
        [SerializeField] private Text m_TxtFill;
        [SerializeField] private string m_GameCode;
        [SerializeField] private bool m_Root = true;

        // Values 
        private BundleVersion lastAv = null;
        private List<string> lstBundle;
        private bool downloading = false, tryGetResource = false;
        private int currentAsset, totalAsset;
        private float timeTryGetResource = 0f;

        // Methods
        public string gamecode
        {
            get { return m_GameCode; }
            set
            {
                m_GameCode = value;
                lstBundle = BundleRequest.Get(gamecode);
                m_GoContent.SetActive(false);
            }
        }
        public int CheckCloud()
        {
            if (lstBundle == null || lstBundle.Count <= 0) lstBundle = BundleRequest.Get(gamecode);
            if (lstBundle == null || lstBundle.Count <= 0) return 0;

            int count = 0;
            foreach (BundleVersion av in BundleHandler.main.lstCloud)
            {
                if (lstBundle.Contains(av.name) && av.reachability == BundleVersion.Reachability.Cloud)
                    count++;//available to download
            }
            return count;
        }
        public void StartDownload()
        {
            if (downloading) return;
            if (CheckCloud() > 0)
                HandleNetData.QueueNetData(NetData.DOWNLOAD_RES_STARTED, gamecode);
        }
        private IEnumerator DownloadAndCacheAsset()
        {
            foreach (BundleVersion av in BundleHandler.main.lstCloud)
            {//process one asset per time            
                if (lstBundle.Contains(av.name))
                {
                    lastAv = av;
                    if (av.reachability == BundleVersion.Reachability.Cloud)
                    {//begin download from cloud                    
                        downloading = true;
                        //Using this conditional says we want to wait for our Caching system to be ready before trying to download bundles
                        while (!Caching.ready)
                            yield return null;
                        //Download the bundle
                        using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(av.URI, av.hash, 0))
                        {
                            av.www = www;
                            av.reachability = BundleVersion.Reachability.Downloading;

                            www.timeout = 20;
                            yield return www.SendWebRequest();

                            if (www.responseCode > 0)
                                Game.main.netBehavior.InternetConnected();

                            if (www.result != UnityWebRequest.Result.Success)
                            {
                                LogWriterHandle.WriteLog("------- " + www.error);
                                av.www = null;
                                av.reachability = BundleVersion.Reachability.Cloud;
                                ErrorWhenGetResource();
                            }
                            else
                            {//finished download => add to local;
                                av.bundle = DownloadHandlerAssetBundle.GetContent(www);
                                av.www = null;
                                av.reachability = BundleVersion.Reachability.FinishedDownloading;

                                BundleHandler.main.AddToLocal(av);
                                lstBundle.Remove(av.name);
                                Recursive();
                            }
                        }
                        break;
                    }
                    else if (av.reachability == BundleVersion.Reachability.Downloading)
                    {
                        yield return new WaitForEndOfFrame();
                        StartCoroutine(DownloadAndCacheAsset());
                        break;
                    }
                    else if (av.reachability == BundleVersion.Reachability.FinishedDownloading)
                    {
                        lstBundle.Remove(av.name);
                        Recursive();
                        break;
                    }
                }
            }
        }
        private IEnumerator DownloadAssetDone()
        {
            yield return new WaitForSeconds(0.2f);
            downloading = false;
            HandleNetData.QueueNetData(NetData.DOWNLOAD_RES_FINISHED, gamecode);
        }
        private void ErrorWhenGetResource()
        {
            downloading = tryGetResource = false;
            StopAllCoroutines();

            string msg = LangHandler.Get("res-1", "Cập nhật gói tài nguyên thất bại");
            if (!Game.main.netBehavior.IsConnected())
                msg += "\n" + LangHandler.Get("host-3", "Không có internet. Vui lòng kiểm tra Wi-Fi hoặc dữ liệu di động của bạn");
            PopupConfirm.Show(content: msg, action1: LangHandler.Get("confirm-3", "Thử lại"), action2: LangHandler.Get("confirm-15", "Đóng"),
                action1Callback: go => { tryGetResource = true; },
                action2Callback: go => { HandleNetData.QueueNetData(NetData.DOWNLOAD_RES_FAILED, gamecode); });
        }
        private void DoTryGetResource(float deltaTime)
        {
            timeTryGetResource += deltaTime;
            if (timeTryGetResource >= 1f)
            {
                timeTryGetResource = 0f;
                if (tryGetResource)
                {
                    tryGetResource = false;
                    StartCoroutine(DownloadAndCacheAsset());
                }
            }
        }
        private void Recursive()
        {
            HandleNetData.QueueNetData(NetData.DOWNLOAD_RES_PROGRESS, new object[] { gamecode, totalAsset, currentAsset, 1f });

            currentAsset += 1;
            if (currentAsset > totalAsset)
            {
                currentAsset = totalAsset;
                HandleNetData.QueueNetData(NetData.DOWNLOAD_RES_PROGRESS, new object[] { gamecode, totalAsset, currentAsset, 1f });
                StartCoroutine(DownloadAssetDone());
            }
            else StartCoroutine(DownloadAndCacheAsset());
        }

        public override bool ProcessNetData(int id, object o)
        {
            if (base.ProcessNetData(id, o)) return true;
            switch (id)
            {
                case NetData.DOWNLOAD_RES_STARTED:
                    {
                        if (!string.IsNullOrEmpty(gamecode))
                        {
                            string label = (string)o;
                            if (label.Contains(gamecode) || gamecode.Contains(label))
                            {
                                m_GoContent.SetActive(true);
                                if (m_Root)
                                {
                                    if (lstBundle == null || lstBundle.Count <= 0) lstBundle = BundleRequest.Get(gamecode);
                                    totalAsset = lstBundle.Count;
                                    currentAsset = 1;
                                    StartCoroutine(DownloadAndCacheAsset());
                                }
                            }
                        }
                        break;
                    }
                case NetData.DOWNLOAD_RES_PROGRESS:
                    {
                        if (!string.IsNullOrEmpty(gamecode))
                        {
                            object[] x = (object[])o;
                            string name = (string)x[0];
                            int total = (int)x[1];
                            int current = (int)x[2];
                            float progress = (float)x[3];

                            float delta = (total > 0) ? 1f / total : 1f;
                            float amount = (current - 1 + progress) * delta;

                            if (name.Contains(gamecode) || gamecode.Contains(name))
                            {
                                float percent = 100f * amount;
                                //m_ImgFill.fillAmount = amount;
                                m_TxtFill.text = percent.ToString("n0") + "%";
                            }
                        }
                        break;
                    }
                case NetData.DOWNLOAD_RES_FINISHED:
                case NetData.DOWNLOAD_RES_FAILED:
                    {
                        if (!string.IsNullOrEmpty(gamecode))
                        {
                            string name = (string)o;
                            if (name.Contains(gamecode) || gamecode.Contains(name))
                                m_GoContent.SetActive(false);
                        }
                        break;
                    }
            }
            return false;
        }

        // Start is called before the first frame update   
        //void Start() { }

        // Update is called once per frame
        void Update()
        {
            if (lastAv != null && lastAv.www != null)
            {
                HandleNetData.QueueNetData(NetData.DOWNLOAD_RES_PROGRESS, new object[] { gamecode, totalAsset, currentAsset, lastAv.www.downloadProgress });
            }

            DoTryGetResource(Time.deltaTime);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            StopAllCoroutines();
            if (downloading)
            {
                downloading = false;
                lstBundle = BundleRequest.Get(gamecode);
                foreach (BundleVersion av in BundleHandler.main.lstCloud)
                {
                    if (lstBundle.Contains(av.name) && av.reachability == BundleVersion.Reachability.Downloading)
                    {//stop all downloading in case of switch scene                    
                        av.www = null;
                        av.reachability = BundleVersion.Reachability.Cloud;
                    }
                }
            }
        }
    }
}