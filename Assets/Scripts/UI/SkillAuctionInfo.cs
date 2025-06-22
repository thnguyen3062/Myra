using GIKCore.DB;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillAuctionInfo : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI m_TextDesc;
    long skillID;
    DBHeroSkill dbSkill;
    public void InitData(long skillID)
    {
        dbSkill = Database.GetDBSkillAuction(skillID);
        if (dbSkill != null)
        {
            SkillAuctionDataInfo info = CardData.Instance.GetSkillBidInfo(skillID);

            string desc = "";
            if (GamePrefs.LastLang == 1)
                desc = info.descEN;
            else if (GamePrefs.LastLang == 2)
                desc = info.descJP;
            else if (GamePrefs.LastLang == 3)
                desc = info.descKR;
            else if (GamePrefs.LastLang == 4)
                desc = info.descCN;
            else if (GamePrefs.LastLang == 5)
                desc = info.descTW;
            m_TextDesc.text = desc;
            m_TextDesc.font = FontData.Instance.GetFontDataInfo((long)GamePrefs.LastLang).fontAsset;
        }
    }
}
