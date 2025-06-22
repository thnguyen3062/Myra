using DG.Tweening;
using GIKCore.Net;
using GIKCore.Sound;
using GIKCore.Utilities;
using PathologicalGames;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using EZCameraShake;
using GIKCore.Bundle;

public struct CardStageProps
{
    public long id;
    public long heroId;
    public int stage;
}
public struct CardUpdateHeroMatrix
{
    public long battleId;
    public long heroId;
    public CardOwner own;
    //public long shard;
    public DBHero heroInfo;
}
public struct CardUltiStage
{
    public long heroId;
    public int stage;
}

public class BoardCard : Card
{
    #region Serialized Fields
    [Header("Board Object")]
    [SerializeField] private GameObject[] boardMesh; // skinned mesh renderer (print + frame)
    [SerializeField] private GameObject shadowGO;
    [SerializeField] private GameObject outlineTouchGO;
    [SerializeField] private MeshRenderer spellBoardMesh;

    [SerializeField] private TextMeshPro healthText;
    [SerializeField] private TextMeshPro damageText;
    [SerializeField] private TextMeshPro breakerText;
    [SerializeField] private TextMeshPro godSlayerText;
    [SerializeField] private Animator cardAnimator;
    [SerializeField] private Material normalMatFrameMinion;
    [SerializeField] private Material normalMatPrintMinion;
    [SerializeField] private Material normalMatFrameHighGod, normalMatFrameLowGod;
    [SerializeField] private Material normalMatPrintGod;

    [Header("Others")]
    [SerializeField] private GameObject outline;

    [Header("Card Effects")]
    [SerializeField] private Transform shieldEffect;
    private Transform shieldSkeletonEffect;
    [SerializeField] private GameObject m_CardAttackEffect;
    [SerializeField] private GameObject m_TiredEffect;
    [SerializeField] private Transform comboXEffect;
    [SerializeField] private Transform cleaveEffect;
    [SerializeField] private Transform overrunEffect;
    [SerializeField] private Transform breakerEffect;
    [SerializeField] private Transform gloryEffect;
    [SerializeField] private Transform healingEffect;
    [SerializeField] private Transform buffEffect;
    [SerializeField] private Transform shieldSpawnPosition;
    [SerializeField] private Transform addShardEffect;
    [SerializeField] private Transform impactFireEffect;
    [SerializeField] private Transform flyingObject;
    [SerializeField] private GameObject[] breakerKeyEffect;
    [SerializeField] private Transform impactBreakerEffect;
    [SerializeField] private GameObject breakerNumber;
    [SerializeField] private GameObject[] godSlayerKeyEffect;
    [SerializeField] private Transform impactgodSlayerEffect;
    [SerializeField] private GameObject godSlayerNumber;
    [SerializeField] private Transform godSlayerImpactPos;
    [SerializeField] private GameObject[] comboKeyEffect;
    [SerializeField] private Transform impactComboEffect;
    [SerializeField] private Transform comboImpactPos;
    [SerializeField] private GameObject[] cleaveKeyEffect;
    [SerializeField] private Transform impactCleaveEffect;
    [SerializeField] private GameObject cleaveIcon;
    [SerializeField] private Transform impactCleavePos;
    [SerializeField] private GameObject[] pierceEffect;
    [SerializeField] private GameObject pierceIcon;
    [SerializeField] private Transform impactPierceEffect;
    [SerializeField] private Transform impactPierceImpactPos;
    [SerializeField] private Transform impactMinionDie;
    [SerializeField] private Transform impactDiePosition;

    [SerializeField] private Transform movableEffectPosition;
    [SerializeField] private Transform movableEffect;
    [SerializeField] private ParticleSystem attackFX;
    [SerializeField] private Transform m_SkillEffectPosition;
    [SerializeField] private Transform m_EffectBidYourCard;

    [SerializeField] private Transform addBreakerEffect;
    [SerializeField] private Transform addCleaveEffect;
    [SerializeField] private Transform addComboEffect;
    [SerializeField] private Transform addGodSlayerEffect;
    [SerializeField] private Transform addOverrunEffect;
    [SerializeField] private Transform addPierceEffect;
    [SerializeField] private Transform premiumEffects;
    [SerializeField] private Transform iconSkillUnlockPrefab;
    [SerializeField] private Transform skillUnlockProjectTile;
    [SerializeField] private Transform skillInfoPrefab;
    [SerializeField] private Transform skillInfoPosition;
    [SerializeField] private Transform keywordInfo;
    #endregion

    #region Private Fields
    private int attackCount;
    private CardSkillHandler skillHandle;
    private BoardCard currentCardTarget;
    private TowerController currentTowerTarget;
    private List<SkillEffect> lstSkillEffect = new List<SkillEffect>();
    private GameObject movableEffectGO;
    #endregion

    #region HideInspector Fields
    [HideInInspector] public CardSlot slot;
    [HideInInspector] public SC_GodLinkColor godLinkEffect;
    [HideInInspector] public bool isFragile;
    [HideInInspector] public bool isRevival;
    [HideInInspector] public bool isTired;
    #endregion

    #region Actions
    //public event ICallback.CallFunc onDissolve;
    public event ICallback.CallFunc2<BoardCard> onAddToListSkill;
    //public event ICallback.CallFunc2<BoardCard> onRemoveFromListSkill;
    public event ICallback.CallFunc2<BoardCard> onEndSkillActive;
    #endregion

    #region Debug Fields
    [Header("Debug")]
    public bool isDebug;
    [HideInInspector] public long atkValue;
    [HideInInspector] public long hpValue;
    [HideInInspector] public long hpMaxValue;
    [HideInInspector] public long cleaveValue;
    [HideInInspector] public long pierceValue;
    [HideInInspector] public long breakerValue;
    [HideInInspector] public long comboValue;
    [HideInInspector] public long overrunValue;
    [HideInInspector] public long shieldValue;
    [HideInInspector] public long godSlayerValue;

    private bool isHold = false;
    private float holdTimeToPreview = 0;
    #endregion

    #region Unity Methods
    private void Start()
    {
        skillHandle = GetComponent<CardSkillHandler>();
        if (GameBattleScene.instance != null)
        {
            GameBattleScene.instance.onGameBattleChangeTurn += OnEndRound;
            GameBattleScene.instance.onResetAttackCount += SetAttackCount;
            GameBattleScene.instance.onFinishChooseOneTarget += OnEndSkillActive;
            GameBattleScene.instance.onEndSkillActive += OnEndSkillActive;
            GameBattleScene.instance.onGameConfirmStartBattle += OnGameConfirmStartBattle;
            GameBattleScene.instance.onGameBattleSimulation += OnGameBattleSimulation;
            //GameBattleScene.instance.onDragShard += OnDragShard;
            //GameBattleScene.instance.onEndDragShard += OnEndDragShard;

        }
    }

    private void FixedUpdate()
    {
        OnDragging();
        if (isHold)
        {
            holdTimeToPreview += Time.deltaTime;
            if (holdTimeToPreview > 0.3f && !isDragging)
            {
                holdTimeToPreview = 0;
                isHold = false;
                if(GameBattleScene.instance!= null)
                    UIManager.instance.ShowPreviewHandCard(this, heroInfoTmp, this.frameC);
                else
                    TutorialController.instance.ShowPreviewHandCard(this, heroInfoTmp, this.frameC);
            }

        }
    }
    #endregion

    public void SetBoardCardData(long battleID, long heroID, long frame, long atk, long hp, long mana, CardOwner owner, CardSlot slot)
    {
        SetCardData(battleID, heroID, frame, owner,atk,hp, mana);
        this.slot = slot;
    }

