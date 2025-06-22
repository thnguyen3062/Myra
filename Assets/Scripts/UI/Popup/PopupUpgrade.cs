using GIKCore.Utilities;
using GIKCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GIKCore.UI;
using GIKCore.Net;
using GIKCore.DB;
using GIKCore.Lang;
using pbdson;
using SimpleJSON;

public class PopupUpgradeProps
{
    public int status;
    public long heroId;
    public int type;
    public int frameId = -1;
    public int curAtk = -1;
    public int curHp = -1;
    public int curMana = -1;
    public int gold = -1;
    public int goldRequired = 0;
    public int essence = -1;
    public int essenceRequired = 0;
    public int copyAvailable = -1;
    public int copyRequired = 0;
    public bool canUpgrade;
    public int nextAtk = -1;
    public int nextHp = -1;
    public int nextMana = -1;
    public int shardBonus = -1;
    public int shardCurrent = 0;
    public int shardAmount = 0;
}
public class PopupUpgrade : GameListener
{
    // Fields
    [SerializeField] private SC_Info_Upgrade m_InforUpgrade;
    [SerializeField] private SC_Bar_Nonstop m_ShardBarManager;
    [SerializeField] private CardUserInfor m_GodCard;
    [SerializeField] private CardUserInfor m_SpellCard;
    [SerializeField] private CardUserInfor m_MinionCard;
    [SerializeField] private GameObject m_ButtonClose;
    [SerializeField] private GameObject m_ListUpgradeRequired;
    // God
    [SerializeField] private GameObject m_GodImproveAtk;
    [SerializeField] private TextMeshProUGUI m_GodImproveAtkTxt;
    [SerializeField] private GameObject m_GodImproveHp;
    [SerializeField] private TextMeshProUGUI m_GodImproveHpTxt;
    [SerializeField] private GameObject m_GodImproveMana;
    [SerializeField] private TextMeshProUGUI m_GodImproveManaTxt;
    // Minion
    [SerializeField] private GameObject m_MinionImproveAtk;
    [SerializeField] private TextMeshProUGUI m_MinionImproveAtkTxt;
    [SerializeField] private GameObject m_MinionImproveHp;
    [SerializeField] private TextMeshProUGUI m_MinionImproveHpTxt;
    [SerializeField] private GameObject m_MinionImproveMana;
    [SerializeField] private TextMeshProUGUI m_MinionImproveManaTxt;
    // Minion
    [SerializeField] private GameObject m_SpellImproveMana;
    [SerializeField] private TextMeshProUGUI m_SpellImproveManaTxt;

    // Values
    private PopupUpgradeProps dataPopupUpgrade;

