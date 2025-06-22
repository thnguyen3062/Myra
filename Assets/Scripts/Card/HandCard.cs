using DG.Tweening;
using GIKCore.Lang;
using GIKCore.Sound;
using GIKCore.UI;
using GIKCore.Utilities;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandCard : Card
{
    #region Serialized Fields
    [SerializeField] private Animator animator;
    [Header("Spell Object")]
    [SerializeField] private GameObject spellObject;
    [SerializeField] private MeshRenderer spellPrint;
    [SerializeField] private MeshRenderer spellColor;
    [SerializeField] private GameObject[] spellFrame;
    [SerializeField] private GameObject[] spellRarity;
    [SerializeField] private List<MeshRenderer> lstSpellShards;
    [SerializeField] private TextMeshPro spellNameText;
    [SerializeField] private TextMeshPro spellDescription;
    [SerializeField] private Transform spellRangeEff;
    [SerializeField] private Material normalMatSpellPrint;
    [SerializeField] private Material normalMatSpellFrame;

    [Header("Minion Object")]
    [SerializeField] private GameObject minionObject;
    [SerializeField] private MeshRenderer minionPrint;
    [SerializeField] private MeshRenderer minionColor;
    [SerializeField] private GameObject[] minionFrame;
    [SerializeField] private GameObject[] minionRarity;
    [SerializeField] private List<MeshRenderer> lstMinionShards;
    [SerializeField] private TextMeshPro minionNameText;
    [SerializeField] private TextMeshPro minionRegionText;
    [SerializeField] private TextMeshPro minionDamageText;
    [SerializeField] private TextMeshPro minionHealthText;
    [SerializeField] private TextMeshPro minionDescription;
    [SerializeField] private Material normalMatMinionPrint;
    [SerializeField] private Material normalMatMinionFrame;

    [Header("Others")]
    [SerializeField] private TextMeshPro manaText;
    [SerializeField] private TextMeshPro idText;
    [SerializeField] private MeshRenderer outline;
    [SerializeField] private Material minionOutlineBlackMaterial;
    [SerializeField] private Material spellOutlineBlackMaterial;
    [SerializeField] private Material minionOutlineGlow;
    [SerializeField] private Material spellOutlineGlow;
    [SerializeField] private BoxCollider m_HandCardCollider;
    [SerializeField] private Transform premiumEffects;
    [SerializeField] private GameObject m_DestroyEffect;
    [SerializeField] protected Transform m_SpawnPositionEffect;
    [SerializeField] protected Transform m_EffectSkillOnYourCard;
    #endregion

    #region HideInspector Fields
    [HideInInspector] public long tmpMana = 0;
    [HideInInspector] public long tmpHp = 0;
    [HideInInspector] public long tmpAtk = 0;
    [HideInInspector] public bool isFleeting;
    [HideInInspector] public bool shardCondition = false;
    #endregion

    #region Private Fields
    private Transform highlightSpellRange;
    private bool spellTutSummon = false;
    #endregion

    #region Actions
    public event ICallback.CallFunc2<HandCard> onAddToListSkill;
    //public event ICallback.CallFunc2<HandCard> onRemoveFromListSkill;
    public event ICallback.CallFunc2<HandCard> onEndSkillActive;
    #endregion

    #region Unity Methods
    private void Start()
    {
        if (GameBattleScene.instance != null)
        {
            GameBattleScene.instance.onUpdateMana += OnUpdateMana;
            GameBattleScene.instance.onFinishChooseOneTarget += OnEndOneEffectSkillActive;
            GameBattleScene.instance.onEndSkillActive += OnEndSkillActive;
            GameBattleScene.instance.onGameConfirmStartBattle += OnGameConfirmStartBattle;
            GameBattleScene.instance.onGameBattleSimulation += GameBattleSimulation;
            //GameBattleScene.instance.onUpdateShard += OnUpdateShard;
        }
    }

    private void OnEnable()
    {
        m_HandCardCollider.enabled = false;
        manaText.gameObject.SetActive(false);
        m_DestroyEffect.SetActive(false);
    }
    #endregion

    public void SetHandCardData(long battleID, long id, long frame, CardOwner owner, long atk, long hp, long mana)
    {
        SetCardData(battleID, id, frame, owner);
        OnUpdateManaText(mana);
        OnUpdateAtkText(atk);
        OnUpdateHpText(hp);
    }
    private void FixedUpdate()
    {
        OnDragging();
    }

    public override void SetCardData(long battleID, long id, long frame, CardOwner owner, long atk = -1, long hp = -1, long mana = -1)
    {
        base.SetCardData(battleID, id, frame, owner);

        if (outline != null)
            outline.material = heroInfo.type == DBHero.TYPE_TROOPER_NORMAL ? minionOutlineBlackMaterial : spellOutlineBlackMaterial;
        if (tmpMana == -1)
            OnUpdateManaText(heroInfo.mana);
        Texture cardTexture = CardData.Instance.GetOnBoardTexture(heroInfo.id);
        //Texture shardTexture = CardData.Instance.GetShardTexture(heroInfo.color);
        manaText.gameObject.SetActive(cardOwner == CardOwner.Player);
        idText.text = heroID.ToString();
        CardDataInfo info = CardData.Instance.GetCardDataInfo(heroInfo.id);
        string txt = "";
        if (info != null)
        {
            info.description.ForEach(x =>
            {
                //if (x.Contains("<link=\"overrun\">") || x.Contains("<link=\"fraglie\">") || x.Contains("<link=\"combo\">") || x.Contains("<link=\"breaker\">") || x.Contains("<link=\"cleave\">") || x.Contains("<link=\"godslayer\">") || x.Contains("<link=\"pierce\">"))
                //{
                //    x = "<size=4>" + x + "</size>";
                //}
                if (x.Contains("<link=\"overrun\">") || x.Contains("<link=\"fraglie\">") || x.Contains("<link=\"combo\">") || x.Equals("<link=\"breaker\">") || x.Contains("<link=\"cleave\">") || x.Contains("<link=\"godslayer\">") || x.Contains("<link=\"pierce\">"))
                {
                    x = "<size=2>" + x + "</size>";
                }
                txt += x;
            });
            txt = txt.Replace("\\n", "\n");
        }
        txt += UpdateHeroInfo(heroInfo);
        minionObject.SetActive(heroInfo.type == DBHero.TYPE_TROOPER_NORMAL);
        if (heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
            spellObject.SetActive(true);
        else
            spellObject.SetActive(false);

        if (heroInfo.type == DBHero.TYPE_TROOPER_NORMAL)
            SetMinionData(cardTexture, info.name, txt);
        else if (heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
            SetSpellData(cardTexture, info.name, txt);
        m_HandCardCollider.enabled = true;
    }

    private void SetSpellData(Texture cardTexture, string cardName, string description)
    {
        //if (outline != null)
        //    outline.material = spellOutlineGlow;
        foreach (GameObject go in spellRarity)
            go.SetActive(false);
        spellRarity[heroInfo.rarity - 1].SetActive(true);
        Texture cardFrame = CardData.Instance.GetCardFrameTexture("Spell_" + frameC + "_" + /*hero.rarity*/1);
        Texture frameMask = CardData.Instance.GetCardFrameMaskTexture("M_" + heroInfo.type + "_" + /*hero.rarity*/1);
        Texture flareMask = CardData.Instance.GetCardFrameMaskTexture("T_FlareMask");


        //if (heroInfo.rarity > 3)
        //{
        //    spellFrame[1].SetActive(true);
        //    spellFrame[1].GetComponent<MeshRenderer>().material.SetTexture("_print_img", cardFrame);
        //    spellFrame[1].GetComponent<MeshRenderer>().material.SetTexture("FrameMask", frameMask);
        //    spellFrame[1].GetComponent<MeshRenderer>().material.SetTexture("_FlareMask", flareMask);
        //    spellFrame[0].SetActive(false);
        //    if (frameC >= 3)
        //    {
        //        //set frame
        //        var mat = spellFrame[1].GetComponent<MeshRenderer>().material;
        //        Material matframe = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_" + heroInfo.type + "_" + frameC)) as Material;
        //        mat = matframe;
        //        //mat.shader = Shader.Find("Shader Graps/MetalFlareShader-Speed");
        //        //Texture main = CardData.Instance.GetCardFrameTexture("Spell_" + frameC + "_" + heroInfo.rarity);
        //        Texture main = CardData.Instance.GetCardFrameSprite("Spell_" + frameC + "_" + /*hero.rarity*/1).texture;
        //        //mat.SetTexture("Texture2D_08500469d2944895a9ad392692f50c3c", main);
        //        mat.SetTexture("_MainTex", main);
        //        //Texture mask = CardData.Instance.GetCardFrameMaskTexture("M_" + heroInfo.type + "_" + heroInfo.rarity);
        //        Texture mask = CardData.Instance.GetCardFrameMaskSprite("M_" + heroInfo.type + "_" + /*hero.rarity*/1).texture;
        //        //mat.SetTexture("Texture2D_17ff7d92927445598a03023feb129259", mask);
        //        mat.SetTexture("_FrameMask", mask);
        //        mat.SetTexture("_FlareMask", flareMask);
        //        spellFrame[1].GetComponent<MeshRenderer>().material = mat;
        //        //spellFrame[1].GetComponent<MeshRenderer>().material.SetFloat("TotalDuration", 1);
        //        //premiumEffects.GetChild((int)frameC - 3).gameObject.SetActive(true);
        //        //printf ??ng
        //        spellPrint.material = Instantiate(CardData.Instance.GetAnimatedMaterial("M_" + heroID)) as Material;
        //        spellPrint.material.shader = Shader.Find("ReadyPrefab/Shader3.14");
        //    }
        //    else
        //    {
        //        spellFrame[1].GetComponent<MeshRenderer>().material.SetFloat("TotalDuration", 0);
        //        //print th??ng
        //        spellPrint.material.SetTexture("_BaseMap", cardTexture);
        //    }
        //}
        //else
        //{

        //high vs low dang chung mat
            spellFrame[1].SetActive(false);
            spellFrame[0].SetActive(true);
            spellFrame[0].GetComponent<MeshRenderer>().material = normalMatSpellFrame;
            spellPrint.material = normalMatSpellPrint;
            spellFrame[0].GetComponent<MeshRenderer>().material.SetTexture("_print_img", cardFrame);
            spellFrame[0].GetComponent<MeshRenderer>().material.SetTexture("FrameMask", frameMask);
            spellFrame[0].GetComponent<MeshRenderer>().material.SetTexture("_FlareMask", flareMask);
            if (frameC >= 3)
            {
                var mat = spellFrame[0].GetComponent<MeshRenderer>().material;
                Material matframe = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_" + heroInfo.type + "_" + frameC)) as Material;
                mat = matframe;
                //Shader.Find("Shader Graps/MetalFlareShader-Speed");
                //Texture main = CardData.Instance.GetCardFrameTexture("Spell_" + frameC + "_" + heroInfo.rarity);
                Texture main = CardData.Instance.GetCardFrameSprite("Spell_" + frameC + "_" + /*hero.rarity*/1).texture;
                //mat.SetTexture("Texture2D_08500469d2944895a9ad392692f50c3c", main);
                mat.SetTexture("_MainTex", main);
                //Texture mask = CardData.Instance.GetCardFrameMaskTexture("M_" + heroInfo.type + "_" + heroInfo.rarity);
                Texture mask = CardData.Instance.GetCardFrameMaskSprite("M_" + heroInfo.type + "_" + /*hero.rarity*/1).texture;
                //mat.SetTexture("Texture2D_17ff7d92927445598a03023feb129259", mask);
                mat.SetTexture("_FrameMask", mask);

                mat.SetTexture("_FlareMask", flareMask);
                spellFrame[0].GetComponent<MeshRenderer>().material = mat;
                //spellFrame[0].GetComponent<MeshRenderer>().material.SetFloat("TotalDuration", 1);
                premiumEffects.GetChild((int)frameC - 3).gameObject.SetActive(true);
                //printf ??ng
                spellPrint.material = Instantiate(CardData.Instance.GetAnimatedMaterial("M_" + heroID)) as Material;
                spellPrint.material.shader = Shader.Find("ReadyPrefab/Shader3.14");
            }
            else
            {
                spellFrame[0].GetComponent<MeshRenderer>().material.SetFloat("TotalDuration", 0);
                //print th??ng
                spellPrint.material.SetTexture("_BaseMap", cardTexture);
            }
        //}

        Texture cardColor = CardData.Instance.GetCardColorTexture("Spell_" + heroInfo.color + "_" + /*hero.rarity*/1);
        spellColor.material.SetTexture("_BaseMap", cardColor);
        spellNameText.text = cardName;
        spellDescription.text = description;
        //for (int i = 0; i < lstSpellShards.Count; i++)
        //{
        //    if (i < heroInfo.shardRequired)
        //    {
        //        lstSpellShards[i].gameObject.SetActive(true);  
        //        lstSpellShards[i].material.SetTexture("_BaseMap", shardTexture);
        //    }
        //    else
        //    {
        //        lstSpellShards[i].gameObject.SetActive(false);
        //    }
        //}
    }

    private void SetMinionData(Texture cardTexture, string cardName, string description)
    {
        //if (outline != null)
        //    outline.material = minionOutlineGlow;
        for (int j = 0; j < minionRarity.Length; j++)
        {
            minionRarity[j].gameObject.SetActive((j + 1) == heroInfo.rarity);
        }
        foreach (GameObject go in minionRarity)
            go.SetActive(false);
        minionRarity[heroInfo.rarity - 1].SetActive(true);
        Texture cardFrame = CardData.Instance.GetCardFrameTexture("Mortal_" + frameC + "_" + /*hero.rarity*/1);
        Texture frameMask = CardData.Instance.GetCardFrameMaskTexture("M_" + heroInfo.type + "_" + /*hero.rarity*/1);
        Texture flareMask = CardData.Instance.GetCardFrameMaskSprite("T_FlareMask").texture;
        //if (heroInfo.rarity > 3)
        //{
        //minionFrame[1].SetActive(true);
        //minionFrame[1].GetComponent<MeshRenderer>().material.SetTexture("_print_img", cardFrame);
        //minionFrame[1].GetComponent<MeshRenderer>().material.SetTexture("FrameMask", frameMask);
        //minionFrame[1].GetComponent<MeshRenderer>().material.SetTexture("_FlareMask", flareMask);
        //minionFrame[0].SetActive(false);
        //if (frameC >= 3)
        //{
        //    var mat = minionFrame[1].GetComponent<MeshRenderer>().material;
        //    Material matframe = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_" + heroInfo.type + "_" + frameC)) as Material;
        //    mat = matframe;
        //    //Shader.Find("Shader Graps/MetalFlareShader-Speed");
        //    //Texture main = CardData.Instance.GetCardFrameTexture("Mortal_" + frameC + "_" + heroInfo.rarity);
        //    Texture main = CardData.Instance.GetCardFrameSprite("Mortal_" + frameC + "_" + /*hero.rarity*/1).texture;
        //    //mat.SetTexture("Texture2D_08500469d2944895a9ad392692f50c3c", main);
        //    mat.SetTexture("_MainTex", main);
        //    //Texture mask = CardData.Instance.GetCardFrameMaskTexture("M_" + heroInfo.type + "_" + heroInfo.rarity);
        //    Texture mask = CardData.Instance.GetCardFrameMaskSprite("M_" + heroInfo.type + "_" +/*hero.rarity*/1).texture;
        //    //mat.SetTexture("Texture2D_17ff7d92927445598a03023feb129259", mask);
        //    mat.SetTexture("_FrameMask", mask);

        //    mat.SetTexture("_FlareMask", flareMask);
        //    minionFrame[1].GetComponent<MeshRenderer>().material = mat;
        //    //minionFrame[1].GetComponent<MeshRenderer>().material.SetFloat("TotalDuration", 1);
        //    premiumEffects.GetChild((int)frameC - 3).gameObject.SetActive(true);
        //    //printf ??ng
        //    minionPrint.material = Instantiate(CardData.Instance.GetAnimatedMaterial("M_" + heroID)) as Material;
        //    minionPrint.material.shader = Shader.Find("ReadyPrefab/Shader3.14");
        //    minionPrint.material.SetVector("_TileOff", new Vector4(0.83f, 1.1f, 0.1f, 0.05f));
        //}
        //else
        //{
        //    minionFrame[1].GetComponent<MeshRenderer>().material.SetFloat("TotalDuration", 0);
        //    //print th??ng
        //    minionPrint.material.SetTexture("_BaseMap", cardTexture);
        //}
        //}
        //else
        //{
        // hi?n t?i high vs low ?ang dùng chung 1 frame
        minionFrame[1].SetActive(false);
        minionFrame[0].SetActive(true);
        minionPrint.material = normalMatMinionPrint;
        minionFrame[0].GetComponent<MeshRenderer>().material = normalMatMinionFrame;
        minionFrame[0].GetComponent<MeshRenderer>().material.SetTexture("_print_img", cardFrame);
        minionFrame[0].GetComponent<MeshRenderer>().material.SetTexture("FrameMask", frameMask);
        minionFrame[0].GetComponent<MeshRenderer>().material.SetTexture("_FlareMask", flareMask);
        if (frameC >= 3)
        {
            var mat = minionFrame[0].GetComponent<MeshRenderer>().material;
            Material matframe = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_" + heroInfo.type + "_" + frameC)) as Material;
            mat = matframe;
            //mat.shader = Shader.Find("Shader Graps/Metal Flare Shader-Speed");
            //Texture main = CardData.Instance.GetCardFrameTexture("Mortal_" + frameC + "_" + heroInfo.rarity);
            Texture main = CardData.Instance.GetCardFrameSprite("Mortal_" + frameC + "_" + /*hero.rarity*/1).texture;
            //mat.SetTexture("Texture2D_08500469d2944895a9ad392692f50c3c", main);
            mat.SetTexture("_MainTex", main);
            //Texture mask = CardData.Instance.GetCardFrameMaskTexture("M_" + heroInfo.type + "_" + heroInfo.rarity);
            Texture mask = CardData.Instance.GetCardFrameMaskSprite("M_" + heroInfo.type + "_" + /*hero.rarity*/1).texture;
            //mat.SetTexture("Texture2D_17ff7d92927445598a03023feb129259", mask);
            mat.SetTexture("_FrameMask", mask);

            mat.SetTexture("_FlareMask", flareMask);
            minionFrame[0].GetComponent<MeshRenderer>().material = mat;

            //minionFrame[0].GetComponent<MeshRenderer>().material.SetFloat("TotalDuration", 1);
            premiumEffects.GetChild((int)frameC - 3).gameObject.SetActive(true);
            minionPrint.material = Instantiate(CardData.Instance.GetAnimatedMaterial("M_" + heroID)) as Material;
            minionPrint.material.shader = Shader.Find("ReadyPrefab/Shader3.14");
            minionPrint.material.SetVector("_TileOff", new Vector4(0.83f, 1.1f, 0.1f, -0.05f));
        }
        else
        {
            minionFrame[0].GetComponent<MeshRenderer>().material.SetFloat("TotalDuration", 0);
            //print th??ng
            minionPrint.material.SetTexture("_BaseMap", cardTexture);
        }
        //}
        Texture cardColor = CardData.Instance.GetCardColorTexture("Minion_" + heroInfo.color);
        minionColor.material.SetTexture("_BaseMap", cardColor);
        minionDamageText.text = heroInfoTmp.atk.ToString();
        minionHealthText.text = heroInfoTmp.hp.ToString();
        minionRegionText.text = heroInfoTmp.cardRegionDict[heroInfo.speciesId];
        minionNameText.text = cardName;
        minionDescription.text = description;
        //for (int i = 0; i < lstMinionShards.Count; i++)
        //{
        //    if (i < heroInfo.shardRequired)
        //    {
        //        lstMinionShards[i].gameObject.SetActive(true);
        //        lstMinionShards[i].material.SetTexture("_BaseMap", shardTexture);
        //    }
        //    else
        //    {
        //        lstMinionShards[i].gameObject.SetActive(false);
        //    }
        //}
    }
    private string UpdateHeroInfo(DBHero hero)
    {
        string descriptUpdate = "";
        if (hero != null)
        {
            //x.Contains("<link=\"overrun\">") || x.Contains("<link=\"fraglie\">") || x.Contains("<link=\"combo\">")
            //|| x.Contains("<link=\"breaker\">") || x.Contains("<link=\"cleave\">") || x.Contains("<link=\"godslayer\">") || x.Contains("<link=\"pierce\">")

            if (hero.combo > 0)
            {
                string txt = "<link=\"combo\"><sprite=8></link>";
                descriptUpdate += txt;
            }
            if (hero.overrun > 0)
            {
                string txt = "<link=\"overrun\"><sprite=0></link>";
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
                    txt = "<link=\"breaker\"><sprite=12></link>";
                }
                else if (hero.breaker == 2)
                {
                    txt = "<link=\"breaker\"><sprite=11></link>";
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
                    txt = "<link=\"cleave\"><sprite=15></link>";
                }
                else if (hero.cleave == 2)
                {
                    txt = "<link=\"cleave\"><sprite=14></link>";
                }
                else
                {
                    txt = "<link=\"cleave\"><sprite=13></link>";
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
                //trans.GetComponent<KeywordInfo>().InitData(txt, "Fragile", hero.cleave);
            }
        }
        return descriptUpdate;
    }
    public void OnUpdateManaText(long mana)
    {
        manaText.text = mana.ToString();
        tmpMana = mana;
        if (heroInfoTmp != null)
        {
            UpdateHeroInfo(manaCost: mana);
        }
    }
    public void OnUpdateAtkText(long atk)
    {
        minionDamageText.text = atk.ToString();
        tmpAtk = atk;
        if (heroInfoTmp != null)
        {
            UpdateHeroInfo(atk: atk);
        }
    }
    public void OnUpdateHpText(long hp)
    {
        minionHealthText.text = hp.ToString();
        tmpHp = hp;
        if (heroInfoTmp != null)
        {
            UpdateHeroInfo(hp: hp);
        }
    }
    private void OnUpdateMana(int index, long mana, ManaState state, long usedMana)
    {
        if (cardOwner == CardOwner.Enemy)
            return;
        if (index == 0)
        {
            if (GameBattleScene.instance.IsYourTurn)
            {
                if (GameBattleScene.instance.CheckCardCanUseCondition(this))
                {
                    HighlighCard();
                    canSelect = false;
                }
                else
                    UnHighlighCard();
            }
        }
    }
    //private void OnUpdateShard(int index, long shard)
    //{
    //    if (cardOwner == CardOwner.Enemy)
    //        return;
    //    if (index == 0)
    //    {
    //        if (GameBattleScene.instance.IsYourTurn)
    //        {
    //            if (GameBattleScene.instance.CheckCardCanUseCondition(this))
    //            {
    //                HighlighCard();
    //                canSelect = false;
    //            }
    //            else
    //                UnHighlighCard();
    //        }
    //    }
    //}

    public override void OnGameConfirmStartBattle()
    {

        base.OnGameConfirmStartBattle();
        MoveFail();

    }
    public void GameBattleSimulation(bool isPlayer, long roundCount,long turnMana)
    {
        if (cardOwner == CardOwner.Enemy)
            return;
        if (isPlayer)
        {

            if (GameBattleScene.instance.CheckCardCanUseCondition(this))
            {
                HighlighCard();
                canSelect = false;
            }
            else
                UnHighlighCard();
        }
        else
        {
            UnHighlighCard();
        }
    }

    public void HighlighCard()
    {
        base.HighlightUnit();
        outline.material = heroInfo.type == DBHero.TYPE_TROOPER_NORMAL ? minionOutlineGlow : spellOutlineGlow;
        canSelect = true;
    }

    public void UnHighlighCard()
    {
        base.UnHighlightUnit();
        if (heroInfo != null)
            outline.material = heroInfo.type == DBHero.TYPE_TROOPER_NORMAL ? minionOutlineBlackMaterial : spellOutlineBlackMaterial;
        canSelect = false;
    }
    public override void OnEndSkillActive()
    {
        UnHighlighCard();
        MoveFail();
        onEndSkillActive?.Invoke(this);
        if (GameBattleScene.instance != null)
        {
            if (GameBattleScene.instance.IsYourTurn && this.cardOwner == CardOwner.Player)
            {
                if (GameBattleScene.instance.CheckCardCanUseCondition(this))
                {
                    HighlighCard();
                    canSelect = false;
                }
                else
                    UnHighlighCard();
            }
        }
    }
    public void OnEndOneEffectSkillActive()
    {
        UnHighlighCard();
        onEndSkillActive?.Invoke(this);
        if (GameBattleScene.instance != null)
        {
            if (GameBattleScene.instance.IsYourTurn && this.cardOwner == CardOwner.Player)
            {
                if (GameBattleScene.instance.CheckCardCanUseCondition(this))
                {
                    HighlighCard();
                    canSelect = false;
                }
                else
                    UnHighlighCard();
            }
        }
    }
    public void DrawTo(Vector3 to, Transform screenPoint, float twistForThisCard = 0, ICallback.CallFunc complete = null)
    {
        transform.DOKill();
        if (cardOwner == CardOwner.Player)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOMove(screenPoint.position, 0.4f))
                .Insert(0.15f, transform.DORotate(new Vector3(0, 0, 90), 0.2f))
                .Insert(0.35f, transform.DORotate(Vector3.zero, 0.15f))
                .AppendInterval(0.2f)
                .Append(transform.DOMove(to, 0.4f))
                .Insert(0.6f, transform.DORotate(new Vector3(0, -30, 0), 0.2f))
                .Insert(0.8f, transform.DORotate(new Vector3(0, -twistForThisCard, 0), 0.2f))
                .OnComplete(() =>
                {
                    complete?.Invoke();
                });
        }
        else
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOMove(to, 0.4f))
                .Insert(0.1f, transform.DORotate(new Vector3(0, -150f, 180), 0.2f))
                .Insert(0.3f, transform.DORotate(new Vector3(0, -180f, 180f), 0.1f))
                .OnComplete(() =>
                {
                    complete?.Invoke();
                });
        }
    }

    public void ShowEnemyCardOnScreen(Transform screenPoint, ICallback.CallFunc callback = null)
    {
        manaText.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(screenPoint.position, 0.2f))
            .Append(transform.DORotate(new Vector3(15, 0, 180f), 0.1f))
            .Append(transform.GetChild(0).transform.DOLocalRotate(new Vector3(0, 0, 0), 0.2f))
            .AppendInterval(2f)
            .OnComplete(() =>
            {
                callback?.Invoke();
            });
    }
    public void ShowSpellEnemyCardOnScreen(Transform screenPoint, ICallback.CallFunc callback = null)
    {
        manaText.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(screenPoint.position, 0.2f).SetEase(Ease.OutCirc))
            .Append(transform.DORotate(new Vector3(15, 0, 180f), 0.3f).SetEase(Ease.OutCirc))
            .Join(transform.GetChild(0).transform.DOLocalRotate(new Vector3(0, 0, 0), 0.3f).SetEase(Ease.OutCirc))
            .Append(transform.DOBlendableMoveBy(new Vector3(0, 0.05f, 0.01f), 2f).SetEase(Ease.OutSine))
            ./*OnComplete(() =>
            {
                callback?.Invoke();
            }).*/InsertCallback(1.5f, () =>
                 {
                     callback?.Invoke();
                 })
            .Append(transform.DOMove(new Vector3(5f, 0f, 0f), 0.5f).SetEase(Ease.InCirc))
            .OnComplete(() =>
            {
                PoolManager.Pools["Card"].Despawn(this.transform);
            });
    }
    public void DiscardHandCardPlayer(Transform screenPoint, ICallback.CallFunc callback = null)
    {
        isMoving = true;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(screenPoint.position, 0.2f).SetEase(Ease.OutCirc))
            .Append(transform.DORotate(new Vector3(15, 0, 180f), 0.3f).SetEase(Ease.OutCirc))
            .Join(transform.GetChild(0).transform.DOLocalRotate(new Vector3(0, 0, 0), 0.3f).SetEase(Ease.OutCirc))
            .InsertCallback(1, () =>
            {
                m_DestroyEffect.SetActive(true);
            })
            .InsertCallback(2f, () =>
                 {
                     callback?.Invoke();
                     PoolManager.Pools["Card"].Despawn(this.transform);
                 });
    }    
    public void DrawCardPlayer(Transform screenPoint, ICallback.CallFunc callback = null)
    {
        isMoving = true;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(screenPoint.position, 0.2f).SetEase(Ease.OutCirc))
            .Append(transform.DORotate(new Vector3(15, 0, 180f), 0.3f).SetEase(Ease.OutCirc))
            .Join(transform.GetChild(0).transform.DOLocalRotate(new Vector3(0, 0, 0), 0.3f).SetEase(Ease.OutCirc))
            .Append(transform.DOBlendableMoveBy(new Vector3(0, 0.05f, 0.01f), 1.5f).SetEase(Ease.OutSine))
            ./*OnComplete(() =>
            {
                callback?.Invoke();
            }).*/InsertCallback(1.5f, () =>
                 {
                     callback?.Invoke();
                 }).InsertCallback(3f, () =>
                  {
                      if (GameBattleScene.instance != null && !GameBattleScene.instance.haveCardToDelete)
                      {
                          foreach (HandCard c in GameBattleScene.instance.Decks[0].GetListCard)
                              c.isMoving = false;
                      }
                  });

    }
    public void ScaleCard(bool isBigger)
    {
        if (isBigger)
            transform.GetChild(0).DOScale(transform.GetChild(0).localScale * 2.9f, 0.5f).SetEase(Ease.OutBounce);
        else
            MoveBack();
    }
    public void ShowEffectSkillBid()
    {
        //có th? sau dung cho ca skill k phai bid
        Transform trans = PoolManager.Pools["Effect"].Spawn(m_EffectSkillOnYourCard, m_SpawnPositionEffect);
        trans.localPosition = Vector3.zero;
        trans.localScale = Vector3.one;
        trans.GetComponent<ParticleEffectCallback>().SetOnComplete(() =>
        {
            PoolManager.Pools["Effect"].Despawn(trans);
        });
    }    
    #region Movement
    public override void UpdatePosition()
    {
        initPosition = transform.position;
        initRotation = transform.rotation;
    }
    private void ResetPositionBeforeTween()
    {
        transform.GetChild(0).localScale = Vector3.one;
        transform.position = initPosition;
        transform.rotation = initRotation;
    }    
