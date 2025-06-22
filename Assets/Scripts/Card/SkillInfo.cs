using GIKCore.Lang;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfo : MonoBehaviour
{
    [SerializeField] private Image skillImg,colorImage;
    [SerializeField] private GameObject ultiFrame;
    [SerializeField] private TextMeshProUGUI nameSkill;
    [SerializeField] private TextMeshProUGUI type;
    [SerializeField] private TextMeshProUGUI descript;
    [SerializeField] private Image fillImg;
    private DBHeroSkill data;
    private int shardFree;
    // Start is called before the first frame update
    public void InitDataSkill(DBHero cardBuff,DBHeroSkill db, long color)
    {
        this.data = db;
        CardDataInfo info = CardData.Instance.GetCardDataInfo(cardBuff.id);
        Sprite sprite = CardData.Instance.GetUltiSprite(db.id.ToString());
        if (ultiFrame != null)
            ultiFrame.SetActive(db.isUltiType);
        if (sprite != null)
            skillImg.sprite = sprite;
        
        if (type != null)
        { 
            if (db.skill_type == 0)
            {
                type.text = LangHandler.Get("121", "PASSIVE");
            }
            else
                type.text = LangHandler.Get("122", "ACTIVE");
        }
        if(colorImage!=null)
        {
            if (db.isUltiType)
                colorImage.sprite = CardData.Instance.GetCardColorSprite("Ulti_" + color);
            else
                colorImage.sprite=CardData.Instance.GetCardColorSprite("Skill_"+color);
        }
        if (info != null)
        {
            if (descript != null)
            {
                descript.text = info.description[0];
            }
            if (nameSkill != null)
                nameSkill.text = cardBuff.name;
        }
    }
    public void UpdateHeroSkillInfo( long shardAdd, long curShardFree)
    {
    //    if (fillImg != null)
    //    {
    //        fillImg.gameObject.SetActive(true);
    //        int count = this.data.min_shard + this.shardFree;
    //        foreach (Image s in shardImage)

    //        {
    //            s.color = new Color(0.5f, 0.5f, 0.5f);
    //        }
    //        for (int i = 0; i < count; i++)
    //        {
    //            if (shardAdd < this.data.min_shard)
    //            {
    //                if (i < shardAdd)
    //                    shardImage[i].color = new Color(1, 1, 1);
    //            }
    //            else
    //            {
    //                // ?? min shard m?i cong den free shard
    //                if (i < data.min_shard + curShardFree)
    //                    shardImage[i].color = new Color(1, 1, 1);
    //            }
    //        }
    //        if (shardAdd < this.data.min_shard)
    //        {
    //            fillImg.fillAmount =1f- ((float)shardAdd / (float)count);
    //        }
    //        else
    //        {
    //            if (curShardFree < this.shardFree)
    //                fillImg.fillAmount = 1f-((float)(curShardFree + this.data.min_shard) / (float)count);
    //            else
    //                fillImg.fillAmount = 0;
    //        }
    //    }
       
    }    
}
