using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAuctionPreview : MonoBehaviour
{
    [SerializeField] private SkillAuctionInfo m_Skill1, m_Skill2;
    // Start is called before the first frame update
    public void SetData(List<long> lst)
    {
        if(lst.Count>1)
        {
            m_Skill1.InitData(lst[0]);
            m_Skill1.gameObject.SetActive(true); 
            m_Skill2.InitData(lst[1]);
            m_Skill2.gameObject.SetActive(true); 
        }  
        else
        {
            m_Skill1.InitData(lst[0]);
            m_Skill1.gameObject.SetActive(true);
            m_Skill2.gameObject.SetActive(false);
        }    
    }    
    // Update is called once per frame 
    void Update()
    {   
        
    }
}
