using GIKCore;
using GIKCore.Net;
using GIKCore.DB;
using GIKCore.Utilities;
using GIKCore.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupWebview : GameListener
{
    // Fields
    [SerializeField] private GameObject m_BoxPart;
    [SerializeField] private RectTransform m_RectPart, m_RectFull;
    [SerializeField] private TweenAlphaCanvasGroup m_TweenFade;

    // Values    
    private UniWebView webView = null;

    // Methods
    public void DoRecycle()
    {
        CloseWebView();
        gameObject.SetActive(false);
    }

    public void SetData(string url, bool fullscreen)
    {
        if (webView == null) CreateWebView();
        if (webView != null)
        {
            m_BoxPart.SetActive(!fullscreen);
            webView.ReferenceRectTransform = fullscreen ? m_RectFull : m_RectPart;
            webView.Load(url);
            webView.Show();
            //if (Config.allowLog) Debug.Log("------- webview -> Url: " + url);
        }
    }

    private void CreateWebView()
    {
        webView = gameObject.AddComponent<UniWebView>();
        webView.AddUrlScheme("myra");
        webView.OnMessageReceived += OnMessageReceived;
        webView.OnShouldClose += OnShouldClose;
    }
    private void CloseWebView()
    {
        webView.CleanCache();
        Destroy(webView);
        webView = null;
    }
    private void OnMessageReceived(UniWebView view, UniWebViewMessage message)
    {
        //Toast.Show(message.Path+"  "+message.Scheme);
        //Toast.Show("Login Successfully");
        Debug.Log("at: " + message.Args["at"]);
        Debug.Log("rt: " + message.Args["rt"]);
        if (message.Path.Equals("browser"))
        {//ascheme://browser?uri='base64'
            //string link = ICrypto.Base64Decode(message.Args["uri"]);
            //Util.OpenURL(link);
        }
        else if (message.Path.Equals("iap"))
        {//ascheme://iap?sku=iap.iap1
            //string sku = message.Args["sku"];
            //Game.main.IAP.BuyProduct(sku);
        }
        else if (message.Path.Equals("close"))
        {//ascheme://close
            DoRecycle();
        }
        else if (message.Path.Equals("result"))
        {
            GameData.main.accessToken = message.Args["at"];
            GameData.main.refreshToken = message.Args["rt"];
            Game.main.socket.LoginBlockchain(message.Args["at"], message.Args["rt"]);
            DoRecycle();
        }
    }
    private bool OnShouldClose(UniWebView view)
    {
        DoRecycle();
        //if (Config.allowLog) Debug.Log("------- OnShouldClose -------");
        return true;
    }

    public override bool ProcessNetData(int id, object data)
    {
        if (base.ProcessNetData(id, data)) return true;
        switch (id)
        {
            case NetData.CLOSE_POPUP:
                {
                    string name = (string)data;
                    if (name.Equals("PopupWebview")) m_TweenFade.Play();
                    break;
                }
        }
        return false;
    }

    //void Start() { }

    // Update is called once per frame
    //void Update() { }

    void OnEnable()
    {
        transform.SetAsLastSibling();
    }

    public static void Show(string url, bool fullscreen = false)
    {
        Transform target = IUtil.LoadPrefabRecycle("Prefabs/Common/", "PopupWebview", Game.main.canvas.transform);
        //to make webview work correctly, have to set active first
        target.GetComponent<PopupWebview>().SetData(url, fullscreen);
    }
}
