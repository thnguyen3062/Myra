using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GIKCore.Utilities;
using TMPro;
using GIKCore;
using Spine.Unity;
using GIKCore.Lang;
using GIKCore.Sound;

public enum ManaState
{
    StartTurn,
    Simulate,
    UseDone,
    Update,
    None
}

public class ButtonOrbController : MonoBehaviour
{
    public static ButtonOrbController instance;
    private bool isClosed = false;

    public List<SkeletonAnimation> lstPlayerOrb;
    public List<SkeletonAnimation> lstEnemyOrb;
    [SerializeField] private GameObject playerBonusManaContainer;
    [SerializeField] private GameObject enemyBonusManaContainer;
    // 0 locked no turn, 1 unlocked no turn, 2 activated no turn, 3 locked turn, 4 unlocked turn
    // 5 activated turn, 6 activateing turn
    [SerializeField] private SkeletonAnimation buttonAnimation;
    [SerializeField] private TextMeshPro buttonText;

    // 0 deactive, 1 active
    [SerializeField] private SkeletonAnimation playerTurn;
    [SerializeField] private SkeletonAnimation enemyTurn;
    [SerializeField] private TextMeshPro playerManaCount;
    [SerializeField] private TextMeshPro enemyManaCount;
    [SerializeField] private SpriteRenderer m_CombatOrb;

    [SerializeField] private SkeletonAnimation buttonEffect;

    [Header("Field Effect")]
    [SerializeField] private GameObject m_RoundStartAlly;
    [SerializeField] private GameObject m_RoundStartEnemy;
    [SerializeField] private GameObject m_CombatAlly;
    [SerializeField] private GameObject m_CombatEnemy;
    [SerializeField] private GameObject m_ChangeStartAlly;
    [SerializeField] private GameObject m_ChangeStartEnemy;

    //Scene has Bid
    [SerializeField] private GameObject m_YourTurnAfterBid;
    [SerializeField] private GameObject m_OppTurnAfterBid;
    private GameObject currentFieldState;
    private GameObject currentTurnState;

    // 0 combat, 1 player turn, 2 enemey turn
    private const int COMBAT_STATE = 1;
    private const int PLAYER_STATE = 2;
    private const int ENEMY_STATE = 3;
    private const int ROUND_START_STATE = 4;
    private int lastState = 0;
    private bool isGotShard;
    private long maxMana;

