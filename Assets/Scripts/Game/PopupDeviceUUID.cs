using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GIKCore;
using GIKCore.Net;
using GIKCore.DB;
using GIKCore.Utilities;

public class PopupDeviceUUID : GameListener
{
    // Field    
    [SerializeField] private UniWebView m_WebView;

    // Methods
    public void DoDestroy() { Destroy(gameObject); }

    public void SetData()
    {
        Color color = Color.white;
        ColorUtility.TryParseHtmlString("#FFFFFF00", out color);

        m_WebView.AddUrlScheme("ascheme");
        m_WebView.BackgroundColor = color;
        m_WebView.SetBackButtonEnabled(false);
        m_WebView.OnMessageReceived += OnMessageReceived;

        //m_WebView.Load(Config.deviceIdURL);
        //m_WebView.Show();
        
        m_WebView.Load(Config.deviceIdURL);
        m_WebView.Show();
    }
    private void OnMessageReceived(UniWebView view, UniWebViewMessage message)
    {
        if (Config.allowLog) Debug.Log("OnMessageReveived -> " + message.RawMessage);
        if (message.Path.Equals("uuid"))
        {//ascheme://uuid?value=device_uuid            
            Set(message.Args["value"]);
            DoDestroy();
        }
    }

    // Start is called before the first frame update
    //void Start() { }

    // Update is called once per frame
    //void Update() { }

    void OnEnable()
    {
        transform.SetAsLastSibling();
    }

    public static void Set(string x = "", bool sendEvent = true)
    {        
        string s = string.Join("-", x, SystemInfo.deviceModel, SystemInfo.deviceName, SystemInfo.deviceType);
        Constants.DEVICE_ID = GamePrefs.DeviceId = MD5CryptoServiceProvider.GetMd5String(s);
        if (sendEvent) HandleNetData.QueueNetData(NetData.GET_DEVICE_UUID);
    }
    public static void Get()
    {
        Constants.DEVICE_ID = GamePrefs.DeviceId;
        if (!string.IsNullOrEmpty(Constants.DEVICE_ID))
        {
            HandleNetData.QueueNetData(NetData.GET_DEVICE_UUID);
            return;
        }

#if UNITY_WEBGL
        WebGLNative.GetDeviceIdJS();
#else        
        if (string.IsNullOrEmpty(Config.deviceIdURL))
            Set(SystemInfo.deviceUniqueIdentifier);
        else
        {
            Transform target = IUtil.LoadPrefabRecycle("Prefabs/Common/", "PopupDeviceUUID", Game.main.canvas.transform);
            //to make webview work correctly, have to set active first
            target.GetComponent<PopupDeviceUUID>().SetData();
        }
#endif
    }
}
