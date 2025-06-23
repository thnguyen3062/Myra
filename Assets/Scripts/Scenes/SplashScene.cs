using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleJSON;
using TMPro;
using GIKCore;
using GIKCore.Net;
using GIKCore.Bundle;
using GIKCore.Lang;
using GIKCore.DB;
using GIKCore.Sound;
using UnityEngine.SceneManagement;
//using OneSignalSDK;
//using OneSignalSDK;

public class SplashScene : GameListener
{
    // Fields
    [SerializeField] private GameObject m_GoProgress;
    [SerializeField] private Image m_ImgFill;
    [SerializeField] private TextMeshProUGUI m_TxtProgress;
    [SerializeField] private TextMeshProUGUI m_TxtFact;
    [SerializeField] private GameObject m_GuideSlides;
    // Values 
    private BundleVersion lastAv = null;
    private List<BundleVersion> lstAssetCached = new List<BundleVersion>();
    private int totalAssetCached = 0, currentAssetCached = 0;
    private bool tryGetConfig = false, tryGetResource = false, onLoadAssetFromCache = false, newDomain = false;
    private float timeTryGetConfig = 0f, timeTryGetResource = 0f;

    // Methods
    private void DoTryGetConfig(float deltaTime)
    {
        timeTryGetConfig += deltaTime;
        if (timeTryGetConfig >= 1f)
        {
            timeTryGetConfig = 0f;
            if (tryGetConfig)
            {
                tryGetConfig = false;
                Game.main.http.GetConfig();
            }
        }
    }

    private void ErrorWhenGetConfig()
    {
        Config.countConnect = 0;
        tryGetConfig = false;

        string msg = LangHandler.Get("config-1", "Cập nhật cấu hình game thất bại");
        if (Application.internetReachability == NetworkReachability.NotReachable)
            msg += "\n" + LangHandler.Get("host-3", "Không có internet. Vui lòng kiểm tra Wi-Fi hoặc dữ liệu di động của bạn");
        string action1 = "";
#if (UNITY_ANDROID || UNITY_IOS)
        action1 = LangHandler.Get("confirm-8", "Thoát game");
#endif
        PopupConfirm.Show(content: msg, action1: action1, action2: LangHandler.Get("confirm-3", "Thử lại"),
            action1Callback: go => { Application.Quit(); },
            action2Callback: go => { tryGetConfig = true; });
    }

