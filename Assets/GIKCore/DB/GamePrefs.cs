using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GIKCore.Utilities;
using GIKCore.Lang;

namespace GIKCore.DB
{
    public class GamePrefs
    {
        // Values
        private const string KEY_LAST_JSON_CONFIG = "key-last-json-config";
        private const string KEY_LAST_JSON_RESOURCE = "key-last-json-resource";
        private const string KEY_LAST_LANG = "key-last-lang";      
        private const string KEY_LAST_LOGIN_TYPE = "key-last-login-type";
        private const string KEY_REMEMBER = "key-remember";
        private const string KEY_USERNAME = "key-username";
        private const string KEY_PASSWORD = "key-password";
        private const string KEY_DEVICE_ID = "key-device-id";
        private const string KEY_TOS = "tos-user-";
        private const string KEY_SHOP_ITEM = "key-shop-item";
        private const string KEY_NEW_CARD = "key-new-card";

        public static bool isLoggedIn = false;

        // Methods
        public static void Logout()
        {
            isLoggedIn = false;
            Remember = true;//default
            Password = "";
            //if (LastLoginType == (int)Constants.LoginType.Facebook)
            //    Game.main.FB.Logout();
            Username = "";
            LastLoginType = (int)Constants.LoginType.Logout;
        }
        public static string LastJsonConfig
        {
            get { return PlayerPrefs.GetString(FormatKey(KEY_LAST_JSON_CONFIG), ""); }
            set { PlayerPrefs.SetString(FormatKey(KEY_LAST_JSON_CONFIG), value); }
        }
        public static bool Remember
        {
            get
            {
                int on = PlayerPrefs.GetInt(FormatKey(KEY_REMEMBER), 1);
                return (on == 1);
            }
            set
            {
                if (!value) Username = Password = "";
                int on = (value == true) ? 1 : 0;
                PlayerPrefs.SetInt(FormatKey(KEY_REMEMBER), on);
            }
        }
        public static string LastJsonResource
        {
            get { return PlayerPrefs.GetString(FormatKey(KEY_LAST_JSON_RESOURCE), ""); }
            set { PlayerPrefs.SetString(FormatKey(KEY_LAST_JSON_RESOURCE), value); }
        }
        public static int LastLang
        {
            get { return PlayerPrefs.GetInt(FormatKey(KEY_LAST_LANG), LangData.TYPE_EN); }
            set { PlayerPrefs.SetInt(FormatKey(KEY_LAST_LANG), value); }
        }
        public static int LastLoginType
        {
            get { return PlayerPrefs.GetInt(FormatKey(KEY_LAST_LOGIN_TYPE), (int)Constants.LoginType.None); }
            set { PlayerPrefs.SetInt(FormatKey(KEY_LAST_LOGIN_TYPE), value); }
        }
        public static string Username
        {
            get { return PlayerPrefs.GetString(FormatKey(KEY_USERNAME), ""); }
            set
            {
                string tmp = Remember ? value : "";
                PlayerPrefs.SetString(FormatKey(KEY_USERNAME), tmp);
            }
        }
        public static string Password
        {
            get
            {
                string tmp = PlayerPrefs.GetString(FormatKey(KEY_PASSWORD), "");
                string pass = string.IsNullOrEmpty(tmp) ? "" : ICrypto.TripleDESDecrypt(tmp, FormatKey(KEY_PASSWORD), "");
                return pass;
            }
            set
            {
                string tmp = Remember ? value : "";
                string pass = string.IsNullOrEmpty(tmp) ? "" : ICrypto.TripleDESEncrypt(tmp, FormatKey(KEY_PASSWORD));
                PlayerPrefs.SetString(FormatKey(KEY_PASSWORD), pass);
            }
        }
        public static bool TOS
        {
            get
            {
                int on = PlayerPrefs.GetInt(FormatKey(KEY_TOS + Username) , 0);
                return (on == 1);
            }
            set
            {              
                int on = (value == true) ? 1 : 0;
                PlayerPrefs.SetInt(FormatKey(KEY_TOS + Username), on);
            }
        }
        public static string NewCards
        {
            get
            {
                return PlayerPrefs.GetString(FormatKey(KEY_NEW_CARD + Username), "");
            }
            set
            {
                PlayerPrefs.SetString(FormatKey(KEY_NEW_CARD + Username), value);
            }
        }
        public static string DeviceId
        {
            get { return PlayerPrefs.GetString(FormatKey(KEY_DEVICE_ID), ""); }
            set { PlayerPrefs.SetString(FormatKey(KEY_DEVICE_ID), value); }
        }
        public static string SaveShopItem
        {
            get { return PlayerPrefs.GetString(FormatKey(KEY_SHOP_ITEM), ""); }
            set { PlayerPrefs.SetString(FormatKey(KEY_SHOP_ITEM), value); }
        }
        private static string FormatKey(string key)
        {
            return MD5CryptoServiceProvider.GetMd5String(Constants.PKN + "-" + key);
        }

#if UNITY_EDITOR
        [MenuItem("Tools/Delete GamePrefs")]
        public static void DeleteGamePrefs()
        {
            PlayerPrefs.DeleteAll();
            LogWriterHandle.WriteLog("Delete all game prefs successful!");
        }
#endif
    }
}