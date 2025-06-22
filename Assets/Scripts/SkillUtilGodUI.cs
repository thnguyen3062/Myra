using GIKCore.Bundle;
using GIKCore.Lang;
using GIKCore.Net;
using GIKCore.Sound;
using GIKCore.UI;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SkillUtilGodUI : GameListener
{
    private Card card;
    private long heroId;
    private DBHeroSkill skillU;
    [SerializeField] private Image printImg;
    [SerializeField] private GameObject outlineTouchGO;
    [SerializeField] private GameObject canUse;
    [SerializeField] private GameObject activated;
    [SerializeField] private GameObject unusable;
    [SerializeField] private GameObject countImg;
    [SerializeField] private Image imgFill;
    [SerializeField] private Transform unlockUltiVfxPrefab;
    [SerializeField] private Transform effPosition;
    private bool isTouch;
    private float countTime;
    private bool isHold;

    private void Start()
    {
        countImg.SetActive(false);
        countTime = 0;
        canUse.SetActive(false);
        effPosition.gameObject.SetActive(false);
        activated.SetActive(false);
        unusable.SetActive(false);
        if (GameBattleScene.instance != null)
        {
            GameBattleScene.instance.isUseUltimate += OnActivatedUlti;
            GameBattleScene.instance.onGameBattleSimulation += GameBattleSimulation;
        }
    }
    public void GameBattleSimulation(bool isPlayer, long roundCount,long turnMana)
    {
        if (isPlayer)
        {
            if (CheckHeroActiveSkill())
            {
                canUse.SetActive(true);
            }    
            else
                canUse.SetActive(false);
        }
        else
        {
            if (canUse != null && canUse.activeInHierarchy)
                canUse.SetActive(false);
        }
    }
    private void OnUpdateShard(int index, long shard)
    {
        if (index == 0)
        {
            if (GameBattleScene.instance.IsYourTurn)
            {
                if (CheckHeroActiveSkill())
                {
                    if (!canUse.activeSelf)
                    {
                        ////voice ultimate chung 
                        //    //SoundHandler.main.PlaySFX("Ultimate","voicegod");
                        //Transform trans = PoolManager.Pools["Effect"].Spawn(unlockUltiVfxPrefab);
                        //trans.SetParent(this.effPosition);
                        //trans.localPosition = Vector3.zero;
                        //trans.localScale = Vector3.one;
                        //trans.position = Vector3.zero;

                        //trans.GetComponent<ParticleEffectParentCallback>().SetOnComplete(() =>
                        //{
                        //    PoolManager.Pools["Effect"].Despawn(trans);
                        //});
                        effPosition.gameObject.SetActive(true);
                    }
                    canUse.SetActive(true);

                }    
                else
                    canUse.SetActive(false);
            }
        }
    }
    public override bool ProcessNetData(int id, object data)
    {
        if (base.ProcessNetData(id, data)) return true;
        switch (id)
        {
            case NetData.Card_UPDATE_ULTI_STAGE:
                {
                    if (GameBattleScene.instance == null)
                        break;
                    CardUltiStage cus = (CardUltiStage)data;
                    if (cus.heroId == heroId)
                    {
                        if (cus.stage == 0)
                        {
                            if (CheckHeroActiveSkill())
                            {
                                if(!canUse.activeSelf)
                                {

                                    SoundHandler.main.PlaySFX("UltimateUnlocked", "sounds");
                                    //Transform trans = PoolManager.Pools["Effect"].Spawn(unlockUltiVfxPrefab);

                                    //trans.SetParent(this.effPosition);
                                    //trans.localPosition = Vector3.zero;
                                    //trans.localScale = Vector3.one;

                                    //trans.position = Vector3.zero;
                                    //Debug.Log(trans.position);

                                    //trans.GetComponent<ParticleEffectParentCallback>().SetOnComplete(() =>
                                    //{
                                    //    PoolManager.Pools["Effect"].Despawn(trans);
                                    //});
                                    effPosition.gameObject.SetActive(true);
                                }    
                                canUse.SetActive(true);
                            }    
                            else
                                canUse.SetActive(false);

                        }
                        if (GameData.main.isUsedUlti)
                        {
                            if (GameData.main.ultiID == heroId)
                            {
                                activated.SetActive(true);
                                canUse.SetActive(false);
                            }
                            else
                            {
                                unusable.SetActive(true);
                                canUse.SetActive(false);
                            }

                        }
                    }
                    break;
                }
        }
        return false;
    }

    // Start is called before the first frame update
    public void InitData(DBHeroSkill s, long heroId)
    {
        // lay ra tu db heroskill
        //coler
        //set callback on click
        skillU = s;
        this.heroId = heroId;

        //Sprite ultiSprite = CardData.Instance.GetUltiSpriteButton(s.id.ToString());
        //printImg.sprite = ultiSprite;
        //canUse.GetComponent<Image>().sprite = ultiSprite;
        //unusable.GetComponent<Image>().sprite = ultiSprite;
        //activated.GetComponent<Image>().sprite = CardData.Instance.GetUltiSpriteButton("A_"+s.id.ToString());
    }
    public void ChangeState(int index)
    {
        // ch?a ?? ?i?u ki?n sd
        //?? ?k sd
        // ?ã sd 
        if (index == 1)
        {
            activated.SetActive(true);
            canUse.SetActive(false);
        }
        if (index == 2)
        {
            unusable.SetActive(true);
            canUse.SetActive(false);
        }
    }
    private bool CheckHeroActiveSkill()
    {
        //change mode skill can active
        if (skillU != null)
        {
            if (!GameBattleScene.instance.isGameStarted || !GameBattleScene.instance.IsYourTurn)
                return false;
            if (GameData.main.isUsedUlti)
                return false;
            //bool shardOk = CheckShard();
            //if (!shardOk)
            //{
            //    return false;
            //}
            if (skillU.isUltiType && !GameData.main.isUsedUlti)
            {
                return true;
            }
            else if (skillU.isUltiType && GameData.main.isUsedUlti)
            {
                //@Tony
                //card.SetSkillState(false);
                Toast.Show(LangHandler.Get("82", "Ultimate already activated for this match"));
            }
            if (!skillU.isUltiType && card.countDoActiveSkill == 0)
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
            else if (!skillU.isUltiType && card.countDoActiveSkill != 0)
            {
                Toast.Show(LangHandler.Get("146", "You can use active skill once per turn"));
            }
        }
        else
        {
        }

        return false;
    }
    private void OnActivatedUlti(long heroid)
    {
        
        //CheckHeroSkill(TYPE_WHEN_SUMON, card,slot.xPos,slot.yPos);
        if (heroid == this.heroId)
        {
            string voiceName = Database.GetHero(this.heroId).heroNumber.ToString() + "_" + VoiceData.TYPE_ULTIMATE_ACTIVATED + "_" + Random.Range(1, 3).ToString();
            AudioClip clip = BundleHandler.LoadSound(voiceName, "voicegod");
            if (clip != null)
                SoundHandler.main.PlaySFX(voiceName, "voicegod");
            ChangeState(1);
        }    
        else
            ChangeState(2);
    }
    public void PointerDown()
    {
        isTouch = true;
        ShowOutlineTouch(true);
    }
    public void PoiterUp()
    {
        isHold = false;
        isTouch = false;
        countTime = 0;
        imgFill.fillAmount = 0;
        countImg.SetActive(false);
        ShowOutlineTouch(false);
    }
    public void OnClick()
    {
        ShowOutlineTouch(false);
        if (!isHold && countTime < 0.2f)
        {
            //hien thi tool tip 
        }
    }
    public void ActiveSkill()
    {
        //neu du dieu kien
        BoardCard currentGod = GameBattleScene.instance.lstCardInBattle.FirstOrDefault(w => w.heroID == heroId);
        if (currentGod != null)
        {
            currentGod.OnActiveSkill(skillU);
        }
    }
    public void ShowOutlineTouch(bool isShow)
    {
        if (isShow)
            outlineTouchGO.SetActive(true);
        else
            outlineTouchGO.SetActive(false);
    }
    bool CheckShard()
    {
        bool isOk = false;

        BoardCard currentGod = GameBattleScene.instance.lstCardInBattle.FirstOrDefault(w => w.heroID == heroId && w.cardOwner == CardOwner.Player);

        if (currentGod != null)
        {
            //if (currentGod.countShardAddded >= skillU.min_shard)
            //{
            //    if (skillU.max_shard == -1)
            //        isOk = true;
            //    else if (card.countShardAddded <= skillU.max_shard)
            //        isOk = true;
            //    foreach (ConditionSkill cond in skillU.lstConditionSkill)
            //    {
            //        if (cond.type == 19)
            //        {
            //            if (cond.number <= GameBattleScene.instance.currentShard)
            //                isOk = true;
            //            else
            //                isOk = false;
            //        }
            //    }
            //}

        }
        else
            isOk = false;
        return isOk;
    }
    private void Update()
    {
        if (!isTouch)
            return;
        if (!GameBattleScene.instance.isGameStarted || !GameBattleScene.instance.IsYourTurn)
            return;
        countTime += Time.deltaTime;
        if (countTime > 0.2f)
        {
            countImg.SetActive(true);
            imgFill.fillAmount = countTime / 2;
            if (countTime >= 2f && isHold == false)
            {
                isHold = true;
                ActiveSkill();
            }

        }

    }
}
