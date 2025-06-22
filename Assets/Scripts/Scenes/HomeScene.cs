using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GIKCore;
using GIKCore.Net;
using GIKCore.Lang;
using GIKCore.DB;
using GIKCore.Utilities;

public class HomeScene : GameListener
{
    // Fields
    [SerializeField] private GameObject m_PanelProfile, m_PanelLogin;
    [SerializeField] private TextMeshProUGUI m_TxtName;

    // Methods
    public void DoClickLogout()
    {
        PopupConfirm.Show(content: LangHandler.Get("logout-1", "Bạn có muốn đăng xuất?"),
            action1: LangHandler.Get("confirm-6", "Có"), action2: LangHandler.Get("confirm-7", "Không"),
            action1Callback: go =>
            {
                Game.main.socket.Logout();

                if (ProgressionController.instance != null)
                {
                    Destroy(ProgressionController.instance.gameObject);
                    ProgressionController.instance = null;
                }
#if UNITY_WEBGL
                Game.main.LoadScene("HomeScene");
#else
                Game.main.LoadScene("HomeScene", delay: 0.3f, curtain: true);
#endif                
            });
    }
    
    public void GoBattle() {
        if (PopupLogin.NeedLoginFirst()) return;
        Game.main.LoadScene("BattleLoadingScene");
    }

    private void InitData()
    {
        m_PanelProfile.SetActive(false);
        m_PanelLogin.SetActive(false);

        if (!GamePrefs.isLoggedIn)//not login yet
        {
            m_PanelLogin.SetActive(true);
        }
        else
        {
            m_PanelProfile.SetActive(true);
            SetProfileData();
            Game.main.socket.GetUserHeroCard();            
        }
    }

    private void SetProfileData()
    {
        m_TxtName.text = GameData.main.profile.displayName;
    }

    public override bool ProcessSocketData(int serviceId, byte[] data)
    {
        if (base.ProcessSocketData(serviceId, data)) return true;
        switch (serviceId)
        {
            case IService.LOGIN:
                {
                    InitData();
                    break;
                }
            case IService.GET_USER_HERO_CARD:
                {
                    Game.main.socket.GetUserBattleDeck();
                    break;
                }
            case IService.GET_USER_BATTLE_DECK:
                {
                    PopupHeroCard.Show();
                    break;
                }
            case IService.SET_USER_BATTLE_DECK:
                {
                    GoBattle();
                    break;
                }
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (GameData.main.isX2Login)
        {
            GameData.main.isX2Login = false;
            PopupConfirm.Show(content: LangHandler.Get("logout-3", "Tài khoản của bạn bị đăng nhập trên thiết bị khác"));
        }
        InitData();
    }

    // Update is called once per frame
    //void Update() { }
}
