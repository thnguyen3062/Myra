#if !UNITY_STANDALONE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using SimpleJSON;
using System;
using GIKCore.Utilities;
using GIKCore;
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

public class IFacebook : MonoBehaviour
{
    public static bool allowLog = true;

    // Values
    public string UserId { get; set; } = "";
    public string TokenString { get; set; } = "";

    private ICallback.CallFunc onLoginSuccessCB, onLoginFailCB;

    // Methods
    // ------- Facebook -------    
    public void Login(ICallback.CallFunc onLoginSuccessCallback, ICallback.CallFunc onLoginFailCallback = null)
    {
        onLoginSuccessCB = onLoginSuccessCallback;
        onLoginFailCB = onLoginFailCallback;
#if (UNITY_ANDROID || UNITY_IOS)
        if (!FB.IsInitialized)
        {
            Debug.Log("------- FB: Login - Initialize the Facebook SDK");
            FB.Init(() => { InitCallback(true); }, OnHideUnity);
        }
        else
        {
            Debug.Log("------- FB: Already initialized, signal an app activation App Event");
            FB.ActivateApp();
            // Continue with Facebook SDK
            ContinueWithFacebookSDK();
        }
#elif UNITY_WEBGL
        WebGLNative.FBLoginJS();
#endif
    }

    public void Logout()
    {
#if (UNITY_ANDROID || UNITY_IOS)
        FB.LogOut();
#elif UNITY_WEBGL
        WebGLNative.FBLogoutJS();
#endif
    }

    public bool IsLogedin()
    {
        //Debug.Log("===================================== " + FB.IsInitialized + "=====================" + FB.IsLoggedIn);
        return FB.IsLoggedIn;
        //return FB.IsInitialized;
    }

    public void AppEvents(string logEvent, float? valueToSum = null, Dictionary<string, object> parameters = null)
    {
        FB.LogAppEvent(logEvent, valueToSum, parameters);
    }

    public void FacebookIDResponse(string data)
    {
#if UNITY_WEBGL
        if (HttpResponseData.IsError(data)) Logout();
        else
        {
            JSONNode N = JSON.Parse(data);
            if (N["id"] != null)
            {
                UserId = N["id"].Value;
                if (!string.IsNullOrEmpty(TokenString))
                {
                    GamePrefs.LastLoginType = (int)Constants.LoginType.Facebook;
                    Game.main.session.LoginFacebook(UserId, TokenString);
                }
            }
            else Logout();
        }
#endif
    }

#if (UNITY_ANDROID || UNITY_IOS)
    private void LoginWithPermissions()
    {
        //List<string> readPerms = new List<string> { "public_profile", "email" };
        //List<string> publishPerms = new List<string> { "publish_actions" };
        FB.LogInWithReadPermissions(callback: AuthCallback);
    }
    private void ContinueWithFacebookSDK()
    {
        if (allowLog) Debug.Log("------- FB: IsLoggedIn = " + FB.IsLoggedIn);
        if (FB.IsLoggedIn)
        {
            if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(TokenString))
            {
                Logout();
                LoginWithPermissions();
            }
            else
            {
                if (onLoginSuccessCB != null) onLoginSuccessCB();
            }
        }
        else
        {
            LoginWithPermissions();
        }
    }
    private void AuthCallback(ILoginResult result)
    {
        if (!string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("------- FB: Error = " + result.Error);
            if (onLoginFailCB != null) onLoginFailCB();
            return;
        }

        if (allowLog) Debug.Log("------- FB: AuthCallback = " + result.RawResult);

        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            AccessToken aToken = AccessToken.CurrentAccessToken;
            UserId = aToken.UserId;
            TokenString = aToken.TokenString;

            if (onLoginSuccessCB != null) onLoginSuccessCB();
            Game.main.socket.LoginFacebook(TokenString, 0);
            if (allowLog)
            {
                Debug.Log("------- FB: UserId = " + UserId);
                Debug.Log("------- FB: TokenString = " + TokenString);
                foreach (string perm in aToken.Permissions)
                {
                    Debug.Log(perm);
                }
            }
        }
        else
        {
            if (onLoginFailCB != null) onLoginFailCB();
            if (allowLog) Debug.Log("------- FB: User cancelled login");
        }
    }
    private void InitCallback(bool continueWithFacebookSDK)
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            Debug.Log("------- FB: Already initialized, signal an app activation App Event");
            FB.ActivateApp();
            // Continue with Facebook SDK
            if (continueWithFacebookSDK) ContinueWithFacebookSDK();
        }
        else
        {
            Debug.Log("------- FB: Failed to Initialize the Facebook SDK");
        }
    }
    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }
#endif

    // Use this for initialization
    void Awake()
    {
        Game.main.FB = this;
        //#if UNITY_EDITOR
        //        Debug.Log("Login FB");
#if (UNITY_ANDROID || UNITY_IOS)
        if (!FB.IsInitialized)
        {
            Debug.Log("------- FB: Awake - Initialize the Facebook SDK");
            FB.Init(() => { InitCallback(false); }, OnHideUnity);
        }
        else
        {
            Debug.Log("------- FB: Already initialized, signal an app activation App Event");
            FB.ActivateApp();
        }
#endif
    }
}
#endif