#if UNITY_STANDALONE || UNITY_EDITOR

    public override void OnMouseOver()
    {
        if (isMoving)
            return;
        if (cardOwner == CardOwner.Enemy)
            return;
        if (isInteracted)
            return;
        if (isDragging)
            return;
        if (isTouch)
            return;
        if (IsPointerOverUIObject())
            return;
        base.OnMouseOver();
        if (!isMoving)
        {
            ResetPositionBeforeTween();
            //transform.DOMove(transform.position + new Vector3(0, 0.5f, 0), 0.05f);
            transform.DOMove(transform.position - transform.forward * 1.9f, 0.05f);
            transform.GetChild(0).DOScale(transform.GetChild(0).localScale * 2.9f, 0.05f).SetEase(Ease.OutBounce);
            Interested(true);
            SoundHandler.main.PlaySFX("SelectCard", "sounds");
            if (TutorialController.instance != null)
                TutorialController.instance.HideTutBox();
        }
    }

    public override void OnMouseExit()
    {
        if (isMoving)
            return;
        if (cardOwner == CardOwner.Enemy)
            return;
        if (isSelected)
            return;
        if (isDragging)
            return;
        if (isTouch)
            return;
        if (cursor != null)
            return;
        base.OnMouseExit();
        MoveBack(0.05f);
        Interested(false);
    }

    public override void OnMouseDown()
    {
        if (GameBattleScene.instance == null)
            return;
        if (cardOwner == CardOwner.Enemy)
            return;
        base.OnMouseDown();
        if (IsPointerOverUIObject())
            return;
        if (isMoving)
            return;
        if (GameBattleScene.instance.haveCardToDelete)
            return;
        if (GameBattleScene.instance.skillState != SkillState.None)
        {
            if (GameBattleScene.instance.lstSelectedSkillHandCard.Count < 1)
            {
                if (canSelect)
                {
                    onAddToListSkill?.Invoke(this);
                    //  transform.DOMove(transform.position - transform.forward * 0.3f, 0.05f);
                    isSelected = true;
                }
            }
        }
        else
        {
            if (GameBattleScene.instance.IsYourTurn && cardOwner == CardOwner.Player)
            {
                isTouch = true;
            }
            else
                isTouch = false;
        }
    }

    public override void OnMouseDrag()
    {
        if (isDragging)
            return;

        if (GameBattleScene.instance == null)
        {
            bool canDrag = false;
            Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit1;
            if (TutorialController.instance.m_TutorialID == 0)
            {
                if (TutorialController.instance.index == 7)
                {
                    canDrag = true;
                }
                else if (TutorialController.instance.index == 12)
                {
                    canDrag = true;
                }
                else if (TutorialController.instance.index == 20)
                {
                    canDrag = true;
                }

            }
            else
            {

                if (TutorialController.instance.index == 5)
                {
                    canDrag = true;
                }
                else if (TutorialController.instance.index == 15)
                {
                    canDrag = true;
                }
                else if (TutorialController.instance.index == 17)
                {
                    canDrag = true;
                }
                else if (TutorialController.instance.index == 18 || TutorialController.instance.index == 20)
                {
                    canDrag = true;
                }
                else if (TutorialController.instance.index == 27)
                {
                    canDrag = true;
                }
            }
            if (canDrag)
            {
                if (Physics.Raycast(ray1, out hit1/*, Mathf.Infinity, ~layerMask*/))
                {
                    if (hit1.collider != null)
                    {
                        if (hit1.collider.gameObject != gameObject)
                        {
                            transform.GetChild(0).DOScale(Vector3.one, 0.05f);
                            //transform.position = transform.position + transform.forward * 0.2f;
                            isSelected = true;
                            transform.DOMove(initPosition + new Vector3(0, 0.3f, -0.3f), 0.01f).onComplete += delegate
                            {
                                isDragging = true;
                                CreateCloneOnDraggingTutorial();
                                animator.SetBool("isDrag", true);
                            };
                            if (heroInfo.type != DBHero.TYPE_TROOPER_MAGIC && heroInfo.type != DBHero.TYPE_BUFF_MAGIC && isSelected)
                            {
                                if (TutorialController.instance.m_TutorialID == 0)
                                {
                                    foreach (CardSlot slot in BattleSceneTutorial.instance.ChooseSelfAnyBlank())
                                    {
                                        if (slot.yPos != 3 && slot.yPos != 2)
                                        {
                                            slot.HighLightSlot();

                                        }
                                    }
                                }
                                else
                                {


                                    //if (TutorialController.instance.index == 5)
                                    //{
                                    //    foreach (CardSlot slot in BattleSceneTutorial.instance.ChooseSelfAnyBlank())
                                    //    {
                                    //        if (slot.xPos == 1 && slot.yPos == 0)
                                    //        {
                                    //            slot.HighLightSlot();
                                    //        }
                                    //    }
                                    //}
                                    //else if (TutorialController.instance.index == 15)
                                    //{
                                    //    foreach (CardSlot slot in BattleSceneTutorial.instance.ChooseSelfAnyBlank())
                                    //    {
                                    //        if (slot.yPos == 2)
                                    //        {
                                    //            slot.HighLightSlot();
                                    //        }
                                    //    }
                                    //}
                                    //else if (TutorialController.instance.index == 17)
                                    //{
                                    //    foreach (CardSlot slot in BattleSceneTutorial.instance.ChooseSelfAnyBlank())
                                    //    {
                                    //        slot.HighLightSlot();
                                    //    }
                                    //}
                                    //else if (TutorialController.instance.index == 18 || TutorialController.instance.index == 20)
                                    //{
                                    //    foreach (CardSlot slot in BattleSceneTutorial.instance.ChooseSelfAnyBlank())
                                    //    {
                                    //        if (slot.yPos == 2)
                                    //            slot.HighLightSlot();
                                    //    }
                                    //}

                                    foreach (CardSlot slot in BattleSceneTutorial.instance.ChooseSelfAnyBlank())
                                    {
                                        if(slot.yPos !=3)
                                           slot.HighLightSlot();
                                    }
                                }
                            }
                            else
                            {
                                if (highlightSpellRange == null && (heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || heroInfo.type == DBHero.TYPE_BUFF_MAGIC))
                                {
                                    Transform eff = PoolManager.Pools["Effect"].Spawn(spellRangeEff, BattleSceneTutorial.instance.spellRangePos);
                                    highlightSpellRange = eff;
                                    highlightSpellRange.localPosition = Vector3.zero;
                                    highlightSpellRange.localScale = Vector3.one;
                                    highlightSpellRange.localRotation = Quaternion.Euler(Vector3.zero);
                                    // highlightSpellRange.GetComponent<Animation>().Play("HighlightBlue");
                                }
                                TutorialController.instance.HideTutBox();
                            }
                        }

                    }

                }
            }
            return;
        }
        if (cardOwner == CardOwner.Enemy)
            return;
        if (!isTouch)
            return;
        if (Time.time - currentClickTime < 0.3f)
            return;
        if (GameBattleScene.instance.skillState != SkillState.None)
            return;
        if (!GameBattleScene.instance.IsYourTurn)
            return;
        base.OnMouseDrag();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit/*, Mathf.Infinity, ~layerMask*/))
        {
            if (hit.collider != null)
            {  
                if (hit.collider.gameObject != gameObject)
                {
                    transform.GetChild(0).DOScale(Vector3.one, 0.05f);
                    //transform.position = transform.position + transform.forward * 0.2f;
                    isSelected = true;
                    transform.DOMove(initPosition + new Vector3(0, 0.3f, -0.3f), 0.01f).onComplete += delegate
                    {
                        isDragging = true;
                        CreateCloneOnDragging();
                        animator.SetBool("isDrag", true);
                    };

                }
                if (heroInfo.type != DBHero.TYPE_TROOPER_MAGIC && heroInfo.type != DBHero.TYPE_BUFF_MAGIC && isSelected)
                {
                    foreach (CardSlot slot in GameBattleScene.instance.ChooseSelfAnyBlank())
                    {
                        if (slot.yPos != 3)
                            slot.HighLightSlot();
                    }
                }
                else
                {
                    if (highlightSpellRange == null && (heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || heroInfo.type == DBHero.TYPE_BUFF_MAGIC))
                    {
                        Transform eff = PoolManager.Pools["Effect"].Spawn(spellRangeEff, GameBattleScene.instance.spellRangePos);
                        highlightSpellRange = eff;
                        highlightSpellRange.localPosition = Vector3.zero;
                        highlightSpellRange.localScale = Vector3.one;
                        highlightSpellRange.localRotation = Quaternion.Euler(Vector3.zero);
                        // highlightSpellRange.GetComponent<Animation>().Play("HighlightBlue");
                    }
                }
            }

        }

    }
    public override void OnMouseUp()
    {
        if (GameBattleScene.instance == null)
        {
            transform.GetChild(0).DOScale(Vector3.one, 0.05f);
            Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit1;
            if (Physics.Raycast(ray1, out hit1, Mathf.Infinity, layerMask))
            {
                if (hit1.collider != null)
                {
                    CardSlot slot = hit1.collider.GetComponent<CardSlot>();
                    if (TutorialController.instance.m_TutorialID == 0)
                    {
                        if (TutorialController.instance.index == 20 && !spellTutSummon)
                        {
                            if (!BattleSceneTutorial.instance.Decks[0].GetDeckBound().IntersectRay(ray1) && isDragging && (newCardClone != null))
                            {
                                if (newCardClone != null)
                                    newCardClone.gameObject.SetActive(true);
                                BattleSceneTutorial.instance.EndDradSpell();
                                spellTutSummon = true;
                                if (cursor != null)
                                {
                                    PoolManager.Pools["Card"].Despawn(cursor.transform);
                                    cursor = null;
                                }
                                //this.UnHighlighCard();
                            }
                            else
                            {
                                MoveFail();
                            }
                        }
                        else if (slot != null)
                        {
                            if (TutorialController.instance.index == 7)
                            {
                                BattleSceneTutorial.instance.EndDragCard(slot);
                            }
                            else if (TutorialController.instance.index == 12)
                            {
                                BattleSceneTutorial.instance.EndDragCard(slot);
                            }
                            MoveFail();

                        }
                        else
                        {
                            MoveFail();
                        }
                    }
                    else
                    {
                        if ((TutorialController.instance.index == 18 || TutorialController.instance.index == 20) && !spellTutSummon && heroInfo.type == DBHero.TYPE_TROOPER_MAGIC)
                        {
                            if (!BattleSceneTutorial.instance.Decks[0].GetDeckBound().IntersectRay(ray1) && isDragging && (newCardClone != null))
                            {
                                if (newCardClone != null)
                                    newCardClone.gameObject.SetActive(true);
                                BattleSceneTutorial.instance.EndDradSpell();
                                spellTutSummon = true;
                                if (cursor != null)
                                {
                                    PoolManager.Pools["Card"].Despawn(cursor.transform);
                                    cursor = null;
                                }
                            }
                            else
                            {
                                MoveFail();
                            }
                        }
                        else if (slot != null && heroInfo.type != DBHero.TYPE_TROOPER_MAGIC)
                        {
                            if (TutorialController.instance.index == 5)
                            {
                                if (heroID != 107)
                                {
                                    Toast.Show(LangHandler.Get("745", "Not enough mana."));
                                    //warning : not enough mana  

                                }
                                else
                                    BattleSceneTutorial.instance.EndDragCard(slot);
                            }
                            if (TutorialController.instance.index == 15)
                            {
                                if (heroID != 30)
                                {
                                    Toast.Show("This might not be the best action. Let's try another card.");
                                }
                                else
                                    BattleSceneTutorial.instance.EndDragCard(slot);
                            }
                            if (TutorialController.instance.index == 17)
                            {
                                if (heroInfo.mana > BattleSceneTutorial.instance.currentMana)
                                    Toast.Show(LangHandler.Get("744", "Not enough mana to play"));
                            }
                            if (TutorialController.instance.index == 18 || TutorialController.instance.index == 20)
                            {
                                BattleSceneTutorial.instance.EndDragCard(slot);
                            }
                            if (TutorialController.instance.index == 27)
                            {
                                BattleSceneTutorial.instance.EndDragCard(slot);
                            }
                            MoveFail();
                        }
                        else
                        {
                            if (TutorialController.instance.index == 5)
                            {
                                if (heroID != 107)
                                {
                                    Toast.Show(LangHandler.Get("745", "Not enough mana."));
                                    //warning : not enough mana  

                                }
                            }
                            if (TutorialController.instance.index == 15)
                            {
                                if (heroID != 30)
                                {
                                    Toast.Show("This might not be the best action. Let's try another card.");
                                }
                            }
                            if (TutorialController.instance.index == 17)
                            {
                                if (heroInfo.mana > BattleSceneTutorial.instance.currentMana)
                                    Toast.Show(LangHandler.Get("744", "Not enough mana to play"));
                            }
                            MoveFail();
                        }

                    }
                }
                else
                    MoveFail();

                foreach (CardSlot slot in BattleSceneTutorial.instance.playerSlotContainer)
                    slot.UnHighLightSlot();
            }
            if (animator.GetBool("isDrag"))
            {
                animator.SetBool("isDrag", false);
                MoveBack(0.05f);
            }
            if (highlightSpellRange != null)
            {
                PoolManager.Pools["Effect"].Despawn(highlightSpellRange);
                highlightSpellRange = null;
            }
            isDragging = false;
            isTouch = false;
            Interested(false);
            if (isSelected)
            {
                isSelected = false;
            }
            return;
        }
        if (cardOwner == CardOwner.Enemy)
            return;
        base.OnMouseUp();
        if (!isTouch)
            return;
        if (GameBattleScene.instance.skillState != SkillState.None)
            return;
        if (GameBattleScene.instance.haveCardToDelete)
            return;
        if (animator.GetBool("isDrag"))
        {
            animator.SetBool("isDrag", false);
            MoveBack(0.05f);
        }
        if (highlightSpellRange != null)
        {
            PoolManager.Pools["Effect"].Despawn(highlightSpellRange);
            highlightSpellRange = null;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!GameBattleScene.instance.Decks[0].GetDeckBound().IntersectRay(ray) && GameBattleScene.instance.IsYourTurn)
        {
            if ((heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || heroInfo.type == DBHero.TYPE_BUFF_MAGIC) && newCardClone != null)
            {
                newCardClone.gameObject.SetActive(true);
                //if (!shardCondition)
                //    GameBattleScene.instance.WarningShardCondition();
                GameBattleScene.instance.SummonCardInBattlePhase(this, -1, -1);
            }
            else
            {
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.GetComponent<CardSlot>() != null)
                        {
                            if (hit.collider.GetComponent<CardSlot>().type != SlotType.Enemy && hit.collider.GetComponent<CardSlot>().state != SlotState.Full)
                            {
                                CardSlot targetSlot = hit.collider.GetComponent<CardSlot>();
                                GameBattleScene.instance.SummonCardInBattlePhase(this, targetSlot.xPos, targetSlot.yPos);
                            }
                            else
                                MoveFail();
                        }
                        else
                            MoveFail();

                    }
                }
            }
        }
        else
            MoveFail();

        if (isSelected)
        {
            isSelected = false;
        }
        isDragging = false;
        isTouch = false;
        Interested(false);
        foreach (CardSlot slot in GameBattleScene.instance.playerSlotContainer)
            slot.UnHighLightSlot();
    }
