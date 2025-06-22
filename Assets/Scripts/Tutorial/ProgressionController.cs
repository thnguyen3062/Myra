using DG.Tweening;
using GIKCore;
using GIKCore.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using GIKCore.Net;
using TMPro;
using GIKCore.UI;
using GIKCore.Lang;
using pbdson;

public class ProgressionController : GameListener
{
    [SerializeField] private GameObject[] progressionStates;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_InputField m_Nickname;
    [SerializeField] private GameObject m_BlackHole;
    [SerializeField] private GameObject m_WaitingPanel;
    public long index;
    public static ProgressionController instance;
    public bool canSkip = true;
    private string m_Name = "";
    protected override void Awake()
    {
        base.Awake();
        if (GameData.main.userProgressionState < 14)
        {
            DontDestroyOnLoad(transform.root.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        instance = this;
    }
    private void CheckInitState(long svState)
    {
        switch (svState)
        {
            case 0:
                {
                    index = 0;
                    Game.main.LoadScene("BattleSceneTutorial", () =>
                    {
                        BattleSceneTutorial.instance.SetTutorial(0);
                    }, delay: 0.3f, curtain: true);
                    progressionStates[0].SetActive(false);
                    break;
                }
            case 1:
                {
                    index = 0;
                    progressionStates[0].SetActive(false);
                    HomeChestProps hc = new HomeChestProps();
                    hc.state = HomeChestProps.ChestState.NOT_ACTIVATED;
                    hc.status = HomeChestProps.ChestStatus.CHEST_OPEN_NOW;
                    hc.timeChest = HomeChestProps.TimeChest.TIME_3H;
                    hc.remainTime = 0;
                    hc.isNew = true;
                    hc.canOpen = true;
                    hc.turnOnPointClick = true;
                    hc.id = -1;
                    HomeChestProps hcEmpty = new HomeChestProps();
                    hcEmpty.state = HomeChestProps.ChestState.EMPTY;
                    hcEmpty.status = HomeChestProps.ChestStatus.CHEST_EMPTY;
                    hcEmpty.id = 0;
                    GameData.main.listHomeChestProps.Clear();
                    GameData.main.listHomeChestProps.Add(hc);
                    GameData.main.listHomeChestProps.Add(hcEmpty);
                    GameData.main.listHomeChestProps.Add(hcEmpty);
                    GameData.main.listHomeChestProps.Add(hcEmpty);
                    Game.main.LoadScene("HomeSceneNew",
                   () => {
                       HomeSceneNew.instance.UpdateHomeChestTut();
                       HandleNetData.QueueNetData(NetData.RECEIVE_LOCAL_BALANCE, new List<long>() { 0, 0, 0, 0, 0 });
                   }, delay: 0.3f, curtain: true);
                    break;
                }
            case 2:
                {
                    index = 1;
                    progressionStates[1].SetActive(false);
                    HomeChestProps hc = new HomeChestProps();
                    hc.state = HomeChestProps.ChestState.ACTIVATED;
                    hc.status = HomeChestProps.ChestStatus.CHEST_OPENING;
                    hc.timeChest = HomeChestProps.TimeChest.TIME_3H;
                    hc.remainTime = 5000;
                    hc.isNew = false;
                    hc.canOpen = true;
                    hc.id = -1;
                    HomeChestProps hcEmpty = new HomeChestProps();
                    hcEmpty.state = HomeChestProps.ChestState.EMPTY;
                    hcEmpty.status = HomeChestProps.ChestStatus.CHEST_EMPTY;
                    hcEmpty.id = 0;
                    GameData.main.listHomeChestProps.Clear();
                    GameData.main.listHomeChestProps.Add(hc);
                    GameData.main.listHomeChestProps.Add(hcEmpty);
                    GameData.main.listHomeChestProps.Add(hcEmpty);
                    GameData.main.listHomeChestProps.Add(hcEmpty);
                    Game.main.LoadScene("HomeSceneNew",
                   () => {
                       HomeSceneNew.instance.UpdateHomeChestTut();

                   }, delay: 0f, curtain: true);
                    break;
                }
            case 3:
                {
                    index = 1;
                    Game.main.LoadScene("HomeSceneNew",
                        () =>
                        {
                            //progressionStates[index].SetActive(true);
                            DisplayState(index);
                            HandleNetData.QueueNetData(NetData.RECEIVE_LOCAL_BALANCE, new List<long>() { 500, 0, 100, 50, 0 });
                        }, delay: 0.3f, curtain: true);

                    break;
                }
            case 4:
                {
                    index = 2;
                    Game.main.LoadScene("CollectionScene", onLoadSceneSuccess: () =>
                    {
                        HandleNetData.QueueNetData(NetData.RECEIVE_LOCAL_BALANCE, new List<long>() { 500, 0, 100, 50, 0 });
                        CollectionScene.instance.InitDataContent(1);
                        canSkip = true;
                        DisplayState(index);
                    }, delay: 0.3f, curtain: true);
                    break;
                }
            case 5:
                {
                    index = 5;
                    Game.main.LoadScene("HomeSceneNew",
                       () =>
                       {
                           //progressionStates[index].SetActive(true);
                           DisplayState(index);
                           HandleNetData.QueueNetData(NetData.RECEIVE_LOCAL_BALANCE, new List<long>() { 300, 0, 100, 0, 5 });
                       }, delay: 0.3f, curtain: true);
                    break;
                }
            case 6:
                {
                    index = 5;
                    Game.main.LoadScene("HomeSceneNew",
                       () =>
                       {
                           progressionStates[index].SetActive(false);
                           HomeChestProps hc = new HomeChestProps();
                           hc.state = HomeChestProps.ChestState.NOT_ACTIVATED;
                           hc.status = HomeChestProps.ChestStatus.CHEST_OPEN_NOW;
                           hc.timeChest = HomeChestProps.TimeChest.TIME_8H;
                           hc.remainTime = 0;
                           hc.isNew = true;
                           hc.canOpen = true;
                           hc.turnOnPointClick = true;
                           hc.id = -1;
                           HomeChestProps hcEmpty = new HomeChestProps();
                           hcEmpty.state = HomeChestProps.ChestState.EMPTY;
                           hcEmpty.status = HomeChestProps.ChestStatus.CHEST_EMPTY;
                           hcEmpty.id = 0;
                           GameData.main.listHomeChestProps.Clear();
                           GameData.main.listHomeChestProps.Add(hc);
                           GameData.main.listHomeChestProps.Add(hcEmpty);
                           GameData.main.listHomeChestProps.Add(hcEmpty);
                           GameData.main.listHomeChestProps.Add(hcEmpty);
                           Game.main.LoadScene("HomeSceneNew",
                          () => {
                              HomeSceneNew.instance.UpdateHomeChestTut();
                              HandleNetData.QueueNetData(NetData.RECEIVE_LOCAL_BALANCE, new List<long>() { 300, 0, 100, 0, 5 });
                          }, delay: 0.3f, curtain: true);
                       }, delay: 0.3f, curtain: true);
                    break;
                }
            case 7:
                {
                    index = 6;
                    progressionStates[index].SetActive(false);
                    HomeChestProps hc = new HomeChestProps();
                    hc.state = HomeChestProps.ChestState.ACTIVATED;
                    hc.status = HomeChestProps.ChestStatus.CHEST_OPENING;
                    hc.timeChest = HomeChestProps.TimeChest.TIME_8H;
                    hc.remainTime = 5000;
                    hc.isNew = false;
                    hc.canOpen = true;
                    hc.id = -1;
                    HomeChestProps hcEmpty = new HomeChestProps();
                    hcEmpty.state = HomeChestProps.ChestState.EMPTY;
                    hcEmpty.status = HomeChestProps.ChestStatus.CHEST_EMPTY;
                    hcEmpty.id = 0;
                    GameData.main.listHomeChestProps.Clear();
                    GameData.main.listHomeChestProps.Add(hc);
                    GameData.main.listHomeChestProps.Add(hcEmpty);
                    GameData.main.listHomeChestProps.Add(hcEmpty);
                    GameData.main.listHomeChestProps.Add(hcEmpty);
                    Game.main.LoadScene("HomeSceneNew",
                   () => {
                       HomeSceneNew.instance.UpdateHomeChestTut();
                       HandleNetData.QueueNetData(NetData.RECEIVE_LOCAL_BALANCE, new List<long>() { 300, 0, 100, 0, 5 });
                   }, delay: 0f, curtain: true);
                    break;
                }
            case 8:
                {
                    index = 6;
                    progressionStates[index].SetActive(false);
                    Game.main.LoadScene("CollectionScene", onLoadSceneSuccess: () =>
                    {
                        HandleNetData.QueueNetData(NetData.RECEIVE_LOCAL_BALANCE, new List<long>() { 2300, 0, 300, 400, 5 });
                        CollectionScene.instance.InitDataContent(1);
                        m_BlackHole.SetActive(true);
                    }, delay: 0.3f, curtain: true);
                    break;
                }
            case 9:
                {
                    index = 9;
                    
                    Game.main.LoadScene("HomeSceneNew",
                       () => {
                           DisplayState(index);
                           HandleNetData.QueueNetData(NetData.RECEIVE_LOCAL_BALANCE, new List<long>() { 1800, 0, 300, 300, 15 });
                       }, delay: 0.3f, curtain: true);
                    break;
                }
            case 10:
                {
                    index = 10;

                    Game.main.LoadScene("HomeSceneNew",
                       () => {
                           Game.main.socket.GetUserCurrency();
                           DisplayState(index);
                       }, delay: 0.3f, curtain: true);
                    break;
                }
            case 11:
                {
                    index = 11;

                    Game.main.LoadScene("HomeSceneNew",
                       () => {
                           Game.main.socket.GetUserCurrency();
                           DisplayState(index);
                       }, delay: 0.3f, curtain: true);
                    break;
                }
            case 12:
                {
                    index = 12;

                    Game.main.LoadScene("HomeSceneNew",
                       () => {
                           Game.main.socket.GetUserCurrency();
                           DisplayState(index);
                       }, delay: 0.3f, curtain: true);
                    break;
                }
            case 13:
                {
                    index = 13;

                    Game.main.LoadScene("HomeSceneNew",
                       () => {
                           Game.main.socket.GetUserCurrency();
                           DisplayState(index);
                       }, delay: 0.3f, curtain: true);
                    break;
                }
        }

    }
    private void Start()
    {
        m_BlackHole.SetActive(false);
        foreach (GameObject state in progressionStates)
        {
            state.SetActive(false);
        }
        CheckInitState(GameData.main.userProgressionState);

    }
    public void NextState()
    {
            panel.SetActive(true);
            foreach (GameObject state in progressionStates)
            {
                state.SetActive(false);
            }
            index++;
            DisplayState(index);
    }
    public void HideState()
    {
        progressionStates[index].SetActive(false);
    }
    private void DisplayState(long state)
    {
        m_BlackHole.SetActive(false);
        progressionStates[index].SetActive(true);
        switch (state)
        {
            case 0:
                {
                    PopupChestDetailProps props = new PopupChestDetailProps();
                    props.chestId = 0;
                    props.numberCard = 3;
                    props.gold = 500;
                    props.exp = 1350;
                    props.essence = 50;
                    props.sizeCommon = 12;
                    props.sizeRare = 6;   
                    props.sizeEpic = 2;
                    props.sizeLegendary = 1;
                    props.isActivate = false;
                    props.haveOtherActivated = false;
                    props.gemPrice = 0;
                    props.chestType = 3;
                    props.remainTime = 5;

                    progressionStates[index].transform.GetChild(0).GetComponent<PopupChestDetail>().SetData(props);

                    break;
                }
            case 1:
                {
                    //get default name
                    GetDefaultNickname();
                    m_Nickname.onValueChanged.AddListener(OnInputFieldValueChanged);
                    

                    break;
                }
            case 2:
                {
                    progressionStates[index].GetComponent<SetTextProgression>().SetData(LangHandler.Get("734", "Welcome") + " " + GameData.main.profile.screenname + "! " + LangHandler.Get("735", "Where were we? Oh, upgrade cards!"));
                    break;
                }
            case 3:
                {
                    PopupUpgradeProps props = new PopupUpgradeProps();
                    props.status = 1;
                    props.heroId = 107;
                    props.type = 0;
                    props.frameId = 1;
                    props.curAtk = 1;
                    props.curHp = 1;
                    props.curMana = 1;
                    props.gold = 500;
                    props.goldRequired = 200;
                    props.essence = 50;
                    props.essenceRequired = 50;
                    props.copyAvailable = 5;
                    props.copyRequired = 3;
                    props.canUpgrade = true;
                    props.nextAtk = 2;
                    props.nextHp = 1;
                    props.nextMana = 1;
                    props.shardBonus = 5;
                    progressionStates[index].transform.GetChild(0).GetComponent<PopupUpgrade>().SetData(props);
                    break;

                }
            case 4:
                {
                    progressionStates[index].GetComponent<SetTextProgression>().SetData(LangHandler.Get("739", "A challenger awaits") + ", " + GameData.main.profile.screenname + ". " + LangHandler.Get("737", "We can test your stronger card.")  );
                    
                    break;
                }
            case 6:
                {
                    PopupChestDetailProps props = new PopupChestDetailProps();
                    props.chestId = 0;
                    props.numberCard = 9;
                    props.gold = 2000;
                    props.exp = 200;
                    props.essence = 400;
                    props.sizeCommon = 12;
                    props.sizeRare = 6;
                    props.sizeEpic = 2;
                    props.sizeLegendary = 1;
                    props.isActivate = false;
                    props.haveOtherActivated = false;
                    props.gemPrice = 0;
                    props.chestType = 8;
                    props.remainTime = 5;

                    progressionStates[index].transform.GetChild(0).GetComponent<PopupChestDetail>().SetData(props);
                    break;
                }

            case 7:
                {
                    PopupUpgradeProps props = new PopupUpgradeProps();
                    props.status = 1;
                    props.heroId = 107;
                    props.type = 0;
                    props.frameId = 2;
                    props.curAtk = 2;
                    props.curHp = 1;
                    props.curMana = 1;
                    props.gold = 2300;
                    props.goldRequired = 500;
                    props.essence = 400;
                    props.essenceRequired = 100;
                    props.copyAvailable = 10;
                    props.copyRequired = 7;
                    props.canUpgrade = true;
                    props.nextAtk = 2;
                    props.nextHp = 2;
                    props.nextMana = 1;
                    props.shardBonus = 10;

                    progressionStates[index].transform.GetChild(0).GetComponent<PopupUpgrade>().SetData(props);
                    break;

                }
            case 8:
                {
                    progressionStates[index].GetComponent<SetTextProgression>().SetData(LangHandler.Get("738","Ohh, upgrading cards help you strengthen your base") + ", " + GameData.main.profile.screenname);
                    
                    break;
                }

            case 9:
                {
                    progressionStates[index].GetComponent<SetTextProgression>().SetData(GameData.main.profile.screenname +", " + LangHandler.Get("775", "I was hiding this secret from you, but now you are ready.") +"\n" +LangHandler.Get("776", "The bonus scroll - Let's try them out"));
                    break;
                }
            case 10:
                {
                    progressionStates[index].GetComponent<SetTextProgression>().SetData(LangHandler.Get("741", "Ohh, exp helps you level up") + ", " + GameData.main.profile.screenname + "." + LangHandler.Get("742", "Tap here to see!"));
                    break;
                }
            case 11:
                {
                    progressionStates[index].GetComponent<SetTextProgression>().SetData(LangHandler.Get("778", "Most people would have given up by now.") + "\n " + LangHandler.Get("779", "But I have faith that tough ones shall remain."));
                    break;
                }
            case 13:
                {
                    if (!PlayerPrefs.HasKey("TRACK_END_PROGRESSION"))
                    {
                        PlayerPrefs.SetInt("TRACK_END_PROGRESSION", 1);
                        ITrackingParameter pr = new ITrackingParameter() { name = "tutorial_2_progress_complete", value = "true" };
                        ITracking.LogEventFirebase(ITracking.TRACK_END_PROGRESSION, pr);
                    }
                    Game.main.socket.UpdateProgression(13);
                    GameData.main.userProgressionState = 14;
                    string str = string.Format(LangHandler.Get("780", "Few have the will to brave this new world like you do, {0}."), GameData.main.profile.screenname) +"\n"+ LangHandler.Get("781", "I can see you among the bests out there!");
                    progressionStates[index].GetComponent<SetTextProgression>().SetData(str);
                    break;
                }
        }
    }
    public void DoActionInState()
    {
        switch (index)
        {

            case 0:
                {
                    if (GameData.main.userProgressionState == 1)
                    {
                        // ??i state cho ruong time chest
                        // unlock time chest 1 
                        DisplayState(index);
                        index++;

                    }
                    break;
                }
            case 1:
                {
                    if (GameData.main.userProgressionState == 1)
                    {
                        // click unlock chest
                        Game.main.socket.UpdateProgression(1);
                        GameData.main.userProgressionState = 2;
                        progressionStates[0].SetActive(false);
                        HomeChestProps hc = new HomeChestProps();
                        hc.state = HomeChestProps.ChestState.ACTIVATED;
                        hc.status = HomeChestProps.ChestStatus.CHEST_OPENING;
                        hc.timeChest = HomeChestProps.TimeChest.TIME_3H;
                        hc.remainTime = 5000;
                        hc.isNew = false;
                        hc.canOpen = true;
                        hc.id = -1;
                        hc.turnOnPointClick = true;
                        HomeChestProps hcEmpty = new HomeChestProps();
                        hcEmpty.state = HomeChestProps.ChestState.EMPTY;
                        hcEmpty.status = HomeChestProps.ChestStatus.CHEST_EMPTY;
                        hcEmpty.id = 0;

                        GameData.main.listHomeChestProps.Clear();
                        GameData.main.listHomeChestProps.Add(hc);
                        GameData.main.listHomeChestProps.Add(hcEmpty);
                        GameData.main.listHomeChestProps.Add(hcEmpty);
                        GameData.main.listHomeChestProps.Add(hcEmpty);
                        HomeSceneNew.instance.UpdateHomeChestTut();
                    }
                    else if (GameData.main.userProgressionState == 2)
                    {
                        //click open reward chest 
                        Game.main.socket.UpdateProgression(2);
                        GameData.main.userProgressionState = 3;
                        //reward 

                        PopupOpenRewardProps partialReward = new PopupOpenRewardProps();
                        RewardCardBuilder card = new RewardCardBuilder();
                        card.heroId = 107;
                        card.frame = 1;
                        card.requireCopy = 3;
                        card.realIronCopy = 5;
                        card.realRewardCount = 3;
                        List<RewardCardBuilder> listRewardCardBuilder = new List<RewardCardBuilder>();
                        listRewardCardBuilder.Add(card);
                        RewardBalanceBuilder balance = new RewardBalanceBuilder();
                        balance.curGold = 500;
                        balance.lvGold = 500;
                        balance.curEssence = 50;
                        balance.lvEssence = 50;
                        balance.curExp = 100;
                        balance.lvExp = 100;

                        partialReward.rewardBalanceBuilder = balance;
                        partialReward.listRewardCardBuilder = listRewardCardBuilder;
                        partialReward.onClose = () =>
                        {
                            //index 1
                            DisplayState(index);
                            //goi sv lay ten mac dinh
                        };
                        partialReward.isShowListReward = true;
                        partialReward.isShowButtonUpgrade = false;
                        PopupOpenReward.Show(partialReward);

                        HomeChestProps hcEmpty = new HomeChestProps();
                        hcEmpty.state = HomeChestProps.ChestState.EMPTY;
                        hcEmpty.status = HomeChestProps.ChestStatus.CHEST_EMPTY;
                        hcEmpty.id = 0;
                        GameData.main.listHomeChestProps.Clear();
                        GameData.main.listHomeChestProps.Add(hcEmpty);
                        GameData.main.listHomeChestProps.Add(hcEmpty);
                        GameData.main.listHomeChestProps.Add(hcEmpty);
                        GameData.main.listHomeChestProps.Add(hcEmpty);
                        HomeSceneNew.instance.UpdateHomeChestTut();
                        HandleNetData.QueueNetData(NetData.RECEIVE_LOCAL_BALANCE, new List<long>() { 500, 0, 100, 50, 0 });


                    }
                    else if (GameData.main.userProgressionState == 3)
                    {
                        // g?i service lên 
                    }
                    break;
                }
            case 2:
                {
                    //Game.main.LoadScene("CollectionScene", onLoadSceneSuccess: () =>
                    //{
                    //    CollectionScene.instance.InitDataContent(1);
                    //    progressionStates[index].SetActive(true);
                    //    DisplayState(index);
                    //}, delay: 0.3f, curtain: true);
                    break;
                }
            case 3:
                {
                    // click upgrade card lan 1 
                    Game.main.socket.UpdateProgression(4);
                    GameData.main.userProgressionState = 5;
                    canSkip = false;
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
                    progressionStates[index].transform.GetChild(0).GetComponent<PopupUpgrade>().DoUpgradeOnTut(props);
                    HandleNetData.QueueNetData(NetData.RECEIVE_LOCAL_BALANCE, new List<long>() { 300, 0, 100, 0, 5 });

                    break;

                }
            case 4:
                {
                    // hien thi nut play
                    Game.main.LoadScene("HomeSceneNew",
                       () =>
                       {
                           NextState();
                       }, delay: 0.2f, curtain: true);

                    break;
                }
            case 5:
                {
                    if (GameData.main.userProgressionState == 5)
                    {
                        Game.main.LoadScene("BattleSceneTutorial", () =>
                        {
                            BattleSceneTutorial.instance.SetTutorial(1);
                        }, delay: 0.3f, curtain: true);

                        progressionStates[index].SetActive(false);
                    }

                    break;
                }
            case 6:
                {
                    if (GameData.main.userProgressionState == 6)
                    {
                        Game.main.socket.UpdateProgression(6);
                        GameData.main.userProgressionState = 7;
                        progressionStates[6].SetActive(false);
                        HomeChestProps hc = new HomeChestProps();
                        hc.state = HomeChestProps.ChestState.ACTIVATED;
                        hc.status = HomeChestProps.ChestStatus.CHEST_OPENING;
                        hc.timeChest = HomeChestProps.TimeChest.TIME_8H;
                        hc.remainTime = 5000;
                        hc.isNew = false;
                        hc.canOpen = true;
                        hc.id = -1;
                        hc.turnOnPointClick = true;
                        HomeChestProps hcEmpty = new HomeChestProps();
                        hcEmpty.state = HomeChestProps.ChestState.EMPTY;
                        hcEmpty.status = HomeChestProps.ChestStatus.CHEST_EMPTY;
                        hcEmpty.id = 0;
                        GameData.main.listHomeChestProps.Clear();
                        GameData.main.listHomeChestProps.Add(hc);
                        GameData.main.listHomeChestProps.Add(hcEmpty);
                        GameData.main.listHomeChestProps.Add(hcEmpty);
                        GameData.main.listHomeChestProps.Add(hcEmpty);
                        HomeSceneNew.instance.UpdateHomeChestTut();

                    }
                    else if (GameData.main.userProgressionState == 7)
                    {

                        //click open reward chest  2
                        Game.main.socket.UpdateProgression(7);
                        GameData.main.userProgressionState = 8;
                        progressionStates[index].SetActive(false);
                        PopupOpenRewardProps partialReward = new PopupOpenRewardProps();
                        RewardCardBuilder card = new RewardCardBuilder();
                        card.heroId = 107;
                        card.frame = 2;
                        card.requireCopy = 7;
                        card.realIronCopy = 10;
                        card.realRewardCount = 8;
                        RewardCardBuilder card2 = new RewardCardBuilder();
                        card2.heroId = 22;
                        card2.frame = 1;
                        card2.requireCopy = 3;
                        card2.realIronCopy = 4;
                        card2.realRewardCount = 1;
                        List<RewardCardBuilder> listRewardCardBuilder = new List<RewardCardBuilder>();
                        listRewardCardBuilder.Add(card);
                        listRewardCardBuilder.Add(card2);
                        RewardBalanceBuilder balance = new RewardBalanceBuilder();
                        balance.curGold = 2300;
                        balance.lvGold = 2000;
                        balance.curEssence = 400;
                        balance.lvEssence = 400;
                        balance.curExp = 300;
                        balance.lvExp = 200;

                        partialReward.rewardBalanceBuilder = balance;
                        partialReward.listRewardCardBuilder = listRewardCardBuilder;
                        partialReward.onClose = () =>
                        {
                            Game.main.LoadScene("CollectionScene", onLoadSceneSuccess: () =>
                            {
                                CollectionScene.instance.InitDataContent(1);
                                m_BlackHole.SetActive(true);
                            }, delay: 0.3f, curtain: true);
                        };
                        partialReward.isShowListReward = true;
                        partialReward.isShowButtonUpgrade = false;
                        PopupOpenReward.Show(partialReward);

                        HomeChestProps hcEmpty = new HomeChestProps();
                        hcEmpty.state = HomeChestProps.ChestState.EMPTY;
                        hcEmpty.status = HomeChestProps.ChestStatus.CHEST_EMPTY;
                        hcEmpty.id = 0;
                        GameData.main.listHomeChestProps.Clear();
                        GameData.main.listHomeChestProps.Add(hcEmpty);
                        GameData.main.listHomeChestProps.Add(hcEmpty);
                        GameData.main.listHomeChestProps.Add(hcEmpty);
                        GameData.main.listHomeChestProps.Add(hcEmpty);
                        HomeSceneNew.instance.UpdateHomeChestTut();
                        HandleNetData.QueueNetData(NetData.RECEIVE_LOCAL_BALANCE, new List<long>() { 2300, 0, 300, 400, 5 });
                    }
                    break;
                }
            case 7:
                {
                    Game.main.socket.UpdateProgression(8);
                    GameData.main.userProgressionState = 9;
                    canSkip = false;
                    PopupUpgradeProps props = new PopupUpgradeProps();
                    props.status = 1;
                    props.heroId = 107;
                    props.type = 0;
                    props.frameId = 3;
                    props.curAtk = 2;
                    props.curHp = 2;
                    props.curMana = 1;
                    props.gold = 1800;
                    props.goldRequired = 800;
                    props.essence = 300;
                    props.essenceRequired = 250;
                    props.copyAvailable = 3;
                    props.copyRequired = 10;
                    props.canUpgrade = false;
                    props.nextAtk = 3;
                    props.nextHp = 2;
                    props.nextMana = 1;
                    props.shardBonus = 20;
                    props.shardCurrent = 15;
                    props.shardAmount = 60;
                    progressionStates[index].transform.GetChild(0).GetComponent<PopupUpgrade>().DoUpgradeOnTut(props);
                    HandleNetData.QueueNetData(NetData.RECEIVE_LOCAL_BALANCE, new List<long>() { 1800, 0, 300, 300, 15 });
                    break;
                }
            case 8:
                {
                    Game.main.LoadScene("HomeSceneNew",
                       () =>
                       {
                           NextState();
                       }, delay: 0.2f, curtain: true);
                    break;
                }
            case 9:
                {
                    //if (!PlayerPrefs.HasKey("TRACK_END_PROGRESSION"))
                    //{
                    //    PlayerPrefs.SetInt("TRACK_END_PROGRESSION", 1);
                    //    ITrackingParameter pr = new ITrackingParameter() { name = "tutorial_2_progress_complete", value = "true" };
                    //    ITracking.LogEventFirebase(ITracking.TRACK_END_PROGRESSION, pr);
                    //}
                    if (GameData.main.userProgressionState == 9)
                    {
                        progressionStates[index].SetActive(false);
                        m_WaitingPanel.SetActive(true);
                        Game.main.LoadScene("SelectDeckScene", delay: 6f, curtain: true,onLoadSceneSuccess: () => { m_WaitingPanel.SetActive(false); });
                    }
                    else if (GameData.main.userProgressionState == 10)
                    {
                        Game.main.LoadScene("HomeSceneNew", () =>
                        {
                           NextState();
                           Game.main.socket.GetUserCurrency();
                        }, delay: 0.3f, curtain: true);

                    }

                    break;
                }
            case 10:
                {

                    break;
                }
            case 11:
                {
                    if (GameData.main.userProgressionState == 11)
                    {
                        progressionStates[index].SetActive(false);
                        Game.main.LoadScene("SelectDeckScene", delay: 0.3f, curtain: true);
                    }
                    else if (GameData.main.userProgressionState == 12)
                    {
                        Game.main.LoadScene("HomeSceneNew", () =>
                        {
                            Game.main.socket.GetUserCurrency();
                            NextState();
                        }, delay: 0.3f, curtain: true);

                    }
                    break;
                }
            case 13:
                {
                    Destroy(this.gameObject);
                    break;
                }

        }
    }
    public void GetDefaultNickname()
    {
        Game.main.socket.GetDefaultScreenName();
    }
    public void OnClickConfirmNickname()
    {
        // l?u thanh cong thi nang state => 3 (cur = 4) . index len 2
        //next state

        if(string.IsNullOrEmpty(m_Name))
            m_Name = m_Nickname.placeholder.GetComponent<TextMeshProUGUI>().text;
        Game.main.socket.SetScreenName(m_Name);
        
    }
    void OnInputFieldValueChanged(string inputText)
    {
        m_Name = inputText;
    }
    IEnumerator Wait(float time, ICallback.CallFunc func)
    {
        yield return new WaitForSeconds(time);
        if (func != null)
        {
            func?.Invoke();
        }
    }
    public override bool ProcessSocketData(int serviceId, byte[] data)
    {
        if (base.ProcessSocketData(serviceId, data)) return true;
        switch (serviceId)
        {
            case IService.GET_RANDOM_SCREEN_NAME:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    LogWriterHandle.WriteLog("GET_RANDOM_SCREEN_NAME==" +string.Join(",", cv.aString));
                    if (cv.aString != null)
                        m_Nickname.placeholder.GetComponent<TextMeshProUGUI>().text  = cv.aString[0];

                    break;
                }
            case IService.SET_SCREEN_NAME:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    LogWriterHandle.WriteLog("SET_SCREEN_NAME==" + string.Join(",", cv.aString));
                    if (cv.aLong[0] == 0)
                        Toast.Show(cv.aString[0]);
                    else
                    {
                        Toast.Show("Success! Welcome, " + cv.aString[0] + "!");
                        //Game.main.socket.UpdateProgression(3, cv.aString[0]);
                        GameData.main.userProgressionState = 4;
                        GameData.main.profile.SetData(GameData.main.profile.userID, GameData.main.profile.username, cv.aString[0]);
                        progressionStates[index].SetActive(false);
                        Game.main.LoadScene("CollectionScene", () =>
                        {
                            // init data cho updrade c?a collection
                            CollectionScene.instance.InitDataContent(1);
                            NextState();
                        }, delay: 0.5f, curtain: true);
                    }    
                        
                    break;
                    
                }
        }
        return false;
    }
}
