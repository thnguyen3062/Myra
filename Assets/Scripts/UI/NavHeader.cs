using GIKCore;
using GIKCore.Lang;
using GIKCore.Net;
using GIKCore.Sound;
using GIKCore.UI;
using GIKCore.Utilities;
using pbdson;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NavHeader : GameListener
{
    // Fields
    [SerializeField] private SettingController m_SettingController;
    [SerializeField] private TextMeshProUGUI m_ScreenName, m_Level, m_Gem, m_Coin, m_Essence;
    [SerializeField] private TextMeshProUGUI m_BaseHp, m_BaseHpPopup;
    [SerializeField] private Image m_ExpProgressBar;

    // Values

    // Methods
    public NavHeader SetData()
    {
        m_ScreenName.text = GameData.main.profile.displayName;
        m_Gem.text = IUtil.FormatKoinAndRoundUp(GameData.main.userCurrency.gem);
        m_Coin.text = IUtil.FormatKoinAndRoundUp(GameData.main.userCurrency.gold);
        m_Essence.text = IUtil.FormatKoinAndRoundUp(GameData.main.userCurrency.essence);
        DBBaseHp baseHp = Database.GetBaseHp((int)GameData.main.userCurrency.shard);
        if (m_BaseHp != null)
            m_BaseHp.text = baseHp.hp + "";
        if (m_BaseHpPopup != null)
            m_BaseHpPopup.text = baseHp.hp + "";
        //Debug.Log("exp: " + GameData.main.userCurrency.exp);
        DBLevel level = Database.GetUserLevelInfo(GameData.main.userCurrency.exp);
        m_Level.text = level.id + "";
        m_ExpProgressBar.fillAmount = (float)level.expCurrent / (float)level.expToUpLevel;
        return this;
    }
    public void DoClickSetting()
    {
        SoundHandler.main.PlaySFX("900_click_4", "sounds");
        m_SettingController.gameObject.SetActive(true);
    }

    public void DoClickDisableButton()
    {
        SoundHandler.main.PlaySFX("900_click_4", "sounds");
        Toast.Show(LangHandler.Get("toast-8", "Coming Soon!"));
    }
    void Start()
    {
        SetData();
    }
    protected override void Awake()
    {
        base.Awake();
    }
    public override bool ProcessSocketData(int id, byte[] data)
    {
        if (base.ProcessSocketData(id, data))
            return true;

        switch (id)
        {
            case IService.GET_BALANCE:
                {
                    CommonVector commonVector = ISocket.Parse<CommonVector>(data);
                    GameData.main.userCurrency.gold = commonVector.aLong[0];
                    GameData.main.userCurrency.gem = commonVector.aLong[1];
                    GameData.main.userCurrency.exp = commonVector.aLong[2];
                    GameData.main.userCurrency.essence = commonVector.aLong[3];
                    GameData.main.userCurrency.shard = commonVector.aLong[4];
                    m_Gem.text = IUtil.FormatKoinAndRoundUp(GameData.main.userCurrency.gem);
                    m_Coin.text = IUtil.FormatKoinAndRoundUp(GameData.main.userCurrency.gold);
                    m_Essence.text = IUtil.FormatKoinAndRoundUp(GameData.main.userCurrency.essence);
                    DBLevel level = Database.GetUserLevelInfo(GameData.main.userCurrency.exp);
                    m_Level.text = level.id + "";
                    m_ExpProgressBar.fillAmount = (float)level.expCurrent / (float)level.expToUpLevel;
                    DBBaseHp baseHp = Database.GetBaseHp((int)GameData.main.userCurrency.shard);
                    if (m_BaseHp != null)
                        m_BaseHp.text = baseHp.hp + "";
                    if (m_BaseHpPopup != null)
                        m_BaseHpPopup.text = baseHp.hp + "";
                    break;
                }
        }
        return false;
    }
    public override bool ProcessNetData(int id, object o)
    {
        if (base.ProcessNetData(id, o)) return true;
        switch (id)
        {
            case NetData.RECEIVE_LOCAL_BALANCE:
                {
                    //0 gold 1 gem 2 exp 3 essence 4 shard 
                    List<long> list = (List<long>)o;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (i == 0)
                            GameData.main.userCurrency.gold = list[0];
                        else if (i == 1)
                            GameData.main.userCurrency.gem = list[1];
                        else if (i == 2)
                            GameData.main.userCurrency.exp = list[2];
                        else if (i == 3)
                            GameData.main.userCurrency.essence = list[3];
                        else if (i == 4)
                            GameData.main.userCurrency.shard = list[4];
                    }
                    m_Gem.text = IUtil.FormatKoinAndRoundUp(GameData.main.userCurrency.gem);
                    m_Coin.text = IUtil.FormatKoinAndRoundUp(GameData.main.userCurrency.gold);
                    m_Essence.text = IUtil.FormatKoinAndRoundUp(GameData.main.userCurrency.essence);
                    DBLevel level = Database.GetUserLevelInfo(GameData.main.userCurrency.exp);
                    m_Level.text = level.id + "";
                    m_ExpProgressBar.fillAmount = (float)level.expCurrent / (float)level.expToUpLevel;
                    DBBaseHp baseHp = Database.GetBaseHp((int)GameData.main.userCurrency.shard);
                    if (m_BaseHp != null)
                        m_BaseHp.text = baseHp.hp + "";
                    if (m_BaseHpPopup != null)
                        m_BaseHpPopup.text = baseHp.hp + "";
                    break;
                }
        }
        return false;
    }
}
