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

public class SelectDeckEventScene : GameListener
{
    [SerializeField] private GameObject[] decks, crystals;
    [SerializeField] private GameObject settingUI;
    [SerializeField] private GameObject popupEditPrefab;
    List<long> lstTrooper = new List<long>();
    List<long> lstGod = new List<long>();
    private long currentDeck = -1;
    private GameObject popupEdit;
    [SerializeField] private TextMeshProUGUI totalMatch;
    [SerializeField] private TextMeshProUGUI totalWin;
    [SerializeField] private TextMeshProUGUI startTime, endTime,myraTxt;

    private void StartGame()
    {
        totalWin.text = GameData.main.eventTotalWin.ToString();
        totalMatch.text = GameData.main.eventTotalMatch.ToString();
        //startTime.text = GameData.main.startTimeCurrentEvent;
        //endTime.text = GameData.main.endTimeCurrentEvent;
        if (PlayerPrefs.HasKey("CurrentDeck"))
        {
            settingUI.SetActive(false);
            int lastDeck = -1;
            //int currentDeck = PlayerPrefs.GetInt("CurrentDeck");
            //SetBattleDeck(currentDeck);
            for (int i = 0; i < GameData.main.lstDeckInfo.Count; i++)
            {
            //    if (GameData.main.lstDeckInfo[i].isLastDeckEvent)
            //    {
            //        lastDeck = i;
            //        SetBattleDeck(lastDeck);
            //    }
            }
            foreach (GameObject deck in decks)
            {
                RectTransform rt = deck.gameObject.GetComponent(typeof(RectTransform)) as RectTransform;
                rt.sizeDelta = new Vector2(870, 117);
                deck.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
            }
            foreach (GameObject crystal in crystals)
            {
                crystal.SetActive(false);
            }
            //if (GameData.main.lstDeckInfo.Count > 4)
            //{
            //    for (int i = 4; i < GameData.main.lstDeckInfo.Count; i++)
            //    {
            //        if (GameData.main.lstDeckInfo[i].isDeckEven)
            //        {
            //            decks[i].SetActive(true);
            //            long id = GameData.main.lstDeckInfo[i].deckID;
            //            Game.main.socket.GetUserDeckDetail(id);
            //        }
            //    }
            //}
            //else
            //{
            //    for (int i = 4; i < GameData.main.lstDeckInfo.Count; i++)
            //    {
            //        decks[i].SetActive(false);
            //    }
            //}
            
            if (currentDeck != -1)
            {
                decks[lastDeck].gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(984, 152);
                crystals[lastDeck].SetActive(true);
                // editButton[lastDeck].SetActive(true);
                decks[lastDeck].GetComponent<Image>().color = new Color(1, 1, 1);
            }
        }
        else
        {
            foreach (GameObject deck in decks)
            {
                RectTransform rt = deck.gameObject.GetComponent(typeof(RectTransform)) as RectTransform;
                rt.sizeDelta = new Vector2(870, 117);
                deck.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
            }
            foreach (GameObject crystal in crystals)
            {
                crystal.SetActive(false);
            }
            //if (GameData.main.lstDeckInfo.Count > 4)
            //{
            //    for (int i = 4; i < GameData.main.lstDeckInfo.Count; i++)
            //    {
            //        if (GameData.main.lstDeckInfo[i].isDeckEven)
            //        {
            //            decks[i].SetActive(true);
            //            long id = GameData.main.lstDeckInfo[i].deckID;
            //            Game.main.socket.GetUserDeckDetail(id);
            //            if (!FindObjectOfType<EditFrameControl>())
            //            {
            //                popupEdit = Instantiate(popupEditPrefab, Game.main.canvas.panelPopup);
            //                popupEdit.SetActive(false);
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    for (int i = 4; i < GameData.main.lstDeckInfo.Count; i++)
            //    {
            //        decks[i].SetActive(false);
            //    }
            //}
        }
    }
    public void DoClickSelectMode()
    {
        Game.main.socket.GetMode();
        Game.main.LoadScene("SelectModeScene", delay: 0.3f, curtain: true);
    }
    public void DoClickCollection()
    {
        Game.main.socket.GetUserDeck();
        Game.main.socket.GetUserPack();
        Game.main.LoadScene("CollectionScene", delay: 0.3f, curtain: true);
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
            GameData.main.currentPlayMode = "event";
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
        Game.main.LoadScene("HomeSceneNew", delay: 0.3f, curtain: true);
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
    public void SetBattleDeck(int type)
    {
        currentDeck = GameData.main.lstDeckInfo[type].deckID;

        lstGod = new List<long>();
        lstTrooper = new List<long>();
        foreach (GameObject deck in decks)
        {
            RectTransform rt = deck.gameObject.GetComponent(typeof(RectTransform)) as RectTransform;
            rt.sizeDelta = new Vector2(870, 117);
            deck.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
        }
        foreach (GameObject crystal in crystals)
        {
            crystal.SetActive(false);
        }
        decks[type].gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(984, 152);
        decks[type].GetComponent<Image>().color = new Color(1, 1, 1);
        crystals[type].SetActive(true);
        PlayerPrefs.SetInt("CurrentDeck", type);
    }
    void InitDataCustomDeckCard(ListCommonVector listCommonVector)
    {
        CommonVector cv0 = listCommonVector.aVector[0];
        if (cv0.aLong[0] == 0)
        {
            Toast.Show(LangHandler.Get("69", "Load failed"));
        }
        else
        {
        }
        long deckId = cv0.aLong[1];
        for (int i = 4; i < GameData.main.lstDeckInfo.Count; i++)
        {
            if (GameData.main.lstDeckInfo[i].deckID == deckId)
            {
                CommonVector cv1 = listCommonVector.aVector[1];
                HeroCard hc = new HeroCard();
                if (cv1.aLong.Count > 0)
                {
                    hc = GameData.main.lstHeroCard.FirstOrDefault(x => x.id == cv1.aLong[0]);
                }
                else hc = null;
                string name = GameData.main.lstDeckInfo[i].deckName;
                bool status = (GameData.main.lstDeckInfo[i].deckStatus == 1) ? true : false;
                decks[i].gameObject.GetComponent<CustomDeckPrefabControl>().InitData(deckId, name, status, hc);
                break;
            }
        }
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
             case IService.GET_USER_EVENT_INFO:
                CommonVector cv1 = ISocket.Parse<CommonVector>(data);
                Debug.Log("GET_EVENT_INFO: " + string.Join(",", cv1.aLong));

                if (cv1.aLong.Count > 0)
                {
                    if (cv1.aLong[0] != -1)
                    {
                        GameData.main.eventTotalMatch = cv1.aLong[0];
                        GameData.main.eventTotalWin = cv1.aLong[1];
                        GameData.main.myraEvent = cv1.aLong[2];
                        long start = cv1.aLong[2];
                        long end = cv1.aLong[3];
                        DateTime startTime1, endTime1;
                        startTime1 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(start).ToLocalTime();
                        endTime1 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(end).ToLocalTime();
                        startTime.text = startTime1.ToString("MM-dd-yyyy");
                        endTime.text = endTime1.ToString("MM-dd-yyyy");
                        myraTxt.text = LangHandler.Get("139", "TOTAL PRIZE POOL: Myra ") + cv1.aLong[4];
                        Debug.Log("Start Time: " + startTime1.ToString("MM-dd-yyyy") + " |End Time: " + endTime1.ToString("MM-dd-yyyy"));
                    }
                    else
                    {
                        GameData.main.myraEvent = 0;
                        GameData.main.eventTotalMatch = 0;
                        GameData.main.eventTotalWin = 0;
                    }
                }
                break;
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
                    StartGame();
                    break;
                }
            case IService.GET_USER_DECK_DETAIL:
                ListCommonVector listCommonVector = ISocket.Parse<ListCommonVector>(data);
                InitDataCustomDeckCard(listCommonVector);
                break;
        }
        return false;
    }
}
