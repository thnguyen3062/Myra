using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GIKCore;
using GIKCore.Net;
using GIKCore.Utilities;
using TMPro;
using pbdson;
using UnityEngine.UI;
using GIKCore.Sound;
using GIKCore.UI;
using System.Linq;

public class FindMatchScene : GameListener
{
    // Fields
    [SerializeField] private TextMeshProUGUI m_TxtTime;
    [SerializeField] private CellCollectionDeck m_CellCollectionDeck;
    [SerializeField] private GameObject m_BackgroundNormal, m_BackgroundRank, m_BoxNormal, m_BoxRank;
    private HeroCard cardFindMatch;
    // Values
    ITimeDelta findTime = 0;

    // Methods
    public void DoCancel()
    {
        SoundHandler.main.PlaySFX("900_click_3", "sounds");
        Game.main.socket.GameBattleLeave();
        Game.main.socket.GetUserDeck();
        Game.main.socket.GetRank();
        
    }

    private void PlayerBattleJoin(ListCommonVector lstCommonVector)
    {
        foreach (CommonVector cv in lstCommonVector.aVector)
        {
            BattlePlayer player = new BattlePlayer();
            player.position = cv.aLong[0];
            player.id = cv.aLong[1];
            player.username = cv.aString[0];
            player.screenname = cv.aString[1];

            GameData.main.mLstBattlePlayer.Add(player);
        }
    }
    private void PlayerBattleLeave(CommonVector commonVector)
    {
        long position = commonVector.aLong[0];
        long id = commonVector.aLong[1];
        bool isHidenService = commonVector.aLong[2] == 1 ? true : false;

        GameData.main.isOutToModeSelection = commonVector.aLong[3]==1 ? true : false;
        foreach (BattlePlayer player in GameData.main.mLstBattlePlayer)
            if (player.id == id)
            {
                GameData.main.mLstBattlePlayer.Remove(player);
                break;
            }
        if(!isHidenService)
        {
            if (GameData.main.isOutToModeSelection)
            {
                GameData.main.isOutToModeSelection = false;
                Game.main.socket.GetMode();
                Game.main.LoadScene("SelectModeScene", delay: 0.3f, curtain: true);
            }
            else
            {
                Game.main.socket.GetUserDeck();
                Game.main.socket.GetRank();
                if (GameData.main.currentPlayMode == "unrank")
                    Game.main.LoadScene("SelectDeckScene", delay: 0.3f, curtain: true);
                else if (GameData.main.currentPlayMode == "event")
                    Game.main.LoadScene("SelectDeckEventScene", delay: 0.3f, curtain: true);
                else
                    Game.main.LoadScene("SelectDeckRankScene", delay: 0.3f, curtain: true);
            }
        }  
    }
    private void GameStart(CommonVector commonVector)
    {
        SoundHandler.main.PlaySFX("MatchFound", "sounds");
        LogWriterHandle.WriteLog("GAME START=" + IUtil.ConvertListLongToString(commonVector.aLong));
        LogWriterHandle.WriteLog("GAME START=" + IUtil.ConvertListStringToString(commonVector.aString));
        long idFirst = commonVector.aLong[0];
        string usernameFirst = commonVector.aString[0];

        foreach (BattlePlayer player in GameData.main.mLstBattlePlayer)
            if (player.id == idFirst)
            {
                player.isFirst = true;
                player.towerHealth = commonVector.aLong[1];

            }
            else
                player.towerHealth = commonVector.aLong[2];
        GameData.main.lstSkillAuction.Clear();
        for (int i =3; i<commonVector.aLong.Count;i++)
        {
            GameData.main.lstSkillAuction.Add(commonVector.aLong[i]);
        }    
        Debug.Log(GameData.main.lstSkillAuction.Count);
        Game.main.LoadScene("BattleScene", delay: 0.3f, curtain: true);
        GameData.main.isUsedUlti = false;
    }

    public override bool ProcessSocketData(int serviceId, byte[] data)
    {
        if (base.ProcessSocketData(serviceId, data)) return true;
        switch (serviceId)
        {
            case IService.GAME_BATTLE_JOIN:
                {
                    ListCommonVector lstCommonVector = ISocket.Parse<ListCommonVector>(data);
                    PlayerBattleJoin(lstCommonVector);
                    break;
                }  

            case IService.GAME_BATTLE_LEAVE:
                {
                    CommonVector commonVector = ISocket.Parse<CommonVector>(data);
                    PlayerBattleLeave(commonVector);
                    break;
                }

            case IService.GAME_START:
                {
                    CommonVector commonVector = ISocket.Parse<CommonVector>(data);
                    GameStart(commonVector);
                    break;
                }
            case IService.GET_USER_DECK_DETAIL:
                {
                    ListCommonVector listCommonVector = ISocket.Parse<ListCommonVector>(data);
                    InitDataCustomDeckCard(listCommonVector);
                    break;
                }

        }
        return false;
    }
    private void InitDataCustomDeckCard(ListCommonVector listCommonVector)
    {
        CommonVector cv0 = listCommonVector.aVector[0];
        if (cv0.aLong[0] == 0)
        {
        }
        else
        {           
        }
        CommonVector cv1 = listCommonVector.aVector[1];
        HeroCard hc = GameData.main.lstHeroCard.FirstOrDefault(x => x.id == cv1.aLong[0]);
        DBHero db = hc.GetDatabase();
        //Sprite sprite = CardData.Instance.GetGodCardNewSprite(db.id);
        PlayerPrefs.SetInt("CustomDeckPreview", (int)hc.id);
        Debug.Log("CustomDeckPreview" + hc.id);
        StartGame();
    }

    void StartGame()
    {
        GetPreviewSprite();
        //preViewDeckImage.sprite = godPreviewImage[PlayerPrefs.GetInt("CurrentDeck")];
        GameData.main.mLstBattlePlayer = new List<BattlePlayer>();
        Game.main.socket.GameBattleJoin();
        SoundHandler.main.PlaySFX("Finding Match", "sounds");
    }
    private void Start()
    {
        m_BackgroundNormal.SetActive(GameData.main.currentPlayMode == "unrank");
        m_BoxNormal.SetActive(GameData.main.currentPlayMode == "unrank");
        m_BackgroundRank.SetActive(GameData.main.currentPlayMode != "unrank");
        m_BoxRank.SetActive(GameData.main.currentPlayMode != "unrank");
        if (!GameData.main.hadCustomDeck)
        {
            StartGame();
            StartCoroutine(Delay());
        }
        else
            Game.main.socket.GetUserDeckDetail(GameData.main.lstDeckInfo[PlayerPrefs.GetInt("CurrentDeck")].deckID);
    }
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.5f);
        SoundHandler.main.PlaySFX("Finding Match", "sounds");
    }    
    private void GetPreviewSprite()
    {
        DeckInfo fmDeck = GameData.main.lstDeckInfo[PlayerPrefs.GetInt("CurrentDeck")];
        if (fmDeck != null)
        {
            m_CellCollectionDeck.gameObject.SetActive(true);
            m_CellCollectionDeck.SetData(fmDeck);
        } else
        {
        }
    }
    // Update is called once per frame
    void Update()
    {
        long timePass = findTime.GetTimePassInSeconds();
        if (timePass > 0) findTime += timePass;
        m_TxtTime.text = ITimer.GetTimeDisplay(findTime.time, ITimeSpanFormat.MM_SS);
    }
}
