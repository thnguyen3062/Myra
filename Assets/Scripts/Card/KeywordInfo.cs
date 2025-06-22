using GIKCore.DB;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeywordInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI keywordSprite;
    [SerializeField] private TextMeshProUGUI descript;
    public void InitData(string keySprite,string keyData,long count1=-1)
    {
        keywordSprite.text = keySprite;
        string desc = "";
        if (GamePrefs.LastLang==1)
            desc = Database.GetKeywordInfo(keyData).descriptionEN;
        else if (GamePrefs.LastLang==2)
            desc = Database.GetKeywordInfo(keyData).descriptionJP;
        else if (GamePrefs.LastLang == 3)
            desc = Database.GetKeywordInfo(keyData).descriptionKR;
        else if (GamePrefs.LastLang == 4)
            desc = Database.GetKeywordInfo(keyData).descriptionCN;
        else if (GamePrefs.LastLang == 5)
            desc = Database.GetKeywordInfo(keyData).descriptionTW;
        if (count1!= -1)
        {
            descript.text = string.Format(desc, count1);
        }
        descript.font = FontData.Instance.GetFontDataInfo((long)GamePrefs.LastLang).fontAsset;
    }    
}
