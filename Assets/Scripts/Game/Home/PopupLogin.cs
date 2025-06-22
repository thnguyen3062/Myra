using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using pbdson;
using GIKCore;
using GIKCore.DB;
using GIKCore.Net;
using GIKCore.Lang;
using GIKCore.UI;
using GIKCore.Utilities;

public class PopupLogin : GameListener
{
    public enum Type { Login, Register }

    // Fields    
    [SerializeField] private PanelLogin m_PanelLogin;
    [SerializeField] private PanelRegister m_PanelRegister;
    [SerializeField] private ITween m_TweenBlur;

    // Methods
    public void InitData(Type type)
    {
        m_PanelLogin.SetActive(false);
        m_PanelLogin.DoReset();
        m_PanelRegister.SetActive(false);
        m_PanelRegister.DoReset();

        if (type == Type.Login) m_PanelLogin.SetActive(true);
        else if (type == Type.Register) m_PanelRegister.SetActive(true);
    }

    public override bool ProcessSocketData(int serviceId, byte[] data)
    {
        if (base.ProcessSocketData(serviceId, data)) return true;
        switch (serviceId)
        {
            case IService.LOGIN:
                {
                    m_TweenBlur.Play();
                    break;
                }
            case IService.REGISTER:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    long status = cv.aLong[0];
                    string msg = cv.aString[0];

                    if (status == 0)//false
                    {
                        if (!string.IsNullOrEmpty(msg)) Toast.Show(msg);
                       // m_PanelRegister.DoFalse();
                    }
                    else
                    {
                        m_TweenBlur.Play();
                        if (!string.IsNullOrEmpty(msg))
                            PopupConfirm.Show(msg,
                                action1: LangHandler.Get("login-1", defaultValue: "Đăng nhập"),
                                action1Callback: go => { LoginNormal(m_PanelRegister.username, m_PanelRegister.password); });
                        else LoginNormal(m_PanelRegister.username, m_PanelRegister.password);
                    }
                    break;
                }           
        }
        return false;
    }

    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();
        m_PanelLogin.SetOnOpenPanelRegister(() => { m_PanelRegister.SetActive(true); });
        m_PanelRegister.SetOnOpenPanelLogin(() => { m_PanelLogin.SetActive(true); });
    }

    public static void LoginNormal(string username, string password)
    {        
        if (string.IsNullOrEmpty(username)) Toast.Show(LangHandler.Get("login-9", defaultValue: "Cung cấp tài khoản"));
        else if (string.IsNullOrEmpty(password)) Toast.Show(LangHandler.Get("login-10", defaultValue: "Cung cấp mật khẩu"));
        else
        {
            if (!IUtil.IsSplashScene())
                Game.main.netBlock.ShowNow();

            GamePrefs.LastLoginType = (int)Constants.LoginType.Normal;
            Game.main.socket.LoginNormal(username, password);            
        }
    }
    public static void LoginFacebook(ICallback.CallFunc onLoginSuccessCallback = null, ICallback.CallFunc onLoginFailCallback = null)
    {
        //Game.main.FB.Login(
        //    () =>
        //    {
        //        GamePrefs.LastLoginType = (int)Constants.LoginType.Facebook;
        //        Game.main.socket.LoginFacebook(Game.main.FB.UserId, Game.main.FB.TokenString);
        //        if (onLoginSuccessCallback != null) onLoginSuccessCallback();
        //    },
        //    () => { if (onLoginFailCallback != null) onLoginFailCallback(); });        
    }

    private static void LoginDone()
    {
        Game.main.netBlock.Show();
        if (!IUtil.IsActiveSceneSameAs("HomeSceneNew"))
            Game.main.LoadScene("HomeSceneNew", delay: 0.3f);
    }
    private static void AutoLogin()
    {
        int lastLoginType = GamePrefs.LastLoginType;        
        if (lastLoginType == (int)Constants.LoginType.Normal)
        {
            string username = GamePrefs.Username;
            string password = GamePrefs.Password;

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                LoginNormal(username, password);
            }
        }
        else if (lastLoginType == (int)Constants.LoginType.Facebook)
        {
#if (UNITY_ANDROID || UNITY_IOS)
            LoginFacebook();
#endif
        }

        LoginDone();
    }
    public static void OAuthToken()
    {
        GameData.DoReset();
#if UNITY_WEBGL
        //if access_token # null or empty => get facebook id; if success => login; if not => do nothing => login via button
        //if access_token = null or empty => do nothing => login via button
        string access_token = ICrypto.Base64Decode(WebGLNative.GetParamFromHrefJS("access_token"));
        if (!string.IsNullOrEmpty(access_token))
        {
            Game.main.FB.TokenString = access_token;
            Game.main.http.GetFacebookID(access_token);
            LoginDone();
        }
        else AutoLogin();
#else
        AutoLogin();
#endif        
    }

    public static void Show(Type type = Type.Login)
    {
        Transform target = IUtil.LoadPrefabRecycle("Prefabs/Home/", "PopupLogin", Game.main.canvas.panelPopup);
        target.GetComponent<PopupLogin>().InitData(type);
    }
    public static void Hide()
    {
        Transform target = Game.main.canvas.panelPopup.Find("PopupLogin");
        if (target != null) target.gameObject.SetActive(false);
    }

    public static bool NeedLoginFirst()
    {
        if (!GamePrefs.isLoggedIn)
        {
            Show(Type.Login);
            return true;
        }
        return false;
    }

}
