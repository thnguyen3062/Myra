using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillGodPreviewFull : MonoBehaviour
{
    [SerializeField] private GameObject skillInfoPrefab;
   
    private DBHero god;
    // Start is called before the first frame update
    void Start()
    {
        
    }
   public void InitSkill(long godId)
    {
        //god = Database.GetHero(godId);
        //int index = 0;
        //for (int i = 0; i < god.lstHeroSkill.Count; i++)
        //{
        //    if (CardData.Instance.GetCardSkillDataInfo(god.id).skillIds.Contains(god.lstHeroSkill[i].id))
        //    {
        //        index++;
        //        DBHeroSkill skill = god.lstHeroSkill[i];
        //        GameObject go = Instantiate(skillInfoPrefab, this.transform);
        //        SkillInfo info = go.GetComponent<SkillInfo>();
        //        info.InitDataSkill(skill, god.color, index);
        //    }
        //}    
    }    
}


