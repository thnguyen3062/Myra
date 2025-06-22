using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GIKCore;
using GIKCore.Net;
using GIKCore.Lang;
using GIKCore.DB;
using GIKCore.Utilities;
using GIKCore.UI;
using GIKCore.Sound;
using TMPro;
using pbdson;
using UnityEngine.UI;
using System.Linq;
using System;
using UIEngine.UIPool;
using DG.Tweening;

public class SelectDeckRankScene : GameListener
{
    // Fields
    [SerializeField] private GameObject settingUI;
    [SerializeField] private GameObject popupEditPrefab, rankInfoPrefab;
    [SerializeField] private GameObject playerrankRankInfo;
    [SerializeField] private GameObject rankInfoPopupUI;
    [SerializeField] private Transform lstRankInfoContent;
    [SerializeField] private TextMeshProUGUI timeDuration, rankSeasonTxt;

    [SerializeField] private GridPoolGroup m_Grid;
    [SerializeField] private CellCollectionDeck m_DetailCellCollectionDeck;
    [SerializeField] private GameObject m_PopupSelectDeck;
    [SerializeField] private RectTransform m_SelectDeckDetail;
    [SerializeField] private Image m_CDNImg;
    [SerializeField] GameObject m_TransitionPopup;
    [SerializeField] RankInfo m_LastRank, m_CurrentRank;
    [SerializeField] private CustomDeckScene m_CustomDeckPopup;
    [SerializeField] private GameObject m_BtnEditDeck;

    // Values
    List<long> lstTrooper = new List<long>();
    List<long> lstGod = new List<long>();
    private long currentDeck = -1;
    private GameObject popupEdit;
    private long timeRankCountdown = 0;
    string countDown = "";
    // Methods
    private void StartGame()
    {
        if(GameData.main.lstDeckInfo.Count > 0)
        {
            GameData.main.lstDeckInfo[0].isSelected = true;
            m_DetailCellCollectionDeck.SetData(GameData.main.lstDeckInfo[0]);
        }
        m_Grid.SetAdapter(GameData.main.lstDeckInfo);
        SetBattleDeck(0);
    }
    public void DoClickSelectMode()
    {
        Game.main.socket.GetMode();
        Game.main.LoadScene("SelectModeScene", delay: 0.3f, curtain: true);
    }
    public void DoClickCollection()
    {
        Game.main.socket.GetUserDeck();
        Game.main.LoadScene("CollectionScene", delay: 0.3f, curtain: true);
    }
    public void DoClickQuest()
    {
        PopupFirstTime.Show("rank");
        //PopupConfirm.Show(content: "  - Players battle to earn points to fill in their progress bar." + "\n" +
        //    "  - You get points for each win, and lose points for each loss in ranked matches." + "\n" +
        //    "  - When your process bar is full, you will be promoted to a higher rank. You will be demoted when the process bar is empty." + "\n" +
        //    "  - Every rank season has a duration. When this duration is over, your rank progress will be reset." + "\n" +
        //    "  - Your rank progress might be reduced if you have not played any ranked games for a while." + "\n" +
        //    "  - There might be rules to ban/boost specific cards applied to each season.",
        //    align: TextAlignmentOptions.Left);
    }
    public void DoClickRankInfo()
    {//lay du lieu va hien list rank
        if (lstRankInfoContent.childCount == 18)
        {
            rankInfoPopupUI.SetActive(true);
        }
        else
        {
            int index = 0;
            foreach (Transform tran in lstRankInfoContent)
                Destroy(tran.gameObject);
            List<DBRank> lstRank = new List<DBRank>();
            lstRank = Database.GetListRank();
            if(lstRank.Count > 0&& lstRank[0].id== 1)
                lstRank.Reverse();
            for (int i =0;i< lstRank.Count; i++)
            {
                DBRank db = lstRank[i];
                GameObject go = Instantiate(rankInfoPrefab, lstRankInfoContent);
                go.GetComponent<RankInfo>().InitData(db.id);
                if (db.id == GameData.main.userRankInfo.curUserRank)
                {
                    index = i;
                }
            }
            rankInfoPopupUI.SetActive(true);
            lstRankInfoContent.GetComponent<swipe>().SetPos(index);
        }

    }
    public void DoClickEdit(GameObject go)
    {
        CustomDeckPrefabControl infor = go.GetComponent<CustomDeckPrefabControl>();
        string name = infor.GetDeckName();
        HeroCard hc = infor.GetPreviewDeckCard();
        if (popupEdit.GetComponent<EditFrameControl>() != null)
            popupEdit.GetComponent<EditFrameControl>().InitData(name, hc, infor.GetDeckID());
        popupEdit.SetActive(true);
    }
    public void DoClickPlay()
    {
        //DoSetBattleDeckAuto();
        //test
        if (currentDeck != -1)
        {
            GameData.main.currentPlayMode = "rank";
            for (int i = 0; i < GameData.main.lstDeckInfo.Count; i++)
            {
                if (currentDeck == GameData.main.lstDeckInfo[i].deckID)
                {
                    GameData.main.hadCustomDeck = !GameData.main.lstDeckInfo[i].isDefaultDeck;
                    break;
                }
            }
            Game.main.socket.SetUserBattleDeck(currentDeck, GameData.main.currentPlayMode);
        }
        else
        {
            Toast.Show(LangHandler.Get("68", "Please choose one deck to play!"));
        }
    }
    public void DoClickBack()
    {
        Game.main.socket.GetMode();
        Game.main.LoadScene("SelectModeScene", delay: 0.3f, curtain: true);
    }
    public void DoClickSetting()
    {
        settingUI.SetActive(true);
    }
    public void DoClickDisableButton()
    {
        Toast.Show(LangHandler.Get("toast-8", "Coming Soon!"));
    }
    public void DoSetBattleDeckAuto()
    {
        if (lstGod.Count > 0 && lstTrooper.Count > 0)
        {
            // GameData.main.idGodFindMatch = type;/*lstGod[IUtil.RandomInt(0, lstGod.Count - 1)];*/
            Game.main.socket.SetUserBattleDeck(lstGod, lstTrooper);
        }
        else Toast.Show(LangHandler.Get("68", "Please choose one deck to play!"));
    }
    public void DoClickHidePopupSelectDeck()
    {
        m_SelectDeckDetail.DOAnchorPosX(0f, 0.3f).OnComplete(() =>
        {
            m_PopupSelectDeck.SetActive(false);
        });
    }
    public void SetBattleDeck(int type)
    {
        currentDeck = GameData.main.lstDeckInfo[type].deckID;

        PlayerPrefs.SetInt("CurrentDeck", type);
        m_PopupSelectDeck.SetActive(true);
        DeckInfo deck = GameData.main.lstDeckInfo.Find(x => x.deckID == currentDeck);
        m_BtnEditDeck.SetActive(!deck.isDefaultDeck);
        //m_SelectDeckDetail.DOAnchorPosX(-706f, 0.3f);
    }
    public void DoClickEditDeck()
    {
        m_CustomDeckPopup.gameObject.SetActive(true);
        Game.main.socket.GetUserDeckDetail(currentDeck);
    }