    private void LoadConfigDone()
    {
        
            LoadAssetDone();
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
                if (onLoadAssetFromCache)
                    StartCoroutine(LoadAssetFromCache());
                else Game.main.http.GetResource();
            }
        }
    }

    private void ErrorWhenGetResource()
    {
        BundleHandler.main.countConnect = 0;
        tryGetResource = false;
        StopAllCoroutines();

        string msg = LangHandler.Get("res-1", "Cập nhật gói tài nguyên thất bại");
        if (Application.internetReachability == NetworkReachability.NotReachable)
            msg += "\n" + LangHandler.Get("host-3", "Không có internet. Vui lòng kiểm tra Wi-Fi hoặc dữ liệu di động của bạn"); ;
        string action1 = "";
#if (UNITY_ANDROID || UNITY_IOS)
        action1 = LangHandler.Get("confirm-8", "Thoát game");
#endif
        PopupConfirm.Show(content: msg, action1: action1, action2: LangHandler.Get("confirm-3", "Thử lại"),
            action1Callback: go => { Application.Quit(); },
            action2Callback: go => { tryGetResource = true; });
    }
    private void ParseResource(string aJSON)
    {
        GamePrefs.LastJsonResource = aJSON;//save last json resource
        Debug.Log(aJSON);
        JSONNode node = JSON.Parse(aJSON);
        JSONArray arrayAsset = node["data"].AsArray;
        string url = node["url"].Value;

        lstAssetCached.Clear();
        BundleHandler.main.lstCloud.Clear();
        for (int i = 0; i < arrayAsset.Count; i++)
        {
            string file = arrayAsset[i].Value;
            string[] split = file.Split(new string[] { BundleHandler.SPLIT }, System.StringSplitOptions.RemoveEmptyEntries);

            if (split.Length > 1)
            {
                BundleVersion av = new BundleVersion();
                av.name = split[0];
                av.URI = url + file;
                string hashString = split[1];
                av.hash = Hash128.Parse(hashString);

                lstAssetCached.Add(av);
                //BundleHandler.main.lstCloud.Add(av);
                //if (Caching.IsVersionCached(av.URI, av.hash) || av.name.Equals(BundleHandler.Sounds) || av.name.Equals(BundleHandler.Database)|| av.name.Equals(BundleHandler.Tutorial1)|| av.name.Equals(BundleHandler.UI))
                //    lstAssetCached.Add(av);
                //else Caching.ClearAllCachedVersions(av.name);
            }
        }
        totalAssetCached = lstAssetCached.Count;
        if (totalAssetCached > 0)
        {
            
            m_GoProgress.SetActive(true);
            m_ImgFill.fillAmount = 0f;
            m_GuideSlides.SetActive(true);
            currentAssetCached = 1;
            StartCoroutine(LoadAssetFromCache());
        }
        else LoadAssetDone();
    }
    private IEnumerator LoadAssetFromCache()
    {
        onLoadAssetFromCache = true;
        if (lstAssetCached.Count > 0)
        {
            BundleVersion av = lstAssetCached[0];
            lastAv = av;
            //Using this conditional says we want to wait for our Caching system to be ready before trying to download bundles
            while (!Caching.ready)
                yield return null;
            //Download the bundle
            using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(av.URI, av.hash, 0))
            {
                av.www = www;
                av.reachability = BundleVersion.Reachability.Downloading;

                www.timeout = 600;
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    //Debug.Log("------- " + www.error + "|" + av.name + "|" + av.URI);
                    av.www = null;
                    av.reachability = BundleVersion.Reachability.Cloud;
                    ErrorWhenGetResource();
                }
                else
                {//finished download => add to local
                    av.bundle = DownloadHandlerAssetBundle.GetContent(www);
                    av.www = null;
                    av.reachability = BundleVersion.Reachability.FinishedDownloading;
                    //Debug.Log("8=======D~~~ Download Res=" + av.name + "|url=" + av.URI);
                    BundleHandler.main.AddToLocal(av);
                    lstAssetCached.RemoveAt(0);

                    if (av.name.Equals(BundleHandler.Database))
                    {
                        LangHandler.Parse();//re parse database lang
                        Database.ParseAll();//re parse database game
                    }

                    currentAssetCached += 1;
                    if (currentAssetCached > totalAssetCached)
                        currentAssetCached = totalAssetCached;
                    StartCoroutine(LoadAssetFromCache());

                }
            }
        }
        else LoadAssetDone();
    }
    private void LoadAssetDone()
    {
       // m_TxtProgress.text = "Let's go!";
        m_ImgFill.fillAmount = 1f;
        onLoadAssetFromCache = false;
        m_GuideSlides.SetActive(false);
        //PopupDeviceUUID.Get();
        SoundHandler.main.Init("BackgroundMusicMain");

        Game.main.LoadScene("BattleSceneTutorial", () =>
        {
            BattleSceneTutorial.instance.SetTutorial(0);
        }, delay: 0.3f, curtain: true);
    }
    private void DownloadProgress()
    {
        if (lastAv != null && lastAv.www != null)
        {
            float delta = (totalAssetCached > 0) ? 1f / totalAssetCached : 1f;
            float amount = (currentAssetCached - 1 + lastAv.www.downloadProgress) * delta;
            float percent = 100f * amount;

            m_ImgFill.fillAmount = amount;
            m_TxtProgress.text = /*"Downloading..." + */percent.ToString("n0") + "%";
        }
    }

    public override bool ProcessHttpResponseData(int id, string data)
    {
        if (base.ProcessHttpResponseData(id, data)) return true;
        switch (id)
        {
            case HttpResponseData.GET_CONFIG:
                {
                    if (string.IsNullOrEmpty(data) || HttpResponseData.IsError(data))
                    {
                        if (!string.IsNullOrEmpty(GamePrefs.LastJsonConfig) && Config.countConnect >= 10)
                        {
                            Config.Parse(GamePrefs.LastJsonConfig);
                            LoadConfigDone();
                        }
                        else
                        {
                            if (Config.countConnect < 15)
                                tryGetConfig = true;
                            else if (Config.countConnect == 15)
                                ErrorWhenGetConfig();
                        }
                    }
                    else
                    {
                        Config.Parse(data);
                        LoadConfigDone();
                    }
                    break;
                }
            case HttpResponseData.GET_RESOURCE:
                {
                    if (string.IsNullOrEmpty(data) || HttpResponseData.IsError(data))
                    {
                        if (!string.IsNullOrEmpty(GamePrefs.LastJsonResource) && BundleHandler.main.countConnect >= 10)
                        {
                            ParseResource(GamePrefs.LastJsonResource);
                        }
                        else
                        {
                            if (BundleHandler.main.countConnect < 15)
                                tryGetResource = true;
                            else if (BundleHandler.main.countConnect == 15)
                                ErrorWhenGetResource();
                        }
                    }
                    else
                    {
                        ParseResource(data);
                    }
                    break;
                }
        }

        return false;
    }
    public override bool ProcessNetData(int id, object data)
    {
        if (base.ProcessNetData(id, data)) return true;
        switch (id)
        {
            case NetData.GET_DEVICE_UUID:
                {
                    Game.main.socket.allowConnect = true;
                    Game.main.socket.Connect();
                    //SoundHandler.main.PlaySFX("DealCard", "sounds", true);
                    SoundHandler.main.Init("BackgroundMusicMain");
                    break;
                }
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        LangHandler.Init();
        Database.ParseAll();
        //#if UNITY_IOS        
        //        //do nothing
        //#else        
        //        Game.main.http.GetConfig();
        //#endif
        //OneSignal.Default.Initialize("81248baf-19ec-470c-af73-90da62c6387e");
        LoadConfigDone();
        bool isScreenMode = PlayerPrefs.GetInt("IsWindow", 0) == 1 ? true : false;
        Screen.fullScreenMode = isScreenMode ? FullScreenMode.Windowed : FullScreenMode.FullScreenWindow;
        m_TxtFact.text = LangHandler.Get("733", "\"<#B29436><b>FACT:</b> <#ffffff> You can only use specific color card if you have the corresponding color God card in your deck.\"");
    }

    // Update is called once per frame
    void Update()
    {
        DownloadProgress();
        //DoTryGetConfig(Time.deltaTime);
        //DoTryGetResource(Time.deltaTime);
    }
}
