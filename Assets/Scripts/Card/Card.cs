using DG.Tweening;
using TMPro;
using UnityEngine;
using GIKCore.Utilities;
using System.Collections;
using UnityEngine.VFX;
using PathologicalGames;
using System.Linq;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine.UI;
using GIKCore.Sound;
using Spine;
using UnityEngine.EventSystems;
using GIKCore.UI;
using UnityEngine.InputSystem;
using GIKCore.Net;

public class Card : MonoBehaviour
{
    public long battleID;
    public long heroID;
    public long frameC; 

    [HideInInspector] public long coutUsedSkill = 0;
    [HideInInspector] public long countDoActiveSkill = 0;
    //public long skillID;
    //public long effectType;
    public DBHeroSkill skill;
    [HideInInspector] public List<DBHeroSkill> lstSkill = new List<DBHeroSkill>();
    public DBHero heroInfo;
    public DBHero heroInfoTmp; 
    [HideInInspector] public bool canSelect = false;
    [HideInInspector] public CardOwner cardOwner;
    [HideInInspector] protected Vector3 initPosition;
    [HideInInspector] protected Quaternion initRotation;
    [HideInInspector] public bool isDragging;
    [HideInInspector] protected Cursor cursor;

    public LayerMask layerMask;

    [HideInInspector] public float currentClickTime, currentTime = 0;
    [HideInInspector] public bool isTouch;
    [HideInInspector] public CardSlot currentSelectedCardSlot;
    [HideInInspector] public Transform newCardClone;
    [Header("Card Clone")]
    public GameObject cardCloneMinion, cardCloneSpell, cardCloneGod;

    /// <summary>
    /// Is pointer over?
    /// </summary>
    [HideInInspector] public bool isInteracted;
    [HideInInspector] public bool isSelected;
    [HideInInspector] public bool isMoving;
   
    public bool isHoldM = false;
    #region Unity Methods

    public bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    void OnDisable()
    {
        heroID = -1;
        battleID = -1;
    }
    #endregion

    #region Action Methods

    public virtual void OnEndRound(long index)
    {
        if (!isDragging)
            return;
    }

    public virtual void OnGameConfirmStartBattle()
    {
        isDragging = false;
        countDoActiveSkill = 0;
        if (cursor != null)
        {
            cursor.gameObject.SetActive(true);
            PoolManager.Pools["Card"].Despawn(cursor.transform);
            cursor = null;
            if (newCardClone != null)
            {
                PoolManager.Pools["Card"].Despawn(newCardClone);
                newCardClone.GetComponent<CardOnBoardClone>().cloneSlot = null;
                newCardClone = null;
            }

        }
    }
    #endregion

    public virtual void SetCardData(long battleID, long id,long frame,CardOwner owner, long atk =-1, long hp=-1, long mana=-1)
    {
        this.battleID = battleID;
        heroID = id;
        this.frameC = frame;
        cardOwner = owner;
        heroInfo = Database.GetHero(id);
        heroInfoTmp = heroInfo.Clone();
        //UpdateHeroMatrix(heroInfo.atk, heroInfo.hp, -1, heroInfo.cleave, heroInfo.pierce, heroInfo.breaker, heroInfo.combo, heroInfo.overrun, -1, godSlayerValue, 0);

        heroInfo.lstHeroSkill.ForEach(x =>
        {
            lstSkill.Add(x);
        });
    }

    public void SetSkill(DBHeroSkill skill)
    {
        this.skill = skill;
    }

    public void SetSkillReady(DBHeroSkill skill)
    {
        this.skill = skill;
    }

    public void OnActiveSkill(DBHeroSkill cardSkill)
    {
        skill = cardSkill;
        UIManager.instance.godInfoObject.gameObject.SetActive(false);
        if (cardOwner == CardOwner.Enemy)
            return;

        if (GameBattleScene.instance.IsYourTurn && GameBattleScene.instance.isGameStarted)
        {
            GameBattleScene.instance.DoActiveSkill(this);
           
        }
    }

    /// <summary>
    /// User is interesting on this card.
    /// </summary>
    /// <param name="value"></param>
    public void Interested(bool value, bool animateColor = true)
    {
        isInteracted = value;
    }

    public void MoveBack(float time = 0.2f, ICallback.CallFunc callback = null)
    {
        transform.GetChild(0).DOScale(Vector3.one, time);
        MoveTo(initPosition, time, () =>
        {
            callback?.Invoke();
        });
        transform.rotation = initRotation;

    }

    public void MoveTo(Vector3 to, float duration, ICallback.CallFunc complete = null)
    {
        isMoving = true;
        transform.DOKill();
        transform.DOMove(to, duration).OnComplete(() =>
        {
            complete?.Invoke();
            isMoving = false;
        });
    }

