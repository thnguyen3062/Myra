using System.Collections;
using System.Collections.Generic;
using TMPro;
using UIEngine.Extensions;
using UnityEngine;
using GIKCore;
using UnityEngine.UI;
using System.Linq;
using GIKCore.Net;
using GIKCore.UI;
using GIKCore.Utilities;
using GIKCore.Lang;
using PathologicalGames;
using DG.Tweening;

public class GodCardUI : GameListener
{
    //field
    [SerializeField] private Image m_GodIcon;
    [SerializeField] private Image m_GodColor;
    [SerializeField] private UIDragDrop m_UIDragDrop;
    [SerializeField] private TextMeshProUGUI m_Count;
    [SerializeField] private GameObject defeatState, usableState, noManaState;
    [SerializeField] private GameObject usableEffNoGodSummon, summonGodTxt;
    //[SerializeField] private GameObject outlineTouchGO;

    //values
    [HideInInspector] public Transform newCardClone, newIconClone;
    [HideInInspector] public Cursor cursor;
    [SerializeField] private List<Image> lstShardAdded;
    public GameObject cardCloneGod, iconCloneGod;
    private Sprite godSprite;
    public Vector3 initPosisiotn;
    bool isdrag;
    bool isTouch;
    bool isHold;
    float countTime;
    DBHero hero;
    public ICallback.CallFunc onDropCard;
    public LayerMask layerMask;
    private CardSlot currentSelectedCardSlot;
    public long battleId;
    public long frameC;

    CardOwner cardOwner;
    bool isdead, isSummon = false;
    bool canDrag = true;
    public bool isClone = false;

    private long maxHealth;
    private Color originColor = new Color(1, 1, 1);
    private Color pressColor = new Color(.5f, .5f, .5f);
    private Vector3 originScale = new Vector3(0.45f, .45f, .45f);
    private Vector3 pressScale = new Vector3(0.65f, .65f, .65f);
    //methods
    public long CurrentCardID
    {
        get;
        private set;
    }

    public long CurrentCardBattleID
    {
        get;
        private set;
    }

    public UIDragDrop Event
    {
        get
        {
            return m_UIDragDrop;
        }
    }

    public bool allowDrag
    {
        get
        {
            return m_UIDragDrop.allowDrag;
        }
        set
        {
            m_UIDragDrop.allowDrag = value;
        }
    }

    private void OnEnable()
    {
        lstShardAdded.ForEach(x => x.gameObject.SetActive(false));
    }
    public void InitData(long battleID, long id, long frame,long atk, long hp, long mana, string godName, CardOwner owner)
    {
        hero = Database.GetHero(id).Clone();
        hero.atk = atk;
        hero.hp = hp;  
        hero.mana = mana;
        maxHealth = hp;
        hero.lstUnlockSkillCard.Clear();
        CurrentCardID = id;
        battleId = battleID;
        frameC = frame;
        CurrentCardBattleID = battleID;
        gameObject.name = godName + battleId;
        cardOwner = owner;
        m_Count.text = hero.mana.ToString();
        godSprite = CardData.Instance.GetGodIconSprite(hero.heroNumber.ToString());
        usableState.SetActive(false);    
        usableEffNoGodSummon.SetActive(false);
        summonGodTxt.SetActive(false);
        if (godSprite != null)
        {
            //highlightSprite = CardData.Instance.GetGodIconSprite(id + "_Glow");
            m_GodIcon.sprite = godSprite;
            //m_GodIcon.material.SetTexture("_BaseMap",godTexture) ;
            //m_GodIcon.SetNativeSize();
            //m_GodIcon.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            //m_GodColor.sprite = CardData.Instance.GetGodIconSprite("God_" + Database.GetHero(id).color);
        }
        initPosisiotn = transform.position;
        if (GameData.main.lstGodDead.Count > 0)
        {
            foreach (long battle in GameData.main.lstGodDead)
            {
                if (battle == battleId)
                {
                    ChangeState(0);
                    transform.SetAsFirstSibling();
                }
            }
        }
    }

