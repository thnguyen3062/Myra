using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GIKCore.Net;
using pbdson;
using GIKCore;
using GIKCore.Utilities;
using GIKCore.UI;
using System.Linq;
using DG.Tweening;
using UnityEngine.SceneManagement;
using GIKCore.Sound;
using UnityEngine.Video;
using UnityEngine.UI;
using Spine.Unity;
using GIKCore.Lang;
using TMPro;  
using EZCameraShake;
using GIKCore.Bundle;
using System.IO;

public enum BATTLE_STATE
{
    WAIT_COMFIRM,
    NORMAL
}

public class GameBattleScene : GameListener
{
    public static GameBattleScene instance;

    const long POS_6h = 0;
    const long POS_12h = 1;
    const int MAX_PLAYER = 2;

    [HideInInspector] public BATTLE_STATE battleState;

    #region Serialized Field
    // Fields
    [SerializeField] private GameObject m_MinionCard;
    [SerializeField] private GameObject m_MinionOnBoardCard;
    [SerializeField] private GameObject m_GodCard;
    [SerializeField] private GameObject m_EnemyMinionCard;
    [SerializeField] private GameObject m_SpellOnBoard;
    [SerializeField] private GameObject m_EnemyMinionOnBoardCard;
    [SerializeField] private GameObject m_EnemyGodOnBoardCard;
    [SerializeField] private Transform showOpponentCardPoint;
    [SerializeField] private List<Transform> spawnedSpellLine;
    [SerializeField] private List<GameObject> turnEffect;
    //[SerializeField] private List<GameObject> wayDirectionEffect;
    [SerializeField] private GameObject fadedScreen;
    [SerializeField] private GameObject settingUI;
    [SerializeField] private Transform shardFlyEffect;
    [SerializeField] private Transform bathalaShard;
    [SerializeField] private GameObject[] m_AllyCrackTower;
    [SerializeField] private GameObject[] m_EnemyCrackTower;

    [Header("Video")]
    [SerializeField] private VideoPlayer victoryVideo;
    [SerializeField] private GameObject victoryRenderTexture;
    [SerializeField] private VideoPlayer defeatedVideo;
    [SerializeField] private GameObject defeatedRenderTexture;
    [SerializeField] private VideoPlayer ultimateVideo;
    [SerializeField] private GameObject ultimateRenderTexture;

    [Header("Spawn Effect")]
    [SerializeField] private Transform waterSpawnEffect;
    [SerializeField] private Transform holySpawnEffect;
    [SerializeField] private Transform natureVineSpawnEffect;
    [SerializeField] private Transform minionSkillSummonEffect;
    [SerializeField] private Transform enemyMinionSkillSummonEffect;
    [SerializeField] private Transform hadesSkillSummonEffect;
    [SerializeField] private Transform bathalaSkillSummonEffect;
    [SerializeField] private Transform poseidonSummonEffect;
    [SerializeField] private Transform sontinhSummonEffect;
    [SerializeField] private Transform m_EnemyHandcardSkillEffect;
    [SerializeField] private GameObject[] turnArrow;

    [SerializeField] private GameObject progressionPref;
    [SerializeField] private Transform ultiVfxContent;
    [SerializeField] private GameObject endGameRankUI;
    [SerializeField] private LayerMask groundLayer;
    #endregion

    //values scene
    List<BattlePlayer> mLstBattlePlayer = new List<BattlePlayer>();

    [Space(10)]
    public List<CardSlot> playerSlotContainer;
    public List<CardSlot> enemySlotContainer;

    public HandDeckLayout[] Decks = new HandDeckLayout[2];

    public Transform[] spawnPosition;
    public Transform magicSpawnPosition;
    public Transform spellRangePos;
    public Transform skillUnlockVFXSpawnPos;
    List<DBHero> lstHeroPlayer = new List<DBHero>();
    List<long> lstHeroBattleID = new List<long>();
    List<long> lstHeroFrame = new List<long>();
    List<long> lstHeroFragile = new List<long>();
    List<long> lstHeroAtk = new List<long>();
    List<long> lstHeroHp = new List<long>();
    List<long> lstHeroMana = new List<long>();
    public List<DBHero> lstGodPlayer = new List<DBHero>();
    List<long> lstGodPlayerBattleID = new List<long>();
    List<long> lstGodPlayerFrame = new List<long>();
    List<long> lstGodPlayerAtk = new List<long>();
    List<long> lstGodPlayerHp = new List<long>();
    List<long> lstGodPlayerMana = new List<long>();
    List<DBHero> lstGodEnemy = new List<DBHero>();
    List<long> lstGodEnemyBattleID = new List<long>();
    List<long> lstGodEnemyFrame = new List<long>();
    List<long> lstGodEnemyAtk = new List<long>();
    List<long> lstGodEnemyHp = new List<long>();
    List<long> lstGodEnemyMana = new List<long>();
    //[HideInInspector] public List<Card> lstMulliganCard = new List<Card>();
    //[HideInInspector] public bool isMulligan;

    [HideInInspector] public List<BoardCard> lstCardInBattle = new List<BoardCard>();

    public List<LaneController> lstLaneInBattle;
    public List<TowerController> lstTowerInBattle;
    //[HideInInspector] public Card currentCardSelected;

    public int playerIndex;

    public bool IsYourTurn;
    //[HideInInspector] public long currentShard;
    [HideInInspector] public long currentMana;
    [HideInInspector] public long tmpCurrentMana;
    public SpellLineController spellLine;

    // Action
    #region Action
    public event ICallback.CallFunc4<bool, long, long> onGameBattleSimulation;
    public event ICallback.CallFunc2<bool> onSubGameBattleSimulationHasBid;
    public event ICallback.CallFunc onGameStartRound;
    public event ICallback.CallFunc10<BidState,long,long,long,long,long> onBidStateStart;
    public event ICallback.CallFunc3<BidState, bool> onBidEnd;
    public event ICallback.CallFunc8<BidState, bool, long , long> onUpBidState;
    public event ICallback.CallFunc onConfirmEndEffectBid;
    public event ICallback.CallFunc4<bool, string, long> onInitPlayer;
    public event ICallback.CallFunc8<int, long, ManaState, long> onUpdateMana;
    public event ICallback.CallFunc3<int, long> onUpdateShard;
    public event ICallback.CallFunc3<int, long> onUpdateShardClick;
    public event ICallback.CallFunc2<string> onGameBattleEndGame;
    public event ICallback.CallFunc onGameConfirmStartBattle;
    public event ICallback.CallFunc2<bool> onGameChooseWay;
    public event ICallback.CallFunc onGameChooseWayRequest;
    public event ICallback.CallFunc2<bool> onGameStartupConfirm;
    public event ICallback.CallFunc onGameStartupEnd;
    public event ICallback.CallFunc9<List<long>, List<DBHero>, List<long>, List<long>, List<long>, List<long>, List<long>, List<DBHero>, List<long>, List<long>, List<long>, List<long>> onGameDealCard;
    public event ICallback.CallFunc onEndSkillActive;
    public event ICallback.CallFunc onFinishChooseOneTarget;
    //public event ICallback.CallFunc onDragShard;
    //public event ICallback.CallFunc onEndDragShard;

    public event ICallback.CallFunc3<int, Card> onGameDealHandCard;
    public event ICallback.CallFunc2<long> onGameBattleChangeTurn;
    public event ICallback.CallFunc2<bool> onChangeSkillState;
    public event ICallback.CallFunc8<bool, bool, long, long> onResumeTime;
    public event ICallback.CallFunc2<long> onResumeRoundCount;
    public event ICallback.CallFunc onResetAttackCount;
    public event ICallback.CallFunc2<long> onSpawnRandomGod;
    public event ICallback.CallFunc2<long> onSpawnRandomGodEnemy;
    public event ICallback.CallFunc3<long, long> onActiveSkillActive;
    public event ICallback.CallFunc2<long> isUseUltimate;
    public ICallback.CallFunc4<int, int, bool> onTowerDestroyed;
    #endregion

    [HideInInspector] public GameObject currentGodCardUI;
    [HideInInspector] public CardSlot selectedCardSlot;
    [HideInInspector] public TowerController selectedTower;
    [HideInInspector] public LaneController selectedLane;
    [HideInInspector] public Cursor gameCursor;
    public Transform cursorObject;

    /*[HideInInspector]*/
    public SkillState skillState = SkillState.None;
    [HideInInspector] public List<BoardCard> lstSelectedSkillBoardCard = new List<BoardCard>();
    [HideInInspector] public List<HandCard> lstSelectedSkillHandCard = new List<HandCard>();
    [HideInInspector] public CardOnBoardClone onBoardClone;

    public TextMeshProUGUI chooseTargets;
    public TextMeshProUGUI getShardSucceed;
    public GameObject cancelSkill;

    private long turnCount;
    private long roundCount;
    private float waitTimeToBattle = 0;
    private float waitTimeToEffectSkill = 0;
    private float timeToProcess = 0.02f;
    private bool isSurrender = false;
    public long currentAvailableRegion = -1;
    private Card curCardSkill;
    public Touch touch;
    private Card curTouchCard;
    private Vector2 touchStartPosition, touchEndPosition;
    private float touchTime = 0;
    public bool isGameStarted { private set; get; }

    //xu ly skill co target
    private bool isStartFindTarget = false;
    private int countSkillInfo = 0;
    private int countEffect = 0;
    private CommonVector lstTarget = new CommonVector();
    //xu ly queue
    List<DataQueue> mQueue = new List<DataQueue>();
    bool onProcessData = false;
    List<Action> lstSkillQueue = new List<Action>();
    bool canContinue = true;
    private bool isWattingForVFX = false;
    public void DoClickSetting()
    {
        settingUI.SetActive(true);
    }
    public override bool ProcessSocketData(int serviceId, byte[] data)
    {

        if (base.ProcessSocketData(serviceId, data)) return true;

        switch (serviceId)
        {
            case IService.GAME_BATTLE_LEAVE:
                {
                    if (isSurrender)
                    {
                        CommonVector commonVector = ISocket.Parse<CommonVector>(data);
                        WriteLogBattle("SURRENDER", string.Join(",", commonVector.aString), string.Join(",", commonVector.aLong));
                        //StartCoroutine(PlayerSurrender(commonVector));
                        GameData.main.isOutToModeSelection = commonVector.aLong[3] == 1 ? true : false;
                    }
                    return false;
                }
        }
        mQueue.Add(new DataQueue(serviceId, data));

        return false;
    }

    IEnumerator PlayerSurrender(CommonVector commonVector)
    {
        yield return new WaitForSeconds(0f);

        if (isSurrender)
        {
            if (GameData.main.userProgressionState >= 14)
            {
                UIManager.instance.OnSurrender(0);
                yield return new WaitForSeconds(3);
                fadedScreen.SetActive(true);
                fadedScreen.GetComponent<Image>().DOFade(1, 0.2f).onComplete += delegate
                {
                    defeatedRenderTexture.SetActive(true);
                    defeatedVideo.clip = CardData.Instance.GetVideo("Defeat");
                    defeatedVideo.gameObject.SetActive(true);
                    defeatedVideo.Play();
                };
                yield return new WaitForSeconds(5);
                SoundHandler.main.Init("BackgroundMusicMain");
                yield return new WaitForSeconds(0.5f);
                Game.main.socket.GetMode();
                Game.main.LoadScene("SelectModeScene", delay: 0.3f, curtain: true);
                //SceneManager.LoadSceneAsync("SelectModeScene");
            }
        }
    }

    protected void Update()
    {
        if (timeToProcess > 0)
        {
            timeToProcess -= Time.deltaTime;
        }
        else
        {
            timeToProcess = 0.2f;
            ProcessQueue();
        }
        //if (Input.GetMouseButtonDown(1))
        //{
        //    //if (currentSkillCard == null)
        //    //    return;
        //    //if (!currentSkillCard.isMoveCompleted)
        //    //    return;

        //    SkillFailCondition();
        //}

        if (canContinue && lstSkillQueue.Count > 0 && !isWattingForVFX)
        {
            StartCoroutine(SimulateSkillEffectSingle(lstSkillQueue[0]));
            lstSkillQueue.RemoveAt(0);
        }


#if UNITY_ANDROID && !UNITY_EDITOR
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            Ray ray = Camera.main.ScreenPointToRay(touch.position);

            if (touch.phase == TouchPhase.Began)
            {
                if (Physics.Raycast(ray, out RaycastHit hit/*, Mathf.Infinity*/))
                {
                    if (hit.collider.GetComponent<Card>() != null)
                    {
                        curTouchCard = hit.collider.GetComponent<Card>();
                        LogWriterHandle.WriteLog(curTouchCard.heroInfo.name);
                        touchStartPosition = touch.position;
                        touchTime = Time.time;
                        curTouchCard.OnTouchDown();
                    }

                }
            }
            if (touch.phase == TouchPhase.Stationary && curTouchCard != null)
            {
                if (Time.time - touchTime >= 0.01f)
                {
                    curTouchCard.OnTouchHold();
                }
            }
            if (touch.phase == TouchPhase.Ended && curTouchCard != null)
            {
                touchEndPosition = touch.position;
                float x = touchEndPosition.x - touchStartPosition.x;
                float y = touchEndPosition.y - touchStartPosition.y;

                if (Mathf.Abs(x) == 0 && Mathf.Abs(y) == 0 && (Time.time - touchTime) < 0.1f)
                {
                    // click
                    //curTouchCard.OnClickInfo();
                    //return;
                }
                if (curTouchCard.isHoldM)
                {
                    //end hover
                    curTouchCard.OnEndHold();

                }
                    //end drag , onn mouseup
                    //khong biet touch time 0.3 co can k , tam thoi giu de tranh spam 
                    curTouchCard.OnTouchEnd();
                UIManager.instance.ClosePreviewHandCard();
                curTouchCard = null;
            }
            if (touch.phase == TouchPhase.Moved && curTouchCard != null)
            {
                //drag = false=> begin drag , drag=true -> dragging
                if (Time.time - touchTime > 0.1f)
                {
                    curTouchCard.OnTouchHold();
                    curTouchCard.OnTouchMove();
                    //dragging
                }
            }
        }
#endif

    }

    private void ProcessQueue()
    {
        if (!onProcessData && mQueue.Count > 0)
        {
            DataQueue q = mQueue[0];
            ProcessDataQueue(q.serviceId, q.data);

            mQueue.RemoveAt(0);
        }
    }

    public bool ProcessDataQueue(int serviceId, byte[] data)
    {

        switch (serviceId)
        {
            case IService.GAME_DEAL_CARDS:
                {
                    ListCommonVector lstCommonVector = ISocket.Parse<ListCommonVector>(data);
                    StartCoroutine(GameDealCards(lstCommonVector));

                    break;
                }

            case IService.GAME_MULLIGAN:
                {
                    CommonVector commonVector = ISocket.Parse<CommonVector>(data);
                    StartCoroutine(GameCardMulligan(commonVector));
                    break;
                }

            case IService.GAME_FIRST_GOD_SUMMON:
                {
                    CommonVector commonVector = ISocket.Parse<CommonVector>(data);
                    StartCoroutine(GameFirstGodSummon(commonVector));
                    break;
                }

            case IService.GAME_MOVE_GOD_SUMMON:
                {
                    CommonVector commonVector = ISocket.Parse<CommonVector>(data);
                    StartCoroutine(GameMoveGodSumon(commonVector));
                    break;
                }

            case IService.GAME_STARTUP_CONFIRM:
                {
                    CommonVector commonVector = ISocket.Parse<CommonVector>(data);
                    StartCoroutine(GameStartupConfirm(commonVector));
                    break;
                }

            case IService.GAME_STARTUP_END:
                {
                    ListCommonVector listCommonVector = ISocket.Parse<ListCommonVector>(data);
                    StartCoroutine(GameStartupEnd(listCommonVector));
                    break;
                }

            case IService.GAME_START_BATTLE:
                {

                    ListAction listAction = ISocket.Parse<ListAction>(data);
                    StartCoroutine(GameStartBattle(listAction));

                    break;
                }
            case IService.GAME_START_BID:
                {
                    CommonVector commonVector = ISocket.Parse<CommonVector>(data);

                    WriteLogBattle("GAME_START_BID: ", string.Join(",", commonVector.aString), string.Join(",", commonVector.aLong));
                    StartCoroutine(GameStartBid(commonVector));
                    break;
                }
            case IService.GAME_UP_BID:
                {
                    CommonVector commonVector = ISocket.Parse<CommonVector>(data);
                    StartCoroutine(GameUpBid(commonVector));   
                    
                    break;                             
                }
            case IService.GAME_BID_RESULT:
                {
                    ListAction listAction = ISocket.Parse<ListAction>(data);
                    StartCoroutine(GameBidResult(listAction));
                    break;
                }
            case IService.GAME_DELETE_CARDS:
                {
                    CommonVector commonVector = ISocket.Parse<CommonVector>(data);
                    StartCoroutine(DeleteCardsOnHand(commonVector, true));
                    break;
                }

            case IService.GAME_MOVE_CARD_IN_BATTLE:
                {
                    ListAction listAction = ISocket.Parse<ListAction>(data);
                    StartCoroutine(GameMoveCardInBattle(listAction));
                    break;
                }

            case IService.GAME_SUMMON_CARD_IN_BATTLE:
                {
                    ListAction listAction = ISocket.Parse<ListAction>(data);
                    StartCoroutine(GameSumonCardInBattle(listAction));
                    break;
                }

            case IService.GAME_CONFIRM_STARTBATTLE:
                {
                    ListAction listAction = ISocket.Parse<ListAction>(data);
                    StartCoroutine(GameConfirmStartBattle(listAction));
                    break;
                }

            case IService.GAME_CHOOSE_WAY_REQUEST:
                {

                    CommonVector commonVector = ISocket.Parse<CommonVector>(data);
                    StartCoroutine(GameChooseWayRequest(commonVector));
                    break;
                }

            case IService.GAME_SIMULATE_BATTLE:
                {
                    ListAction listAction = ISocket.Parse<ListAction>(data);
                    StartCoroutine(GameSimulateBattle(listAction));
                    break;
                }


            case IService.GAME_SIMULATE_CONFIRM:
                {
                    CommonVector commonVector = ISocket.Parse<CommonVector>(data);
                    GameSimulateConfirm(commonVector);
                    break;
                }

            case IService.GAME_BATTLE_ENDROUND:
                {
                    ListAction listAction = ISocket.Parse<ListAction>(data);
                    StartCoroutine(GameBattleEndRound(listAction));
                    break;
                }

            case IService.GAME_BATTLE_ENDGAME:
                {
                    ListCommonVector lstCommonVector = ISocket.Parse<ListCommonVector>(data);
                    StartCoroutine(GameBattleEndGame(lstCommonVector));
                    break;
                }

            case IService.GAME_SIMULATE_SKILLS:
            case IService.GAME_ACTIVE_SKILL:
                {
                    ListAction listAction = ISocket.Parse<ListAction>(data);

                    foreach (Action a in listAction.aAction)
                    {
                        switch (a.actionId)
                        {
                            case IService.GAME_STATUS_SKILL:
                                {
                                    CommonVector cv = ISocket.Parse<CommonVector>(a.data);
                                    if (cv.aLong[0] == 0)
                                    {
                                        Toast.Show(cv.aString[0]);
                                        SkillFailCondition();
                                    }
                                    else
                                    {
                                        long battleId = cv.aLong[1];
                                        long skillId = cv.aLong[2];

                                        foreach (Card card in GetListPlayerCardInBattle())
                                        {
                                            if (card.battleID == battleId && card.skill != null)
                                            {
                                                if (card.skill.id == skillId && card.skill.isUltiType)
                                                {
                                                    GameData.main.isUsedUlti = true;
                                                    isUseUltimate?.Invoke(card.heroID);
                                                    Toast.Show(LangHandler.Get("81", "Ultimate Activated!"));
                                                    // Transform target = IUtil.LoadPrefabRecycle("Prefabs/UltiVFX/", card.heroInfo.heroNumber.ToString(), ultiVfxContent);
                                                    Object target = CardData.Instance.GetUltiVfxPrefab(card.heroInfo.heroNumber.ToString());
                                                    if (target != null)
                                                    {
                                                        GameObject vfx = Instantiate(target, ultiVfxContent) as GameObject;

                                                        if (vfx != null)
                                                        {
                                                            vfx.transform.position = Vector3.zero;
                                                            vfx.transform.localScale = Vector3.one;
                                                        }
                                                    }
                                                    StartCoroutine(WaitDisplayUltiVfx());
                                                }
                                                else if (card.skill.id == skillId && !card.skill.isUltiType)
                                                {
                                                    card.countDoActiveSkill++;
                                                    onActiveSkillActive?.Invoke(skillId, battleId);
                                                }
                                            }
                                        }
                                        //truyen heronumber vao 

                                    }
                                    break;
                                }

                            case IService.GAME_SIMULATE_SKILLS_ON_BATTLE:

                                ListAction listActionSkill = ISocket.Parse<ListAction>(a.data);
                                //StartCoroutine(SimulateSkillEffect(listActionSkill, false));
                                foreach (Action ac in listActionSkill.aAction)
                                {
                                    lstSkillQueue.Add(ac);
                                }


                                break;


                        }
                    }
                    break;
                }

            case IService.GAME_UPDATE_HERO_MATRIC:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    WriteLogBattle("UPDATE_HERO_MATRIX: ", string.Join(",", cv.aLong), string.Join(",", cv.aString));
                    StartCoroutine(UpdateHeroMatric(cv, false));
                    break;
                }
            case IService.GET_PROFILE:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    if (cv.aLong[0] <= 10)
                    {
                        string totalMatch = cv.aLong[0].ToString();
                        string eventReward = "NB_REWARDED_" + totalMatch;
                        string paramName = "get_reward_battle_" + totalMatch;
                        if (!PlayerPrefs.HasKey(eventReward))
                        {
                            PlayerPrefs.SetInt(eventReward, 1);
                            ITrackingParameter pr = new ITrackingParameter() { name = paramName, value = "true" };
                            ITracking.LogEventFirebase(eventReward, pr);
                        }
                    }
                    break;
                }
        }
        return false;
    }
    private IEnumerator WaitDisplayUltiVfx()
    {
        CameraShaker.Instance.ShakeOnce(1.5f, 7, .1f, 5f);
        isWattingForVFX = true;
        yield return new WaitForSeconds(4.4f);

        isWattingForVFX = false;
    }
    private IEnumerator UpdateHeroMatric(CommonVector cv, bool attached)
    {
        yield return new WaitForSeconds(0f);
        onProcessData = true;

        //[batleId,atk,hp,hpMax,cleave.Pierce, Breaker, Combo, Overrun, Shield, shard]
        int BLOCK = 11;
        int num = cv.aLong.Count / BLOCK;

        WriteLogBattle("UpdateHeroMatric", GameData.main.profile.username, string.Join(",", cv.aLong));
        List<long> lstBuffCards = new List<long>();

        for (int i = 0; i < num; i++)
        {
            int start = 12;
            long buffSize = cv.aLong[i * BLOCK + 11];
            for (int j = 0; j < buffSize; j++)
            {
                lstBuffCards.Add(cv.aLong[start + j]);
            }
            //update hero matric
            //35,2,5,5,0,0,0,0,0,0,1
            BoardCard card = lstCardInBattle.FirstOrDefault(x => x.battleID == cv.aLong[i * BLOCK]);
            if (card != null)
            {
                card.UpdateHeroMatrix(cv.aLong[i * BLOCK + 1], cv.aLong[i * BLOCK + 2], cv.aLong[i * BLOCK + 3], cv.aLong[i * BLOCK + 4], cv.aLong[i * BLOCK + 5],
                    cv.aLong[i * BLOCK + 6], cv.aLong[i * BLOCK + 7], cv.aLong[i * BLOCK + 8], cv.aLong[i * BLOCK + 9], cv.aLong[i * BLOCK + 10], 0, 0, lstBuffCards);
            }
        }

        yield return new WaitForSeconds(0.5f);
        if (!attached)
            onProcessData = false;
    }


    public List<BoardCard> GetListPlayerCardInBattle()
    {
        var lst = lstCardInBattle.Where(c => c.cardOwner == CardOwner.Player);
        return lst.ToList();
    }

    public List<BoardCard> GetListEnemyCardInBattle()
    {
        var lst = lstCardInBattle.Where(c => c.cardOwner == CardOwner.Enemy);
        return lst.ToList();
    }

    IEnumerator SimulateSkillEffectSingle(Action action)
    {
        canContinue = false;
        yield return new WaitForSeconds(0);
        switch (action.actionId)
        {
            case IService.GAME_SKILL_EFFECT:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(action.data);
                    WriteLogBattle("GAME_SKILL_EFFECT: ", string.Join(",", cv.aString), string.Join(",", cv.aLong));
                    //if (cv.aLong[0] == 0)
                    //{
                    //    Toast.Show(cv.aString[0]);
                    //    break;
                    //}

                    //yield return new WaitForSeconds(0.5f);

                    OnEndSkillPhase();
                    if (cv.aLong.Count > 0)
                    {

                        long skillId = cv.aLong[0];
                        long effectId = cv.aLong[1];
                        long battleId = cv.aLong[2];
                        bool isBidSkill = cv.aLong[3]==1;
                        long clientIndexPlayer = 0;
                        foreach (Card c in lstCardInBattle)
                            if (c.battleID == battleId)
                            {
                                if (c.cardOwner == CardOwner.Player)
                                    clientIndexPlayer = 0;
                                else
                                    clientIndexPlayer = 1;
                            }

                        var buffCard = lstCardInBattle.FirstOrDefault(x => x.battleID == battleId);
                        if (buffCard != null)
                        {
                            if (buffCard.cardOwner == CardOwner.Player)
                                clientIndexPlayer = 0;
                            else
                                clientIndexPlayer = 1;
                        }
                        else
                        {
                            yield return new WaitForSeconds(0.5f);
                            buffCard = lstCardInBattle.FirstOrDefault(x => x.battleID == battleId);
                            if (buffCard != null)
                            {
                                if (buffCard.cardOwner == CardOwner.Player)
                                    clientIndexPlayer = 0;
                                else
                                    clientIndexPlayer = 1;
                            }
                        }
                        if(isBidSkill)
                        {
                            yield return new WaitForSeconds(3f);
                        }    

                        LogWriterHandle.WriteLog("GAME_SKILL_EFFECT 78: " + string.Join(",", cv.aLong) + "|" + clientIndexPlayer);
                        WriteLogBattle("GAME_SKILL_EFFECT 78:", GameData.main.profile.username, string.Join(",", cv.aLong));
                        //foreach (Card card in GetListPlayerCardInBattle())
                        //{
                        //    if (card.battleID == battleId && card.skill != null)
                        //    {
                        //        if (card.skill.id == skillId && card.skill.isUltiType)
                        //        {

                        //            Debug.Log("wait for done vfx");
                        //            yield return new WaitForSeconds(4.4f);
                        //            //GameData.main.isUsedUlti = true;
                        //            //isUseUltimate?.Invoke(card.heroID);
                        //            //Toast.Show(LangHandler.Get("81", "Ultimate Activated!"));
                        //        }
                        //        //else if (card.skill.id == skillId && !card.skill.isUltiType)
                        //        //{
                        //        //    card.countDoActiveSkill++;
                        //        //    onActiveSkillActive?.Invoke(skillId, battleId);
                        //        //}
                        //    }
                        //}
                        SkillSound soundSkill = CardData.Instance.GetCardSkillSound(skillId);
                        if (soundSkill != null)
                        {
                            string sound = soundSkill.soundID;
                            SoundHandler.main.PlaySFX(sound, "soundvfx");
                        }
                        switch ((int)effectId)
                        {
                            case (int)DBHeroSkill.EFFECT_BUFF_HP:
                                {
                                    //buff hp for list hero
                                    //[skillId, type_eff, battleId,[valueHpBuff1, buffBatleId1, heroHp1, HeroHpmax1]]
                                    //GAME_SKILL_EFFECT 78: 40,2,72,5,72,4,5
                                    //GAME_SKILL_EFFECT 78: 5,2,6,1,2,2,4,5
                                    int START = 4;
                                    int BLOCK = 4;
                                    int num = (cv.aLong.Count - START) / BLOCK;
                                    for (int i = 0; i < num; i++)
                                    {
                                        long valueHpBuff = cv.aLong[START + i * BLOCK];
                                        long buffBatleId = cv.aLong[START + i * BLOCK + 1];
                                        long heroHp = cv.aLong[START + i * BLOCK + 2];
                                        long heroHpmax = cv.aLong[START + i * BLOCK + 3];
                                        //Do buff HP ....
                                        ProjectileDataInfo info = ProjectileData.Instance.projectileInfo.FirstOrDefault(x => x.skillID == skillId && x.effectID == effectId);
                                        if (buffBatleId < 0)
                                        {
                                            var lstTower = lstTowerInBattle.Where(t => buffBatleId < -10 ? t.pos == GetClientPosFromServerPos(1) : t.pos == GetClientPosFromServerPos(0));
                                            long id = buffBatleId < -10 ? (long)Mathf.Abs(buffBatleId) - 11 : (long)Mathf.Abs(buffBatleId) - 1;
                                            var tower = lstTower.FirstOrDefault(t => t.id == id);

                                            if (tower != null)
                                            {

                                                if (info != null && buffCard != null)
                                                {
                                                    buffCard.OnCastSkill(skillId, effectId, tower.gameObject, () =>
                                                    {
                                                        tower.OnHealing(heroHpmax, valueHpBuff);
                                                    });
                                                }
                                                else
                                                    tower.OnHealing(heroHpmax, valueHpBuff);
                                            }
                                        }
                                        else
                                        {
                                            var card = lstCardInBattle.FirstOrDefault(x => x.battleID == buffBatleId);
                                            if (skillId == 40)
                                            {
                                                if (buffCard != null)
                                                {

                                                    buffCard.OnGlory(() =>
                                                    {
                                                        if (card != null)
                                                        {
                                                            if (info != null && buffCard != null)
                                                            {
                                                                buffCard.OnCastSkill(skillId, effectId, card.gameObject, () =>
                                                                {
                                                                    card.OnHealing(valueHpBuff, heroHp, heroHpmax);
                                                                });
                                                            }
                                                            else
                                                                card.OnHealing(valueHpBuff, heroHp, heroHpmax);
                                                        }
                                                    });
                                                }
                                            }
                                            else
                                            {
                                                if (card != null)
                                                {
                                                    if (info != null && buffCard != null)
                                                    {
                                                        buffCard.OnCastSkill(skillId, effectId, card.gameObject, () =>
                                                        {
                                                            card.OnHealing(valueHpBuff, heroHp, heroHpmax);
                                                        });
                                                    }
                                                    else
                                                        card.OnHealing(valueHpBuff, heroHp, heroHpmax);
                                                }
                                            }
                                        }
                                    }

                                    //@Chau GAME_SKILL_EFFECT 78: 156,2,59,3,-3,15,15|0 hồi máu cho trụ

                                    break;
                                }
                            case (int)DBHeroSkill.EFFECT_TMP_INCREASE_ATK_AND_HP:
                                {
                                    //increase TMP ATK and HP
                                    //[skillId,type_eff,battleId,[valueAtkBuff1,valueHpBuff1,buffBatleId1,heroAtk1,heroHp1,HeroHpmax1]]
                                    //38,0,32,32,1,0,3,3,3
                                    int START = 4;
                                    int BLOCK = 6;
                                    int num = (cv.aLong.Count - START) / BLOCK;
                                    ProjectileDataInfo info = ProjectileData.Instance.projectileInfo.FirstOrDefault(x => x.skillID == skillId && x.effectID == effectId);
                                    for (int i = 0; i < num; i++)
                                    {

                                        long valueAtkBuff = cv.aLong[START + i * BLOCK];
                                        long valueHpBuff = cv.aLong[START + i * BLOCK + 1];
                                        long buffBatleId = cv.aLong[START + i * BLOCK + 2];
                                        long heroAtk = cv.aLong[START + i * BLOCK + 3];
                                        long heroHp = cv.aLong[START + i * BLOCK + 4];
                                        long HeroHpmax = cv.aLong[START + i * BLOCK + 5];
                                        //Do increse TMP ATK buff HP ....
                                        var card = lstCardInBattle.FirstOrDefault(x => x.battleID == buffBatleId);
                                        LogWriterHandle.WriteLog("EFFECT_TMP_INCREASE_ATK_AND_HP = " + buffBatleId + " | " + valueAtkBuff + " | " + valueHpBuff + "|" + (card == null ? "NULL" : "NOT NULL"));
                                        if (card != null)
                                        {
                                            if (info != null && buffCard != null)
                                            {
                                                LogWriterHandle.WriteLog(buffCard.gameObject.name);
                                                buffCard.OnCastSkill(skillId, effectId, card.gameObject, () =>
                                                {
                                                    card.OnBuffEffect(valueAtkBuff, valueHpBuff, heroAtk, heroHp, HeroHpmax);
                                                });
                                            }
                                            else
                                                card.OnBuffEffect(valueAtkBuff, valueHpBuff, heroAtk, heroHp, HeroHpmax);

                                        }
                                    }

                                    yield return new WaitForSeconds(0.3f);

                                    break;
                                }
                            case (int)DBHeroSkill.EFFECT_INCREASE_ATK_AND_HP:
                                {
                                    //increase ATK and HP
                                    //[skillId,type_eff,battleId,[valueAtkBuff1,valueHpBuff1,buffBatleId1,heroAtk1,heroHp1,HeroHpmax1]]
                                    int START = 4;
                                    int BLOCK = 6;
                                    int num = (cv.aLong.Count - START) / BLOCK;
                                    ProjectileDataInfo info = ProjectileData.Instance.projectileInfo.FirstOrDefault(x => x.skillID == skillId && x.effectID == effectId);
                                    for (int i = 0; i < num; i++)
                                    {
                                        long valueAtkBuff = cv.aLong[START + i * BLOCK];
                                        long valueHpBuff = cv.aLong[START + i * BLOCK + 1];
                                        long buffBatleId = cv.aLong[START + i * BLOCK + 2];
                                        long heroAtk = cv.aLong[START + i * BLOCK + 3];
                                        long heroHp = cv.aLong[START + i * BLOCK + 4];
                                        long HeroHpmax = cv.aLong[START + i * BLOCK + 5];
                                        //Do increse ATK buff HP ....
                                        var card = lstCardInBattle.FirstOrDefault(x => x.battleID == buffBatleId);


                                        // Bai duoc buff co the tao ra tu eff type 11 duoc tra dong thoi voi eff buff type 1 -> wait time roi tim lai card.
                                        float waitTimeIfCardCreateOnEffect = 0f;
                                        if(card != null)
                                        {
                                            waitTimeIfCardCreateOnEffect = 1;
                                        }    
                                        yield return new WaitForSeconds(waitTimeIfCardCreateOnEffect);
                                        if(waitTimeIfCardCreateOnEffect>0)
                                            card = lstCardInBattle.FirstOrDefault(x => x.battleID == buffBatleId);
                                        LogWriterHandle.WriteLog("EFFECT_TMP_INCREASE_ATK_AND_HP = " + buffBatleId + " | " + valueAtkBuff + " | " + valueHpBuff + "|" + (card == null ? "NULL" : "NOT NULL"));
                                        // do glory
                                        if (skillId == 27 || skillId == 41 || skillId == 35)
                                        {

                                            if (buffCard != null)
                                            {
                                                buffCard.OnGlory(() =>
                                                {
                                                    if (card != null)
                                                    {
                                                        //ProjectileDataInfo info = ProjectileData.Instance.projectileInfo.FirstOrDefault(x => x.skillID == skillId && x.effectID == effectId);
                                                        if (info != null && buffCard != null)
                                                        {
                                                            buffCard.OnCastSkill(skillId, effectId, card.gameObject, () =>
                                                        {
                                                            card.OnBuffEffect(valueAtkBuff, valueHpBuff, heroAtk, heroHp, HeroHpmax);
                                                        });
                                                        }
                                                        else
                                                        {
                                                            card.OnBuffEffect(valueAtkBuff, valueHpBuff, heroAtk, heroHp, HeroHpmax);
                                                        }
                                                    }
                                                });
                                            }
                                        }
                                        else
                                        {
                                            if (card != null)
                                            {
                                                if (info != null && buffCard != null)
                                                {
                                                    buffCard.OnCastSkill(skillId, effectId, card.gameObject, () =>
                                                    {
                                                        card.OnBuffEffect(valueAtkBuff, valueHpBuff, heroAtk, heroHp, HeroHpmax);
                                                    });

                                                }
                                                else
                                                {
                                                    card.OnBuffEffect(valueAtkBuff, valueHpBuff, heroAtk, heroHp, HeroHpmax);
                                                }
                                            }
                                        }
                                    }
                                    break;
                                }
                            case (int)DBHeroSkill.EFFECT_MANA_CREATE_SHARD:
                                {
                                    break;
                                }
                            case (int)DBHeroSkill.EFFECT_MOVE_HERO:
                                {
                                    long serverIndexPlayer = cv.aLong[4];
                                    CardSlot targetSlot1 = GetSlot(IsMeByServerPos(serverIndexPlayer) ? SlotType.Player : SlotType.Enemy, cv.aLong[6], cv.aLong[7]);
                                    CardSlot targetSlot2 = GetSlot(IsMeByServerPos(serverIndexPlayer) ? SlotType.Player : SlotType.Enemy, cv.aLong[9], cv.aLong[10]);
                                    BoardCard card1 = GetBoardCard(cv.aLong[5]);
                                    BoardCard card2 = GetBoardCard(cv.aLong[8]);
                                    card1.MoveToSlot(targetSlot1);
                                    card2.MoveToSlot(targetSlot2);
                                    break;
                                }
                            case (int)DBHeroSkill.EFFECT_DRAW_CARD: 
                                {
                                    long serverIndexPlayer = cv.aLong[4];
                                    int BLOCK = 16, START = 5;
                                    int num = (cv.aLong.Count - START) / BLOCK;
                                    //GAME_SKILL_EFFECT 78: 12,5,47,44,3,1,1,1,0,0,1,0,0,0,0,0
                                    ProjectileDataInfo info = ProjectileData.Instance.projectileInfo.FirstOrDefault(x => x.skillID == skillId && x.effectID == effectId);
                                    for (int i = 0; i < num; i++)
                                    {
                                        long newBattleId = cv.aLong[START + i * BLOCK];
                                        long newHeroId = cv.aLong[START + i * BLOCK + 1];
                                        long frame = cv.aLong[START + i * BLOCK + 2];
                                        long atk = cv.aLong[START + i * BLOCK + 3];
                                        long hp = cv.aLong[START + i * BLOCK + 4];
                                        long fragile = cv.aLong[START + i * BLOCK + 6];
                                        long fleeting = cv.aLong[START + i * BLOCK + 7];
                                        long cardMana = cv.aLong[START + i * BLOCK + 15];
                                        //add heroid --> check fragile va precide
                                        //Do draw card ....
                                        if (info != null && buffCard != null)
                                        {
                                            buffCard.OnCastSkill(skillId, effectId, null, () => { });
                                        }
                                        if (IsMeByServerPos(serverIndexPlayer))
                                        {
                                            DBHero hero = Database.GetHero(newHeroId);
                                            AddNewCard(0, hero, newBattleId, frame, atk, hp, cardMana, false, card =>
                                            {
                                                card.isFleeting = fleeting != 0;
                                            });
                                        }
                                        else
                                        {
                                            DBHero hero = new DBHero
                                            {
                                                id = -1
                                            };
                                            AddNewCard(1, hero, -1, 1, 0, 0, 0);
                                        }
                                    }
                                    break;
                                }
                            case (int)DBHeroSkill.EFFECT_DEAL_DAME:
                                {
                                    //deal dame fountain
                                    //-----------GAME_SKILL = 0,6,42,///-11,1,14
                                    //GAME_SKILL_EFFECT 78: 43,6,36,41,1,4,5,41,1,3,5,41,1,2,5
                                    //12,6,47,40,1,5,6
                                    //11,6,27,-11,2,13
                                    //GAME_SKILL_EFFECT 78: 83,6,41,3,1,3,6,14,1,1,2,16,1,0,1 | 0
                                    int START = 4;
                                    int BLOCK = 4;
                                    int num = (cv.aLong.Count - START) / BLOCK;
                                    float waitTime = 0;
                                    bool isAtkCardDead = false;
                                    //yield return new WaitForSeconds(0.5f);
                                    yield return new WaitForSeconds(0f);
                                    var attackCard = lstCardInBattle.FirstOrDefault(x => x.battleID == cv.aLong[2]);
                                    for (int i = 0; i < num; i++)
                                    {
                                        long battleiD = cv.aLong[START + i * BLOCK];
                                        long deal = cv.aLong[START + i * BLOCK + 1];
                                        long newhp = cv.aLong[START + i * BLOCK + 2];
                                        if (attackCard != null)
                                        {
                                            if (battleiD < 0)
                                            {
                                                var lstTower = lstTowerInBattle.Where(t => battleiD < -10 ? t.pos == GetClientPosFromServerPos(1) : t.pos == GetClientPosFromServerPos(0));
                                                long id = battleiD < -10 ? (long)Mathf.Abs(battleiD) - 11 : (long)Mathf.Abs(battleiD) - 1;
                                                var defTower = lstTower.FirstOrDefault(t => t.id == id);
                                                if (defTower != null)
                                                {
                                                    ProjectileDataInfo info = ProjectileData.Instance.projectileInfo.FirstOrDefault(x => x.skillID == skillId && x.effectID == effectId);
                                                    if (info != null)
                                                    {
                                                        if (skillId == 42)
                                                        {
                                                            for (int j = 0; j < deal; j++)
                                                            {
                                                                attackCard.OnCastSkill(skillId, effectId, defTower.gameObject, () =>
                                                                {
                                                                    TowerDealDamage(battleiD, deal / deal, newhp);
                                                                });
                                                                yield return new WaitForSeconds(0.5f);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            attackCard.OnCastSkill(skillId, effectId, defTower.gameObject, () =>
                                                            {
                                                                TowerDealDamage(battleiD, deal / deal, newhp);
                                                            });
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (attackCard != null)
                                                            attackCard.OnAttackWithSkill(defTower.transform, delegate { TowerDealDamage(battleiD, deal, newhp); }, out waitTime);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var card = lstCardInBattle.FirstOrDefault(x => x.battleID == battleiD);
                                                if (card != null)
                                                {
                                                    if (attackCard != null)
                                                    {
                                                        ProjectileDataInfo info = ProjectileData.Instance.projectileInfo.FirstOrDefault(x => x.skillID == skillId && x.effectID == effectId);
                                                        if (info != null)
                                                        {
                                                            attackCard.OnCastSkill(skillId, effectId, card.gameObject, () =>
                                                            {
                                                                card.OnDamaged(deal, newhp);
                                                                if (newhp <= 0)
                                                                {
                                                                    if (card.battleID == attackCard.battleID)
                                                                    {
                                                                        isAtkCardDead = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        card.OnDeath();
                                                                        SoundHandler.main.PlaySFX("BrokenCard1", "sounds");
                                                                        lstCardInBattle.Remove(card);
                                                                    }
                                                                }
                                                            });
                                                        }
                                                        else
                                                        {
                                                            attackCard.OnAttackWithSkill(card.transform, () =>
                                                            {
                                                                card.OnDamaged(deal, newhp);
                                                                if (newhp <= 0)
                                                                {
                                                                    if (card.battleID == attackCard.battleID)
                                                                    {
                                                                        isAtkCardDead = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        card.OnDeath();
                                                                        SoundHandler.main.PlaySFX("BrokenCard1", "sounds");
                                                                        lstCardInBattle.Remove(card);
                                                                    }
                                                                }
                                                            }, out waitTime);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        card.OnDamaged(deal, newhp);
                                                        if (newhp <= 0)
                                                        {
                                                            card.OnDeath();
                                                            SoundHandler.main.PlaySFX("BrokenCard1", "sounds");
                                                            lstCardInBattle.Remove(card);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ProjectileDataInfo info = ProjectileData.Instance.projectileInfo.FirstOrDefault(x => x.skillID == skillId && x.effectID == effectId);
                                            if (battleiD < 0)
                                            {
                                                var lstTower = lstTowerInBattle.Where(t => battleiD < -10 ? t.pos == GetClientPosFromServerPos(1) : t.pos == GetClientPosFromServerPos(0));
                                                long id = battleiD < -10 ? (long)Mathf.Abs(battleiD) - 11 : (long)Mathf.Abs(battleiD) - 1;
                                                var defTower = lstTower.FirstOrDefault(t => t.id == id);
                                                if (defTower != null)
                                                {
                                                    // long seige
                                                    if (skillId == 42 && attackCard.heroID == 40)
                                                    {
                                                        for (int j = 0; j < deal; j++)
                                                        {
                                                            TowerDealDamage(battleiD, deal / deal, newhp);
                                                            yield return new WaitForSeconds(0.5f);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        TowerDealDamage(battleiD, deal, newhp);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var card = lstCardInBattle.FirstOrDefault(x => x.battleID == battleiD);
                                                if (card != null)
                                                {
                                                    card.OnCastSkill(skillId, effectId, card.gameObject, () =>
                                                     {
                                                         card.OnDamaged(deal, newhp);
                                                         if (newhp <= 0)
                                                         {
                                                             card.OnDeath();
                                                             SoundHandler.main.PlaySFX("BrokenCard1", "sounds");
                                                             lstCardInBattle.Remove(card);
                                                         }
                                                     });
                                                    
                                                }
                                            }
                                        }
                                        waitTimeToBattle += waitTime;
                                        yield return new WaitForSeconds(0.1f);
                                    }
                                    yield return new WaitForSeconds(waitTime);
                                    if (isAtkCardDead)
                                    {
                                        attackCard.OnDeath();
                                        SoundHandler.main.PlaySFX("BrokenCard1", "sounds");
                                        lstCardInBattle.Remove(attackCard);
                                    }
                                }
                                break;
                            case (int)DBHeroSkill.EFFECT_INCREASE_SPECIAL_PARAM:
                                {

                                    //increase special param
                                    //[skillId,type_eff, battleId,[battleId1,cleave1Add,Pierce1Add, Breaker1Add, Combo1Add, Overrun1Add, Shield1Add, godSlayerAdd cleave1, Pierce1, Breaker1, Combo1, Overrun1, Shield1, GodSplayer]]
                                    int START = 4;
                                    int BLOCK = 18;
                                    int num = (cv.aLong.Count - START) / BLOCK;
                                    ProjectileDataInfo info = ProjectileData.Instance.projectileInfo.FirstOrDefault(x => x.skillID == skillId && x.effectID == effectId);
                                    //if (skillId == 30 || skillId == 214)
                                    //{
                                    //    ultimateRenderTexture.gameObject.SetActive(true);
                                    //    ultimateVideo.clip = CardData.Instance.GetVideo("Ares");
                                    //    ultimateVideo.gameObject.SetActive(true);
                                    //    ultimateVideo.Play();
                                    //    yield return new WaitForSeconds((float)ultimateVideo.length);
                                    //    ultimateRenderTexture.SetActive(false);
                                    //    ultimateVideo.gameObject.SetActive(false);
                                    //}

                                    for (int i = 0; i < num; i++)
                                    {
                                        long battleIdReceive = cv.aLong[START + i * BLOCK];
                                        long cleaveAdd = cv.aLong[START + i * BLOCK + 1];
                                        long pierceAdd = cv.aLong[START + i * BLOCK + 2];
                                        long breakerAdd = cv.aLong[START + i * BLOCK + 3];
                                        long combo1Add = cv.aLong[START + i * BLOCK + 4];
                                        long overrun1Add = cv.aLong[START + i * BLOCK + 5];
                                        long shield1Add = cv.aLong[START + i * BLOCK + 6];
                                        long godSlayerAdd = cv.aLong[START + i * BLOCK + 7];
                                        long rootAdd = cv.aLong[START + i * BLOCK + 8];
                                        long cleave = cv.aLong[START + i * BLOCK + 9];
                                        long pierce = cv.aLong[START + i * BLOCK + 10];
                                        long breaker = cv.aLong[START + i * BLOCK + 11];
                                        long combo = cv.aLong[START + i * BLOCK + 12];
                                        long overrun = cv.aLong[START + i * BLOCK + 13];
                                        long shield = cv.aLong[START + i * BLOCK + 14];
                                        long godSlayer = cv.aLong[START + i * BLOCK + 15];
                                        long root = cv.aLong[START + i * BLOCK + 16];
                                        long tired = cv.aLong[START + i * BLOCK + 17];

                                        //Do increse special param ....
                                        // trước để wait 0.5 nhưng do eff hiện lên quá chậm. nếu lỗi thì chuyển lại 0.5
                                        yield return new WaitForSeconds(0.1f);
                                        var card = lstCardInBattle.FirstOrDefault(x => x.battleID == battleIdReceive);
                                        if (card != null)
                                        {
                                            if (info != null && buffCard != null)
                                            {
                                                buffCard.OnCastSkill(skillId, effectId, card.gameObject, () =>
                                                {
                                                    card.OnAddSpecialBuff(skillId, cleaveAdd, pierceAdd, breakerAdd, combo1Add, overrun1Add, shield1Add, godSlayerAdd, cleave, pierce, breaker, combo, overrun, shield, godSlayer);
                                                });
                                            }
                                            else
                                                card.OnAddSpecialBuff(skillId, cleaveAdd, pierceAdd, breakerAdd, combo1Add, overrun1Add, shield1Add, godSlayerAdd, cleave, pierce, breaker, combo, overrun, shield, godSlayer);
                                        }
                                    }
                                    break;
                                }
                            case (int)DBHeroSkill.EFFECT_TMP_INCREASE_SPECIAL_PARAM:
                                {
                                    //increase TMP special param
                                    //[skillId,type_eff, battleId,[battleId1,cleave1Add,Pierce1Add, Breaker1Add, Combo1Add, Overrun1Add, Shield1Add, GodSplayerAdd, cleave1, Pierce1, Breaker1, Combo1, Overrun1, Shield1, GodSplayer]]
                                    int START = 4;
                                    int BLOCK = 15;
                                    int num = (cv.aLong.Count - START) / BLOCK;
                                    ProjectileDataInfo info = ProjectileData.Instance.projectileInfo.FirstOrDefault(x => x.skillID == skillId && x.effectID == effectId);
                                    for (int i = 0; i < num; i++)
                                    {
                                        long battleIdReceive = cv.aLong[START + i * BLOCK];
                                        long cleaveAdd = cv.aLong[START + i * BLOCK + 1];
                                        long pierceAdd = cv.aLong[START + i * BLOCK + 2];
                                        long breakerAdd = cv.aLong[START + i * BLOCK + 3];
                                        long combo1Add = cv.aLong[START + i * BLOCK + 4];
                                        long overrun1Add = cv.aLong[START + i * BLOCK + 5];
                                        long shield1Add = cv.aLong[START + i * BLOCK + 6];
                                        long godSlayerAdd = cv.aLong[START + i * BLOCK + 7];

                                        long cleave = cv.aLong[START + i * BLOCK + 8];
                                        long pierce = cv.aLong[START + i * BLOCK + 9];
                                        long breaker = cv.aLong[START + i * BLOCK + 10];
                                        long combo = cv.aLong[START + i * BLOCK + 11];
                                        long overrun = cv.aLong[START + i * BLOCK + 12];
                                        long shield = cv.aLong[START + i * BLOCK + 13];
                                        long godSlayer = cv.aLong[START + i * BLOCK + 14];
                                        //Do increse TMP special param ....
                                        var card = lstCardInBattle.FirstOrDefault(x => x.battleID == battleIdReceive);
                                        if (card != null)
                                        {
                                            if (info != null && buffCard != null)
                                            {
                                                buffCard.OnCastSkill(skillId, effectId, card.gameObject, () =>
                                                {
                                                    card.OnAddSpecialBuff(skillId, cleaveAdd, pierceAdd, breakerAdd, combo1Add, overrun1Add, shield1Add, godSlayerAdd, cleave, pierce, breaker, combo, overrun, shield, godSlayer);
                                                });
                                            }
                                            else
                                                card.OnAddSpecialBuff(skillId, cleaveAdd, pierceAdd, breakerAdd, combo1Add, overrun1Add, shield1Add, godSlayerAdd, cleave, pierce, breaker, combo, overrun, shield, godSlayer);
                                        }
                                    }

                                    break;
                                }
                            case (int)DBHeroSkill.EFFECT_READY:
                                {
                                    //ready hero
                                    //GAME_SKILL_EFFECT 78: 59,9,19,19,1 | 0
                                    //68,9,57,3,1 | 0
                                    int START = 4;
                                    int BLOCK = 2;
                                    int num = (cv.aLong.Count - 4) / BLOCK;
                                    yield return new WaitForSeconds(0.2f);
                                    var attackCard = lstCardInBattle.FirstOrDefault(x => x.battleID == cv.aLong[2]);
                                    for (int i = 0; i < num; i++)
                                    {
                                        long battleIdReceive = cv.aLong[START + i * BLOCK];
                                        long isTired = cv.aLong[START + i * BLOCK + 1];
                                        // change state of hero
                                        var card = lstCardInBattle.FirstOrDefault(x => x.battleID == battleIdReceive);
                                        if (card != null)
                                        {
                                            if (attackCard != null)
                                            {
                                                ProjectileDataInfo info = ProjectileData.Instance.projectileInfo.FirstOrDefault(x => x.skillID == skillId && x.effectID == effectId);
                                                if (info != null)
                                                {
                                                    attackCard.OnCastSkill(skillId, effectId, card.gameObject, () =>
                                                    {
                                                        card.SetTired(isTired);
                                                        card.SetMovable(isTired == 0);
                                                    });
                                                }
                                                else
                                                {
                                                    card.SetTired(isTired);
                                                    card.SetMovable(isTired == 0);
                                                }
                                            }
                                            else
                                            {
                                                card.SetTired(isTired);
                                                card.SetMovable(isTired == 0);
                                            }
                                        }
                                    }
                                    break;
                                }
                            case (int)DBHeroSkill.EFFECT_FIGHT:
                                {
                                    long playerCardBattleID = cv.aLong[4];
                                    long enemyCardBattleID = cv.aLong[5];
                                    var playerCard = lstCardInBattle.FirstOrDefault(x => x.battleID == playerCardBattleID);
                                    var enemyCard = lstCardInBattle.FirstOrDefault(x => x.battleID == enemyCardBattleID);
                                    ProjectileDataInfo info = ProjectileData.Instance.projectileInfo.FirstOrDefault(x => x.skillID == skillId && x.effectID == effectId);
                                    if (skillId == 437 || skillId == 29)
                                        if (info != null && buffCard != null)
                                        {
                                            buffCard.OnCastSkill(skillId, effectId, enemyCard.gameObject, () =>
                                              {

                                              }, false);
                                        }

                                    break;
                                }
                            case (int)DBHeroSkill.EFFECT_SUMMON_VIRTUAL_HERO:
                                {

                                    //[skillId,type_eff, battleId,[BattleId1,heroId1,Atk1,Hp1,fragile1,Precide1,row1,colum1]]
                                    //GAME_SKILL_EFFECT 78: 17,11,38,75,45,1,5,0,0,0,1
                                    //fragile: tướng ở trên bàn chỉ tồn tại trong 1 turn hoặc 1 round
                                    //precide: quân bài ở trên tay chỉ tồn tại dc 1 turn

                                    long serverIndexPlayer = cv.aLong[4];

                                    //if (skillId == 17 || skillId == 234)
                                    //{
                                    //    ultimateRenderTexture.gameObject.SetActive(true);
                                    //    ultimateVideo.clip = CardData.Instance.GetVideo("Poseidon");
                                    //    ultimateVideo.gameObject.SetActive(true);
                                    //    ultimateVideo.Play();
                                    //    yield return new WaitForSeconds((float)ultimateVideo.length);
                                    //    ultimateRenderTexture.SetActive(false);
                                    //    ultimateVideo.gameObject.SetActive(false);
                                    //}

                                    int BLOCK = 11, START = 5;
                                    int num = (cv.aLong.Count - START) / BLOCK;
                                    ProjectileDataInfo info = ProjectileData.Instance.projectileInfo.FirstOrDefault(x => x.skillID == skillId && x.effectID == effectId);
                                    if (buffCard != null && info != null)
                                    {
                                        buffCard.OnCastSkill(skillId, effectId, null, () =>
                                        {
                                        });
                                    }
                                    for (int i = 0; i < num; i++)
                                    {
                                        long isDiscardCard = cv.aLong[START + i * BLOCK];
                                        long newBattleId = cv.aLong[START + i * BLOCK + 1];
                                        long heroId = cv.aLong[START + i * BLOCK + 2];
                                        long frame = cv.aLong[START + i * BLOCK + 3];
                                        long atk = cv.aLong[START + i * BLOCK + 4];
                                        long hp = cv.aLong[START + i * BLOCK + 5];
                                        long mana = cv.aLong[START + i * BLOCK + 6];
                                        long fragile = cv.aLong[START + i * BLOCK + 7];
                                        long fleeting = cv.aLong[START + i * BLOCK + 8];
                                        long row = cv.aLong[START + i * BLOCK + 9];
                                        long colum = cv.aLong[START + i * BLOCK + 10];
                                        //Do sumon virtual card
                                        WriteLogBattle("SUMMON_VIRTUAL_HERO: ", "BattleID: " + newBattleId + ", ROW: " + row + ", COL: " + colum + ", HEROID: " + heroId, "");
                                        CardSlot slot = null;
                                        Transform spawnEffect = null;
                                        float delayTime = 0f;
                                        GameObject objectToSpawn = null;
                                        CardOwner owner = CardOwner.Player;

                                        if (IsMeByServerPos(serverIndexPlayer))
                                        {
                                            slot = playerSlotContainer.FirstOrDefault(x => x.xPos == row && x.yPos == colum);
                                            objectToSpawn = m_MinionOnBoardCard;
                                            owner = CardOwner.Player;
                                        }
                                        else
                                        {
                                            slot = enemySlotContainer.FirstOrDefault(x => x.xPos == row && x.yPos == colum);
                                            objectToSpawn = m_EnemyMinionOnBoardCard;
                                            owner = CardOwner.Enemy;
                                        }

                                        if (slot != null)
                                        {
                                            if (buffCard != null)
                                            {
                                                if (buffCard.heroInfo.type == DBHero.TYPE_GOD)
                                                {
                                                    switch (skillId)
                                                    {
                                                        case 14:
                                                        case 16:
                                                        case 17:
                                                            {
                                                                spawnEffect = poseidonSummonEffect;
                                                                delayTime = 1f;
                                                                DefaultSpawn();
                                                                break;
                                                            }
                                                        case 233:
                                                        case 234:
                                                            {
                                                                spawnEffect = poseidonSummonEffect;
                                                                delayTime = 1f;
                                                                DefaultSpawn();
                                                                break;
                                                            }
                                                        case 126:
                                                        case 228:
                                                            {
                                                                spawnEffect = hadesSkillSummonEffect;
                                                                delayTime = 1.45f;
                                                                StartCoroutine(CreateCardEffectAnimation(newBattleId, heroId, frame, atk, hp, mana, objectToSpawn, slot.transform, spawnEffect, slot, owner, delayTime, (card) =>
                                                                 {
                                                                     UpdatHeroMatrixSummon(card);
                                                                 }));
                                                                break;
                                                            }
                                                        case 196:
                                                        case 283:
                                                            {
                                                                spawnEffect = sontinhSummonEffect;
                                                                delayTime = 1f;
                                                                DefaultSpawn();
                                                                break;
                                                            }
                                                        default:
                                                            {
                                                                spawnEffect = IsMeByServerPos(serverIndexPlayer) ? minionSkillSummonEffect : enemyMinionSkillSummonEffect;
                                                                delayTime = 0.5f;
                                                                DefaultSpawn();
                                                                break;
                                                            }
                                                    }
                                                }
                                                else
                                                {
                                                    spawnEffect = IsMeByServerPos(serverIndexPlayer) ? minionSkillSummonEffect : enemyMinionSkillSummonEffect;
                                                    delayTime = 0.5f;
                                                    DefaultSpawn();
                                                }
                                            }
                                            else
                                            {
                                                spawnEffect = IsMeByServerPos(serverIndexPlayer) ? minionSkillSummonEffect : enemyMinionSkillSummonEffect;
                                                delayTime = 0.5f;
                                                DefaultSpawn();
                                            }
                                        }
                                        void UpdatHeroMatrixSummon(BoardCard card)
                                        {
                                            card.UpdateHeroMatrix(atk, hp, hp, 0, 0, 0, 0, 0, 0, 0, 0);
                                            card.isFragile = fragile != 0;
                                            SoundHandler.main.PlaySFX("SummonCard", "sounds");
                                        }

                                        void DefaultSpawn()
                                        {
                                            StartCoroutine(CreateCardOnEffect(newBattleId, heroId, frame, atk, hp, mana, objectToSpawn, slot.transform, spawnEffect, slot, owner, delayTime, (card) =>
                                              {
                                                  UpdatHeroMatrixSummon(card);
                                                  if (isDiscardCard == 1)
                                                      card.OnDeath();

                                              }));
                                        }
                                    }

                                    break;
                                }
                            case (int)DBHeroSkill.EFFECT_SUMMON_CARD_IN_HAND:
                                {
                                    long serverIndexPlayer = cv.aLong[4];
                                    int BLOCK = 17, START = 5;
                                    int num = (cv.aLong.Count - START) / BLOCK;

                                    ProjectileDataInfo info = ProjectileData.Instance.projectileInfo.FirstOrDefault(x => x.skillID == skillId && x.effectID == effectId);
                                    //GAME_SKILL_EFFECT 78: 12,5,47,44,3,1,1,1,0,0,1,0,0,0,0,0
                                    //GAME_SKILL_EFFECT 78: 49,12,3,0,64,68,0,0,0,0,1,0,0,0,0,0,0 | 0
                                    for (int i = 0; i < num; i++)
                                    {
                                        long newBattleId = cv.aLong[START + i * BLOCK];
                                        long newHeroId = cv.aLong[START + i * BLOCK + 1];
                                        long frame = cv.aLong[START + i * BLOCK + 2];
                                        long atk = cv.aLong[START + i * BLOCK + 3];
                                        long hp = cv.aLong[START + i * BLOCK + 4];
                                        long hpMax = cv.aLong[START + i * BLOCK + 5];
                                        long fragile = cv.aLong[START + i * BLOCK + 6];
                                        long fleeting = cv.aLong[START + i * BLOCK + 7];
                                        long cleave = cv.aLong[START + i * BLOCK + 8];
                                        long pierce = cv.aLong[START + i * BLOCK + 9];
                                        long breaker = cv.aLong[START + i * BLOCK + 10];
                                        long combo = cv.aLong[START + i * BLOCK + 11];
                                        long overrun = cv.aLong[START + i * BLOCK + 12];
                                        long shield = cv.aLong[START + i * BLOCK + 13];
                                        long godslayer = cv.aLong[START + i * BLOCK + 14];
                                        long cardMana = cv.aLong[START + i * BLOCK + 15];
                                        long discardAnim = cv.aLong[START + i * BLOCK + 16];
                                        // anim discard

                                        //add heroid --> check fragile va precide
                                        //Do draw card ....
                                        if (info != null && buffCard != null)
                                        {
                                            buffCard.OnCastSkill(skillId, effectId, null, () =>
                                            {
                                                if (IsMeByServerPos(serverIndexPlayer))
                                                {
                                                    DBHero hero = Database.GetHero(newHeroId);
                                                    if (discardAnim == 0)
                                                    {
                                                        AddNewCard(0, hero, newBattleId, frame, atk, hp, cardMana, fleeting == 1, card =>
                                                        {
                                                            card.isFleeting = fleeting != 0;
                                                            card.OnUpdateManaText(cardMana);
                                                            //if (buffCard != null)
                                                            //    buffCard.SummonNewCard(card.transform.position, null);

                                                        });
                                                    }
                                                    else
                                                    {
                                                        haveCardToDelete = true;
                                                        DiscardANewCard(0, hero, newBattleId, frame, atk, hp, cardMana, fleeting == 1, card =>
                                                               {
                                                                   haveCardToDelete = false;
                                                                   foreach (HandCard c in Decks[0].GetListCard)
                                                                       c.isMoving = false;
                                                               });
                                                    }
                                                }
                                                else
                                                {
                                                    DBHero hero = new DBHero
                                                    {
                                                        id = -1
                                                    };
                                                    if (discardAnim == 0)
                                                    {

                                                        AddNewCard(1, hero, -1, 1, -1, -1, -1
                                                            , true);
                                                    }

                                                }
                                            });
                                        }
                                        else
                                        {
                                            if (IsMeByServerPos(serverIndexPlayer))
                                            {
                                                DBHero hero = Database.GetHero(newHeroId);
                                                if (discardAnim == 0)
                                                {
                                                    AddNewCard(0, hero, newBattleId, frame, atk, hp, cardMana, fleeting == 1, card =>
                                                    {
                                                        card.isFleeting = fleeting != 0;
                                                        card.OnUpdateManaText(cardMana);
                                                        //if (buffCard != null)
                                                        //    buffCard.SummonNewCard(card.transform.position, null);

                                                    });
                                                }
                                                else
                                                {
                                                    haveCardToDelete = true;
                                                    DiscardANewCard(0, hero, newBattleId, frame, atk, hp, cardMana, fleeting == 1, card =>
                                                    {
                                                        haveCardToDelete = false;
                                                        foreach (HandCard c in Decks[0].GetListCard)
                                                            c.isMoving = false;
                                                    });
                                                }
                                            }
                                            else
                                            {
                                                DBHero hero = new DBHero
                                                {
                                                    id = -1
                                                };
                                                if (discardAnim == 0)
                                                {

                                                    AddNewCard(1, hero, -1, 1, -1, -1, -1
                                                        , true);
                                                }
                                            }
                                        }
                                    }



                                    break;
                                }
                            case (int)DBHeroSkill.EFFECT_USER_MANA_MAX:
                                {
                                    long serverIndexPlayer = cv.aLong[4];
                                    //GAME_SKILL_EFFECT 78: 57,13,46,1,1,7 | 0
                                    //@Chau them mana cho user
                                    long addedMana = cv.aLong[4];
                                    long currentMana = cv.aLong[5];
                                    yield return new WaitForSeconds(1);
                                    onUpdateMana?.Invoke((int)GetClientPosFromServerPos(serverIndexPlayer), currentMana, ManaState.StartTurn, 0);
                                    break;
                                }
                            case (int)DBHeroSkill.EFFECT_LEAVE_CARD_IN_HAND:
                                {
                                    long serverIndexPlayer = cv.aLong[4];
                                    int BLOCK = 2, START = 5;
                                    int num = (cv.aLong.Count - START) / BLOCK;
                                    //GAME_SKILL_EFFECT 78: 72,14,27,1,44,78 | 0

                                    for (int i = 0; i < num; i++)
                                    {
                                        long deleteBattleId = cv.aLong[START + i * BLOCK];
                                        long deleteHeroId = cv.aLong[START + i * BLOCK + 1];

                                        var myCards = Decks[0].GetListCard;
                                        foreach (HandCard card in myCards)
                                            if (deleteBattleId == card.battleID)
                                            {
                                                card.DiscardHandCardPlayer(showOpponentCardPoint, () =>
                                                {
                                                    Decks[0].RemoveCard(card);
                                                    PoolManager.Pools["Card"].Despawn(card.transform);
                                                });

                                                break;
                                            }
                                    }
                                    Decks[0].ReBuildDeck();

                                    break;
                                }
                            case (int)DBHeroSkill.EFFECT_TMP_INCREASE_MANA_MAX:
                                {
                                    LogWriterHandle.WriteLog("EFFECT_TMP_INCREASE_MANA_MAX:" + cv.aLong[5] + " mana");
                                    long serverIndexPlayer = cv.aLong[3];
                                    onUpdateMana?.Invoke((int)GetClientPosFromServerPos(serverIndexPlayer), cv.aLong[5], ManaState.UseDone, 0);
                                    break;
                                }
                            case (int)DBHeroSkill.EFFECT_INCREASE_HERO_MANA:
                                {
                                    int BLOCK = 2, START = 4;
                                    int num = (cv.aLong.Count - START) / BLOCK;

                                    for (int i = 0; i < num; i++)
                                    {
                                        long battleID = cv.aLong[START + i * BLOCK];
                                        long additionMana = cv.aLong[START + i * BLOCK + 1];
                                        var card = Decks[0].GetListCard.FirstOrDefault(x => x.battleID == battleID);
                                        if (card != null)
                                            card.OnUpdateManaText(additionMana);
                                    }
                                    break;
                                }
                            case (int)DBHeroSkill.EFFECT_TMP_INCREASE_HERO_MANA:
                                {
                                    int BLOCK = 2, START = 4;
                                    int num = (cv.aLong.Count - START) / BLOCK;

                                    for (int i = 0; i < num; i++)
                                    {
                                        long battleID = cv.aLong[START + i * BLOCK];
                                        long additionMana = cv.aLong[START + i * BLOCK + 1];
                                        var card = Decks[0].GetListCard.FirstOrDefault(x => x.battleID == battleID);
                                        if (card != null)
                                            card.OnUpdateManaText(additionMana);
                                    }
                                    break;
                                }
                            case (int)DBHeroSkill.EFFECT_PLAY_ALL_COLOR_CARD:
                                {
                                    currentAvailableRegion = cv.aLong[4];
                                    break;
                                }
                            case (int)DBHeroSkill.EFFECT_PENATY:
                                {
                                    WriteLogBattle("EFFECT_PENATY: ", "", string.Join(",", cv.aLong));
                                    int BLOCK = 3, START = 4;
                                    int num = (cv.aLong.Count - START) / BLOCK;
                                    float waitTime = 0;
                                    yield return new WaitForSeconds(0.2f);
                                    //LogWriterHandle.WriteLog("than bi chet" + cv.aLong[2]);
                                    //var attackCard = lstCardInBattle.FirstOrDefault(x => x.battleID == cv.aLong[2]);
                                    // cv.along[2] la battle id cua than bi chet
                                    for (int i = 0; i < num; i++)
                                    {
                                        long battleiD = cv.aLong[START + i * BLOCK];
                                        long deal = cv.aLong[START + i * BLOCK + 1];
                                        long newhp = cv.aLong[START + i * BLOCK + 2];
                                        if (battleiD < 0)
                                        {
                                            var lstTower = lstTowerInBattle.Where(t => battleiD < -10 ? t.pos == GetClientPosFromServerPos(1) : t.pos == GetClientPosFromServerPos(0));
                                            long id = battleiD < -10 ? (long)Mathf.Abs(battleiD) - 11 : (long)Mathf.Abs(battleiD) - 1;
                                            var defTower = lstTower.FirstOrDefault(t => t.id == id);
                                            if (defTower != null)
                                            {
                                                ProjectileDataInfo info = ProjectileData.Instance.projectileInfo.FirstOrDefault(x => x.skillID == skillId);
                                                if (info != null)
                                                {
                                                    TowerDealDamage(battleiD, deal, newhp);
                                                }
                                                else
                                                {
                                                    TowerDealDamage(battleiD, deal, newhp);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // battle>0 -> card
                                            WriteLogBattle("EFFECT_PENATY: ", "", string.Join(",", cv.aLong));
                                        }
                                    }
                                    break;
                                }
                            case (int)DBHeroSkill.EFFECT_SWAP_MANA_ATK:
                                {
                                    WriteLogBattle("EFFECT_SWAP_MANA_ATK: ", "", string.Join(",", cv.aLong));
                                    int BLOCK = 4, START = 5;
                                    int num = (cv.aLong.Count - START) / BLOCK;
                                    long serverIndexPlayer = cv.aLong[4];
                                    if ((int)GetClientPosFromServerPos(serverIndexPlayer) == 0)
                                    {
                                        for (int i = 0; i < num; i++)
                                        {
                                            long cardBattleId = cv.aLong[START + i * BLOCK];
                                            long cardHeroId = cv.aLong[START + i * BLOCK + 1];
                                            long cardATK = cv.aLong[START + i * BLOCK + 2];
                                            long cardMana = cv.aLong[START + i * BLOCK + 3];

                                            var myCards = Decks[0].GetListCard;
                                            foreach (HandCard card in myCards)
                                                if (cardBattleId == card.battleID)
                                                {
                                                    card.OnUpdateAtkText(cardATK);
                                                    card.OnUpdateManaText(cardMana);

                                                    if (isBidSkill)
                                                        card.ShowEffectSkillBid();

                                                    break;
                                                }
                                        }
                                        //Decks[0].ReBuildDeck();
                                    }
                                    else
                                    {
                                        Transform  trans= PoolManager.Pools["Effect"].Spawn(m_EnemyHandcardSkillEffect, ultiVfxContent);
                                        trans.GetComponent<ParticleEffectParentCallback>().SetOnComplete(() =>
                                        {
                                            PoolManager.Pools["Effect"].Despawn(trans);
                                        });
                                        //Decks[1].ReBuildDeck();
                                    }
                                    break;
                                }
                            case (int)DBHeroSkill.EFFECT_DUPLICATE_CARD_IN_HAND:
                                {
                                    WriteLogBattle("EFFECT_DUPLICATE_CARD_IN_HAND: ", "", string.Join(",", cv.aLong));
                                    int BLOCK = 17, START = 5;
                                    int num = (cv.aLong.Count - START) / BLOCK;
                                    long serverIndexPlayer = cv.aLong[4];

                                    ProjectileDataInfo info = ProjectileData.Instance.projectileInfo.FirstOrDefault(x => x.skillID == skillId && x.effectID == effectId);
                                    if (isBidSkill&& !IsMeByServerPos(serverIndexPlayer))
                                    {
                                        // skill cho enemy chi hien thi chung 1 lan nen check rieng
                                        Transform trans = PoolManager.Pools["Effect"].Spawn(m_EnemyHandcardSkillEffect, ultiVfxContent);
                                        trans.GetComponent<ParticleEffectParentCallback>().SetOnComplete(() =>
                                        {
                                            PoolManager.Pools["Effect"].Despawn(trans);
                                        });
                                    }
                                        //GAME_SKILL_EFFECT 78: 12,5,47,44,3,1,1,1,0,0,1,0,0,0,0,0
                                        //GAME_SKILL_EFFECT 78: 49,12,3,0,64,68,0,0,0,0,1,0,0,0,0,0,0 | 0
                                        for (int i = 0; i < num; i++)
                                    {
                                        long newBattleId = cv.aLong[START + i * BLOCK];
                                        long newHeroId = cv.aLong[START + i * BLOCK + 1];
                                        long frame = cv.aLong[START + i * BLOCK + 2];
                                        long atk = cv.aLong[START + i * BLOCK + 3];
                                        long hp = cv.aLong[START + i * BLOCK + 4];
                                        long hpMax = cv.aLong[START + i * BLOCK + 5];
                                        long fragile = cv.aLong[START + i * BLOCK + 6];
                                        long fleeting = cv.aLong[START + i * BLOCK + 7];
                                        long cleave = cv.aLong[START + i * BLOCK + 8];
                                        long pierce = cv.aLong[START + i * BLOCK + 9];
                                        long breaker = cv.aLong[START + i * BLOCK + 10];
                                        long combo = cv.aLong[START + i * BLOCK + 11];
                                        long overrun = cv.aLong[START + i * BLOCK + 12];
                                        long shield = cv.aLong[START + i * BLOCK + 13];
                                        long godslayer = cv.aLong[START + i * BLOCK + 14];

                                        long cardMana = cv.aLong[START + i * BLOCK + 15];
                                        long discardAnim = cv.aLong[START + i * BLOCK + 16];
                                        if (info != null && buffCard != null)
                                        {
                                            buffCard.OnCastSkill(skillId, effectId, null, () =>
                                            {
                                                if (IsMeByServerPos(serverIndexPlayer))
                                                {
                                                    DBHero hero = Database.GetHero(newHeroId);
                                                    if (discardAnim == 0)
                                                    {
                                                        AddNewCard(0, hero, newBattleId, frame, atk, hp, cardMana, fleeting == 1, card =>
                                                        {
                                                            card.isFleeting = fleeting != 0;
                                                            card.OnUpdateManaText(cardMana);
                                                            if (isBidSkill)
                                                                card.ShowEffectSkillBid();
                                                            //if (buffCard != null)
                                                            //    buffCard.SummonNewCard(card.transform.position, null);

                                                        });
                                                    }
                                                    else
                                                    {
                                                        haveCardToDelete = true;
                                                        DiscardANewCard(0, hero, newBattleId, frame, atk, hp, cardMana, fleeting == 1, card =>
                                                        {
                                                            haveCardToDelete = false;
                                                            foreach (HandCard c in Decks[0].GetListCard)
                                                                c.isMoving = false;
                                                        });
                                                    }
                                                }
                                                else
                                                {
                                                    DBHero hero = new DBHero
                                                    {
                                                        id = -1
                                                    };
                                                    if (discardAnim == 0)
                                                    {

                                                        AddNewCard(1, hero, -1, 1, -1, -1, -1
                                                            , true);
                                                    }
                                                }
                                            });
                                        }
                                        else
                                        {
                                            if (IsMeByServerPos(serverIndexPlayer))
                                            {
                                                DBHero hero = Database.GetHero(newHeroId);
                                                if (discardAnim == 0)
                                                {
                                                    AddNewCard(0, hero, newBattleId, frame, atk, hp, cardMana, fleeting == 1, card =>
                                                    {
                                                        card.isFleeting = fleeting != 0;
                                                        card.OnUpdateManaText(cardMana);
                                                        if (isBidSkill)
                                                            card.ShowEffectSkillBid();
                                                        //if (buffCard != null)
                                                        //    buffCard.SummonNewCard(card.transform.position, null);

                                                    });
                                                }
                                                else
                                                {
                                                    haveCardToDelete = true;
                                                    DiscardANewCard(0, hero, newBattleId, frame, atk, hp, cardMana, fleeting == 1, card =>
                                                    {
                                                        haveCardToDelete = false;
                                                        foreach (HandCard c in Decks[0].GetListCard)
                                                            c.isMoving = false;
                                                    });
                                                }
                                            }
                                            else
                                            {
                                                DBHero hero = new DBHero
                                                {
                                                    id = -1
                                                };
                                                if (discardAnim == 0)
                                                {

                                                    AddNewCard(1, hero, -1, 1, -1, -1, -1
                                                        , true);
                                                }
                                            }
                                        }
                                    }
                                        break;
                                }

                            case (int)DBHeroSkill.EFFECT_REPLACE_CARD_ON_BOARD:
                                {
                                    WriteLogBattle("EFFECT_REPLACE_CARD_ON_BOARD: ", "", string.Join(",", cv.aLong));
                                    int BLOCK = 16, START = 5;
                                    int num = (cv.aLong.Count - START) / BLOCK;
                                    long serverIndexPlayer = cv.aLong[4];

                                    ProjectileDataInfo info = ProjectileData.Instance.projectileInfo.FirstOrDefault(x => x.skillID == skillId && x.effectID == effectId);
                                    if (buffCard != null && info != null)
                                    {
                                        buffCard.OnCastSkill(skillId, effectId, null, () =>
                                        {
                                        });
                                    }
                                    for (int i = 0; i < num; i++)
                                    {
                                        long battleIdCardReplaced = cv.aLong[START + i * BLOCK];
                                        long heroId = cv.aLong[START + i * BLOCK + 1];
                                        long frame = cv.aLong[START + i * BLOCK + 2];
                                        long atk = cv.aLong[START + i * BLOCK + 3 ];
                                        long hp = cv.aLong[START + i * BLOCK + 4];
                                        long hpMax = cv.aLong[START + i * BLOCK + 5];
                                        long fragile = cv.aLong[START + i * BLOCK + 6];
                                        long fleeting = cv.aLong[START + i * BLOCK + 7];
                                        long cleave = cv.aLong[START + i * BLOCK + 8];
                                        long pierce = cv.aLong[START + i * BLOCK + 9];
                                        long breaker = cv.aLong[START + i * BLOCK + 10];
                                        long combo = cv.aLong[START + i * BLOCK + 11];
                                        long overrun = cv.aLong[START + i * BLOCK + 12];
                                        long shield = cv.aLong[START + i * BLOCK + 13];
                                        long godslayer = cv.aLong[START + i * BLOCK + 14];
                                        long cardMana = cv.aLong[START + i * BLOCK + 15];
                                        var card = lstCardInBattle.FirstOrDefault(x => x.battleID == battleIdCardReplaced);
                                        if(card != null)
                                        {
                                            Transform spawnEffect = IsMeByServerPos(serverIndexPlayer) ? minionSkillSummonEffect : enemyMinionSkillSummonEffect;
                                            float delayTime = 0.5f;
                                            if (isBidSkill)
                                                card.ShowEffectSkillBid();
                                            StartCoroutine(CreateCardReplaceOnEffect(battleIdCardReplaced, heroId, frame, atk, hp, cardMana, spawnEffect, delayTime, (card) =>
                                                {
                                                    card.UpdateHeroMatrix(atk, hp, hpMax,cleave, pierce, breaker, combo, overrun, shield, godslayer);
                                                    card.isFragile = fragile != 0;
                                                    
                                                    SoundHandler.main.PlaySFX("SummonCard", "sounds");

                                                }));
                                        }    
                                    }
                                        break;
                                }
                            case (int)DBHeroSkill.EFFECT_DEAL_DAME_ON_HAND:
                                {
                                    WriteLogBattle("EFFECT_DEAL_DAME_ON_HAND: ", "", string.Join(",", cv.aLong));
                                    int BLOCK = 3, START = 5;
                                    int num = (cv.aLong.Count - START) / BLOCK;
                                    long serverIndexPlayer = cv.aLong[4];
                                    if ((int)GetClientPosFromServerPos(serverIndexPlayer) == 0)
                                    {
                                        for (int i = 0; i < num; i++)
                                        {
                                            long cardBattleId = cv.aLong[START + i * BLOCK];
                                            long cardHeroId = cv.aLong[START + i * BLOCK + 1];
                                            long cardATK = cv.aLong[START + i * BLOCK + 2];

                                            var myCards = Decks[0].GetListCard;
                                            foreach (HandCard card in myCards)
                                                if (cardBattleId == card.battleID)
                                                {
                                                    card.OnUpdateAtkText(cardATK);
                                                    if (isBidSkill)
                                                        card.ShowEffectSkillBid();
                                                    break;
                                                }
                                        }
                                        
                                        //Decks[0].ReBuildDeck();
                                    }
                                    else
                                    {
                                        Transform trans = PoolManager.Pools["Effect"].Spawn(m_EnemyHandcardSkillEffect, ultiVfxContent);
                                        trans.GetComponent<ParticleEffectParentCallback>().SetOnComplete(() =>
                                        {
                                            PoolManager.Pools["Effect"].Despawn(trans);
                                        });
                                    }
                                    break;
                                }
                        }
                        StartCoroutine(HideMagicCard(battleId, skillId));
                    }
                    else
                    {
                        //StartCoroutine(HideMagicCard(currentCardSelected.battleID));
                    }
                    break;
                }

            case IService.GAME_BATTLE_ATTACK:
                {
                    yield return new WaitForSeconds(0.5f);
                    CommonVector cv = ISocket.Parse<CommonVector>(action.data);
                    WriteLogBattle("BATTLE_ATTACK_SKILL_SIMULATE: ", "", string.Join(",", cv.aLong));

                    //long totalCount = cv.aLong[2] + 3;
                    long totalCount = cv.aLong[4] + 5;

                    for (int i = 0; i < cv.aLong.Count; i += (int)totalCount)
                    {
                        var atkCard = lstCardInBattle.FirstOrDefault(c => c.battleID == cv.aLong[i]);
                        // atk tower
                        if (cv.aLong[i + 1] < 0)
                        {
                            var lstTower = lstTowerInBattle.Where(t => cv.aLong[i + 1] < -10 ? t.pos == GetClientPosFromServerPos(1) : t.pos == GetClientPosFromServerPos(0));
                            long id = cv.aLong[i + 1] < -10 ? (long)Mathf.Abs(cv.aLong[i + 1]) - 11 : (long)Mathf.Abs(cv.aLong[i + 1]) - 1;
                            var defTower = lstTower.FirstOrDefault(t => t.id == id);
                            if (atkCard != null && defTower != null)
                            {
                                atkCard.OnAttackTower(defTower, out float waitTime);
                                yield return new WaitForSeconds(waitTime);
                            }
                        }
                        // atk card
                        else
                        {
                            var defCard = lstCardInBattle.FirstOrDefault(c => c.battleID == cv.aLong[i + 1]);
                            if (atkCard != null && defCard != null)
                            {
                                atkCard.OnAttackCard(defCard, out float waitTime);
                                yield return new WaitForSeconds(waitTime);
                            }
                        }


                        //int count = 4;
                        int count = i + 6;

                        List<SkillEffect> effects = new List<SkillEffect>();
                        if (cv.aLong[i + 2] > 0)
                        {
                            SkillEffect eff = new SkillEffect();
                            eff.typeEffect = DBHero.KEYWORD_GODSLAYER;
                            effects.Add(eff);
                        }
                        if (cv.aLong[i + 3] > 0)
                        {
                            SkillEffect eff = new SkillEffect();
                            eff.typeEffect = DBHero.KEYWORD_DEFENDER;
                            effects.Add(eff);
                        }
                        // them ca eff cho breaker va godSlayer
                        while (count < i + cv.aLong[i + 4] + 5)
                        {
                            // loai effect
                            if (cv.aLong[i + 5] > 0)
                            {
                                long typeEffect = cv.aLong[count];
                                long defCount = cv.aLong[count + 1];
                                SkillEffect eff = new SkillEffect();
                                eff.typeEffect = typeEffect;
                                eff.defCount = defCount;
                                for (int j = 0; j < defCount; j++)
                                {
                                    // effect here
                                    //cac quan ảnh huong : bai hoac tru,
                                    //switch case cho tung effect.  goi sang card, truyen vao loai effect và so luong bai kem theo id quan bi anh huong 
                                    if (cv.aLong[count + 2 + j] < 0)
                                    {
                                        var lstTower = lstTowerInBattle.Where(t => cv.aLong[i + 1] < -10 ? t.pos == GetClientPosFromServerPos(1) : t.pos == GetClientPosFromServerPos(0));
                                        long id = cv.aLong[i + 1] < -10 ? (long)Mathf.Abs(cv.aLong[i + 1]) - 11 : (long)Mathf.Abs(cv.aLong[i + 1]) - 1;
                                        var defTower = lstTower.FirstOrDefault(t => t.id == id);
                                        eff.lstTowerImpact.Add(defTower);
                                    }
                                    else
                                    {
                                        var defCard = lstCardInBattle.FirstOrDefault(c => c.battleID == cv.aLong[i + 1]);
                                        eff.lstCardImpact.Add(defCard);
                                    }
                                    if (j == defCount - 1)
                                    {
                                        effects.Add(eff);
                                    }
                                }
                                count += (int)defCount + 2;
                                atkCard.DisplayListEffect(effects);
                            }

                        }
                        totalCount = cv.aLong[i + 4] + 5;
                    }
                    yield return new WaitForSeconds(0.25f);
                    break;
                }
            case IService.GAME_BATTLE_DEAL_DAMAGE:
                {
                    CommonVector cv1 = ISocket.Parse<CommonVector>(action.data);
                    LogWriterHandle.WriteLog("==========DEAL_DAMAGE SKILLS PREPARE: " + string.Join(",", cv1.aLong));
                    WriteLogBattle("DEAL_DAMAGE_SKILL_SIMULATE: ", GameData.main.profile.username, string.Join(",", cv1.aLong));

                    yield return new WaitForSeconds(0.5f);

                    for (int i = 0; i < cv1.aLong.Count; i += 3)
                    {
                        if (cv1.aLong[i] < 0)
                        {
                            TowerDealDamage(cv1.aLong[i], cv1.aLong[i + 1], cv1.aLong[i + 2]);
                        }
                        else
                        {
                            var lstCard = lstCardInBattle.Where(c => c.battleID == cv1.aLong[i]);
                            lstCard.ToList().ForEach(card => card.OnDamaged(cv1.aLong[i + 1], cv1.aLong[i + 2]));
                        }
                    }

                    break;
                }
            case IService.GAME_BATTLE_HERO_DEAD:
                {
                    ListCommonVector lstCv = ISocket.Parse<ListCommonVector>(action.data);
                    yield return new WaitForSeconds(1f);
                    foreach (CommonVector c in lstCv.aVector)
                    {
                        LogWriterHandle.WriteLog("HERO_DEAD PREPARE: " + string.Join(",", c.aLong));
                        WriteLogBattle("HERO_DEAD PREPARE: ", GameData.main.profile.username, string.Join(",", c.aLong));
                        //along[0] là hp remain cua tru
                        for (int i = 1; i < c.aLong.Count; i += 3)
                        {
                            var lstCard = lstCardInBattle.Where(s => s.battleID == c.aLong[i]);
                            lstCard.ToList().ForEach(card =>
                            {
                                card.OnDeath();
                                //PoolManager.Pools["Card"].Despawn(card.transform);
                                SoundHandler.main.PlaySFX("BrokenCard1", "sounds");
                                // remove card in list after destroy
                                lstCardInBattle.Remove(card);
                            });

                            // if no damage to tower, continue
                            if (c.aLong[i + 1] == 0)
                            {
                                continue;
                            }
                            else
                            {
                                long towerID = 0;
                                if (GetServerPostFromUsername(c.aString[0]) == 0)
                                {
                                    towerID = c.aLong[i + 2] * -1;
                                }
                                else
                                {
                                    towerID = (c.aLong[i + 2] * -1) - 10;
                                }
                                TowerDealDamage(towerID, c.aLong[i + 1], c.aLong[(int)c.aLong[i + 2] - 1]);
                            }
                        }
                    }
                    break;
                }
            case IService.GAME_BATTLE_HERO_TIRED:
                {
                    CommonVector cv2 = ISocket.Parse<CommonVector>(action.data);
                    WriteLogBattle("HERO_TIRED_SKILL_SIMULATE: ", GameData.main.profile.username, string.Join(",", cv2.aLong));

                    yield return new WaitForSeconds(0.25f);

                    var cardTireds = lstCardInBattle.Where(card => cv2.aLong.Contains(card.battleID));
                    cardTireds.ToList().ForEach(card =>
                    {
                        card.SetTired(1);
                        card.SetMovable(false);
                    });
                    break;
                }
            case IService.GAME_BATTLE_HERO_READY:
                {
                    CommonVector cv3 = ISocket.Parse<CommonVector>(action.data);
                    WriteLogBattle("HERO_READY_SKILL_SIMULATE: ", GameData.main.profile.username, string.Join(",", cv3.aLong));

                    yield return new WaitForSeconds(1);

                    break;
                }
            case IService.GAME_SIMULATE_SKILLS_ON_BATTLE:
                {
                    ListAction listActionSkill = ISocket.Parse<ListAction>(action.data);
                    foreach (Action ac in listActionSkill.aAction)
                        lstSkillQueue.Add(ac);
                    //StartCoroutine(SimulateSkillEffect(listActionSkill, true));
                    //yield return new WaitForSeconds(CalculateTimeForSkill(listActionSkill));
                }
                break;
        }

        canContinue = true;
    }
    private IEnumerator HideMagicCard(long battleId, long skillId = -1, bool isPlayer = true)
    {
        if (isPlayer)
        {
            float wait = 1.5f;
            if (skillId == 43)
                wait = 3.5f;
            yield return new WaitForSeconds(wait);

        }
        else
            yield return new WaitForSeconds(6);

        BoardCard card = lstCardInBattle.FirstOrDefault(x => x.battleID == battleId && (x.heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || x.heroInfo.type == DBHero.TYPE_BUFF_MAGIC));
        if (card != null)
        {
            LogWriterHandle.WriteLog("=====HideMagicCard =" + card.battleID + "---" + card.heroInfo.type);
            lstCardInBattle.Remove(card);
            PoolManager.Pools["Card"].Despawn(card.transform);
        }
    }

    //logic
    private IEnumerator GameDealCards(ListCommonVector listCommonVector)
    {
        yield return new WaitForSeconds(0f);
        onProcessData = true;
        // draw card
        // show than cua 2 ben
        // draw card cho 2 ben
        SoundHandler.main.PlaySFX("DealCard", "sounds");

        if (listCommonVector.aVector.Count >= 2)
        {


            IsYourTurn = true;
            turnCount = 0;

            foreach (CommonVector cv in listCommonVector.aVector)
            {
                WriteLogBattle("DEAL_CARD: ", string.Join(",", cv.aString), string.Join(",", cv.aLong));
                //LogWriterHandle.WriteLog("DEAL_CARD: " + string.Join(",", cv.aString) + "|" + string.Join(",", cv.aLong));

            }

            CommonVector cv0 = listCommonVector.aVector[0];
            CommonVector cv1 = listCommonVector.aVector[1];

            if (GameData.main.profile.username.Equals(cv0.aString[0]))
            {
                for (int i = 1; i < cv0.aLong.Count; i += 6)
                {
                    AddListCard(lstGodPlayer, lstGodPlayerBattleID, lstGodPlayerFrame, lstGodPlayerAtk, lstGodPlayerHp, lstGodPlayerMana, cv0.aLong[i], cv0.aLong[i - 1], cv0.aLong[i + 1], cv0.aLong[i + 2], cv0.aLong[i + 3], cv0.aLong[i + 4]);

                }
                for (int i = 1; i < cv1.aLong.Count; i += 6)
                {
                    AddListCard(lstGodEnemy, lstGodEnemyBattleID, lstGodEnemyFrame, lstGodEnemyAtk, lstGodEnemyHp, lstGodEnemyMana, cv1.aLong[i], cv1.aLong[i - 1], cv1.aLong[i + 1], cv1.aLong[i + 2], cv1.aLong[i + 3], cv1.aLong[i + 4]);
                }
            }
            else
            {
                for (int i = 1; i < cv0.aLong.Count; i += 6)
                {
                    AddListCard(lstGodEnemy, lstGodEnemyBattleID, lstGodEnemyFrame, lstGodEnemyAtk, lstGodEnemyHp, lstGodEnemyMana, cv0.aLong[i], cv0.aLong[i - 1], cv0.aLong[i + 1], cv0.aLong[i + 2], cv0.aLong[i + 3], cv0.aLong[i + 4]);
                }
                for (int i = 1; i < cv1.aLong.Count; i += 6)
                {
                    AddListCard(lstGodPlayer, lstGodPlayerBattleID, lstGodPlayerFrame, lstGodPlayerAtk, lstGodPlayerHp, lstGodPlayerMana, cv1.aLong[i], cv1.aLong[i - 1], cv1.aLong[i + 1], cv1.aLong[i + 2], cv1.aLong[i + 3], cv1.aLong[i + 4]);
                }
            }

            onGameDealCard?.Invoke(lstGodPlayerBattleID, lstGodPlayer, lstGodPlayerFrame, lstGodPlayerAtk, lstGodPlayerHp, lstGodPlayerMana, lstGodEnemyBattleID, lstGodEnemy, lstGodEnemyFrame, lstGodEnemyAtk, lstGodEnemyHp, lstGodEnemyMana);

            CommonVector cv2 = listCommonVector.aVector[2];
            for (int i = 1; i < cv2.aLong.Count; i += 6)
            {
                AddListCard(lstHeroPlayer, lstHeroBattleID, lstHeroFrame, lstHeroAtk, lstHeroHp, lstHeroMana, cv2.aLong[i], cv2.aLong[i - 1], cv2.aLong[i + 1], cv2.aLong[i + 2], cv2.aLong[i + 3], cv2.aLong[i + 4]);
            }

            //onGameDealHandCard?.Invoke(lstHeroPlayer.Count, m_MinionCard.GetComponent<Card>());
            DrawDeckStart(0, lstHeroPlayer, lstHeroBattleID, lstHeroFrame, lstHeroAtk, lstHeroHp, lstHeroMana);

            List<DBHero> lstHero = new List<DBHero>();
            List<long> lstID = new List<long>();
            List<long> lstFrame = new List<long>();
            List<long> lstAtk = new List<long>();
            List<long> lstHp = new List<long>();
            List<long> lstMana = new List<long>();

            for (int i = 1; i < cv2.aLong.Count; i += 6)
            {
                DBHero hero = new DBHero();
                hero.id = -1;
                lstHero.Add(hero);
                lstID.Add(-1);
                lstFrame.Add(1);
                lstAtk.Add(1);
                lstHp.Add(1);
                lstMana.Add(1);
            }
            DrawDeckStart(1, lstHero, lstID, lstFrame, lstAtk, lstHp, lstMana);
            yield return new WaitForSeconds(1f);
            onProcessData = false;
        }
    }

    private void AddListCard(List<DBHero> lstHero, List<long> lstID, List<long> lstFrame, List<long> lstAtk, List<long> lstHp, List<long> lstMana, long heroID, long battleID, long frame, long atk, long hp, long mana, List<long> lstFragile = null, long fragile = -1)
    {
        DBHero hero = Database.GetHero(heroID);
        lstHero.Add(hero);
        lstID.Add(battleID);
        lstFrame.Add(frame);
        lstAtk.Add(atk);
        lstHp.Add(hp);
        lstMana.Add(mana);
        if (lstFragile != null)
            lstFragile.Add(fragile);
    }

    private IEnumerator GameCardMulligan(CommonVector commonVector)
    {
        yield return new WaitForSeconds(0f);
        onProcessData = true;

        //if (commonVector.aLong[0] == 0)
        //{
        //    return;
        //}

        //WriteLogBattle("MULLIGAN: ", string.Join(",", commonVector.aString), string.Join(",", commonVector.aLong));

        //if (GameData.main.profile.username.Equals(commonVector.aString[0]))
        //{
        //    List<DBHero> lstNewHero = new List<DBHero>();
        //    List<long> lstNewHeroBattleID = new List<long>();

        //    for (int i = 2 + (int)commonVector.aLong[1]; i < commonVector.aLong.Count; i += 2)
        //    {
        //        DBHero hero = Database.GetHero(commonVector.aLong[i + 1]);
        //        lstNewHero.Add(hero);
        //        lstNewHeroBattleID.Add(commonVector.aLong[i]);
        //        AddNewCard(0, hero, commonVector.aLong[i]);
        //    }
        //}
        //else
        //{
        //    List<DBHero> lstNewHero = new List<DBHero>();
        //    List<long> lstNewHeroBattleID = new List<long>();
        //    List<Card> currentCard = new List<Card>();

        //    for (int i = 0; i < Decks[1].cards.Length; i++)
        //    {
        //        if (Decks[1].cards[i] != null)
        //        {
        //            currentCard.Add(Decks[1].cards[i]);
        //        }
        //    }
        //    for (int i = 0; i < (int)commonVector.aLong[1]; i++)
        //    {
        //        Card removeCard = currentCard[i];
        //        Decks[1].Remove(removeCard);
        //        PoolManager.Pools["Card"].Despawn(removeCard.transform);
        //    }

        //    for (int i = 0; i < (int)commonVector.aLong[1]; i++)
        //    {
        //        DBHero hero = new DBHero();
        //        hero.id = -1;
        //        lstNewHero.Add(hero);
        //        lstNewHeroBattleID.Add(0);
        //        AddNewCard(1, hero, -1);
        //    }
        //}
        //lstMulliganCard.Clear();
        //isMulligan = false;

        yield return new WaitForSeconds(0f);
        onProcessData = false;
    }

    private IEnumerator GameFirstGodSummon(CommonVector commonVector)
    {
        yield return new WaitForSeconds(0f);
        onProcessData = true;

        if (commonVector.aLong[0] == 0)
        {
            Toast.Show(commonVector.aString[0]);
            onProcessData = false;
        }
        else
        {
            //Destroy(currentGodCardUI);

            CardSlot slot = playerSlotContainer.FirstOrDefault(x => x.xPos == commonVector.aLong[7] && x.yPos == commonVector.aLong[8]);
            if (slot != null)
            {
                if (commonVector.aLong[2] == 1)
                {
                    //sua frame
                    StartCoroutine(CreateCard(commonVector.aLong[1], commonVector.aLong[2], commonVector.aLong[3], commonVector.aLong[4], commonVector.aLong[5], commonVector.aLong[6], m_GodCard, slot.transform, null, slot, CardOwner.Player, 0.4f, (card) =>
                    {
                        //CheckHeroSkill(TYPE_WHEN_SUMON, card);

                        string voiceName = card.heroInfo.heroNumber.ToString() + "_" + VoiceData.TYPE_SUMMON + "_" + Random.Range(1, 3).ToString();
                        //CheckHeroSkill(TYPE_WHEN_SUMON, card,slot.xPos,slot.yPos);
                        AudioClip clip = BundleHandler.LoadSound(voiceName, "voicegod");
                        if (clip != null)
                            SoundHandler.main.PlaySFX(voiceName, "voicegod");

                        SoundHandler.main.PlaySFX("SummonCard", "sounds");
                    }));
                }
                else
                {
                    StartCoroutine(CreateCard(commonVector.aLong[1], commonVector.aLong[2], commonVector.aLong[3], commonVector.aLong[4], commonVector.aLong[5], commonVector.aLong[6], m_GodCard, slot.transform, null, slot, CardOwner.Player, 0.4f, (card) =>
                     {
                         //CheckHeroSkill(TYPE_WHEN_SUMON, card);
                         string voiceName = card.heroInfo.heroNumber.ToString() + "_" + VoiceData.TYPE_SUMMON + "_" + Random.Range(1, 3).ToString();
                         //CheckHeroSkill(TYPE_WHEN_SUMON, card,slot.xPos,slot.yPos);
                         AudioClip clip = BundleHandler.LoadSound(voiceName, "voicegod");
                         if (clip != null)
                             SoundHandler.main.PlaySFX(voiceName, "voicegod");
                         SoundHandler.main.PlaySFX("SummonCard", "sounds");
                     }));
                }
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(1f);
            onProcessData = false;
        }

    }
    private BoardCard GetBoardCard(long battleID)
    {
        var card = lstCardInBattle.FirstOrDefault(x => x.battleID == battleID);
        return card;
    }

    private HandCard GetHandCard(long battleID)
    {
        var card = Decks[0].GetListCard.FirstOrDefault(x => x.battleID == battleID);
        return card;
    }

    private CardSlot GetSlot(SlotType type, long x, long y)
    {
        List<CardSlot> lstSlot = type == SlotType.Player ? playerSlotContainer : enemySlotContainer;
        var slot = lstSlot.FirstOrDefault(s => s.xPos == x && s.yPos == y);
        return slot;
    }

    private IEnumerator GameMoveGodSumon(CommonVector commonVector)
    {
        yield return new WaitForSeconds(0f);
        onProcessData = true;

        LogWriterHandle.WriteLog("Movegod summon: " + string.Join(",", commonVector.aLong));
        if (commonVector.aLong[0] == 0)
            Toast.Show(commonVector.aString[0]);

        BoardCard card = GetBoardCard(commonVector.aLong[1]);
        if (card != null)
        {
            CardSlot slot = GetSlot(SlotType.Player, commonVector.aLong[4], commonVector.aLong[5]);
            if (slot != null)
                card.MoveToSlot(slot);
        }

        yield return new WaitForSeconds(0.5f);
        onProcessData = false;
    }

    private IEnumerator GameStartupConfirm(CommonVector commonVector)
    {
        yield return new WaitForSeconds(0f);
        onProcessData = true;

        onGameStartupConfirm?.Invoke(GameData.main.profile.username.Equals(commonVector.aString[0]));

        yield return new WaitForSeconds(0.5f);
        onProcessData = false;
    }

    private IEnumerator GameStartupEnd(ListCommonVector listCommonVector)
    {
        yield return new WaitForSeconds(0f);
        onProcessData = true;
        float waitingTime = 0;
        bool exist = false;
        BoardCard enemyGod = null;
        onGameStartupEnd?.Invoke();
        foreach (CommonVector cv in listCommonVector.aVector)
        {
            WriteLogBattle("START_UP_END: ", string.Join(",", cv.aString), string.Join(",", cv.aLong));

            if (!GameData.main.profile.username.Equals(cv.aString[0]))
            {
                if (cv.aLong.Count > 0)
                {
                    CardSlot slot = enemySlotContainer.FirstOrDefault(x => x.xPos == cv.aLong[0] && x.yPos == cv.aLong[1]);
                    if (slot != null)
                    {
                        string voiceName = "";
                        //sua frame
                        StartCoroutine(CreateCard(cv.aLong[2], cv.aLong[3], cv.aLong[4], cv.aLong[5], cv.aLong[6], cv.aLong[7], m_EnemyGodOnBoardCard, slot.transform, holySpawnEffect, slot, CardOwner.Enemy, 0.1f, (card) =>
                        {
                            //CheckHeroSkill(TYPE_WHEN_SUMON, card);
                            enemyGod = card;
                            voiceName = enemyGod.heroInfo.heroNumber + "_" + VoiceData.TYPE_SUMMON + "_" + Random.Range(1, 3).ToString();
                            SoundHandler.main.PlaySFX("SummonCard", "sounds");
                            StartCoroutine(PlayVoiceGodOnFirstGodSummon(voiceName));
                        }));




                    }
                    onSpawnRandomGodEnemy?.Invoke(cv.aLong[3]);
                    waitingTime += 1f;
                }
            }
            else if (GameData.main.profile.username.Equals(cv.aString[0]))
            {
                if (cv.aLong.Count > 2)
                {
                    exist = false;
                    foreach (Card c in lstCardInBattle)
                        if (c.battleID == cv.aLong[2])
                            exist = true;
                    if (!exist)
                    {
                        CardSlot slot = playerSlotContainer.FirstOrDefault(x => x.xPos == cv.aLong[0] && x.yPos == cv.aLong[1]);
                        if (slot != null)
                        {
                            if (cv.aLong[3] == 1)
                            {
                                StartCoroutine(CreateCard(cv.aLong[2], cv.aLong[3], cv.aLong[4], cv.aLong[5], cv.aLong[6], cv.aLong[7], m_GodCard, slot.transform, null, slot, CardOwner.Player, 0.4f, (card) =>
                                {
                                    string voiceName = card.heroInfo.heroNumber.ToString() + "_" + VoiceData.TYPE_SUMMON + "_" + Random.Range(1, 3).ToString();
                                    //CheckHeroSkill(TYPE_WHEN_SUMON, card,slot.xPos,slot.yPos);
                                    AudioClip clip = BundleHandler.LoadSound(voiceName, "voicegod");
                                    if (clip != null)
                                        SoundHandler.main.PlaySFX(voiceName, "voicegod");
                                    SoundHandler.main.PlaySFX("SummonCard", "sounds");
                                }));
                            }
                            else
                            {
                                StartCoroutine(CreateCard(cv.aLong[2], cv.aLong[3], cv.aLong[4], cv.aLong[5], cv.aLong[6], cv.aLong[7], m_GodCard, slot.transform, null, slot, CardOwner.Player, 0.4f, (card) =>
                                {
                                    string voiceName = card.heroInfo.heroNumber.ToString() + "_" + VoiceData.TYPE_SUMMON + "_" + Random.Range(1, 3).ToString();
                                    //CheckHeroSkill(TYPE_WHEN_SUMON, card,slot.xPos,slot.yPos);
                                    AudioClip clip = BundleHandler.LoadSound(voiceName, "voicegod");
                                    if (clip != null)
                                        SoundHandler.main.PlaySFX(voiceName, "voicegod");
                                    SoundHandler.main.PlaySFX("SummonCard", "sounds");
                                }));
                            }
                            exist = true;
                            LogWriterHandle.WriteLog("onSpawnRandomGod" + cv.aLong[2] + "/" + cv.aLong[3]);
                            onSpawnRandomGod?.Invoke(cv.aLong[2]);
                            yield return new WaitForSeconds(0.5f);
                        }

                        waitingTime += 1f;
                    }
                }
            }

        }


        yield return new WaitForSeconds(1f);
        onProcessData = false;
    }
    IEnumerator PlayVoiceGodOnFirstGodSummon(string voiceName)
    {
        if (GetListPlayerCardInBattle().Count == 0)
        {
            //neu minh chưa summon first god,pass đồng thời với enemy ->  đợi god mk noi xong moi den than dich
            yield return new WaitForSeconds(3f);

            //CheckHeroSkill(TYPE_WHEN_SUMON, card,slot.xPos,slot.yPos);
            AudioClip clip = BundleHandler.LoadSound(voiceName, "voicegod");
            if (clip != null)
                SoundHandler.main.PlaySFX(voiceName, "voicegod");
        }
        else
        {
            yield return new WaitForSeconds(0f);
            //CheckHeroSkill(TYPE_WHEN_SUMON, card,slot.xPos,slot.yPos);
            AudioClip clip = BundleHandler.LoadSound(voiceName, "voicegod");
            if (clip != null)
                SoundHandler.main.PlaySFX(voiceName, "voicegod");
        }
    }
    long turnMana;
    private IEnumerator GameStartBattle(ListAction listAction)
    {
        yield return new WaitForSeconds(0f);
        onProcessData = true;

        if (!isGameStarted)
            isGameStarted = true;
        waitTimeToBattle = 0;
        float delayTime = 0f;
        bool isBid = false;

        foreach (Action a in listAction.aAction)
        {
            switch (a.actionId)
            {
                case IService.GAME_DELETE_CARDS:
                    {
                        CommonVector commonVector = ISocket.Parse<CommonVector>(a.data);
                        WriteLogBattle("GAME_DELETE_CARDS: ", string.Join(",", commonVector.aString), string.Join(",", commonVector.aLong));
                        StartCoroutine(DeleteCardsOnHand(commonVector, true));

                        break;
                    }
                case IService.GAME_START_BATTLE_DETAIL:

                    {
                        CommonVector commonVector = ISocket.Parse<CommonVector>(a.data);
                        WriteLogBattle("GAME_START_BATTLE: ", string.Join(",", commonVector.aString), string.Join(",", commonVector.aLong));
                        //way
                        turnCount = commonVector.aLong[0];
                        turnArrow[0].SetActive(turnCount % 2 == 0);
                        turnArrow[1].SetActive(turnCount % 2 != 0);
                        turnMana = commonVector.aLong[2];
                        UIManager.instance.OnUpdateListBidSkill();
                        onGameStartRound?.Invoke();
                        //bat dau luot cua minh 
                        IsYourTurn = GameData.main.profile.username.Equals(commonVector.aString[0]);
                        ButtonOrbController.instance.UpdateBattleSword(commonVector.aString[0], commonVector.aString[0], GameState.RoundStart);
                        SoundHandler.main.PlaySFX("NewTurn", "sounds");

                        for (int i = 1; i < commonVector.aString.Count; i++)
                        {
                            long cardSize = commonVector.aLong[3 + (i - 1)];
                            string username = commonVector.aString[i];
                            if (cardSize > 0)
                            {
                                if (GameData.main.profile.username.Equals(username))
                                {
                                    long battleID = commonVector.aLong[commonVector.aLong.Count - 7];
                                    long heroID = commonVector.aLong[commonVector.aLong.Count - 6];
                                    long frame = commonVector.aLong[commonVector.aLong.Count - 5];
                                    long atk = commonVector.aLong[commonVector.aLong.Count - 4];
                                    long hp = commonVector.aLong[commonVector.aLong.Count - 3];
                                    long cardMana = commonVector.aLong[commonVector.aLong.Count - 2];
                                    DBHero hero = Database.GetHero(heroID);
                                    if (Decks[0].GetListCard.Count < 10)
                                    {
                                        AddNewCard(0, hero, battleID, frame, atk, hp, cardMana);
                                    }
                                    else
                                    {
                                        DiscardANewCard(0, hero, battleID, frame, atk, hp, cardMana, false, card =>
                                        {
                                            foreach (HandCard c in Decks[0].GetListCard)
                                                c.isMoving = false;
                                        });
                                    }
                                }
                                else
                                {
                                    if (Decks[1].GetListCard.Count < 10)
                                    {
                                        DBHero hero = new DBHero();
                                        hero.id = -1;
                                        AddNewCard(1, hero, -1, 1, -1, -1, -1);
                                    }
                                }
                            }
                        }
                        SoundHandler.main.PlaySFX("MamaRegen1", "sounds");
                        onUpdateMana?.Invoke(0, commonVector.aLong[2], ManaState.StartTurn, 0);
                        onUpdateMana?.Invoke(1, commonVector.aLong[2], ManaState.StartTurn, 0);
                        isBid = commonVector.aLong[commonVector.aLong.Count - 1] == 1 ? true : false;
                        //lstCardInBattle.ForEach(card => CheckHeroSkill(TYPE_WHEN_START_TURN, card));
                        delayTime += 3f;
                        break;
                    }

                case IService.GAME_SIMULATE_SKILLS_ON_BATTLE:
                    {
                        ListAction listActionSkill = ISocket.Parse<ListAction>(a.data);
                        yield return new WaitForSeconds(1.5f);
                        foreach (Action ac in listActionSkill.aAction)
                            lstSkillQueue.Add(ac);
                        //StartCoroutine(SimulateSkillEffect(listActionSkill, true));
                        //delayTime += CalculateTimeForSkill(listAction);
                        break;
                    }


            }
        }

        yield return new WaitForSeconds(delayTime);

        if (isBid)
        {
            Game.main.socket.GameStartBid();
        }
        else
        {
            onBidStateStart?.Invoke(BidState.NoBid,-1,-1,-1,-1,-1);
            StartCoroutine(GameStartBattleSimulation(GetTurnEffect()));
        }
        yield return new WaitForSeconds(0f);
        onProcessData = false;
    }
    public void DoUpBid()
    {
        if (canUpBid)
            Game.main.socket.GameUpBid();
    }
    bool canUpBid = false;
    private IEnumerator GameStartBid(CommonVector cv)
    {
        yield return new WaitForSeconds(0f);
        onProcessData = true;
        long playerShard =0,enemyShard=0;
        long playerBid=0,enemyBid=0;
        int BLOCK = 2;
        for (int i =0; i<cv.aString.Count; i++)
        {
            if(GameData.main.profile.username.Equals(cv.aString[i]))
            { 
                playerBid = cv.aLong[i * BLOCK];
                playerShard = cv.aLong[(i* BLOCK)+1];
            }    
            else
            {
                enemyBid = cv.aLong[i * BLOCK];
                enemyShard = cv.aLong[(i* BLOCK)+1];
            }    

        }
        onBidStateStart?.Invoke(BidState.StartBid, playerShard, enemyShard, playerBid, enemyBid,-1);
        canUpBid = true;
        yield return new WaitForSeconds(0.5f);
        onProcessData = false;
        yield return new WaitForSeconds(19f);
        canUpBid = false;
    }
    private IEnumerator GameUpBid(CommonVector commonVector)
    {
        yield return new WaitForSeconds(0f);
        onProcessData = true;
        WriteLogBattle("GAME_UP_BID: ", string.Join(",", commonVector.aString), string.Join(",", commonVector.aLong));
        if (commonVector.aLong[0] == 1)
        {
            onUpBidState?.Invoke(BidState.UpBid, GameData.main.profile.username.Equals(commonVector.aString[0]), commonVector.aLong[1], commonVector.aLong[2]);

        }
        else
        {
            Toast.Show(commonVector.aString[0]);
        }
        yield return new WaitForSeconds(0.5f);
        onProcessData = false;
    }   
    private IEnumerator GameBidResult(ListAction listAction)
    {
        yield return new WaitForSeconds(0f);
        onProcessData = false;
        float delayTime = 0f;
        foreach (Action a in listAction.aAction)
        {
            switch (a.actionId)
            {
                case IService.GAME_SUB_BID_RESULT:
                    CommonVector commonVector = ISocket.Parse<CommonVector>(a.data);
                    WriteLogBattle("BID_RESULT: ", string.Join(",", commonVector.aString), string.Join(",", commonVector.aLong));
                    BidState state;
                    if (commonVector.aLong[0] == 0)
                    {
                        state = BidState.Lose;

                    }
                    else if (commonVector.aLong[0] == 1)
                    {
                        state = BidState.Random;
                    }
                    else
                    {
                        state = BidState.Win;
                    }
                    onBidEnd?.Invoke(state, GameData.main.profile.username.Equals(commonVector.aString[0]));
                    break;


                case IService.GAME_SIMULATE_SKILLS_ON_BATTLE:

                    ListAction listActionSkill = ISocket.Parse<ListAction>(a.data);

                    bool needWait = false;
                    foreach (Action aa in listActionSkill.aAction)
                    {
                        CommonVector c = ISocket.Parse<CommonVector>(aa.data);

                        if (c.aLong.Count > 1)
                        {
                            long effectId = c.aLong[1];
                            if (effectId == DBHeroSkill.EFFECT_FIGHT)
                                needWait = true;
                        }
                    }

                    yield return new WaitForSeconds(2.5f);
                    delayTime += 2.5f;
                    if (needWait)
                    {
                        yield return new WaitForSeconds(1.5f);
                        delayTime += 1.5f;
                    }

                    //StartCoroutine(SimulateSkillEffect(listActionSkill, true));
                    //delayTime += CalculateTimeForSkill(listActionSkill);
                    foreach (Action ac in listActionSkill.aAction)
                        lstSkillQueue.Add(ac);
                    break;

                

            }
        }

        yield return new WaitForSeconds(delayTime);
        onProcessData = false;
    }    
    public bool haveCardToDelete = false;
    private IEnumerator DeleteCardsOnHand(CommonVector commonVector, bool attached)
    {

        yield return new WaitForSeconds(3f);
        if (!attached)
            onProcessData = true;

        var myCards = Decks[0].GetListCard;


        foreach (long battleId in commonVector.aLong)
        {
            foreach (HandCard card in myCards)
            {
                if (battleId == card.battleID)
                {
                    haveCardToDelete = true;
                    card.DiscardHandCardPlayer(showOpponentCardPoint, () =>
                    {
                        Decks[0].RemoveCard(card);
                        PoolManager.Pools["Card"].Despawn(card.transform);
                        haveCardToDelete = false;
                        card.isMoving = false;
                    });
                    break;
                }
            }
        }
        yield return new WaitForSeconds(1f);
        if (!attached)
            onProcessData = false;
    }

    public List<GameObject> GetTurnEffect()
    {
        return turnEffect;
    }

    public void OnEndEffectBidPhase( float waitTime)
    {
        StartCoroutine(DelayCallback(waitTime, () => { StartCoroutine(GameStartBattleSimulation(GetTurnEffect(),true)); }));
    }
    public IEnumerator GameStartBattleSimulation(List<GameObject> turnEffect, bool hadBidPhase = false)
    {
        yield return new WaitForSeconds(0.5f);
        onGameBattleSimulation?.Invoke(IsYourTurn, roundCount, turnMana);
        if (IsYourTurn)
        {
            turnEffect[0].SetActive(true);
            turnEffect[1].SetActive(false);
        }
        else
        {
            turnEffect[0].SetActive(false);
            turnEffect[1].SetActive(true);
        }
        if (hadBidPhase)
        {
            float timeEffectSkillBid = 1;
            yield return new WaitForSeconds(timeEffectSkillBid);
            onSubGameBattleSimulationHasBid?.Invoke(IsYourTurn);
        }    
    }

    private IEnumerator GameMoveCardInBattle(ListAction listAction)
    {
        yield return new WaitForSeconds(0f);
        onProcessData = true;
        float delayTime = 0f;
        battleState = BATTLE_STATE.NORMAL;

        foreach (Action a in listAction.aAction)
        {

            switch (a.actionId)
            {
                case IService.GAME_MOVE_CARD_IN_BATTLE_DETAIL:
                    CommonVector commonVector = ISocket.Parse<CommonVector>(a.data);
                    WriteLogBattle("MOVE_CARD: ", string.Join(",", commonVector.aString), string.Join(",", commonVector.aLong));
                    //1,3,3,0,2,0,0,1,2,2,0,0,0,2,1
                    if (commonVector.aLong[0] == 0)
                    {
                        Toast.Show(commonVector.aString[0]);
                    }
                    else
                    {
                        if (IsYourTurn)
                        {
                            for (int i = 1; i < commonVector.aLong.Count; i += 7)
                            {
                                BoardCard card = GetBoardCard(commonVector.aLong[i]);
                                if (card != null)
                                {
                                    CardSlot slot = GetSlot(SlotType.Player, commonVector.aLong[i + 4], commonVector.aLong[i + 5]);
                                    if (slot != null)
                                        card.MoveToSlot(slot);
                                }
                                card.SetTired(commonVector.aLong[i + 6]);
                                card.SetMovable(commonVector.aLong[i + 6] == 0);
                                card.MoveFail();
                            }
                        }
                        else
                        {
                            for (int i = 1; i < commonVector.aLong.Count; i += 7)
                            {
                                BoardCard card = GetBoardCard(commonVector.aLong[i]);
                                if (card != null)
                                {
                                    CardSlot slot = GetSlot(SlotType.Enemy, commonVector.aLong[i + 4], commonVector.aLong[i + 5]);
                                    if (slot != null)
                                        card.MoveToSlot(slot);
                                }
                                card.SetTired(commonVector.aLong[i + 6]);
                                card.SetMovable(commonVector.aLong[i + 6] == 0);
                            }
                        }
                        delayTime += 0.3f;
                    }
                    break;
            }
        }
        yield return new WaitForSeconds(delayTime);
        Debug.Log("end move card");
        onProcessData = false;

    }

    private IEnumerator CreateCard(long battleID, long heroID, long frame, long atk, long hp, long mana, GameObject spawnObject, Transform spawnPos, Transform effectToSpawn, CardSlot targetSlot, CardOwner owner, float delay, ICallback.CallFunc2<BoardCard> callback = null)
    {
        if (effectToSpawn != null)
        {
            Transform trans = PoolManager.Pools["Effect"].Spawn(effectToSpawn);
            trans.position = targetSlot.transform.position;
            trans.GetComponent<ParticleEffectParentCallback>()
                .SetOnPlay();
        }
        yield return new WaitForSeconds(delay);
        Transform spawnCard = PoolManager.Pools["Card"].Spawn(spawnObject);
        spawnCard.parent = targetSlot != null ? targetSlot.transform : null;
        spawnCard.position = targetSlot != null ? targetSlot.transform.position : spawnPos.position;
        spawnCard.localRotation = Quaternion.Euler(Vector3.zero);

        BoardCard card = spawnCard.GetComponent<BoardCard>();
        card.SetBoardCardData(battleID, heroID, frame, atk, hp, mana, owner, targetSlot);
        if (targetSlot != null)
        {
            targetSlot.ChangeSlotState(SlotState.Full, card);
            card.slot = targetSlot;
            card.SetSummonAnimation();
            card.UpdatePosition();
        }
        lstCardInBattle.Add(card);
        Debug.Log("add card");
        callback?.Invoke(card);
    }
    private IEnumerator CreateCardOnEffect(long battleID, long heroID, long frame, long atk, long hp, long mana, GameObject spawnObject, Transform spawnPos, Transform effectToSpawn, CardSlot targetSlot, CardOwner owner, float delay, ICallback.CallFunc2<BoardCard> callback = null)
    {
        if (effectToSpawn != null)
        {
            Transform trans = PoolManager.Pools["Effect"].Spawn(effectToSpawn);
            trans.position = targetSlot.transform.position;
            trans.GetComponent<ParticleEffectParentCallback>()
                .SetOnPlay();
        }
        yield return new WaitForSeconds(delay);
        Transform spawnCard = PoolManager.Pools["Card"].Spawn(spawnObject);
        spawnCard.parent = targetSlot != null ? targetSlot.transform : null;
        spawnCard.position = targetSlot != null ? targetSlot.transform.position : spawnPos.position;
        spawnCard.localRotation = Quaternion.Euler(Vector3.zero);

        BoardCard card = spawnCard.GetComponent<BoardCard>();
        card.SetBoardCardData(battleID, heroID, frame, atk, hp, mana, owner, targetSlot);
        if (targetSlot != null)
        {
            targetSlot.ChangeSlotState(SlotState.Full, card);
            card.slot = targetSlot;

            //card.SetSummonAnimation();
            card.UpdatePosition();
        }
        lstCardInBattle.Add(card);
        Debug.Log("add card on eff");
        callback?.Invoke(card);
    }

    private IEnumerator CreateCardEffectAnimation(long battleID, long heroID, long frame, long atk, long hp, long mana, GameObject spawnObject, Transform spawnPos, Transform effectToSpawn, CardSlot targetSlot, CardOwner owner, float delay, ICallback.CallFunc2<BoardCard> callback = null)
    {
        if (effectToSpawn != null)
        {
            Transform trans = PoolManager.Pools["Effect"].Spawn(effectToSpawn);
            trans.position = targetSlot.transform.position;
            trans.GetComponent<AnimationParentCallback>()
                .SetOnEndAnim(() =>
                {
                    PoolManager.Pools["Effect"].Despawn(trans);
                });
        }
        yield return new WaitForSeconds(delay);
        Transform spawnCard = PoolManager.Pools["Card"].Spawn(spawnObject);
        spawnCard.parent = targetSlot != null ? targetSlot.transform : null;
        spawnCard.position = targetSlot != null ? targetSlot.transform.position : spawnPos.position;
        spawnCard.localRotation = Quaternion.Euler(Vector3.zero);

        BoardCard card = spawnCard.GetComponent<BoardCard>();
        card.SetBoardCardData(battleID, heroID, frame, atk, hp, mana, owner, targetSlot);
        if (targetSlot != null)
        {
            targetSlot.ChangeSlotState(SlotState.Full, card);
            card.slot = targetSlot;
            //card.SetSummonAnimation();
            card.UpdatePosition();
        }
        lstCardInBattle.Add(card);

        callback?.Invoke(card);
    }

    private IEnumerator CreateCardReplaceOnEffect(long battleID, long newHeroID, long newFrame, long newAtk, long newHp, long newMana, Transform effectToSpawn, float delay, ICallback.CallFunc2<BoardCard> callback = null)
    {
        BoardCard card = lstCardInBattle.FirstOrDefault(x => x.battleID == battleID);
        if (card != null)
        {
            if (effectToSpawn != null)
            {
                Transform trans = PoolManager.Pools["Effect"].Spawn(effectToSpawn);
                trans.position = card.slot.transform.position;
                trans.GetComponent<ParticleEffectParentCallback>()
                    .SetOnPlay();
            }
            yield return new WaitForSeconds(delay);

            card.SetBoardCardData(battleID, newHeroID, newFrame, newAtk, newHp, newMana, card.cardOwner, card.slot);
            
        }
        callback?.Invoke(card);
    }
    private IEnumerator GameSumonCardInBattle(ListAction listAction)
    {
        yield return new WaitForSeconds(0f);
        onProcessData = true;
        float delayTime = 0f;
        float skillWaitTime = 2f;
        bool isPlayer = false;
        battleState = BATTLE_STATE.NORMAL;
        foreach (Action a in listAction.aAction)
        {
            switch (a.actionId)
            {
                case IService.GAME_SUMMON_CARD_IN_BATTLE_DETAIL:
                    CommonVector commonVector = ISocket.Parse<CommonVector>(a.data);
                    WriteLogBattle("SUMMON CARD: ", string.Join(",", commonVector.aString), string.Join(",", commonVector.aLong));
                    if (commonVector.aLong[0] == 0)
                    {
                        Toast.Show(commonVector.aString[0]);
                        // hiennt fail to summon
                    }
                    else
                    {

                        long mana = commonVector.aLong[1];
                        onUpdateMana?.Invoke(GameData.main.profile.username.Equals(commonVector.aString[0]) ? 0 : 1, mana, ManaState.UseDone, 0);

                        //Destroy(currentGodCardUI);

                        DBHero heroSummon = Database.GetHero(commonVector.aLong[3]);
                        // is god
                        isPlayer = GameData.main.profile.username.Equals(commonVector.aString[0]);
                        if (GameData.main.profile.username.Equals(commonVector.aString[0]))
                        {
                            if (commonVector.aLong[8] == -1 || commonVector.aLong[9] == -1)
                            {
                                HandCard card = GetHandCard(commonVector.aLong[2]);
                                if (card != null)
                                {
                                    StartCoroutine(CreateCard(commonVector.aLong[2], commonVector.aLong[3], commonVector.aLong[4], commonVector.aLong[5], commonVector.aLong[6], commonVector.aLong[7], m_SpellOnBoard, magicSpawnPosition, null, null, CardOwner.Player, 0, (boardCard) =>
                                    {
                                        //CheckHeroSkill(TYPE_WHEN_START_TURN, boardCard);
                                    }));
                                    card.MoveFail();
                                    Decks[0].RemoveCard(card);
                                    Decks[0].ReBuildDeck(0);
                                    PoolManager.Pools["Card"].Despawn(card.transform);

                                    //buff  card
                                    if (heroSummon.type == DBHero.TYPE_BUFF_MAGIC)
                                    {
                                        //bo bai buff trung tren tay
                                        HandCard cardBuffRemove = GetHandCard(commonVector.aLong[12]);
                                        if (cardBuffRemove != null)
                                        {
                                            cardBuffRemove.DiscardHandCardPlayer(showOpponentCardPoint, () =>
                                            {
                                                cardBuffRemove.transform.localRotation = Quaternion.Euler(180f, -90f, 0);
                                                Decks[0].RemoveCard(cardBuffRemove);
                                                Decks[0].ReBuildDeck(0);
                                                PoolManager.Pools["Card"].Despawn(cardBuffRemove.transform);
                                            });

                                        }
                                        else
                                        {
                                            DBHero hero = Database.GetHero(commonVector.aLong[3]);
                                            DiscardANewCard(0, hero, commonVector.aLong[12], commonVector.aLong[4], commonVector.aLong[5], commonVector.aLong[6], commonVector.aLong[7], false, card =>
                                            {
                                                foreach (HandCard c in Decks[0].GetListCard)
                                                    c.isMoving = false;
                                            });

                                        }

                                        // boc them bai neu bi huy bai tren tay 
                                        if (commonVector.aLong[13] != -1)
                                        {
                                            yield return new WaitForSeconds(0.5f);
                                            long battleID = commonVector.aLong[13];
                                            long heroID = commonVector.aLong[14];
                                            long frame = commonVector.aLong[15];
                                            long atk = commonVector.aLong[16];
                                            long hp = commonVector.aLong[17];
                                            long cardMana = commonVector.aLong[18];
                                            DBHero hero = Database.GetHero(heroID);
                                            AddNewCard(0, hero, battleID, frame, atk, hp, cardMana);
                                        }
                                        //hieu ung cho god duoc buff + update hero info + god preview 
                                        BoardCard godBuff = GetListPlayerCardInBattle().FirstOrDefault(x => x.battleID == commonVector.aLong[19]);
                                        if (godBuff != null)
                                        {
                                            List<long> lstBuffCard = new List<long>();
                                            lstBuffCard.Add(heroSummon.id);
                                            foreach (long skillBuff in heroSummon.lstBuffSkillID)
                                            {
                                                if (CardData.Instance.GetCardSkillDataInfo(godBuff.heroID).skillIds.Contains(skillBuff))
                                                {
                                                    godBuff.UpdateHeroInfo(lstbuffSkillCards: lstBuffCard);
                                                    godBuff.CheckUnlockSkill(heroSummon, skillBuff);

                                                }
                                            }
                                        }
                                        StartCoroutine(HideMagicCard(commonVector.aLong[2], -1, true));

                                    }
                                }
                            }
                            else
                            {
                                CardSlot slot = GetSlot(SlotType.Player, commonVector.aLong[8], commonVector.aLong[9]);
                                if (slot != null)
                                {
                                    if (heroSummon.type == DBHero.TYPE_GOD)
                                    {
                                        Transform spawnEffect = null;
                                        float delay = 0;
                                        if (commonVector.aLong[3] == 1)
                                        {
                                            spawnEffect = natureVineSpawnEffect;
                                            delay = 0.4f;
                                        }
                                        else
                                        {
                                            spawnEffect = holySpawnEffect;
                                            delay = 0.1f;
                                        }
                                        //sua frame
                                        StartCoroutine(CreateCard(commonVector.aLong[2], commonVector.aLong[3], commonVector.aLong[4], commonVector.aLong[5], commonVector.aLong[6], commonVector.aLong[7], m_GodCard, slot.transform, null, slot, CardOwner.Player, delay, (card) =>
                                        {
                                            card.isFragile = commonVector.aLong[11] != 0;

                                            card.isRevival = commonVector.aLong[10] != 0;
                                            if (!card.isRevival)
                                            {
                                                string voiceName = card.heroInfo.heroNumber.ToString() + "_" + VoiceData.TYPE_SUMMON + "_" + Random.Range(1, 3).ToString();
                                                //CheckHeroSkill(TYPE_WHEN_SUMON, card,slot.xPos,slot.yPos);
                                                AudioClip clip = BundleHandler.LoadSound(voiceName, "voicegod");
                                                if (clip != null)
                                                    SoundHandler.main.PlaySFX(voiceName, "voicegod");
                                            }
                                            else
                                            {
                                                string voiceName = card.heroInfo.heroNumber.ToString() + "_" + VoiceData.TYPE_REVIVAL + "_" + Random.Range(1, 3).ToString();
                                                //CheckHeroSkill(TYPE_WHEN_SUMON, card,slot.xPos,slot.yPos);
                                                AudioClip clip = BundleHandler.LoadSound(voiceName, "voicegod");
                                                if (clip != null)
                                                    SoundHandler.main.PlaySFX(voiceName, "voicegod");
                                            }
                                            SoundHandler.main.PlaySFX("SummonCard", "sounds");
                                            card.Placed();
                                        }));
                                        //doi trang thai cua god ui
                                        onSpawnRandomGod?.Invoke(commonVector.aLong[3]);
                                    }
                                    else
                                    {
                                        HandCard cardToRemove = GetHandCard(commonVector.aLong[2]);
                                        if (cardToRemove != null)
                                        {
                                            //sua frame
                                            Transform effectSpawnCard = null;
                                            if (cardToRemove.heroID == 149)
                                                effectSpawnCard = hadesSkillSummonEffect;
                                            StartCoroutine(CreateCard(commonVector.aLong[2], commonVector.aLong[3], commonVector.aLong[4], commonVector.aLong[5], commonVector.aLong[6], commonVector.aLong[7], m_MinionOnBoardCard, slot.transform, effectSpawnCard, slot, CardOwner.Player, 0.4f, (card) =>
                                            {
                                                card.isFragile = commonVector.aLong[11] != 0;
                                                //CheckHeroSkill(TYPE_WHEN_SUMON, card,slot.xPos,slot.yPos);
                                                SoundHandler.main.PlaySFX("SummonCard", "sounds");

                                            }));
                                            Decks[0].RemoveCard(cardToRemove);
                                            cardToRemove.MoveFail();
                                            PoolManager.Pools["Card"].Despawn(cardToRemove.transform);
                                        }
                                    }

                                }
                            }
                        }
                        else
                        {
                            if (commonVector.aLong[8] == -1 || commonVector.aLong[9] == -1)
                            {
                                var card = Decks[1].GetListCard.FirstOrDefault(x => x != null);
                                if (card != null)
                                {
                                    card.SetHandCardData(commonVector.aLong[2], commonVector.aLong[3], commonVector.aLong[4], CardOwner.Enemy, commonVector.aLong[5], commonVector.aLong[6], commonVector.aLong[7]);
                                    Decks[1].RemoveCard(card, 1);
                                    card.ShowSpellEnemyCardOnScreen(showOpponentCardPoint, () =>
                                    {
                                        //sua frame
                                        StartCoroutine(CreateCard(commonVector.aLong[2], commonVector.aLong[3], commonVector.aLong[4], commonVector.aLong[5], commonVector.aLong[6], commonVector.aLong[7], m_SpellOnBoard, magicSpawnPosition, null, null, CardOwner.Enemy, 0.4f, (boardCard) =>
                                        {
                                            SoundHandler.main.PlaySFX("SummonCard", "sounds");
                                        }));
                                    });
                                    if (heroSummon.type == DBHero.TYPE_BUFF_MAGIC)
                                    {
                                        //bo bai buff trung tren tay

                                        //bỏ đoạn này vì k biet duoc bai buff bi bo cua dich co o tren tay hay khong
                                        //if (commonVector.aLong[12] != -1)
                                        //{
                                        //    var removeCard = Decks[1].GetListCard.FirstOrDefault(x => x != null);
                                        //    Decks[1].RemoveCard(removeCard, 1);
                                        //}
                                        // boc them bai neu bi huy bai tren tay 
                                        if (commonVector.aLong[13] != -1)
                                        {
                                            if (Decks[1].GetListCard.Count < 10)
                                            {
                                                DBHero hero = new DBHero();
                                                hero.id = -1;
                                                AddNewCard(1, hero, -1, 1);
                                            }
                                        }
                                        BoardCard godBuff = GetListEnemyCardInBattle().FirstOrDefault(x => x.battleID == commonVector.aLong[19]);
                                        if (godBuff != null)
                                        {
                                            List<long> lstBuffCard = new List<long>();
                                            lstBuffCard.Add(heroSummon.id);
                                            foreach (long skillBuff in heroSummon.lstBuffSkillID)
                                            {
                                                if (CardData.Instance.GetCardSkillDataInfo(godBuff.heroID).skillIds.Contains(skillBuff))
                                                {
                                                    godBuff.CheckUnlockSkill(heroSummon, skillBuff);
                                                    godBuff.UpdateHeroInfo(lstbuffSkillCards: lstBuffCard);
                                                }
                                            }
                                        }
                                        StartCoroutine(HideMagicCard(commonVector.aLong[2], -1, false));
                                        // update god preview 
                                    }
                                }

                            }
                            else
                            {
                                CardSlot slot = enemySlotContainer.FirstOrDefault(x => x.xPos == commonVector.aLong[8] && x.yPos == commonVector.aLong[9]);
                                if (slot != null)
                                {
                                    if (heroSummon.type == DBHero.TYPE_GOD)
                                    {
                                        Transform spawnEffect = null;
                                        float delay = 0;
                                        if (commonVector.aLong[3] == 1)
                                        {
                                            spawnEffect = natureVineSpawnEffect;
                                            delay = 0.4f;
                                        }
                                        else
                                        {
                                            spawnEffect = holySpawnEffect;
                                            delay = 0.1f;
                                        }
                                        //sua frame
                                        StartCoroutine(CreateCard(commonVector.aLong[2], commonVector.aLong[3], commonVector.aLong[4], commonVector.aLong[5], commonVector.aLong[6], commonVector.aLong[7], m_EnemyGodOnBoardCard, slot.transform, null, slot, CardOwner.Enemy, delay, (card) =>
                                        {
                                            card.isFragile = commonVector.aLong[11] != 0;
                                            card.isRevival = commonVector.aLong[10] != 0;

                                            if (!card.isRevival)
                                            {
                                                string voiceName = card.heroInfo.heroNumber.ToString() + "_" + VoiceData.TYPE_SUMMON + "_" + Random.Range(1, 3).ToString();
                                                //CheckHeroSkill(TYPE_WHEN_SUMON, card,slot.xPos,slot.yPos);
                                                AudioClip clip = BundleHandler.LoadSound(voiceName, "voicegod");
                                                if (clip != null)
                                                    SoundHandler.main.PlaySFX(voiceName, "voicegod");
                                            }
                                            else
                                            {
                                                string voiceName = card.heroInfo.heroNumber.ToString() + "_" + VoiceData.TYPE_REVIVAL + "_" + Random.Range(1, 3).ToString();
                                                //CheckHeroSkill(TYPE_WHEN_SUMON, card,slot.xPos,slot.yPos);
                                                AudioClip clip = BundleHandler.LoadSound(voiceName, "voicegod");
                                                if (clip != null)
                                                    SoundHandler.main.PlaySFX(voiceName, "voicegod");
                                            }
                                            SoundHandler.main.PlaySFX("SummonCard", "sounds");
                                        }));
                                        onSpawnRandomGodEnemy?.Invoke(commonVector.aLong[3]);
                                    }
                                    else
                                    {
                                        var card = Decks[1].GetListCard.FirstOrDefault(x => x != null);
                                        if (card != null)
                                        {
                                            card.SetHandCardData(commonVector.aLong[2], commonVector.aLong[3], commonVector.aLong[4], CardOwner.Enemy, commonVector.aLong[5], commonVector.aLong[6], commonVector.aLong[7]);
                                            Decks[1].RemoveCard(card, 1);
                                            Transform effectSpawnCard = null;
                                            if (commonVector.aLong[3] == 149)
                                                effectSpawnCard = hadesSkillSummonEffect;
                                            card.ShowSpellEnemyCardOnScreen(showOpponentCardPoint, () =>
                                            {
                                                //sua frame
                                                StartCoroutine(CreateCard(commonVector.aLong[2], commonVector.aLong[3], commonVector.aLong[4], commonVector.aLong[5], commonVector.aLong[6], commonVector.aLong[7], m_EnemyMinionOnBoardCard, slot.transform, effectSpawnCard, slot, CardOwner.Enemy, 0.4f, (boardCard) =>
                                                {
                                                    boardCard.isFragile = commonVector.aLong[11] != 0;
                                                    SoundHandler.main.PlaySFX("SummonCard", "sounds");
                                                }));
                                                //PoolManager.Pools["Card"].Despawn(card.transform);
                                            });

                                        }
                                    }
                                }
                            }
                        }
                        bool firstCleave = commonVector.aLong[20] == 1;
                        bool firstBreaker = commonVector.aLong[21] == 1;
                        bool firsstOverrun = commonVector.aLong[22] == 1;
                        bool firstCombo = commonVector.aLong[23] == 1;
                        bool firstPierce = commonVector.aLong[24] == 1;
                        bool firstGodslayer = commonVector.aLong[25] == 1;
                        bool firstBackdoor = commonVector.aLong[26] == 1;
                        bool firstDefender = commonVector.aLong[27] == 1;
                        bool firstFragile = commonVector.aLong[28] == 1;

                        float count = 0;
                        if (firstCleave)
                        {
                            string txt = "";
                            if (heroSummon.cleave == 1)
                            {
                                txt = "<link=\"cleave\"><sprite=15></link>";
                            }
                            else if (heroSummon.cleave == 2)
                            {
                                txt = "<link=\"cleave\"><sprite=14></link>";
                            }
                            else
                            {
                                txt = "<link=\"cleave\"><sprite=13></link>";
                            }
                            StartCoroutine(DelayCallback(count, () =>
                            {
                                UIManager.instance.OnMeetFirstKeyword(txt, "Cleave", heroSummon.cleave);
                            }));
                            count+=2;
                        }
                        if (firstBreaker)
                        {
                            string txt = "";
                            if (heroSummon.breaker == 1)
                            {
                                txt = "<link=\"breaker\"><sprite=12></link>";
                            }
                            else if (heroSummon.breaker == 2)
                            {
                                txt = "<link=\"breaker\"><sprite=11></link>";
                            }
                            else if (heroSummon.breaker == 3)
                            {
                                txt = "<link=\"breaker\"><sprite=19></link>";
                            }
                            else
                            {
                                txt = "<link=\"breaker\"><sprite=20></link>";
                            }

                            StartCoroutine(DelayCallback(count, () =>
                            {
                                UIManager.instance.OnMeetFirstKeyword(txt, "Breaker", heroSummon.breaker);
                            }));
                            count+=2;
                        } 
                        if(firsstOverrun)
                        {
                            string txt = "<link=\"overrun\"><sprite=0></link>";
                            StartCoroutine(DelayCallback(count, () =>
                            {
                                UIManager.instance.OnMeetFirstKeyword(txt, "Overrun");
                            }));
                            count+=2;
                        }  
                        if(firstCombo)
                        {
                            string txt = "<link=\"combo\"><sprite=8></link>";
                            StartCoroutine(DelayCallback(count, () =>
                            {
                                UIManager.instance.OnMeetFirstKeyword(txt, "Combo");
                            }));
                            count+=2;
                        }   
                        if(firstPierce)
                        {
                            string txt = "<link=\"pierce\"><sprite=16></link>";
                            StartCoroutine(DelayCallback(count, () =>
                            {
                                UIManager.instance.OnMeetFirstKeyword(txt, "Pierce");
                            }));
                            count+=2;
                        }   
                        if(firstGodslayer)
                        {
                            string txt = "";
                            if (heroSummon.godSlayer == 1)
                            {
                                txt = "<link=\"godslayer\"><sprite=17></link>";
                            }
                            else
                            {
                                txt = "<link=\"godslayer\"><sprite=18></link>";
                            }
                            StartCoroutine(DelayCallback(count, () =>
                            {
                                UIManager.instance.OnMeetFirstKeyword(txt, "Godslayer", heroSummon.godSlayer);
                            }));
                            count+=2;
                        }
                        if (firstBackdoor)
                        {
                            string txt = "";
                            StartCoroutine(DelayCallback(count, () =>
                            {
                                UIManager.instance.OnMeetFirstKeyword(txt, "Backdoor");
                            }));
                            count +=2;
                        }
                        if (firstDefender)
                        {
                            string txt = "";
                            StartCoroutine(DelayCallback(count, () =>
                            {
                                UIManager.instance.OnMeetFirstKeyword(txt, "Defender");
                            }));
                            count += 2;
                        }
                        if (firstFragile)
                        {
                            string txt = "<link=\"fraglie\"><sprite=7></link>";
                            StartCoroutine(DelayCallback(count, () =>
                            {
                                UIManager.instance.OnMeetFirstKeyword(txt, "Fragile");
                            }));
                        }
                        delayTime += 2f;
                    }

                    break;


                case IService.GAME_SIMULATE_SKILLS_ON_BATTLE:



                    if (!isPlayer)
                        yield return new WaitForSeconds(skillWaitTime);

                    ListAction listActionSkill = ISocket.Parse<ListAction>(a.data);

                    bool needWait = false;
                    foreach (Action aa in listActionSkill.aAction)
                    {
                        CommonVector c = ISocket.Parse<CommonVector>(aa.data);

                        if (c.aLong.Count > 1)
                        {
                            long effectId = c.aLong[1];
                            if (effectId == DBHeroSkill.EFFECT_FIGHT)
                                needWait = true;
                        }
                    }

                    if (needWait)
                    {
                        yield return new WaitForSeconds(1.5f);
                        delayTime += 1.5f;
                    }

                    //StartCoroutine(SimulateSkillEffect(listActionSkill, true));
                    //delayTime += CalculateTimeForSkill(listActionSkill);
                    Debug.Log("add skill");
                    yield return new WaitForSeconds(1);
                    delayTime += 1;
                    foreach (Action ac in listActionSkill.aAction)
                        lstSkillQueue.Add(ac);
                    break;

                case IService.GAME_UPDATE_HERO_MATRIC:

                    // doi summon xong moi update
                    yield return new WaitForSeconds(1f);
                    delayTime += 1f;

                    CommonVector cv = ISocket.Parse<CommonVector>(a.data);
                    WriteLogBattle("UPDATE_HERO_MATRIC", string.Join(",", cv.aString), string.Join(",", cv.aLong));
                    yield return new WaitForSeconds(0.5f);
                    StartCoroutine(UpdateHeroMatric(cv, true));

                    delayTime += 0.5f;

                    break;

            }
        }

        yield return new WaitForSeconds(delayTime);
        Debug.Log("end summond card");
        onProcessData = false;
    }
    private IEnumerator DelayCallback(float time, ICallback.CallFunc cb)
    {
        yield return new WaitForSeconds(time);
        cb?.Invoke();
    }    
    private IEnumerator GameConfirmStartBattle(ListAction listAction)
    {
        yield return new WaitForSeconds(0);
        onProcessData = true;

        LogWriterHandle.WriteLog("GameConfirmStartBattle=" + listAction.aAction.Count);
        IsYourTurn = false;

        onGameConfirmStartBattle?.Invoke();
        foreach (Action a in listAction.aAction)
        {

            switch (a.actionId)
            {
                case IService.GAME_PREPARE_SIMULATE_BATTLE:
                    {

                        yield return new WaitForSeconds(1);
                        ListAction listAction1 = ISocket.Parse<ListAction>(a.data);

                        foreach (Action action in listAction1.aAction)
                        {
                            if (action.actionId == IService.GAME_SIMULATE_SKILLS_ON_BATTLE)
                            {
                                ListAction listAction2 = ISocket.Parse<ListAction>(action.data);
                                foreach (Action ac in listAction2.aAction)
                                    lstSkillQueue.Add(ac);
                            }
                        }

                        break;
                    }

                case IService.GAME_CONFIRM_STARTBATTLE_DETAIL:

                    yield return new WaitForSeconds(0);

                    CommonVector commonVector = ISocket.Parse<CommonVector>(a.data);

                    long nextIndex = commonVector.aLong[1];
                    WriteLogBattle("GAME_CONFIRM_STARTBATTLE_DETAIL", string.Join(",", commonVector.aString), string.Join(",", commonVector.aLong));
                    SkillFailCondition();

                    // check 2 ben deu xong
                    if (nextIndex == -1)
                    {
                        yield return new WaitForSeconds(0.5f);
                        onGameBattleChangeTurn?.Invoke(1);
                        StartCoroutine(StartChooseWay(GameData.main.profile.username.Equals(commonVector.aString[2])));
                        ButtonOrbController.instance.UpdateBattleSword(commonVector.aString[0], commonVector.aString[2], GameState.Combat);
                    }
                    else
                    {
                        roundCount += 1;
                        onGameBattleChangeTurn?.Invoke(0);
                        StartCoroutine(GameStartBattleSimulation(GetTurnEffect()));

                        IsYourTurn = GameData.main.profile.username.Equals(commonVector.aString[1]);
                        ButtonOrbController.instance.UpdateBattleSword(commonVector.aString[1], commonVector.aString[2], GameState.ChangeTurn);
                    }

                    break;
            }
        }
        onResetAttackCount?.Invoke();
        yield return new WaitForSeconds(1f);
        onProcessData = false;

    }
    IEnumerator StartChooseWay(bool isPlayer)
    {
        turnEffect.ForEach(x => x.SetActive(false));
        yield return new WaitForSeconds(0.5f);
        onGameChooseWay?.Invoke(isPlayer);

        if (isPlayer)
            ChooseWayRequest(turnCount % 2 == 0 ? 0 : 1);
        SoundHandler.main.PlaySFX(turnCount % 2 == 0 ? "AttackPhase1" : "AttackPhase2", "sounds");
    }

    private IEnumerator GameChooseWayRequest(CommonVector commonVector)
    {
        yield return new WaitForSeconds(0f);
        onProcessData = true;

        WriteLogBattle("CHOOSE_WAY: ", string.Join(",", commonVector.aString), string.Join(",", commonVector.aLong));
        onGameChooseWayRequest?.Invoke();

        yield return new WaitForSeconds(0.5f);
        onProcessData = false;
    }

    private IEnumerator GameSimulateBattle(ListAction listAction)
    {
        yield return new WaitForSeconds(waitTimeToBattle + 0.5f);
        onProcessData = true;

        foreach (Action a in listAction.aAction)
        {
            switch (a.actionId)
            {
                case IService.GAME_BATTLE_ATTACK:
                    yield return new WaitForSeconds(0.5f);
                    CommonVector cv = ISocket.Parse<CommonVector>(a.data);
                    WriteLogBattle("BATTLE_ATTACK: ", "", string.Join(",", cv.aLong));
                    yield return new WaitForSeconds(waitTimeToEffectSkill);
                    waitTimeToEffectSkill = 0;
                    long totalCount = cv.aLong[4] + 5;
                    //long totalCount = cv.aLong[2] + 3;
                    for (int i = 0; i < cv.aLong.Count; i += (int)totalCount)
                    {
                        var atkCard = lstCardInBattle.FirstOrDefault(c => c.battleID == cv.aLong[i]);
                        // atk tower
                        if (cv.aLong[i + 1] < 0)
                        {

                            var lstTower = lstTowerInBattle.Where(t => cv.aLong[i + 1] < -10 ? t.pos == GetClientPosFromServerPos(1) : t.pos == GetClientPosFromServerPos(0));
                            long id = cv.aLong[i + 1] < -10 ? (long)Mathf.Abs(cv.aLong[i + 1]) - 11 : (long)Mathf.Abs(cv.aLong[i + 1]) - 1;
                            var defTower = lstTower.FirstOrDefault(t => t.id == id);
                            if (atkCard != null && defTower != null)
                            {
                                atkCard.OnAttackTower(defTower, out float waitTime);
                                yield return new WaitForSeconds(waitTime);
                                waitTimeToBattle += waitTime;
                            }
                        }
                        // atk card
                        else
                        {
                            var defCard = lstCardInBattle.FirstOrDefault(c => c.battleID == cv.aLong[i + 1]);
                            if (atkCard != null && defCard != null)
                            {
                                atkCard.OnAttackCard(defCard, out float waitTime);
                                yield return new WaitForSeconds(waitTime);
                                waitTimeToBattle += waitTime;
                            }
                        }

                        int count = i + 6;

                        List<SkillEffect> effects = new List<SkillEffect>();
                        if (cv.aLong[i + 2] > 0)
                        {
                            SkillEffect eff = new SkillEffect();
                            eff.typeEffect = DBHero.KEYWORD_GODSLAYER;
                            effects.Add(eff);
                        }
                        if (cv.aLong[i + 3] > 0)
                        {
                            SkillEffect eff = new SkillEffect();
                            eff.typeEffect = DBHero.KEYWORD_DEFENDER;
                            effects.Add(eff);
                        }
                        // them ca eff cho breaker va godSlayer

                        while (count < i + cv.aLong[i + 4] + 5)
                        {
                            // loai effect
                            if (cv.aLong[i + 5] > 0)
                            {
                                long typeEffect = cv.aLong[count];
                                long defCount = cv.aLong[count + 1];
                                SkillEffect eff = new SkillEffect();
                                eff.typeEffect = typeEffect;
                                eff.defCount = defCount;
                                for (int j = 0; j < defCount; j++)
                                {
                                    // effect here
                                    //cac quan ảnh huong : bai hoac tru,
                                    //switch case cho tung effect.  goi sang card, truyen vao loai effect và so luong bai kem theo id quan bi anh huong 
                                    if (cv.aLong[count + 2 + j] < 0)
                                    {
                                        var lstTower = lstTowerInBattle.Where(t => cv.aLong[i + 1] < -10 ? t.pos == GetClientPosFromServerPos(1) : t.pos == GetClientPosFromServerPos(0));
                                        long id = cv.aLong[i + 1] < -10 ? (long)Mathf.Abs(cv.aLong[i + 1]) - 11 : (long)Mathf.Abs(cv.aLong[i + 1]) - 1;
                                        var defTower = lstTower.FirstOrDefault(t => t.id == id);
                                        eff.lstTowerImpact.Add(defTower);
                                    }
                                    else
                                    {
                                        var defCard = lstCardInBattle.FirstOrDefault(c => c.battleID == cv.aLong[i + 1]);
                                        eff.lstCardImpact.Add(defCard);
                                    }
                                    if (j == defCount - 1)
                                    {
                                        effects.Add(eff);
                                    }
                                }
                                count += (int)defCount + 2;
                                atkCard.DisplayListEffect(effects);
                            }

                        }
                        totalCount = cv.aLong[i + 4] + 5;

                    }
                    yield return new WaitForSeconds(0.25f);
                    break;

                case IService.GAME_BATTLE_DEAL_DAMAGE:
                    CommonVector cv1 = ISocket.Parse<CommonVector>(a.data);
                    WriteLogBattle("DEAL_DAMAGE: ", "", string.Join(",", cv1.aLong));
                    yield return new WaitForSeconds(0.5f);
                    for (int i = 0; i < cv1.aLong.Count; i += 3)
                    {
                        if (cv1.aLong[i] < 0)
                        {
                            TowerDealDamage(cv1.aLong[i], cv1.aLong[i + 1], cv1.aLong[i + 2]);
                        }
                        else
                        {
                            var lstCard = lstCardInBattle.Where(c => c.battleID == cv1.aLong[i]);
                            lstCard.ToList().ForEach(card => card.OnDamaged(cv1.aLong[i + 1], cv1.aLong[i + 2]));
                        }
                    }
                    break;

                case IService.GAME_BATTLE_HERO_DEAD:
                    ListCommonVector lstCv = ISocket.Parse<ListCommonVector>(a.data);
                    yield return new WaitForSeconds(1);
                    foreach (CommonVector c in lstCv.aVector)
                    {
                        WriteLogBattle("HERO_DEAD: ", string.Join(",", c.aString), string.Join(",", c.aLong));
                        //along[0] là hp remain cua tru
                        for (int i = 1; i < c.aLong.Count; i += 3)
                        {
                            var lstCard = lstCardInBattle.Where(s => s.battleID == c.aLong[i]);
                            lstCard.ToList().ForEach(card =>
                            {
                                card.OnDeath();
                                //PoolManager.Pools["Card"].Despawn(card.transform);
                                // remove card in list after destroy
                                SoundHandler.main.PlaySFX("BrokenCard1", "sounds");
                                lstCardInBattle.Remove(card);
                            });

                            // if no damage to tower, continue
                            if (c.aLong[i + 1] == 0)
                            {
                                continue;
                            }
                            else
                            {
                                long towerID = 0;
                                if (GetServerPostFromUsername(c.aString[0]) == 0)
                                {
                                    towerID = c.aLong[i + 2] * -1;
                                }
                                else
                                {
                                    towerID = (c.aLong[i + 2] * -1) - 10;
                                }
                                TowerDealDamage(towerID, c.aLong[i + 1], c.aLong[(int)c.aLong[i + 2] - 1]);
                            }
                        }
                    }
                    break;

                case IService.GAME_BATTLE_HERO_TIRED:
                    CommonVector cv2 = ISocket.Parse<CommonVector>(a.data);
                    WriteLogBattle("HERO_TIRED: ", "", string.Join(",", cv2.aLong));

                    yield return new WaitForSeconds(0.25f);

                    var cardTireds = lstCardInBattle.Where(card => cv2.aLong.Contains(card.battleID));
                    cardTireds.ToList().ForEach(card =>
                    {
                        card.SetTired(1);
                        card.SetMovable(false);
                    });
                    break;
                case IService.GAME_BATTLE_HERO_READY:
                    {
                        CommonVector cv3 = ISocket.Parse<CommonVector>(a.data);
                        WriteLogBattle("HERO_READY: ", GameData.main.profile.username, string.Join(",", cv3.aLong));

                        var cardReady = lstCardInBattle.Where(card => cv3.aLong.Contains(card.battleID));
                        cardReady.ToList().ForEach(card =>
                        {
                            card.SetTired(0);
                            card.SetMovable(true);
                        });

                        break;
                    }
                case IService.GAME_SIMULATE_SKILLS_ON_BATTLE:

                    ListAction listActionSkill = ISocket.Parse<ListAction>(a.data);
                    foreach (Action ac in listActionSkill.aAction)
                        lstSkillQueue.Add(ac);
                    waitTimeToEffectSkill += 1;

                    break;
            }
        }
        yield return new WaitForSeconds(2);

        CommonVector common = new CommonVector();
        Game.main.socket.GameSimulateConfirm(common);

        yield return new WaitForSeconds(0f);
        onProcessData = false;
    }

    void TowerDealDamage(long towerID, long damage, long hpRemain)
    {
        var lstTower = lstTowerInBattle.Where(t => towerID < -10 ? t.pos == GetClientPosFromServerPos(1) : t.pos == GetClientPosFromServerPos(0));
        long id = towerID < -10 ? (long)Mathf.Abs(towerID) - 11 : (long)Mathf.Abs(towerID) - 1;
        var tower = lstTower.FirstOrDefault(t => t.id == id);
        if (tower != null)
        {
            tower.OnDamaged(damage, hpRemain, hpRemain == 0);
            if (hpRemain <= 0)
            {
                onTowerDestroyed?.Invoke(tower.pos, tower.id, false);
            }
        }
    }

    private void GameSimulateConfirm(CommonVector commonVector)
    {
        onProcessData = false;
    }


    private IEnumerator GameBattleEndRound(ListAction lst)
    {

        yield return new WaitForSeconds(0f);
        onProcessData = true;

        foreach (Action a in lst.aAction)
        {
            switch (a.actionId)
            {
                case IService.GAME_SIMULATE_SKILLS_ON_BATTLE:
                    {
                        ListAction listActionSkill = ISocket.Parse<ListAction>(a.data);

                        //StartCoroutine(SimulateSkillEffect(listActionSkill, false));
                        foreach (Action ac in listActionSkill.aAction)
                        {
                            lstSkillQueue.Add(ac);
                        }
                        break;
                    }
            }
        }
        lstCardInBattle.ForEach(x => x.SetTired(0));

        //onUpdateMana?.Invoke(0, 0, -1);
        //onUpdateMana?.Invoke(1, 0, -1);
        //if (currentCardSelected != null)
        //{
        //    if (currentSkillCard != null)
        //    {
        //        currentSkillCard.GetComponent<DragAndDrop>().ResetSlot();
        //        currentSkillCard.currentSlot.ChangeSlotState(SlotState.Empty, currentSkillCard.GetComponent<BoardCard>());
        //        currentSkillCard.currentSlot = null;
        //    }
        //    Decks[0].AddNewCard(currentSkillCard.GetComponent<HandCard>());
        //    Decks[0].ReBuildDeck();
        //    currentCardSelected = null;
        //}
        roundCount = 0;
        turnCount += 1;
        onGameBattleChangeTurn?.Invoke(-1);

        var card = Decks[0].GetListCard.Where(x => x.isFleeting);
        if (card != null)
        {
            card.ToList().ForEach(x =>
            {
                if (x.battleID > 0)
                {
                    Decks[0].RemoveCard(x);
                    PoolManager.Pools["Card"].Despawn(x.transform);
                }
            });
        }
        for (int i = 0; i < lstCardInBattle.Count; i++)
        {
            if (lstCardInBattle[i].isFragile)
            {
                lstCardInBattle[i].OnDeath();
                SoundHandler.main.PlaySFX("BrokenCard1", "sounds");
                lstCardInBattle.Remove(lstCardInBattle[i]);
            }
        }
        Decks[0].ReBuildDeck(0);
        //lstCardInBattle.ForEach(x =>
        //{
        //    if (x.isFragile)
        //    {
        //        x.OnDeath();
        //        SoundHandler.main.PlaySFX("BrokenCard", "sounds");
        //        lstCardInBattle.Remove(x);
        //    }
        //});

        yield return new WaitForSeconds(1f);
        onProcessData = false;
    }

    [SerializeField] Transform rewardPopupPrefab;
    [SerializeField] Transform rewardPopupPrefabProgression;
    private IEnumerator GameBattleEndGame(ListCommonVector lst)
    {
        if(waitTimeToBattle >0)
            yield return new WaitForSeconds(waitTimeToBattle);
        foreach (CommonVector cv in lst.aVector)
            LogWriterHandle.WriteLog("GAME_BATTLE_END_GAME: \nALong: " + string.Join(",", cv.aLong) + "\nAString: " + string.Join(",", cv.aString));
        if (!GameData.main.passFirst10Match)
            Game.main.socket.GetProfile();
        CommonVector cv1 = lst.aVector[0];
        switch (cv1.aLong[0])
        {
            case 0:
                {
                    //unrank
                    GameData.main.currentPlayMode = "unrank";
                    break;
                }
            case 1:
                {
                    //rank
                    GameData.main.currentPlayMode = "rank";
                    break;
                }
        }
        if (cv1.aLong[1] == 1 || cv1.aLong[1] == -1)
        {
            string user = "";
            //UIManager.instance.OnSurrender(1);
            if (GameData.main.profile.username.Equals(cv1.aString[0]))
            {
                SoundHandler.main.PlaySFX("VoiceVictory", "sounds");
                user = LangHandler.Get("147", "Your Opponent") + " ";
            }
            else
            {
                SoundHandler.main.PlaySFX("VoiceDefeat", "sounds");
                user = LangHandler.Get("148", "You") + " ";
            }
            switch (cv1.aLong[2])
            {
                case 0:

                    break;
                case 1:
                    Toast.Show(user + LangHandler.Get("149", "Surrendered."));
                    break;
                case 2:
                    Toast.Show(user + LangHandler.Get("150", "had no card to draw."));
                    break;
                case 3:
                    Toast.Show(user + LangHandler.Get("had enough 12 zodiac on board."));
                    break;
                case 4:
                    Toast.Show(user + LangHandler.Get("152", "AFK."));
                    break;

            }
            yield return new WaitForSeconds(3f);
            onProcessData = true;

            fadedScreen.SetActive(true);
            fadedScreen.GetComponent<Image>().DOFade(1, 0.2f).onComplete += delegate
            {
                if (GameData.main.profile.username.Equals(cv1.aString[0]))
                {
                    victoryRenderTexture.SetActive(true);
                    victoryVideo.clip = CardData.Instance.GetVideo("Victory");
                    victoryVideo.gameObject.SetActive(true);
                    victoryVideo.Play();
                    SoundHandler.main.PlaySFX("Victory", "sounds");
                }
                else
                {
                    defeatedRenderTexture.SetActive(true);
                    defeatedVideo.clip = CardData.Instance.GetVideo("Defeat");
                    //defeatedVideo.url = Path.Combine(Application.streamingAssetsPath, "Videos/Defeat.mp4");
                    defeatedVideo.gameObject.SetActive(true);
                    defeatedVideo.Play();
                    SoundHandler.main.PlaySFX("Defeat", "sounds");
                }
            };


            //Game.main.socket.GameBattleLeave();
            yield return new WaitForSeconds(5);
            SoundHandler.main.Init("BackgroundMusicMain");
            if (lst.aVector.Count >= 4)
            {
                RewardModel model = new RewardModel();
                CommonVector cv2 = lst.aVector[1];
                model.gold = cv2.aLong[0];
                model.currentGold = cv2.aLong[1];
                model.exp = cv2.aLong[2];
                model.currentExp = cv2.aLong[3];
                model.essence = cv2.aLong[4];
                model.currentEssence = cv2.aLong[5];
                model.hasTimeChest = cv2.aLong[6] == 1;
                model.timeChest = cv2.aLong[7];
                model.timeChestText = cv2.aString[0];
                CommonVector cv3 = lst.aVector[2];
                model.rewardNewbie = false;
                //model.rewardNewbie = cv3.aLong[0] == 0 ? true : false;
                //model.rewardNewbieEffect = cv3.aLong[1] == 1 ? true : false;
                //model.rewardNewbieImg = cv3.aString[0];

                CommonVector cv4 = lst.aVector[3];
                model.eloAdd = cv4.aLong[0];
                model.currentElo = cv4.aLong[1];
                model.currentRank = cv4.aLong[2];
                //if (GameData.main.userProgressionState < 15)
                //{
                //    Transform trans = PoolManager.Pools["RewardPopup"].Spawn(rewardPopupPrefabProgression);
                //    trans.SetParent(Game.main.canvas.panelPopup);
                //    trans.localScale = Vector3.one;
                //    trans.localPosition = Vector3.zero;
                //    trans.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
                //    trans.GetComponent<RewardScene>().InitData(model);
                //    trans.GetComponent<Animator>().Play("Spawn");
                //}
                //else
                //{
                //oninitrewardpopup
                if (GameData.main.currentPlayMode == "unrank")
                    OnInitRewardPopup(model);
                else
                {
                    endGameRankUI.SetActive(true);
                    endGameRankUI.GetComponent<EndGameRank>().InitData(model.eloAdd, model.currentElo, model.currentRank);
                    endGameRankUI.GetComponent<EndGameRank>().SetOnComplete(() => OnInitRewardPopup(model));
                }
                //}

                //trans.GetComponent<RewardScene>().onRewardComplete += OnRewardComplete;
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                OnRewardComplete();
            }
            //yield return new WaitForSeconds(0.5f);
            //OnRewardComplete();

        }
        else
        {
            switch (cv1.aLong[2])
            {
                case 0:
                    Toast.Show(LangHandler.Get("154", "Match Draw"));
                    break;
                case 1:
                    break;
                case 2:
                    Toast.Show(LangHandler.Get("155", "Both of players have no card to draw."));
                    break;
                case 3:
                    Toast.Show(LangHandler.Get("156", "Both of players have 12 zodiac on board."));
                    break;
            }
            yield return new WaitForSeconds(3f);
            onProcessData = true;

            fadedScreen.SetActive(true);
            fadedScreen.GetComponent<Image>().DOFade(1, 0.2f).onComplete += delegate
            {
                //Game.main.socket.GameBattleLeave();
                SoundHandler.main.Init("BackgroundMusicMain");
            };
            GameData.main.isUsedUlti = false;
            yield return new WaitForSeconds(0.5f);
            OnRewardComplete();
        }
        onProcessData = false;
    }
    private void OnInitRewardPopup(RewardModel model)
    {
        Transform trans;
        trans = PoolManager.Pools["RewardPopup"].Spawn(rewardPopupPrefab);
        trans.SetParent(Game.main.canvas.panelPopup);
        trans.localScale = Vector3.one;
        trans.localPosition = Vector3.zero;
        trans.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        trans.GetComponent<RewardScene>().InitData(model);
    }
    private void OnRewardComplete()
    {
        LogWriterHandle.WriteLog("Reward____________");
        Game.main.socket.GetMode();
        Game.main.LoadScene("SelectModeScene", delay: 0.3f, curtain: true);
    }

    // Methods
    protected override void Awake()  
    {
        base.Awake();
        instance = this;
    }

    //---
    private IEnumerator InitPlayer()
    {
        yield return new WaitForSeconds(0f);
        onProcessData = true;
        if (!GameData.main.isResume)
        {
            mLstBattlePlayer = GameData.main.mLstBattlePlayer;
            CalculateClientPosstion();
            foreach (var player in mLstBattlePlayer)
            {
                onInitPlayer?.Invoke(player.clientPostion == POS_6h, player.screenname, player.towerHealth);
            }
            UIManager.instance.InitListBidSkill();
        }
        else if (GameData.main.resumeData != null)
        {

            LogWriterHandle.WriteLog("INIT RESUME GAME=" + GameData.main.resumeData.aVector.Count);
            //0,17,1,0,0,1,74,0,0,0
            ListCommonVector lcv = GameData.main.resumeData;
            if (lcv.aVector.Count == 10)
            {
                CommonVector cv0 = lcv.aVector[0];
                LogWriterHandle.WriteLog("PLAYER_STATUS = " + string.Join(",", cv0.aLong) + " String: " + string.Join(",", cv0.aString));
                int BLOCK = 8, BLOCK_STR = 2;
                int num = (cv0.aLong.Count - 2) / BLOCK;
                mLstBattlePlayer = new List<BattlePlayer>();

                for (int i = 0; i < num; i++)
                {
                    BattlePlayer player = new BattlePlayer
                    {
                        username = cv0.aString[i * BLOCK_STR],
                        screenname = cv0.aString[i * BLOCK_STR + 1],
                        position = cv0.aLong[i * BLOCK],
                        id = cv0.aLong[i * BLOCK + 1],
                        state = cv0.aLong[i * BLOCK + 3],
                        mana = cv0.aLong[i * BLOCK + 4],
                        towerHealth = cv0.aLong[i * BLOCK + 5]
                    };
                    if (cv0.aLong[i * BLOCK + 2] == 1)
                    {
                        GameData.main.profile = new UserModel();
                        GameData.main.profile.SetData(player.id, player.username, player.screenname);
                    }
                    mLstBattlePlayer.Add(player);
                    onUpdateMana?.Invoke(GameData.main.profile.username.Equals(player.username) ? 0 : 1, player.mana, ManaState.StartTurn, player.mana);
                    if (GameData.main.profile.username.Equals(player.username))
                    {
                        turnMana = player.mana;
                        GameData.main.userProgressionState = player.state;
                        //if (GameData.main.userProgressionState < 9 && ProgressionController.instance == null)
                        //{
                        //    GameData.main.isResumeOnProgress = true;
                        //    GameObject go = Instantiate(progressionPref);
                        //}
                        int index = 0;
                        var pTower = lstTowerInBattle.Where(x => x.pos == 0);
                        if (pTower != null)
                        {
                            pTower.ToList().ForEach(x =>
                            {
                                x.UpdateHealth(cv0.aLong[i * BLOCK + 5 + index]);
                                if (cv0.aLong[i * BLOCK + 6 + index] <= 0)
                                {
                                    onTowerDestroyed?.Invoke(x.pos, x.id, true);
                                }
                            });
                        }
                        GameData.main.isUsedUlti = cv0.aLong[i * BLOCK + 6] == 1;
                        GameData.main.ultiID = cv0.aLong[i * BLOCK + 7];
                        //dung ulti chua , hero id đã dùng nếu có  
                    }
                    else
                    {
                        int index = 0;
                        var eTower = lstTowerInBattle.Where(x => x.pos == 1);
                        if (eTower != null)
                        {
                            eTower.ToList().ForEach(x =>
                            {
                                x.UpdateHealth(cv0.aLong[i * BLOCK + 5 + index]);
                            });
                        }
                    }
                }
                currentAvailableRegion = cv0.aLong[cv0.aLong.Count - 2];
                //new : Huong tra ve them huong danh khi resume
                turnCount = cv0.aLong[cv0.aLong.Count - 1];
                turnArrow[0].SetActive(turnCount % 2 == 0);
                turnArrow[1].SetActive(turnCount % 2 != 0);
                CalculateClientPosstion();
                foreach (var player in mLstBattlePlayer)
                {
                    onInitPlayer?.Invoke(player.clientPostion == POS_6h, player.screenname, player.towerHealth);
                }
                //----set list card

                IsYourTurn = true;

                CommonVector cv3 = lcv.aVector[3];
                LogWriterHandle.WriteLog("LST_CARD_ON_HAND: " + string.Join(",", cv3.aLong));
                long playerIndex = cv3.aLong[0];
                long numCard = cv3.aLong[1];
                int BLOCK_HERO = 7;
                long numCardEnemy = cv3.aLong[cv3.aLong.Count - 1];
                //======== CARD = 1,15,45,40,0,41,35,0,46,41,0,39,32,0,34,24,0,5
                for (int i = 0; i < numCard; i++)
                {
                    AddListCard(lstHeroPlayer, lstHeroBattleID, lstHeroFrame, lstHeroAtk, lstHeroHp, lstHeroMana, cv3.aLong[2 + i * BLOCK_HERO + 1], cv3.aLong[2 + i * BLOCK_HERO], cv3.aLong[2 + i * BLOCK_HERO + 2], cv3.aLong[2 + i * BLOCK_HERO + 4], cv3.aLong[2 + i * BLOCK_HERO + 5], cv3.aLong[2 + i * BLOCK_HERO + 6], lstHeroFragile, cv3.aLong[2 + i * BLOCK_HERO + 3]);

                }
                DrawDeckStart(0, lstHeroPlayer, lstHeroBattleID, lstHeroFrame, lstHeroAtk, lstHeroHp, lstHeroMana, lstHeroFragile);

                List<DBHero> lstHero = new List<DBHero>();
                List<long> lstID = new List<long>();

                for (int i = 1; i < numCardEnemy; i++)
                {
                    DBHero hero = new DBHero();
                    hero.id = -1;
                    lstHero.Add(hero);
                    lstID.Add(-1);
                }
                DrawDeckStart(1, lstHero, lstID, lstHeroFrame);

                //Lst Card Dead
                lstGodPlayer = new List<DBHero>();
                lstGodEnemy = new List<DBHero>();
                CommonVector cv5 = lcv.aVector[6];
                LogWriterHandle.WriteLog("LST_CARD_DEAD: " + string.Join(",", cv5.aLong));
                if (cv5.aLong.Count > 0)
                {
                    int start5 = 0;
                    int BLOCK5 = 15;
                    long playerIndex51 = cv5.aLong[0];
                    LogWriterHandle.WriteLog("IS ME = " + IsMeByServerPos(playerIndex51) + " " + playerIndex51 + " " + (IsMeByServerPos(playerIndex51) ? 1 : 0));
                    long numCardPlayer51 = cv5.aLong[1];
                    bool isMe5 = IsMeByServerPos(playerIndex51);
                    for (int i = 0; i < numCardPlayer51; i++)
                    {
                        DBHero hero = Database.GetHero(cv5.aLong[2 + i * BLOCK5 + 1]);
                        if (hero != null)
                        {
                            if (hero.type == DBHero.TYPE_GOD)
                            {
                                GameData.main.lstGodDead.Add(cv5.aLong[2 + i * BLOCK5]);
                                //if (isMe5)
                                //    AddListCard(lstGodPlayer, lstGodPlayerBattleID, cv5.aLong[2 + i * BLOCK5 + 1], cv5.aLong[2 + i * BLOCK5]);
                                //else
                                //    AddListCard(lstGodEnemy, lstGodEnemyBattleID, cv5.aLong[2 + i * BLOCK5 + 1], cv5.aLong[2 + i * BLOCK5]);
                            }
                        }

                    }
                    start5 = 2 + (int)numCardPlayer51 * BLOCK5;
                    long playerIndex52 = cv5.aLong[start5];
                    isMe5 = (IsMeByServerPos(playerIndex52));
                    long numCardPlayer52 = cv5.aLong[start5 + 1];

                    for (int i = 0; i < numCardPlayer52; i++)
                    {
                        DBHero hero = Database.GetHero(cv5.aLong[2 + i * BLOCK5 + 1]);
                        if (hero != null)
                        {
                            if (hero.type == DBHero.TYPE_GOD)
                            {
                                GameData.main.lstGodDead.Add(cv5.aLong[start5 + 2 + i * BLOCK5]);
                                //if (isMe5)
                                //{
                                //    AddListCard(lstGodPlayer, lstGodPlayerBattleID, cv5.aLong[start5 + 2 + i * BLOCK5 + 1], cv5.aLong[start5 + 2 + i * BLOCK5]);
                                //}
                                //else
                                //{
                                //    AddListCard(lstGodEnemy, lstGodEnemyBattleID, cv5.aLong[start5 + 2 + i * BLOCK5 + 1], cv5.aLong[start5 + 2 + i * BLOCK5]);
                                //}
                            }
                        }
                    }
                }

                //----set list god
                CommonVector cv6 = lcv.aVector[7];
                LogWriterHandle.WriteLog("LST_CARD_GOD_ORIGIN: " + string.Join(",", cv6.aLong));
                //0,1,1,22,1,5,23,1,24,1,25,1,26,2,27,2
                //sua frame
                int start6 = 0;
                int BLOCK_GOD = 6;
                long playerIndex61 = cv6.aLong[0];
                bool isMe = IsMeByServerPos(playerIndex61);
                long numCardPlayer61 = cv6.aLong[1];
                for (int i = 0; i < numCardPlayer61; i++)
                {
                    if (isMe)
                        AddListCard(lstGodPlayer, lstGodPlayerBattleID, lstGodPlayerFrame, lstGodPlayerAtk, lstGodPlayerHp, lstGodPlayerMana, cv6.aLong[2 + i * BLOCK_GOD + 1], cv6.aLong[2 + i * BLOCK_GOD], cv6.aLong[2 + i * BLOCK_GOD + 2], cv6.aLong[2 + i * BLOCK_GOD + 3], cv6.aLong[2 + i * BLOCK_GOD + 4], cv6.aLong[2 + i * BLOCK_GOD + 5]);
                    else
                        AddListCard(lstGodEnemy, lstGodEnemyBattleID, lstGodEnemyFrame, lstGodEnemyAtk, lstGodEnemyHp, lstGodEnemyMana, cv6.aLong[2 + i * BLOCK_GOD + 1], cv6.aLong[2 + i * BLOCK_GOD], cv6.aLong[2 + i * BLOCK_GOD + 2], cv6.aLong[2 + i * BLOCK_GOD + 3], cv6.aLong[2 + i * BLOCK_GOD + 4], cv6.aLong[2 + i * BLOCK_GOD + 5]);
                }

                start6 = 2 + (int)numCardPlayer61 * BLOCK_GOD;

                long playerIndex62 = cv6.aLong[start6];
                isMe = (IsMeByServerPos(playerIndex62));
                long numCardPlayer62 = cv6.aLong[start6 + 1];
                for (int i = 0; i < numCardPlayer62; i++)
                {
                    if (isMe)
                    {
                        AddListCard(lstGodPlayer, lstGodPlayerBattleID, lstGodPlayerFrame, lstGodPlayerAtk, lstGodPlayerHp, lstGodPlayerMana, cv6.aLong[start6 + 2 + i * BLOCK_GOD + 1], cv6.aLong[start6 + 2 + i * BLOCK_GOD], cv6.aLong[start6 + 2 + i * BLOCK_GOD + 2], cv6.aLong[start6 + 2 + i * BLOCK_GOD + 3], cv6.aLong[start6 + 2 + i * BLOCK_GOD + 4], cv6.aLong[start6 + 2 + i * BLOCK_GOD + 5]);
                    }
                    else
                    {
                        AddListCard(lstGodEnemy, lstGodEnemyBattleID, lstGodEnemyFrame, lstGodEnemyAtk, lstGodEnemyHp, lstGodEnemyMana, cv6.aLong[start6 + 2 + i * BLOCK_GOD + 1], cv6.aLong[start6 + 2 + i * BLOCK_GOD], cv6.aLong[start6 + 2 + i * BLOCK_GOD + 2], cv6.aLong[start6 + 2 + i * BLOCK_GOD + 3], cv6.aLong[start6 + 2 + i * BLOCK_GOD + 4], cv6.aLong[start6 + 2 + i * BLOCK_GOD + 5]);
                    }
                }
                onGameDealCard?.Invoke(lstGodPlayerBattleID, lstGodPlayer, lstGodPlayerFrame, lstGodPlayerAtk, lstGodPlayerHp, lstGodPlayerMana, lstGodEnemyBattleID, lstGodEnemy, lstGodEnemyFrame, lstGodEnemyAtk, lstGodEnemyHp, lstGodPlayerMana);

                //card on table
                //0,1,2,23,1,5,5,0,0,0,0,0,0,0,0,0,0,1
                //0,17,1,0,0,1,74,0,0,0
                CommonVector cv2 = lcv.aVector[2];
                LogWriterHandle.WriteLog("Table state: " + string.Join(",", cv2.aLong));
                if (cv2.aLong.Count > 0)
                {
                    long playerIndex21 = cv2.aLong[0];
                    LogWriterHandle.WriteLog("IS ME = " + IsMeByServerPos(playerIndex21) + " " + playerIndex21 + " " + (IsMeByServerPos(playerIndex21) ? 1 : 0));
                    long numCardPlayer21 = cv2.aLong[1];
                    int START = 0;
                    int TOTAL = 0;
                    int BLOCK_CARD_TABLE = 0;
                    for (int i = 0; i < numCardPlayer21; i++)
                    {
                        long BUFF_SIZE = cv2.aLong[2 + TOTAL + 19 - 1];
                        BLOCK_CARD_TABLE = 19 + (int)BUFF_SIZE;
                        List<long> lstBuffCards = new List<long>();
                        for (int j = 0; j < BUFF_SIZE; j++)
                        {
                            lstBuffCards.Add(cv2.aLong[2 + 19 + TOTAL + j]);
                        }
                        PlaceCardInBattleOnResume(IsMeByServerPos(playerIndex21) ? 0 : 1,
                            cv2.aLong[2 + TOTAL],
                            cv2.aLong[2 + TOTAL + 1],
                            cv2.aLong[2 + TOTAL + 2],
                            cv2.aLong[2 + TOTAL + 3],
                            cv2.aLong[2 + TOTAL + 4],
                            cv2.aLong[2 + TOTAL + 5],
                            cv2.aLong[2 + TOTAL + 6],
                            cv2.aLong[2 + TOTAL + 7],
                            cv2.aLong[2 + TOTAL + 8],
                            cv2.aLong[2 + TOTAL + 9],
                            cv2.aLong[2 + TOTAL + 10],
                            cv2.aLong[2 + TOTAL + 11],
                            cv2.aLong[2 + TOTAL + 12],
                            cv2.aLong[2 + TOTAL + 13],
                            cv2.aLong[2 + TOTAL + 14],
                            cv2.aLong[2 + TOTAL + 15],
                            cv2.aLong[2 + TOTAL + 16],
                            cv2.aLong[2 + TOTAL + 17],
                            lstBuffCards);
                        TOTAL = TOTAL + BLOCK_CARD_TABLE;
                    }
                    START = 2 + TOTAL;
                    TOTAL = 0;
                    if (START < cv2.aLong.Count)
                    {
                        long playerIndex22 = cv2.aLong[START];
                        long numCardPlayer22 = cv2.aLong[START + 1];
                        for (int i = 0; i < numCardPlayer22; i++)
                        {

                            long BUFF_SIZE = cv2.aLong[START + 2 + TOTAL + 19 - 1];
                            BLOCK_CARD_TABLE = 19 + (int)BUFF_SIZE;
                            List<long> lstBuffCards = new List<long>();
                            for (int j = 0; j < BUFF_SIZE; j++)
                            {
                                lstBuffCards.Add(cv2.aLong[START + 2 + 19 + TOTAL + j]);
                            }
                            PlaceCardInBattleOnResume(IsMeByServerPos(playerIndex22) ? 0 : 1,
                                cv2.aLong[START + 2 + TOTAL],
                                cv2.aLong[START + 2 + TOTAL + 1],
                                cv2.aLong[START + 2 + TOTAL + 2],
                                cv2.aLong[START + 2 + TOTAL + 3],
                                cv2.aLong[START + 2 + TOTAL + 4],
                                cv2.aLong[START + 2 + TOTAL + 5],
                                cv2.aLong[START + 2 + TOTAL + 6],
                                cv2.aLong[START + 2 + TOTAL + 7],
                                cv2.aLong[START + 2 + TOTAL + 8],
                                cv2.aLong[START + 2 + TOTAL + 9],
                                cv2.aLong[START + 2 + TOTAL + 10],
                                cv2.aLong[START + 2 + TOTAL + 11],
                                cv2.aLong[START + 2 + TOTAL + 12],
                                cv2.aLong[START + 2 + TOTAL + 13],
                                cv2.aLong[START + 2 + TOTAL + 14],
                                cv2.aLong[START + 2 + TOTAL + 15],
                                cv2.aLong[START + 2 + TOTAL + 16],
                                cv2.aLong[START + 2 + TOTAL + 17],
                                lstBuffCards);
                            TOTAL = TOTAL + BLOCK_CARD_TABLE;
                        }
                    }
                }

                //list card buff da duoc su dung
                CommonVector cv4 = lcv.aVector[4];
                LogWriterHandle.WriteLog("LST_CARD_BUFF: " + string.Join(",", cv4.aLong));
                if (cv4.aLong.Count > 0)
                {
                    int start4 = 0;
                    int BLOCK_GOD4 = 1;
                    long playerIndex41 = cv4.aLong[0];
                    bool isMe4 = IsMeByServerPos(playerIndex41);
                    long numCardPlayer41 = cv4.aLong[1];
                    for (int i = 0; i < numCardPlayer41; i++)
                    {
                        DBHero cardBuff = Database.GetHero(cv4.aLong[2 + i * BLOCK_GOD]);
                        if (isMe)
                        {
                            if (cardBuff != null && cardBuff.type == DBHero.TYPE_BUFF_MAGIC)
                            {
                                List<long> lstBuffCard = new List<long>();
                                lstBuffCard.Add(cardBuff.id);
                                foreach (BoardCard bCard in GetListPlayerCardInBattle())
                                {
                                    if (bCard.heroID == cardBuff.ownerGodID)
                                    {
                                        bCard.UpdateHeroInfo(lstbuffSkillCards: lstBuffCard);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (cardBuff != null && cardBuff.type == DBHero.TYPE_BUFF_MAGIC)
                            {
                                List<long> lstBuffCard = new List<long>();
                                lstBuffCard.Add(cardBuff.id);
                                foreach (BoardCard bCard in GetListEnemyCardInBattle())
                                {
                                    if (bCard.heroID == cardBuff.ownerGodID)
                                    {
                                        bCard.UpdateHeroInfo(lstbuffSkillCards: lstBuffCard);
                                    }
                                }
                            }
                        }
                    }

                    start4 = 2 + (int)numCardPlayer41 * BLOCK_GOD4;

                    long playerIndex42 = cv4.aLong[start4];
                    isMe4 = (IsMeByServerPos(playerIndex42));
                    long numCardPlayer42 = cv4.aLong[start4 + 1];
                    for (int i = 0; i < numCardPlayer42; i++)
                    {
                        DBHero cardBuff = Database.GetHero(cv4.aLong[start4 + 2 + i * BLOCK_GOD]);
                        if (isMe)
                        {
                            if (cardBuff != null && cardBuff.type == DBHero.TYPE_BUFF_MAGIC)
                            {
                                List<long> lstBuffCard = new List<long>();
                                lstBuffCard.Add(cardBuff.id);
                                foreach (BoardCard bCard in GetListPlayerCardInBattle())
                                {
                                    if (bCard.heroID == cardBuff.ownerGodID)
                                    {
                                        bCard.UpdateHeroInfo(lstbuffSkillCards: lstBuffCard);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (cardBuff != null && cardBuff.type == DBHero.TYPE_BUFF_MAGIC)
                            {
                                List<long> lstBuffCard = new List<long>();
                                lstBuffCard.Add(cardBuff.id);
                                foreach (BoardCard bCard in GetListEnemyCardInBattle())
                                {
                                    if (bCard.heroID == cardBuff.ownerGodID)
                                    {
                                        bCard.UpdateHeroInfo(lstbuffSkillCards: lstBuffCard);
                                    }
                                }
                            }
                        }
                    }
                }

                //list scroll skill bid
                CommonVector cv7 = lcv.aVector[8];
                LogWriterHandle.WriteLog("LST_SCROLL: " + string.Join(",", cv7.aLong));
                if (cv7.aLong.Count > 0)
                {
                    for (int i = 1; i < cv7.aLong.Count; i++)
                    {
                        GameData.main.lstSkillAuction.Add(cv7.aLong[i]);
                    }
                    long currentSkill = cv7.aLong[0];
                    UIManager.instance.InitListSkillOnResume(currentSkill);
                }

                const int TIME_ROUND_START = 1;// time counter defaut 60s
                const int TIME_FIRST_SUMMON = 2;// time counter defaut 30s
                const int TIME_CHOSING_WAY = 5;// time counter defaut 60s
                const int TIME_ROUND_COMBAT = 8;// time counter defaut 60s x 3
                const int TIME_ROUND_END = 10; //(not play)
                                               //table status
                const int TIME_ROUND_BID = 11;
                CommonVector cv1 = lcv.aVector[1];
                LogWriterHandle.WriteLog("Table state2: " + string.Join(",", cv1.aLong));
                long currentPlayer = cv1.aLong[0];
                int timingResume = (int)cv1.aLong[1];
                long timeRemain = cv1.aLong[2];

                //Be ghep ho anh doan code nay
                long numberTurn = cv1.aLong[3];
                long round = cv1.aLong[4];
                switch (timingResume)
                {
                    case TIME_ROUND_START:
                        {
                            if (!isGameStarted)
                                isGameStarted = true;
                            StartCoroutine(GameStartBattleSimulation(GetTurnEffect()));
                            IsYourTurn = GameData.main.profile.username.Equals(GetUsernameFromServerPos(currentPlayer));
                            //onUpdateMana?.Invoke(0, numberTurn, ManaState.Update, 0);
                            //onUpdateMana?.Invoke(1, numberTurn, ManaState.Update, 0);
                            onResumeTime?.Invoke(IsMeByServerPos(currentPlayer), !IsMeByServerPos(currentPlayer), timeRemain, 60);
                            //if (nextIndexPlayer == -1)
                            //{
                            //    onResumeRoundCount?.Invoke(0);
                            //    roundCount = 1;
                            //}
                            //else
                            //{
                            //    onResumeRoundCount?.Invoke(-1);
                            //    roundCount = 0;
                            //}
                            roundCount = round;
                            break;
                        }
                    case TIME_FIRST_SUMMON:
                        {
                            //cho phep user ra bai phep, mulligan bài, set button ok đúng trạng thái
                            onResumeTime?.Invoke(true, true, timeRemain, 30);
                            break;
                        }
                    case TIME_CHOSING_WAY:
                        {
                            //chọn ngẫu nhiên đi
                            onResumeTime?.Invoke(IsMeByServerPos(currentPlayer), !IsMeByServerPos(currentPlayer), timeRemain, 60);
                            StartChooseWay(IsMeByServerPos(currentPlayer));
                            break;
                        }
                    case TIME_ROUND_COMBAT:
                        {
                            //ngồi chờ thôi, button hiện đúng trạng thái đang combat, còn lại k phải làm gì cả
                            onGameBattleChangeTurn?.Invoke(-1);
                            break;
                        }
                    case TIME_ROUND_END:
                        {
                            break;
                        }
                    case TIME_ROUND_BID:
                        {
                            onResumeTime?.Invoke(false,false, timeRemain, 20);
                            break;
                        }
                }
 
                //List Bid Status
                CommonVector cv8 = lcv.aVector[9];
                LogWriterHandle.WriteLog("LST_BID_STATUS: " + string.Join(",", cv8.aLong));
                if (cv8.aLong.Count > 0)
                {
                    bool isBidPhase = cv8.aLong[0] == 1;
                    if (isBidPhase)
                    {
                        int START = 1;
                        long playerShard = 0, enemyShard = 0;
                        long playerBid = 0, enemyBid = 0;
                        int BLOCK8 = 2;
                        for (int i = 0; i < cv8.aString.Count; i++)
                        {
                            if (GameData.main.profile.username.Equals(cv8.aString[i]))
                            {
                                playerBid = cv8.aLong[START + i * BLOCK8];
                                playerShard = cv8.aLong[START + (i * BLOCK8) + 1];
                            }
                            else
                            {
                                enemyBid = cv8.aLong[START + i * BLOCK8];
                                enemyShard = cv8.aLong[START + (i * BLOCK8 ) + 1];
                            }
                        }
                        onBidStateStart?.Invoke(BidState.StartBid, playerShard, enemyShard, playerBid, enemyBid,timeRemain );
                        canUpBid = true;
                    }
                }

            }
            GameData.main.isResume = false;
            //yield return new WaitForSeconds(3f);
        }
        yield return new WaitForSeconds(0f);
        onProcessData = false;

    }

    void DrawDeckStart(int index, List<DBHero> heroList = null, List<long> battleIDList = null, List<long> frameList = null, List<long> atkList = null, List<long> hpList = null, List<long> manaList = null, List<long> fragileList = null)
    {
        for (int i = 0; i < heroList.Count; i++)
        {
            Transform cardGO = null;
            if (heroList[i].id > 0)
                cardGO = PoolManager.Pools["Card"].Spawn(m_MinionCard);
            else
                cardGO = PoolManager.Pools["Card"].Spawn(/*m_EnemyMinionCard*/m_MinionCard);
            cardGO.position = spawnPosition[index].position + new Vector3(0, 0.2f, 0);
            cardGO.rotation = Quaternion.Euler(cardGO.rotation.x, 180, cardGO.rotation.z);
            //LogWriterHandle.WriteLog(cardGO.gameObject.name);
            HandCard card = cardGO.GetComponent<HandCard>();

            if (battleIDList[i] > 0)
            {
                card.SetHandCardData(battleIDList[i], heroList[i].id, frameList[i], CardOwner.Player, atkList[i], hpList[i], manaList[i]);
                if (fragileList != null)
                    card.isFleeting = fragileList[i] == 1;
            }
            else
                card.cardOwner = CardOwner.Enemy;
            Decks[index].AddNewCard(card);
            card.transform.localRotation = Quaternion.Euler(180f, -90f, 0);
        }
        //SoundHandler.main.PlaySFX("DrawCard");
        Decks[index].ReBuildDeck(index);
        //Debug.Log(cardGO.transform.localRotation.eulerAngles + "3");
        //Decks[index].RebuildDeckOnDrawDeck(index, showOpponentCardPoint);
    }

    void AddNewCard(int index, DBHero hero = null, long battleID = 0, long frame = 0, long atk = -1, long hp = -1, long mana = -1, bool isFleeting = false, ICallback.CallFunc2<HandCard> callback = null)
    {
        Transform cardGO;
        if (hero.id > 0)
            cardGO = PoolManager.Pools["Card"].Spawn(m_MinionCard);
        else
            cardGO = PoolManager.Pools["Card"].Spawn(/*m_EnemyMinionCard*/m_MinionCard);

        if (isFleeting)
            cardGO.position = Decks[index].transform.position;
        else
            cardGO.position = spawnPosition[index].position + new Vector3(0, 0.2f, 0);
        cardGO.rotation = Quaternion.Euler(0, 0f, 180);
        if(battleID <0)
            Debug.Log(cardGO.rotation.eulerAngles);

        HandCard card = cardGO.GetComponent<HandCard>();

        if (battleID > 0)
        {
            card.SetHandCardData(battleID, hero.id, frame, CardOwner.Player, atk, hp, mana);
            foreach (HandCard c in Decks[0].GetListCard)
                c.isMoving = true;
            card.DrawCardPlayer(showOpponentCardPoint, () =>
            {
                Decks[index].AddNewCard(card);
                card.transform.localRotation = Quaternion.Euler(180f, -90f, 0);
                Decks[index].ReBuildDeck(index, () => callback?.Invoke(card));
            });
        }
        else
        {
            card.cardOwner = CardOwner.Enemy;
            Decks[index].AddNewCard(card);

            card.transform.localRotation = Quaternion.Euler(0f, 0f, 180);
            card.transform.GetChild(0).transform.localRotation = Quaternion.Euler(new Vector3(180, 0, 0)); 
            Decks[index].ReBuildDeck(index, () => { callback?.Invoke(card);
            });

        }



        //Decks[index].RebuildDeckOnAddCard(index, showOpponentCardPoint, card);
    }

    void DiscardANewCard(int index, DBHero hero = null, long battleID = 0, long frame = 0, long atk = -1, long hp = -1, long mana = -1, bool isFleeting = false, ICallback.CallFunc2<HandCard> callback = null)
    {
        Transform cardGO;
        if (hero.id > 0)
            cardGO = PoolManager.Pools["Card"].Spawn(m_MinionCard);
        else
            cardGO = PoolManager.Pools["Card"].Spawn(/*m_EnemyMinionCard*/m_MinionCard);

        if (isFleeting)
            cardGO.position = Decks[index].transform.position;
        else
            cardGO.position = spawnPosition[index].position + new Vector3(0, 0.2f, 0);
        cardGO.rotation = Quaternion.Euler(0, -90f, 180);

        HandCard card = cardGO.GetComponent<HandCard>();

        if (battleID > 0)
        {
            card.SetHandCardData(battleID, hero.id, frame, CardOwner.Player, atk, hp, mana);
            foreach (HandCard c in Decks[0].GetListCard)
                c.isMoving = true;
            card.DiscardHandCardPlayer(showOpponentCardPoint, () =>
            {
                 callback?.Invoke(card);
                foreach (HandCard c in Decks[0].GetListCard)
                    c.isMoving = false;
            });
        }
        //Decks[index].RebuildDeckOnAddCard(index, showOpponentCardPoint, card);
    }
    public void OnMulliganCard()
    {
        CommonVector cv = new CommonVector();
        //foreach (Card id in lstMulliganCard)
        //{
        //    cv.aLong.Add(id.battleID);
        //    LogWriterHandle.WriteLog("On Mulligan: " + id.heroID);
        //    Decks[0].Remove(id);
        //    lstHeroBattleID.Remove(id.battleID);
        //    lstHeroPlayer.Remove(Database.GetHero(id.heroID));
        //    PoolManager.Pools["Card"].Despawn(id.transform);
        //}

        Game.main.socket.GameMulligan(cv);
    }

    public void SummonGodInReadyPhase(long battleID, long godID)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, groundLayer))
        {
            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<CardSlot>() != null)
                {
                    if (hit.collider.GetComponent<CardSlot>().type == SlotType.Player)
                    {
                        CardSlot slot = hit.collider.GetComponent<CardSlot>();

                        CommonVector cv = new CommonVector();
                        cv.aLong.Add(battleID);
                        cv.aLong.Add(slot.xPos);
                        cv.aLong.Add(slot.yPos);

                        Game.main.socket.GameFirstGodSummon(cv);
                    }
                }
            }
        }
    }

    public void MoveGodInReadyPhase(BoardCard card, long lastRow, long lastCol, long currentRow, long currentCol)
    {
        CommonVector cv = new CommonVector();
        cv.aLong.Add(lastRow);
        cv.aLong.Add(lastCol);
        cv.aLong.Add(currentRow);
        cv.aLong.Add(currentCol);
        Game.main.socket.GameMoveGodSummon(cv);
    }

    public void ReadyConfirm()
    {
        Game.main.socket.GameStartupConfirm();
    }

    public void SummonCardInBattlePhase(HandCard card, long row, long col)
    {
        LogWriterHandle.WriteLog("SummonCardInBattlePhase==" + card.heroInfo.color);
        CommonVector cv = new CommonVector();
        cv.aLong.Add(card.battleID);
        cv.aLong.Add(row);
        cv.aLong.Add(col);

        if (CanSummon(card))
        {
            if (!CheckHeroSkill(TYPE_WHEN_SUMON, card, row, col))
            {
                Game.main.socket.GameSummonCardInBatttle(cv);
                battleState = BATTLE_STATE.WAIT_COMFIRM;
                card.MoveFail();
            }

        }
        else
            card.MoveFail();

    }
    public bool CanSummon(HandCard card)
    {
        if (card.tmpMana <= currentMana)
        {

            if (card.heroInfo.speciesId == currentAvailableRegion)
                return true;

            return true;
        }
        Toast.Show(LangHandler.Get("55", "Not enough mana requirement"));
        return false;
    }
    public bool CheckCardCanUseCondition(HandCard card)


    {
        // hien 
        // hight light tat ca card du kieu kien de summon xuong san : shard mana skill
        // check moi khi turn player bat dau, sau moi lan keo bai xong
        if (card.heroInfoTmp.mana <= currentMana)
        {
            if (card.heroInfo.color == DBHero.COLOR_WHITE)
                return true;

            if (card.heroInfo.speciesId == currentAvailableRegion)
                return true;
            bool isOk = false;
            if (card.heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
            {
                foreach (Card c in GetListPlayerCardInBattle())
                {
                    if (c.heroID == card.heroInfo.ownerGodID)
                    {
                        isOk = true;
                    }
                }
            }
            else
                isOk = true;


            return isOk;
        }
        return false;

    }

    public void SummonGodInBattlePhase(long battleID, long godID)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, groundLayer))
        {
            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<CardSlot>() != null)
                {
                    if (hit.collider.GetComponent<CardSlot>().type == SlotType.Player)
                    {
                        CardSlot slot = hit.collider.GetComponent<CardSlot>();
                        DBHero god = Database.GetHero(godID);
                        CommonVector cv = new CommonVector();
                        cv.aLong.Add(battleID);
                        cv.aLong.Add(slot.xPos);
                        cv.aLong.Add(slot.yPos);
                        foreach (DBHeroSkill skill in god.lstHeroSkill)
                        {
                            if (skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                            {
                                cv.aLong.Add(skill.id);
                            }
                        }
                        Game.main.socket.GameSummonCardInBatttle(cv);

                    }
                }
            }
        }
    }

    public void MoveCardInBattlePhase(BoardCard card, long row1, long col1, long row2, long col2)
    {
        CommonVector cv = new CommonVector();
        cv.aLong.Add(row1);
        cv.aLong.Add(col1);
        cv.aLong.Add(row2);
        cv.aLong.Add(col2);

        battleState = BATTLE_STATE.WAIT_COMFIRM;
        Game.main.socket.GameMoveCardInbattle(cv);
    }

    public void OnConfirmStartBattle()
    {
        IsYourTurn = false;
        CommonVector cv = new CommonVector();

        Game.main.socket.GameConfirmStartBattle();
    }

    public void ChooseWayRequest(int index)
    {
        CommonVector cv = new CommonVector();
        cv.aLong.Add(index);

        //Game.main.socket.GameChooseWayRequest(cv);
    }

    private bool IsMe(string username)
    {
        if (username.Equals(GameData.main.profile.username))
            return true;
        return false;
    }

    private bool IsMeByClientPos(long clientPos)
    {
        if (clientPos == 0)
            return true;
        return false;
    }

    private bool IsMeByServerPos(long serverPos)
    {
        return IsMe(GetUsernameFromServerPos(serverPos));
    }

    // chi dung cho case 2 player, neu doi phai viet lai
    private void CalculateClientPosstion()
    {
        if (mLstBattlePlayer.Count < 0 || mLstBattlePlayer.Count > MAX_PLAYER)
            return;

        foreach (BattlePlayer player in mLstBattlePlayer)
        {
            if (IsMe(player.username))
            {
                player.clientPostion = POS_6h;
                foreach (BattlePlayer otherPlayer in mLstBattlePlayer)
                    if (!IsMe(otherPlayer.username))
                        otherPlayer.clientPostion = POS_12h;

                break;
            }
        }

    }

    private long GetClientPosFromServerPos(long serverPos)
    {
        foreach (BattlePlayer player in mLstBattlePlayer)
            if (player.position == serverPos)
                return player.clientPostion;
        return -1;
    }

    private string GetUsernameFromServerPos(long serverPos)
    {
        foreach (BattlePlayer player in mLstBattlePlayer)
            if (player.position == serverPos)
                return player.username;
        return "";
    }

    private long GetServerPostFromClientPos(long clientPos)
    {
        foreach (BattlePlayer player in mLstBattlePlayer)
            if (player.clientPostion == clientPos)
                return player.position;

        return -1;
    }

    private long GetServerPostFromUsername(string username)
    {
        foreach (BattlePlayer player in mLstBattlePlayer)
            if (player.username.Equals(username))
                return player.position;

        return -1;
    }

    private long GetClientPostFromUsername(string username)
    {
        foreach (BattlePlayer player in mLstBattlePlayer)
            if (player.username.Equals(username))
                return player.clientPostion;
        return -1;
    }
    private void WriteLogBattle(string logPhase, string logString, string logLong)
    {
        LogWriterHandle.WriteLog("- " + logPhase + "\n" + logString + "\n" + logLong);
        //LogWriterHandle.WriteLog("- " + logPhase + "\n" + logString + "\n" + logLong);
    }

    public bool CheckHeroSkill(int when, Card card, long row = -1, long col = -1)
    {
        foreach (DBHero hero in Database.lstHero)
            if (hero.id == card.heroID)
                foreach (DBHeroSkill skill in hero.lstHeroSkill)
                {
                    card.SetSkill(skill);
                    if (skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL && when == TYPE_WHEN_SUMON)
                    {
                        if (CheckSkllCondition(card,row, col))
                        {
                            //find target do now
                            bool skillPS = false;
                            foreach (ListEffectsSkill skInfo in skill.lstEffectsSkills)
                            {
                                foreach (EffectSkill effSkill in skInfo.lstEffect)
                                {
                                    if (effSkill.target >= 1000)
                                    {
                                        skillPS = true;
                                    }
                                }
                            }
                            if (skillPS)
                            {
                                card.SetSkillReady(skill);
                                FindTargetToActiveSkill(card, true, row, col);
                                return true;
                            }
                            //if (skill.target >= 1000)
                            //{
                            //    card.SetSkillReady(skill);
                            //    FindTargetToActiveSkill(card, true, row, col);
                            //    return true;
                            //}
                            else
                            {
                                CommonVector cv = new CommonVector();
                                cv.aLong.Add(card.battleID);
                                cv.aLong.Add(row);
                                cv.aLong.Add(col);
                                cv.aLong.Add(card.skill.id);

                                LogWriterHandle.WriteLog("----------------OnActivateSkill SUMMON w target <= 1000 =" + string.Join(",", cv.aLong));
                                Game.main.socket.GameSummonCardInBatttle(cv);

                                return true;
                            }
                        }
                    }
                    else if (skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL && when != TYPE_WHEN_SUMON)
                    {
                        //change mode skill can active
                        if (card.skill != null)
                        {
                            if (card.skill.isUltiType && !GameData.main.isUsedUlti)
                            {
                                card.SetSkillReady(skill);
                                return true;
                            }
                            else if (card.skill.isUltiType && GameData.main.isUsedUlti)
                            {

                                //@Tony
                                //card.SetSkillState(false);
                                Toast.Show(LangHandler.Get("82", "Ultimate already activated for this match"));
                            }
                            if (!card.skill.isUltiType && card.countDoActiveSkill == 0)
                            {
                                card.SetSkillReady(skill);
                                return true;
                            }
                            else if (!card.skill.isUltiType && card.countDoActiveSkill != 0)
                            {
                                Toast.Show(LangHandler.Get("146", "You can use active skill once per turn."));
                            }
                        }
                        else
                        {
                            card.SetSkillReady(skill);
                        }
                        return false;
                    }
                }
        return false;

    }
    public bool CheckHeroActiveSkill(int when, Card card)
    {
        //change mode skill can active
        if (card.skill != null)
        {
            bool shardOk = true;
            //bool shardOk = CheckShard(card);
            if (!shardOk)
            {
                return false;
            }
            if (card.skill.isUltiType && !GameData.main.isUsedUlti)
            {
                return true;
            }
            else if (card.skill.isUltiType && GameData.main.isUsedUlti)
            {

                //@Tony
                //card.SetSkillState(false);
                Toast.Show(LangHandler.Get("82", "Ultimate already activated for this match"));
            }
            if (!card.skill.isUltiType && card.countDoActiveSkill == 0)
            {
                BoardCard bCard = card.GetComponent<BoardCard>();
                if (bCard != null)
                {
                    if (bCard.isTired)
                        Toast.Show(LangHandler.Get("145", "You can't use active skill when god is tired."));
                    else
                        return true;
                }
                //return true;
            }
            else if (!card.skill.isUltiType && card.countDoActiveSkill != 0)
            {
                Toast.Show(LangHandler.Get("146", "You can use active skill once per turn."));
            }
        }
        else
        {
        }
        return false;
    } 

    public void DoActiveSkill(Card card)
    {
        if (card.skill == null)
            return;

        if (CheckShard(card) && CheckSkllCondition(card))
        {
            //find target do now
            if (CheckHeroActiveSkill(TYPE_WHEN_START_TURN, card))
            {
                bool skillPS = false;
                foreach (ListEffectsSkill skInfo in card.skill.lstEffectsSkills)
                {
                    foreach (EffectSkill effSkill in skInfo.lstEffect)
                    {
                        if (effSkill.target >= 1000)
                        {
                            skillPS = true;
                        }
                    }
                }
                if (skillPS)
                {
                    FindTargetToActiveSkill(card);
                }
                //if (skill.target >= 1000)
                //{
                //    card.SetSkillReady(skill);
                //    FindTargetToActiveSkill(card, true, row, col);
                //    return true;
                //}
                else
                {
                    CommonVector cv = new CommonVector();
                    cv.aLong.Add(card.skill.id);
                    cv.aLong.Add(card.battleID);

                    LogWriterHandle.WriteLog("----------------OnActivateSkill SUMMON w target <= 1000 =" + string.Join(",", cv.aLong));
                    Game.main.socket.GameActiveSkill(cv);
                }
            }

        }
        else
        {
            Toast.Show(LangHandler.Get("157", "Not eligible to activate skill."));
        }
    }

    public void CheckAvailableSkillCondition()
    {
        switch (skillState)
        {
            case SkillState.ANY_ALLY_UNIT:
                if (lstSelectedSkillBoardCard.Count < 1)
                {
                    chooseTargets.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    chooseTargets.transform.parent.gameObject.SetActive(true);
                }
                break;
            case SkillState.ANY_UNIT_BUT_SELF:
                if (lstSelectedSkillBoardCard.Count < 1)
                {
                    chooseTargets.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    chooseTargets.transform.parent.gameObject.SetActive(true);
                }
                break;
            case SkillState.TWO_ANY_ALLIES:
                if (lstSelectedSkillBoardCard.Count < 2)
                {
                    chooseTargets.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    chooseTargets.transform.parent.gameObject.SetActive(true);
                }
                break;
            case SkillState.TWO_ANY_ENEMIES:
                if (lstSelectedSkillBoardCard.Count < 2)
                {
                    chooseTargets.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    chooseTargets.transform.parent.gameObject.SetActive(true);
                }
                break;
            case SkillState.CHOOSE_FOUNTAIN:
                if (selectedTower != null)
                {
                    chooseTargets.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    chooseTargets.transform.parent.gameObject.SetActive(true);
                }
                break;
            case SkillState.CHOOSE_LANE:
                if (selectedLane != null)
                {
                    chooseTargets.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    chooseTargets.transform.parent.gameObject.SetActive(true);
                }
                break;
            case SkillState.CHOOSE_SELF_BLANK_NEXT:
                if (selectedCardSlot != null)
                {
                    chooseTargets.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    chooseTargets.transform.parent.gameObject.SetActive(true);
                }
                break;
            case SkillState.ANY_UNIT:
                if (lstSelectedSkillBoardCard.Count < 1 || selectedTower != null)
                {
                    chooseTargets.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    chooseTargets.transform.parent.gameObject.SetActive(true);
                }
                break;

        }
    }

    void FindTargetToActiveSkill(Card card, bool isSummon = false, long row = -1, long col = -1)
    {
        curCardSkill = card;
        if (!isStartFindTarget)
        {
            countSkillInfo = 0;
            countEffect = 0;
            lstTarget = new CommonVector();
            if (!cancelSkill.activeSelf)
                cancelSkill.SetActive(true);
        }
        isStartFindTarget = true;
        //onChangeSkillState?.Invoke(true);
        //ButtonOrbController.instance.onActiveSkill += ActiveSkill;
        onEndSkillActive += EndSkill;


        void ActiveSkill()
        {
            OnActivateSkill();
        }

        void EndSkill()
        {
            if (card.newCardClone != null)
            {
                card.MoveFail();
            }
            //ButtonOrbController.instance.onActiveSkill -= ActiveSkill;
            onEndSkillActive -= EndSkill;
        }
        LogWriterHandle.WriteLog(card.battleID);
        LogWriterHandle.WriteLog(card.skill.id);

        bool skillPS = false;

        Find();
        void Find()
        {
            for (int i = 0; i <= card.skill.lstEffectsSkills.Count; i++)
            {
                if (i == countSkillInfo)
                {
                    for (int j = 0; j < card.skill.lstEffectsSkills[i].lstEffect.Count; j++)
                    {
                        if (j == countEffect)
                        {
                            ListEffectsSkill skillInfo = card.skill.lstEffectsSkills[i];
                            EffectSkill eff = card.skill.lstEffectsSkills[i].lstEffect[j];
                            if (eff.target >= 1000)
                            {
                                skillPS = true;
                                if (!chooseTargets.transform.parent.gameObject.activeSelf)
                                {
                                    chooseTargets.transform.parent.gameObject.SetActive(true);
                                }
                                switch (eff.target)
                                {
                                    case DBHeroSkill.CHOSE_SELF_BLANK_NEXT:
                                        {
                                            chooseTargets.text = LangHandler.Get("130", "Choose 1 Slot");
                                            // not in use
                                            //chọn 1 ô trống ben canh minh (thường để dùng skill triệu hồi)
                                            List<CardSlot> slots = ChooseSelfBlankNext(card.GetComponent<BoardCard>());
                                            if (slots.Count == 0)
                                            {
                                                if (skillInfo.info == 0 && countSkillInfo == 0)
                                                {
                                                    if (card.heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || card.heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
                                                    {
                                                        card.GetComponent<HandCard>().MoveFail();
                                                        //SkillFailCondition();
                                                        Toast.Show(LangHandler.Get("stt-45", "Target not found."));
                                                    }
                                                    else if (card.heroInfo.type == DBHero.TYPE_GOD)
                                                    {
                                                        //SkillFailCondition();
                                                        Toast.Show(LangHandler.Get("stt-45", "Target not found."));
                                                    }
                                                    else
                                                    {
                                                        if (card.heroInfo.type == DBHero.TYPE_TROOPER_NORMAL && isSummon)
                                                        {
                                                            CommonVector cv = new CommonVector();
                                                            cv.aLong.Add(card.battleID);
                                                            cv.aLong.Add(row);
                                                            cv.aLong.Add(col);
                                                            Game.main.socket.GameSummonCardInBatttle(cv);
                                                        }
                                                    }
                                                    SkillFailCondition();

                                                }
                                                else if (skillInfo.info > 0)
                                                {
                                                    // trường hợp k co target thi sẽ gui -1
                                                    lstTarget.aLong.Add(-1);
                                                    if (countEffect < skillInfo.lstEffect.Count - 1)
                                                    {
                                                        countEffect++;
                                                        Find();
                                                    }
                                                    else
                                                    {
                                                        if (countSkillInfo < card.skill.lstEffectsSkills.Count - 1)
                                                        {
                                                            countSkillInfo++;
                                                            countEffect = 0;
                                                            Find();
                                                        }
                                                        else
                                                        {
                                                            ActiveSkill();
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                foreach (CardSlot cs in slots)
                                                    cs.HighLightSlot();
                                                skillState = SkillState.CHOOSE_SELF_BLANK_NEXT;
                                            }
                                            break;
                                        }
                                    case DBHeroSkill.ANY_ALLY_BUT_SELF:
                                        {
                                            //chọn ally unit bất kì nhưng không phải bản thân
                                            //ChooseAllyUnitButSelf(card);
                                            chooseTargets.text = LangHandler.Get("136", "Choose 1 Ally");
                                            List<BoardCard> cards = GetListPlayerCardInBattle().Where(x => x != card).ToList(); //ChooseAnyUnit(true, card.GetComponent<BoardCard>());
                                            if (cards.Count == 0)
                                            {
                                                if (skillInfo.info == 0 && countSkillInfo == 0)
                                                {
                                                    if (card.heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || card.heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
                                                    {
                                                        card.GetComponent<HandCard>().MoveFail();
                                                        //SkillFailCondition();
                                                        Toast.Show(LangHandler.Get("stt-45", "Target not found"));
                                                    }
                                                    else if (card.heroInfo.type == DBHero.TYPE_GOD)
                                                    {
                                                        //SkillFailCondition();
                                                        Toast.Show(LangHandler.Get("stt-45", "Target not found"));
                                                    }
                                                    else
                                                    {
                                                        if (card.heroInfo.type == DBHero.TYPE_TROOPER_NORMAL && isSummon)
                                                        {
                                                            CommonVector cv = new CommonVector();
                                                            cv.aLong.Add(card.battleID);
                                                            cv.aLong.Add(row);
                                                            cv.aLong.Add(col);
                                                            Game.main.socket.GameSummonCardInBatttle(cv);
                                                        }
                                                    }
                                                    SkillFailCondition();
                                                }
                                                else if (skillInfo.info > 0)
                                                {
                                                    // trường hợp k co target thi sẽ gui -1
                                                    lstTarget.aLong.Add(-1);
                                                    if (countEffect < skillInfo.lstEffect.Count - 1)
                                                    {
                                                        countEffect++;
                                                        Find();
                                                    }
                                                    else
                                                    {
                                                        if (countSkillInfo < card.skill.lstEffectsSkills.Count - 1)
                                                        {
                                                            countSkillInfo++;
                                                            countEffect = 0;
                                                            Find();
                                                        }
                                                        else
                                                        {
                                                            ActiveSkill();
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                cards.ForEach(x =>
                                                {
                                                    x.HighlightUnit();
                                                    x.onAddToListSkill += AddBoardCardToSkillList;
                                                    //x.onRemoveFromListSkill += RemoveBoardCardFromSkillList;
                                                    x.onEndSkillActive += OnEndSkillBoard;
                                                });
                                                skillState = SkillState.ANY_ALLY_BUT_SELF;
                                            }
                                            break;
                                        }
                                    case DBHeroSkill.TWO_ANY_ENEMIES:
                                        {
                                            // chọn 2 hero địch
                                            chooseTargets.text = LangHandler.Get("125", "Choose 2 Enemies");
                                            List<BoardCard> cards = GetListEnemyCardInBattle();
                                            if (cards.Count <= 1)
                                            {
                                                if (skillInfo.info == 0 && countSkillInfo == 0)
                                                {
                                                    if (card.heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || card.heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
                                                    {
                                                        card.GetComponent<HandCard>().MoveFail();
                                                        //SkillFailCondition();
                                                        Toast.Show(LangHandler.Get("stt-45", "Target not found."));
                                                    }
                                                    else if (card.heroInfo.type == DBHero.TYPE_GOD)
                                                    {
                                                        //SkillFailCondition();
                                                        Toast.Show(LangHandler.Get("stt-45", "Target not found."));
                                                    }
                                                    else
                                                    {
                                                        if (card.heroInfo.type == DBHero.TYPE_TROOPER_NORMAL && isSummon)
                                                        {
                                                            CommonVector cv = new CommonVector();
                                                            cv.aLong.Add(card.battleID);
                                                            cv.aLong.Add(row);
                                                            cv.aLong.Add(col);
                                                            Game.main.socket.GameSummonCardInBatttle(cv);
                                                        }
                                                    }
                                                    SkillFailCondition();

                                                }
                                                else if (skillInfo.info > 0)
                                                {
                                                    // trường hợp k co target thi sẽ gui -1
                                                    lstTarget.aLong.Add(-1);
                                                    if (countEffect < skillInfo.lstEffect.Count - 1)
                                                    {
                                                        countEffect++;
                                                        Find();
                                                    }
                                                    else
                                                    {
                                                        if (countSkillInfo < card.skill.lstEffectsSkills.Count - 1)
                                                        {
                                                            countSkillInfo++;
                                                            countEffect = 0;
                                                            Find();
                                                        }
                                                        else
                                                        {
                                                            ActiveSkill();
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                cards.ForEach(x =>
                                                {
                                                    x.HighlightUnit();
                                                    x.onAddToListSkill += AddBoardCardToSkillList;
                                                    //x.onRemoveFromListSkill += RemoveBoardCardFromSkillList;
                                                    x.onEndSkillActive += OnEndSkillBoard;
                                                });
                                                skillState = SkillState.TWO_ANY_ENEMIES;
                                            }
                                            break;
                                        }
                                    case DBHeroSkill.ANY_ALLY_UNIT:
                                        {
                                            // chọn ally unit bất kì kể cả phải bản thân
                                            //ChooseAnyAllyUnit();
                                            chooseTargets.text = LangHandler.Get("136", "Choose 1 Ally");
                                            List<BoardCard> cards = GetListPlayerCardInBattle();
                                            CardOnBoardClone clone = null;
                                            cards.ForEach(x =>
                                            {
                                                x.HighlightUnit();
                                                x.onAddToListSkill += AddBoardCardToSkillList;
                                                //x.onRemoveFromListSkill += RemoveBoardCardFromSkillList;
                                                x.onEndSkillActive += OnEndSkillBoard;
                                            });
                                            if (card.newCardClone != null)
                                                clone = card.newCardClone.GetComponent<CardOnBoardClone>();
                                            if (clone != null)
                                            {
                                                clone.HighlightUnit();
                                                clone.onAddToListSkill += AddCloneCardToSkillList;
                                                //clone.onRemoveFromListSkill += RemoveCloneCardFromSkillList;
                                                clone.onEndSkillActive += OnEndSkillClone;
                                            }
                                            skillState = SkillState.ANY_ALLY_UNIT;
                                            break;
                                        }
                                    case DBHeroSkill.TWO_ANY_ALLIES:
                                        {
                                            // chọn 2 hero đồng minh (vd: đổi chỗ)
                                            //ChooseTwoAllyUnit(card);
                                            chooseTargets.text = LangHandler.Get("126", "Choose 2 Allies");
                                            List<BoardCard> cards = GetListPlayerCardInBattle();
                                            CardOnBoardClone clone = null;
                                            if (card.newCardClone != null)
                                                clone = card.newCardClone.GetComponent<CardOnBoardClone>();

                                            if (clone != null)
                                            {
                                                {
                                                    if (cards.Count + 1 <= 1)
                                                    {
                                                        if (skillInfo.info == 0 && countSkillInfo == 0)
                                                        {
                                                            if (card.heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || card.heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
                                                            {
                                                                card.GetComponent<HandCard>().MoveFail();
                                                                //SkillFailCondition();
                                                                Toast.Show(LangHandler.Get("stt-45", "Target not found."));
                                                            }
                                                            else if (card.heroInfo.type == DBHero.TYPE_GOD)
                                                            {
                                                                //SkillFailCondition();
                                                                Toast.Show(LangHandler.Get("stt-45", "Target not found."));
                                                            }
                                                            else
                                                            {
                                                                if (card.heroInfo.type == DBHero.TYPE_TROOPER_NORMAL && isSummon)
                                                                {
                                                                    CommonVector cv = new CommonVector();
                                                                    cv.aLong.Add(card.battleID);
                                                                    cv.aLong.Add(row);
                                                                    cv.aLong.Add(col);
                                                                    Game.main.socket.GameSummonCardInBatttle(cv);
                                                                }
                                                            }
                                                            SkillFailCondition();

                                                        }
                                                        else if (skillInfo.info > 0)
                                                        {
                                                            // trường hợp k co target thi sẽ gui -1
                                                            lstTarget.aLong.Add(-1);
                                                            if (countEffect < skillInfo.lstEffect.Count - 1)
                                                            {
                                                                countEffect++;
                                                                Find();
                                                            }
                                                            else
                                                            {
                                                                if (countSkillInfo < card.skill.lstEffectsSkills.Count - 1)
                                                                {
                                                                    countSkillInfo++;
                                                                    countEffect = 0;
                                                                    Find();
                                                                }
                                                                else
                                                                {
                                                                    ActiveSkill();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        cards.ForEach(x =>
                                                        {
                                                            x.HighlightUnit();
                                                            x.onAddToListSkill += AddBoardCardToSkillList;
                                                            //x.onRemoveFromListSkill += RemoveBoardCardFromSkillList;
                                                            x.onEndSkillActive += OnEndSkillBoard;
                                                        });
                                                        clone.HighlightUnit();
                                                        clone.onAddToListSkill += AddCloneCardToSkillList;
                                                        //clone.onRemoveFromListSkill += RemoveCloneCardFromSkillList;
                                                        clone.onEndSkillActive += OnEndSkillClone;
                                                        skillState = SkillState.TWO_ANY_ALLIES;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (cards.Count <= 1)
                                                {
                                                    if (skillInfo.info == 0 && countSkillInfo == 0)
                                                    {
                                                        if (card.heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || card.heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
                                                        {
                                                            card.GetComponent<HandCard>().MoveFail();
                                                            //SkillFailCondition();
                                                            Toast.Show(LangHandler.Get("stt-45", "Target not found."));
                                                        }
                                                        else if (card.heroInfo.type == DBHero.TYPE_GOD)
                                                        {
                                                            //SkillFailCondition();
                                                            Toast.Show(LangHandler.Get("stt-45", "Target not found."));
                                                        }
                                                        else
                                                        {
                                                            if (card.heroInfo.type == DBHero.TYPE_TROOPER_NORMAL && isSummon)
                                                            {
                                                                CommonVector cv = new CommonVector();
                                                                cv.aLong.Add(card.battleID);
                                                                cv.aLong.Add(row);
                                                                cv.aLong.Add(col);
                                                                Game.main.socket.GameSummonCardInBatttle(cv);
                                                            }
                                                        }
                                                        SkillFailCondition();

                                                    }
                                                    else if (skillInfo.info > 0)
                                                    {
                                                        // trường hợp k co target thi sẽ gui -1
                                                        lstTarget.aLong.Add(-1);
                                                        if (countEffect < skillInfo.lstEffect.Count - 1)
                                                            countEffect++;
                                                        else
                                                        {
                                                            if (countSkillInfo < card.skill.lstEffectsSkills.Count - 1)
                                                            {
                                                                countSkillInfo++;
                                                                countEffect = 0;
                                                                Find();
                                                            }
                                                            else
                                                            {
                                                                ActiveSkill();
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    cards.ForEach(x =>
                                                    {
                                                        x.HighlightUnit();
                                                        x.onAddToListSkill += AddBoardCardToSkillList;
                                                        //x.onRemoveFromListSkill += RemoveBoardCardFromSkillList;
                                                        x.onEndSkillActive += OnEndSkillBoard;
                                                    });
                                                    skillState = SkillState.TWO_ANY_ALLIES;
                                                }
                                            }

                                            break;
                                        }

                                    case DBHeroSkill.CHOSSE_FOUNTAIN:
                                        {
                                            // chọn foutain
                                            chooseTargets.text = LangHandler.Get("128", "Choose 1 Tower");
                                            lstTowerInBattle.Where(t => t.pos == 1 && t.towerHealth > 0).ToList().ForEach(x =>
                                            {
                                                x.HighLightTower();
                                                x.onAddToListSkill += AddTowerToSkillList;
                                                // x.onRemoveFromListSkill += RemoveTowerFromSkillList;
                                                x.onEndSkillActive += OnEndSkillTower;
                                            });
                                            skillState = SkillState.CHOOSE_FOUNTAIN;
                                            break;
                                        }

                                    case DBHeroSkill.ANY_ALLY_LANE_UNITS:
                                        {
                                            chooseTargets.text = LangHandler.Get("133", "Choose 1 Lane");
                                            lstLaneInBattle.ForEach(x =>
                                            {
                                                x.HighlightLane();
                                                x.onAddToListSkill += AddLaneToSkillList;
                                                //x.onRemoveFromListSkill += RemoveLaneFromSkillList;
                                                x.onEndSkillActive += OnEndSkillLane;
                                            });
                                            skillState = SkillState.CHOOSE_LANE;
                                            break;
                                        }

                                    case DBHeroSkill.CHOSSE_LANE:
                                        {
                                            // chọn lane
                                            chooseTargets.text = LangHandler.Get("133", "Choose 1 Lane");
                                            lstLaneInBattle.ForEach(x =>
                                            {
                                                x.HighlightLane();
                                                x.onAddToListSkill += AddLaneToSkillList;
                                                //x.onRemoveFromListSkill += RemoveLaneFromSkillList;
                                                x.onEndSkillActive += OnEndSkillLane;
                                            });
                                            skillState = SkillState.CHOOSE_LANE;
                                            break;
                                        }

                                    case DBHeroSkill.ANY_UNIT:
                                        {
                                            chooseTargets.text = LangHandler.Get("127", "Choose 1 Unit");
                                            lstCardInBattle.ForEach(x =>
                                            {
                                                x.HighlightUnit();
                                                x.onAddToListSkill += AddBoardCardToSkillList;
                                                //x.onRemoveFromListSkill += RemoveBoardCardFromSkillList;
                                                x.onEndSkillActive += OnEndSkillBoard;
                                            });
                                            if (card.newCardClone != null)
                                            {
                                                CardOnBoardClone clone = card.newCardClone.GetComponent<CardOnBoardClone>();
                                                if (clone != null)
                                                {
                                                    clone.HighlightUnit();
                                                    clone.onAddToListSkill += AddCloneCardToSkillList;
                                                    //clone.onRemoveFromListSkill += RemoveCloneCardFromSkillList;
                                                    clone.onEndSkillActive += OnEndSkillClone;
                                                }

                                            }
                                            skillState = SkillState.ANY_UNIT;
                                            break;
                                        }
                                    case DBHeroSkill.ANY_ENEMY_UNIT:
                                        {
                                            chooseTargets.text = LangHandler.Get("135", "Choose 1 Enemy");
                                            List<BoardCard> cards = GetListEnemyCardInBattle();
                                            if (cards.Count == 0)
                                            {
                                                if (skillInfo.info == 0 && countSkillInfo == 0)
                                                {
                                                    if (card.heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || card.heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
                                                    {
                                                        card.GetComponent<HandCard>().MoveFail();
                                                        //SkillFailCondition();
                                                        Toast.Show(LangHandler.Get("stt-45", "Target not found."));
                                                    }
                                                    else if (card.heroInfo.type == DBHero.TYPE_GOD)
                                                    {
                                                        //SkillFailCondition();
                                                        Toast.Show(LangHandler.Get("stt-45", "Target not found."));
                                                    }
                                                    else
                                                    {
                                                        if (card.heroInfo.type == DBHero.TYPE_TROOPER_NORMAL && isSummon)
                                                        {
                                                            CommonVector cv = new CommonVector();
                                                            cv.aLong.Add(card.battleID);
                                                            cv.aLong.Add(row);
                                                            cv.aLong.Add(col);
                                                            Game.main.socket.GameSummonCardInBatttle(cv);
                                                        }
                                                    }
                                                    SkillFailCondition();

                                                }
                                                else if (skillInfo.info > 0)
                                                {
                                                    // trường hợp k co target thi sẽ gui -1
                                                    lstTarget.aLong.Add(-1);
                                                    if (countEffect < skillInfo.lstEffect.Count - 1)
                                                    {
                                                        countEffect++;
                                                        Find();
                                                    }
                                                    else
                                                    {
                                                        if (countSkillInfo < card.skill.lstEffectsSkills.Count - 1)
                                                        {
                                                            countSkillInfo++;
                                                            countEffect = 0;
                                                            Find();
                                                        }
                                                        else
                                                        {
                                                            ActiveSkill();
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                cards.ForEach(x =>
                                                {
                                                    x.HighlightUnit();
                                                    x.onAddToListSkill += AddBoardCardToSkillList;
                                                    //x.onRemoveFromListSkill += RemoveBoardCardFromSkillList;
                                                    x.onEndSkillActive += OnEndSkillBoard;
                                                });
                                                skillState = SkillState.ANY_ENEMY_UNIT;
                                            }
                                            break;
                                        }
                                    case DBHeroSkill.ANY_LANE_UNITS:
                                        {
                                            chooseTargets.text = LangHandler.Get("133", "Choose 1 Lane");
                                            lstLaneInBattle.ForEach(x =>
                                            {
                                                x.HighlightLane();
                                                x.onAddToListSkill += AddLaneToSkillList;
                                                //x.onRemoveFromListSkill += RemoveLaneFromSkillList;
                                                x.onEndSkillActive += OnEndSkillLane;
                                            });
                                            skillState = SkillState.ANY_LANE_UNIT;
                                            break;
                                        }
                                    case DBHeroSkill.RANDOM_ENEMY_IN_SELECTED_LANE:
                                        {

                                            chooseTargets.text = LangHandler.Get("133", "Choose 1 Lane");
                                            lstLaneInBattle.ForEach(x =>
                                            {
                                                x.HighlightLane();
                                                x.onAddToListSkill += AddLaneToSkillList;
                                                //x.onRemoveFromListSkill += RemoveLaneFromSkillList;
                                                x.onEndSkillActive += OnEndSkillLane;
                                            });
                                            skillState = SkillState.CHOOSE_LANE;
                                            break;
                                        }

                                    case DBHeroSkill.ANY_ALL_UNIT:
                                        {
                                            // đã bỏ
                                            ChooseEnemyTower();
                                            GetListEnemyCardInBattle().ForEach(x =>
                                            {
                                                x.HighlightUnit();
                                                x.onAddToListSkill += AddBoardCardToSkillList;
                                                //x.onRemoveFromListSkill += RemoveBoardCardFromSkillList;
                                                x.onEndSkillActive += OnEndSkillBoard;
                                            });
                                            skillState = SkillState.ANY_ALL_UNIT;
                                            break;
                                        }

                                    case DBHeroSkill.MY_HAND_CARD:
                                        {

                                            chooseTargets.text = LangHandler.Get("134", "Choose 1 Hand Card");
                                            // không liên quan gì đến bản thân 
                                            skillState = SkillState.MY_HAND_CARD;
                                            Decks[0].GetListCard.Where(c => c != card).ToList().ForEach(x =>
                                            {
                                                x.HighlighCard();
                                                x.onAddToListSkill += AddHandCardToSkillList;
                                                //x.onRemoveFromListSkill += RemoveHandCardFromSkillList;
                                                x.onEndSkillActive += OnEndSkillHand;
                                            });
                                            break;
                                        }
                                    case DBHeroSkill.ANY_MOTAL:
                                        {

                                            chooseTargets.text = LangHandler.Get("132", "Choose 1 Mortal");
                                            CardOnBoardClone clone = null;
                                            List<BoardCard> cards = lstCardInBattle.Where(x => x.heroInfo.type == DBHero.TYPE_TROOPER_NORMAL).ToList();
                                            if (card.newCardClone != null)
                                                clone = card.newCardClone.GetComponent<CardOnBoardClone>();
                                            if (clone != null && card.heroInfo.type == DBHero.TYPE_TROOPER_NORMAL)
                                            {
                                                cards.ForEach(x =>
                                                {
                                                    x.HighlightUnit();
                                                    x.onAddToListSkill += AddBoardCardToSkillList;
                                                    //x.onRemoveFromListSkill += RemoveBoardCardFromSkillList;
                                                    x.onEndSkillActive += OnEndSkillBoard;
                                                });
                                                clone.HighlightUnit();
                                                clone.onAddToListSkill += AddCloneCardToSkillList;
                                                //clone.onRemoveFromListSkill += RemoveCloneCardFromSkillList;
                                                clone.onEndSkillActive += OnEndSkillClone;
                                                skillState = SkillState.ANY_MOTAL;
                                            }
                                            else
                                            {
                                                if (cards.Count == 0)
                                                {
                                                    if (skillInfo.info == 0 && countSkillInfo == 0)
                                                    {
                                                        if (card.heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || card.heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
                                                        {
                                                            card.GetComponent<HandCard>().MoveFail();
                                                            //SkillFailCondition();
                                                            Toast.Show(LangHandler.Get("stt-45", "Target not found."));
                                                        }
                                                        else if (card.heroInfo.type == DBHero.TYPE_GOD)
                                                        {
                                                            //SkillFailCondition();
                                                            Toast.Show(LangHandler.Get("stt-45", "Target not found."));
                                                        }

                                                        SkillFailCondition();

                                                    }
                                                    else if (skillInfo.info > 0)
                                                    {
                                                        // trường hợp k co target thi sẽ gui -1
                                                        lstTarget.aLong.Add(-1);
                                                        if (countEffect < skillInfo.lstEffect.Count - 1)
                                                        {
                                                            countEffect++;
                                                            Find();
                                                        }
                                                        else
                                                        {
                                                            if (countSkillInfo < card.skill.lstEffectsSkills.Count - 1)
                                                            {
                                                                countSkillInfo++;
                                                                countEffect = 0;
                                                                Find();
                                                            }
                                                            else
                                                            {
                                                                ActiveSkill();
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    cards.ForEach(x =>
                                                    {
                                                        x.HighlightUnit();
                                                        x.onAddToListSkill += AddBoardCardToSkillList;
                                                        //x.onRemoveFromListSkill += RemoveBoardCardFromSkillList;
                                                        x.onEndSkillActive += OnEndSkillBoard;
                                                    });
                                                }
                                            }
                                            skillState = SkillState.ANY_MOTAL;
                                            break;
                                        }

                                    case DBHeroSkill.ANY_BLANK_ALLY:
                                        {

                                            chooseTargets.text = LangHandler.Get("131", "Choose 1 Position to summon");
                                            //chọn 1 ô trống ben canh minh (thường để dùng skill triệu hồi)
                                            List<CardSlot> slots = playerSlotContainer.Where(x => x.state == SlotState.Empty).ToList();
                                            if (slots.Count == 0)
                                            {
                                                if (skillInfo.info == 0 && countSkillInfo == 0)
                                                {
                                                    if (card.heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || card.heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
                                                    {
                                                        card.GetComponent<HandCard>().MoveFail();
                                                        //SkillFailCondition();
                                                        Toast.Show(LangHandler.Get("stt-45", "Target not found."));
                                                    }
                                                    else if (card.heroInfo.type == DBHero.TYPE_GOD)
                                                    {
                                                        //SkillFailCondition();
                                                        Toast.Show(LangHandler.Get("stt-45", "Target not found."));
                                                    }
                                                    else
                                                    {
                                                        if (card.heroInfo.type == DBHero.TYPE_TROOPER_NORMAL && isSummon)
                                                        {
                                                            CommonVector cv = new CommonVector();
                                                            cv.aLong.Add(card.battleID);
                                                            cv.aLong.Add(row);
                                                            cv.aLong.Add(col);
                                                            Game.main.socket.GameSummonCardInBatttle(cv);
                                                        }
                                                    }
                                                    SkillFailCondition();

                                                }
                                                else if (skillInfo.info > 0)
                                                {
                                                    // trường hợp k co target thi sẽ gui -1
                                                    lstTarget.aLong.Add(-1);
                                                    if (countEffect < skillInfo.lstEffect.Count - 1)
                                                    {
                                                        countEffect++;
                                                        Find();
                                                    }
                                                    else
                                                    {
                                                        if (countSkillInfo < card.skill.lstEffectsSkills.Count - 1)
                                                        {
                                                            countSkillInfo++;
                                                            countEffect = 0;
                                                            Find();
                                                        }
                                                        else
                                                        {
                                                            ActiveSkill();
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                slots.ForEach(x =>
                                                {
                                                    x.HighLightSlot();
                                                    x.onAddToListSkill += AddSlotToSkillList;
                                                    //x.onRemoveFromListSkill -= RemoveSlotFromSkillList;
                                                    x.onEndSkillActive -= OnEndSkillSlot;
                                                });
                                                skillState = SkillState.ANY_BLANK_ALLY;
                                            }
                                            break;
                                        }

                                    case DBHeroSkill.ANY_LANE_ENEMY:
                                        {
                                            // chọn enemy cung duong 

                                            chooseTargets.text = LangHandler.Get("135", "Choose 1 Enemy");
                                            List<BoardCard> cards = GetListEnemyCardInBattle().Where(x => (col <= 1 && x.slot.yPos <= col) || (col >= 2 && x.slot.yPos >= col)).ToList();

                                            if (cards.Count == 0)
                                            {
                                                if (skillInfo.info == 0 && countSkillInfo == 0)
                                                {
                                                    if (card.heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || card.heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
                                                    {
                                                        card.GetComponent<HandCard>().MoveFail();
                                                        //SkillFailCondition();
                                                        Toast.Show(LangHandler.Get("stt-45", "Target not found."));
                                                    }
                                                    else if (card.heroInfo.type == DBHero.TYPE_GOD)
                                                    {
                                                        //SkillFailCondition();
                                                        Toast.Show(LangHandler.Get("stt-45", "Target not found."));
                                                    }
                                                    else
                                                    {
                                                        if (card.heroInfo.type == DBHero.TYPE_TROOPER_NORMAL && isSummon)
                                                        {
                                                            CommonVector cv = new CommonVector();
                                                            cv.aLong.Add(card.battleID);
                                                            cv.aLong.Add(row);
                                                            cv.aLong.Add(col);
                                                            Game.main.socket.GameSummonCardInBatttle(cv);
                                                        }
                                                    }
                                                    SkillFailCondition();

                                                }
                                                else if (skillInfo.info > 0)
                                                {
                                                    // trường hợp k co target thi sẽ gui -1
                                                    lstTarget.aLong.Add(-1);
                                                    if (countEffect < skillInfo.lstEffect.Count - 1)
                                                    {
                                                        countEffect++;
                                                        Find();
                                                    }
                                                    else
                                                    {
                                                        if (countSkillInfo < card.skill.lstEffectsSkills.Count - 1)
                                                        {
                                                            countSkillInfo++;
                                                            countEffect = 0;
                                                            Find();
                                                        }
                                                        else
                                                        {
                                                            ActiveSkill();
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                cards.ForEach(x =>
                                                {
                                                    x.HighlightUnit();
                                                    x.onAddToListSkill += AddBoardCardToSkillList;
                                                    //x.onRemoveFromListSkill += RemoveBoardCardFromSkillList;
                                                    x.onEndSkillActive += OnEndSkillBoard;
                                                });
                                                skillState = SkillState.ANY_LANE_ENEMY;
                                            }
                                            break;
                                        }

                                    case DBHeroSkill.ANY_COL_ENEMY:
                                        {

                                            chooseTargets.text = LangHandler.Get("135", "Choose 1 Enemy");
                                            List<BoardCard> cards = GetListEnemyCardInBattle().Where(x => x.slot.yPos == col).ToList();

                                            if (cards.Count == 0)
                                            {
                                                if (skillInfo.info == 0 && countSkillInfo == 0)
                                                {
                                                    if (card.heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || card.heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
                                                    {
                                                        card.GetComponent<HandCard>().MoveFail();
                                                        //SkillFailCondition();
                                                        Toast.Show(LangHandler.Get("stt-45", "Target not found."));
                                                    }
                                                    else if (card.heroInfo.type == DBHero.TYPE_GOD)
                                                    {
                                                        //SkillFailCondition();
                                                        Toast.Show(LangHandler.Get("stt-45", "Target not found."));
                                                    }
                                                    else
                                                    {
                                                        if (card.heroInfo.type == DBHero.TYPE_TROOPER_NORMAL && isSummon)
                                                        {
                                                            CommonVector cv = new CommonVector();
                                                            cv.aLong.Add(card.battleID);
                                                            cv.aLong.Add(row);
                                                            cv.aLong.Add(col);
                                                            Game.main.socket.GameSummonCardInBatttle(cv);
                                                        }
                                                    }
                                                    SkillFailCondition();

                                                }
                                                else if (skillInfo.info > 0)
                                                {
                                                    // trường hợp k co target thi sẽ gui -1
                                                    lstTarget.aLong.Add(-1);
                                                    if (countEffect < skillInfo.lstEffect.Count - 1)
                                                    {
                                                        countEffect++;
                                                        Find();
                                                    }
                                                    else
                                                    {
                                                        if (countSkillInfo < card.skill.lstEffectsSkills.Count - 1)
                                                        {
                                                            countSkillInfo++;
                                                            countEffect = 0;
                                                            Find();
                                                        }
                                                        else
                                                        {
                                                            ActiveSkill();
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                cards.ForEach(x =>
                                                {
                                                    x.HighlightUnit();
                                                    x.onAddToListSkill += AddBoardCardToSkillList;
                                                    //x.onRemoveFromListSkill += RemoveBoardCardFromSkillList;
                                                    x.onEndSkillActive += OnEndSkillBoard;
                                                });
                                                skillState = SkillState.ANY_COL_ENEMY;
                                            }
                                            break;
                                        }

                                    case DBHeroSkill.ANY_TARGET:
                                        {

                                            chooseTargets.text = LangHandler.Get("129", "Choose 1 Target");
                                            lstCardInBattle.ForEach(x =>
                                            {
                                                x.HighlightUnit();
                                                x.onAddToListSkill += AddBoardCardToSkillList;
                                                //x.onRemoveFromListSkill += RemoveBoardCardFromSkillList;
                                                x.onEndSkillActive += OnEndSkillBoard;
                                            });
                                            lstTowerInBattle.Where(x => x.towerHealth > 0).ToList().ForEach(t =>
                                            {
                                                t.HighLightTower();
                                                t.onAddToListSkill += AddTowerToSkillList;
                                                //t.onRemoveFromListSkill += RemoveTowerFromSkillList;
                                                t.onEndSkillActive += OnEndSkillTower;
                                            });

                                            CardOnBoardClone clone = null;
                                            if (card.newCardClone != null)
                                                card.newCardClone.GetComponent<CardOnBoardClone>();
                                            if (clone != null)
                                            {
                                                clone.HighlightUnit();
                                                clone.onAddToListSkill += AddCloneCardToSkillList;
                                                //clone.onRemoveFromListSkill += RemoveCloneCardFromSkillList;
                                                clone.onEndSkillActive += OnEndSkillClone;
                                            }
                                            skillState = SkillState.ANY_TARGET;
                                            break;
                                        }
                                    case DBHeroSkill.ANY_ALLY_TARGET:
                                        {
                                            // than linh tru dong minh 

                                            chooseTargets.text = LangHandler.Get("137", "Choose 1 Ally target");
                                            GetListPlayerCardInBattle().ForEach(c =>
                                            {
                                                c.HighlightUnit();
                                                c.onAddToListSkill += AddBoardCardToSkillList;
                                                // c.onRemoveFromListSkill += RemoveBoardCardFromSkillList;
                                                c.onEndSkillActive += OnEndSkillBoard;
                                            });
                                            lstTowerInBattle.Where(t => t.pos == 0 && t.towerHealth > 0).ToList().ForEach(x =>
                                            {
                                                x.HighLightTower();
                                                x.onAddToListSkill += AddTowerToSkillList;
                                                // x.onRemoveFromListSkill += RemoveTowerFromSkillList;
                                                x.onEndSkillActive += OnEndSkillTower;
                                            });
                                            CardOnBoardClone clone = null;
                                            if (card.newCardClone != null)
                                                card.newCardClone.GetComponent<CardOnBoardClone>();
                                            if (clone != null)
                                            {
                                                clone.HighlightUnit();
                                                clone.onAddToListSkill += AddCloneCardToSkillList;
                                                //clone.onRemoveFromListSkill += RemoveCloneCardFromSkillList;
                                                clone.onEndSkillActive += OnEndSkillClone;
                                            }
                                            skillState = SkillState.ANY_ALLY_TARGET;
                                            break;
                                        }
                                    case DBHeroSkill.RANDOM_UNIT_IN_SELECTED_LANE:
                                        {
                                            // chon 1 lane => random unit

                                            chooseTargets.text = LangHandler.Get("133", "Choose 1 Lane");
                                            lstLaneInBattle.ForEach(x =>
                                            {
                                                x.HighlightLane();
                                                x.onAddToListSkill += AddLaneToSkillList;
                                                //x.onRemoveFromListSkill += RemoveLaneFromSkillList;
                                                x.onEndSkillActive += OnEndSkillLane;
                                            });
                                            skillState = SkillState.RANDOM_UNIT_IN_SELECTED_LANE;
                                            break;
                                        }
                                    case DBHeroSkill.ANY_TARGET_BUT_SELF:
                                        {

                                            chooseTargets.text = LangHandler.Get("129", "Choose 1 Target");
                                            lstCardInBattle.Where(c => c != card).ToList().ForEach(x =>
                                            {
                                                x.HighlightUnit();
                                                x.onAddToListSkill += AddBoardCardToSkillList;
                                                //x.onRemoveFromListSkill += RemoveBoardCardFromSkillList;
                                                x.onEndSkillActive += OnEndSkillBoard;
                                            });
                                            lstTowerInBattle.Where(x => x.towerHealth > 0).ToList().ForEach(t =>
                                            {
                                                t.HighLightTower();
                                                t.onAddToListSkill += AddTowerToSkillList;
                                                //t.onRemoveFromListSkill += RemoveTowerFromSkillList;
                                                t.onEndSkillActive += OnEndSkillTower;
                                            });
                                            skillState = SkillState.ANY_TARGET_BUT_SELF;
                                            break;
                                        }
                                    case DBHeroSkill.ANY_MORTAL_BUT_SELF:
                                        {

                                            chooseTargets.text = LangHandler.Get("132", "Choose 1 Mortal");
                                            List<BoardCard> cards = lstCardInBattle.Where(x => (x.heroInfo.type == DBHero.TYPE_TROOPER_NORMAL) && (x != card)).ToList();
                                            if (cards.Count == 0)
                                            {
                                                if (skillInfo.info == 0 && countSkillInfo == 0)
                                                {
                                                    if (card.heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || card.heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
                                                    {
                                                        card.GetComponent<HandCard>().MoveFail();
                                                        //SkillFailCondition();
                                                        Toast.Show(LangHandler.Get("stt-45", "Target not found."));
                                                    }
                                                    else if (card.heroInfo.type == DBHero.TYPE_GOD)
                                                    {
                                                        //SkillFailCondition();
                                                        Toast.Show(LangHandler.Get("stt-45", "Target not found."));
                                                    }
                                                    else
                                                    {
                                                        if (card.heroInfo.type == DBHero.TYPE_TROOPER_NORMAL && isSummon)
                                                        {
                                                            CommonVector cv = new CommonVector();
                                                            cv.aLong.Add(card.battleID);
                                                            cv.aLong.Add(row);
                                                            cv.aLong.Add(col);
                                                            Game.main.socket.GameSummonCardInBatttle(cv);
                                                        }
                                                    }
                                                    SkillFailCondition();

                                                }
                                                else if (skillInfo.info > 0)
                                                {
                                                    // trường hợp k co target thi sẽ gui -1
                                                    lstTarget.aLong.Add(-1);
                                                    if (countEffect < skillInfo.lstEffect.Count - 1)
                                                    {
                                                        countEffect++;
                                                        Find();
                                                    }
                                                    else
                                                    {
                                                        if (countSkillInfo < card.skill.lstEffectsSkills.Count - 1)
                                                        {
                                                            countSkillInfo++;
                                                            countEffect = 0;
                                                            Find();
                                                        }
                                                        else
                                                        {
                                                            ActiveSkill();
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                cards.ForEach(x =>
                                                {
                                                    x.HighlightUnit();
                                                    x.onAddToListSkill += AddBoardCardToSkillList;
                                                    //x.onRemoveFromListSkill += RemoveBoardCardFromSkillList;
                                                    x.onEndSkillActive += OnEndSkillBoard;
                                                });
                                            }
                                            skillState = SkillState.ANY_MORTAL_BUT_SELF;
                                            break;
                                        }
                                    case DBHeroSkill.ANY_ALLY_TARGET_BUT_SELF:
                                        {

                                            chooseTargets.text = LangHandler.Get("137", "Choose 1 Ally target");
                                            GetListPlayerCardInBattle().Where(c => c != card).ToList().ForEach(c =>
                                            {
                                                c.HighlightUnit();
                                                c.onAddToListSkill += AddBoardCardToSkillList;
                                                // c.onRemoveFromListSkill += RemoveBoardCardFromSkillList;
                                                c.onEndSkillActive += OnEndSkillBoard;
                                            });
                                            lstTowerInBattle.Where(t => t.pos == 0 && t.towerHealth > 0).ToList().ForEach(x =>
                                            {
                                                x.HighLightTower();
                                                x.onAddToListSkill += AddTowerToSkillList;
                                                // x.onRemoveFromListSkill += RemoveTowerFromSkillList;
                                                x.onEndSkillActive += OnEndSkillTower;
                                            });
                                            skillState = SkillState.ANY_ALLY_TARGET_BUT_SELF;
                                            break;
                                        }
                                    case DBHeroSkill.TWO_ANY_ALLIES_BUT_SELF:
                                        {

                                            chooseTargets.text = LangHandler.Get("126", "Choose 2 Allies");
                                            List<BoardCard> cards = GetListPlayerCardInBattle().Where(x => x != card).ToList();
                                            if (cards.Count <= 1)
                                            {
                                                if (skillInfo.info == 0 && countSkillInfo == 0)
                                                {
                                                    if (card.heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || card.heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
                                                    {
                                                        card.GetComponent<HandCard>().MoveFail();
                                                        //SkillFailCondition();
                                                        Toast.Show(LangHandler.Get("stt-45", "Target not found."));
                                                    }
                                                    else if (card.heroInfo.type == DBHero.TYPE_GOD)
                                                    {
                                                        //SkillFailCondition();
                                                        Toast.Show(LangHandler.Get("stt-45", "Target not found."));
                                                    }
                                                    else
                                                    {
                                                        if (card.heroInfo.type == DBHero.TYPE_TROOPER_NORMAL && isSummon)
                                                        {
                                                            CommonVector cv = new CommonVector();
                                                            cv.aLong.Add(card.battleID);
                                                            cv.aLong.Add(row);
                                                            cv.aLong.Add(col);
                                                            Game.main.socket.GameSummonCardInBatttle(cv);
                                                        }
                                                    }
                                                    SkillFailCondition();

                                                }
                                                else if (skillInfo.info > 0)
                                                {
                                                    // trường hợp k co target thi sẽ gui -1
                                                    lstTarget.aLong.Add(-1);
                                                    if (countEffect < skillInfo.lstEffect.Count - 1)
                                                    {
                                                        countEffect++;
                                                        Find();
                                                    }
                                                    else
                                                    {
                                                        if (countSkillInfo < card.skill.lstEffectsSkills.Count - 1)
                                                        {
                                                            countSkillInfo++;
                                                            countEffect = 0;
                                                            Find();
                                                        }
                                                        else
                                                        {
                                                            ActiveSkill();
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                cards.ForEach(x =>
                                                {
                                                    x.HighlightUnit();
                                                    x.onAddToListSkill += AddBoardCardToSkillList;
                                                    //x.onRemoveFromListSkill += RemoveBoardCardFromSkillList;
                                                    x.onEndSkillActive += OnEndSkillBoard;
                                                });
                                                skillState = SkillState.TWO_ANY_ALLIES_BUT_SELF;
                                            }
                                            break;
                                        }
                                    case DBHeroSkill.TWO_ANY_ALLIES_JUNGLE_LAW:
                                        {
                                            chooseTargets.text = LangHandler.Get("124", "Choose upto 2 allies");
                                            List<BoardCard> cards = GetListPlayerCardInBattle();
                                            CardOnBoardClone clone = null;
                                            if (card.newCardClone != null)
                                                clone = card.newCardClone.GetComponent<CardOnBoardClone>();

                                            if (clone != null)
                                            {
                                                cards.ForEach(x =>
                                                {
                                                    x.HighlightUnit();
                                                    x.onAddToListSkill += AddBoardCardToSkillList;
                                                    //x.onRemoveFromListSkill += RemoveBoardCardFromSkillList;
                                                    x.onEndSkillActive += OnEndSkillBoard;
                                                });
                                                clone.HighlightUnit();
                                                clone.onAddToListSkill += AddCloneCardToSkillList;
                                                //clone.onRemoveFromListSkill += RemoveCloneCardFromSkillList;
                                                clone.onEndSkillActive += OnEndSkillClone;
                                                skillState = SkillState.TWO_ANY_ALLIES_JUNGLE_LAW;
                                            }
                                            else
                                            {
                                                int count = cards.Count;
                                                if (cards.Count == 0)
                                                {
                                                    if (skillInfo.info == 0 && countSkillInfo == 0)
                                                    {
                                                        if (card.heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || card.heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
                                                        {
                                                            card.GetComponent<HandCard>().MoveFail();
                                                            //SkillFailCondition();
                                                            Toast.Show(LangHandler.Get("stt-45", "Target not found."));
                                                        }
                                                        else if (card.heroInfo.type == DBHero.TYPE_GOD)
                                                        {
                                                            //SkillFailCondition();
                                                            Toast.Show(LangHandler.Get("stt-45", "Target not found."));
                                                        }
                                                        else
                                                        {
                                                            if (card.heroInfo.type == DBHero.TYPE_TROOPER_NORMAL && isSummon)
                                                            {
                                                                CommonVector cv = new CommonVector();
                                                                cv.aLong.Add(card.battleID);
                                                                cv.aLong.Add(row);
                                                                cv.aLong.Add(col);
                                                                Game.main.socket.GameSummonCardInBatttle(cv);
                                                            }
                                                        }
                                                        SkillFailCondition();

                                                    }
                                                    else if (skillInfo.info > 0)
                                                    {
                                                        // trường hợp k co target thi sẽ gui -1
                                                        lstTarget.aLong.Add(-1);
                                                        if (countEffect < skillInfo.lstEffect.Count - 1)
                                                        {
                                                            countEffect++;
                                                            Find();
                                                        }
                                                        else
                                                        {
                                                            if (countSkillInfo < card.skill.lstEffectsSkills.Count - 1)
                                                            {
                                                                countSkillInfo++;
                                                                countEffect = 0;
                                                                Find();
                                                            }
                                                            else
                                                            {
                                                                ActiveSkill();
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    cards.ForEach(x =>
                                                    {

                                                        LogWriterHandle.WriteLog("check2" + cards.Count() + "/" + countEffect);
                                                        LogWriterHandle.WriteLog("log skill " + x.battleID + "___________" + x.gameObject.name);
                                                        x.HighlightUnit();
                                                        x.onAddToListSkill += AddBoardCardToSkillList;
                                                        //x.onRemoveFromListSkill += RemoveBoardCardFromSkillList;
                                                        x.onEndSkillActive += OnEndSkillBoard;
                                                    });
                                                    skillState = SkillState.TWO_ANY_ALLIES_JUNGLE_LAW;
                                                }
                                            }
                                            break;
                                        }
                                    case DBHeroSkill.ANY_UNIT_BUT_SELF:
                                        {

                                            chooseTargets.text = LangHandler.Get("127", "Choose 1 Unit");
                                            lstCardInBattle.Where(x => x != card).ToList().ForEach(x =>
                                            {
                                                x.HighlightUnit();
                                                x.onAddToListSkill += AddBoardCardToSkillList;
                                                //x.onRemoveFromListSkill += RemoveBoardCardFromSkillList;
                                                x.onEndSkillActive += OnEndSkillBoard;
                                            });
                                            //if (card.newCardClone != null)
                                            //{
                                            //    CardOnBoardClone clone = card.newCardClone.GetComponent<CardOnBoardClone>();
                                            //    if (clone != null)
                                            //    {
                                            //        clone.HighlightUnit();
                                            //        clone.onAddToListSkill += AddCloneCardToSkillList;
                                            //        //clone.onRemoveFromListSkill += RemoveCloneCardFromSkillList;
                                            //        clone.onEndSkillActive += OnEndSkillClone;
                                            //    }

                                            //}
                                            skillState = SkillState.ANY_UNIT_BUT_SELF;
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                if (countEffect < skillInfo.lstEffect.Count - 1)
                                {
                                    countEffect++;
                                    Find();
                                }
                                else
                                {
                                    if (countSkillInfo < card.skill.lstEffectsSkills.Count - 1)
                                    {
                                        countSkillInfo++;
                                        countEffect = 0;
                                        Find();
                                    }

                                }
                            }
                        }
                    }
                }


            }
            //if (!skillPS)
            //{

            //    CommonVector cv = new CommonVector();
            //    cv.aLong.Add(card.skill.id);
            //    cv.aLong.Add(card.battleID);

            //    LogWriterHandle.WriteLog("ACTIVE SKILL NO TARGET =" + string.Join(",", cv.aLong));

            //    Game.main.socket.GameActiveSkill(cv);
            //}
        }
        void NextSkill()
        {
            if (countSkillInfo == card.skill.lstEffectsSkills.Count - 1 && countEffect == card.skill.lstEffectsSkills[countSkillInfo].lstEffect.Count - 1)
            {
                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                {
                    if (countSkillInfo == 0)
                    {
                        bool skillTrue = false;
                        if (card.skill.lstEffectsSkills[0].info > 0 && lstTarget.aLong.Count > 0)
                        {
                            foreach (EffectSkill eff in card.skill.lstEffectsSkills[0].lstEffect)
                                if (eff.target < 1000)
                                {
                                    skillTrue = true;
                                }
                            foreach (long id in lstTarget.aLong)
                                if (id != (-1))
                                    skillTrue = true;
                            if (skillTrue)
                            {
                                CommonVector cv = new CommonVector();
                                cv.aLong.Add(card.skill.id);
                                cv.aLong.Add(card.battleID);
                                if (lstTarget.aLong.Count > 0)
                                    cv.aLong.AddRange(lstTarget.aLong);

                                Game.main.socket.GameActiveSkill(cv);
                            }
                            else
                            {
                                Toast.Show(LangHandler.Get("stt-45", "Target not found"));
                                SkillFailCondition();
                            }

                        }
                        else
                        {
                            CommonVector cv = new CommonVector();
                            cv.aLong.Add(card.skill.id);
                            cv.aLong.Add(card.battleID);
                            if (lstTarget.aLong.Count > 0)
                                cv.aLong.AddRange(lstTarget.aLong);

                            Game.main.socket.GameActiveSkill(cv);
                        }
                    }
                    else
                    {
                        CommonVector cv = new CommonVector();
                        cv.aLong.Add(card.skill.id);
                        cv.aLong.Add(card.battleID);
                        if (lstTarget.aLong.Count > 0)
                            cv.aLong.AddRange(lstTarget.aLong);

                        Game.main.socket.GameActiveSkill(cv);
                    }


                }
                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                {
                    if (countSkillInfo == 0)
                    {
                        bool skillTrue = false;
                        if (card.skill.lstEffectsSkills[0].info > 0 && lstTarget.aLong.Count > 0)
                        {
                            foreach (EffectSkill eff in card.skill.lstEffectsSkills[0].lstEffect)
                                if (eff.target < 1000)
                                {
                                    skillTrue = true;
                                }
                            foreach (long id in lstTarget.aLong)
                                if (id != (-1))
                                    skillTrue = true;
                            if (skillTrue)
                            {
                                CommonVector cv = new CommonVector();
                                cv.aLong.Add(card.battleID);
                                if (card.heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || card.heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
                                {
                                    cv.aLong.Add(-1);
                                    cv.aLong.Add(-1);
                                }
                                else
                                {
                                    if (card.newCardClone != null)
                                    {
                                        cv.aLong.Add(card.newCardClone.GetComponent<CardOnBoardClone>().cloneSlot.xPos);
                                        cv.aLong.Add(card.newCardClone.GetComponent<CardOnBoardClone>().cloneSlot.yPos);
                                    }
                                }
                                cv.aLong.Add(card.skill.id);
                                if (lstTarget.aLong.Count > 0)
                                {
                                    cv.aLong.AddRange(lstTarget.aLong);
                                }
                                Game.main.socket.GameSummonCardInBatttle(cv);
                            }
                            else
                            {
                                if (card.heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || card.heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
                                {
                                    card.GetComponent<HandCard>().MoveFail();


                                    //SkillFailCondition();
                                    Toast.Show(LangHandler.Get("stt-45", "Target not found"));
                                }
                                else if (card.heroInfo.type == DBHero.TYPE_GOD)
                                {
                                    //SkillFailCondition();
                                    Toast.Show(LangHandler.Get("stt-45", "Target not found"));
                                }
                                else
                                {
                                    if (card.heroInfo.type == DBHero.TYPE_TROOPER_NORMAL && isSummon)
                                    {
                                        CommonVector cv = new CommonVector();
                                        cv.aLong.Add(card.battleID);
                                        cv.aLong.Add(row);
                                        cv.aLong.Add(col);
                                        Game.main.socket.GameSummonCardInBatttle(cv);
                                    }
                                }
                                SkillFailCondition();
                            }
                        }
                        else
                        {
                            CommonVector cv = new CommonVector();
                            cv.aLong.Add(card.battleID);
                            if (card.heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || card.heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
                            {
                                cv.aLong.Add(-1);
                                cv.aLong.Add(-1);
                            }
                            else
                            {
                                if (card.newCardClone != null)
                                {
                                    cv.aLong.Add(card.newCardClone.GetComponent<CardOnBoardClone>().cloneSlot.xPos);
                                    cv.aLong.Add(card.newCardClone.GetComponent<CardOnBoardClone>().cloneSlot.yPos);
                                }
                            }
                            cv.aLong.Add(card.skill.id);
                            if (lstTarget.aLong.Count > 0)
                            {
                                cv.aLong.AddRange(lstTarget.aLong);
                            }
                            Game.main.socket.GameSummonCardInBatttle(cv);
                        }
                    }
                    else
                    {
                        CommonVector cv = new CommonVector();
                        cv.aLong.Add(card.battleID);
                        if (card.heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || card.heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
                        {
                            cv.aLong.Add(-1);
                            cv.aLong.Add(-1);
                        }
                        else
                        {
                            if (card.newCardClone != null)
                            {
                                cv.aLong.Add(card.newCardClone.GetComponent<CardOnBoardClone>().cloneSlot.xPos);
                                cv.aLong.Add(card.newCardClone.GetComponent<CardOnBoardClone>().cloneSlot.yPos);
                            }
                        }
                        cv.aLong.Add(card.skill.id);
                        if (lstTarget.aLong.Count > 0)
                        {
                            cv.aLong.AddRange(lstTarget.aLong);
                        }
                        Game.main.socket.GameSummonCardInBatttle(cv);
                    }
                }
                OnEndSkillPhase();

            }
            else
            {
                OnFinishChooseOneTarget();
                if (countEffect < card.skill.lstEffectsSkills[countSkillInfo].lstEffect.Count - 1)
                {
                    countEffect++;
                    Find();
                }
                else
                {
                    if (countSkillInfo < card.skill.lstEffectsSkills.Count - 1)
                    {
                        countSkillInfo++;
                        countEffect = 0;
                        Find();
                    }
                }
            }
        }
        void OnActivateSkill()
        {
            //hien thi text tren man hinh khi cast phep
            if (card == null || card.skill == null)
                return;
            LogWriterHandle.WriteLog("----------------OnActivateSkill =" + skillState + "|target =" + card.skill.target);
            for (int i = 0; i <= card.skill.lstEffectsSkills.Count; i++)
            {
                if (i == countSkillInfo)
                {
                    ListEffectsSkill skillInfo = card.skill.lstEffectsSkills[i];
                    for (int j = 0; j < card.skill.lstEffectsSkills[i].lstEffect.Count; j++)
                    {
                        if (j == countEffect)
                        {
                            EffectSkill eff = card.skill.lstEffectsSkills[i].lstEffect[j];
                            if (eff.target < 1000)
                            {
                                //if (countSkillInfo == card.skill.lstEffectsSkills.Count - 1 && countEffect == skillInfo.lstEffect.Count - 1)
                                //{
                                //    if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                //    {
                                //        CommonVector cv = new CommonVector();
                                //        cv.aLong.Add(card.skill.id);
                                //        cv.aLong.Add(card.battleID);
                                //        if (lstTarget.aLong.Count > 0)
                                //            cv.aLong.AddRange(lstTarget.aLong);

                                //        Game.main.socket.GameActiveSkill(cv);
                                //        LogWriterHandle.WriteLog("----------------OnActivateSkill ACTIVE w target <= 1000 =" + string.Join(",", cv.aLong));
                                //    }
                                //    else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                //    {
                                //        CommonVector cv = new CommonVector();
                                //        cv.aLong.Add(card.battleID);
                                //        if (card.heroInfo.type == DBHero.TYPE_TROOPER_MAGIC)
                                //        {
                                //            cv.aLong.Add(-1);
                                //            cv.aLong.Add(-1);
                                //        }
                                //        else
                                //        {
                                //            cv.aLong.Add(card.newCardClone.GetComponent<CardOnBoardClone>().cloneSlot.xPos);
                                //            cv.aLong.Add(card.newCardClone.GetComponent<CardOnBoardClone>().cloneSlot.yPos);
                                //        }
                                //        cv.aLong.Add(card.skill.id);
                                //        if (lstTarget.aLong.Count > 0)
                                //        {
                                //            cv.aLong.AddRange(lstTarget.aLong);
                                //        }
                                //        Game.main.socket.GameSummonCardInBatttle(cv);
                                //    }
                                //}
                                //else
                                //{
                                //    if (countEffect < skillInfo.lstEffect.Count - 1)
                                //    {
                                //        countEffect++;
                                //        Find();
                                //    }
                                //    else
                                //    {
                                //        if (countSkillInfo < card.skill.lstEffectsSkills.Count - 1)
                                //        {
                                //            countSkillInfo++;
                                //            countEffect = 0;
                                //            Find();
                                //        }
                                //        else
                                //        {
                                //            ActiveSkill();
                                //        }
                                //    }
                                //}
                                NextSkill();

                            }
                            else
                            {
                                switch (skillState)
                                {
                                    case SkillState.CHOOSE_SELF_BLANK_NEXT:
                                        {
                                            LogWriterHandle.WriteLog("----------------OnActivateSkill = CHOSE_SELF_BLANK_NEXT");
                                            if (selectedCardSlot != null && card.skill != null)
                                            {
                                                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                {
                                                    long virtualHeroId = -1;
                                                    foreach (DBHero hero in Database.lstHero)
                                                        if (hero.id == card.heroID)
                                                            foreach (DBHeroSkill skill in hero.lstHeroSkill)
                                                                if (skill.id == card.skill.id)
                                                                    foreach (EffectSkill effect in skill.lstEffectSkill)
                                                                        if (effect.type == DBHeroSkill.EFFECT_SUMMON_VIRTUAL_HERO)
                                                                            virtualHeroId = effect.heroId;
                                                    lstTarget.aLong.Add(3);
                                                    lstTarget.aLong.Add(virtualHeroId);
                                                    lstTarget.aLong.Add(selectedCardSlot.xPos);
                                                    lstTarget.aLong.Add(selectedCardSlot.yPos);
                                                    //LogWriterHandle.WriteLog("===========TestSkillSumonVirtual: " + string.Join(",", cv.aLong));
                                                    //Game.main.socket.GameActiveSkill(cv);
                                                }
                                                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                {
                                                    lstTarget.aLong.Add(2);
                                                    lstTarget.aLong.Add(selectedCardSlot.xPos);
                                                    lstTarget.aLong.Add(selectedCardSlot.yPos);

                                                    //Game.main.socket.GameSummonCardInBatttle(cv);
                                                }
                                                NextSkill();
                                            }
                                            else
                                            {
                                                // không đủ điều kiện ở đây
                                                LogWriterHandle.WriteLog("Khong du dieu kien CHOSE_SELF_BLANK_NEXT");
                                            }
                                            break;
                                        }
                                    case SkillState.ANY_ALLY_UNIT:
                                        {
                                            LogWriterHandle.WriteLog("----------------OnActivateSkill ANY_ALLY_UNIT=" + lstSelectedSkillBoardCard.Count + "  " + card.skill.skill_type);

                                            if ((lstSelectedSkillBoardCard.Count == 1 || onBoardClone != null) && card.skill != null)
                                            {
                                                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                {
                                                    long heroBattleId;
                                                    if (onBoardClone != null)
                                                        heroBattleId = card.battleID;
                                                    else
                                                        heroBattleId = lstSelectedSkillBoardCard[0].battleID;

                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(heroBattleId);
                                                }
                                                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                {
                                                    long heroBattleId;
                                                    if (onBoardClone == null)
                                                        heroBattleId = lstSelectedSkillBoardCard[0].battleID;
                                                    else
                                                        heroBattleId = card.battleID;
                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(heroBattleId);
                                                }

                                                NextSkill();
                                            }
                                            else
                                            {
                                                LogWriterHandle.WriteLog("Khong du dieu kien ANY_ALLY_UNIT");
                                            }
                                            break;
                                        }
                                    case SkillState.ANY_ALLY_BUT_SELF:
                                        {
                                            LogWriterHandle.WriteLog("----------------OnActivateSkill = ANY_UNIT_BUT_SELF");

                                            if (lstSelectedSkillBoardCard.Count == 1 && card.skill != null)
                                            {
                                                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                {
                                                    long heroBattleId = lstSelectedSkillBoardCard[0].battleID;
                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(heroBattleId);
                                                }
                                                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                {
                                                    long heroBattleId = lstSelectedSkillBoardCard[0].battleID;

                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(heroBattleId);
                                                }
                                                NextSkill();
                                            }
                                            else
                                            {
                                                // không đủ điều kiện ở đây
                                                LogWriterHandle.WriteLog("Khong du dieu kien ANY_UNIT_BUT_SELF");
                                            }
                                            break;
                                        }
                                    case SkillState.TWO_ANY_ALLIES:
                                        {
                                            LogWriterHandle.WriteLog("TWO_ANY_ALLIES =" + lstSelectedSkillBoardCard.Count);
                                            if ((lstSelectedSkillBoardCard.Count == 2 || lstSelectedSkillBoardCard.Count == 1 && onBoardClone != null) && card.skill != null)
                                            {
                                                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                {

                                                    long heroBattleId1 = lstSelectedSkillBoardCard[0].battleID;
                                                    long x1 = lstSelectedSkillBoardCard[0].GetComponent<BoardCard>().slot.xPos;
                                                    long y1 = lstSelectedSkillBoardCard[0].GetComponent<BoardCard>().slot.yPos;

                                                    long heroBattleId2 = lstSelectedSkillBoardCard[1].battleID;
                                                    long x2 = lstSelectedSkillBoardCard[1].GetComponent<BoardCard>().slot.xPos;
                                                    long y2 = lstSelectedSkillBoardCard[1].GetComponent<BoardCard>().slot.yPos;
                                                    //LogWriterHandle.WriteLog("TWO_ANY_ALLIES=" + string.Join(",", cv.aLong));
                                                    lstTarget.aLong.Add(6);
                                                    lstTarget.aLong.Add(heroBattleId1);
                                                    lstTarget.aLong.Add(x2);
                                                    lstTarget.aLong.Add(y2);
                                                    lstTarget.aLong.Add(heroBattleId2);
                                                    lstTarget.aLong.Add(x1);
                                                    lstTarget.aLong.Add(y1);

                                                }
                                                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                {
                                                    long heroBattleId1, x1, y1;
                                                    long heroBattleId2, x2, y2;
                                                    if (onBoardClone == null)
                                                    {
                                                        heroBattleId1 = lstSelectedSkillBoardCard[0].battleID;
                                                        x1 = lstSelectedSkillBoardCard[0].GetComponent<BoardCard>().slot.xPos;
                                                        y1 = lstSelectedSkillBoardCard[0].GetComponent<BoardCard>().slot.yPos;

                                                        heroBattleId2 = lstSelectedSkillBoardCard[1].battleID;
                                                        x2 = lstSelectedSkillBoardCard[1].GetComponent<BoardCard>().slot.xPos;
                                                        y2 = lstSelectedSkillBoardCard[1].GetComponent<BoardCard>().slot.yPos;
                                                    }
                                                    else
                                                    {
                                                        heroBattleId1 = card.battleID;
                                                        x1 = lstSelectedSkillBoardCard[0].GetComponent<BoardCard>().slot.xPos;
                                                        y1 = lstSelectedSkillBoardCard[0].GetComponent<BoardCard>().slot.yPos;

                                                        heroBattleId2 = lstSelectedSkillBoardCard[1].battleID;
                                                        x2 = card.newCardClone.GetComponent<CardOnBoardClone>().cloneSlot.xPos;
                                                        y2 = card.newCardClone.GetComponent<CardOnBoardClone>().cloneSlot.yPos;
                                                    }
                                                    CommonVector cv = new CommonVector();
                                                    cv.aLong.Add(card.battleID);
                                                    if (card.heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || card.heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
                                                    {
                                                        cv.aLong.Add(-1);
                                                        cv.aLong.Add(-1);
                                                    }
                                                    else
                                                    {
                                                        cv.aLong.Add(card.newCardClone.GetComponent<CardOnBoardClone>().cloneSlot.xPos);
                                                        cv.aLong.Add(card.newCardClone.GetComponent<CardOnBoardClone>().cloneSlot.yPos);
                                                    }


                                                    lstTarget.aLong.Add(6);
                                                    lstTarget.aLong.Add(heroBattleId1);
                                                    lstTarget.aLong.Add(x1);
                                                    lstTarget.aLong.Add(y1);
                                                    lstTarget.aLong.Add(heroBattleId2);
                                                    lstTarget.aLong.Add(x2);
                                                    lstTarget.aLong.Add(y2);
                                                }

                                                NextSkill();
                                            }
                                            else
                                            {
                                                // không đủ điều kiện ở đây
                                                LogWriterHandle.WriteLog("Khong du dieu kien TWO_ANY_ALLIES");

                                            }
                                            break;
                                        }
                                    case SkillState.TWO_ANY_ENEMIES:
                                        {
                                            //LogWriterHandle.WriteLog("----------------OnActivateSkill TWO_ANY_ENEMIES ="+ lstSelectedSkillCard.Count);
                                            if (lstSelectedSkillBoardCard.Count == 2)
                                            {
                                                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                {

                                                    long heroBattle1 = lstSelectedSkillBoardCard[0].battleID;
                                                    long r1 = lstSelectedSkillBoardCard[0].GetComponent<BoardCard>().slot.xPos;
                                                    long c1 = lstSelectedSkillBoardCard[0].GetComponent<BoardCard>().slot.yPos;
                                                    long heroBattle2 = lstSelectedSkillBoardCard[1].battleID;
                                                    long r2 = lstSelectedSkillBoardCard[1].GetComponent<BoardCard>().slot.xPos;
                                                    long c2 = lstSelectedSkillBoardCard[1].GetComponent<BoardCard>().slot.yPos;

                                                    //LogWriterHandle.WriteLog("TWO_ANY_ENEMIES=" + string.Join(",", cv.aLong));
                                                    lstTarget.aLong.Add(6);
                                                    lstTarget.aLong.Add(heroBattle1);
                                                    lstTarget.aLong.Add(r2);
                                                    lstTarget.aLong.Add(c2);

                                                    lstTarget.aLong.Add(heroBattle2);
                                                    lstTarget.aLong.Add(r1);
                                                    lstTarget.aLong.Add(c1);
                                                }
                                                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                {
                                                    long heroBattle1 = lstSelectedSkillBoardCard[0].battleID;
                                                    long r1 = lstSelectedSkillBoardCard[0].GetComponent<BoardCard>().slot.xPos;
                                                    long c1 = lstSelectedSkillBoardCard[0].GetComponent<BoardCard>().slot.yPos;
                                                    long heroBattle2 = lstSelectedSkillBoardCard[1].battleID;
                                                    long r2 = lstSelectedSkillBoardCard[1].GetComponent<BoardCard>().slot.xPos;
                                                    long c2 = lstSelectedSkillBoardCard[1].GetComponent<BoardCard>().slot.yPos;
                                                    lstTarget.aLong.Add(6);
                                                    lstTarget.aLong.Add(heroBattle1);
                                                    lstTarget.aLong.Add(r2);
                                                    lstTarget.aLong.Add(c2);

                                                    lstTarget.aLong.Add(heroBattle2);
                                                    lstTarget.aLong.Add(r1);
                                                    lstTarget.aLong.Add(c1);

                                                }

                                                NextSkill();
                                            }
                                            else
                                            {
                                                // không đủ điều kiện ở đây
                                                LogWriterHandle.WriteLog("Khong du dieu kien TWO_ANY_ENEMIES");
                                            }
                                            break;
                                        }
                                    case SkillState.CHOOSE_FOUNTAIN:
                                        {
                                            LogWriterHandle.WriteLog("CHOSSE_FOUNTAIN=" + (selectedTower == null ? "NULL" : "NOT NULL"));

                                            if (selectedTower != null)
                                            {
                                                long foutainID = 0;

                                                switch (GetServerPostFromUsername(GameData.main.profile.username))
                                                {
                                                    // user o pos 6h -> send pos 12h
                                                    case POS_6h:
                                                        foutainID = -10 - (selectedTower.id + 1);
                                                        break;
                                                    // user o pos 12h -> send pos 6h
                                                    case POS_12h:
                                                        foutainID = -(selectedTower.id + 1);
                                                        break;
                                                }

                                                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                {
                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(foutainID);
                                                }
                                                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                {
                                                    //LogWriterHandle.WriteLog("CHOSSE_FOUNTAIN=" + string.Join(",", cv.aLong));
                                                    ////CHOSSE_FOUNTAIN = 72,-1,-1,42,6,0

                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(foutainID);
                                                }

                                                NextSkill();
                                            }
                                            else
                                            {
                                                // không đủ điều kiện ở đây
                                                LogWriterHandle.WriteLog("Khong du dieu kien TWO_ANY_ALLIES");
                                            }
                                            break;
                                        }
                                    case SkillState.ANY_LANE_UNIT:
                                        {
                                            LogWriterHandle.WriteLog("CHOOSE_LANE=" + (selectedLane == null ? "NULL" : "NOT NULL"));

                                            if (selectedLane != null)
                                            {
                                                long laneID = selectedLane.id;
                                                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                {

                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(laneID);
                                                }
                                                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                {

                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(laneID);
                                                }

                                                NextSkill();
                                            }
                                            else
                                            {
                                                // không đủ điều kiện ở đây
                                                LogWriterHandle.WriteLog("Khong du dieu kien TWO_ANY_ALLIES");
                                            }
                                            break;
                                        }
                                    case SkillState.CHOOSE_LANE:
                                        { // gui len selectedLane.pos (pos 0 hoac pos 2)
                                          //selectedLane.id;

                                            LogWriterHandle.WriteLog("CHOOSE_LANE=" + (selectedLane == null ? "NULL" : "NOT NULL"));

                                            if (selectedLane != null)
                                            {

                                                long laneID = selectedLane.id;
                                                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                {
                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(laneID);
                                                }
                                                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                {
                                                    //LogWriterHandle.WriteLog("selectedLane=" + string.Join(",", cv.aLong));
                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(laneID);
                                                }

                                                NextSkill();
                                            }
                                            else
                                            {
                                                // không đủ điều kiện ở đây
                                                LogWriterHandle.WriteLog("Khong du dieu kien TWO_ANY_ALLIES");

                                            }
                                            break;
                                        }

                                    case SkillState.ANY_UNIT:
                                        {
                                            LogWriterHandle.WriteLog("----------------OnActivateSkill ANY_UNIT=" + lstSelectedSkillBoardCard.Count + "  " + card.skill.skill_type);

                                            if ((lstSelectedSkillBoardCard.Count == 1 || onBoardClone != null) && card.skill != null)
                                            {
                                                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                {
                                                    long heroBattleId = lstSelectedSkillBoardCard[0].battleID;
                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(heroBattleId);
                                                }
                                                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                {
                                                    long heroBattleId;
                                                    if (onBoardClone != null)
                                                        heroBattleId = card.battleID;
                                                    else
                                                        heroBattleId = lstSelectedSkillBoardCard[0].battleID;
                                                    //LogWriterHandle.WriteLog("----------------OnActivateSkill SUMMON ANY_UNIT=" + string.Join(",", cv.aLong));
                                                    //Game.main.socket.GameSummonCardInBatttle(cv);
                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(heroBattleId);
                                                }

                                                NextSkill();
                                            }
                                            else
                                            {
                                                // không đủ điều kiện ở đây
                                                LogWriterHandle.WriteLog("Khong du dieu kien ANY_UNIT");

                                            }
                                            break;
                                        }
                                    case SkillState.ANY_ENEMY_UNIT:
                                        {
                                            LogWriterHandle.WriteLog("----------------OnActivateSkill ANY_ENEMY_UNIT=" + lstSelectedSkillBoardCard.Count + "  " + card.skill.skill_type);

                                            if (lstSelectedSkillBoardCard.Count == 1 && card.skill != null)
                                            {
                                                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                {
                                                    long heroBattleId = lstSelectedSkillBoardCard[0].battleID;

                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(heroBattleId);
                                                }
                                                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                {
                                                    long heroBattleId = lstSelectedSkillBoardCard[0].battleID;
                                                    //LogWriterHandle.WriteLog("----------------OnActivateSkill SUMMON ANY_ENEMY_UNIT=" + string.Join(",", cv.aLong));
                                                    //Game.main.socket.GameSummonCardInBatttle(cv);
                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(heroBattleId);
                                                }

                                                NextSkill();
                                            }
                                            else
                                            {
                                                // không đủ điều kiện ở đây
                                                LogWriterHandle.WriteLog("Khong du dieu kien ANY_ENEMY_UNIT");


                                            }
                                            break;
                                        }
                                    case SkillState.ANY_ALL_UNIT:
                                        {
                                            LogWriterHandle.WriteLog("----------------OnActivateSkill ANY_ALL_UNIT=" + lstSelectedSkillBoardCard.Count + "  " + card.skill.skill_type);

                                            long selectedBattleId = -1000;

                                            if (selectedTower != null)
                                                switch (GetServerPostFromUsername(GameData.main.profile.username))
                                                {
                                                    // user o pos 6h -> send pos 12h
                                                    case POS_6h:
                                                        selectedBattleId = -10 - (selectedTower.id + 1);
                                                        break;
                                                    // user o pos 12h -> send pos 6h
                                                    case POS_12h:
                                                        selectedBattleId = -(selectedTower.id + 1);
                                                        break;
                                                }

                                            if (lstSelectedSkillBoardCard.Count == 1)
                                                selectedBattleId = lstSelectedSkillBoardCard[0].battleID;
                                            if (onBoardClone != null)
                                                selectedBattleId = card.battleID;

                                            if (selectedBattleId != -1000 && card.skill != null)
                                            {
                                                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                {
                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(selectedBattleId);
                                                }
                                                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                {
                                                    //LogWriterHandle.WriteLog("----------------OnActivateSkill ANY_ALL_UNIT=" + string.Join(",", cv.aLong));
                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(selectedBattleId);

                                                }
                                                NextSkill();
                                            }
                                            else
                                            {
                                                // không đủ điều kiện ở đây
                                                LogWriterHandle.WriteLog("Khong du dieu kien ANY_ALL_UNIT");
                                            }
                                            break;
                                        }
                                    case SkillState.MY_HAND_CARD:
                                        {
                                            LogWriterHandle.WriteLog("----------------OnActivateSkill MY_HAND_CARD=" + lstSelectedSkillHandCard.Count + "  " + card.skill.skill_type);

                                            if (lstSelectedSkillHandCard.Count == 1 && card.skill != null)
                                            {
                                                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                {
                                                    long heroBattleId = lstSelectedSkillHandCard[0].battleID;

                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(heroBattleId);
                                                }
                                                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                {
                                                    long heroBattleId = lstSelectedSkillHandCard[0].battleID;

                                                    //LogWriterHandle.WriteLog("----------------OnActivateSkill MY_HAND_CARD=" + string.Join(",", cv.aLong));
                                                    //Game.main.socket.GameSummonCardInBatttle(cv);
                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(heroBattleId);

                                                }

                                                NextSkill();
                                            }
                                            else
                                            {
                                                // không đủ điều kiện ở đây
                                                LogWriterHandle.WriteLog("Khong du dieu kien MY_HAND_CARD");
                                            }
                                            break;
                                        }
                                    case SkillState.ANY_MOTAL:
                                        {
                                            LogWriterHandle.WriteLog("----------------OnActivateSkill ANY_MOTAL=" + lstSelectedSkillBoardCard.Count + "  " + card.skill.skill_type);

                                            if ((lstSelectedSkillBoardCard.Count == 1 || onBoardClone != null) && card.skill != null)
                                            {
                                                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                {
                                                    long heroBattleId = lstSelectedSkillBoardCard[0].battleID;

                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(heroBattleId);
                                                }
                                                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                {
                                                    long heroBattleId;
                                                    if (onBoardClone != null)
                                                        heroBattleId = card.battleID;
                                                    else
                                                        heroBattleId = lstSelectedSkillBoardCard[0].battleID;
                                                    //LogWriterHandle.WriteLog("----------------OnActivateSkill SUMMON ANY_MOTAL=" + string.Join(",", cv.aLong));
                                                    //Game.main.socket.GameSummonCardInBatttle(cv);

                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(heroBattleId);
                                                }
                                                NextSkill();
                                            }
                                            else
                                            {
                                                // không đủ điều kiện ở đây
                                                LogWriterHandle.WriteLog("Khong du dieu kien ANY_MOTAL");
                                            }
                                            break;
                                        }
                                    case SkillState.ANY_BLANK_ALLY:
                                        {
                                            LogWriterHandle.WriteLog("----------------OnActivateSkill = ANY_BLANK_ALLY");
                                            if (selectedCardSlot != null && card.skill != null)
                                            {
                                                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                {
                                                    //long virtualHeroId = -1;
                                                    //foreach (DBHero hero in Database.lstHero)
                                                    //    if (hero.id == card.heroID)
                                                    //        foreach (DBHeroSkill skill in hero.lstHeroSkill)
                                                    //            if (skill.id == card.skill.id)
                                                    //                foreach (EffectSkill effect in skill.lstEffectSkill)
                                                    //                    if (effect.type == DBHeroSkill.EFFECT_SUMMON_VIRTUAL_HERO)
                                                    //                        virtualHeroId = effect.heroId;

                                                    //LogWriterHandle.WriteLog("===========TestSkillSumonVirtual: " + string.Join(",", cv.aLong));
                                                    //Game.main.socket.GameActiveSkill(cv);

                                                    lstTarget.aLong.Add(2);
                                                    lstTarget.aLong.Add(selectedCardSlot.xPos);
                                                    lstTarget.aLong.Add(selectedCardSlot.yPos);
                                                }
                                                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                {
                                                    lstTarget.aLong.Add(2);
                                                    lstTarget.aLong.Add(selectedCardSlot.xPos);
                                                    lstTarget.aLong.Add(selectedCardSlot.yPos);
                                                }

                                                NextSkill();
                                            }
                                            else
                                            {
                                                // không đủ điều kiện ở đây
                                                LogWriterHandle.WriteLog("Khong du dieu kien ANY_BLANK_ALLY");
                                            }
                                            break;
                                        }
                                    case SkillState.ANY_LANE_ENEMY:
                                        { // gui len selectedLane.pos (pos 0 hoac pos 2)
                                            LogWriterHandle.WriteLog("ANY_LANE_ENEMY=" + (selectedLane == null ? "NULL" : "NOT NULL"));
                                            if (lstSelectedSkillBoardCard.Count == 1 && card.skill != null)
                                            {
                                                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                {
                                                    long heroBattleId = lstSelectedSkillBoardCard[0].battleID;
                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(heroBattleId);
                                                }
                                                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                {
                                                    long heroBattleId = lstSelectedSkillBoardCard[0].battleID;
                                                    //LogWriterHandle.WriteLog("----------------OnActivateSkill SUMMON ANY_LANE_ENEMY =" + string.Join(",", cv.aLong));
                                                    //Game.main.socket.GameSummonCardInBatttle(cv);
                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(heroBattleId);
                                                }
                                                NextSkill();
                                            }
                                            else
                                            {
                                                // không đủ điều kiện ở đây
                                                LogWriterHandle.WriteLog("Khong du dieu kien ANY_LANE_ENEMY");
                                            }
                                            break;
                                        }

                                    case SkillState.ANY_COL_ENEMY:
                                        {
                                            LogWriterHandle.WriteLog("----------------OnActivateSkill ANY_COL_ENEMY=" + lstSelectedSkillBoardCard.Count + "  " + card.skill.skill_type);

                                            if (lstSelectedSkillBoardCard.Count == 1 && card.skill != null)
                                            {
                                                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                {
                                                    long heroBattleId = lstSelectedSkillBoardCard[0].battleID;
                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(heroBattleId);
                                                }
                                                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                {
                                                    long heroBattleId = lstSelectedSkillBoardCard[0].battleID;

                                                    //LogWriterHandle.WriteLog("----------------OnActivateSkill SUMMON ANY_COL_ENEMY=" + string.Join(",", cv.aLong));
                                                    //Game.main.socket.GameSummonCardInBatttle(cv);
                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(heroBattleId);
                                                }

                                                NextSkill();
                                            }
                                            else
                                            {
                                                // không đủ điều kiện ở đây
                                                LogWriterHandle.WriteLog("Khong du dieu kien ANY_COL_ENEMY ");
                                            }
                                            break;
                                        }
                                    case SkillState.ANY_TARGET:
                                        {
                                            LogWriterHandle.WriteLog("----------------OnActivateSkill ANY_TARGET=" + lstSelectedSkillBoardCard.Count + "  " + card.skill.skill_type);

                                            long bTargetId = -1;
                                            if (lstSelectedSkillBoardCard.Count == 1)
                                            {
                                                bTargetId = lstSelectedSkillBoardCard[0].battleID;
                                            }
                                            else
                                             if (selectedTower != null)
                                            {

                                                //switch (GetServerPostFromUsername(GameData.main.profile.username))
                                                switch (GetServerPostFromClientPos(selectedTower.pos))
                                                {
                                                    // user o pos 6h -> send pos 12h
                                                    case POS_6h:
                                                        bTargetId = -(selectedTower.id + 1);
                                                        break;
                                                    // user o pos 12h -> send pos 6h
                                                    case POS_12h:
                                                        bTargetId = -10 - (selectedTower.id + 1);
                                                        break;
                                                }
                                            }
                                            else if (onBoardClone != null)
                                            {
                                                bTargetId = card.battleID;
                                            }
                                            LogWriterHandle.WriteLog(bTargetId + " ================== " + card.skill);
                                            if (bTargetId != 0 && card.skill != null)
                                            {
                                                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                {

                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(bTargetId);
                                                }
                                                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                {

                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(bTargetId);
                                                }

                                                NextSkill();
                                            }
                                            else
                                            {
                                                // không đủ điều kiện ở đây
                                                LogWriterHandle.WriteLog("Khong du dieu kien ANY_TARGET");
                                            }
                                            break;
                                        }
                                    case SkillState.ANY_ALLY_TARGET:
                                        {
                                            LogWriterHandle.WriteLog("----------------OnActivateSkillANY_ALLY_TARGET=" + lstSelectedSkillBoardCard.Count + "  " + card.skill.skill_type);

                                            long bTargetId = -1;
                                            if (lstSelectedSkillBoardCard.Count == 1)
                                            {
                                                bTargetId = lstSelectedSkillBoardCard[0].battleID;
                                            }
                                            else
                                             if (selectedTower != null)
                                            {

                                                //switch (GetServerPostFromUsername(GameData.main.profile.username))
                                                switch (GetServerPostFromClientPos(selectedTower.pos))
                                                {
                                                    // user o pos 6h -> send pos 12h
                                                    case POS_6h:
                                                        bTargetId = -(selectedTower.id + 1);
                                                        break;
                                                    // user o pos 12h -> send pos 6h
                                                    case POS_12h:
                                                        bTargetId = -10 - (selectedTower.id + 1);
                                                        break;
                                                }
                                            }
                                            else if (onBoardClone != null)
                                            {
                                                bTargetId = card.battleID;
                                            }
                                            LogWriterHandle.WriteLog(bTargetId + " ================== " + card.skill);
                                            if (bTargetId != 0 && card.skill != null)
                                            {
                                                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                {
                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(bTargetId);
                                                }
                                                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                {
                                                    //LogWriterHandle.WriteLog("----------------OnActivateSkill SUMMON ANY_TARGET=" + string.Join(",", cv.aLong));
                                                    //Game.main.socket.GameSummonCardInBatttle(cv);
                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(bTargetId);
                                                }
                                                NextSkill();
                                            }
                                            else
                                            {
                                                // không đủ điều kiện ở đây
                                                LogWriterHandle.WriteLog("Khong du dieu kien ANY_TARGET");
                                            }
                                            break;
                                        }
                                    case SkillState.RANDOM_UNIT_IN_SELECTED_LANE:
                                        {
                                            // gui len selectedLane.pos (pos 0 hoac pos 2)
                                            //selectedLane.id;

                                            LogWriterHandle.WriteLog("RANDOM_UNIT_IN_SELECTED_LANE=" + (selectedLane == null ? "NULL" : "NOT NULL"));

                                            if (selectedLane != null)
                                            {
                                                long laneID = selectedLane.id;
                                                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                {

                                                    //LogWriterHandle.WriteLog("selectedLane=" + string.Join(",", cv.aLong));

                                                    //Game.main.socket.GameActiveSkill(cv);
                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(laneID);
                                                }
                                                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                {
                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(laneID);
                                                }

                                                NextSkill();
                                            }
                                            else
                                            {
                                                // không đủ điều kiện ở đây
                                                LogWriterHandle.WriteLog("Khong du dieu kien RANDOM_UNIT_IN_SELECTED_LANE");
                                            }

                                            break;
                                        }
                                    case SkillState.ANY_TARGET_BUT_SELF:
                                        {
                                            LogWriterHandle.WriteLog("----------------OnActivateSkill ANY_TARGET_BUT_SELF=" + lstSelectedSkillBoardCard.Count + "  " + card.skill.skill_type);

                                            long bTargetId = -1;
                                            if (lstSelectedSkillBoardCard.Count == 1)
                                            {
                                                bTargetId = lstSelectedSkillBoardCard[0].battleID;
                                            }
                                            else
                                             if (selectedTower != null)
                                            {

                                                //switch (GetServerPostFromUsername(GameData.main.profile.username))
                                                switch (GetServerPostFromClientPos(selectedTower.pos))
                                                {
                                                    // user o pos 6h -> send pos 12h
                                                    case POS_6h:
                                                        bTargetId = -(selectedTower.id + 1);
                                                        break;
                                                    // user o pos 12h -> send pos 6h
                                                    case POS_12h:
                                                        bTargetId = -10 - (selectedTower.id + 1);
                                                        break;
                                                }
                                            }
                                            LogWriterHandle.WriteLog(bTargetId + " ================== " + card.skill);
                                            if (bTargetId != 0 && card.skill != null)
                                            {
                                                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                {
                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(bTargetId);
                                                }
                                                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                {
                                                    //LogWriterHandle.WriteLog("----------------OnActivateSkill ANY_TARGET_BUT_SELF=" + string.Join(",", cv.aLong));
                                                    //Game.main.socket.GameSummonCardInBatttle(cv);
                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(bTargetId);
                                                }

                                                NextSkill();
                                            }
                                            else
                                            {
                                                // không đủ điều kiện ở đây
                                                LogWriterHandle.WriteLog("Khong du dieu kien ANY_TARGET_BUT_SELF");

                                            }
                                            break;
                                        }
                                    case SkillState.ANY_MORTAL_BUT_SELF:
                                        {
                                            LogWriterHandle.WriteLog("----------------OnActivateSkill ANY_MORTAL_BUT_SELF=" + lstSelectedSkillBoardCard.Count + "  " + card.skill.skill_type);

                                            if (lstSelectedSkillBoardCard.Count == 1 && card.skill != null)
                                            {
                                                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                {
                                                    long heroBattleId = lstSelectedSkillBoardCard[0].battleID;
                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(heroBattleId);
                                                }
                                                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                {
                                                    long heroBattleId = lstSelectedSkillBoardCard[0].battleID;

                                                    //LogWriterHandle.WriteLog("----------------OnActivateSkill SUMMON ANY_MORTAL_BUT_SELF=" + string.Join(",", cv.aLong));
                                                    //Game.main.socket.GameSummonCardInBatttle(cv);
                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(heroBattleId);

                                                }

                                                NextSkill();
                                            }
                                            else
                                            {
                                                // không đủ điều kiện ở đây
                                                LogWriterHandle.WriteLog("Khong du dieu kien ANY_MORTAL_BUT_SELF");
                                            }
                                            break;
                                        }
                                    case SkillState.ANY_ALLY_TARGET_BUT_SELF:
                                        {
                                            LogWriterHandle.WriteLog("----------------OnActivateSkillANY_ALLY_TARGET_BUT_SELF=" + lstSelectedSkillBoardCard.Count + "  " + card.skill.skill_type);

                                            long bTargetId = -1;
                                            if (lstSelectedSkillBoardCard.Count == 1)
                                            {
                                                bTargetId = lstSelectedSkillBoardCard[0].battleID;
                                            }
                                            else
                                             if (selectedTower != null)
                                            {

                                                //switch (GetServerPostFromUsername(GameData.main.profile.username))
                                                switch (GetServerPostFromClientPos(selectedTower.pos))
                                                {
                                                    // user o pos 6h -> send pos 12h
                                                    case POS_6h:
                                                        bTargetId = -(selectedTower.id + 1);
                                                        break;
                                                    // user o pos 12h -> send pos 6h
                                                    case POS_12h:
                                                        bTargetId = -10 - (selectedTower.id + 1);
                                                        break;
                                                }
                                            }
                                            LogWriterHandle.WriteLog(bTargetId + " ================== " + card.skill);
                                            if (bTargetId != 0 && card.skill != null)
                                            {
                                                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                {

                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(bTargetId);
                                                }
                                                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                {
                                                    //LogWriterHandle.WriteLog("----------------OnActivateSkill ANY_ALLY_TARGET_BUT_SELF=" + string.Join(",", cv.aLong));
                                                    //Game.main.socket.GameSummonCardInBatttle(cv);

                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(bTargetId);
                                                }

                                                NextSkill();
                                            }
                                            else
                                            {
                                                // không đủ điều kiện ở đây
                                                LogWriterHandle.WriteLog("Khong du dieu kien ANY_ALLY_TARGET_BUT_SELF");
                                            }
                                            break;
                                        }
                                    case SkillState.TWO_ANY_ALLIES_BUT_SELF:
                                        {
                                            LogWriterHandle.WriteLog("TWO_ANY_ALLIES_BUT_SELF=" + lstSelectedSkillBoardCard.Count);
                                            if (lstSelectedSkillBoardCard.Count == 2)
                                            {
                                                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                {

                                                    long heroBattleId1 = lstSelectedSkillBoardCard[0].battleID;
                                                    long x1 = lstSelectedSkillBoardCard[0].GetComponent<BoardCard>().slot.xPos;
                                                    long y1 = lstSelectedSkillBoardCard[0].GetComponent<BoardCard>().slot.yPos;

                                                    long heroBattleId2 = lstSelectedSkillBoardCard[1].battleID;
                                                    long x2 = lstSelectedSkillBoardCard[1].GetComponent<BoardCard>().slot.xPos;
                                                    long y2 = lstSelectedSkillBoardCard[1].GetComponent<BoardCard>().slot.yPos;
                                                    //LogWriterHandle.WriteLog("TWO_ANY_ALLIES_BUT_SELF=" + string.Join(",", cv.aLong));

                                                    //Game.main.socket.GameActiveSkill(cv);

                                                    lstTarget.aLong.Add(6);
                                                    lstTarget.aLong.Add(heroBattleId1);
                                                    lstTarget.aLong.Add(x2);
                                                    lstTarget.aLong.Add(y2);
                                                    lstTarget.aLong.Add(heroBattleId2);
                                                    lstTarget.aLong.Add(x1);
                                                    lstTarget.aLong.Add(y1);
                                                }
                                                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                {
                                                    long heroBattleId1 = lstSelectedSkillBoardCard[0].battleID;
                                                    long x1 = lstSelectedSkillBoardCard[0].GetComponent<BoardCard>().slot.xPos;
                                                    long y1 = lstSelectedSkillBoardCard[0].GetComponent<BoardCard>().slot.yPos;

                                                    long heroBattleId2 = lstSelectedSkillBoardCard[1].battleID;
                                                    long x2 = lstSelectedSkillBoardCard[1].GetComponent<BoardCard>().slot.xPos;
                                                    long y2 = lstSelectedSkillBoardCard[1].GetComponent<BoardCard>().slot.yPos;

                                                    lstTarget.aLong.Add(6);
                                                    lstTarget.aLong.Add(heroBattleId1);
                                                    lstTarget.aLong.Add(x2);
                                                    lstTarget.aLong.Add(y2);
                                                    lstTarget.aLong.Add(heroBattleId2);
                                                    lstTarget.aLong.Add(x1);
                                                    lstTarget.aLong.Add(y1);
                                                }

                                                NextSkill();
                                            }
                                            else
                                            {
                                                // không đủ điều kiện ở đây
                                                LogWriterHandle.WriteLog("Khong du dieu kien TWO_ANY_ALLIES_BUT_SELF");
                                            }
                                            break;
                                        }
                                    case SkillState.ANY_UNIT_BUT_SELF:
                                        {
                                            LogWriterHandle.WriteLog("----------------OnActivateSkill ANY_UNIT_BUT_SELF=" + lstSelectedSkillBoardCard.Count + "  " + card.skill.skill_type);

                                            if (lstSelectedSkillBoardCard.Count == 1 && card.skill != null)
                                            {
                                                if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                {
                                                    long heroBattleId = lstSelectedSkillBoardCard[0].battleID;
                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(heroBattleId);
                                                }
                                                else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                {
                                                    long heroBattleId = lstSelectedSkillBoardCard[0].battleID;

                                                    lstTarget.aLong.Add(1);
                                                    lstTarget.aLong.Add(heroBattleId);

                                                }
                                                NextSkill();
                                            }
                                            else
                                            {
                                                // không đủ điều kiện ở đây
                                                LogWriterHandle.WriteLog("Khong du dieu kien ANY_UNIT_BUT_SELF");
                                            }
                                            break;
                                        }
                                    case SkillState.TWO_ANY_ALLIES_JUNGLE_LAW:
                                        {
                                            LogWriterHandle.WriteLog("----------------OnActivateSkill TWO_ANY_ALLIES_JUNGLE_LAW=" + lstSelectedSkillBoardCard.Count + "  " + card.skill.skill_type);
                                            {

                                                if (((onBoardClone != null && GetListPlayerCardInBattle().Count == 0 && lstSelectedSkillBoardCard.Count == 0) || (onBoardClone == null && GetListPlayerCardInBattle().Count == 1 && lstSelectedSkillBoardCard.Count == 1)) && card.skill != null)
                                                {
                                                    if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                    {
                                                        //god k co clone
                                                        long heroBattleId1, x1, y1;
                                                        heroBattleId1 = lstSelectedSkillBoardCard[0].battleID;
                                                        x1 = lstSelectedSkillBoardCard[0].GetComponent<BoardCard>().slot.xPos;
                                                        y1 = lstSelectedSkillBoardCard[0].GetComponent<BoardCard>().slot.yPos;
                                                        //LogWriterHandle.WriteLog("TWO_ANY_ALLIES_JUNGLE_LAW=" + string.Join(",", cv.aLong));

                                                        //Game.main.socket.GameActiveSkill(cv);
                                                        lstTarget.aLong.Add(3);
                                                        lstTarget.aLong.Add(heroBattleId1);
                                                        lstTarget.aLong.Add(x1);
                                                        lstTarget.aLong.Add(y1);

                                                    }
                                                    else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                    {
                                                        //lay target
                                                        long heroBattleId1, x1, y1;
                                                        if (onBoardClone == null)
                                                        {
                                                            heroBattleId1 = lstSelectedSkillBoardCard[0].battleID;
                                                            x1 = lstSelectedSkillBoardCard[0].GetComponent<BoardCard>().slot.xPos;
                                                            y1 = lstSelectedSkillBoardCard[0].GetComponent<BoardCard>().slot.yPos;
                                                        }
                                                        else
                                                        {
                                                            heroBattleId1 = card.battleID;
                                                            x1 = card.newCardClone.GetComponent<CardOnBoardClone>().cloneSlot.xPos;
                                                            y1 = card.newCardClone.GetComponent<CardOnBoardClone>().cloneSlot.yPos;
                                                        }


                                                        lstTarget.aLong.Add(3);
                                                        lstTarget.aLong.Add(heroBattleId1);
                                                        lstTarget.aLong.Add(x1);
                                                        lstTarget.aLong.Add(y1);
                                                    }

                                                    NextSkill();
                                                }
                                                else if ((onBoardClone != null && GetListPlayerCardInBattle().Count >= 1 && lstSelectedSkillBoardCard.Count == 1) || (onBoardClone == null && GetListPlayerCardInBattle().Count > 1 && lstSelectedSkillBoardCard.Count == 2) && card.skill != null)
                                                {
                                                    if (card.skill.skill_type == DBHeroSkill.TYPE_ACTIVE_SKILL)
                                                    {
                                                        //god k co clone
                                                        long heroBattleId1, x1, y1;
                                                        long heroBattleId2, x2, y2;
                                                        heroBattleId1 = lstSelectedSkillBoardCard[0].battleID;
                                                        x1 = lstSelectedSkillBoardCard[0].GetComponent<BoardCard>().slot.xPos;
                                                        y1 = lstSelectedSkillBoardCard[0].GetComponent<BoardCard>().slot.yPos;
                                                        if (onBoardClone == null)
                                                        {
                                                            heroBattleId2 = lstSelectedSkillBoardCard[1].battleID;
                                                            x2 = lstSelectedSkillBoardCard[1].GetComponent<BoardCard>().slot.xPos;
                                                            y2 = lstSelectedSkillBoardCard[1].GetComponent<BoardCard>().slot.yPos;

                                                        }
                                                        else
                                                        {
                                                            heroBattleId2 = card.battleID;
                                                            x2 = card.newCardClone.GetComponent<CardOnBoardClone>().cloneSlot.xPos;
                                                            y2 = card.newCardClone.GetComponent<CardOnBoardClone>().cloneSlot.yPos;

                                                        }
                                                        //LogWriterHandle.WriteLog("TWO_ANY_ALLIES_JUNGLE_LAW=" + string.Join(",", cv.aLong));

                                                        //Game.main.socket.GameActiveSkill(cv);

                                                        lstTarget.aLong.Add(6);
                                                        lstTarget.aLong.Add(heroBattleId1);
                                                        lstTarget.aLong.Add(x1);
                                                        lstTarget.aLong.Add(y1);
                                                        lstTarget.aLong.Add(heroBattleId2);
                                                        lstTarget.aLong.Add(x2);
                                                        lstTarget.aLong.Add(y2);
                                                    }
                                                    else if (card.skill.skill_type == DBHeroSkill.TYPE_SUMMON_SKILL)
                                                    {
                                                        //lay target
                                                        long heroBattleId1, x1, y1;
                                                        long heroBattleId2, x2, y2;
                                                        if (onBoardClone == null)
                                                        {
                                                            heroBattleId1 = lstSelectedSkillBoardCard[0].battleID;
                                                            x1 = lstSelectedSkillBoardCard[0].GetComponent<BoardCard>().slot.xPos;
                                                            y1 = lstSelectedSkillBoardCard[0].GetComponent<BoardCard>().slot.yPos;
                                                            heroBattleId2 = lstSelectedSkillBoardCard[1].battleID;
                                                            x2 = lstSelectedSkillBoardCard[1].GetComponent<BoardCard>().slot.xPos;
                                                            y2 = lstSelectedSkillBoardCard[1].GetComponent<BoardCard>().slot.yPos;

                                                        }
                                                        else
                                                        {
                                                            heroBattleId1 = card.battleID;
                                                            x1 = card.newCardClone.GetComponent<CardOnBoardClone>().cloneSlot.xPos;
                                                            y1 = card.newCardClone.GetComponent<CardOnBoardClone>().cloneSlot.yPos;
                                                            heroBattleId2 = lstSelectedSkillBoardCard[0].battleID;
                                                            x2 = lstSelectedSkillBoardCard[0].GetComponent<BoardCard>().slot.xPos;
                                                            y2 = lstSelectedSkillBoardCard[0].GetComponent<BoardCard>().slot.yPos;
                                                        }

                                                        lstTarget.aLong.Add(6);
                                                        lstTarget.aLong.Add(heroBattleId1);
                                                        lstTarget.aLong.Add(x1);
                                                        lstTarget.aLong.Add(y1);
                                                        lstTarget.aLong.Add(heroBattleId2);
                                                        lstTarget.aLong.Add(x2);
                                                        lstTarget.aLong.Add(y2);
                                                    }

                                                    NextSkill();

                                                }
                                                else
                                                {
                                                    LogWriterHandle.WriteLog("Khong du dieu kien TWO_ANY_ALLIES_JUNGLE_LAW ");
                                                }
                                            }
                                            break;
                                        }
                                }

                                //OnEndSkillPhase();
                            }

                        }
                    }
                }
            }
        }
        void AddHandCardToSkillList(HandCard cardToAdd)
        {
            lstSelectedSkillHandCard.Add(cardToAdd);
            UpdateSpellTarget(cardToAdd.transform, !isSummon ? card.transform : card.newCardClone);
            ActiveSkill();
        }

        void AddBoardCardToSkillList(BoardCard cardToAdd)
        {
            if (cardToAdd.canSelect && !lstSelectedSkillBoardCard.Contains(cardToAdd))
            {
                cardToAdd.canSelect = false;
                lstSelectedSkillBoardCard.Add(cardToAdd);
                UpdateSpellTarget(cardToAdd.transform, !isSummon ? card.transform : card.newCardClone);
                ActiveSkill();
            }
        }

        void AddCloneCardToSkillList(CardOnBoardClone cardToAdd)
        {
            onBoardClone = cardToAdd;
            UpdateSpellTarget(cardToAdd.transform, !isSummon ? card.transform : card.newCardClone);
            ActiveSkill();
        }

        void AddTowerToSkillList(TowerController tower)
        {
            selectedTower = tower;
            UpdateSpellTarget(tower.transform, !isSummon ? card.transform : card.newCardClone);
            ActiveSkill();
        }

        void AddLaneToSkillList(LaneController lane)
        {
            selectedLane = lane;
            UpdateSpellTarget(lane.transform, !isSummon ? card.transform : card.newCardClone);
            ActiveSkill();
        }

        void AddSlotToSkillList(CardSlot slot)
        {
            selectedCardSlot = slot;
            UpdateSpellTarget(slot.transform, !isSummon ? card.transform : card.newCardClone);
            ActiveSkill();
        }


        void OnEndSkillHand(HandCard card)
        {
            card.onAddToListSkill -= AddHandCardToSkillList;
            //
            //card.onRemoveFromListSkill -= RemoveHandCardFromSkillList;
            card.onEndSkillActive -= OnEndSkillHand;
        }

        void OnEndSkillBoard(BoardCard card)
        {
            card.onAddToListSkill -= AddBoardCardToSkillList;
            //card.onRemoveFromListSkill -= RemoveBoardCardFromSkillList;
            card.onEndSkillActive -= OnEndSkillBoard;
        }

        void OnEndSkillClone(CardOnBoardClone card)
        {
            card.onAddToListSkill -= AddCloneCardToSkillList;
            //card.onRemoveFromListSkill -= RemoveCloneCardFromSkillList;
            card.onEndSkillActive -= OnEndSkillClone;
        }

        void OnEndSkillTower(TowerController tower)
        {
            tower.onAddToListSkill -= AddTowerToSkillList;
            //tower.onRemoveFromListSkill -= RemoveTowerFromSkillList;
            tower.onEndSkillActive -= OnEndSkillTower;
        }

        void OnEndSkillLane(LaneController lane)
        {
            lane.onAddToListSkill -= AddLaneToSkillList;
            //lane.onRemoveFromListSkill -= RemoveLaneFromSkillList;
            lane.onEndSkillActive -= OnEndSkillLane;
        }

        void OnEndSkillSlot(CardSlot slot)
        {
            slot.onAddToListSkill -= AddSlotToSkillList;
            //slot.onRemoveFromListSkill -= RemoveSlotFromSkillList;
            slot.onEndSkillActive -= OnEndSkillSlot;
        }
    }

    public void DoClickCancelSkill()
    {
        if (curCardSkill != null)
            curCardSkill.MoveFail();
        OnEndSkillPhase();

    }
    //LogWriterHandle.WriteLog(card.skill.target);

    List<CardSlot> ChooseSelfBlankNext(BoardCard card)
    {
        CardSlot slot = card.slot;
        long xl = slot.xPos - 1;
        long xr = slot.xPos + 1;
        long yl = slot.yPos - 1;
        long yr = slot.yPos + 1;

        List<CardSlot> slots = new List<CardSlot>();

        // left
        CardSlot availableSlot1 = playerSlotContainer.FirstOrDefault(x => x.xPos == slot.xPos - 1 && x.yPos == slot.yPos && x.state == SlotState.Empty);
        if (availableSlot1 != null)
        {
            //availableSlot1.HighLightSlot();
            slots.Add(availableSlot1);
        }
        // right
        CardSlot availableSlot2 = playerSlotContainer.FirstOrDefault(x => x.xPos == slot.xPos + 1 && x.yPos == slot.yPos && x.state == SlotState.Empty);
        if (availableSlot2 != null)
        {
            //availableSlot2.HighLightSlot();
            slots.Add(availableSlot2);
        }
        // up
        CardSlot availableSlot3 = playerSlotContainer.FirstOrDefault(x => x.xPos == slot.xPos && x.yPos == slot.yPos + 1 && x.state == SlotState.Empty);
        if (availableSlot3 != null)
        {
            if (!(availableSlot3.yPos == 2))
            {
                //availableSlot3.HighLightSlot();
                slots.Add(availableSlot3);
            }
        }
        // down
        CardSlot availableSlot4 = playerSlotContainer.FirstOrDefault(x => x.xPos == slot.xPos && x.yPos == slot.yPos - 1 && x.state == SlotState.Empty);
        if (availableSlot4 != null)
        {
            if (!(availableSlot4.yPos == 1))
            {
                //availableSlot4.HighLightSlot();
                slots.Add(availableSlot4);
            }
        }

        return slots;
    }

    public List<CardSlot> ChooseSelfAnyBlank()
    {

        List<CardSlot> slots = new List<CardSlot>();

        foreach (CardSlot slot in playerSlotContainer)
            if (slot.state == SlotState.Empty)
                slots.Add(slot);
        return slots;
    }

    List<HandCard> ChooseHandCard()
    {
        Decks[0].GetListCard.ForEach(x => x.HighlighCard());
        return Decks[0].GetListCard;
    }

    void ChooseEnemyTower()
    {
        lstTowerInBattle.ForEach(t =>
        {
            if (t.pos == 1 && t.towerHealth > 0)
            {
                t.HighLightTower();
            }
        });
    }

    void HighLightPlayerUnit(bool butSelf, Card card = null)
    {
        if (butSelf)
        {
            var allyUnit = GetListPlayerCardInBattle().Where(x => x != card);
            allyUnit.ToList().ForEach(x => x.HighlightUnit());
        }
        else
        {
            GetListPlayerCardInBattle().ForEach(x => x.HighlightUnit());
        }
    }

    void HighLightEnemyUnit()
    {
        GetListEnemyCardInBattle().ForEach(x => x.HighlightUnit());
    }

    List<BoardCard> ChooseAnyUnit(bool butSelf, BoardCard card = null)
    {
        List<BoardCard> cards = new List<BoardCard>();


        if (butSelf)
        {
            var allyUnit = GetListPlayerCardInBattle().Where(x => x != card);
            allyUnit.ToList().ForEach(x => cards.Add(x));
        }
        else
        {
            GetListPlayerCardInBattle().ForEach(x => cards.Add(x));
        }


        return cards;
    }

    List<BoardCard> ChooseTwoEnemyUnit()
    {
        List<BoardCard> cards = new List<BoardCard>();
        GetListEnemyCardInBattle().ForEach(x => cards.Add(x));

        return cards;
    }

    void ChooseAnyAllyUnit()
    {
        HighLightPlayerUnit(false);
        skillState = SkillState.ANY_ALLY_UNIT;
    }

    List<Card> ChooseTwoAllyUnit(Card card)
    {
        List<Card> cards = new List<Card>();

        var allyUnit = GetListPlayerCardInBattle().Where(x => x != card);
        cards.AddRange(allyUnit.ToList());
        //.ForEach(x => x.HighlightUnit());

        //HighLightPlayerUnit(true, card);
        //skillState = SkillState.TWO_ANY_ALLIES;

        return cards;
    }

    void ChooseLane(SkillState state)
    {
        lstLaneInBattle.ForEach(l =>
        {
            l.HighlightLane();
        });
        skillState = state;
    }

    public bool CheckShard(Card card)
    {

        if (card.heroInfo.color == DBHero.COLOR_WHITE)
            return true;

        if (card.heroInfo.speciesId == currentAvailableRegion)
            return true;

        //bool isOk = false;

        //foreach (Card c in GetListPlayerCardInBattle())
        //{

        //    if (card.heroInfo.color == c.heroInfo.color)
        //    {
        //        if (c.countShardAddded >= card.heroInfo.shardRequired && c.countShardAddded >= card.skill.min_shard)
        //            if (card.skill.max_shard == -1)
        //                isOk = true;
        //            else if (c.countShardAddded <= card.skill.max_shard)
        //                isOk = true;
        //    }
        //}
        //return isOk;
        return true;
    }

    bool CheckSkllCondition(Card card, long row =-1 , long col =-1)
    {

        if (card.skill != null)
        {
            foreach (ConditionSkill condition in card.skill.lstConditionSkill)
            {
                switch (condition.type)
                {
                    case ConditionSkill.SELF_IN_FRONT_ROW:
                        {
                            if (row != 0)
                                return false;
                            break;
                        }
                    case ConditionSkill.SELF_IN_BACK_ROW:
                        {
                            if (row != 1)
                                return false;
                            break;
                        }
                    case ConditionSkill.CONDITION_TYPE_SMALLER:
                        {

                            int number = condition.number;
                            int species = condition.species;
                            int pos = condition.pos;
                            int count = 1;
                            if (pos == 0)
                            {
                                foreach (Card c in lstCardInBattle)
                                    if (c.heroInfo.speciesId == species)
                                        count++;
                            }
                            else if (pos == 1)
                            {

                                foreach (Card c in GetListPlayerCardInBattle())
                                    if (c.heroInfo.speciesId == species)
                                        count++;

                            }
                            else if (pos == 2)
                            {
                                foreach (Card c in GetListEnemyCardInBattle())
                                    if (c.heroInfo.speciesId == species)
                                        count++;
                            }

                            if (count >= number)
                                return false;

                            break;
                        }

                    case ConditionSkill.CONDITION_TYPE_BIGGER:
                        {
                            int number = condition.number;
                            int species = condition.species;
                            int pos = condition.pos;
                            int count = 1;

                            if (pos == 0)
                            {
                                foreach (Card c in lstCardInBattle)
                                    if (c.heroInfo.speciesId == species)
                                        count++;
                            }
                            else if (pos == 1)
                            {

                                foreach (Card c in GetListPlayerCardInBattle())
                                    if (c.heroInfo.speciesId == species)
                                        count++;

                            }
                            else if (pos == 2)
                            {
                                foreach (Card c in GetListEnemyCardInBattle())
                                    if (c.heroInfo.speciesId == species)
                                        count++;
                            }
                            if (count < number)
                                return false;

                            break;
                        }
                }
                return true;
            }
        }
        return false;
    }

    int TYPE_WHEN_SUMON = 0;
    int TYPE_WHEN_START_TURN = 1;


    public void TurnOnEffectAttackTower(bool isPlayerTowerBeingAttack, bool isLeft)
    {
        foreach (GameObject g in m_AllyCrackTower)
            g.SetActive(false);
        foreach (GameObject g in m_EnemyCrackTower)
            g.SetActive(false);
        int lane = isLeft ? 0 : 1;
        if (isPlayerTowerBeingAttack)
        {
            m_AllyCrackTower[lane].SetActive(true);
            m_AllyCrackTower[lane].GetComponent<ParticleSystem>().Play();
        }
        else
        {
            m_EnemyCrackTower[lane].SetActive(true);
            m_EnemyCrackTower[lane].GetComponent<ParticleSystem>().Play();
        }
    }
    // Use this for initialization
    void Start()
    {
        battleState = BATTLE_STATE.NORMAL;
        SoundHandler.main.Init("BackgroundMusicBattle");

        ButtonOrbController.instance.onMulliganCard += OnMulliganCard;
        ButtonOrbController.instance.onReadyConfirm += ReadyConfirm;
        ButtonOrbController.instance.onConfirmStartBattle += OnConfirmStartBattle;
        //ButtonOrbController.instance.onActiveSkill += OnConfirmSkill;
        UIManager.instance.onSurrender += OnSurrender;
        isGameStarted = false;
        StartCoroutine(InitPlayer());
#if UNITY_IOS || UNITY_ANDROID
        Application.targetFrameRate = 60;
#endif

        //Database.ParseAll();
        //List<DBHero> lstHero = new List<DBHero>();
        //List<long> lstbtID = new List<long>();
        //for (int i = 4; i < 10; i++)
        //{
        //    //if (id.Contains(i))
        //    //{
        //    DBHero hero = new DBHero
        //    {
        //        id = i
        //    };
        //    //AddNewCard(0, hero, i);
        //    lstHero.Add(hero);
        //    lstbtID.Add(i);
        //    //}
        //}
        //DrawDeckStart(0, lstHero, lstbtID);
    }
    private void OnSurrender()
    {
        isSurrender = true;
        Game.main.socket.GameBattleLeave();
    }

    public void OnEndSkillPhase()
    {
        lstSelectedSkillHandCard.Clear();
        lstSelectedSkillBoardCard.Clear();
        curCardSkill = null;
        onBoardClone = null;
        selectedTower = null;
        selectedCardSlot = null;
        selectedLane = null;
        UpdateSpellTarget(null, null, true);
        cancelSkill.SetActive(false);
        skillState = SkillState.None;
        UpdateSpellTarget(null, null, true);
        onUpdateMana(0, currentMana, ManaState.UseDone, 0);
        chooseTargets.transform.parent.gameObject.SetActive(false);
        isStartFindTarget = false;
        onEndSkillActive?.Invoke();


    }
    private void OnFinishChooseOneTarget()
    {
        lstSelectedSkillHandCard.Clear();
        lstSelectedSkillBoardCard.Clear();
        onBoardClone = null;
        selectedTower = null;
        selectedCardSlot = null;
        selectedLane = null;
        UpdateSpellTarget(null, null, true);
        skillState = SkillState.None;
        UpdateSpellTarget(null, null, true);
        onUpdateMana(0, currentMana, ManaState.UseDone, 0);
        chooseTargets.transform.parent.gameObject.SetActive(false);
        onFinishChooseOneTarget?.Invoke();
    }

    public void UpdateSpellTarget(Transform target, Transform startTransform, bool isDespawn = false)
    {
        if (isDespawn)
        {
            spawnedSpellLine.ForEach(p => PoolManager.Pools["Effect"].Despawn(p));
            spawnedSpellLine.Clear();
        }
        else
        {
            Transform trans = PoolManager.Pools["Effect"].Spawn(spellLine.transform);
            trans.transform.position = new Vector3(0, 0.12f, 0);
            spawnedSpellLine.Add(trans);
            trans.GetComponent<SpellLineController>().AssignTarget(startTransform.position, target);
        }
    }
    private void SkillFailCondition()
    {
        OnEndSkillPhase();
    }

    const float TIME_DELAY_EFFECT_BUFF_HP = 0.2f;
    const float TIME_DELAY_EFFECT_TMP_INCREASE_ATK_AND_HP = 0.2f;
    const float TIME_DELAY_EFFECT_INCREASE_ATK_AND_HP = 0.2f;
    const float TIME_DELAY_EFFECT_MANA_CREATE_SHARD = 0.4f;
    const float TIME_DELAY_EFFECT_MOVE_HERO = 0.5f;
    const float TIME_DELAY_EFFECT_DRAW_CARD = 0.4f;
    const float TIME_DELAY_EFFECT_DEAL_DAME = 0.5f;
    const float TIME_DELAY_EFFECT_INCREASE_SPECIAL_PARAM = 0.2f;
    const float TIME_DELAY_EFFECT_TMP_INCREASE_SPECIAL_PARAM = 0.2f;
    const float TIME_DELAY_EFFECT_READY = 0.2f;
    const float TIME_DELAY_EFFECT_FIGHT = 1f;
    const float TIME_DELAY_EFFECT_SUMMON_VIRTUAL_HERO = 2f;
    const float TIME_DELAY_EFFECT_SUMMON_CARD_IN_HAND = 1f;
    const float TIME_DELAY_EFFECT_USER_MANA_MAX = 0.2f;
    const float TIME_DELAY_EFFECT_LEAVE_CARD_IN_HAND = 0.5f;
    const float TIME_DELAY_EFFECT_TMP_INCREASE_MANA_MAX = 0.2f;
    const float TIME_DELAY_EFFECT_INCREASE_HERO_MANA = 0f;
    const float TIME_DELAY_EFFECT_DEALDAMGE_TO_HEAL = 0.5f;
    const float TIME_DELAY_EFFECT_PLAY_ALL_COLOR_CARD = 0f;
    const float TIME_DELAY_EFFECT_TMP_INCREASE_HERO_MANA = 0f;

    private float GetTimeDelayForSkill(int effectId)
    {
        switch ((int)effectId)
        {
            case (int)DBHeroSkill.EFFECT_BUFF_HP:
                return TIME_DELAY_EFFECT_BUFF_HP;
            case (int)DBHeroSkill.EFFECT_TMP_INCREASE_ATK_AND_HP:
                return TIME_DELAY_EFFECT_TMP_INCREASE_ATK_AND_HP;
            case (int)DBHeroSkill.EFFECT_INCREASE_ATK_AND_HP:
                return TIME_DELAY_EFFECT_INCREASE_ATK_AND_HP;
            case (int)DBHeroSkill.EFFECT_MANA_CREATE_SHARD:
                return TIME_DELAY_EFFECT_MANA_CREATE_SHARD;
            case (int)DBHeroSkill.EFFECT_MOVE_HERO:
                return TIME_DELAY_EFFECT_MOVE_HERO;
            case (int)DBHeroSkill.EFFECT_DRAW_CARD:
                return TIME_DELAY_EFFECT_DRAW_CARD;
            case (int)DBHeroSkill.EFFECT_DEAL_DAME:
                return TIME_DELAY_EFFECT_DEAL_DAME;
            case (int)DBHeroSkill.EFFECT_INCREASE_SPECIAL_PARAM:
                return TIME_DELAY_EFFECT_INCREASE_SPECIAL_PARAM;
            case (int)DBHeroSkill.EFFECT_TMP_INCREASE_SPECIAL_PARAM:
                return TIME_DELAY_EFFECT_TMP_INCREASE_SPECIAL_PARAM;
            case (int)DBHeroSkill.EFFECT_READY:
                return TIME_DELAY_EFFECT_READY;
            case (int)DBHeroSkill.EFFECT_FIGHT:
                return TIME_DELAY_EFFECT_FIGHT;
            case (int)DBHeroSkill.EFFECT_SUMMON_VIRTUAL_HERO:
                return TIME_DELAY_EFFECT_SUMMON_VIRTUAL_HERO;
        }


        return 0f;
    }

    //public void OnDragShard()
    //{
    //    onDragShard?.Invoke();
    //}
    //public void OnEndDragShard()
    //{
    //    onEndDragShard?.Invoke();
    //}
    #region Resume
    private void PlaceCardInBattleOnResume(long isMe, long battleID, long heroID, long frame, long atk, long hp, long hpMax, long mana, long cleave, long pierce, long breaker, long combo, long overrun, long shield, long godSlayer, long fragile, long precide, long row, long col, List<long> lstBuffCards = null)
    {
        switch (isMe)
        {
            case 0:
                var targetSlot = playerSlotContainer.FirstOrDefault(x => x.xPos == row && x.yPos == col);
                if (targetSlot != null)
                {
                    DBHero hero = Database.GetHero(heroID);
                    StartCoroutine(CreateCard(battleID, heroID, frame, atk, hp, mana, hero.type == DBHero.TYPE_GOD ? m_GodCard : m_MinionOnBoardCard, targetSlot.transform, null, targetSlot, CardOwner.Player, 0, (card) =>
                    {
                        card.UpdateHeroMatrix(atk, hp, hpMax, cleave, pierce, breaker, combo, overrun, shield, godSlayer, fragile, precide, lstBuffCards);
                    }));
                }
                break;
            case 1:
                var targetSlotEnemy = enemySlotContainer.FirstOrDefault(x => x.xPos == row && x.yPos == col);
                if (targetSlotEnemy != null)
                {
                    DBHero hero = Database.GetHero(heroID);
                    StartCoroutine(CreateCard(battleID, heroID, frame, atk, hp, mana, hero.type == DBHero.TYPE_GOD ? m_EnemyGodOnBoardCard : m_EnemyMinionOnBoardCard, targetSlotEnemy.transform, null, targetSlotEnemy, CardOwner.Enemy, 0, (card) =>
                    {
                        card.UpdateHeroMatrix(atk, hp, hpMax, cleave, pierce, breaker, combo, overrun, shield, godSlayer, fragile, precide, lstBuffCards);
                    }));
                }
                break;
        }
    }
    #endregion


}


public class DataQueue
{
    public int serviceId { private set; get; }
    public byte[] data { private set; get; }


    public DataQueue(int serviceId, byte[] data)
    {
        this.serviceId = serviceId;
        this.data = data;
    }
}