    #region SKILL
    public virtual void HighlightUnit()
    {
        canSelect = true;
    }

    public virtual void UnHighlightUnit()
    {
        canSelect = false;
    }

    public virtual void OnEndSkillActive()
    {

    }
    #endregion

    #region Movement
    public virtual void UpdatePosition()
    {

    }
    public void UpdateHeroInfo(long atk = -1, long hp = -1, long cleave = -1, long pierce = -1, long breaker = -1, long combo = -1, long overrun = -1, long godSlayer = -1,  long fragile = -1, long precide = -1, long manaCost = -1,List<long> lstbuffSkillCards = null)
    {
        if (atk != -1)
            heroInfoTmp.atk = atk;
        if (hp != -1)
            heroInfoTmp.hp = hp;
        if (cleave != -1)
            heroInfoTmp.cleave = cleave;
        if (pierce != -1)
            heroInfoTmp.pierce = pierce;
        if (breaker != -1)
            heroInfoTmp.breaker = breaker;
        if (combo != -1)
            heroInfoTmp.combo = combo;
        if (overrun != -1)
            heroInfoTmp.overrun = overrun;
        if (fragile != -1)
            heroInfoTmp.isFragile = fragile == 1;
        if (godSlayer != -1)
            heroInfoTmp.godSlayer = godSlayer;
        if(manaCost != -1 )
            heroInfoTmp.mana = manaCost;
        if (lstbuffSkillCards != null)
        {
            this.heroInfoTmp.lstUnlockSkillCard.Clear();
            List<DBHero> list = new List<DBHero>();
                foreach (long cID in lstbuffSkillCards)
                {
                    DBHero buffHero = Database.GetHero(cID);
                    list.Add(buffHero);
                }
            heroInfoTmp.lstUnlockSkillCard = list;
            HandleNetData.QueueNetData(NetData.CARD_UPDATE_MATRIX, new CardUpdateHeroMatrix() { battleId = this.battleID, heroId = this.heroID, own = CardOwner.Player, heroInfo = heroInfoTmp }); ; ;
        }
    }
#if UNITY_STANDALONE || UNITY_EDITOR
    public virtual void OnMouseDown()
    {
        if (IsPointerOverUIObject())
            return;
        if(GameBattleScene.instance!=null)
        {
            if (GameBattleScene.instance.skillState == SkillState.None)
                currentClickTime = Time.time;
        }    
    }
   
    public virtual void OnMouseDrag()
    {
        if(GameBattleScene.instance!= null)
        {
            if (cardOwner == CardOwner.Enemy)
                return;
            if (GameBattleScene.instance.battleState == BATTLE_STATE.WAIT_COMFIRM)
                return;
            if (!isTouch)
                return;
            if (Time.time - currentClickTime < 0.3f)
                return;
            if (isDragging)
                return;
            if (GameBattleScene.instance.skillState != SkillState.None)
                return;
        }   
        //isDragging = true;
        //isSelected = true;
        //Transform trans = PoolManager.Pools["Card"].Spawn(GameBattleScene.instance.cursorObject);
        //cursor = trans.GetComponent<Cursor>();
        //if (newCardClone == null)
        //{
        //    // spawn for both spell, minion, god
        //    if (heroInfo.type == DBHero.TYPE_TROOPER_MAGIC)
        //        newCardClone = PoolManager.Pools["Card"].Spawn(cardCloneSpell.transform);
        //    else if (heroInfo.type == DBHero.TYPE_TROOPER_NORMAL)
        //        newCardClone = PoolManager.Pools["Card"].Spawn(cardCloneMinion);
        //    else
        //        newCardClone = PoolManager.Pools["Card"].Spawn(cardCloneGod);
        //    newCardClone.GetComponent<CardOnBoardClone>().InitData(heroID);
        //    newCardClone.gameObject.SetActive(false);
        //}
        //cursor.gameObject.SetActive(true);
        //cursor.InitCursor(transform.position, this, newCardClone);
    }


    public virtual void OnMouseUp()
    {
        if (Time.time - currentClickTime <= 0.3 && GameBattleScene.instance.skillState == SkillState.None)
        {
            return;
        }
        if (cardOwner == CardOwner.Enemy)
            return;
        if (!isTouch)
            return;
        if (!isDragging)
            return;
        isDragging = false;
        isSelected = false;
        if (currentSelectedCardSlot != null)
        {
            currentSelectedCardSlot.UnHighlightSelectedSlot();
            currentSelectedCardSlot = null;
        }
        if (cursor != null)
        {
            PoolManager.Pools["Card"].Despawn(cursor.transform);
            cursor = null;
        }
    }