    #region ACTION
    public event ICallback.CallFunc onMulliganCard;
    public event ICallback.CallFunc onReadyConfirm;
    public event ICallback.CallFunc onConfirmStartBattle;
    //public event ICallback.CallFunc onActiveSkill;
    public event ICallback.CallFunc onEndSkillActive;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (GameBattleScene.instance != null)
        {
            GameBattleScene.instance.onGameDealCard += GameDealCard;
            GameBattleScene.instance.onGameStartRound += GameStartRound;
            GameBattleScene.instance.onGameBattleSimulation += GameBattleSimulation;
            GameBattleScene.instance.onSubGameBattleSimulationHasBid += SubGameBattleSimulation;
            GameBattleScene.instance.onGameChooseWay += GameChooseWay;
            GameBattleScene.instance.onGameChooseWayRequest += GameChooseWayRequest;
            GameBattleScene.instance.onUpdateMana += UpdateMana;
            //GameBattleScene.instance.onChangeSkillState += OnConfirmButton;
            GameBattleScene.instance.onGameBattleChangeTurn += onGameBattleChangeTurn;
            GameBattleScene.instance.onResumeTime += OnResume;
            //GameBattleScene.instance.onUpdateShard += UpdateShard;
            //GameBattleScene.instance.onUpdateShardClick += OnUpdateShardClick;
            //GameBattleScene.instance.onEndSkillActive += OnEndSkillActive;
            //dragDropUI.onGetShard += OnGetShard;
            //dragDropUI.onDragShard +=  OnDragShard;
            //dragDropUI.onEndDragShard +=  OnEndDragShard;
            ChangeButtonState(PLAYER_STATE, 0);
        }
    }

    private void OnMouseDown()
    {
        if (GameBattleScene.instance == null)
            return;
        if (!isClosed)
        {
            if (!GameBattleScene.instance.isGameStarted)
            {
                buttonAnimation.state.SetAnimation(0, "button_activate", false).Complete += delegate
                {
                    ChangeButtonState(ENEMY_STATE, 0, () =>
                    {
                        onReadyConfirm?.Invoke();
                    });
                };
            }
            else if (GameBattleScene.instance.IsYourTurn)
            {
                buttonAnimation.state.SetAnimation(0, "button_activate", false).Complete += delegate
                {
                    if (GameBattleScene.instance.skillState != SkillState.None)
                    {
                        //OnConfirmButton(false);
                        //onActiveSkill?.Invoke();
                        return;
                    }
                    else
                    {
                        // end turn
                        onConfirmStartBattle?.Invoke();
                    }
                };
            }
        }
    }

    private void OnEndSkillActive()
    {
        onEndSkillActive?.Invoke();
    }

    private void GameDealCard(List<long> a, List<DBHero> b, List<long> f1, List<long> atk1, List<long> hp1, List<long> m1, List<long> c, List<DBHero> d, List<long> f2, List<long> atk2, List<long> hp2, List<long> m2)
    {
        ChangeButtonState(PLAYER_STATE, 1);
    }

    private void GameBattleSimulation(bool isYourTurn, long round, long turnMana)
    {
        playerTurn.gameObject.SetActive(isYourTurn);
        enemyTurn.gameObject.SetActive(!isYourTurn);
        if (isYourTurn)
        {
            
            ChangeButtonState(PLAYER_STATE, 0);
        }
        else
        {
            ChangeButtonState(ENEMY_STATE, 0);
        }
    }
    private void SubGameBattleSimulation(bool isYourTurn)
    {
        if (currentTurnState != null)
            currentTurnState.SetActive(false);
        if (isYourTurn)
        {

            m_YourTurnAfterBid.SetActive(true);
            currentTurnState = m_YourTurnAfterBid;
            SoundHandler.main.PlaySFX("VoiceYourTurn", "sounds");
        }
        else
        {
            m_OppTurnAfterBid.SetActive(true);
            currentTurnState =m_OppTurnAfterBid;
            SoundHandler.main.PlaySFX("VoiceOpponentTurn", "sounds");
        }
    }    
    private void GameStartRound()
    {
        playerTurn.gameObject.SetActive(false);
        enemyTurn.gameObject.SetActive(false);
        ChangeButtonState(ROUND_START_STATE, 0);
    }

    public void UpdateBattleSword(string userName, string firstPlayerName, GameState state)
    {
        if (currentFieldState != null)
            currentFieldState.SetActive(false);

        switch (state)
        {
            // dau round, neu minh di truoc chay hieu ung chuyen kiem
            // player nhan kiem 1, enemy nhan kiem 2 va nguoc lai
            case GameState.RoundStart:
                {
                    // is your turn
                    if (GameData.main.profile.username.Equals(userName))
                    {
                        StartCoroutine(Delay(() => { SoundHandler.main.PlaySFX("VoiceYourTurn", "sounds"); }, 3f));
                        m_RoundStartAlly.SetActive(true);
                        currentFieldState = m_RoundStartAlly;
                    }
                    else
                    {
                        StartCoroutine(Delay(() => { SoundHandler.main.PlaySFX("VoiceOpponentTurn", "sounds"); }, 3f));
                        m_RoundStartEnemy.SetActive(true);
                        currentFieldState = m_RoundStartEnemy;
                    }

                    break;
                }
            // doi turn, neu luot cua minh, doi trang thai tu kiem 2 toi -> sang kiem 2 sang'
            // enemy tu kiem 1 sang' sang kiem 1 toi va nguoc lai
            case GameState.ChangeTurn:
                {
                    // is your turn
                    if (GameData.main.profile.username.Equals(userName))
                    {
                        SoundHandler.main.PlaySFX("VoiceYourTurn", "sounds");
                        m_ChangeStartAlly.SetActive(true);
                        currentFieldState = m_ChangeStartAlly;
                    }
                    else
                    {
                        SoundHandler.main.PlaySFX("VoiceOpponentTurn", "sounds");
                        m_ChangeStartEnemy.SetActive(true);
                        currentFieldState = m_ChangeStartEnemy;
                    }

                    break;
                }
            // neu minh di truoc, chay hieu ung combat enemy
            // chuyen tu kiem 2 sang sang combat
            case GameState.Combat:
                {
                    SoundHandler.main.PlaySFX("VoiceCombat", "sounds");
                    if (GameData.main.profile.username.Equals(firstPlayerName))
                    {
                        m_CombatAlly.SetActive(true);
                        currentFieldState = m_CombatAlly;
                    }
                    else
                    {
                        m_CombatEnemy.SetActive(true);
                        currentFieldState = m_CombatEnemy;
                    }
                    break;
                }
        }
    }

    private void OnResume(bool p1, bool p2, long time, long timeMax)
    {
        if(!p1&& !p2)
        {
            playerTurn.gameObject.SetActive(false);
            enemyTurn.gameObject.SetActive(false);
            ChangeButtonState(ROUND_START_STATE, 0);
        } 
        else if (p1)
        {
            ChangeButtonState(PLAYER_STATE, 0);
        }
        else
        {
            ChangeButtonState(ENEMY_STATE, 0);
        }
    }

    private void GameChooseWay(bool isPlayer)
    {
        ChangeButtonState(COMBAT_STATE, 0);
    }

    private void GameChooseWayRequest()
    {
        ChangeButtonState(COMBAT_STATE, 1);
    }

    private void OnConfirmButton(bool isConfirm)
    {
        if (isConfirm)
            ChangeButtonState(PLAYER_STATE, 1);
        else
            ChangeButtonState(PLAYER_STATE, 0);
    }

    private void onGameBattleChangeTurn(long index)
    {
        if (index == -1)
        {
            GameChooseWayRequest();
        }
    }
    private void UpdateMana(int index, long mana, ManaState state, long usedMana)
    {
        Debug.Log("update mana:" + index +"/"+ "mana: "+mana +"/"+ state + "/"+ usedMana);
        if (index == 0)
        {
            playerManaCount.text = mana.ToString();
        }
        else
            enemyManaCount.text = mana.ToString();

        if (state == ManaState.StartTurn)
            maxMana = mana;

        playerBonusManaContainer.SetActive(index == 0 && mana > 6);
        enemyBonusManaContainer.SetActive(index == 1 && mana > 6);

        // 0 locked no turn, 1 unlocked no turn, 2 activated no turn, 3 locked turn, 4 unlocked turn
        // 5 activated turn, 6 activateing turn
        int manaIndex = 0;
        switch (index)
        {
            case 0:
                if (state == ManaState.StartTurn)
                {
                    lstPlayerOrb.ForEach(x =>
                    {
                        if (manaIndex < maxMana)
                        {
                            if (manaIndex < mana)
                            {
                                x.state.SetAnimation(0, "mana_unlock", false).Complete += delegate
                                {
                                    x.state.SetAnimation(0, "mana_open_idle", true);
                                };
                            }
                            else
                            {
                                x.state.SetAnimation(0, "mana_close", false);
                            }
                            manaIndex++;
                        }
                    });
                    GameBattleScene.instance.currentMana = mana;
                }
                if (state == ManaState.UseDone)
                {
                    lstPlayerOrb.ForEach(x =>
                    {
                        if (manaIndex >= mana && manaIndex < maxMana)
                        {
                            x.state.SetAnimation(0, "mana_activating", false).Complete += delegate
                            {
                                x.state.SetAnimation(0, "mana_open", false);
                            };
                        }
                        manaIndex++;
                    });
                    GameBattleScene.instance.currentMana = mana;
                }
                break;
            case 1:
                if (state == ManaState.StartTurn)
                {
                    lstEnemyOrb.ForEach(x =>
                    {
                        if (manaIndex < maxMana)
                        {
                            if (manaIndex < mana)
                            {
                                x.state.SetAnimation(0, "mana_unlock", false).Complete += delegate
                                {
                                    x.state.SetAnimation(0, "mana_open_idle", true);
                                };
                            }
                            else
                            {
                                x.state.SetAnimation(0, "mana_close", false);
                            }
                            manaIndex++;
                        }
                    });
                }
                if (state == ManaState.UseDone)
                {
                    lstEnemyOrb.ForEach(x =>
                    {
                        if (manaIndex >= mana && manaIndex < maxMana)
                        {
                            x.state.SetAnimation(0, "mana_activating", false).Complete += delegate
                            {
                                x.state.SetAnimation(0, "mana_open", false);
                            };
                        }
                        manaIndex++;
                    });
                }
                break;
        }
    }


    private void ChangeButtonState(int state, int value, ICallback.CallFunc complete = null)
    {
        if (state != PLAYER_STATE)
            isClosed = true;
        else
            isClosed = false;
        if (lastState != state)
        {
            lastState = state;
            //buttonEffect.state.Event += delegate
            //{
            switch (state)
            {
                case PLAYER_STATE:
                    buttonAnimation.state.SetAnimation(0, "button_idle", false);

                    buttonText.text = value == 0 ? LangHandler.Get("118", "END TURN") : LangHandler.Get("119", "OK");
                    m_CombatOrb.sortingOrder = 0;
                    break;
                case ENEMY_STATE:
                    buttonAnimation.state.SetAnimation(0, "enemy_turn_button_idle", false);
                    buttonText.text = LangHandler.Get("120", "ENEMY TURN"); 
                    m_CombatOrb.sortingOrder = 0;
                    break;
                case ROUND_START_STATE:
                    buttonAnimation.state.SetAnimation(0, "enemy_turn_button_idle", false);
                    buttonText.text = LangHandler.Get("120", "WAITING");
                    m_CombatOrb.sortingOrder = 0;
                    break;
                case COMBAT_STATE:
                    buttonAnimation.state.SetAnimation(0, "combat_button_idle", false);
                    buttonText.text = "";
                    m_CombatOrb.sortingOrder = 4;
                    break;
            }
            complete?.Invoke();
            //};
            buttonEffect.state.SetAnimation(0, "tranform_button_effect", false);
        }
    }
    IEnumerator Delay(ICallback.CallFunc cb, float delayTime)
    {
        if (cb != null)
            cb?.Invoke();
        yield return new WaitForSeconds(delayTime);
    }
}