    public override void SetCardData(long battleID, long heroID, long frame, CardOwner owner, long atk, long hp, long mana)
    {
        base.SetCardData(battleID, heroID, frame, owner,atk , hp, mana );  
        heroInfoTmp.atk = atk;
        heroInfoTmp.hp = hp;
        heroInfoTmp.mana = mana;
        Texture cardTexture = CardData.Instance.GetOnBoardTexture(heroInfo.id);
        if (heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
            SetSpellData(cardTexture);
        else if (heroInfo.type == DBHero.TYPE_TROOPER_NORMAL)
            SetMinionData(cardTexture);
        else
            SetGodData(cardTexture);
        breakerValue = heroInfoTmp.breaker;
        if (heroInfo.type != DBHero.TYPE_TROOPER_MAGIC && heroInfo.type != DBHero.TYPE_BUFF_MAGIC)
        {
            attackFX.GetComponent<AttackFXChangeSprite>().SetAttack(heroInfo.type == DBHero.TYPE_GOD, heroInfo.rarity);
            shadowGO.GetComponent<ShadowChangeSprite>().SetShadow(heroInfo.type == DBHero.TYPE_GOD, heroInfo.rarity);
            outline.GetComponent<ShadowChangeSprite>().SetShadow(heroInfo.type == DBHero.TYPE_GOD, heroInfo.rarity);
            outlineTouchGO.GetComponent<ShadowChangeSprite>().SetShadow(heroInfo.type == DBHero.TYPE_GOD, heroInfo.rarity);
        }
        if (keywordInfo != null)
        {
            string kwUpdate = UpdateHeroKeywordInfo(this.heroInfoTmp);
            if (!string.IsNullOrEmpty(kwUpdate))
            {
                keywordInfo.gameObject.SetActive(true);
                keywordInfo.GetChild(0).GetComponent<TextMeshProUGUI>().text = kwUpdate;
            }
            else
            {
                keywordInfo.gameObject.SetActive(false);
            }
        }
        SetTired(0);
    }

    private void SetSpellData(Texture cardTexture)
    {
        spellBoardMesh.material.SetTexture("_print", cardTexture);
    }

    private void SetMinionData(Texture cardTexture)
    {
        Texture cardFrame = CardData.Instance.GetCardFrameTexture("MortalB_" + frameC + "_" + /*hero.rarity*/1);
        Texture frameMask = CardData.Instance.GetCardFrameMaskTexture("MMortalB_" + /*hero.rarity*/1);
        Texture flareMask = CardData.Instance.GetCardFrameMaskTexture("T_FlareMask");
        if (heroInfo.rarity > 3)
        {
            boardMesh[1].SetActive(true);
            var mat = boardMesh[1].GetComponent<SkinnedMeshRenderer>().materials;
            mat[0] = normalMatPrintMinion;
            mat[1] = normalMatFrameMinion;
            mat[1].SetTexture("_print_img", cardFrame);
            mat[1].SetTexture("FrameMask", frameMask);
            mat[1].SetTexture("_FlareMask", flareMask);
            boardMesh[1].GetComponent<SkinnedMeshRenderer>().materials = mat;

            boardMesh[0].SetActive(false);
            foreach (Transform eff in premiumEffects)
                eff.gameObject.SetActive(false);
            if (frameC >= 3)
            {
                var mats = boardMesh[1].GetComponent<SkinnedMeshRenderer>().materials;
                mats[0] = Instantiate(CardData.Instance.GetAnimatedMaterialBoardCard("M_" + heroID+"_3d")) as Material;
                mats[0].shader = Shader.Find("ReadyPrefab/Shader3.14");
                mats[0].SetVector("_TileOff", new Vector4(0.6f, 0.9f, 0.15f, 0.15f));
                //set frame
                Material matframe = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_" + heroInfo.type + "_" + frameC)) as Material;
                mats[1] = matframe;
                Texture main = CardData.Instance.GetCardFrameTexture("MortalB_" + frameC + "_" + /*hero.rarity*/1);
                //mats[1].SetTexture("Texture2D_08500469d2944895a9ad392692f50c3c", main);
                mats[1].SetTexture("_MainTex", main);
                Texture mask = CardData.Instance.GetCardFrameMaskTexture("MMortalB_" + /*hero.rarity*/1);
                //mats[1].SetTexture("Texture2D_17ff7d92927445598a03023feb129259", mask);
                mats[1].SetTexture("_FrameMask", mask);

                mats[1].SetTexture("_FlareMask", flareMask);
                boardMesh[1].GetComponent<SkinnedMeshRenderer>().materials = mats;

                //boardMesh[1].GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat("TotalDuration", 1);
                premiumEffects.GetChild((int)frameC - 3).gameObject.SetActive(true);
                premiumEffects.GetChild((int)frameC - 3).GetChild(0).GetComponent<MeshRenderer>().material.SetTexture("Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d", frameMask);
            }
            else
            {
                boardMesh[1].GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat("TotalDuration", 0);
                boardMesh[1].GetComponent<SkinnedMeshRenderer>().materials[0].SetTexture("_print_img", cardTexture);
            }
        }
        else
        { // thu tu material bi dao nguoc thu tu giua low va high frame
            boardMesh[1].SetActive(false);
            boardMesh[0].SetActive(true);
            var mat = boardMesh[0].GetComponent<SkinnedMeshRenderer>().materials;
            mat[1] = normalMatPrintMinion;
            mat[0] = normalMatFrameMinion;

            mat[0].SetTexture("_print_img", cardFrame);
            mat[0].SetTexture("FrameMask", frameMask);
            mat[0].SetTexture("_FlareMask", flareMask);
            boardMesh[0].GetComponent<SkinnedMeshRenderer>().materials = mat;
            foreach (Transform eff in premiumEffects)
                eff.gameObject.SetActive(false);
            if (frameC >= 3)
            {
                var mats = boardMesh[0].GetComponent<SkinnedMeshRenderer>().materials;
                mats[1] = Instantiate(CardData.Instance.GetAnimatedMaterialBoardCard("M_" + heroID+"_3d")) as Material;
                mats[1].shader = Shader.Find("ReadyPrefab/Shader3.14");
                mats[1].SetVector("_TileOff", new Vector4(0.6f, 0.9f, 0.15f,0.15f));
                //set frame
                Material matframe = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_" + heroInfo.type + "_" + frameC)) as Material;
                mats[0] = matframe;
                Texture main = CardData.Instance.GetCardFrameTexture("MortalB_" + frameC + "_" + /*hero.rarity*/1);
                //mats[0].SetTexture("Texture2D_08500469d2944895a9ad392692f50c3c", main);
                mats[0].SetTexture("_MainTex", main);
                Texture mask = CardData.Instance.GetCardFrameMaskTexture("MMortalB_" + /*hero.rarity*/1);
                //mats[0].SetTexture("Texture2D_17ff7d92927445598a03023feb129259", mask);
                mats[0].SetTexture("_FrameMask", mask);
                mats[0].SetTexture("_FlareMask", flareMask);
                boardMesh[0].GetComponent<SkinnedMeshRenderer>().materials = mats;

                //boardMesh[0].GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("TotalDuration", 1);
                premiumEffects.GetChild((int)frameC - 3).gameObject.SetActive(true);
                premiumEffects.GetChild((int)frameC - 3).GetChild(0).GetComponent<MeshRenderer>().material.SetTexture("Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d", frameMask);
            }
            else
            {
                boardMesh[0].GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("TotalDuration", 0);
                boardMesh[0].GetComponent<SkinnedMeshRenderer>().materials[1].SetTexture("_print_img", cardTexture);
            }
        }

        if (healthText != null)
            healthText.text = heroInfoTmp.hp.ToString();
        if (damageText != null)
            damageText.text = heroInfoTmp.atk.ToString();
        if (breakerText != null)
            breakerText.text = heroInfoTmp.breaker.ToString();
        if (godSlayerText != null)
            godSlayerText.text = heroInfoTmp.godSlayer.ToString();
    }

    private void SetGodData(Texture cardTexture)
    {
        Texture cardFrame = CardData.Instance.GetCardFrameTexture("GodB_" + frameC + "_" + heroInfo.rarity + "_" + heroInfo.color);
        Texture frameMask = CardData.Instance.GetCardFrameMaskTexture("MGodB_" + heroInfo.rarity);
        Texture flareMask = CardData.Instance.GetCardFrameMaskTexture("T_FlareMask");
        foreach (GameObject go in boardMesh)
            go.SetActive(false);
        GameObject f = boardMesh[heroInfo.rarity - 1];
        f.SetActive(true);
        if (heroInfo.rarity > 3)
        {
            var mat = f.GetComponent<SkinnedMeshRenderer>().materials;
            mat[0] = normalMatPrintGod;
            mat[1] = normalMatFrameLowGod;
            mat[1].SetTexture("_print_img", cardFrame);
            mat[1].SetTexture("FrameMask", frameMask);
            mat[1].SetTexture("_FlareMask", flareMask);
            f.GetComponent<SkinnedMeshRenderer>().materials = mat;
            foreach (Transform eff in premiumEffects)
                eff.gameObject.SetActive(false);
            if (frameC >= 3)
            {
                //set print
                var mats = f.GetComponent<SkinnedMeshRenderer>().materials;
                mats[0] = Instantiate(CardData.Instance.GetAnimatedMaterialBoardCard("M_" + heroID+"_3d")) as Material;
                mats[0].shader = Shader.Find("ReadyPrefab/Shader3.14");
                mats[0].SetVector("_TileOff", new Vector3(0.68f, 0.93f, 0.17f));
                //set frame
                Material matframe = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_" + heroInfo.type + "_" + frameC)) as Material;
                mats[1] = matframe;
                Texture main = CardData.Instance.GetCardFrameTexture("GodB_" + frameC + "_" + heroInfo.rarity + "_" + heroInfo.color);
                //mats[1].SetTexture("Texture2D_08500469d2944895a9ad392692f50c3c", main);
                mats[1].SetTexture("_MainTex", main);
                Texture mask = CardData.Instance.GetCardFrameMaskTexture("MGodB_" + heroInfo.rarity);
                //mats[1].SetTexture("Texture2D_17ff7d92927445598a03023feb129259", mask);
                mats[1].SetTexture("_FrameMask", mask);

                mats[1].SetTexture("_FlareMask", flareMask);

                f.GetComponent<SkinnedMeshRenderer>().materials = mats;



                //f.GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat("TotalDuration", 2);
                premiumEffects.GetChild((int)frameC - 3).gameObject.SetActive(true);
                premiumEffects.GetChild((int)frameC - 3).GetChild(0).GetComponent<MeshRenderer>().material.SetTexture("Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d", frameMask);
            }
            else
            {
                f.GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat("TotalDuration", 0);
                f.GetComponent<SkinnedMeshRenderer>().materials[0].SetTexture("_print_img", cardTexture);
            }

        }
        else
        {
            var mat = f.GetComponent<SkinnedMeshRenderer>().materials;
            mat[0] = normalMatFrameLowGod;
            mat[1] = normalMatPrintGod;
            mat[0].SetTexture("_print_img", cardFrame);
            mat[0].SetTexture("FrameMask", frameMask);
            mat[0].SetTexture("_FlareMask", flareMask);
            f.GetComponent<SkinnedMeshRenderer>().materials = mat;
            foreach (Transform eff in premiumEffects)
                eff.gameObject.SetActive(false);
            if (frameC >= 3)
            {
                var mats = f.GetComponent<SkinnedMeshRenderer>().materials;
                mats[1] = Instantiate(CardData.Instance.GetAnimatedMaterialBoardCard("M_" + heroID+"_3d")) as Material;
                mats[1].shader = Shader.Find("ReadyPrefab/Shader3.14");
                mats[1].SetVector("_TileOff", new Vector3(0.68f, 0.93f, 0.17f));
                //set frame
                Material matframe = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_" + heroInfo.type + "_" + frameC)) as Material;
                mats[0] = matframe;
                Texture main = CardData.Instance.GetCardFrameTexture("GodB_" + frameC + "_" + heroInfo.rarity + "_" + heroInfo.color);
                //mats[0].SetTexture("Texture2D_08500469d2944895a9ad392692f50c3c", main);
                mats[0].SetTexture("_MainTex", main);
                Texture mask = CardData.Instance.GetCardFrameMaskTexture("MGodB_" + heroInfo.rarity);
                //mats[0].SetTexture("Texture2D_17ff7d92927445598a03023feb129259", mask);
                
                mats[0].SetTexture("_FrameMask", mask);

                mats[0].SetTexture("_FlareMask", flareMask);
                f.GetComponent<SkinnedMeshRenderer>().materials = mats;
                //f.GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("TotalDuration", 1);
                premiumEffects.GetChild((int)frameC - 3).gameObject.SetActive(true);
                premiumEffects.GetChild((int)frameC - 3).GetChild(0).GetComponent<MeshRenderer>().material.SetTexture("Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d", frameMask);
            }
            else
            {
                f.GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("TotalDuration", 0);
                f.GetComponent<SkinnedMeshRenderer>().materials[1].SetTexture("_print_img", cardTexture);
            }
        }


        if (healthText != null)
            healthText.text = heroInfoTmp.hp.ToString();
        if (damageText != null)
            damageText.text = heroInfoTmp.atk.ToString();
        if (breakerText != null)
            breakerText.text = heroInfoTmp.breaker.ToString();
        if (godSlayerText != null)
            godSlayerText.text = heroInfoTmp.godSlayer.ToString();
    }

    private void OnGameBattleSimulation(bool isYourTurn,long c,long c2)
    {
        if (cardOwner == CardOwner.Player)
            SetMovable(isYourTurn);
        else
            SetMovable(!isYourTurn);
    }
    //private void OnDragShard()
    //{
    //    if (cardOwner == CardOwner.Enemy)
    //        return;
    //    if (GameBattleScene.instance != null)
    //    {
    //        if (GameBattleScene.instance.IsYourTurn)
    //        {
    //            if (heroInfo.type == DBHero.TYPE_GOD)
    //            {
    //                //tween
    //                HighlightUnit();
    //                transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.01f).SetEase(Ease.OutElastic);

    //            }
    //        }
    //    }
    //}
    //private void OnEndDragShard()
    //{
    //    if (cardOwner == CardOwner.Enemy)
    //        return;
    //    if (GameBattleScene.instance != null)
    //    {
    //        if (GameBattleScene.instance.IsYourTurn)
    //        {
    //            if (heroInfo.type == DBHero.TYPE_GOD)
    //            {
    //                //tween
    //                UnHighlightUnit();
    //                transform.DOScale(new Vector3(1f, 1f, 1f), 0.01f).SetEase(Ease.OutElastic);

    //            }
    //        }
    //    }
    //}
    private void SetAttackCount()
    {
        attackCount = 0;
    }
    public override void OnGameConfirmStartBattle()
    {
        base.OnGameConfirmStartBattle();

    }
    public override void OnEndRound(long index)
    {
        if (index == -1)
        {
            if (shieldValue > 0 && shieldSkeletonEffect != null)
                shieldSkeletonEffect.gameObject.SetActive(false);

            if (gameObject.activeSelf && isFragile)
            {
                OnDeath();
                SoundHandler.main.PlaySFX("BrokenCard1", "sounds");
                GameBattleScene.instance.lstCardInBattle.Remove(this);

            }
        }
    }

    public override void OnEndSkillActive()
    {
        UnHighlightUnit();
        onEndSkillActive?.Invoke(this);
    }

    public override void HighlightUnit()
    {
        if (outline != null)
            outline.SetActive(true);
        canSelect = true;
    }

    public override void UnHighlightUnit()
    {
        if (outline != null)
            outline.SetActive(false);
        canSelect = false;
    }

    public void ShowEffectSkillBid()
    {
        Transform trans = PoolManager.Pools["Effect"].Spawn(m_EffectBidYourCard, m_SkillEffectPosition);
        trans.localScale = Vector3.one;
        trans.localPosition = Vector3.zero;
        trans.GetComponent<ParticleEffectCallback>().SetOnComplete(() =>
        {
            PoolManager.Pools["Effect"].Despawn(trans);
        });
    }    
    public void Selected()
    {
        if (heroInfo.type != DBHero.TYPE_TROOPER_MAGIC && heroInfo.type != DBHero.TYPE_BUFF_MAGIC)
            cardAnimator.SetBool("_IsSelected", true);
    }

    public void Placed()
    {
        SetIdleAnimation();
    }

    public void SetSummonAnimation()
    {
        if (heroInfo.type != DBHero.TYPE_TROOPER_MAGIC && heroInfo.type != DBHero.TYPE_BUFF_MAGIC)
            cardAnimator.SetTrigger("_IsSpawn");
        if (heroInfo.type == DBHero.TYPE_GOD)
        {
            HandleNetData.QueueNetData(NetData.CARD_CHANGE_STAGE, new CardStageProps() { id = battleID, heroId = this.heroID, stage = 1 });
            HandleNetData.QueueNetData(NetData.Card_UPDATE_ULTI_STAGE, new CardUltiStage() { heroId = heroID, stage = 0 });
        }
    }
    
    public void SetIdleAnimation()
    {
        if (heroInfo.type != DBHero.TYPE_TROOPER_MAGIC && heroInfo.type != DBHero.TYPE_BUFF_MAGIC)
            cardAnimator.SetBool("_IsSelected", false);
    }

    #region Battle
    public void OnAttackCard(BoardCard target, out float waitTime)
    {
        waitTime = 0;
        attackCount += 1;
        attackFX.gameObject.SetActive(true);
        attackFX.Play();
        //if (attackCount > 0 && comboValue > 0)
        //{
        //    Transform trans = PoolManager.Pools["Effect"].Spawn(comboXEffect.transform);
        //    trans.parent = transform;
        //    trans.localPosition = new Vector3(0, 0.21f, 0);
        //    trans.localRotation = Quaternion.Euler(-90f, 0, 0);
        //    SkeletonAnimation comboEffect = trans.GetComponent<SkeletonAnimation>();
        //    waitTime = comboEffect.skeletonDataAsset.GetSkeletonData(true).FindAnimation("start").Duration;
        //    SoundHandler.main.PlaySFX("Combo", "soundvfx");
        //    comboEffect.AnimationState.SetAnimation(0, "start", false).Complete += delegate
        //    {
        //        PoolManager.Pools["Effect"].Despawn(trans);
        //        OnAttackCardTarget(target);
        //    };
        //}
        //else
        //{
        //    waitTime = 0;
        //    attackCount += 1;
        //    OnAttackCardTarget(target);
        //}
        OnAttackCardTarget(target);
    }

    public void OnAttackTower(TowerController target, out float waitTime)
    {
        if (attackCount > 0 && comboValue > 0)
        {
            Transform trans = PoolManager.Pools["Effect"].Spawn(comboXEffect.transform);
            trans.parent = transform;
            trans.localPosition = new Vector3(0, 0.21f, 0);
            trans.localRotation = Quaternion.Euler(-90f, 0, 0);
            SkeletonAnimation comboEffect = trans.GetComponent<SkeletonAnimation>();
            waitTime = comboEffect.skeletonDataAsset.GetSkeletonData(true).FindAnimation("start").Duration;
            SoundHandler.main.PlaySFX("Combo", "soundvfx");
            comboEffect.AnimationState.SetAnimation(0, "start", false).Complete += delegate
            {
                OnAttackTowerTarget(target);
                PoolManager.Pools["Effect"].Despawn(trans);
            };
        }
        else
        {
            OnAttackTowerTarget(target);
            waitTime = 0;
        }
    }

    private void OnAttackCardTarget(BoardCard target)
    {
        currentCardTarget = target;
        if (!cardAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            return;

        if (comboValue > 0 && attackCount == 1)
        {
            foreach (GameObject go in comboKeyEffect)
                go.SetActive(true);
        }
        if (!target.isTired)
        {
            if (slot.xPos == 0 && slot.xPos == currentCardTarget.slot.xPos)
                cardAnimator.SetTrigger("_Attack0");
            else if (slot.xPos == 1 && slot.xPos == currentCardTarget.slot.xPos)
                cardAnimator.SetTrigger("_Attack1");
            else
                cardAnimator.SetTrigger("_Attack05");
        }
        else
        {
            if (slot.xPos == 0 && slot.xPos == currentCardTarget.slot.xPos)
                cardAnimator.SetTrigger("_Attack0");
            else if (slot.xPos == 1 && currentCardTarget.slot.xPos ==1)
                cardAnimator.SetTrigger("_Attack2");
            else
                cardAnimator.SetTrigger("_Attack1");
        }    
    }

    private void OnAttackTowerTarget(TowerController target)
    {
        currentTowerTarget = target;
        if (!cardAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            return;
        if (slot.xPos == 0)
            cardAnimator.SetTrigger("_Attack2");
        else
            cardAnimator.SetTrigger("_Attack3");
    }

    public void OnDamaged(long damage, long health)
    {
        DamagePopup.Create(transform.position, damage, PopupType.Damage);
        healthText.text = health.ToString();
        UpdateHeroInfo(hp: health);
        HandleNetData.QueueNetData(NetData.CARD_UPDATE_MATRIX, new CardUpdateHeroMatrix() { battleId = battleID, heroId = this.heroID, own = cardOwner, heroInfo = this.heroInfoTmp});
        hpValue = health;
        if (shieldValue > 0 && shieldSkeletonEffect != null)
            shieldSkeletonEffect.gameObject.SetActive(false);
    }

    public void OnBeingAttacked()
    {
        if (cardAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            cardAnimator.SetTrigger("_BeingAttacked");
        }
    }

    public void OnAttackTriggerCallback()
    {
        DisplayAttackEffect();
        if (heroInfo.type == DBHero.TYPE_GOD)
        {
            string voiceName = heroInfo.heroNumber.ToString() + "_" + VoiceData.TYPE_ATTACK + "_" + Random.Range(1, 5).ToString();
            //CheckHeroSkill(TYPE_WHEN_SUMON, card,slot.xPos,slot.yPos);
            AudioClip clip = BundleHandler.LoadSound(voiceName, "voicegod");
            if (clip != null)
                SoundHandler.main.PlaySFX(voiceName, "voicegod");
        }
        if (currentCardTarget != null)
        {
            m_CardAttackEffect.SetActive(true);
            m_CardAttackEffect.GetComponent<ParticleEffectParentCallback>().SetOnComplete(() =>
            {
                m_CardAttackEffect.SetActive(false);
            }).SetCallbackAfterPlay();
            if (heroInfoTmp.atk >= 4)
            {
                CameraShaker.Instance.ShakeOnce(1f, 8, .1f, 0.8f);
            }
            else
            {
                CameraShaker.Instance.ShakeOnce(0.5f, 8, .1f, 0.8f);
            }    
            currentCardTarget.OnBeingAttacked();
            currentCardTarget = null;
            SoundHandler.main.PlaySFX("CardAttack2", "sounds");
        }
        if (currentTowerTarget != null)
        {
            if (heroInfoTmp.atk > 4)
            {
                CameraShaker.Instance.ShakeOnce(1, 3, .1f, .6f);
            }
            else
            {
                CameraShaker.Instance.ShakeOnce(0.7f, 2, .1f, .6f);
            }

            if(GameBattleScene.instance!=null)
            {
                GameBattleScene.instance.TurnOnEffectAttackTower(this.cardOwner == CardOwner.Enemy, slot.yPos < 2 ? true : false);
            }    
            else
            {
                BattleSceneTutorial.instance.TurnOnEffectAttackTower(this.cardOwner == CardOwner.Enemy, slot.yPos < 2 ? true : false);
            }
            
            Transform trans = PoolManager.Pools["Effect"].Spawn(impactFireEffect.transform);
            trans.position = currentTowerTarget.transform.position;
            trans.GetComponent<ParticleEffectParentCallback>().SetOnPlay();
            currentTowerTarget.OnBeingAttacked();
            currentTowerTarget = null;
            SoundHandler.main.PlaySFX("CardAttack3", "sounds");

        }
    }
    public void OnEndAttackCallback()
    {
        attackFX.gameObject.SetActive(false);
        breakerNumber.SetActive(false);
        foreach (GameObject go in breakerKeyEffect)
            go.SetActive(false);
        godSlayerNumber.SetActive(false);
        foreach (GameObject go in godSlayerKeyEffect)
            go.SetActive(false);
        foreach (GameObject go in comboKeyEffect)
            go.SetActive(false);
        cleaveIcon.SetActive(false);
        foreach (GameObject go in cleaveKeyEffect)
            go.SetActive(false);
        pierceIcon.SetActive(false);
        foreach (GameObject go in pierceEffect)
            go.SetActive(false);
        foreach (GameObject go in comboKeyEffect)
            go.SetActive(false);
    }
    public void DisplayListEffect(List<SkillEffect> lstEff)
    {
        // set bang null khi dung xong 
        lstSkillEffect = lstEff;
        foreach (SkillEffect effect in lstEff)
        {
            switch (effect.typeEffect)
            {
                case DBHero.KEYWORD_CLEAVE:
                    {
                        cleaveIcon.SetActive(true);
                        foreach (GameObject go in cleaveKeyEffect)
                            go.SetActive(true);
                        break;
                    }
                case DBHero.KEYWORD_PIERCE:
                    {
                        pierceIcon.SetActive(true);
                        foreach (GameObject go in pierceEffect)
                            go.SetActive(true);
                        break;
                    }
                case DBHero.KEYWORD_OVERRUN:
                    {
                        break;
                    }
                case DBHero.KEYWORD_BREAKER:
                    {
                        breakerNumber.SetActive(true);
                        foreach (GameObject go in breakerKeyEffect)
                            go.SetActive(true);
                        break;
                    }
                case DBHero.KEYWORD_GODSLAYER:
                    {
                        godSlayerNumber.SetActive(true);
                        foreach (GameObject go in godSlayerKeyEffect)
                            go.SetActive(true);
                        break;
                    }
                case DBHero.KEYWORD_COMBO:
                    {
                        break;
                    }
                case DBHero.KEYWORD_DEFENDER:
                    {
                        break;
                    }
            }
        }
    }
    private void DisplayListEffectImpact()
    {
        foreach (SkillEffect effect in lstSkillEffect)
        {
            switch (effect.typeEffect)
            {
                case DBHero.KEYWORD_CLEAVE:
                    {
                        // sqawn la ok 
                        if (currentCardTarget != null)
                        {
                            Transform trans = PoolManager.Pools["Effect"].Spawn(impactCleaveEffect.transform);
                            trans.parent = currentCardTarget.impactCleavePos;
                            trans.localRotation = Quaternion.Euler(0, 0, 0);
                            trans.localPosition = new Vector3(0, 0f, 0);
                            if (slot.yPos == 0 || slot.yPos == 2)
                            {
                                // attack from left to right (x = -1)
                                StartCoroutine(trans.gameObject.GetComponent<CleaveEffectControl>().OnCleaveHit(false));
                            }
                            else if (slot.yPos == 1 || slot.yPos == 3)
                            {
                                // attack from right to left (x = 1)
                                StartCoroutine(trans.gameObject.GetComponent<CleaveEffectControl>().OnCleaveHit(true));
                            }
                        }
                        break;
                    }
                case DBHero.KEYWORD_PIERCE:
                    {
                        //spawn kem target
                        break;
                    }
                case DBHero.KEYWORD_OVERRUN:
                    {
                        //spawn 1-2 target 
                        break;
                    }
                case DBHero.KEYWORD_BREAKER:
                    {
                        //spawn la ok , danh thang vao target la tru
                        if (currentTowerTarget != null)
                        {
                            Transform trans = PoolManager.Pools["Effect"].Spawn(impactBreakerEffect);
                            trans.parent = currentTowerTarget.transform;
                            trans.localPosition = new Vector3(0, 0f, 0);
                            trans.localRotation = Quaternion.Euler(0f, 0, 0);
                        }
                        break;
                    }
                case DBHero.KEYWORD_GODSLAYER:
                    {
                        //spawn la ok , danh thang vao target la than
                        if (currentCardTarget != null)
                        {
                            Transform trans = PoolManager.Pools["Effect"].Spawn(impactgodSlayerEffect.transform);
                            trans.parent = currentCardTarget.godSlayerImpactPos;
                            trans.localRotation = Quaternion.Euler(0, 0, 0);
                            trans.localPosition = new Vector3(0, 0f, 0f);
                            trans.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = godSlayerValue.ToString();
                        }
                        break;
                    }
                case DBHero.KEYWORD_COMBO:
                    {
                        break;
                    }
                case DBHero.KEYWORD_DEFENDER:
                    {
                        break;
                    }
            }
        }
    }
    private void DisplayAttackEffect()
    {
        // attack card
        if (currentCardTarget != null)
        {
            if (cleaveValue > 0)
            {
                //Transform trans = PoolManager.Pools["Effect"].Spawn(impactCleaveEffect.transform);
                //trans.parent = impactCleavePos;
                //trans.localRotation = Quaternion.Euler(0, 0, 0);
                //trans.localPosition = new Vector3(0, 0f, 0);
                //if (slot.yPos == 0 || slot.yPos == 2)
                //{
                //    // attack from left to right (x = -1)
                //    StartCoroutine(trans.gameObject.GetComponent<CleaveEffectControl>().OnCleaveHit(false));
                //}
                //else if (slot.yPos == 1 || slot.yPos == 3)
                //{
                //    // attack from right to left (x = 1)
                //    StartCoroutine(trans.gameObject.GetComponent<CleaveEffectControl>().OnCleaveHit(true));
                //}
            }
            if (overrunValue > 0)
            {
                Transform trans = PoolManager.Pools["Effect"].Spawn(overrunEffect.transform);
                trans.parent = trans;
                trans.localRotation = Quaternion.Euler(-90f, 0, 0);
                trans.localPosition = new Vector3(0, 0.21f, 0);
                trans.GetComponent<SkeletonAnimation>().state.SetAnimation(0, "start", false).Complete += delegate
                {
                    PoolManager.Pools["Effect"].Despawn(trans);
                };
            }
            if (godSlayerValue > 0)
            {
                //Transform trans = PoolManager.Pools["Effect"].Spawn(impactgodSlayerEffect.transform);
                //trans.parent = godSlayerImpactPos.transform;
                //trans.localRotation = Quaternion.Euler(0, 0, 0);
                //trans.localPosition = new Vector3(0,0f, 0f);
                //trans.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = godSlayerValue.ToString();
            }
            if (comboValue > 0)
            {
                //Transform trans = PoolManager.Pools["Effect"].Spawn(impactComboEffect.transform);
                //trans.parent = comboImpactPos.transform;
                //trans.localRotation = Quaternion.Euler(0, 0, 0);
                //trans.localPosition = new Vector3(0, 0f, 0f);
            }
        }
        if (currentTowerTarget != null)
        {
            if (breakerValue > 0)
            {
                Transform trans = PoolManager.Pools["Effect"].Spawn(impactBreakerEffect);
                trans.parent = currentTowerTarget.transform;
                trans.localPosition = new Vector3(0, 0f, 0);
                trans.localRotation = Quaternion.Euler(0f, 0, 0);
                //OnAttackWithSkill(currentTowerTarget.transform, null, out float waitTime);
            }
        }
    }

    public void OnAttackWithSkill(Transform target, ICallback.CallFunc callback, out float waitTime)
    {
        TwoPointEffectHandle twoPoint = new TwoPointEffectHandle();
        Transform trans = PoolManager.Pools["Effect"].Spawn(breakerEffect.transform);
        trans.parent = transform;
        trans.localPosition = new Vector3(0, 0.21f, 0);
        trans.localRotation = Quaternion.Euler(-90f, 0, 0);
        SkeletonAnimation towerDealDamageEffect = trans.GetComponent<SkeletonAnimation>();
        twoPoint = towerDealDamageEffect.GetComponent<TwoPointEffectHandle>();
        twoPoint.SetupEffect(twoPoint.transform.position, target.position, () =>
        {
            towerDealDamageEffect.AnimationState.Event += ((entry, e) =>
            {
                SoundHandler.main.PlaySFX("Breaker", "soundvfx");
            });
            towerDealDamageEffect.state.SetAnimation(0, "start", false).Complete += delegate
            {
                Destroy(trans.gameObject);

                callback?.Invoke();
            };
        });
        waitTime = towerDealDamageEffect.skeletonDataAsset.GetSkeletonData(true).FindAnimation("start").Duration;
    }

    public void OnCastSkill(long skillID, long effectID, GameObject target, ICallback.CallFunc callback, bool waitToImpact = true)
    {
        skillHandle.CastSkill(skillID, effectID, this.transform, callback, target,waitToImpact);
    }

    public void OnGlory(ICallback.CallFunc callback)
    {
        Transform trans = PoolManager.Pools["Effect"].Spawn(gloryEffect.transform);
        trans.parent = transform;
        trans.localPosition = new Vector3(0, 0, 0.17f);

        trans.localRotation = Quaternion.Euler(70f, cardOwner == CardOwner.Player ? -180f : 0, 0);
        SkeletonAnimation gloryAnimation = trans.GetComponent<SkeletonAnimation>();

        gloryAnimation.AnimationState.Event += (entry, e) =>
        {
            if (e.Data.Name == "run")
            {
                callback?.Invoke();
                SoundHandler.main.PlaySFX("Glory", "soundvfx");
            }
        };

        gloryAnimation.AnimationState.SetAnimation(0, "start", false).Complete += delegate
        {
            PoolManager.Pools["Effect"].Despawn(trans);
        };
    }

    public void SetTired(long isTired)
    {
        // chi so material nguoc 
        float setTired = (isTired == 0) ? 1 : 0.2f;
        if(m_TiredEffect != null)
            m_TiredEffect.SetActive(isTired == 1);
        int setTiredPremium = (isTired == 0) ? 0 : 1;
        if (boardMesh != null && heroInfo.type != DBHero.TYPE_TROOPER_MAGIC && heroInfo.type != DBHero.TYPE_BUFF_MAGIC)
        {
            if (heroInfo.type == DBHero.TYPE_GOD)
            {
                if (heroInfo.rarity > 3)
                {
                    if (frameC >= 3)
                        boardMesh[heroInfo.rarity - 1].GetComponent<SkinnedMeshRenderer>().materials[0].SetInt("_Tired", setTiredPremium);
                    else
                        boardMesh[heroInfo.rarity - 1].GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("_tired", setTired);

                }
                else
                {
                    if (frameC >= 3)
                        boardMesh[heroInfo.rarity - 1].GetComponent<SkinnedMeshRenderer>().materials[1].SetInt("_Tired", setTiredPremium);
                    else
                        boardMesh[heroInfo.rarity - 1].GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat("_tired", setTired);

                }
            }
            else
            {
                if (heroInfo.rarity > 3)
                {
                    if (frameC >= 3)
                        boardMesh[1].GetComponent<SkinnedMeshRenderer>().materials[0].SetInt("_Tired", setTiredPremium);
                    else
                        boardMesh[1].GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat("_tired", setTired);

                }
                else
                {
                    if (frameC >= 3)
                        boardMesh[0].GetComponent<SkinnedMeshRenderer>().materials[1].SetInt("_Tired", setTiredPremium);
                    else
                        boardMesh[0].GetComponent<SkinnedMeshRenderer>().materials[1].SetFloat("_tired", setTired);
                }
            }
        }
        this.isTired = isTired == 1 ? true : false;
        if (this.isTired)
        {
            SoundHandler.main.PlaySFX("Debuff", "sounds");
        }
    }

    public void SetMovable(bool canMove)
    {
        if (heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
            return;
        if (movableEffectGO == null)
        {
            Transform trans = PoolManager.Pools["Effect"].Spawn(movableEffect);
            trans.SetParent(movableEffectPosition);
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.Euler(Vector3.zero);
            trans.localScale = Vector3.one;
            movableEffectGO = trans.gameObject;
        }
        movableEffectGO.GetComponent<ParticleSystemRenderer>().material.SetFloat("_Opacity", canMove ? 1 : 0.3f);
    }

    
    public void CheckUnlockSkill(DBHero cardBuff,long skillID)
    {
        foreach (DBHeroSkill sk in heroInfo.lstHeroSkill)
        {
            if (!sk.isUltiType && sk.id == skillID)
            {
                if (CardData.Instance.GetCardSkillDataInfo(heroInfo.id).skillIds.Contains(skillID))
                {
                    if (GameBattleScene.instance != null)
                        StartCoroutine(SpawnUnlockSkillVfx(cardBuff, sk.id, heroInfo.color));
                }

            }
        }
    }




    IEnumerator SpawnUnlockSkillVfx(DBHero cardBuff,long id, long color)
    {
        yield return new WaitForSeconds(0f);
        SoundHandler.main.PlaySFX("SkillUnlocked", "sounds");
        Transform iconSkill = PoolManager.Pools["Effect"].Spawn(iconSkillUnlockPrefab);

        iconSkill.GetChild(iconSkill.childCount - 4).GetComponent<ParticleSystem>().GetComponent<ParticleSystemRenderer>().material.SetTexture("_BaseMap", CardData.Instance.GetUltiTexture(id.ToString()));
        iconSkill.GetChild(iconSkill.childCount - 3).GetComponent<ParticleSystem>().GetComponent<ParticleSystemRenderer>().material.SetTexture("_BaseMap", CardData.Instance.GetCardColorTexture("Skill_" + color)); ;
        iconSkill.GetChild(iconSkill.childCount - 1).GetComponent<ParticleSystem>().GetComponent<ParticleSystemRenderer>().material.SetTexture("_BaseMap", CardData.Instance.GetUltiTexture(id.ToString())); ;
        iconSkill.position = GameBattleScene.instance.skillUnlockVFXSpawnPos.position;

        yield return new WaitForSeconds(1.9f);

        Transform projectTile = PoolManager.Pools["Effect"].Spawn(skillUnlockProjectTile);
        projectTile.GetComponent<ParticleSystem>().GetComponent<ParticleSystemRenderer>().material.SetTexture("_BaseMap", CardData.Instance.GetUltiTexture(id.ToString()));
        projectTile.GetChild(projectTile.childCount - 1).GetComponent<ParticleSystem>().GetComponent<ParticleSystemRenderer>().material.SetTexture("_BaseMap", CardData.Instance.GetCardColorTexture("Skill_" + color));

        projectTile.position = iconSkill.position;
        PoolManager.Pools["Effect"].Despawn(iconSkill);

        projectTile.GetComponent<ShootProjectile>().ShootObjectFast(projectTile.position, this.transform.position, () =>
        {
            //PoolManager.Pools["Effect"].Despawn(iconSkill);
            //PoolManager.Pools["Effect"].Despawn(projectTile);
        });
        yield return new WaitForSeconds(0.4f);
        PoolManager.Pools["Effect"].Despawn(projectTile);
        DBHeroSkill dbSkill = null;
        if(cardBuff.ownerGodID == this.heroID)
        {
            for (int i = 0; i < heroInfo.lstHeroSkill.Count; i++)
            {
                if (CardData.Instance.GetCardSkillDataInfo(heroInfo.id).skillIds.Contains(heroInfo.lstHeroSkill[i].id))
                {
                    if (heroInfo.lstHeroSkill[i].id == id)
                    {
                        dbSkill = heroInfo.lstHeroSkill[i];
                        Transform skillInfo = PoolManager.Pools["Effect"].Spawn(skillInfoPrefab);
                        skillInfo.GetComponent<SkillInfo>().InitDataSkill(cardBuff,dbSkill, heroInfo.color);
                        skillInfo.SetParent(skillInfoPosition);
                        skillInfo.localScale = Vector3.one;
                        float rotateMap = (slot.yPos > 1) ? (-4) : 4;
                        skillInfo.localRotation = Quaternion.Euler(0, 0, rotateMap);
                        skillInfo.localPosition = Vector3.zero;
                        skillInfoPosition.GetComponent<CanvasGroup>().alpha = 0;
                        skillInfoPosition.GetComponent<CanvasGroup>().DOFade(1, 0.2f);
                        yield return new WaitForSeconds(2f);
                        skillInfoPosition.GetComponent<CanvasGroup>().DOFade(0, 0.2f);
                        PoolManager.Pools["Effect"].Despawn(skillInfo);
                    }
                }
            }
        }    
        


    }
    public void OnDeath()
    {
        if (this.heroInfo.type == DBHero.TYPE_GOD)
        {
            if (cardOwner == CardOwner.Player)
                GodCardHandler.instance.GodDead(this.battleID);
            else
                GodCardHandler.instance.GodEnemyDead(this.battleID);
            HandleNetData.QueueNetData(NetData.CARD_CHANGE_STAGE, new CardStageProps() { id = battleID, heroId = this.heroID, stage = 0 });
            string voiceName = heroInfo.heroNumber.ToString() + "_" + VoiceData.TYPE_DIE + "_" + Random.Range(1, 3).ToString();
            AudioClip clip = BundleHandler.LoadSound(voiceName, "voicegod");
            if (clip != null)
                SoundHandler.main.PlaySFX(voiceName, "voicegod");
        }
        if (heroInfo.type != DBHero.TYPE_TROOPER_MAGIC && heroInfo.type != DBHero.TYPE_BUFF_MAGIC)
        {
            Transform trans = PoolManager.Pools["Effect"].Spawn(impactMinionDie);
            if (heroInfo.type == DBHero.TYPE_GOD)
            {
                trans.position = impactDiePosition.position + new Vector3 (0,-2,0);
                trans.rotation = impactDiePosition.rotation;
            }
            else
            {
                trans.position = impactDiePosition.position;
                trans.rotation = impactDiePosition.rotation;
            }
            trans.GetComponent<ParticleEffectParentCallback>()
                .SetOnComplete(OnEndDeadAnim)
                .SetOnPlay();
            if(godLinkEffect != null)
                godLinkEffect.OnGodDead();
            transform.gameObject.SetActive(false);
        }
    }

    //IEnumerator DespawnCardDuringDieImpact()
    //{
    //    yield return new WaitForSeconds(0.9f);
    //}
    public void OnEndDeadAnim()
    {
        if (slot != null)
        {
            slot.ChangeSlotState(SlotState.Empty, null);
            slot = null;
        }
        //PoolManager.Pools["Card"].Despawn(transform);
        //if (heroInfo.type != DBHero.TYPE_TROOPER_MAGIC)
        //    cardAnimator.SetBool("_IsDead", false);
    }


    public void SummonNewCard(Vector3 destination, ICallback.CallFunc callback)
    {
        Transform trans = PoolManager.Pools["Effect"].Spawn(flyingObject);
        trans.position = transform.position;
        trans.DOMoveX(destination.x, 0.5f).SetEase(Ease.InQuad);
        trans.DOMoveY(destination.y, 0.5f).SetEase(Ease.OutQuad);
        trans.DOMoveZ(destination.z, 0.5f).SetEase(Ease.InQuad).onComplete += delegate
        {
            callback?.Invoke();
            PoolManager.Pools["Effect"].Despawn(trans);
        };
    }

    public void OnHealing(long hpAmount, long hpvalue, long hpMax)
    {
        Transform effect = PoolManager.Pools["Effect"].Spawn(healingEffect.transform);
        effect.position = transform.position;
        effect.parent = transform;
        outline.SetActive(false);
        effect.GetComponent<VisualEffect>().Play();

        DamagePopup.Create(transform.position, hpAmount, PopupType.Bonus, () =>
        {
            effect.GetComponent<VisualEffect>().Stop();
            PoolManager.Pools["Effect"].Despawn(effect);
        });
        healthText.text = (hpvalue < hpMax ? hpvalue : hpMax).ToString();
        UpdateHeroInfo(hp: (hpvalue < hpMax ? hpvalue : hpMax));
        HandleNetData.QueueNetData(NetData.CARD_UPDATE_MATRIX, new CardUpdateHeroMatrix() { battleId = battleID, heroId = this.heroID, own = cardOwner, heroInfo = this.heroInfoTmp});
        this.hpValue = hpvalue;
    }

    public void OnBuffEffect(long atkAmount, long hpAmount, long atkValue, long hpValue, long hpMax)
    {   
        Transform effect = PoolManager.Pools["Effect"].Spawn(buffEffect.transform);
        effect.position = new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z);
        effect.parent = transform;

        effect.GetComponent<VisualEffect>().Play();
        outline.SetActive(false);

        DamagePopup.Create(healthText.transform.position, hpAmount, PopupType.Bonus);
        DamagePopup.Create(damageText.transform.position, atkAmount, PopupType.Bonus, () =>
        {
            effect.GetComponent<VisualEffect>().Stop();
            PoolManager.Pools["Effect"].Despawn(effect);
        });
        healthText.text = (hpValue < hpMax ? hpValue : hpMax).ToString();
        damageText.text = atkValue.ToString();
        UpdateHeroInfo(atk: atkValue, hp: (hpValue < hpMax ? hpValue : hpMax));
        HandleNetData.QueueNetData(NetData.CARD_UPDATE_MATRIX, new CardUpdateHeroMatrix() { battleId = battleID, heroId = this.heroID, own = this.cardOwner, heroInfo = this.heroInfoTmp });
        this.hpValue = hpValue;
    }

    public void OnAddSpecialBuff(long skillID, long cleaveAdd, long pierceAdd, long breakerAdd, long comboAdd, long overrunAdd, long shieldAdd, long godSlayerAdd, long cleave, long pierce, long breaker, long combo, long overrun, long shield, long godSlayer)
    {
        outline.SetActive(false);

        if (shieldAdd > 0)
        {
            if (shieldSkeletonEffect == null)
            {
                Transform trans = PoolManager.Pools["Effect"].Spawn(shieldEffect.transform, shieldSpawnPosition);
                trans.localPosition = Vector3.zero;
                trans.localRotation = Quaternion.Euler(Vector3.zero);
                trans.localScale = new Vector3(0.85f, 0.45f, 0.6f);
                shieldSkeletonEffect = trans;
            }
            else
            {
                shieldSkeletonEffect.gameObject.SetActive(true);
                shieldSkeletonEffect.localPosition = Vector3.zero;
                shieldSkeletonEffect.rotation = Quaternion.Euler(Vector3.zero);
                shieldSkeletonEffect.localScale = new Vector3(0.85f, 0.45f, 0.6f);

            }
            shieldValue = shield;
        }
        else
        {
            Transform spawnObject = null;

            if (cleaveAdd > 0)
            {
                spawnObject = addCleaveEffect;
                DamagePopup.Create(transform.position, cleaveAdd, PopupType.Bonus, () =>
                {
                });
                cleaveValue = cleave;
                UpdateHeroInfo(cleave: cleaveValue);
            }
            if (pierceAdd > 0)
            {
                spawnObject = addPierceEffect;
                DamagePopup.Create(transform.position, pierceAdd, PopupType.Bonus, () =>
                {
                });
                pierceValue = pierce;

                UpdateHeroInfo(pierce: pierceValue);
            }
            if (breakerAdd > 0)
            {
                spawnObject = addBreakerEffect;
                DamagePopup.Create(transform.position, breakerAdd, PopupType.Bonus, () =>
                {
                });
                breakerValue = breaker;

                UpdateHeroInfo(breaker: breakerValue);
            }
            if (comboAdd > 0)
            {
                spawnObject = addComboEffect;
                DamagePopup.Create(transform.position, comboAdd, PopupType.Bonus, () =>
                {
                });
                comboValue = combo;

                UpdateHeroInfo(combo: comboValue);
            }
            if (overrunAdd > 0)
            {
                spawnObject = addOverrunEffect;
                DamagePopup.Create(transform.position, overrunAdd, PopupType.Bonus, () =>
                {
                });
                overrunValue = overrun;

                UpdateHeroInfo(overrun: overrunValue);
            }
            if (godSlayerAdd > 0)
            {
                spawnObject = addGodSlayerEffect;
                DamagePopup.Create(transform.position, godSlayerAdd, PopupType.Bonus, () =>
                {
                });
                godSlayerValue = godSlayer;
                UpdateHeroInfo(godSlayer: godSlayer);
            }
            Transform trans = PoolManager.Pools["Effect"].Spawn(spawnObject);
            trans.position = transform.position;
            trans.GetComponent<ParticleEffectParentCallback>().SetOnPlay();
            if (keywordInfo != null)
            {
                string kwUpdate = UpdateHeroKeywordInfo(this.heroInfoTmp);
                if (!string.IsNullOrEmpty(kwUpdate))
                {
                    keywordInfo.gameObject.SetActive(true);
                    keywordInfo.GetChild(0).GetComponent<TextMeshProUGUI>().text = kwUpdate;
                }
                else
                {
                    keywordInfo.gameObject.SetActive(false);
                }
            }
        }
    }
    private string UpdateHeroKeywordInfo(DBHero hero)
    {
        string descriptUpdate = "";
        if (hero != null)
        {
            //x.Contains("<link=\"overrun\">") || x.Contains("<link=\"fraglie\">") || x.Contains("<link=\"combo\">")
            //|| x.Contains("<link=\"breaker\">") || x.Contains("<link=\"cleave\">") || x.Contains("<link=\"godslayer\">") || x.Contains("<link=\"pierce\">")

            if (hero.combo > 0)
            {
                string txt = "<link=\"combo\"><sprite=10></link>";
                descriptUpdate += txt;
            }
            if (hero.overrun > 0)
            {
                string txt = "<link=\"overrun\"><sprite=1></link>";
                descriptUpdate += txt;
            }
            if (hero.pierce > 0)
            {
                string txt = "<link=\"pierce\"><sprite=16></link>";
                descriptUpdate += txt;
            }
            if (hero.breaker > 0)
            {
                string txt = "";
                if (hero.breaker == 1)
                {
                    txt = "<link=\"breaker\"><sprite=9></link>";
                }
                else if (hero.breaker == 2)
                {
                    txt = "<link=\"breaker\"><sprite=4></link>";
                }
                else if (hero.breaker == 3)
                {
                    txt = "<link=\"breaker\"><sprite=19></link>";
                }
                else
                {
                    txt = "<link=\"breaker\"><sprite=20></link>";
                }
                descriptUpdate += txt;
            }
            if (hero.godSlayer > 0)
            {
                string txt = "";
                if (hero.godSlayer == 1)
                {
                    txt = "<link=\"godslayer\"><sprite=17></link>";
                }
                else
                {
                    txt = "<link=\"godslayer\"><sprite=18></link>";
                }
                descriptUpdate += txt;
            }
            if (hero.cleave > 0)
            {
                string txt = "";
                if (hero.cleave == 1)
                {
                    txt = "<link=\"cleave\"><sprite=6></link>";
                }
                else if (hero.cleave == 2)
                {
                    txt = "<link=\"cleave\"><sprite=5></link>";
                }
                else
                {
                    txt = "<link=\"cleave\"><sprite=3></link>";
                }
                descriptUpdate += txt;
            }
            if (hero.isFragile)
            {
                // db dang chua co fragile

                //string txt = "<link=\"fraglie\"><sprite=7></link>";
                //descriptUpdate += txt;
                //Transform trans = PoolManager.Pools["Skill"].Spawn(keyWordFullPrefab);
                //trans.SetParent(keyWordContainer);
                //trans.localScale = Vector3.one;
                //trans.GetComponent< KeywordInfo>().InitData(txt, "Fragile", hero.cleave);
            }
        }
        return descriptUpdate;
    }

    public void UpdateHeroMatrix(long atk, long hp, long hpMax, long cleave, long pierce, long breaker, long combo, long overrun, long shield, long godSlayer, long fragile = 0, long precide = 0,List<long> lstCardSkillUnlock = null)
    {
        atkValue = atk;
        hpValue = hp;
        hpMaxValue = hpMax;
        cleaveValue = cleave;
        pierceValue = pierce;
        breakerValue = breaker;
        comboValue = combo;
        overrunValue = overrun;
        shieldValue = shield;
        //countShardAddded = shard;
        isFragile = fragile == 1;
        godSlayerValue = godSlayer;

        //update heroinfo
        // thieu shard free
        UpdateHeroInfo(atk, hp, cleave, pierce, breaker, combo, overrun, godSlayer, fragile, -1, -1, lstCardSkillUnlock );
        DBHero newh = Database.GetHero(heroID);

        HandleNetData.QueueNetData(NetData.CARD_UPDATE_MATRIX, new CardUpdateHeroMatrix() { battleId = battleID, heroId = this.heroID, own = this.cardOwner, heroInfo = this.heroInfoTmp });

        //if (countShardAddded > 0)
        //{
        //    for (int i = 0; i < lstShardAdded.Count; i++)
        //    {
        //        if (i < countShardAddded)
        //        {
        //            lstShardAdded[i].gameObject.SetActive(true);
        //            lstShardAdded[i].sprite = CardData.Instance.GetShardSprite(heroInfo.color); //CardColor.Instance.cardColorInfo[(int)heroInfo.color].shardColorSprite;
        //        }
        //        else
        //            break; 
        //    }
        //}
        if (shieldValue > 0)
        {
            if (shieldSkeletonEffect == null)
            {
                Transform trans = PoolManager.Pools["Effect"].Spawn(shieldEffect, shieldSpawnPosition);
                trans.localPosition = Vector3.zero;
                trans.localRotation = Quaternion.Euler(Vector3.zero);
                trans.localScale = new Vector3(0.21f, 0.23f, 0.21f);
                //shieldSkeletonEffect = trans.GetComponent<ParticleSystem>();
                shieldSkeletonEffect = trans;
            }
            if (!shieldSkeletonEffect.gameObject.activeSelf)
                shieldSkeletonEffect.gameObject.SetActive(true);
            //shieldSkeletonEffect.Play();
        }
        else
        {
            if (shieldSkeletonEffect != null && shieldSkeletonEffect.gameObject.activeSelf)
                shieldSkeletonEffect.gameObject.SetActive(false);
        }

        if (healthText != null)
            healthText.text = hpValue.ToString();
        if (damageText != null)
            damageText.text = atkValue.ToString();
        if (keywordInfo != null)
        {
            string kwUpdate = UpdateHeroKeywordInfo(this.heroInfoTmp);
            if (!string.IsNullOrEmpty(kwUpdate))
            {
                keywordInfo.gameObject.SetActive(true);
                keywordInfo.GetChild(0).GetComponent<TextMeshProUGUI>().text = kwUpdate;
            }
            else
            {
                keywordInfo.gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region Movement
    public override void UpdatePosition()
    {
        initPosition = slot.transform.position;
        initRotation = transform.rotation;
    }
    // Check is single target when active skill
    private bool IsSingleTarget(SkillState skState)
    {
        if (skState == SkillState.TWO_ANY_ENEMIES || skState == SkillState.TWO_ANY_ALLIES || skState == SkillState.TWO_ANY_ALLIES_BUT_SELF || skState == SkillState.TWO_ANY_ALLIES_JUNGLE_LAW)
            return false;
        return true;
    }
#if UNITY_STANDALONE || UNITY_EDITOR
    public override void OnMouseDown()
    {
        ShowOutlineTouch(true);
        transform.DOScale(new Vector3(1.07f, 1.07f, 1.07f), 0.01f).SetEase(Ease.OutElastic);
        base.OnMouseDown();
        
        if (IsPointerOverUIObject())
            return;
        if (GameBattleScene.instance == null)
        {
            isHold = true;
            if (BattleSceneTutorial.instance.IsYourTurn && cardOwner == CardOwner.Player)
                isTouch = true;

            else
                isTouch = false;
        } 
        else
        {   
            
        if (GameBattleScene.instance.skillState != SkillState.None)
        {
            if (IsSingleTarget(GameBattleScene.instance.skillState))
            {
                if (GameBattleScene.instance.lstSelectedSkillBoardCard.Count < 1)
                {
                    if (canSelect)
                        onAddToListSkill?.Invoke(this);
                }
                //else
                //{
                //    onRemoveFromListSkill?.Invoke(this);
                //}
            }
            else
            {
                if (GameBattleScene.instance.lstSelectedSkillBoardCard.Count < 2)
                {
                    if (canSelect)
                    {    
                        onAddToListSkill?.Invoke(this);
                        canSelect = false;
                    }
                    //else
                    //{
                    //    onRemoveFromListSkill?.Invoke(this);
                    //    canSelect = true;
                    //}
                }
                //else
                //{
                //   // onRemoveFromListSkill?.Invoke(this);
                //    canSelect = true;
                //}
            }
        }
        else
        {
            isHold = true;
            if (GameBattleScene.instance.IsYourTurn && cardOwner == CardOwner.Player)
                isTouch = true;

            else
                isTouch = false;
        }
        }
    }

    public override void OnMouseDrag()
    {
        if (isDragging)
            return;
        if (GameBattleScene.instance == null)
        {
            return;
        }
        base.OnMouseDrag();
        if (cardOwner == CardOwner.Enemy)
            return;
        if (!isTouch)
            return;

        if (isMoving)
            return;
        if (isHold)
            return;
        if (Time.time - currentClickTime < 0.3f)
            return;
        if (isSelected)
        {
            Selected();
        }
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit/*, Mathf.Infinity, ~layerMask*/))
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject != gameObject)
                {
                    CreateCloneOnDragging();
                    isDragging = true;
                    isSelected = true;
                    UIManager.instance.ClosePreviewHandCard();
                }
                if (GameBattleScene.instance.skillState == SkillState.None && isSelected)
                {
                    foreach (CardSlot slot in GameBattleScene.instance.ChooseSelfAnyBlank())
                    {
                        if(slot.yPos != 3)
                            slot.HighLightSlot();
                    }
                }
            }
        }
    }
    public override void OnMouseUp()
    {
        ShowOutlineTouch(false);
        transform.DOScale(Vector3.one, 0.01f).SetEase(Ease.OutElastic);
        isHold = false;
        holdTimeToPreview = 0;
        if(GameBattleScene.instance != null)
             UIManager.instance.ClosePreviewHandCard();
        else
            TutorialController.instance.ClosePreviewHandCard();
        base.OnMouseUp();
        Placed();
        if (cardOwner == CardOwner.Enemy)
            return;
        if (GameBattleScene.instance == null)
            return;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<CardSlot>() != null)
                {
                    if (hit.collider.GetComponent<CardSlot>().type != SlotType.Enemy)
                    {
                        CardSlot targetSlot = hit.collider.GetComponent<CardSlot>();
                        if (!isTired && targetSlot != slot)
                        {
                            // move god in prepare phase
                            if (!GameBattleScene.instance.isGameStarted)
                                GameBattleScene.instance.MoveGodInReadyPhase(this, slot.xPos, slot.yPos, targetSlot.xPos, targetSlot.yPos);
                            else
                                GameBattleScene.instance.MoveCardInBattlePhase(this, slot.xPos, slot.yPos, targetSlot.xPos, targetSlot.yPos);
                        }
                    }
                }
            }
        }

        if (newCardClone != null)
        {
            PoolManager.Pools["Card"].Despawn(newCardClone);
            newCardClone.GetComponent<CardOnBoardClone>().cloneSlot = null;
            newCardClone = null;
        }
        foreach (CardSlot slot in GameBattleScene.instance.playerSlotContainer)
            slot.UnHighLightSlot();
        isDragging = false;
    }