    public override bool ProcessNetData(int id, object data)
    {
        if (base.ProcessNetData(id, data)) return true;
        switch (id)
        {
            case NetData.CARD_CHANGE_STAGE:
                {
                    CardStageProps csp = (CardStageProps)data;
                    if (csp.id == battleId)
                    {
                        if (csp.stage == 0)
                            ChangeState(0);
                        else if (csp.stage == 1)
                            ChangeState(1);
                        canDrag = false;
                        CheckCondition();
                    }
                    else
                    {
                        if(csp.heroId == this.hero.id)
                        {
                            if (csp.stage == 0)
                                canDrag = true;
                            else if (csp.stage == 1)
                            { 
                                canDrag = false;
                            }
                        }    
                        CheckCondition();
                    }
                    break;
                }
            case NetData.CARD_UPDATE_MATRIX:
                {
                    CardUpdateHeroMatrix cuhm = (CardUpdateHeroMatrix)data;
                    if (cuhm.heroId == hero.id && cuhm.own == cardOwner)
                    {
                        hero = cuhm.heroInfo;
                    }
                    break;
                }

        }
        return false;
    }
    private void GodShardUpdate(long shard)
    {
        //for (int i = 0; i < lstShardAdded.Count; i++)
        //{
        //    if (i < shard)
        //    {
        //        lstShardAdded[i].gameObject.SetActive(true);
        //        lstShardAdded[i].sprite = CardData.Instance.GetShardSprite(hero.color); //CardColor.Instance.cardColorInfo[(int)heroInfo.color].shardColorSprite;
        //    }
        //}
    }
    private void Start()
    {
        // GameBattleScene.instance.onGameBattleSimulation += (x, y, z) => { allowDrag = x; };
        if (GameBattleScene.instance != null)
        {
            GameBattleScene.instance.onGameDealCard += delegate { allowDrag = true; };
            GameBattleScene.instance.onGameStartupEnd += delegate { MoveFail(); };
            GameBattleScene.instance.onUpdateMana += OnUpdateMana;
            GameBattleScene.instance.onGameBattleSimulation += GameBattleSimulation;
            GameBattleScene.instance.onGameBattleChangeTurn += EndTurn;
        }
        Event.SetOnPointerDownCallback((go) => { OnPointerDown(); });
        Event.SetOnBeginDragCallback((go) => { OnBeginDragCard(); });
        Event.SetOnEndDragCallback((go) => { OnDropCard(go); });
        Event.SetOnPointerClickCallback((go) => { OnClickCard(); });
        Event.SetOnDragCallback((go) => { OnDragCard(); });
        //Event.SetOnPointerEnterCallback((go) => { OnPointerEnter(); });
        Event.SetOnPointerExitCallback((go) => { OnPointerExit(); });
    }
    public void GameBattleSimulation(bool isPlayer, long roundCount,long turnmana)
    {
        if (GameBattleScene.instance == null)
        {
            return;
        }
        if (isClone)
            return;
        if (cardOwner == CardOwner.Enemy)
            return;
        if (isPlayer)
        {
            CheckCondition();
            if(canDrag)
                allowDrag = true;
        }
        else
        {
            allowDrag = false;
            if (usableState != null && usableState.activeInHierarchy)
            {
                usableState.SetActive(false);
                usableEffNoGodSummon.SetActive(false);
                summonGodTxt.SetActive(false);

            }    
        }
    }
    private void EndTurn(long turn)
    {
        //allowDrag = false;
        if (isClone)
            return;
        MoveFail();
        CheckCondition();
        //ShowOutlineTouch(false);
    }
    private void OnUpdateMana(int index, long mana, ManaState state, long usedMana)
    {
        if (cardOwner == CardOwner.Enemy)
            return;
        if (isClone)
            return;
        if (index == 0)
        {
            if (this.gameObject.activeInHierarchy)
                CheckCondition();
        }
    }
    private void OnUpdateShard(int index, long shard)
    {
        if (cardOwner == CardOwner.Enemy)
            return;
        if (index == 0)
        {
            //hien thi shard tren god ui    

        }
    }
    private void OnPointerDown()
    {
        isTouch = true;
        //ShowOutlineTouch(true);
        transform.DOScale(pressScale, 0.01f).SetEase(Ease.OutBounce);
        //GetComponent<CanvasGroup>().DOFade(.5f, 0.1f);
        m_GodIcon.DOBlendableColor(pressColor, 0.01f);
    }
    private void OnClickCard()
    {
        //ShowOutlineTouch(false);
        transform.DOScale(originScale, 0.01f).SetEase(Ease.OutBounce);
        m_GodIcon.DOBlendableColor(originColor, 0.01f);
        //GetComponent<CanvasGroup>().DOFade(1f, 0.1f);
        UIManager.instance.ClosePreviewHandCard();
        isTouch = false;
        isHold = false;
        countTime = 0;

    }

