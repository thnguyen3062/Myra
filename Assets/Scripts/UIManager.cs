using GIKCore.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using GIKCore.Utilities;
using GIKCore.Net;
using PathologicalGames;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    #region Serialized Field
    [SerializeField] private GodCardHandler godCardHandler;
    [SerializeField] private GodPanelController playerGodContainer;
    [SerializeField] private GodPanelController enemyGodContainer;
    [SerializeField] private GameObject blockPanel;
    [SerializeField] private GameObject cardPreview;
    [SerializeField] private GameObject playLoadUI;
    [SerializeField] private CardPreviewInfoBattle godCardPreview;
    [SerializeField] private CardPreviewInfoBattle minionCardPreview;
    [SerializeField] private CardPreviewInfoBattle spellCardPreview;
    [SerializeField] private GameObject cardPreviewShort;
    [SerializeField] private GameObject showCardPrefab;
    [SerializeField] private Transform showCard1, showCard2;
    [SerializeField] private CardPreviewInfo godCardPreviewShort;
    [SerializeField] private CardPreviewInfo minionCardPreviewShort;
    [SerializeField] private CardPreviewInfo spellCardPreviewShort;
    [SerializeField] private TextMeshProUGUI enemyNameText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private GameObject playerReadyText;
    [SerializeField] private GameObject enemyReadyText;
    [SerializeField] private GameObject findMatchSucceedUI;
    [SerializeField] private Image timer1;
    [SerializeField] private Image timer2;
    [SerializeField] private const float START_UP_TIME = 30;
    [SerializeField] private const float TURN_TIME = 60;
    [SerializeField] private const float CHOOSE_WAY_TIME = 20;
    [SerializeField] private Image preViewDeckImage;
    [SerializeField] private GameObject previewCard;
    [SerializeField] private Image surrenderImage;
    [SerializeField] private GameObject surrenderPopup;
    [SerializeField] private Sprite[] surrenderSprite; // 0 surrender, 1 win
    [SerializeField] private GameObject timerObject;
    [SerializeField] private Transform m_FirstKeywordPopup;
    [SerializeField] private Transform m_FirstKeywordPopupContainer;

    [Header("Bid")]
    [SerializeField] private Transform m_ListSkillAuctionContainer;
    [SerializeField] private PrefabSpawner m_BidPrefabSpawner;
    [SerializeField] private GameObject m_BidUI;
    #endregion

    public GodInfoHandler godInfoObject;
    public GodInfoHandler godInfoObjectEnemy;
    public GameObject shardObject;
    private float currentTime;
    private float maxTime;
    private HeroCard cardFindMatch;
    public event ICallback.CallFunc onSurrender;
    public event ICallback.CallFunc5<IEnumerator> showCardOnPlayLoading;

    private void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
        GetPreviewSprite();
    }
    private void Start()
    {
        GameBattleScene.instance.onGameBattleSimulation += GameBattleSimulation;
        GameBattleScene.instance.onInitPlayer += SetName;
        GameBattleScene.instance.onGameStartRound += GameStartRound;
        GameBattleScene.instance.onBidStateStart += OnBidState;
        GameBattleScene.instance.onBidEnd += OnBidEnd;
        GameBattleScene.instance.onUpBidState += OnUpBid;
        GameBattleScene.instance.onGameBattleEndGame += GameBattleEndGame;
        GameBattleScene.instance.onGameConfirmStartBattle += GameConfirmStartBattle;
        GameBattleScene.instance.onGameChooseWay += GameChooseWay;
        GameBattleScene.instance.onGameChooseWayRequest += GameChooseWayRequest;
        GameBattleScene.instance.onGameStartupConfirm += GameStartupConfirm;
        GameBattleScene.instance.onGameStartupEnd += GameStartupEnd;
        GameBattleScene.instance.onGameDealCard += GameDealCard;
        GameBattleScene.instance.onGameBattleChangeTurn += OnGameBattleChangeTurn;
        // godCardHandler.onUpdateGodCount = OnUpdateGodCount;
        GameBattleScene.instance.onResumeTime += OnResume;
        showCardOnPlayLoading += ShowCardOnPlayLoading;
    }

    public void GetPreviewSprite()
    {
        if (PlayerPrefs.GetInt("CurrentDeck") <= 3)
        {
            switch (PlayerPrefs.GetInt("CurrentDeck"))
            {
                case 0:
                    {
                        cardFindMatch = GameData.main.lstHeroCard.FirstOrDefault(x => x.heroId == 22);
                        DBHero hero = Database.GetHero(22);
                        previewCard.SetActive(true);
                        if (cardFindMatch != null)
                            previewCard.GetComponent<CardPreviewInfo>().SetCardPreview(hero, cardFindMatch.frame, false);
                        else
                            previewCard.GetComponent<CardPreviewInfo>().SetCardPreview(hero, 1, false);
                    }
                    break;
                case 1:
                    {
                        cardFindMatch = GameData.main.lstHeroCard.FirstOrDefault(x => x.heroId == 349);
                        DBHero hero = Database.GetHero(349);
                        previewCard.SetActive(true);
                        if (cardFindMatch != null)
                            previewCard.GetComponent<CardPreviewInfo>().SetCardPreview(hero, cardFindMatch.frame, false);
                        else
                            previewCard.GetComponent<CardPreviewInfo>().SetCardPreview(hero, 1, false);
                    }
                    break;
                case 2:
                    {
                        cardFindMatch = GameData.main.lstHeroCard.FirstOrDefault(x => x.heroId == 351);
                        DBHero hero = Database.GetHero(351);
                        previewCard.SetActive(true);
                        if (cardFindMatch != null)
                            previewCard.GetComponent<CardPreviewInfo>().SetCardPreview(hero, cardFindMatch.frame, false);
                        else
                            previewCard.GetComponent<CardPreviewInfo>().SetCardPreview(hero, 1, false);
                    }
                    break;
                case 3:
                    {
                        cardFindMatch = GameData.main.lstHeroCard.FirstOrDefault(x => x.heroId == 350);
                        DBHero hero = Database.GetHero(350);
                        previewCard.SetActive(true);
                        if (cardFindMatch != null)
                            previewCard.GetComponent<CardPreviewInfo>().SetCardPreview(hero, cardFindMatch.frame, false);
                        else
                            previewCard.GetComponent<CardPreviewInfo>().SetCardPreview(hero, 1, false);
                    }
                    break;

            }
            //previewCard.SetActive(true);
            //if(cardFindMatch!= null)
            //    previewCard.GetComponent<CardPreviewInfo>().SetCardPreview(cardFindMatch.heroId, cardFindMatch.frame, false);
            //else
            //    previewCard.GetComponent<CardPreviewInfo>().SetCardPreview(cardFindMatch.heroId, 1, false);
            //preViewDeckImage.sprite = godPreviewImage[PlayerPrefs.GetInt("CurrentDeck")];
        }
        else
        {
            cardFindMatch = GameData.main.lstHeroCard.FirstOrDefault(x => x.id == PlayerPrefs.GetInt("CustomDeckPreview"));
            DBHero hero = Database.GetHero(cardFindMatch.heroId);
            previewCard.SetActive(true);
            previewCard.GetComponent<CardPreviewInfo>().SetCardPreview(hero, cardFindMatch.frame, false);
        }
        findMatchSucceedUI.SetActive(true);
    }
    int countRoundOfBid = 0;
    public void InitListBidSkill()
    {
        List<long> lstSkillID = GameData.main.lstSkillAuction.GetRange(0, 2);
        m_ListSkillAuctionContainer.GetComponent<ListSkillAuctionController>().InitData(lstSkillID);
    }
    public void InitListSkillOnResume(long curIndex)
    {
        countRoundOfBid =(int) curIndex;
        OnUpdateListBidSkill();
    }
    public void OnUpdateListBidSkill()
    {

        if (countRoundOfBid > 6)
        {
            countRoundOfBid = 0;
        }
        List<long> lstSkillID = new List<long>();
        if (countRoundOfBid == 6)
        {
            lstSkillID.Add(GameData.main.lstSkillAuction[6]);
            lstSkillID.Add(GameData.main.lstSkillAuction[0]);
            m_ListSkillAuctionContainer.GetComponent<ListSkillAuctionController>().OnUpdateListSkill(lstSkillID);
        }
        else
        {
            lstSkillID = GameData.main.lstSkillAuction.GetRange(countRoundOfBid, 2);
            m_ListSkillAuctionContainer.GetComponent<ListSkillAuctionController>().OnUpdateListSkill(lstSkillID);
        }
        countRoundOfBid++;
    }

    private void OnResume(bool p1, bool p2, long timeRemain, long timeMax)
    {
        currentTime = timeRemain;
        maxTime = timeMax;
        SetCountDownStatus(p1, p2, timeMax, currentTime);
        //timerObject.GetComponent<Animation>().Play()
    }

    public void OnSurrender()
    {
        onSurrender?.Invoke();
    }

    public void SetTime(long round)
    {
        if (round == 1)
            maxTime = 12;
        else if (round == 2)
            maxTime = 30;
        else if (round == 3)
            maxTime = 40;
        else if (round == 4)
            maxTime = 40;
        else
        {
            maxTime = 50;
        }
        currentTime = maxTime;
    }

    public void OnSurrender(long index)
    {
        surrenderPopup.gameObject.SetActive(true);
        surrenderImage.sprite = surrenderSprite[index];
        surrenderImage.SetNativeSize();
    }

    public void GameDealCard(List<long> godBattleID1, List<DBHero> godHero1, List<long> frame1, List<long> atk1, List<long> hp1, List<long> mana1, List<long> godBattleID2, List<DBHero> godHero2, List<long> frame2, List<long> atk2, List<long> hp2, List<long> mana2)
    {
        //SetStartupTime();
        godCardHandler.InitGodUI(godBattleID1, godHero1, frame1, atk1, hp1, mana1, godBattleID2, godHero2, frame2, atk2, hp2, mana2);
        var groupGodPlayer = godHero1.GroupBy(g => g.heroNumber).Select(grp => grp.ToList()).ToList();
        groupGodPlayer.ForEach(x =>
        {
            GameObject go = Instantiate(showCardPrefab, Vector3.zero, Quaternion.identity, showCard1);
            ShowCardUI card = go.GetComponent<ShowCardUI>();
            card.InitData(x[0].heroNumber);
        });
        var groupGodEnemy = godHero2.GroupBy(g => g.heroNumber).Select(grp => grp.ToList()).ToList();
        groupGodEnemy.ForEach(x =>
        {
            GameObject go = Instantiate(showCardPrefab, Vector3.zero, Quaternion.identity, showCard2);
            ShowCardUI card = go.GetComponent<ShowCardUI>();
            card.InitData(x[0].heroNumber);
        });
        StartCoroutine(showCardOnPlayLoading?.Invoke());

    }
    private IEnumerator ShowCardOnPlayLoading()
    {

        yield return new WaitForSeconds(0.6f);
        playLoadUI.SetActive(true);
        yield return new WaitForSeconds(2f);
        playLoadUI.transform.parent.gameObject.SetActive(false);
    }
    private void GameStartupConfirm(bool isPlayer)
    {
        if (isPlayer)
        {
            playerReadyText.gameObject.SetActive(true);
        }
        else
        {
            enemyReadyText.gameObject.SetActive(true);
        }
    }

    private void GameStartupEnd()
    {
        playerReadyText.gameObject.SetActive(false);
        enemyReadyText.gameObject.SetActive(false);
    }

    private void SetName(bool pos6H, string screenname, long towerHealth)
    {
        if (pos6H)
            playerNameText.text = screenname;
        else
            enemyNameText.text = screenname;
    }

    public void OnCloseGodContainer()
    {
        playerGodContainer.OnClose();
        enemyGodContainer.OnClose();
    }

    //public IEnumerator ShowPreviewCardInBattle(Card card, long id, long frame = 1)
    //{
    //    ShowPreviewHandCard(card, id, frame);

    //    yield return new WaitForSeconds(2);
    //    ClosePreviewHandCard();
    //}

    public void ShowPreviewShortCard(Card card, long id, long frame)
    {
        if (cardPreviewShort.activeSelf)
            return;

        cardPreviewShort.SetActive(true);
        DBHero hero = Database.GetHero(id);
        if (hero.type == DBHero.TYPE_GOD)
        {
            godCardPreviewShort.gameObject.SetActive(true);
            godCardPreviewShort.SetCardPreview(hero, frame, false);
        }
        if (hero.type == DBHero.TYPE_TROOPER_MAGIC)
        {
            spellCardPreviewShort.gameObject.SetActive(true);
            spellCardPreviewShort.SetCardPreview(hero, frame, false);
        }
        if (hero.type == DBHero.TYPE_TROOPER_NORMAL)
        {
            minionCardPreviewShort.gameObject.SetActive(true);
            minionCardPreviewShort.SetCardPreview(hero, frame, false);
        }
    }

    public void ClosePreviewShortCard()
    {
        if (cardPreviewShort.gameObject.activeSelf)
        {
            godCardPreviewShort.gameObject.SetActive(false);
            minionCardPreviewShort.gameObject.SetActive(false);
            cardPreviewShort.gameObject.SetActive(false);
            spellCardPreviewShort.gameObject.SetActive(false);
        }
    }

    public void ShowPreviewHandCard(Card card, DBHero hero, long frame, ICallback.CallFunc complete = null)
    {
        if (cardPreview.activeSelf)
            return;
        cardPreview.SetActive(true);
        if (hero != null)
        {
            if (hero.type == DBHero.TYPE_GOD)
            {
                godCardPreview.gameObject.SetActive(true);
                godCardPreview.SetCardPreview(hero, frame, true);
            }
            if (hero.type == DBHero.TYPE_TROOPER_MAGIC)
            {
                spellCardPreview.gameObject.SetActive(true);
                spellCardPreview.SetCardPreview(hero, frame, true);
            }
            if (hero.type == DBHero.TYPE_TROOPER_NORMAL)
            {
                minionCardPreview.gameObject.SetActive(true);
                minionCardPreview.SetCardPreview(hero, frame, true);
            }
        }
        complete?.Invoke();
    }

    public void ClosePreviewHandCard()
    {
        if (cardPreview.gameObject.activeSelf)
        {
            godCardPreview.gameObject.SetActive(false);
            minionCardPreview.gameObject.SetActive(false);
            cardPreview.gameObject.SetActive(false);
            spellCardPreview.gameObject.SetActive(false);
        }
    }

    private void GameStartRound()
    {
        SetCountDownStatus(false, false, currentTime);
    }
    private void GameBattleSimulation(bool isPlayer, long roundCount, long turnMana)
    {
        SetShardStatus(isPlayer);
        SetTime(turnMana);

        SetCountDownStatus(isPlayer, !isPlayer, maxTime);

    }

    private void GameBattleEndGame(string winner)
    {
        SetCountDownStatus(false, false, maxTime);
    }

    private void GameConfirmStartBattle()
    {
        SetShardStatus(false);
    }

    private void GameChooseWay(bool isPlayer)
    {
        if (isPlayer)
        {
            SetCountDownStatus(isPlayer, !isPlayer, maxTime);
        }
    }

    private void GameChooseWayRequest()
    {
        SetCountDownStatus(false, false, maxTime);
    }

    private void SetCountDownStatus(bool p1, bool p2, float timeMax, float timeRemain = -1)
    {
        timerObject.SetActive(false);
        if (!p1 && !p2)
        {
            timerObject.SetActive(false);
        }
        else
        {
            timerObject.SetActive(true);
            if (timeMax == 12)
            {
                timerObject.GetComponent<Animator>().speed = 2f;
            }
            else if (timeMax == 30)
            {
                timerObject.GetComponent<Animator>().speed = 1;
            }
            else if (timeMax == 40)
            {
                timerObject.GetComponent<Animator>().speed = 0.75f;
            }
            else
            {
                timerObject.GetComponent<Animator>().speed = .6f;
            }
            if (timeRemain != (-1))
            {
                float startTime = (timeMax - timeRemain) / timeMax;
                timerObject.GetComponent<Animator>().Play("TimeCount", 0, startTime);

            }
            else
                timerObject.GetComponent<Animator>().Play("TimeCount");
            //timer1.gameObject.SetActive(true);
            //timer2.gameObject.SetActive(true);
        }
        //timer1.sprite = timerSprite[p1 ? 0 : 1];
        //timer2.sprite = timerSprite[p1 ? 0 : 1];
    }

    private void SetShardStatus(bool isPlayer)
    {
        //getShardButton.interactable = isPlayer;
    }

    private void OnGameBattleChangeTurn(long index)
    {
    }
    public void OnMeetFirstKeyword(string keySprite, string keyData, long count = -1)
    {
        Transform trans = PoolManager.Pools["Effect"].Spawn(m_FirstKeywordPopup);
        trans.SetParent(m_FirstKeywordPopupContainer);
        trans.localPosition = Vector3.zero;
        trans.localScale = Vector3.one;
        trans.GetComponent<KeywordInfo>().InitData(keySprite, keyData, count);
    }
    private void OnBidState(BidState state, long playerShard, long enemyShard, long pBid, long eBid, long timeRemain)
    {
        if (state == BidState.StartBid)
            m_BidPrefabSpawner.OnStartBid(playerShard, enemyShard, pBid, eBid, timeRemain);
        else if (state == BidState.NoBid)
            m_BidPrefabSpawner.OnNoBidRound();
    }
    private void OnBidEnd(BidState state, bool isMe)
    {
        m_BidPrefabSpawner.OnEndBid(state, isMe, out float timeEff);
        GameBattleScene.instance.OnEndEffectBidPhase(timeEff);
    }
    private void OnUpBid(BidState state, bool isMe, long numberBid, long balance)
    {
        if (isMe)
            m_BidPrefabSpawner.OnUpBidYou(numberBid, balance);
        else
            m_BidPrefabSpawner.OnUpBidEnemy(numberBid, balance);
    }
    private void OnDisable()
    {

    }
}