#else
    public override void OnTouchDown()
    {
    
        ShowOutlineTouch(true);
        transform.DOScale(new Vector3(1.07f, 1.07f, 1.07f),0.01f).SetEase(Ease.OutElastic);
        base.OnTouchDown();

        if (IsPointerOverUIObject())
            return;
        if (GameBattleScene.instance == null)
            return;

        if (GameBattleScene.instance.skillState != SkillState.None)
        {
            if (IsSingleTarget(GameBattleScene.instance.skillState))
            {
                if (GameBattleScene.instance.lstSelectedSkillBoardCard.Count < 1)
                {
                    if (canSelect)
                        onAddToListSkill?.Invoke(this);
                }
                //else
                //{
                //    onRemoveFromListSkill?.Invoke(this);
                //}
            }
            else
            {
                if (GameBattleScene.instance.lstSelectedSkillBoardCard.Count < 2)
                {
                    if (canSelect)
                    {
                        onAddToListSkill?.Invoke(this);
                        canSelect = false;
                    }
                    //else
                    //{
                    //    onRemoveFromListSkill?.Invoke(this);
                    //    canSelect = true;
                    //}
                }
                //else
                //{
                //   // onRemoveFromListSkill?.Invoke(this);
                //    canSelect = true;
                //}
            }
        }
        else
        {
            isHold = true;
            if (GameBattleScene.instance.IsYourTurn && cardOwner == CardOwner.Player)
                isTouch = true;
            else
                isTouch = false;
            
        }
    }
    public override void OnTouchMove()
    {
        if (isDragging)
            return;
       
        base.OnTouchMove();
        if (cardOwner == CardOwner.Enemy)
            return;
        if (!isTouch)
            return;
       
        if (isSelected)
        {
            Selected();
        }
        
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(GameBattleScene.instance.touch.position);
        if (Physics.Raycast(ray, out hit/*, Mathf.Infinity, ~layerMask*/))
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject != gameObject)
                {
                    CreateCloneOnDragging();
                    
                    isDragging = true;
                    isSelected = true;
                    UIManager.instance.ClosePreviewHandCard();
                }
                if (GameBattleScene.instance.skillState == SkillState.None && isSelected)
                {
                    foreach (CardSlot slot in GameBattleScene.instance.ChooseSelfAnyBlank())
                    {
                        if(slot.yPos != 3)
                            slot.HighLightSlot();
                    }
                }

            }
        }
    }
    
    public override void OnTouchEnd()
    {
    
        ShowOutlineTouch(false);
        transform.DOScale(Vector3.one, 0.01f).SetEase(Ease.OutElastic);
        isHold = false;
        holdTimeToPreview = 0;
        if(UIManager.instance != null)
            UIManager.instance.ClosePreviewHandCard();
        //if (GameBattleScene.instance == null)
        //{
        //    Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit1;
            
        //    isDragging = false;
        //    isTouch = false;
        //    Interested(false);
        //    MoveFail();
        //    return;
        //}
        base.OnTouchEnd();
        Placed();
        if (cardOwner == CardOwner.Enemy)
            return;
        if (!isTouch)
            return;
            
        if (GameBattleScene.instance == null)
            return;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<CardSlot>() != null)
                {
                    if (hit.collider.GetComponent<CardSlot>().type != SlotType.Enemy)
                    {
                        CardSlot targetSlot = hit.collider.GetComponent<CardSlot>();
                        if (!isTired && targetSlot != slot)
                        {
                            // move god in prepare phase
                            if (!GameBattleScene.instance.isGameStarted)
                                GameBattleScene.instance.MoveGodInReadyPhase(this, slot.xPos, slot.yPos, targetSlot.xPos, targetSlot.yPos);
                            else
                                GameBattleScene.instance.MoveCardInBattlePhase(this, slot.xPos, slot.yPos, targetSlot.xPos, targetSlot.yPos);
                        }
                    }
                }
            }
        }

        if (newCardClone != null)
        {
            PoolManager.Pools["Card"].Despawn(newCardClone);
            newCardClone.GetComponent<CardOnBoardClone>().cloneSlot = null;
            newCardClone = null;
        }
        foreach (CardSlot slot in GameBattleScene.instance.playerSlotContainer)
            slot.UnHighLightSlot();
        isDragging = false;
    }