#else
    public override void OnTouchDown()
    {
        if (GameBattleScene.instance == null)
        {
            if (TutorialController.instance != null)
                TutorialController.instance.HideTutBox();
            return;
        }
        if (cardOwner == CardOwner.Enemy)
            return;
        base.OnTouchDown();
        if (IsPointerOverUIObject())
            return;
        if(isMoving)
            return;
        if (GameBattleScene.instance.skillState != SkillState.None)
        {
            if (GameBattleScene.instance.lstSelectedSkillHandCard.Count < 1)
            {
                if (canSelect)
                {
                    onAddToListSkill?.Invoke(this);
                    //  transform.DOMove(transform.position - transform.forward * 0.3f, 0.05f);
                    isSelected = true;
                }
            }
        }
        else
        {
            if (GameBattleScene.instance.IsYourTurn && cardOwner == CardOwner.Player)
            {
                isTouch = true;
            }
            else
                isTouch = false;
        }
    }
    public override void OnTouchHold()
    {
        //if (GameBattleScene.instance == null)
        //return;
        if (cardOwner == CardOwner.Enemy)
            return;
        if (isInteracted)
            return;
        if (IsPointerOverUIObject())
            return;
        base.OnTouchHold();
        if (!isMoving)
        {
            ResetPositionBeforeTween();
            transform.DOMove(transform.position - transform.forward * 1.9f, 0.05f);
            transform.GetChild(0).DOScale(transform.GetChild(0).localScale * 2.9f, 0.05f).SetEase(Ease.OutBounce);
            Interested(true);
            SoundHandler.main.PlaySFX("SelectCard", "sounds");
            if (TutorialController.instance != null)
                TutorialController.instance.HideTutBox();
        }
    }
    public override void OnTouchEnd()
    {
        if (GameBattleScene.instance == null)
        {

            if (TutorialController.instance != null)
                TutorialController.instance.HideTutBox();
            transform.GetChild(0).DOScale(Vector3.one, 0.05f);
            Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit1;
            if (Physics.Raycast(ray1, out hit1, Mathf.Infinity, layerMask))
            {
                if (hit1.collider != null)
                {
                    CardSlot slot = hit1.collider.GetComponent<CardSlot>();
                    if (TutorialController.instance.m_TutorialID == 0)
                    {
                        if (TutorialController.instance.index == 20 && !spellTutSummon)
                        {
                            if (!BattleSceneTutorial.instance.Decks[0].GetDeckBound().IntersectRay(ray1) && isDragging && (newCardClone != null))
                            {
                                if (newCardClone != null)
                                    newCardClone.gameObject.SetActive(true);
                                BattleSceneTutorial.instance.EndDradSpell();
                                spellTutSummon = true;
                                if (cursor != null)
                                {
                                    PoolManager.Pools["Card"].Despawn(cursor.transform);
                                    cursor = null;
                                }
                                //this.UnHighlighCard();
                            }
                            else
                            {
                                MoveFail();
                            }
                        }
                        else if (slot != null)
                        {
                            if (TutorialController.instance.index == 7)
                            {
                                BattleSceneTutorial.instance.EndDragCard(slot);
                            }
                            else if (TutorialController.instance.index == 12)
                            {
                                BattleSceneTutorial.instance.EndDragCard(slot);
                            }
                            MoveFail();

                        }
                        else
                        {
                            MoveFail();
                        }
                    }
                    else
                    {
                        if ((TutorialController.instance.index == 18 || TutorialController.instance.index == 20) && !spellTutSummon && heroInfo.type == DBHero.TYPE_TROOPER_MAGIC)
                        {
                            if (!BattleSceneTutorial.instance.Decks[0].GetDeckBound().IntersectRay(ray1) && isDragging && (newCardClone != null))
                            {
                                if (newCardClone != null)
                                    newCardClone.gameObject.SetActive(true);
                                BattleSceneTutorial.instance.EndDradSpell();
                                spellTutSummon = true;
                                if (cursor != null)
                                {
                                    PoolManager.Pools["Card"].Despawn(cursor.transform);
                                    cursor = null;
                                }
                            }
                            else
                            {
                                MoveFail();
                            }
                        }
                         else if (slot != null && heroInfo.type != DBHero.TYPE_TROOPER_MAGIC)
                        {
                            if (TutorialController.instance.index == 5)
                            {
                                if (heroID != 107)
                                {
                                    Toast.Show(LangHandler.Get("745", "Not enough mana."));
                                    //warning : not enough mana  

                                }
                                else
                                    BattleSceneTutorial.instance.EndDragCard(slot);
                            }
                            if (TutorialController.instance.index == 15)
                            {
                                if (heroID != 30)
                                {
                                    Toast.Show("This might not be the best action. Let's try another card.");
                                }
                                else
                                    BattleSceneTutorial.instance.EndDragCard(slot);
                            }
                            if (TutorialController.instance.index == 17)
                            {
                                if (heroInfo.mana > BattleSceneTutorial.instance.currentMana)
                                    Toast.Show(LangHandler.Get("744", "Not enough mana to play"));
                            }
                            if (TutorialController.instance.index == 18 || TutorialController.instance.index == 20)
                            {
                                BattleSceneTutorial.instance.EndDragCard(slot);
                            }
                            if (TutorialController.instance.index == 27)
                            {
                                BattleSceneTutorial.instance.EndDragCard(slot);
                            }
                            MoveFail();
                        }
                        else
                        {
                        if (TutorialController.instance.index == 5)
                            {
                                if (heroID != 107)
                                    Toast.Show(LangHandler.Get("745", "Not enough mana."));
                            }
                            if (TutorialController.instance.index == 15)
                            {
                                if (heroID != 30)
                                    Toast.Show("This might not be the best action. Let's try another card.");
                            }
                            if (TutorialController.instance.index == 17)
                            {
                                if (heroInfo.mana > BattleSceneTutorial.instance.currentMana)
                                    Toast.Show(LangHandler.Get("744", "Not enough mana to play"));
                            }
                            MoveFail();
                        }

                    }
                }
                else
                    MoveFail();

                foreach (CardSlot slot in BattleSceneTutorial.instance.playerSlotContainer)
                    slot.UnHighLightSlot();
            }
            if (animator.GetBool("isDrag"))
            {
                animator.SetBool("isDrag", false);
                MoveBack(0.05f);
            }
            if (highlightSpellRange != null)
            {
                PoolManager.Pools["Effect"].Despawn(highlightSpellRange);
                highlightSpellRange = null;
            }
            isDragging = false;
            isTouch = false;
            Interested(false);
            if (isSelected)
            {
                isSelected = false;
            }
            return;
        }

        if (cardOwner == CardOwner.Enemy)
            return;
        if (GameBattleScene.instance.haveCardToDelete)
            return;
        base.OnTouchEnd();
        if (!isTouch)
            return;
        if (GameBattleScene.instance.skillState != SkillState.None)
            return;
        if (animator.GetBool("isDrag"))
        {
            animator.SetBool("isDrag", false);
            MoveBack(0.05f);
        }
        if (highlightSpellRange != null)
        {
            PoolManager.Pools["Effect"].Despawn(highlightSpellRange);
            highlightSpellRange = null;
        }
        Ray ray = Camera.main.ScreenPointToRay(GameBattleScene.instance.touch.position);
        if (!GameBattleScene.instance.Decks[0].GetDeckBound().IntersectRay(ray) && GameBattleScene.instance.IsYourTurn)
        {
            if ((heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || heroInfo.type == DBHero.TYPE_BUFF_MAGIC) && newCardClone != null)
            {
                newCardClone.gameObject.SetActive(true);
                GameBattleScene.instance.SummonCardInBattlePhase(this, -1, -1);
                //if (!shardCondition)
                //   GameBattleScene.instance.WarningShardCondition();
            }
            else
            {
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.GetComponent<CardSlot>() != null)
                        {
                            if (hit.collider.GetComponent<CardSlot>().type != SlotType.Enemy && hit.collider.GetComponent<CardSlot>().state != SlotState.Full)
                            {
                                CardSlot targetSlot = hit.collider.GetComponent<CardSlot>();
                                GameBattleScene.instance.SummonCardInBattlePhase(this, targetSlot.xPos, targetSlot.yPos);
                            }
                            else
                                MoveFail();
                        }
                        else
                            MoveFail();

                    }
                }
            }
        }
        else
            MoveFail();

        if (isSelected)
        {
            isSelected = false;
        }
        isDragging = false;
        isTouch = false;
        Interested(false);
        foreach (CardSlot slot in GameBattleScene.instance.playerSlotContainer)
            slot.UnHighLightSlot();
    }
    public override void OnTouchMove()
    {
        if (isDragging)
            return;

        if (GameBattleScene.instance == null)
        {
            bool canDrag = false;
            Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit1;
            if (TutorialController.instance.m_TutorialID == 0)
            {
                if (TutorialController.instance.index == 7)
                {
                    canDrag = true;
                }
                else if (TutorialController.instance.index == 12)
                {
                    canDrag = true;
                }
                else if (TutorialController.instance.index == 20)
                {
                    canDrag = true;
                }

            }
            else
            {

                if (TutorialController.instance.index == 5)
                {
                    canDrag = true;
                }
                else if (TutorialController.instance.index == 15)
                {
                    canDrag = true;
                }
                else if (TutorialController.instance.index == 17)
                {
                    canDrag = true;
                }
                else if (TutorialController.instance.index == 18 || TutorialController.instance.index == 20)
                {
                    canDrag = true;
                }
                else if (TutorialController.instance.index == 27)
                {
                    canDrag = true;
                }
            }
            if (canDrag)
            {
                if (Physics.Raycast(ray1, out hit1/*, Mathf.Infinity, ~layerMask*/))
                {
                    if (hit1.collider != null)
                    {
                        if (hit1.collider.gameObject != gameObject)
                        {
                            transform.GetChild(0).DOScale(Vector3.one, 0.05f);
                            //transform.position = transform.position + transform.forward * 0.2f;
                            isSelected = true;

                            transform.DOMove(initPosition + new Vector3(0, 0.3f, -0.3f), 0.01f).onComplete += delegate
                            {
                                isDragging = true;
                                CreateCloneOnDraggingTutorial();
                                animator.SetBool("isDrag", true);
                            };
                            if (heroInfo.type != DBHero.TYPE_TROOPER_MAGIC && heroInfo.type != DBHero.TYPE_BUFF_MAGIC && isSelected)
                            {
                                if (TutorialController.instance.m_TutorialID == 0)
                                {
                                    foreach (CardSlot slot in BattleSceneTutorial.instance.ChooseSelfAnyBlank())
                                    {
                                        if (slot.yPos != 3 && slot.yPos != 2)
                                        {
                                            slot.HighLightSlot();

                                        }
                                    }
                                }
                                else
                                {
                                    foreach (CardSlot slot in BattleSceneTutorial.instance.ChooseSelfAnyBlank())
                                    {
                                        if(slot.yPos !=3)
                                           slot.HighLightSlot();
                                    }
                                }
                            }
                            else
                            {
                                if (highlightSpellRange == null && (heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || heroInfo.type == DBHero.TYPE_BUFF_MAGIC))
                                {
                                    Transform eff = PoolManager.Pools["Effect"].Spawn(spellRangeEff, BattleSceneTutorial.instance.spellRangePos);
                                    highlightSpellRange = eff;
                                    highlightSpellRange.localPosition = Vector3.zero;
                                    highlightSpellRange.localScale = Vector3.one;
                                    highlightSpellRange.localRotation = Quaternion.Euler(Vector3.zero);
                                    // highlightSpellRange.GetComponent<Animation>().Play("HighlightBlue");
                                }
                                TutorialController.instance.HideTutBox();
                            }
                        }

                    }
                }
            }
            return;
        }
        if (cardOwner == CardOwner.Enemy)
            return;
        if (!isTouch)
            return;
        if (Time.time - currentClickTime < 0.3f)
            return;
        if (GameBattleScene.instance.skillState != SkillState.None)
            return;
        if (!GameBattleScene.instance.IsYourTurn)
            return;
        base.OnTouchMove();
        Ray ray = Camera.main.ScreenPointToRay(GameBattleScene.instance.touch.position);

        if (!isTouch)
            return;


        RaycastHit hit;

        if (Physics.Raycast(ray, out hit/*, Mathf.Infinity, ~layerMask*/))
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject != gameObject)
                {
                    isSelected = true;
                    transform.GetChild(0).DOScale(Vector3.one, 0.05f);
                    transform.DOMove(initPosition + new Vector3(0, 0.3f, -0.3f), 0.01f).onComplete += delegate
                    {
                        isDragging = true;
                        CreateCloneOnDragging();
                        animator.SetBool("isDrag", true);
                    };
                }
                if (heroInfo.type != DBHero.TYPE_TROOPER_MAGIC && heroInfo.type != DBHero.TYPE_BUFF_MAGIC && isSelected)
                {
                    foreach (CardSlot slot in GameBattleScene.instance.ChooseSelfAnyBlank())
                    {
                        if (slot.yPos != 3)
                            slot.HighLightSlot();
                    }
                }
                else
                {
                    if (highlightSpellRange == null && (heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || heroInfo.type == DBHero.TYPE_BUFF_MAGIC))
                    {
                        Transform eff = PoolManager.Pools["Effect"].Spawn(spellRangeEff, GameBattleScene.instance.spellRangePos);
                        highlightSpellRange = eff;
                        highlightSpellRange.localPosition = Vector3.zero;
                        highlightSpellRange.localScale = Vector3.one;
                        highlightSpellRange.localRotation = Quaternion.Euler(Vector3.zero);
                        highlightSpellRange.GetComponent<Animation>().Play("HighlightBlue");
                    }
                }
            }

        }
    }
    public override void OnEndHold()
    {
        if (cardOwner == CardOwner.Enemy)
            return;
        if (isSelected)
            return;
        if (isDragging)
            return;
        if (cursor != null)
            return;
        base.OnEndHold();
        MoveBack(0.05f);
        Interested(false);
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

#if UNITY_STANDALONE || UNITY_EDITOR
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#elif UNITY_ANDROID
        Ray ray = Camera.main.ScreenPointToRay(GameBattleScene.instance.touch.position);
#endif
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider != null)
            {
                if (heroInfo.type != DBHero.TYPE_TROOPER_MAGIC && heroInfo.type != DBHero.TYPE_BUFF_MAGIC)
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
                else
                {
                    if (hit.collider.tag == "SpellRange")
                    {
                        if (hit.collider.GetComponent<Animation>() != null)
                            if (hit.collider.GetComponent<Animation>().IsPlaying("HighlightBlue"))
                                hit.collider.GetComponent<Animation>().Play("HighlightBlue_Hover");
                            else if (hit.collider.GetComponent<Animation>().IsPlaying("HighlightBlue_Hover"))
                                hit.collider.GetComponent<Animation>().Play("HighlightBlue");
                    }
                }
            }
        }
    }
    #endregion
}