    public override bool ProcessSocketData(int serviceId, byte[] data)
    {
        if (base.ProcessSocketData(serviceId, data)) return true;
        switch (serviceId)
        {
            case IService.SET_USER_BATTLE_DECK:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    LogWriterHandle.WriteLog("SET_USER_BATTLE_DECK==" + cv.aLong[0]);
                    if (cv.aLong[0] == 1)
                        Game.main.LoadScene("FindMatchScene", delay: 0.3f, curtain: true);
                    break;
                }

            case IService.GET_USER_DECK:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    GameData.main.lstDeckInfo = new List<DeckInfo>();
                    int START_LONG = 1;
                    int COUNT_LONG = cv.aString.Count;
                    int BLOCK = 0;
                    for (int i = 0; i < COUNT_LONG; i++)
                    {
                        DeckInfo info = new DeckInfo()
                        {
                            isLastDeck = cv.aLong[0] == cv.aLong[START_LONG + BLOCK + 0],
                            deckID = cv.aLong[START_LONG + BLOCK + 0],
                            deckStatus = cv.aLong[START_LONG + BLOCK + 1],
                            isDefaultDeck = cv.aLong[START_LONG + BLOCK + 2] == 1,
                            deckName = cv.aString[i]
                        };
                        int numberGod = (int)cv.aLong[START_LONG + 3 + BLOCK];
                        if (numberGod > 0)
                        {
                            info.godId = cv.aLong[START_LONG + BLOCK + 4];
                        }
                        else
                        {
                            info.godId = -1;
                        }
                        for (int j = 0; j < 3; j++)
                        {
                            if (j < numberGod)
                                info.lstGodIds.SetValue(cv.aLong[START_LONG + 4 + BLOCK + j], j);
                            else
                                info.lstGodIds.SetValue(-1, j);
                        }
                        BLOCK = BLOCK + 4 + numberGod;


                        GameData.main.lstDeckInfo.Add(info);
                    }
                    m_Grid.SetAdapter(GameData.main.lstDeckInfo);
                    if (GameData.main.selectedDeck != 0)
                    {
                        int type = GameData.main.lstDeckInfo.FindIndex(x => x.deckID == GameData.main.selectedDeck);
                        DeckInfo deckInf = GameData.main.lstDeckInfo.Find(x => x.deckID == GameData.main.selectedDeck);
                        if (deckInf != null)
                        {
                            m_DetailCellCollectionDeck.SetData(deckInf);
                            SetBattleDeck(type);
                            GameData.main.selectedDeck = 0;
                        }
                        else
                        {
                            Toast.Show(LangHandler.Get("159","No Deck Found"));
                        }
                    } else
                    {
                        m_DetailCellCollectionDeck.SetData(GameData.main.lstDeckInfo[0]);
                    }
                    StartGame();
                    break;
                }
            case IService.GET_RANK:
                {
                    ListCommonVector lst = ISocket.Parse<ListCommonVector>(data);
                    CommonVector cv1 = lst.aVector[0];
                    LogWriterHandle.WriteLog("GET_RANK: " + string.Join(",", cv1.aLong));
                    LogWriterHandle.WriteLog("GET_RANK: " + string.Join(",", cv1.aString));
                    RankModel rankInfo = new RankModel();
                    if (cv1.aLong.Count > 0)
                    {
                        if (cv1.aLong[0] == 0)
                        {
                            Toast.Show(cv1.aString[0]);
                        }
                        else
                        {
                            if (cv1.aLong.Count >= 3)
                            {

                                rankInfo.isFirstTime = cv1.aLong[1] == 1;
                                rankInfo.lastElo = (cv1.aLong[2] ==1)? true : false;
                                if(rankInfo.lastElo)
                                {
                                    rankInfo.lastRank = cv1.aLong[3];
                                }
                                if(cv1.aLong.Count >= 4)
                                    rankInfo.curUserRank = cv1.aLong[4];

                                if (cv1.aLong[1] == 1)
                                    rankInfo.seasonImage = cv1.aString[1];
                            }
                            CommonVector cv2 = lst.aVector[1];
                            LogWriterHandle.WriteLog("GET_RANK: " + string.Join(",", cv2.aLong));

                            if (cv2.aLong.Count >= 5)
                            {
                                rankInfo.rankSeasonID = cv2.aLong[0];
                                rankInfo.rankSeasonName = cv2.aLong[1];
                                rankInfo.timeRemain = cv2.aLong[2];
                                rankInfo.curUserElo = cv2.aLong[3];
                                rankInfo.curUserRank = cv2.aLong[4];

                            }
                            GameData.main.userRankInfo = rankInfo;
                            if (GameData.main.userRankInfo.rankSeasonID != 0)
                            {
                                playerrankRankInfo.GetComponent<RankInfo>().InitData(GameData.main.userRankInfo.curUserRank, GameData.main.userRankInfo.curUserElo);
                                rankSeasonTxt.text = LangHandler.Get("142","SEASON") +" " + GameData.main.userRankInfo.rankSeasonName.ToString();
                                timeRankCountdown = GameData.main.userRankInfo.timeRemain;
                                //TimeSpan time = TimeSpan.FromMilliseconds(timeRankCountdown);
                                DisplayTimeCountDownRank(timeRankCountdown, timeDuration);
                            }
                            if (GameData.main.userRankInfo.isFirstTime)
                            {
                                PopupFirstTime.Show("rank");
                                if (!string.IsNullOrEmpty(GameData.main.userRankInfo.seasonImage))
                                {
                                    m_CDNImg.gameObject.SetActive(true);
                                    LoadHttpRankImg(GameData.main.userRankInfo.seasonImage);
                                    //m_CDNImg.gameObject.GetComponent<Button>().onClick.AddListener(delegate
                                    //{
                                    //    this.gameObject.SetActive(false);
                                    //});
                                }
                                if (GameData.main.userRankInfo.lastElo)
                                {
                                    DBRank lastRank = Database.GetRank(GameData.main.userRankInfo.lastRank);
                                    DBRank currentRank = Database.GetRank(GameData.main.userRankInfo.curUserRank);
                                    if (lastRank != null && currentRank != null)
                                    {
                                        m_TransitionPopup.SetActive(false);
                                        m_LastRank.InitData(lastRank.id);

                                        m_CurrentRank.InitData(currentRank.id);
                                        m_CurrentRank.gameObject.SetActive(false);
                                        m_LastRank.gameObject.GetComponent<Button>().onClick.AddListener(delegate
                                        {
                                            m_LastRank.gameObject.GetComponent<CanvasGroup>().DOFade(0, 1f).onComplete += delegate
                                            {
                                                m_CurrentRank.gameObject.SetActive(true);
                                                m_CurrentRank.gameObject.GetComponent<CanvasGroup>().DOFade(1, 1f).onComplete += delegate
                                                {
                                                    m_CurrentRank.gameObject.GetComponent<Button>().interactable = true;
                                                };
                                            };
                                        });
                                        m_CurrentRank.gameObject.GetComponent<Button>().onClick.AddListener(delegate
                                        {
                                            m_TransitionPopup.SetActive(false);
                                        });
                                    }
                                }

                            }
                        } 
                    }    
                    
                    break;
                }
            case IService.GET_USER_DECK_DETAIL:
                ListCommonVector listCommonVector = ISocket.Parse<ListCommonVector>(data);
                break;
        }
        return false;
    }
    IEnumerator coroutine = null;
    public void LoadHttpRankImg(string url, ICallback.CallFunc2<Sprite> onLoaded = null)
    {
        if (coroutine != null)
            Game.main.StopCoroutine(coroutine);

        if (string.IsNullOrEmpty(url)) return;
        coroutine = IUtil.LoadTexture2DFromUrl(url, (Texture2D tex) =>
        {
            if (tex == null) return;
            Sprite sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            m_CDNImg.GetComponent<Image>().sprite = sprite;

            if (onLoaded != null) onLoaded(sprite);
        });
        Game.main.StartCoroutine(coroutine);
    }
    protected override void Awake()
    {
        base.Awake();
        m_Grid.SetCellDataCallback((GameObject go, DeckInfo data, int index) =>
        {
            CellCollectionDeck script = go.GetComponent<CellCollectionDeck>();
            script.SetData(data).SetOnClickCallback((script) =>
            {
                int type = GameData.main.lstDeckInfo.FindIndex(x => x.deckID == data.deckID);
                if (type != -1)
                {
                    m_DetailCellCollectionDeck.SetData(data);
                    SetBattleDeck(type);
                }
                else
                {
                    Toast.Show(LangHandler.Get("159","No Deck Found"));
                }
                for (int i = 0; i < GameData.main.lstDeckInfo.Count; i++)
                {
                    GameData.main.lstDeckInfo[i].isSelected = GameData.main.lstDeckInfo[i].deckID == data.deckID;
                }
                m_Grid.SetAdapter(GameData.main.lstDeckInfo, false);
            });
        });
    }
    private void DisplayTimeCountDownRank(long timeRemain, TextMeshProUGUI timeTxt)
    {
        
        TimeSpan time = TimeSpan.FromMilliseconds(timeRemain);
            if (time.Days == 0)
                countDown = string.Format("{0} Hours, {1} Minutes, {2} Seconds", time.Hours, time.Minutes, time.Seconds);
            else
                countDown = string.Format("{0} Days, {1} Hours, {2} Minutes, {3} Seconds", time.Days, time.Hours, time.Minutes, time.Seconds);
            timeTxt.text = countDown;
    }
    private void Update()
    {
        if(timeRankCountdown>0)
        {
            timeRankCountdown -= (long)(Time.deltaTime * 1000);
            //Debug.Log(timeRankCountdown + "?" + Time.deltaTime + "/" + Time.timeScale);
            DisplayTimeCountDownRank(timeRankCountdown, timeDuration);
            if (timeRankCountdown <= 0)
            {
                Game.main.socket.GetMode();
                Game.main.LoadScene("SelectModeScene", delay: 0.3f, curtain: true);
            }
        }    
    }
}
