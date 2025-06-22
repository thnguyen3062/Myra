using PathologicalGames;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPreviewInfoBattle : MonoBehaviour
{
    [SerializeField] private Image cardPrintImage;
    [SerializeField] private Image frameImage;
    [SerializeField] private Image cardFrameColor;
    [SerializeField] private Transform gemImageContainer;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI manaCost;
    [SerializeField] private TextMeshProUGUI regionText;
    [SerializeField] private TextMeshProUGUI penaltyText;
    [SerializeField] private GameObject rarityImage;
    [SerializeField] Transform skillFullContainer;
    [SerializeField] Transform skillFullPrefab;
    [SerializeField] Transform keyWordContainer;
    [SerializeField] Transform keyWordFullPrefab;
    [SerializeField] Transform effFlares;
    [SerializeField] Transform effPowers;
    // DBHero hero;

    private void OnDisable()
    {
        //if (PoolManager.Pools["Skill"] != null)
        //    PoolManager.Pools["Skill"].DespawnAll();

    }

    public void SetCardPreview(DBHero heroInfo, long frame, bool isFullPreview)
    {
        foreach (Transform child in effFlares)
            child.gameObject.SetActive(false);
        foreach (Transform child in effPowers)
            child.gameObject.SetActive(false);
        CardDataInfo info = CardData.Instance.GetCardDataInfo(heroInfo.id);
        //hero = Database.GetHero(id);

        cardPrintImage.material = null;
        frameImage.material = null;
        if (info != null && heroInfo != null)
        {
            if (heroInfo.type == DBHero.TYPE_GOD)   
                SetGodPreviewCard(heroInfo, frame, isFullPreview);
            if (heroInfo.type == DBHero.TYPE_TROOPER_MAGIC || heroInfo.type == DBHero.TYPE_BUFF_MAGIC)
                SetMagicPreviewCard(heroInfo, frame);
            if (heroInfo.type == DBHero.TYPE_TROOPER_NORMAL)
                SetMinionPreviewCard(heroInfo, frame);
        }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          
        if (info != null)
        {
            if (isFullPreview)
            {
                if (skillFullContainer != null)
                {
                    foreach (Transform child in skillFullContainer.transform)
                        Destroy(child.gameObject);
                    if (heroInfo.lstUnlockSkillCard.Count > 0)
                    {
                        skillFullContainer.gameObject.SetActive(true);
                        for (int i = 0; i < heroInfo.lstUnlockSkillCard.Count; i++)
                        {
                            for (int j = 0; j < heroInfo.lstHeroSkill.Count; j++)
                            {
                                //skill buff card khop voi db skill than va description cua than)
                                if ((heroInfo.lstUnlockSkillCard[i].lstBuffSkillID.Contains(heroInfo.lstHeroSkill[j].id)) && CardData.Instance.GetCardSkillDataInfo(heroInfo.id).skillIds.Contains(heroInfo.lstHeroSkill[j].id))
                                {
                                    DBHeroSkill skill = heroInfo.lstHeroSkill[j];
                                    Transform trans = PoolManager.Pools["Skill"].Spawn(skillFullPrefab);
                                    trans.SetParent(skillFullContainer);
                                    trans.localScale = Vector3.one;
                                    trans.GetComponent<SkillInfo>().InitDataSkill(heroInfo.lstUnlockSkillCard[i], skill, heroInfo.color);
                                }
                            }
                        }
                    }
                    else
                        skillFullContainer.gameObject.SetActive(false);
                }
                if (description != null)
                {
                    description.text = "";
                    if (keyWordContainer != null)
                    {
                        foreach (Transform trans in keyWordContainer)
                            Destroy(trans.gameObject);
                        UpdateHeroInfoFull(heroInfo);
                    }
                    if (info.description.Count > 0)
                    {
                        if (heroInfo.type != DBHero.TYPE_GOD)
                        {
                            string txt = "";
                            info.description.ForEach(x =>
                            {
                                if (!string.IsNullOrEmpty(x))
                                {
                                    if (x.Contains("<link=\"overrun\">") || x.Contains("<link=\"fraglie\">") || x.Contains("<link=\"combo\">") || x.Contains("<link=\"breaker\">") || x.Contains("<link=\"cleave\">") || x.Contains("<link=\"godslayer\">") || x.Contains("<link=\"pierce\">"))
                                        x = "<size=36>" + x + "</size>";
                                    txt += x;
                                }
                            });
                            if (description != null)
                            {
                                txt = txt.Replace("\\n", "\n");
                                description.text = txt;
                            }
                        }
                        else
                        {
                            string txt = "";
                            if (info.description.Count > 0)
                            {
                                info.description.ForEach(x =>
                              {
                                  if (!string.IsNullOrEmpty(x))
                                  {
                                      if (x.Contains("<link=\"overrun\">") || x.Contains("<link=\"fraglie\">") || x.Contains("<link=\"combo\">") || x.Contains("<link=\"breaker\">") || x.Contains("<link=\"cleave\">") || x.Contains("<link=\"godslayer\">") || x.Contains("<link=\"pierce\">"))
                                          x = "<size=36>" + x + "</size>";
                                      txt += x;
                                  }
                              });
                                if (description != null)
                                {
                                    txt = txt.Replace("\\n", "\n");
                                    description.text = txt;
                                }
                            }
                        }
                    }

                }
            }
            else
            {
                description.text = "";
                if (info.description.Count > 0)
                {
                    if (heroInfo.type != DBHero.TYPE_GOD)
                    {
                        string txt = "";
                        info.description.ForEach(x =>
                        {
                            if (!string.IsNullOrEmpty(x))
                            {
                                if (x.Contains("<link=\"overrun\">") || x.Contains("<link=\"fraglie\">") || x.Contains("<link=\"combo\">") || x.Contains("<link=\"breaker\">") || x.Contains("<link=\"cleave\">") || x.Contains("<link=\"godslayer\">") || x.Contains("<link=\"pierce\">"))
                                    x = "<size=36>" + x + "</size>";
                                txt += x;
                            }
                        });
                        if (description != null)
                        {
                            txt = txt.Replace("\\n", "\n");
                            description.text = txt;
                        }
                    }
                    else
                    {
                        //string txt = "";
                        //if (info.description.Count > 0)
                        //{
                        //    for (int i = 0; i < info.description.Count; i++)
                        //    {
                        //        if (string.IsNullOrEmpty(info.skillName[i]))
                        //        {
                        //            string x = info.description[i];


                        //            if (x.Contains("<link=\"overrun\">") || x.Contains("<link=\"fraglie\">") || x.Contains("<link=\"combo\">") || x.Contains("<link=\"breaker\">") || x.Contains("<link=\"cleave\">") || x.Contains("<link=\"godslayer\">") || x.Contains("<link=\"pierce\">"))
                        //            {
                        //                //if (info.keyword.Count > 1)
                        //                //    x = "<size=26>" + x + "</size>";
                        //                //else
                        //                //    x = "<size=40>" + x + "</size>";
                        //            }
                        //            txt += x;
                        //        }
                        //    }
                        //    if (description != null)
                        //    {
                        //        txt = txt.Replace("\\n", "\n");
                        //        description.text = txt;
                        //    }
                        //}
                    }
                }
                UpdateHeroInfo(heroInfo);
            }

        }
    }

    private void UpdateHeroInfo(DBHero hero)
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
                else
                {
                    txt = "<link=\"breaker\"><sprite=11></link>";
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
        string a = description.text;
        a += descriptUpdate;
        description.text = a;
    }
    private void UpdateHeroInfoFull(DBHero hero)
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
                Transform trans = PoolManager.Pools["Skill"].Spawn(keyWordFullPrefab);
                trans.SetParent(keyWordContainer);
                trans.localScale = Vector3.one;
                trans.GetComponent<KeywordInfo>().InitData(txt, "Combo");
            }
            if (hero.overrun > 0)
            {
                string txt = "<link=\"overrun\"><sprite=0></link>";
                descriptUpdate += txt;
                Transform trans = PoolManager.Pools["Skill"].Spawn(keyWordFullPrefab);
                trans.SetParent(keyWordContainer);
                trans.localScale = Vector3.one;
                trans.GetComponent<KeywordInfo>().InitData(txt, "Overrun");
            }
            if (hero.pierce > 0)
            {
                string txt = "<link=\"pierce\"><sprite=16></link>";
                descriptUpdate += txt;
                Transform trans = PoolManager.Pools["Skill"].Spawn(keyWordFullPrefab);
                trans.SetParent(keyWordContainer);
                trans.localScale = Vector3.one;
                trans.GetComponent<KeywordInfo>().InitData(txt, "Pierce");
            }
            if (hero.breaker > 0)
            {
                string txt = "";
                if (hero.breaker == 1)
                {
                    txt = "<link=\"breaker\"><sprite=12></link>";
                }
                else
                {
                    txt = "<link=\"breaker\"><sprite=11></link>";
                }
                descriptUpdate += txt;
                Transform trans = PoolManager.Pools["Skill"].Spawn(keyWordFullPrefab);
                trans.SetParent(keyWordContainer);
                trans.localScale = Vector3.one;
                trans.GetComponent<KeywordInfo>().InitData(txt, "Breaker", hero.breaker);
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
                Transform trans = PoolManager.Pools["Skill"].Spawn(keyWordFullPrefab);
                trans.SetParent(keyWordContainer);
                trans.localScale = Vector3.one;
                trans.GetComponent<KeywordInfo>().InitData(txt, "Godslayer", hero.godSlayer);
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
                Transform trans = PoolManager.Pools["Skill"].Spawn(keyWordFullPrefab);
                trans.SetParent(keyWordContainer);
                trans.localScale = Vector3.one;
                trans.GetComponent<KeywordInfo>().InitData(txt, "Cleave", hero.cleave);
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
        string a = description.text;
        a += descriptUpdate;
        description.text = a;
    }
    private void SetMinionPreviewCard(DBHero hero, long frame)
    {
        frameImage.sprite = CardData.Instance.GetCardFrameSprite("Mortal_" + frame + "_" + /*hero.rarity*/1);
        Texture eNoise1 = CardData.Instance.GetCardFrameTexture("Noise84");
        Texture eNoise2 = CardData.Instance.GetCardFrameTexture("Noise03");
        switch (frame)
        {
            case 3:
                {
                    eNoise1 = CardData.Instance.GetCardFrameTexture("Noise84");
                    eNoise2 = CardData.Instance.GetCardFrameTexture("Noise03");
                    break;
                }
            case 4:
                {
                    eNoise1 = CardData.Instance.GetCardFrameTexture("CrystalNoise");
                    eNoise2 = CardData.Instance.GetCardFrameTexture("RainbowMirrorNoise");
                    break;
                }
            case 5:
                {
                    eNoise1 = CardData.Instance.GetCardFrameTexture("Noise84");
                    eNoise2 = CardData.Instance.GetCardFrameTexture("Noise03");
                    break;
                }

        }
        if (frame >= 3)
        {
            Material mat = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_" + hero.type + "_" + frame + "_UI"));
            frameImage.material = mat;
            Texture main = CardData.Instance.GetCardFrameTexture("Mortal_" + frame + "_" + /*hero.rarity*/1);
            frameImage.material.SetTexture("Texture2D_08500469d2944895a9ad392692f50c3c", main);
            //frameImage.material.SetTexture("_MainTex", main);
            Texture mask = CardData.Instance.GetCardFrameMaskTexture("M_" + hero.type + "_" + /*hero.rarity*/1);
            frameImage.material.SetTexture("Texture2D_17ff7d92927445598a03023feb129259", mask);
            Texture fmask = CardData.Instance.GetCardFrameMaskTexture("T_FlareMask");
            frameImage.material.SetTexture("Texture2D_479d5e414ef0402b8a4a2d69cd8a2a56", fmask);


            //frameImage.material.SetTexture("_FrameMask", mask);
            if (effFlares != null)
            {
                effFlares.GetChild((int)frame - 3).gameObject.SetActive(true);
            }
            if (effPowers != null)
            {
                if (effPowers.childCount == 4)
                {
                    effPowers.GetChild((int)frame - 3 + 1).gameObject.SetActive(true);
                    effPowers.GetChild((int)frame - 3 + 1).GetComponent<Image>().material = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_EP_" + hero.type + "_" + frame));
                    effPowers.GetChild((int)frame - 3 + 1).GetComponent<Image>().material.SetTexture("Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d", mask);
                    effPowers.GetChild((int)frame - 3 + 1).GetComponent<Image>().material.SetTexture("Texture2D_0bfc715abbaf486ab9a057885da41035", eNoise1);
                    effPowers.GetChild((int)frame - 3 + 1).GetComponent<Image>().material.SetTexture("Texture2D_6f8406899a754e3b9b4c2dad6adc7b27", eNoise2);
                }
                else
                {
                    effPowers.GetChild((int)frame - 3).gameObject.SetActive(true);
                    effPowers.GetChild((int)frame - 3).GetComponent<Image>().material = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_EP_" + hero.type + "_" + frame));
                    effPowers.GetChild((int)frame - 3).GetComponent<Image>().material.SetTexture("Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d", mask);
                    effPowers.GetChild((int)frame - 3).GetComponent<Image>().material.SetTexture("Texture2D_0bfc715abbaf486ab9a057885da41035", eNoise1);
                    effPowers.GetChild((int)frame - 3).GetComponent<Image>().material.SetTexture("Texture2D_6f8406899a754e3b9b4c2dad6adc7b27", eNoise2);
                }

            }
            cardPrintImage.material = Instantiate(CardData.Instance.GetAnimatedMaterial("M_" + hero.id)) as Material;
            cardPrintImage.material.shader = Shader.Find("ReadyPrefab/Shader3.14");
            cardPrintImage.sprite = null;
            cardPrintImage.material.SetVector("_TileOff", new Vector4(0.8f, 1.1f, 0.1f, -0.05f));
            cardPrintImage.GetComponent<RectTransform>().sizeDelta = new Vector2(550, 700);
        }
        else
        {
            cardPrintImage.sprite = CardData.Instance.GetOnBoardSprite(hero.id);
            cardPrintImage.GetComponent<RectTransform>().sizeDelta = new Vector2(700, 700);
            Texture mask = CardData.Instance.GetCardFrameMaskTexture("M_" + hero.type + "_" + /*hero.rarity*/1);
            if (effPowers.childCount == 4)
            {
                effPowers.GetChild(0).gameObject.SetActive(true);
                effPowers.GetChild(0).GetComponent<Image>().material = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_EP_" + hero.type + "_" + frame));
                effPowers.GetChild(0).GetComponent<Image>().material.SetTexture("Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d", mask);
                effPowers.GetChild(0).GetComponent<Image>().material.SetTexture("Texture2D_0bfc715abbaf486ab9a057885da41035", eNoise1);
                effPowers.GetChild(0).GetComponent<Image>().material.SetTexture("Texture2D_6f8406899a754e3b9b4c2dad6adc7b27", eNoise2);
            }
        }
        //frameImage.sprite = CardData.Instance.GetCardFrameSprite("Mortal_" + frame + "_" + 1); // hotfix
        cardFrameColor.sprite = CardData.Instance.GetCardColorSprite("Minion_" + hero.color);
        //for (int i = 0; i < shardImage.Length; i++)
        //{
        //    if (i < hero.shardRequired)
        //    {
        //        shardImage[i].gameObject.SetActive(true);
        //        shardImage[i].sprite = CardData.Instance.GetShardSprite(hero.color);
        //    }
        //    else
        //    {
        //        shardImage[i].gameObject.SetActive(false);
        //    }
        //}

        if (damageText != null)
            damageText.text = hero.atk.ToString();
        if (healthText != null)
            healthText.text = hero.hp.ToString();
        if (cardName != null)
            cardName.text = CardData.Instance.GetCardDataInfo(hero.id).name;
        if (manaCost != null)
            manaCost.text = hero.mana.ToString();
        if (regionText != null)
            regionText.text = hero.cardRegionDict[hero.speciesId];
        if (rarityImage != null)
            rarityImage.GetComponent<Image>().sprite = CardData.Instance.GetRaritySprite(hero.rarity);
    }

    private void SetGodPreviewCard(DBHero hero, long frame, bool isfull)
    {
        frameImage.sprite = CardData.Instance.GetCardFrameSprite("God_" + frame + "_" + hero.rarity);
        Texture eNoise1 = CardData.Instance.GetCardFrameTexture("Noise84");
        Texture eNoise2 = CardData.Instance.GetCardFrameTexture("Noise03");
        switch (frame)
        {
            case 3:
                {
                    eNoise1 = CardData.Instance.GetCardFrameTexture("Noise84");
                    eNoise2 = CardData.Instance.GetCardFrameTexture("Noise03");
                    break;
                }
            case 4:
                {
                    eNoise1 = CardData.Instance.GetCardFrameTexture("CrystalNoise");
                    eNoise2 = CardData.Instance.GetCardFrameTexture("RainbowMirrorNoise");
                    break;
                }
            case 5:
                {
                    eNoise1 = CardData.Instance.GetCardFrameTexture("Noise84");
                    eNoise2 = CardData.Instance.GetCardFrameTexture("Noise03");
                    break;
                }

        }
        if (frame >= 3)
        {
            Material mat = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_" + hero.type + "_" + frame + "_UI"));
            frameImage.material = mat;
            Texture main = CardData.Instance.GetCardFrameTexture("God_" + frame + "_" + hero.rarity);
            frameImage.material.SetTexture("Texture2D_08500469d2944895a9ad392692f50c3c", main);
            //frameImage.material.SetTexture("_MainTex", main);
            Texture mask = CardData.Instance.GetCardFrameMaskTexture("M_" + hero.type + "_" + hero.rarity);
            frameImage.material.SetTexture("Texture2D_17ff7d92927445598a03023feb129259", mask);

            Texture fmask = CardData.Instance.GetCardFrameMaskTexture("T_FlareMask");
            frameImage.material.SetTexture("Texture2D_479d5e414ef0402b8a4a2d69cd8a2a56", fmask);
            //frameImage.material.SetTexture("_FrameMask", mask);
            if (effFlares != null)
            {
                effFlares.GetChild((int)frame - 3).gameObject.SetActive(true);
            }
            if (effPowers != null)
            {  
                if (effPowers.childCount == 4)
                {
                    effPowers.GetChild((int)frame - 3 + 1).gameObject.SetActive(true);
                    effPowers.GetChild((int)frame - 3 + 1).GetComponent<Image>().material = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_EP_" + hero.type + "_" + frame));
                    effPowers.GetChild((int)frame - 3 + 1).GetComponent<Image>().material.SetTexture("Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d", mask);
                    effPowers.GetChild((int)frame - 3 + 1).GetComponent<Image>().material.SetTexture("Texture2D_0bfc715abbaf486ab9a057885da41035", eNoise1);
                    effPowers.GetChild((int)frame - 3 + 1).GetComponent<Image>().material.SetTexture("Texture2D_6f8406899a754e3b9b4c2dad6adc7b27", eNoise2);
                }
                else
                {
                    effPowers.GetChild((int)frame - 3).gameObject.SetActive(true);
                    effPowers.GetChild((int)frame - 3).GetComponent<Image>().material = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_EP_" + hero.type + "_" + frame));
                    effPowers.GetChild((int)frame - 3).GetComponent<Image>().material.SetTexture("Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d", mask);
                    effPowers.GetChild((int)frame - 3).GetComponent<Image>().material.SetTexture("Texture2D_0bfc715abbaf486ab9a057885da41035", eNoise1);
                    effPowers.GetChild((int)frame - 3).GetComponent<Image>().material.SetTexture("Texture2D_6f8406899a754e3b9b4c2dad6adc7b27", eNoise2);
                }
            }
            cardPrintImage.material = Instantiate(CardData.Instance.GetAnimatedMaterial("M_" + hero.id)) as Material;
            cardPrintImage.material.shader = Shader.Find("ReadyPrefab/Shader3.14");
            cardPrintImage.sprite = null;
            cardPrintImage.material.SetVector("_TileOff", new Vector3(0.68f, 0.93f, 0.17f));
            cardPrintImage.GetComponent<RectTransform>().sizeDelta = new Vector2(450, 600);
        }
        else
        {
            cardPrintImage.sprite = CardData.Instance.GetOnBoardSprite(hero.id);
            cardPrintImage.GetComponent<RectTransform>().sizeDelta = new Vector2(572, 572);

            Texture mask = CardData.Instance.GetCardFrameMaskTexture("M_" + hero.type + "_" + hero.rarity);
            if (effPowers.childCount == 4)
            {
                effPowers.GetChild(0).gameObject.SetActive(true);
                effPowers.GetChild(0).GetComponent<Image>().material = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_EP_" + hero.type + "_" + frame));
                effPowers.GetChild(0).GetComponent<Image>().material.SetTexture("Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d", mask);
                effPowers.GetChild(0).GetComponent<Image>().material.SetTexture("Texture2D_0bfc715abbaf486ab9a057885da41035", eNoise1);
                effPowers.GetChild(0).GetComponent<Image>().material.SetTexture("Texture2D_6f8406899a754e3b9b4c2dad6adc7b27", eNoise2);
            }
        }
        cardFrameColor.sprite = CardData.Instance.GetCardColorSprite("God_" + hero.color + "_" + hero.rarity);
        //for (int i = 0; i < shardImage.Length; i++)
        //{
        //    if (i < hero.shardRequired)
        //    {
        //        shardImage[i].gameObject.SetActive(true);
        //        shardImage[i].sprite = CardData.Instance.GetShardSprite(hero.color);
        //    }
        //    else
        //    {
        //        shardImage[i].gameObject.SetActive(false);
        //    }
        //}

        if (damageText != null)
            damageText.text = hero.atk.ToString();
        if (healthText != null)
            healthText.text = hero.hp.ToString();
        if (cardName != null)
            cardName.text = CardData.Instance.GetCardDataInfo(hero.id).name;
        if (manaCost != null)
            manaCost.text = hero.mana.ToString();
        if (regionText != null)
            regionText.text = hero.cardRegionDict[hero.speciesId];
        if (penaltyText != null)
            penaltyText.text = "-" + hero.deathCost.ToString();
        if (rarityImage != null)
            rarityImage.GetComponent<Image>().sprite = CardData.Instance.GetRaritySprite(hero.rarity);

    }

    private void SetMagicPreviewCard(DBHero hero, long frame)
    {
        frameImage.sprite = CardData.Instance.GetCardFrameSprite("Spell_" + frame + "_" + /*hero.rarity*/1);
        Texture eNoise1 = CardData.Instance.GetCardFrameTexture("Noise84");
        Texture eNoise2 = CardData.Instance.GetCardFrameTexture("Noise03");
        switch (frame)
        {
            case 3:
                {
                    eNoise1 = CardData.Instance.GetCardFrameTexture("Noise84");
                    eNoise2 = CardData.Instance.GetCardFrameTexture("Noise03");
                    break;
                }
            case 4:
                {
                    eNoise1 = CardData.Instance.GetCardFrameTexture("CrystalNoise");
                    eNoise2 = CardData.Instance.GetCardFrameTexture("RainbowMirrorNoise");
                    break;
                }
            case 5:
                {
                    eNoise1 = CardData.Instance.GetCardFrameTexture("Noise84");
                    eNoise2 = CardData.Instance.GetCardFrameTexture("Noise03");
                    break;
                }

        }
        if (frame >= 3)
        {
            Material mat = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_" + hero.type + "_" + frame + "_UI"));
            frameImage.material = mat;
            Texture main = CardData.Instance.GetCardFrameTexture("Spell_" + frame + "_" + /*hero.rarity*/1);
            frameImage.material.SetTexture("Texture2D_08500469d2944895a9ad392692f50c3c", main);
            //frameImage.material.SetTexture("_MainTex", main);
            Texture mask = CardData.Instance.GetCardFrameMaskTexture("M_" + hero.type + "_" + /*hero.rarity*/1);
            frameImage.material.SetTexture("Texture2D_17ff7d92927445598a03023feb129259", mask);

            Texture fmask = CardData.Instance.GetCardFrameMaskTexture("T_FlareMask");
            frameImage.material.SetTexture("Texture2D_479d5e414ef0402b8a4a2d69cd8a2a56", fmask);

            //frameImage.material.SetTexture("_FrameMask", mask);
            if (effPowers != null)
            {
                if (effPowers != null)
                {
                    if (effPowers.childCount == 4)
                    {
                        effPowers.GetChild((int)frame - 3 + 1).gameObject.SetActive(true);
                        effPowers.GetChild((int)frame - 3 + 1).GetComponent<Image>().material = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_EP_" + hero.type + "_" + frame));
                        effPowers.GetChild((int)frame - 3 + 1).GetComponent<Image>().material.SetTexture("Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d", mask);
                        effPowers.GetChild((int)frame - 3 + 1).GetComponent<Image>().material.SetTexture("Texture2D_0bfc715abbaf486ab9a057885da41035", eNoise1);
                        effPowers.GetChild((int)frame - 3 + 1).GetComponent<Image>().material.SetTexture("Texture2D_6f8406899a754e3b9b4c2dad6adc7b27", eNoise2);
                    }
                    else
                    {
                        effPowers.GetChild((int)frame - 3).gameObject.SetActive(true);
                        effPowers.GetChild((int)frame - 3).GetComponent<Image>().material = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_EP_" + hero.type + "_" + frame));
                        effPowers.GetChild((int)frame - 3).GetComponent<Image>().material.SetTexture("Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d", mask);
                        effPowers.GetChild((int)frame - 3).GetComponent<Image>().material.SetTexture("Texture2D_0bfc715abbaf486ab9a057885da41035", eNoise1);
                        effPowers.GetChild((int)frame - 3).GetComponent<Image>().material.SetTexture("Texture2D_6f8406899a754e3b9b4c2dad6adc7b27", eNoise2);
                    }
                }
            }
            if (effFlares != null)
            {
                effFlares.GetChild((int)frame - 3).gameObject.SetActive(true);
            }
            cardPrintImage.material = Instantiate(CardData.Instance.GetAnimatedMaterial("M_" + hero.id)) as Material;
            cardPrintImage.material.shader = Shader.Find("ReadyPrefab/Shader3.14");
            cardPrintImage.sprite = null;
        }
        else
        {
            cardPrintImage.sprite = CardData.Instance.GetOnBoardSprite(hero.id);
            Texture mask = CardData.Instance.GetCardFrameMaskTexture("M_" + hero.type + "_" + /*hero.rarity*/1);
            if (effPowers.childCount == 4)
            {
                effPowers.GetChild(0).gameObject.SetActive(true);
                effPowers.GetChild(0).GetComponent<Image>().material = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_EP_" + hero.type + "_" + frame));
                effPowers.GetChild(0).GetComponent<Image>().material.SetTexture("Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d", mask);
                effPowers.GetChild(0).GetComponent<Image>().material.SetTexture("Texture2D_0bfc715abbaf486ab9a057885da41035", eNoise1);
                effPowers.GetChild(0).GetComponent<Image>().material.SetTexture("Texture2D_6f8406899a754e3b9b4c2dad6adc7b27", eNoise2);
            }
        }
        cardFrameColor.sprite = CardData.Instance.GetCardColorSprite("Spell_" + hero.color + "_" + /*hero.rarity*/1);
        //for (int i = 0; i < shardImage.Length; i++)
        //{
        //    if (i < hero.shardRequired)
        //    {
        //        shardImage[i].gameObject.SetActive(true);
        //        shardImage[i].sprite = CardData.Instance.GetShardSprite(hero.color);
        //    }
        //    else
        //    {
        //        shardImage[i].gameObject.SetActive(false);
        //    }
        //}

        if (cardName != null)
            cardName.text = /*hero.name;*/CardData.Instance.GetCardDataInfo(hero.id).name;
        if (manaCost != null)
            manaCost.text = hero.mana.ToString();
        if (rarityImage != null)
            rarityImage.GetComponent<Image>().sprite = CardData.Instance.GetRaritySprite(hero.rarity);
    }
}
