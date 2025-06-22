using GIKCore.Utilities;
using GIKCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GIKCore.UI;
using System;
using GIKCore.Lang;

public class PopupChestDetailProps
{
    public long chestId;
    public int numberCard;
    public long gold;
    public long exp;
    public int essence;
    public int sizeCommon;
    public int sizeRare;
    public int sizeEpic;
    public int sizeLegendary;
    public bool isActivate;
    public bool haveOtherActivated;
    public int gemPrice;
    public int chestType;
    public long remainTime;
}
public class PopupChestDetail : MonoBehaviour
{
    // Fields
    // name standard, premium, godly
    [SerializeField] private List<GameObject> m_ListBackground;
    [SerializeField] private TextMeshProUGUI m_ChestLevel;
    [SerializeField] private TextMeshProUGUI m_ChestName;    
    [SerializeField] private TextMeshProUGUI m_TextCards;
    [SerializeField] private TextMeshProUGUI m_TextGold;
    [SerializeField] private TextMeshProUGUI m_TextExp;
    [SerializeField] private TextMeshProUGUI m_TextEssence;
    [SerializeField] private TextMeshProUGUI m_TextAmountLegendary;
    [SerializeField] private TextMeshProUGUI m_TextAmountEpic;
    [SerializeField] private TextMeshProUGUI m_TextAmountRare;
    [SerializeField] private TextMeshProUGUI m_TextAmountCommon;
    [SerializeField] private TextMeshProUGUI m_TextTimeRemainUnlock;
    [SerializeField] private TextMeshProUGUI m_TextTimeRemainLock;
    [SerializeField] private TextMeshProUGUI m_TextPriceGemRunning;
    [SerializeField] private TextMeshProUGUI m_TextPriceGemUnlock;
    [SerializeField] private GoOnOff m_StateOnOff;
    [SerializeField] private GoOnOff m_StateOnStatus;
    // Values
    private PopupChestDetailProps dataChest;
    private ITimeDelta timeChestRemain = 0;
    private bool isSetTimeChest = false;

    // Methods
    public PopupChestDetail SetData(PopupChestDetailProps props)
    {
        DBLevel level = Database.GetUserLevelInfo(GameData.main.userCurrency.exp);
        dataChest = props;
        timeChestRemain = dataChest.remainTime / 1000;
        isSetTimeChest = dataChest.remainTime > 0;

        // header
        m_ChestLevel.text = LangHandler.Get("761", "LEVEL") + " " + level.id;
        switch (dataChest.chestType)
        {
            case 3:
                {
                    if(GameData.main.userProgressionState >=14)
                    {
                        m_ChestName.text = LangHandler.Get("763", "STANDARD CHEST");
                        m_TextTimeRemainLock.text = "3H";
                    }
                    else
                    {
                        m_ChestName.text = LangHandler.Get("764", "WELCOME CHEST");
                        m_TextTimeRemainLock.text = "5S";

                    }
                    break;
                }
            case 8:
                {
                    if (GameData.main.userProgressionState >= 14)
                    {
                        m_ChestName.text = LangHandler.Get("765", "PREMIUM CHEST");
                        m_TextTimeRemainLock.text = "8H";
                    }
                    else
                    {
                        m_ChestName.text = LangHandler.Get("764", "WELCOME CHEST");
                        m_TextTimeRemainLock.text = "5S";
                    }
                    break;
                }
            case 12:
                {
                    m_ChestName.text = LangHandler.Get("766", "GODLY CHEST");
                    m_TextTimeRemainLock.text = "12H";
                    break;
                }
        }

        // background
        if (!dataChest.isActivate && !dataChest.haveOtherActivated)
        {
            m_ListBackground[0].SetActive(true);
            m_ListBackground[1].SetActive(false);
            m_StateOnOff.TurnOff();
        }
        else
        {
            m_ListBackground[0].SetActive(false);
            m_ListBackground[1].SetActive(true);
            m_StateOnOff.TurnOn();
            m_StateOnStatus.Turn(dataChest.isActivate);
        }

        // body
        m_TextCards.text = LangHandler.Get("card-1", "CARDS", to: LangData.To.UpperFistLetter) +  " x" + dataChest.numberCard;
        m_TextGold.text = LangHandler.Get("643", "COINS", to: LangData.To.UpperFistLetter) + " x" + dataChest.gold;
        m_TextExp.text = LangHandler.Get("771", "EXP", to: LangData.To.UpperFistLetter) + " x" + dataChest.exp;
        m_TextEssence.text = LangHandler.Get("645", "ESSENCE", to: LangData.To.UpperFistLetter) + " x" + dataChest.essence;

        // size
        m_TextAmountLegendary.text = dataChest.sizeLegendary + "";
        m_TextAmountEpic.text = dataChest.sizeEpic + "";
        m_TextAmountRare.text = dataChest.sizeRare + "";
        m_TextAmountCommon.text = dataChest.sizeCommon + "";

        // footer
        m_TextTimeRemainUnlock.text = dataChest.remainTime + "";
        m_TextPriceGemRunning.text = dataChest.gemPrice + "";
        m_TextPriceGemUnlock.text = dataChest.gemPrice + "";

        return this;
    }
    public void Close()
    {
        Destroy(gameObject);
    }
    public void DoClickActivate()
    {
        Game.main.socket.ActivateTimeChest(dataChest.chestId);
        Close();
    }
    public void DoClickOpen()
    {
        Game.main.socket.OpenTimeChest(dataChest.chestId);
        Close();
    }
    public static void Show(PopupChestDetailProps data)
    {
        string assetName = "PopupChestDetail";
        GameObject go = IUtil.LoadPrefabWithParent("Prefabs/Home/" + assetName, Game.main.canvas.panelPopup);
        PopupChestDetail script = go.GetComponent<PopupChestDetail>();
        script.SetData(data);
    }
    private void Update()
    {
        if (isSetTimeChest)
        {
            if (timeChestRemain > 0)
            {
                timeChestRemain.MakeTimePassInSeconds();
                if (timeChestRemain <= 0)
                {
                    Game.main.socket.GetTimeChest((int)dataChest.chestId);
                    isSetTimeChest = false;
                    Destroy(gameObject);
                }
                var ts = TimeSpan.FromSeconds(timeChestRemain.time);
                m_TextTimeRemainUnlock.text = string.Format("{0}h {1}min", ts.Hours, ts.Minutes);
            }
        }
    }
}
