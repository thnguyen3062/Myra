using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/FontData")]
public class FontData : ScriptableObject
{
    private static FontData instance;
    public static FontData Instance
    {
        get
        {
            if (instance == null)
            {
                return instance = Resources.Load<FontData>("Data/FontData");
            }
            return instance;
        }
    }

    public List<FontDataInfo> fontDataInfos;

    public FontDataInfo GetFontDataInfo(long id)
    {
        FontDataInfo info = fontDataInfos.FirstOrDefault(x => x.m_LangID == id);
        if (info != null)
            return info;
        return null;
    }
}
