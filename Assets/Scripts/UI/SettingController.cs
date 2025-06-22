using System.Collections;
using System.Collections.Generic;
using GIKCore.Sound;
using GIKCore;
using GIKCore.Net;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GIKCore.UI;
using GIKCore.Lang;
using GIKCore.DB;
using TMPro;
using pbdson;
using UnityEngine.SceneManagement;

public class SettingController : GameListener
{
    [SerializeField] private Slider sliderMusicVolume;
    [SerializeField] private Slider sliderSFXVolume;
    [SerializeField] private GoOnOff muteButton;
    //[SerializeField] private Toggle resizeButton;
    [SerializeField] private GameObject[] options, buttons;
    [SerializeField] private GameObject minimizeWindowObject;
    [SerializeField] private GameObject highlightLang;
    [SerializeField] private TMP_Dropdown dropDown;

    [SerializeField] private List<GameObject> m_ListOptionBg;
    [SerializeField] private List<TextMeshProUGUI> m_ListOptionText;
    //private int currentType = 2;
    int currentLang;

    private void Start()
    {
        if (dropDown != null)
        {
            switch (LangHandler.lastType)
            {
                case LangData.TYPE_EN:
                    {
                        //en
                        dropDown.value = 0;
                        break;
                    }
                case LangData.TYPE_JP:
                    {
                        //jp
                        dropDown.value = 1;
                        break;
                    }
                case LangData.TYPE_KR:
                    {
                        //kr
                        dropDown.value = 2;
                        break;
                    }
                case LangData.TYPE_TC:
                    {
                        //cn
                        dropDown.value = 3;
                        break;
                    }
                case LangData.TYPE_SC:
                    {
                        //tw
                        dropDown.value = 4;
                        break;
                    }
                default:
                    {
                        //default en
                        dropDown.value = 0;
                        break;
                    }
            };
        }
        //foreach (GameObject option in options)
        //{
        //    option.SetActive(false);
        //}
        highlightLang.SetActive(false);

#if UNITY_ANDROID || UNITY_IOS
        if (minimizeWindowObject != null)
            minimizeWindowObject.SetActive(false);
#endif
        //options[currentType].SetActive(true);
        // frame.GetComponent<RectTransform>().anchoredPosition = buttons[currentType].GetComponent<RectTransform>().anchoredPosition;

        sliderMusicVolume.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sliderSFXVolume.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        muteButton.Turn(PlayerPrefs.GetInt("isMute", 0) == 1);
        //resizeButton.isOn = PlayerPrefs.GetInt("IsWindow", 0) == 1;
    }
    public void InitData(int type)
    {
        //frame.transform.DOMove(buttons[type].transform.position, 0.5f);
        for (int i = 0; i < 4; i++)
        {
            options[i].SetActive(false);
            m_ListOptionBg[i].SetActive(false);
            Color colorDefault;
            ColorUtility.TryParseHtmlString("#757574", out colorDefault);
            m_ListOptionText[i].color = colorDefault;
        }
        //foreach (GameObject option in options)
        //{
        //    option.SetActive(false);
        //}
        options[type].SetActive(true);
        m_ListOptionBg[type].SetActive(true);
        Color color;
        ColorUtility.TryParseHtmlString("#ffffff", out color);
        m_ListOptionText[type].color = color;
    }
    public void OnLangguageChange(TMP_Dropdown dropDown)
    {
        int index = dropDown.value;
        if (LangHandler.lastType != (index + 1))
        {
            switch (index)
            {
                case 0:
                    {
                        //en
                        Game.main.socket.SetLanguage(LangData.TYPE_EN);
                        currentLang = LangData.TYPE_EN;
                        GamePrefs.LastLang = LangData.TYPE_EN;
                        break;
                    }
                case 1:
                    {
                        //jp
                        Game.main.socket.SetLanguage(LangData.TYPE_JP);
                        currentLang = LangData.TYPE_JP;
                        GamePrefs.LastLang = LangData.TYPE_JP;

                        break;
                    }
                case 2:
                    {
                        //kr
                        Game.main.socket.SetLanguage(LangData.TYPE_KR);
                        currentLang = LangData.TYPE_KR;
                        GamePrefs.LastLang = LangData.TYPE_KR;
                        break;
                    }
                case 3:
                    {
                        //cn
                        Game.main.socket.SetLanguage(LangData.TYPE_TC);
                        currentLang = LangData.TYPE_TC;
                        GamePrefs.LastLang = LangData.TYPE_TC;
                        break;
                    }
                case 4:
                    {
                        //tw
                        Game.main.socket.SetLanguage(LangData.TYPE_SC);
                        currentLang = LangData.TYPE_SC;
                        GamePrefs.LastLang = LangData.TYPE_SC;
                        break;
                    }
                default:
                    {
                        //en
                        Game.main.socket.SetLanguage(LangData.TYPE_EN);
                        currentLang = LangData.TYPE_EN;
                        GamePrefs.LastLang = LangData.TYPE_EN;
                        break;
                    }
            };  
        }

    }
    private void ChangeLanguage()
    {
        LangHandler.lastType = currentLang;
        GamePrefs.LastLang = LangHandler.lastType;
        CardData.Instance.OnChangeLanguage();
#if UNITY_ANDROID || UNITY_IOS
        if (Game.main.FB.IsLogedin())
        {
            Game.main.FB.Logout();
        }
#endif
        Game.main.socket.Logout();
        if (ProgressionController.instance != null)
        {
            Destroy(ProgressionController.instance.gameObject);
            ProgressionController.instance = null;
        }
        Game.main.LoadScene("HomeSceneNew", delay: 1f, curtain: true);

    }
    public void DoClickSurrender()
    {
        PopupConfirm.Show(content: LangHandler.Get("cf-4", "SELECT OK TO SURRENDER."),
            title: LangHandler.Get("but-23", "CONFIRM"),
            action1: LangHandler.Get("but-20", "CANCEL"), action2: "OK",
            action2Callback: go =>
            {
                this.gameObject.SetActive(false);
                UIManager.instance.OnSurrender();
            });
    }
    public void DoClickLogout()
    {
        PopupConfirm.Show(content: LangHandler.Get("cf-3", "SELECT OK TO LOG OUT."),
            title: LangHandler.Get("but-24", "LOG OUT"),
            action1: LangHandler.Get("but-20", "CANCEL"), action2: "OK",
            action2Callback: go =>
            {
#if UNITY_ANDROID || UNITY_IOS
                if (Game.main.FB.IsLogedin())
                {
                    Game.main.FB.Logout();
                }
#endif
                Game.main.socket.Logout();
                if (ProgressionController.instance != null)
                {
                    Destroy(ProgressionController.instance.gameObject);
                    ProgressionController.instance = null;
                }
                Game.main.LoadScene("HomeSceneNew", delay: 1f, curtain: true);
            });
    }
    public void ChangeMusicVolume()
    {
        float volume = sliderMusicVolume.value;
        SoundHandler.main.ChangeMusicVolume(volume);
        if (muteButton.online)
        {
            PlayerPrefs.SetInt("isMute", 0);
            muteButton.TurnOff();
        }

    }
    public void ChangeSFXVolume()
    {
        float volume = sliderSFXVolume.value;
        SoundHandler.main.ChangeSFXVolume(volume);
        if (muteButton.online)
        {
            PlayerPrefs.SetInt("isMute", 0);
            muteButton.TurnOff();
        }
    }
    public void DoClickClose()
    {
        this.gameObject.SetActive(false);
    }

