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
using UIEngine.UIPool;
using DG.Tweening;
using System;

public class SelectDeckScene : GameListener
{
    //[SerializeField] private GameObject[] decks, crystals, textDetail, backGround, infor;
    [SerializeField] private GameObject settingUI;
    [SerializeField] private GameObject popupEditPrefab;
    [SerializeField] private GridPoolGroup m_Grid;

    [SerializeField] private GameObject m_PopupSelectDeck;
    [SerializeField] private RectTransform m_SelectDeckDetail;
    [SerializeField] private GameObject[] m_ModeBackground;
    [SerializeField] private CellCollectionDeck m_DetailCellCollectionDeck;
    [SerializeField] private GameObject arrowFindMatch;
    [SerializeField] private GameObject arrowSelectDeck;
    [SerializeField] private GameObject m_BtnEditDeck;

    [SerializeField] private CustomDeckScene m_CustomDeckPopup;

    private List<DeckInfo> listDeckinfor = new List<DeckInfo>();
    List<long> lstTrooper = new List<long>();
    List<long> lstGod = new List<long>();
    private long currentDeck = -1;
    private GameObject popupEdit;
    public void DoClickEdit(GameObject go)
    {
        CustomDeckPrefabControl infor = go.GetComponent<CustomDeckPrefabControl>();
        string name = infor.GetDeckName();
        HeroCard hc = infor.GetPreviewDeckCard();
        if (popupEdit.GetComponent<EditFrameControl>() != null)
            popupEdit.GetComponent<EditFrameControl>().InitData(name, hc, infor.GetDeckID());
        popupEdit.SetActive(true);
    }