    // Methods
    public PopupUpgrade SetData(PopupUpgradeProps props)
    {
        dataPopupUpgrade = props;
        m_ListUpgradeRequired.SetActive(!(dataPopupUpgrade.goldRequired == 0 || dataPopupUpgrade.essenceRequired == 0 || dataPopupUpgrade.copyRequired == 0));

        SetCardData().SetCardInfoData().LoadUpgradeData();
        return this;
    }
    public PopupUpgrade SetCardData()
    {
        DBHero hero = Database.GetHero(dataPopupUpgrade.heroId);
        HideInforBounce(hero.type);
        switch (hero.type)
        {
            case DBHero.TYPE_GOD:
                {
                    m_GodCard.gameObject.SetActive(true);
                    m_SpellCard.gameObject.SetActive(false);
                    m_MinionCard.gameObject.SetActive(false);

                    m_GodCard.SetInfoCard(hero, 1, dataPopupUpgrade.frameId, false, dataPopupUpgrade.curAtk, dataPopupUpgrade.curHp, dataPopupUpgrade.curMana, true);
                    break;
                }
            case DBHero.TYPE_TROOPER_MAGIC:
            case DBHero.TYPE_BUFF_MAGIC:
                {
                    m_GodCard.gameObject.SetActive(false);
                    m_SpellCard.gameObject.SetActive(true);
                    m_MinionCard.gameObject.SetActive(false);

                    m_SpellCard.SetInfoCard(hero, 1, dataPopupUpgrade.frameId, false, dataPopupUpgrade.curAtk, dataPopupUpgrade.curHp, dataPopupUpgrade.curMana, true);
                    break;
                }
            case DBHero.TYPE_TROOPER_NORMAL:
                {
                    m_GodCard.gameObject.SetActive(false);
                    m_SpellCard.gameObject.SetActive(false);
                    m_MinionCard.gameObject.SetActive(true);

                    m_MinionCard.SetInfoCard(hero, 1, dataPopupUpgrade.frameId, false, dataPopupUpgrade.curAtk, dataPopupUpgrade.curHp, dataPopupUpgrade.curMana, true);
                    break;
                }
        }
        return this;
    }
    public PopupUpgrade SetInforBounce(long type)
    {
        switch (type)
        {
            case DBHero.TYPE_GOD:
                {
                    if (dataPopupUpgrade.nextAtk != dataPopupUpgrade.curAtk)
                    {
                        if (m_GodImproveAtk != null)
                            m_GodImproveAtk.SetActive(true);
                        if (m_GodImproveAtkTxt != null)
                            m_GodImproveAtkTxt.text = dataPopupUpgrade.nextAtk + "";
                    }
                    else
                    {
                        if (m_GodImproveAtk != null)
                            m_GodImproveAtk.SetActive(false);
                    }
                    if (dataPopupUpgrade.nextHp != dataPopupUpgrade.curHp)
                    {
                        if (m_GodImproveHp != null)
                            m_GodImproveHp.SetActive(true);
                        if (m_GodImproveHpTxt != null)
                            m_GodImproveHpTxt.text = dataPopupUpgrade.nextHp + "";
                    }
                    else
                    {
                        if (m_GodImproveHp != null)
                            m_GodImproveHp.SetActive(false);
                    }
                    if (dataPopupUpgrade.nextMana != dataPopupUpgrade.curMana)
                    {
                        if (m_GodImproveMana != null)
                            m_GodImproveMana.SetActive(true);
                        if (m_GodImproveManaTxt != null)
                            m_GodImproveManaTxt.text = dataPopupUpgrade.nextMana + "";
                    }
                    else
                    {
                        if (m_GodImproveMana != null)
                            m_GodImproveMana.SetActive(false);
                    }
                    break;
                }
            case DBHero.TYPE_TROOPER_MAGIC:
            case DBHero.TYPE_BUFF_MAGIC:
                {
                    if (dataPopupUpgrade.nextMana != dataPopupUpgrade.curMana)
                    {
                        if (m_SpellImproveMana != null)
                            m_SpellImproveMana.SetActive(true);
                        if (m_SpellImproveManaTxt != null)
                            m_SpellImproveManaTxt.text = dataPopupUpgrade.nextMana + "";
                    }
                    else
                    {
                        if (m_SpellImproveMana != null)
                            m_SpellImproveMana.SetActive(false);
                    }
                    break;
                }
            case DBHero.TYPE_TROOPER_NORMAL:
                {
                    if (dataPopupUpgrade.nextAtk != dataPopupUpgrade.curAtk)
                    {
                        if (m_MinionImproveAtk != null)
                            m_MinionImproveAtk.SetActive(true);
                        if (m_MinionImproveAtkTxt != null)
                            m_MinionImproveAtkTxt.text = dataPopupUpgrade.nextAtk + "";
                    }
                    else
                    {
                        if (m_MinionImproveAtk != null)
                            m_MinionImproveAtk.SetActive(false);
                    }
                    if (dataPopupUpgrade.nextHp != dataPopupUpgrade.curHp)
                    {
                        if (m_MinionImproveHp != null)
                            m_MinionImproveHp.SetActive(true);
                        if (m_MinionImproveHpTxt != null)
                            m_MinionImproveHpTxt.text = dataPopupUpgrade.nextHp + "";
                    }
                    else
                    {
                        if (m_MinionImproveHp != null)
                            m_MinionImproveHp.SetActive(false);
                    }
                    if (dataPopupUpgrade.nextMana != dataPopupUpgrade.curMana)
                    {
                        if (m_MinionImproveMana != null)
                            m_MinionImproveMana.SetActive(true);
                        if (m_MinionImproveManaTxt != null)
                            m_MinionImproveManaTxt.text = dataPopupUpgrade.nextMana + "";
                    }
                    else
                    {
                        if (m_MinionImproveMana != null)
                            m_MinionImproveMana.SetActive(false);
                    }
                    break;
                }
        }
        return this;
    }
    public PopupUpgrade HideInforBounce(long type)
    {
        switch (type)
        {
            case DBHero.TYPE_GOD:
                {
                    if (m_GodImproveAtk != null)
                        m_GodImproveAtk.SetActive(false);
                    if (m_GodImproveHp != null)
                        m_GodImproveHp.SetActive(false);
                    if (m_GodImproveMana != null)
                        m_GodImproveMana.SetActive(false);
                    break;
                }
            case DBHero.TYPE_TROOPER_MAGIC:
            case DBHero.TYPE_BUFF_MAGIC:
                {
                    if (m_SpellImproveMana != null)
                        m_SpellImproveMana.SetActive(false);
                    break;
                }
            case DBHero.TYPE_TROOPER_NORMAL:
                {
                    if (m_MinionImproveAtk != null)
                        m_MinionImproveAtk.SetActive(false);
                    if (m_MinionImproveHp != null)
                        m_MinionImproveHp.SetActive(false);
                    if (m_MinionImproveMana != null)
                        m_MinionImproveMana.SetActive(false);
                    break;
                }
        }
        return this;
    }
    public PopupUpgrade SetCardInfoData()
    {
        m_InforUpgrade.requiredAmount = dataPopupUpgrade.copyRequired;
        m_InforUpgrade.currentAmount = dataPopupUpgrade.copyAvailable;

        m_InforUpgrade.CurrentAmountsCoin_Ess_Card.Clear();
        m_InforUpgrade.CurrentAmountsCoin_Ess_Card = new List<int> { dataPopupUpgrade.gold, dataPopupUpgrade.essence, dataPopupUpgrade.copyAvailable };
        m_InforUpgrade.RequiredAmountsCoin_Ess_Card.Clear();
        m_InforUpgrade.RequiredAmountsCoin_Ess_Card = new List<int> { dataPopupUpgrade.goldRequired, dataPopupUpgrade.essenceRequired, dataPopupUpgrade.copyRequired };
        m_InforUpgrade.currentStatIntAtk_Hp_Mana.Clear();
        m_InforUpgrade.currentStatIntAtk_Hp_Mana = new List<int> { dataPopupUpgrade.curAtk, dataPopupUpgrade.curHp, dataPopupUpgrade.curMana };
        m_InforUpgrade.newStatIntAtk_Hp_Mana.Clear();
        m_InforUpgrade.newStatIntAtk_Hp_Mana = new List<int> { dataPopupUpgrade.nextAtk, dataPopupUpgrade.nextHp, dataPopupUpgrade.nextMana };
        m_InforUpgrade.Level = dataPopupUpgrade.frameId;
        if (dataPopupUpgrade.shardBonus <= 0)
            dataPopupUpgrade.shardBonus = 0;
        m_InforUpgrade.BonusAmount = dataPopupUpgrade.shardBonus;
        DBHero hero = Database.GetHero(dataPopupUpgrade.heroId);
        SetInforBounce(hero.type);
        return this;
    }
    public PopupUpgrade LoadUpgradeData()
    {
        m_InforUpgrade.LoadData().UpdateInfo(dataPopupUpgrade.frameId);
        return this;
    }
    public void DoClickUpgrade()
    {
        if (ProgressionController.instance != null)
        {
            if (GameData.main.userProgressionState == 4)
            {
                ProgressionController.instance.NextState();
            }
            if (GameData.main.userProgressionState == 8)
            {
                ProgressionController.instance.NextState();
            }
        }
        if (dataPopupUpgrade.canUpgrade)
            Game.main.socket.Upgrade(dataPopupUpgrade.heroId);
        else
            Toast.Show("Can not upgrade card");
    }
    public void CheckAnim()
    {
        PopupUpgradeProps props = new PopupUpgradeProps();
        props.status = 1;
        props.heroId = 107;
        props.type = 0;
        props.frameId = 2;
        props.curAtk = 2;
        props.curHp = 1;
        props.curMana = 1;
        props.gold = 300;
        props.goldRequired = 500;
        props.essence = 0;
        props.essenceRequired = 100;
        props.copyAvailable = 2;
        props.copyRequired = 7;
        props.canUpgrade = false;
        props.nextAtk = 2;
        props.nextHp = 2;
        props.nextMana = 1;
        props.shardBonus = 10;
        props.shardCurrent = 5;
        props.shardAmount = 60;
        DoUpgradeOnTut(props);
    }
    public void DoUpgradeOnTut(PopupUpgradeProps props)
    {
        if (props.nextAtk == -1)
            props.nextAtk = props.curAtk;
        if (props.nextHp == -1)
            props.nextHp = props.curHp;
        if (props.nextMana == -1)
            props.nextMana = props.curMana;
        dataPopupUpgrade = props;
        if (m_ShardBarManager != null)
        {
            m_ShardBarManager.SetData(props.shardCurrent, props.shardAmount);
        }

        SetCardData();
        IUtil.ScheduleOnce(this, () =>
        {
            SetCardInfoData().LoadUpgradeData();
        }, 2);
        IUtil.ScheduleOnce(this, () =>
        {
            m_ButtonClose.SetActive(true);
            ProgressionController.instance.canSkip = true;
        }, 10);


    }
    public void DoClickClose()
    {
        if (ProgressionController.instance != null)
        {
            if (!ProgressionController.instance.canSkip)
                return;
            ProgressionController.instance.NextState();
        }
        Destroy(gameObject);
    }
    public override bool ProcessSocketData(int serviceId, byte[] data)
    {
        if (base.ProcessSocketData(serviceId, data)) return true;
        switch (serviceId)
        {
            case IService.UPGRADE:
                {
                    Game.main.socket.ViewUpgrade();
                    Game.main.socket.GetUserCurrency();
                    Game.main.socket.GetUserHeroCard();
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    m_ButtonClose.SetActive(false);
                    if (ProgressionController.instance == null)
                    {
                        PopupUpgradeProps props = new PopupUpgradeProps();
                        for (int i = 0; i < cv.aLong.Count; i++)
                        {
                            if (i == 0)
                                props.status = (int)cv.aLong[i];
                            else if (i == 1)
                                props.heroId = cv.aLong[i];
                            else if (i == 2)
                                props.frameId = (int)cv.aLong[i];
                            else if (i == 3)
                                props.curAtk = (int)cv.aLong[i];
                            else if (i == 4)
                                props.curHp = (int)cv.aLong[i];
                            else if (i == 5)
                                props.curMana = (int)cv.aLong[i];
                            else if (i == 6)
                                props.shardAmount = (int)cv.aLong[i];
                            else if (i == 7)
                                props.shardCurrent = (int)cv.aLong[i];
                            else if (i == 8)
                                props.gold = (int)cv.aLong[i];
                            else if (i == 9)
                                props.goldRequired = (int)cv.aLong[i];
                            else if (i == 10)
                                props.essence = (int)cv.aLong[i];
                            else if (i == 11)
                                props.essenceRequired = (int)cv.aLong[i];
                            else if (i == 12)
                                props.copyAvailable = (int)cv.aLong[i];
                            else if (i == 13)
                                props.copyRequired = (int)cv.aLong[i];
                            else if (i == 14)
                                props.canUpgrade = (int)cv.aLong[i] == 1;
                            else if (i == 15)
                                props.nextAtk = (int)cv.aLong[i];
                            else if (i == 16)
                                props.nextHp = (int)cv.aLong[i];
                            else if (i == 17)
                                props.nextMana = (int)cv.aLong[i];
                            else if (i == 18)
                                props.shardBonus = (int)cv.aLong[i];
                        }
                        if (m_ShardBarManager != null)
                        {
                            m_ShardBarManager.SetData(props.shardCurrent, props.shardAmount);
                        }

                        if (props.nextAtk == -1)
                            props.nextAtk = props.curAtk;
                        if (props.nextHp == -1)
                            props.nextHp = props.curHp;
                        if (props.nextMana == -1)
                            props.nextMana = props.curMana;
                        //Debug.Log(string.Format("heroId: {6} ==========curAtk: {0} | nextAtk: {1} | curHp: {2} | nextHp: {3} | curMana: {4} | nextMana: {5}", props.curAtk, props.nextAtk, props.curHp, props.nextHp, props.curMana, props.nextMana, props.heroId));
                        dataPopupUpgrade = props;
                        SetCardData();
                        IUtil.ScheduleOnce(this, () =>
                        {
                            SetCardInfoData().LoadUpgradeData();
                        }, 3);
                        IUtil.ScheduleOnce(this, () =>
                        {
                            m_ButtonClose.SetActive(true);
                        }, 9);
                    }
                    break;
                }
        }
        return false;
    }
    public static void Show(PopupUpgradeProps dataProps)
    {
        string assetName = "PopupUpgrade";
        GameObject go = IUtil.LoadPrefabWithParent("Prefabs/Home/" + assetName, Game.main.canvas.panelPopup);

        PopupUpgrade script = go.GetComponent<PopupUpgrade>();
        script.SetData(dataProps);
    }
}