    public virtual void OnMouseOver()
    {
        if (cardOwner != CardOwner.Player)
            return;
        if (IsPointerOverUIObject())
            return;
        if (!isInteracted)
            return;
        if (isDragging)
            return;
        if (isMoving)
            return;
    }
    

    public virtual void OnMouseExit()
    {
        if (cardOwner != CardOwner.Player)
            return;
        if (!isInteracted)
            return;
        if (isDragging)
            return;
    }
#else
    public virtual void OnTouchDown()
    {
        if (IsPointerOverUIObject())
            return;
        if(GameBattleScene.instance!= null)
            if (GameBattleScene.instance.skillState == SkillState.None)
                currentClickTime = Time.time;
    }
    public virtual void OnTouchHold()
    {
        if (cardOwner != CardOwner.Player)
            return;
        if (isInteracted)
            return;
        isHoldM = true;
    }
    public virtual void OnTouchEnd()
    {
        //if (Time.time - currentClickTime <= 0.3 && GameBattleScene.instance.skillState == SkillState.None)
        //{
        //    OnClickInfo();
        //    return;
        //}
        if (cardOwner == CardOwner.Enemy)
            return;
        if (!isTouch)
            return;
        if (!isDragging)
            return;
        isDragging = false;
        //isSelected = false;
        if (currentSelectedCardSlot != null)
        {
            currentSelectedCardSlot.UnHighlightSelectedSlot();
            currentSelectedCardSlot = null;
        }
        if (cursor.transform != null)
        {
            PoolManager.Pools["Card"].Despawn(cursor.transform);
            cursor = null;
        }
    }
    public virtual void OnTouchMove()
    {
        if (cardOwner == CardOwner.Enemy)
            return;
        if(GameBattleScene.instance!= null)
        {
            if (GameBattleScene.instance.battleState == BATTLE_STATE.WAIT_COMFIRM)
                return;
            if (GameBattleScene.instance.skillState != SkillState.None)
                return;
        }
        if (!isTouch)
            return;
        //if (Time.time - currentClickTime < 0.3f)
        //    return;
        if (isDragging)
            return;
    }
    public virtual void OnEndHold()
    {
        if (cardOwner != CardOwner.Player)
            return;
        if (!isInteracted)
            return;
        if (isDragging)
            return;
        isHoldM=false;
    }
#endif
    public void MoveFail()
    {
        if(cursor!=null)
        {
            PoolManager.Pools["Card"].Despawn(cursor.transform);
            cursor = null;
        }    
        if (newCardClone != null)
        {
            PoolManager.Pools["Card"].Despawn(newCardClone);
            newCardClone = null;
        }
    }

    public void CreateCloneOnDragging()
    {
        Transform trans = PoolManager.Pools["Card"].Spawn(GameBattleScene.instance.cursorObject);
        cursor = trans.GetComponent<Cursor>();
        if (newCardClone == null)
        {
            // spawn for both spell, minion, god
            if (heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
                newCardClone = PoolManager.Pools["Card"].Spawn(cardCloneSpell.transform);
            else if (heroInfo.type == DBHero.TYPE_TROOPER_NORMAL)
                newCardClone = PoolManager.Pools["Card"].Spawn(cardCloneMinion);
            else
                newCardClone = PoolManager.Pools["Card"].Spawn(cardCloneGod);
            newCardClone.GetComponent<CardOnBoardClone>().InitData(heroInfoTmp,battleID,frameC);
            newCardClone.gameObject.SetActive(false);
        }
        cursor.gameObject.SetActive(true);
        cursor.InitCursor(transform.position, this, newCardClone);
    }
    public void CreateCloneOnDraggingTutorial()
    {
        Transform trans = PoolManager.Pools["Card"].Spawn(BattleSceneTutorial.instance.cursorObject);
        cursor = trans.GetComponent<Cursor>();
        if (newCardClone == null)
        {
            // spawn for both spell, minion, god
            if (heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
                newCardClone = PoolManager.Pools["Card"].Spawn(cardCloneSpell.transform);
            else if (heroInfo.type == DBHero.TYPE_TROOPER_NORMAL)
                newCardClone = PoolManager.Pools["Card"].Spawn(cardCloneMinion);
            else
                newCardClone = PoolManager.Pools["Card"].Spawn(cardCloneGod);
            newCardClone.GetComponent<CardOnBoardClone>().InitData(heroInfoTmp,battleID,frameC);
            newCardClone.gameObject.SetActive(false);
        }
        cursor.gameObject.SetActive(true);
        cursor.InitCursor(transform.position, this, newCardClone);
    }
#endregion
}