#endif
    private void OnDragging()
    {
        if (GameBattleScene.instance == null)
            return;
        if (!GameBattleScene.instance.IsYourTurn)
            return;
        if (!isDragging)
            return;
        if (heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
            return;

#if UNITY_STANDALONE || UNITY_EDITOR
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#elif UNITY_ANDROID
        Ray ray = Camera.main.ScreenPointToRay(GameBattleScene.instance.touch.position);
#endif

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider != null)
            {
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
                            if (slot != currentSelectedCardSlot & slot)
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
    }

    public void MoveToSlot(CardSlot targetSlot)
    {
        if (targetSlot == slot)
            return;

        if (targetSlot != null)
        {
            Selected();
            this.slot.state = SlotState.Empty;
            slot = targetSlot;
            slot.ChangeSlotState(SlotState.Full, this);
            if(godLinkEffect != null)
                godLinkEffect.gameObject.SetActive(false);
            MoveTo(targetSlot.transform.position, 0.2f, () =>
            {
                this.transform.rotation = targetSlot.transform.rotation;
                UpdatePosition();
                Placed();
                if (godLinkEffect != null)
                {
                    godLinkEffect.gameObject.SetActive(true);
                    godLinkEffect.SetPosition(this.transform, godLinkEffect.endPoint);
                }
            });
        }
    }    
    public void ShowOutlineTouch(bool isShow)
    {
        if (isShow)
            outlineTouchGO.SetActive(true);
        else
            outlineTouchGO.SetActive(false);
    }
    public void ShowHideShadow(bool isShow)
    {
        if (isShow)
            shadowGO.SetActive(true);
        else
            shadowGO.SetActive(false);
    }
    #endregion
}