    private void OnPointerExit()
    {
        if (GameBattleScene.instance == null)
        {
            return;
        }
        if(!GameBattleScene.instance.IsYourTurn)
        {
            transform.DOScale(originScale, 0.01f).SetEase(Ease.OutBounce);
            m_GodIcon.DOBlendableColor(originColor, 0.01f);
            //GetComponent<CanvasGroup>().DOFade(1f, 0.1f);
            UIManager.instance.ClosePreviewHandCard();
        }    
        m_GodIcon.sprite = godSprite;
    }

    private void OnDragCard()
    {
        if (GameBattleScene.instance == null)
        {
            if (TutorialController.instance.m_TutorialID == 1)
            {
            }
            return;
        }

        if (GameBattleScene.instance.IsYourTurn || !GameBattleScene.instance.isGameStarted)
        {
            Vector3 screenPosition = Input.mousePosition;
            screenPosition.z = Vector3.Dot(transform.position - Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider != null)
                {
                    //if(hit.collider.gameObject != gameObject&& cursor==null)
                    //{
                    //    //tao card clone
                    //    CreateCloneOnDragging();
                    //}    
                    if (hit.collider.GetComponent<CardSlot>() != null)
                    {
                        if (hit.collider.GetComponent<CardSlot>().type != SlotType.Enemy)
                        {
                            CardSlot slot = hit.collider.GetComponent<CardSlot>();
                            if (currentSelectedCardSlot == null)
                            {
                                currentSelectedCardSlot = slot;
                                currentSelectedCardSlot.HighlightSelectedSlot();
                            }
                            else
                            {
                                if (slot != currentSelectedCardSlot)
                                {
                                    currentSelectedCardSlot.UnHighlightSelectedSlot();
                                    currentSelectedCardSlot = slot;
                                    currentSelectedCardSlot.HighlightSelectedSlot();
                                }
                            }
                        }
                    }
                }
            }
            if (isdrag)
            {
                //isdrag = false;
                foreach (CardSlot slot in GameBattleScene.instance.ChooseSelfAnyBlank())
                {
                    slot.HighLightSlot();
                }

            }
        }
    }

    private void OnBeginDragCard()
    {
        if (GameBattleScene.instance == null)
        {
            return;
        }
        UIManager.instance.ClosePreviewHandCard();
        if (!GameBattleScene.instance.IsYourTurn)
            return;

        if (GameBattleScene.instance.IsYourTurn || !GameBattleScene.instance.isGameStarted)
        {
            isdrag = true;

            CreateCloneOnDragging();
        }
        else
        {
            Toast.Show(LangHandler.Get("tut-5", "YOU COULD NOT PLAY CARD IN OPPONENT'S TURN"));
        }
    }

    private void OnDropCard(GameObject go)
    {
        //ShowOutlineTouch(false);
        transform.DOScale(originScale, 0.01f).SetEase(Ease.OutBounce);
        m_GodIcon.DOBlendableColor(originColor, 0.01f);
        //GetComponent<CanvasGroup>().DOFade(1f, 0.1f);
        UIManager.instance.ClosePreviewHandCard();
        isdrag = false;
        countTime = 0;
        isHold = false;
        isTouch = false;
        if (GameBattleScene.instance == null)
        {
            if (TutorialController.instance.m_TutorialID == 1)
            {
                RaycastHit hit;
                if (TutorialController.instance.index == 1 || TutorialController.instance.index == 61)
                {
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
                    {
                        if (hit.collider != null)
                        {
                            if (hit.collider.GetComponent<CardSlot>() != null)
                            {
                                BattleSceneTutorial.instance.EndDragCard(hit.collider.GetComponent<CardSlot>());
                            }
                        }
                    }
                }
            }

            Event.rectTransform = GetComponent<RectTransform>();
            OnPointerExit();
            MoveFail();
            return;
        }
        GameBattleScene.instance.currentGodCardUI = gameObject;
        if (!GameBattleScene.instance.isGameStarted)
        {
            // o day se destroy currentGod CardUI -> doi thanh doi state và ??i th? t? c?a t?ng god .
            GameBattleScene.instance.SummonGodInReadyPhase(CurrentCardBattleID, CurrentCardID);
        }
        else
        {
            if (GameBattleScene.instance.IsYourTurn)
            {
                // o day se destroy currentGod CardUI -> doi thanh doi state và ??i th? t? c?a t?ng god .
                GameBattleScene.instance.SummonGodInBattlePhase(CurrentCardBattleID, CurrentCardID);
            }
        }
        Event.rectTransform = GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
        // UIManager.instance.OnCloseGodContainer();
        onDropCard?.Invoke();
        OnPointerExit();
        foreach (CardSlot slot in GameBattleScene.instance.playerSlotContainer)
            slot.UnHighLightSlot();
        MoveFail();
    }
    public void CheckCondition()
    {
        if (GameBattleScene.instance == null)
        {

            return;
        }
        if (cardOwner == CardOwner.Enemy)
            return;
        if (isClone)
            return;
        if (GameBattleScene.instance.currentMana >= hero.mana)
            ChangeState(2);
        else
            ChangeState(3);

        var lstGod = GameBattleScene.instance.lstCardInBattle.Where(w => w.cardOwner == CardOwner.Player && w.heroID == CurrentCardID).ToList();
        if (lstGod.Count == 0 && GameBattleScene.instance.currentMana >= hero.mana && !isdead)
        {
            //hightligt , can summon 
            usableState.SetActive(true);
            var lstGodPlayer = GameBattleScene.instance.lstCardInBattle.Where(w => w.cardOwner == CardOwner.Player && w.heroInfo.type == DBHero.TYPE_GOD).ToList();
            if(lstGodPlayer.Count == 0)
            {
                usableEffNoGodSummon.SetActive(true);
                var grpGodByheroID = GameBattleScene.instance.lstGodPlayer.Where(w => w.id == this.CurrentCardID).ToList();
                if (transform.GetSiblingIndex()== grpGodByheroID.Count-1)
                    summonGodTxt.SetActive(true);
            }    
            //return true;
        }
        else if (!GameBattleScene.instance.isGameStarted)
        {
            var lstGodFirst = GameBattleScene.instance.lstCardInBattle.Where(w => w.cardOwner == CardOwner.Player && w.heroInfo.type == DBHero.TYPE_GOD).ToList();
            if (lstGodFirst.Count > 0)
            {
                usableState.SetActive(false);
                usableEffNoGodSummon.SetActive(false);
                summonGodTxt.SetActive(false);
            }    
            else
            {
                usableState.SetActive(true);
                usableEffNoGodSummon.SetActive(true);

                var grpGodByheroID = GameBattleScene.instance.lstGodPlayer.Where(w => w.id == this.CurrentCardID).ToList();
                if (transform.GetSiblingIndex() == grpGodByheroID.Count-1)
                    summonGodTxt.SetActive(true);
            }    
        }
        else
        {
            // lstgod>0 -> o tren san
            //lstgod=0 -> k o trem san

            usableState.SetActive(false);
            usableEffNoGodSummon.SetActive(false);
            summonGodTxt.SetActive(false);
        }


    }
    public void ShowOutlineTouch(bool isShow)
    {
        //if (isShow)
        //    outlineTouchGO.SetActive(true);
        //else
        //    outlineTouchGO.SetActive(false);
    }
    public void ChangeState(int index)
    { //chet 0
      //chua chet 1
      //co the dang tren san ho?c ch?a summon
      //du mana 2
      //khong du mana 3
      // o tren san -> set card tren san v?i godui. n?u có ch?t thì callback

        switch (index)
        {
            case 0:
                {
                    //chet
                    defeatState.SetActive(true);
                    isdead = true;
                    break;
                }
            case 1:
                {
                    //chua chet??
                    //summon??
                    isSummon = true;
                    hero.hp = maxHealth;
                    defeatState.SetActive(false);
                    break;
                }
            case 2:
                {
                    // ?? mana
                    if (noManaState.activeInHierarchy)
                        noManaState.SetActive(false);
                    break;
                }
            case 3:
                {
                    //khong ?? mana
                    if (!noManaState.activeInHierarchy)
                        noManaState.SetActive(true);
                    break;
                }
        }


    }
    public void CreateCloneOnDragging()
    {
        Transform trans = PoolManager.Pools["Card"].Spawn(GameBattleScene.instance.cursorObject);
        cursor = trans.GetComponent<Cursor>();

        if (newCardClone == null && newIconClone == null)
        {
            newCardClone = PoolManager.Pools["Card"].Spawn(cardCloneGod);
            newCardClone.GetComponent<CardOnBoardClone>().InitData(hero, CurrentCardBattleID, frameC);
            newCardClone.gameObject.SetActive(false);
            newIconClone = PoolManager.Pools["Card"].Spawn(iconCloneGod, this.transform.position, Quaternion.identity, transform.parent);
            newIconClone.GetComponent<GodCardUI>().InitData(CurrentCardBattleID, CurrentCardID, frameC,hero.atk,hero.hp, hero.mana, hero.name, CardOwner.Player);
            newIconClone.GetComponent<GodCardUI>().isClone = true;
            Event.rectTransform = newIconClone.gameObject.GetComponent<RectTransform>();
            newIconClone.gameObject.SetActive(true);
        }
        Ray ray = Camera.main.ScreenPointToRay(transform.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 hitPosition = hit.point;
            cursor.gameObject.SetActive(true);
            cursor.InitCursor(hitPosition + new Vector3(-0.2f, 0, 0.2f), null, newCardClone, newIconClone);
        }


    }
    public void MoveFail()
    {
        if (cursor != null)
        {
            if (cursor.gameObject.activeSelf)
            {
                PoolManager.Pools["Card"].Despawn(cursor.transform);
                cursor = null;
                if (newCardClone != null)
                {
                    PoolManager.Pools["Card"].Despawn(newCardClone);
                    newCardClone = null;
                }
                if (newIconClone != null)
                {
                    PoolManager.Pools["Card"].Despawn(newIconClone);
                    newIconClone = null;
                }
            }
        }

    }
    public void UpdateHeroInfo(DBHero heroUpdate)
    {
        hero = heroUpdate;
    }    
    private void Update()
    {
        if (!isTouch)
            return;
        countTime += Time.deltaTime;
        if (countTime > 0.3f && !isdrag)
        {
            if (isHold == false)
            {
                isHold = true;
                if (GameBattleScene.instance != null)
                {
                    UIManager.instance.ShowPreviewHandCard(null, this.hero, this.frameC);
                    Debug.Log(cardOwner.ToString());
                }   
            }

        }

    }
}