    public void DoClickLoadPolicy()
    {
        Application.OpenURL("https://mytheria.io/policies");
    }

    public void DoClickLoadinforService()
    {
        Application.OpenURL("https://mytheria.io/terms");
    }
    public void DoClickLoadDeleteAccount()
    {
        Application.OpenURL("https://forms.gle/KvdvWL1hy7GPDqdG7");
    }

    public void DoClickButtonDisactive()
    {
        Toast.Show(LangHandler.Get("toast-8", "Coming Soon!"));
    }
    public void DoClickMute()
    {
        if (muteButton.online)
            SoundHandler.main.Mute();
        else
        {
            SoundHandler.main.ChangeMusicVolume(sliderMusicVolume.value);
            SoundHandler.main.ChangeSFXVolume(sliderSFXVolume.value);
        }
        PlayerPrefs.SetInt("isMute", muteButton.online ? 1 : 0);
    }

    public void DoClickResizeWindow()
    {
        //if (resizeButton.isOn)
        //{
        //    Screen.fullScreenMode = FullScreenMode.Windowed;
        //    Screen.SetResolution(1280, 720, false, 60);
        //}
        //else
        //{
        //    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        //}
        //PlayerPrefs.SetInt("IsWindow", resizeButton.isOn ? 1 : 0);
    }
    public override bool ProcessSocketData(int id, byte[] data)
    {
        if (base.ProcessSocketData(id, data))
            return true;

        switch (id)
        {
            case IService.SET_LANGUAGE:
                CommonVector cv = ISocket.Parse<CommonVector>(data);
                LogWriterHandle.WriteLog(string.Join(",", cv.aLong));
                if (cv.aLong[0] == 1)
                {
                    ChangeLanguage();
                }
                else
                {

                }
                break;

        }

        return false;
    }
}