    public void DoClickSelectMode()
    {
        SoundHandler.main.PlaySFX("900_click_3", "sounds");
        if (GameData.main.userProgressionState < 14)
        {
            Toast.Show(LangHandler.Get("754", "This function will be available at level 4."));
            return;
        }
        if (!GameData.main.userHasReachedLevel6 )
            return;
        Game.main.socket.GetMode();
        Game.main.LoadScene("SelectModeScene", delay: 0.3f, curtain: true);
    }
    public void DoClickCollection()
    {
        Game.main.socket.GetUserDeck();
        Game.main.socket.GetUserPack();
        Game.main.LoadScene("CollectionScene", delay: 0.3f, curtain: true);
    }
    public void DoClickPlay()
    {
        //DoSetBattleDeckAuto();
        SoundHandler.main.PlaySFX("900_click_3", "sounds");
        if (currentDeck != -1)
        {
            GameData.main.currentPlayMode = "unrank";
            for (int i = 0; i < GameData.main.lstDeckInfo.Count; i++)
            {
                if (currentDeck == GameData.main.lstDeckInfo[i].deckID)
                {
                    GameData.main.hadCustomDeck = !GameData.main.lstDeckInfo[i].isDefaultDeck;
                    break;
                }
            }
            Debug.Log("currentDeck: " + currentDeck);
            Game.main.socket.SetUserBattleDeck(currentDeck, GameData.main.currentPlayMode);

        }
        else
        {
            Toast.Show(LangHandler.Get("68", "Please choose one deck to play!"));
        }
    }
    public void DoClickBack()
    {
        if (GameData.main.userProgressionState < 14)
        {
            Toast.Show(LangHandler.Get("754", "This function will be available at level 4."));
            return;
        }    
        if (!GameData.main.userHasReachedLevel6)
        {
            Game.main.LoadScene("HomeSceneNew", delay: 0.3f, curtain: true);
        }
        else
        {

            Game.main.socket.GetMode();
            Game.main.LoadScene("SelectModeScene", delay: 0.3f, curtain: true);
        }
    }
    public void DoClickSetting()
    {
        if (GameData.main.userProgressionState <14)
        {
            Toast.Show(LangHandler.Get("754", "This function will be available at level 4."));
            return;
        }
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
            Game.main.socket.SetUserBattleDeck(lstGod, lstTrooper);
        }
        else Toast.Show(LangHandler.Get("68", "Please choose one deck to play!"));
    }
    public void DoClickHidePopupSelectDeck()
    {
        m_SelectDeckDetail.DOAnchorPosX(0f, 0.3f).OnComplete(() =>
        {
            m_PopupSelectDeck.SetActive(false);
            CheckClickHideFindMatch();
        });
    }
    public void SetBattleDeck(int type)
    {
        currentDeck = GameData.main.lstDeckInfo[type].deckID;
        //lstGod = new List<long>();
        //lstTrooper = new List<long>();

        // Old select deck
        //foreach (GameObject deck in decks)
        //{
        //    RectTransform rt = deck.gameObject.GetComponent(typeof(RectTransform)) as RectTransform;
        //    rt.sizeDelta = new Vector2(870, 117);
        //    deck.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
        //}
        //foreach (GameObject crystal in crystals)
        //{
        //    crystal.SetActive(false);
        //}
        //foreach (GameObject text in textDetail)
        //{
        //    if(text!=null)
        //        text.SetActive(false);
        //}
        //foreach (GameObject bg in backGround)
        //{
        //    bg.SetActive(false);
        //}
        //foreach (GameObject in4 in infor)
        //{
        //    in4.SetActive(false);
        //}
        //decks[type].gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(984, 152);
        //decks[type].GetComponent<Image>().color = new Color(1, 1, 1);
        //crystals[type].SetActive(true);
        //backGround[type].SetActive(true);
        //if(textDetail[type]!= null)
        //textDetail[type].SetActive(true);
        //infor[type].SetActive(true);

        PlayerPrefs.SetInt("CurrentDeck", type);
        m_PopupSelectDeck.SetActive(true);
        m_SelectDeckDetail.DOAnchorPosX(-706f, 0.3f);
        DeckInfo deck = GameData.main.lstDeckInfo.Find(x => x.deckID == currentDeck);
        m_BtnEditDeck.SetActive(!deck.isDefaultDeck);

        //if (lstGod.Count > 0 && lstTrooper.Count > 0)
        //{
        //    GameData.main.idGodFindMatch = type;
        //}
    }
    
    public override bool ProcessSocketData(int serviceId, byte[] data)
    {
        if (base.ProcessSocketData(serviceId, data)) return true;
        switch (serviceId)
        {
            case IService.SET_USER_BATTLE_DECK:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    if (cv.aLong[0] == 1)
                        Game.main.LoadScene("FindMatchScene", delay: 0.3f, curtain: true);
                    break;
                }
            //case IService.GET_RANK:
            //    {
            //        ListCommonVector lst = ISocket.Parse<ListCommonVector>(data);
            //        CommonVector cv1 = lst.aVector[0];
            //        LogWriterHandle.WriteLog("GET_RANK: " + string.Join(",", cv1.aLong));
            //        LogWriterHandle.WriteLog("GET_RANK: " + string.Join(",", cv1.aString));
            //        RankModel rankInfo = new RankModel();
            //        if (cv1.aLong.Count > 0)
            //        {
            //            if (cv1.aLong[0] == 0)
            //            {
            //            }
            //            else
            //            {
            //                if (cv1.aLong.Count >= 3)
            //                {

            //                    rankInfo.isFirstTime = (cv1.aLong[1] == 1) ? true : false;
            //                    rankInfo.lastElo = (cv1.aLong[2] == 1) ? true : false;
            //                    if (rankInfo.lastElo)
            //                    {
            //                        rankInfo.lastRank = cv1.aLong[3];
            //                        rankInfo.curUserRank = cv1.aLong[4];
            //                    }
            //                    if (cv1.aLong[1] == 1)
            //                        rankInfo.seasonImage = cv1.aString[1];
            //                }
            //                CommonVector cv2 = lst.aVector[1];
            //                LogWriterHandle.WriteLog("GET_RANK: " + string.Join(",", cv2.aLong));

            //                if (cv2.aLong.Count >= 5)
            //                {
            //                    rankInfo.rankSeasonID = cv2.aLong[0];
            //                    rankInfo.rankSeasonName = cv2.aLong[1];
            //                    rankInfo.timeRemain = cv2.aLong[2];
            //                    rankInfo.curUserElo = cv2.aLong[3];
            //                    rankInfo.curUserRank = cv2.aLong[4];

            //                }
            //                GameData.main.userRankInfo = rankInfo;
            //                m_RankInfo.InitData(rankInfo.curUserRank, rankInfo.curUserElo);
            //            }
            //        }
                   
            //        break;
            //    }

            case IService.GET_USER_DECK_DETAIL:
                {
                    ListCommonVector listCommonVector = ISocket.Parse<ListCommonVector>(data);
                    break;
                }
            case IService.GET_USER_DECK:
                {
                    GameData.main.lstDeckInfo.Clear();
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                        LogWriterHandle.WriteLog("GET_USER_DECK: \nALong: " + string.Join(",", cv.aLong) + "\nAString: " + string.Join(",", cv.aString));
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
                    }
                    break;
                }
        }
        return false;
    }
    private void ProcessGetUserDeckDetail(ListCommonVector listCommonVector)
    {
        foreach (CommonVector cv in listCommonVector.aVector)
        {
            //Debug.Log(string.Join(",", cv.aString));
        }
    }
    protected override void Awake()
    {
        base.Awake();
        m_Grid.SetCellDataCallback((GameObject go, DeckInfo data, int index) =>
        {
            CellCollectionDeck script = go.GetComponent<CellCollectionDeck>();
            script.SetData(data, index == 0).SetOnClickCallback((script) =>
            {
                int type = GameData.main.lstDeckInfo.FindIndex(x => x.deckID == data.deckID);
                if (type != -1)
                {
                    m_DetailCellCollectionDeck.SetData(data);
                    SetBattleDeck(type);
                    CheckClickDeck();
                }
                else
                {
                    Toast.Show(LangHandler.Get("159", "No Deck Found"));
                }
                for(int i = 0; i < GameData.main.lstDeckInfo.Count; i++)
                {
                    GameData.main.lstDeckInfo[i].isSelected = GameData.main.lstDeckInfo[i].deckID == data.deckID;
                }
                m_Grid.SetAdapter(GameData.main.lstDeckInfo, false);
                //if(type != null)
                //Game.main.socket.GetUserDeckDetail(script.deckId);
            });
        });
    }
    private void Start()
    {
        m_Grid.SetAdapter(GameData.main.lstDeckInfo);
    }
    public void CheckClickDeck()
    {
        if (GameData.main.passFirst10Match)
            return;
        arrowFindMatch.SetActive(true);
    }
    public void CheckClickHideFindMatch()
    {
        if (GameData.main.passFirst10Match)
            return;
        arrowFindMatch.SetActive(false);
    }
    public void DoClickEditDeck()
    {
        m_CustomDeckPopup.gameObject.SetActive(true);
        Game.main.socket.GetUserDeckDetail(currentDeck);
    }
}
