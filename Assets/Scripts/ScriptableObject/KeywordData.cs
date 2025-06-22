using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/KeywordData")]
public class KeywordData : ScriptableObject
{
    private static KeywordData instance;
    public static KeywordData Instance
    {
        get
        {
            if(instance == null)
            {
                return instance = Resources.Load<KeywordData>("Data/KeywordData");
            }
            return instance;
        }
    }
    public List<KeywordDataInfo> keywordDataInfo;
}
