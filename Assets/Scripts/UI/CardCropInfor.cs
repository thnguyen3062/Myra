using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardCropInfor : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TxtName, m_TxtNumber, m_TxtMana;
    //[SerializeField] private Image m_Ulti, m_Skill1, m_Skill2;
    //[SerializeField] private Image m_Frame;
    //[SerializeField] private List<Image> m_ListShardCrystal;
    public void SetInfo(DBHero db, int countHC)
    { 
        //if(db.type == DBHero.TYPE_GOD)
        //{
        //    Sprite frameUlti = Resources.Load<Sprite>("Pack/Images/Frame/ShopItemGodFrame/ShopItemGodUlti_" + Database.GetHero(db.id).color);
        //    if (frameUlti != null)
        //        m_Ulti.sprite = frameUlti;

        //    CardSkillDataInfo cardSkill = CardData.Instance.GetCardSkillDataInfo(db.id);

        //    if (cardSkill != null && cardSkill.skillIds.Count >= 2)
        //    {
        //        Sprite skill1 = Resources.Load<Sprite>("Pack/Images/UltimateSkillNew/" + cardSkill.skillIds[0]);
        //        if (skill1 != null)
        //        {
        //            m_Skill1.sprite = skill1;
        //        }

        //        Sprite skill2 = Resources.Load<Sprite>("Pack/Images/UltimateSkillNew/" + cardSkill.skillIds[1]);
        //        if (skill2 != null)
        //        {
        //            m_Skill2.sprite = skill2;
        //        }
        //    }
        //}
        //Sprite frameRes = Resources.Load<Sprite>("Pack/Images/Frame/ShopItemNormalFrame/ShopItemNormalFrame_" + Database.GetHero(db.id).color);
        //if (frameRes != null)
        //    m_Frame.sprite = frameRes;

        //for (int i = 0; i < m_ListShardCrystal.Count; i++)
        //{
        //    Sprite shardIcon = Resources.Load<Sprite>("Pack/Images/Shard/" + Database.GetHero(db.id).color);
        //    if (shardIcon != null)
        //        m_ListShardCrystal[i].sprite = shardIcon;
        //    m_ListShardCrystal[i].gameObject.SetActive(i < db.shardRequired);
        //}
        m_TxtName.text = db.name.ToString();
        m_TxtNumber.text= countHC.ToString();
        m_TxtMana.text = db.mana.ToString();
    }
}
