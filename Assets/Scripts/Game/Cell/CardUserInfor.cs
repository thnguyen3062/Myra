using GIKCore.DB;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class CardUserInfor : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TxtDetail;
    [SerializeField] private TextMeshProUGUI m_TxtHP, m_TxtAtk, m_TxtMana;
    [SerializeField] private TextMeshProUGUI m_Count;
    [SerializeField] private TextMeshProUGUI m_TxtName;
    [SerializeField] private TextMeshProUGUI m_TxtRegion;
    [SerializeField] private TextMeshProUGUI m_TxtPenalty;
    [SerializeField] private Image m_Highlight;
    [SerializeField] private Image m_CardImg, m_filterColor, m_Frame, m_Rarity;
    [SerializeField] private GameObject m_NormalPower, m_GoldPower, m_BlackPower, m_DiamondPower, m_BlackFlare, m_GoldFlare, m_DiamondFlare;
    [SerializeField] private Image m_FrameBackNew, m_FrameBackSelect;
    [SerializeField] private Image m_BlackFrame;
    [SerializeField] private GameObject m_NewFlag;

    public void SetInfoCard(DBHero db, int countHC, int frame, bool isCollection = false, int atk = -1, int hp = -1, int mana = -1, bool isShowCardAnim = false)
    {
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
        if (m_NormalPower != null)
        {
            m_NormalPower.SetActive(true);
            Image img = m_NormalPower.GetComponent<Image>();
            if (img != null)
            {
                Material mat = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_EP_" + db.type + "_" + 1));
                img.material = mat;

                Texture mask = CardData.Instance.GetCardFrameTexture("M_" + db.type + "_" + db.rarity);
                img.material.SetTexture("Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d", mask);
                img.material.SetTexture("Texture2D_0bfc715abbaf486ab9a057885da41035", eNoise1);
                img.material.SetTexture("Texture2D_6f8406899a754e3b9b4c2dad6adc7b27", eNoise2);

            }

        }
        m_Frame.material = null;
        // power
        if (m_GoldPower != null)
            m_GoldPower.SetActive(false);
        if (m_BlackPower != null)
            m_BlackPower.SetActive(false);
        if (m_DiamondPower != null)
            m_DiamondPower.SetActive(false);

        // flare
        if (m_BlackFlare != null)
            m_BlackFlare.SetActive(false);
        if (m_GoldFlare != null)
            m_GoldFlare.SetActive(false);
        if (m_DiamondFlare != null)
            m_DiamondFlare.SetActive(false);

        int rarity = (int)db.rarity;
        if (db.type == DBHero.TYPE_TROOPER_NORMAL || db.type == DBHero.TYPE_TROOPER_MAGIC)
            rarity = 1;

        switch (frame)
        {
            case 3:
                {
                    if (m_NormalPower != null)
                        m_NormalPower.SetActive(false);
                    m_GoldPower.SetActive(true);
                    m_GoldFlare.SetActive(true);

                    Image img = m_GoldPower.GetComponent<Image>();
                    if (img != null)
                    {
                        Material mat = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_EP_" + db.type + "_" + frame));
                        img.material = mat;

                        Texture mask = CardData.Instance.GetCardFrameTexture("M_" + db.type + "_" + rarity);
                        img.material.SetTexture("Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d", mask);
                        img.material.SetTexture("Texture2D_0bfc715abbaf486ab9a057885da41035", eNoise1);
                        img.material.SetTexture("Texture2D_6f8406899a754e3b9b4c2dad6adc7b27", eNoise2);

                    }
                    break;
                }
            case 4:
                {
                    if (m_NormalPower != null)
                        m_NormalPower.SetActive(false);
                    m_DiamondPower.SetActive(true);
                    m_DiamondFlare.SetActive(true);

                    Image img = m_DiamondPower.GetComponent<Image>();
                    if (img != null)
                    {
                        Material mat = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_EP_" + db.type + "_" + frame));
                        img.material = mat;

                        Texture mask = CardData.Instance.GetCardFrameTexture("M_" + db.type + "_" + rarity);
                        img.material.SetTexture("Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d", mask);
                        img.material.SetTexture("Texture2D_0bfc715abbaf486ab9a057885da41035", eNoise1);
                        img.material.SetTexture("Texture2D_6f8406899a754e3b9b4c2dad6adc7b27", eNoise2);

                    }
                    break;
                }
            case 5:
                {
                    if (m_NormalPower != null)
                        m_NormalPower.SetActive(false);
                    m_BlackPower.SetActive(true);
                    m_BlackFlare.SetActive(true);
                    Image img = m_BlackPower.GetComponent<Image>();
                    if (img != null)
                    {
                        Material mat = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_EP_" + db.type + "_" + frame));
                        img.material = mat;
                        Texture mask = CardData.Instance.GetCardFrameTexture("M_" + db.type + "_" + rarity);
                        img.material.SetTexture("Texture2D_e8cc20a2855f42e1b851a2d926ed5f3d", mask);
                        img.material.SetTexture("Texture2D_0bfc715abbaf486ab9a057885da41035", eNoise1);
                        img.material.SetTexture("Texture2D_6f8406899a754e3b9b4c2dad6adc7b27", eNoise2);
                    }
                    break;
                }
        }
        if (frame >= 3)
        {
            Material mat = Instantiate(CardData.Instance.GetCardFrameMaterial("Mat_" + db.type + "_" + frame + "_UI"));
            m_Frame.material = mat;
            switch (db.type)
            {
                case 0:
                    {
                        Texture main = CardData.Instance.GetCardFrameTexture("God_" + frame + "_" + db.rarity);
                        m_Frame.material.SetTexture("Texture2D_08500469d2944895a9ad392692f50c3c", main);

                        Texture masktexture = CardData.Instance.GetCardFrameMaskTexture("M_" + db.type + "_" + db.rarity);
                        m_Frame.material.SetTexture("Texture2D_17ff7d92927445598a03023feb129259", masktexture);
                        Texture fmask = CardData.Instance.GetCardFrameMaskTexture("T_FlareMask");
                        m_Frame.material.SetTexture("Texture2D_479d5e414ef0402b8a4a2d69cd8a2a56", fmask);
                        //m_Frame.material.SetTexture("_MainTex", main);  
                        break;
                    };
                case 1:
                    {
                        Texture main = CardData.Instance.GetCardFrameTexture("Mortal_" + frame + "_" + /*db.rarity*/1);
                        m_Frame.material.SetTexture("Texture2D_08500469d2944895a9ad392692f50c3c", main);

                        Texture masktexture = CardData.Instance.GetCardFrameMaskTexture("M_" + db.type + "_" + db.rarity);
                        m_Frame.material.SetTexture("Texture2D_17ff7d92927445598a03023feb129259", masktexture);
                        Texture fmask = CardData.Instance.GetCardFrameMaskTexture("T_FlareMask");
                        m_Frame.material.SetTexture("Texture2D_479d5e414ef0402b8a4a2d69cd8a2a56", fmask);
                        //m_Frame.material.SetTexture("_MainTex", main);
                        break;
                    };
                case 2:
                    {
                        Texture main = CardData.Instance.GetCardFrameTexture("Spell_" + frame + "_" + /*db.rarity*/1);
                        m_Frame.material.SetTexture("Texture2D_08500469d2944895a9ad392692f50c3c", main);

                        Texture masktexture = CardData.Instance.GetCardFrameMaskTexture("M_" + db.type + "_" + db.rarity);


                        m_Frame.material.SetTexture("Texture2D_17ff7d92927445598a03023feb129259", masktexture);
                        Texture fmask = CardData.Instance.GetCardFrameMaskTexture("T_FlareMask");
                        m_Frame.material.SetTexture("Texture2D_479d5e414ef0402b8a4a2d69cd8a2a56", fmask);
                        //m_Frame.material.SetTexture("_MainTex", main);
                        break;


                    };
                case 3:

                    {
                        Texture main = CardData.Instance.GetCardFrameTexture("Spell_" + frame + "_" + /*db.rarity*/1);
                        m_Frame.material.SetTexture("Texture2D_08500469d2944895a9ad392692f50c3c", main);

                        Texture masktexture = CardData.Instance.GetCardFrameMaskTexture("M_" + db.type + "_" + db.rarity);
                        m_Frame.material.SetTexture("Texture2D_17ff7d92927445598a03023feb129259", masktexture);
                        Texture fmask = CardData.Instance.GetCardFrameMaskTexture("T_FlareMask");
                        m_Frame.material.SetTexture("Texture2D_479d5e414ef0402b8a4a2d69cd8a2a56", fmask);
                        break;
                    }

            }
            Texture mask;
            if (db.type != 0)
                mask = CardData.Instance.GetCardFrameMaskTexture("M_" + db.type + "_" + 1);
            else
                mask = CardData.Instance.GetCardFrameMaskTexture("M_" + db.type + "_" + db.rarity);
            m_Frame.material.SetTexture("Texture2D_17ff7d92927445598a03023feb129259", mask);
            //m_Frame.material.SetTexture("_FrameMask", mask);

        }

        if (m_CardImg != null)
        {
            m_CardImg.sprite = CardData.Instance.GetOnBoardSprite(db.id);
            if (isShowCardAnim && frame >= 3)
            {
                switch (db.type)
                {
                    case DBHero.TYPE_GOD:
                        {
                            m_CardImg.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 400);
                            break;
                        }
                    case DBHero.TYPE_TROOPER_NORMAL:
                        {
                            m_CardImg.GetComponent<RectTransform>().sizeDelta = new Vector2(487, 666);
                            break;
                        }
                    case DBHero.TYPE_TROOPER_MAGIC:
                    case DBHero.TYPE_BUFF_MAGIC:
                        {
                            m_CardImg.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 500);
                            break;
                        }
                }
                m_CardImg.material = Instantiate(CardData.Instance.GetAnimatedMaterial("M_" + db.id)) as Material;
                m_CardImg.material.shader = Shader.Find("ReadyPrefab/Shader3.14");
                m_CardImg.sprite = null;
                m_CardImg.material.SetVector("_TileOff", new Vector3(0.68f, 0.93f, 0.17f));
            }
        }
        if (m_filterColor != null)
        {
            if (db.type == DBHero.TYPE_GOD)
            {

                m_filterColor.sprite = CardData.Instance.GetCardColorSprite("God_" + db.color + "_" + db.rarity);
                m_Frame.sprite = CardData.Instance.GetCardFrameSprite("God_" + frame + "_" + db.rarity);
                //for (int i = 0; i < gem.Length; i++)
                //{
                //    if (i < db.shardRequired)
                //    {
                //        gem[i].gameObject.SetActive(true);
                //        gem[i].sprite = CardData.Instance.GetShardSprite(db.color);
                //    }
                //    else
                //    {
                //        gem[i].gameObject.SetActive(false);
                //    }
                //}
            }
            if (db.type == DBHero.TYPE_TROOPER_MAGIC || db.type == DBHero.TYPE_BUFF_MAGIC)
            {
                //m_filterColor.sprite = CardData.Instance.GetCardColorSprite("Spell_" + db.color + "_" + db.rarity);
                m_filterColor.sprite = CardData.Instance.GetCardColorSprite("Spell_" + db.color + "_" + 1); // hotfix
                //m_Frame.sprite = CardData.Instance.GetCardFrameSprite("Spell_" + frame + "_" + db.rarity);
                m_Frame.sprite = CardData.Instance.GetCardFrameSprite("Spell_" + frame + "_" + 1);
                //for (int i = 0; i < gem.Length; i++)
                //{
                //    if (i < db.shardRequired)
                //    {
                //        gem[i].gameObject.SetActive(true);
                //        gem[i].sprite = CardData.Instance.GetShardSprite(db.color);
                //    }
                //    else
                //    {
                //        gem[i].gameObject.SetActive(false);
                //    }
                //}
            }
            if (db.type == DBHero.TYPE_TROOPER_NORMAL)
            {
                m_filterColor.sprite = CardData.Instance.GetCardColorSprite("Minion_" + db.color);
                //m_Frame.sprite = CardData.Instance.GetCardFrameSprite("Mortal_" + frame + "_" + db.rarity);
                m_Frame.sprite = CardData.Instance.GetCardFrameSprite("Mortal_" + frame + "_" + 1); // hotfix
                //for (int i = 0; i < gem.Length; i++)
                //{
                //    if (i < db.shardRequired)
                //    {
                //        gem[i].gameObject.SetActive(true);
                //        gem[i].sprite = CardData.Instance.GetShardSprite(db.color);
                //    }
                //    else
                //    {
                //        gem[i].gameObject.SetActive(false);
                //    }
                //}
            }
        }

        if (m_Count != null)
            m_Count.text = "X" + countHC.ToString();
        if (m_TxtName != null)
            m_TxtName.text = db.name;/*CardData.Instance.GetCardDataInfo(db.id).name;*/
        if (m_TxtHP != null)
        {
            if (hp != -1)
                m_TxtHP.text = hp + "";
            else
                m_TxtHP.text = db.hp.ToString();
        }
        if (m_TxtAtk != null)
        {
            if (atk != -1)
                m_TxtAtk.text = atk + "";
            else
                m_TxtAtk.text = db.atk.ToString();
        }
        if (m_TxtMana != null)
        {
            if (mana != -1)
                m_TxtMana.text = mana + "";
            else
                m_TxtMana.text = db.mana.ToString();
        }
        if (m_TxtRegion != null)
            m_TxtRegion.text = db.cardRegionDict[db.speciesId];
        if (m_TxtPenalty != null)
            m_TxtPenalty.text = "-" + db.deathCost.ToString();
        if (m_Rarity != null)
            m_Rarity.sprite = CardData.Instance.GetRaritySprite(db.rarity);

        if (m_BlackFrame != null)
        {
            m_BlackFrame.gameObject.SetActive(countHC == 0);
            m_BlackFrame.sprite = CardData.Instance.GetCardFrameBlack(db.type, db.rarity);
        }
        if (m_FrameBackNew != null)
            m_FrameBackNew.sprite = CardData.Instance.GetCardFrameNew(db.type, db.rarity);
        if (m_FrameBackSelect != null)
            m_FrameBackSelect.sprite = CardData.Instance.GetCardFrameSelect(db.type, db.rarity);

        if (m_FrameBackNew != null)
        {
            string jsonNewCard = GamePrefs.NewCards;
            JSONNode jNode = JSON.Parse(jsonNewCard);
            JSONArray jArr = jNode.AsArray;
            List<long> lstNewCards = new List<long>();
            if (jArr != null)
            {
                for (int i = 0; i < jArr.Count; i++)
                    lstNewCards.Add(jArr[i]);
            }

            m_FrameBackNew.gameObject.SetActive(lstNewCards.Contains(db.id));
            if (m_NewFlag != null)
                m_NewFlag.SetActive(lstNewCards.Contains(db.id));

            lstNewCards.Remove(db.id);
            string json = "";
            for (int idx = 0; idx < lstNewCards.Count; idx++)
            {
                json += lstNewCards[idx] + (idx < lstNewCards.Count - 1 ? "," : "");
            }
            GamePrefs.NewCards = string.Format("[{0}]", json);
        }

        //if (GameData.main.userProgressionState >= 15)
        //{
        //if (countHC <= 0)
        //{
        //    m_CardImg.color = new Color(0.08f, 0.08f, 0.08f);
        //}
        //else
        //{
        //    m_CardImg.color = new Color(1f, 1f, 1f, 1f);
        //}
        //}
        CardDataInfo info = CardData.Instance.GetCardDataInfo(db.id);
        SetDescription(info, db, isCollection);
    }
    private void SetDescription(CardDataInfo info, DBHero db, bool isCollection = false)
    {
        if (m_TxtDetail != null)
        {
            m_TxtDetail.text = "";
            if (info != null)
            {

                if (info.description.Count > 0)
                {
                    if (db.type != DBHero.TYPE_GOD)
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
                        if (m_TxtDetail != null)
                        {
                            txt = txt.Replace("\\n", "\n");
                            m_TxtDetail.text = txt;
                        }
                    }
                    else
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
                        if (m_TxtDetail != null)
                        {
                            txt = txt.Replace("\\n", "\n");
                            m_TxtDetail.text = txt;
                        }
                    }
                }
            }


        }
    }

    public void CreatScaleEffect(int count)
    {
        ScaleText.Create(m_Count.transform.position, count, this.transform.parent);
    }

    public void SetFrameBackSelect(bool turn)
    {
        if (ProgressionController.instance == null)
            m_FrameBackSelect.gameObject.SetActive(turn);
    }
    public void SetPowerFrame(bool turn)
    {
        m_GoldPower.SetActive(turn);
        m_BlackPower.SetActive(turn);
        m_DiamondPower.SetActive(turn);
        m_BlackFlare.SetActive(turn);
        m_GoldFlare.SetActive(turn);
        m_DiamondFlare.SetActive(turn);
        if (!turn)
            m_Frame.material = null;
    }
}
