using AppsFlyerSDK;
using Firebase.Analytics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct ITrackingParameter
{
    public string name;
    public string value;
}
public class ITracking
{
    public const string TRACK_OPEN_APP_FIRST = "first_open";
    public const string TRACK_FIRST_LOGIN_MYRA = "first_login_mytheria_account";
    public const string TRACK_FIRST_LOGIN_GG = "first_login_google";
    public const string TRACK_FIRST_LOGIN_SUCCESS = "first_login";
    public const string TRACK_END_TUT1 = "finish_tutorial_1";
    public const string TRACK_END_TUT2 = "finish_tutorial_2";

    public const string TRACK_END_PROGRESSION = "tutorial_2_progress_complete";
    // newbie quest
    public const string NB_REWARDED_1 = "get_reward_battle_1";
    public const string NB_REWARDED_2 = "get_reward_battle_2";
    public const string NB_REWARDED_3 = "get_reward_battle_3";
    public const string NB_REWARDED_4 = "get_reward_battle_4";
    public const string NB_REWARDED_5 = "get_reward_battle_5";
    public const string NB_REWARDED_6 = "get_reward_battle_6";
    public const string NB_REWARDED_7 = "get_reward_battle_7";
    public const string NB_REWARDED_8 = "get_reward_battle_8";
    public const string NB_REWARDED_9 = "get_reward_battle_9";
    public const string NB_REWARDED_10 = "get_reward_battle_10";

    public static void LogEvent(string eventName, params ITrackingParameter[] parameters)
    {
#if UNITY_ANDROID || UNITY_IOS
        try
        {
            Dictionary<string, string> appsflyerParams = new Dictionary<string, string>();
            List<Parameter> firebaseParams = new List<Parameter>();
            if (parameters != null && parameters.Length > 0)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    string parameterName = parameters[i].name;
                    string parameterValue = parameters[i].value;

                    if (!string.IsNullOrEmpty(parameterValue))
                    {
                        appsflyerParams.Add(string.IsNullOrEmpty(parameterName) ? AFInAppEvents.CONTENT_TYPE : parameterName, parameterValue);
                        firebaseParams.Add(new Parameter(string.IsNullOrEmpty(parameterName) ? FirebaseAnalytics.ParameterContentType : parameterName, parameterValue));
                    }
                }
            }
            FirebaseAnalytics.LogEvent(eventName, firebaseParams.ToArray());
            AppsFlyer.sendEvent(eventName, appsflyerParams);

            appsflyerParams.Clear(); appsflyerParams = null;
            firebaseParams.Clear(); firebaseParams = null;
        }
        catch (System.Exception e) { }
#endif
    }

    public static void LogEventAppsflyer(string eventName, params ITrackingParameter[] parameters)
    {
#if UNITY_ANDROID || UNITY_IOS
        try
        {
            Dictionary<string, string> appsflyerParams = new Dictionary<string, string>();
            if (parameters != null && parameters.Length > 0)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    string parameterName = parameters[i].name;
                    string parameterValue = parameters[i].value;

                    if (!string.IsNullOrEmpty(parameterValue))
                    {
                        appsflyerParams.Add(string.IsNullOrEmpty(parameterName) ? AFInAppEvents.CONTENT_TYPE : parameterName, parameterValue);
                    }
                }
            }

            AppsFlyer.sendEvent(eventName, appsflyerParams);
            appsflyerParams.Clear(); appsflyerParams = null;
        }
        catch (System.Exception e) { }
#endif
    }

    public static void LogEventFirebase(string eventName, params ITrackingParameter[] parameters)
    {
#if UNITY_ANDROID || UNITY_IOS
        try
        {
            List<Parameter> firebaseParams = new List<Parameter>();
            if (parameters != null && parameters.Length > 0)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    string parameterName = parameters[i].name;
                    string parameterValue = parameters[i].value;

                    if (!string.IsNullOrEmpty(parameterValue))
                    {
                        firebaseParams.Add(new Parameter(string.IsNullOrEmpty(parameterName) ? FirebaseAnalytics.ParameterContentType : parameterName, parameterValue));
                    }
                }
            }
            FirebaseAnalytics.LogEvent(eventName, firebaseParams.ToArray());
            firebaseParams.Clear(); firebaseParams = null;
        }
        catch (System.Exception e) { }
#endif
    }
}
