using GIKCore;
using GIKCore.Lang;
using GIKCore.Net;
using GIKCore.Sound;
using GIKCore.UI;
using GIKCore.Utilities;
using pbdson;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModeSelection : GameListener
{
    public static ModeSelection instance;

    [SerializeField] private GameObject settingUI;
    [SerializeField] private SummaryEvent summaryEvent;
    [SerializeField] private TextMeshProUGUI startTimeTxt, endTimeTxt,myraTxt;
    [SerializeField] private PanelRankLeaderboard m_PanelRankLeaderboard;
    [SerializeField] private RankRewardsPopup m_RankRewardUI;
    [SerializeField] private Button m_RankButton;

    public event ICallback.CallFunc onReceiveSuccess; 

    private DateTime startTime, endTime;
    private long timeRankCountdown = 0;
    private TextMeshProUGUI countDownTxt = null;
    private bool displayRankTimeRemain = false;
    private List<RewardRank> lstRankReward = new List<RewardRank>();
    private void Start()
    {
    }
    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }
    public override bool ProcessSocketData(int id, byte[] data)
    {
        if (base.ProcessSocketData(id, data))
            return true;

        switch (id)
        {
            case IService.GET_EVENT:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    LogWriterHandle.WriteLog(string.Join(",", cv.aLong));
                    if (cv.aLong[0] == 0)
                    {
                        Toast.Show(cv.aString[0]);
                    }
                    else if (cv.aLong[0] == 1)
                    {
                        bool canJoin = (cv.aLong[1] == 1) ? true : false;
                        if (!canJoin)
                        {
                            Toast.Show(cv.aString[0]);
                        }
                        else
                        {

                            GoToEvent();
                        }
                    }
                    else
                    {
                        CommonVector newCv = new CommonVector();
                        newCv.aLong.Add(1);
                        Game.main.socket.GetUserEventInfo(newCv);
                    }
                    break;
                }
            case IService.GET_USER_EVENT_INFO:
                {
                    CommonVector cv1 = ISocket.Parse<CommonVector>(data);
                    LogWriterHandle.WriteLog("GET_EVENT_INFO: " + string.Join(",", cv1.aLong));

                    if (cv1.aLong.Count > 0)
                    {
                        if (cv1.aLong[0] != -1)
                        {
                            GameData.main.eventTotalMatch = cv1.aLong[0];
                            GameData.main.eventTotalWin = cv1.aLong[1];
                            GameData.main.myraEvent = cv1.aLong[4];
                            long start = cv1.aLong[2];
                            long end = cv1.aLong[3];
                            startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(start).ToLocalTime();
                            endTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(end).ToLocalTime();
                            LogWriterHandle.WriteLog("Start Time: " + startTime.ToString("MM-dd-yyyy") + " |End Time: " + endTime.ToString("MM-dd-yyyy"));
                        }
                        else
                        {
                            GameData.main.eventTotalMatch = 0;
                            GameData.main.eventTotalWin = 0;
                        }
                        OnShowSummary(GameData.main.eventTotalMatch, GameData.main.eventTotalWin, GameData.main.myraEvent);
                    }
                    else
                    {
                        summaryEvent.gameObject.SetActive(false);
                    }
                    break;
                }
            case IService.GET_RANK:
                {
                    //ListCommonVector lst = ISocket.Parse<ListCommonVector>(data);
                    //CommonVector cv1 = lst.aVector[0];
                    //LogWriterHandle.WriteLog("GET_RANK: " + string.Join(",", cv1.aLong));
                    //RankModel rankInfo = new RankModel();
                    //if (cv1.aLong.Count > 0)
                    //{
                    //    if (cv1.aLong[0] == 0)
                    //    {
                    //        Toast.Show(cv1.aString[0]);
                    //    }
                    //    else
                    //    {
                    //        if (cv1.aLong.Count >= 5)
                    //        {

                    //            rankInfo.isFirstTime = (cv1.aLong[1] == 1) ? true : false;
                    //            rankInfo.lastElo = (cv1.aLong[2] == 1) ? true : false;
                    //            rankInfo.lastRank = cv1.aLong[3];
                    //            rankInfo.curUserRank = cv1.aLong[4];
                    //            if (cv1.aLong[1] == 1)
                    //                rankInfo.seasonImage = cv1.aString[1];
                    //        }
                    //    }
                    //}

                    //CommonVector cv2 = lst.aVector[1];
                    //LogWriterHandle.WriteLog("GET_RANK: " + string.Join(",", cv2.aLong));

                    //if (cv1.aLong.Count >= 4)
                    //{
                    //    rankInfo.rankSeasonID = cv2.aLong[0];
                    //    rankInfo.rankSeasonName = cv2.aLong[1];
                    //    rankInfo.timeRemain = cv2.aLong[2];
                    //    rankInfo.curUserElo = cv2.aLong[3];

                    //}
                    //GameData.main.userRankInfo = rankInfo;
                    //GoToRank();
                    break;
                }
            case IService.GET_MODE:
                {
                    CommonVector cv1 = ISocket.Parse<CommonVector>(data);
                    LogWriterHandle.WriteLog("GET_MODE" + string.Join(",", cv1.aLong));
                    if (cv1.aLong.Count > 0)
                    {
                        if (cv1.aLong[0] == 0)
                        {
                            //khong co mua rank
                            SetRankButton(cv1.aLong[0]);


                        }
                        else if (cv1.aLong[0] == 1)
                        {
                            // rank s?p bat dau
                            SetRankButton(cv1.aLong[0], cv1.aLong[1]);
                        }
                        else
                        {
                            //rank s?p ket thuc
                            SetRankButton(cv1.aLong[0], cv1.aLong[1]);
                        }
                    }
                    break;
                }
            case IService.GET_LEADER_BOARD:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    int STR_START = 2;
                    int STR_BLOCK = 2;
                    int INT_START = 4;
                    int INT_BLOCK = 2;
                    int BLOCK_COUNT = (cv.aString.Count - STR_START) / STR_BLOCK;
                    CellRankProps curUser = new CellRankProps();
                    curUser.userName = cv.aString[0];
                    curUser.screenName = cv.aString[1];
                    curUser.rankIndex = (int)cv.aLong[1];
                    curUser.rankId = (int)cv.aLong[2];
                    curUser.eloPoint = (int)cv.aLong[3];

                    int timeUpdate = (int)cv.aLong[0];
                    List<CellRankProps> adapter = new List<CellRankProps>();
                    for (int i = 0; i < BLOCK_COUNT; i++)
                    {
                        CellRankProps prop = new CellRankProps();
                        prop.userName = cv.aString[STR_START + i * STR_BLOCK];
                        prop.screenName = cv.aString[STR_START + i * STR_BLOCK + 1];
                        prop.rankId = (int)cv.aLong[INT_START + i * INT_BLOCK];
                        prop.eloPoint = (int)cv.aLong[INT_START + i * INT_BLOCK + 1];
                        prop.rankIndex = i;
                        adapter.Add(prop);
                    }
                    m_PanelRankLeaderboard.SetData(adapter, curUser, timeUpdate); 
                    m_RankRewardUI.gameObject.SetActive(false);
                    break;
                }
            case IService.VIEW_REWARD:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    LogWriterHandle.WriteLog("VIEW_REWARD" + string.Join(",", cv.aLong));
                    LogWriterHandle.WriteLog("VIEW_REWARD" + string.Join(",", cv.aString));
                    if (cv.aLong[0] ==0)
                    {
                        Toast.Show(cv.aString[0]);
                    }    
                    else
                    {
                        lstRankReward.Clear();
                        int count = 0;
                        for (int i = 2; i < cv.aLong.Count; i += 9)
                        {
                            count++;
                            RewardRank model = new RewardRank();
                            model.rankSeasonID = cv.aLong[1];
                            model.rankID = cv.aLong[i];
                            model.gold = cv.aLong[i + 1];
                            if(cv.aLong[i + 2] != -1)
                            {
                                CardRewardRank card = new CardRewardRank();
                                card.heroID = cv.aLong[i + 2];
                                card.frame = cv.aLong[i + 3];
                                card.count =(int) cv.aLong[i + 4];
                                card.cardImg = cv.aString[(count-1)*2];
                                model.card = card;
                            }
                            if(cv.aLong[i + 5] != -1)
                            {
                                ItemRewardRank item = new ItemRewardRank();
                                item.itemID = cv.aLong[i + 5];
                                item.count =(int) cv.aLong[i + 6];
                                item.itemImg = cv.aString[(count - 1) * 2+1];
                                model.item = item;
                            }
                            model.isAchieved = cv.aLong[i + 7]  ==1 ? true : false;
                            model.isReceive = cv.aLong[i + 8] ==1 ? true : false; 
                            lstRankReward.Add(model);
                            
                        }
                        m_RankRewardUI.InitData(lstRankReward);
                        m_PanelRankLeaderboard.gameObject.SetActive(false);

                    } 
                    break;
                }
            case IService.GET_REWARD:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    if (cv.aLong[0] == 0)
                    {
                        Toast.Show(cv.aString[0]);
                    }
                    else
                    {
                        onReceiveSuccess?.Invoke();
                    }    
                    break;
                }
        }

        return false;
    }

    private void SetRankButton(long type, long timeRemain = -1)
    {
        TextMeshProUGUI timeTxt = m_RankButton.gameObject.transform.GetChild(m_RankButton.transform.childCount-2).GetComponent<TextMeshProUGUI>();
        switch (type)
        {
            case 0:
                {
                    //k co mua rank nao 
                    m_RankButton.interactable = false;
                    m_RankButton.onClick.AddListener(delegate { Toast.Show(" k co rank!"); });
                    break;
                }
            case 1:
                {
                    //k co mua rank nao 
                    m_RankButton.image.sprite = m_RankButton.spriteState.disabledSprite;
                    m_RankButton.onClick.AddListener(delegate { Toast.Show(" sap co rank!"); });
                    timeRankCountdown = timeRemain;
                    TimeSpan time = TimeSpan.FromMilliseconds(timeRemain);

                    if(time.Days<10)
                    {
                        displayRankTimeRemain = true;
                        countDownTxt= timeTxt ;
                        timeTxt.gameObject.SetActive(true);
                        DisplayTimeCountDownRank(timeRankCountdown, timeTxt);
                    }   
                    else
                    {
                        countDownTxt = null;
                        timeTxt.gameObject.SetActive(false);
                    }    
                    
                    break;
                }
            case 2:
                {
                    //rank s?p k?t thúc
                    timeRankCountdown = timeRemain;
                    TimeSpan time = TimeSpan.FromMilliseconds(timeRemain);
                    if (time.Days < 10)
                    {
                        displayRankTimeRemain = true;
                        countDownTxt = timeTxt;
                        timeTxt.gameObject.SetActive(true);
                        DisplayTimeCountDownRank(timeRankCountdown, timeTxt);
                        if (time.Days == 0 && time.Hours ==0 && time.Minutes ==0 &&time.Seconds<=3)
                        {
                            m_RankButton.image.sprite = m_RankButton.spriteState.disabledSprite;
                            m_RankButton.onClick.AddListener(delegate { Toast.Show(" sap kthuc rank!"); });
                        }  
                        else
                        {
                            m_RankButton.onClick.AddListener(delegate { GoToRank(); });
                        }    
                    }
                    else
                    {
                        countDownTxt = null;
                        timeTxt.gameObject.SetActive(false);
                        m_RankButton.onClick.AddListener(delegate { GoToRank(); });
                    }
                    break;
                }

        }
    }    
    private void DisplayTimeCountDownRank(long timeRemain , TextMeshProUGUI timeTxt)
    {
        string countDown = "";
        TimeSpan time = TimeSpan.FromMilliseconds(timeRemain);
        if (time.Days < 10)
        {
            if (time.Days == 0)
                countDown = string.Format("{0} Hours, {1} Minutes, {2} Seconds", time.Hours, time.Minutes, time.Seconds);
            else
                countDown = string.Format("{0} Days, {1} Hours, {2} Minutes, {3} Seconds", time.Days, time.Hours, time.Minutes, time.Seconds);
            timeTxt.text = countDown;
        }
    }    
    public void DoClickBack()
    {
        SoundHandler.main.PlaySFX("900_click_4", "sounds");
        Game.main.LoadScene("HomeSceneNew", delay: 0.3f, curtain: true);
    }
    public void DoClickSetting()
    {
        SoundHandler.main.PlaySFX("900_click_4", "sounds");
        settingUI.SetActive(true);
    }

    public void DoClickRewardPopup()
    {
        //Toast.Show(LangHandler.Get("toast-8", "Coming Soon!"));
        SoundHandler.main.PlaySFX("900_click_3", "sounds");
        Game.main.socket.ViewReward();
    }   
    public void DoClickLeaderBoard()
    {
        SoundHandler.main.PlaySFX("900_click_3", "sounds");
        Game.main.socket.GetLeaderboard();
    }
    public void DoClickDisableButton()
    {
        SoundHandler.main.PlaySFX("900_click_4", "sounds");
        Toast.Show(LangHandler.Get("toast-8", "Coming Soon!"));
    }
    public void DoClickNormalMode()
    {
        SoundHandler.main.PlaySFX("900_click_3", "sounds");
        Game.main.socket.GetUserDeck();
        Game.main.socket.GetRank();
        Game.main.LoadScene("SelectDeckScene", delay: 0.3f, curtain: true);
    }
    public void DoClickEvenMode()
    {
        // check condition
        //not met
        //met 
        //end Event
        Game.main.socket.GetEvent();
    }
    public void DoClickRankMode()
    {
        SoundHandler.main.PlaySFX("900_click_3", "sounds");
        Game.main.socket.GetRank();
    }

    public void GoToEvent()
    {
       
        CommonVector cv = new CommonVector();
        cv.aLong.Add(1);
        Game.main.socket.GetUserEventInfo(cv);
        Game.main.socket.GetUserDeck();
        Game.main.LoadScene("SelectDeckEventScene", delay: 0.3f, curtain: true);
    }
    public void GoToRank()
    {
        Game.main.socket.GetUserDeck();
        Game.main.socket.GetRank();
        Game.main.LoadScene("SelectDeckRankScene", delay: 0.3f, curtain: true);
    }

    public void OnShowSummary(long match, long win,long myra)
    {
        startTimeTxt.text = startTime.ToString("MM-dd-yyyy");
        endTimeTxt.text = endTime.ToString("MM-dd-yyyy");
        myraTxt.text = LangHandler.Get("139","TOTAL PRIZE POOL: Myra ") + myra;
        summaryEvent.gameObject.SetActive(true);
        summaryEvent.OnInitSummary(match, win);
    }
    private void Update()
    {
        if (timeRankCountdown >0 && countDownTxt != null && displayRankTimeRemain == true)
        {
            timeRankCountdown -= (long) (Time.deltaTime*1000);
            DisplayTimeCountDownRank(timeRankCountdown, countDownTxt);
            if(timeRankCountdown <= 0)
            {
                displayRankTimeRemain = false;
                countDownTxt.gameObject.SetActive(false);
                Game.main.socket.GetMode();
                Game.main.LoadScene("SelectModeScene", delay: 0.3f, curtain: true);
            }    

        }    
    }
}
