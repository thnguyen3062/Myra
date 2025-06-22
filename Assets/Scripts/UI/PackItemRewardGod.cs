using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PackItemRewardGod : MonoBehaviour
{
    // Fields
    [SerializeField] private Image m_ImageCard, m_Frame, m_Ulti, m_Skill1, m_Skill2;
    [SerializeField] private TextMeshProUGUI m_Mana, m_CardName, m_Count;

    // Values

    // Methods
    public void SetData(ItemReward data)
    {
        DBHero hero = Database.GetHero(data.id);


        //Sprite cardRes = Resources.Load<Sprite>("Pack/Images/CardOnBoard_" + Database.GetHero(hero.id).color + "/" + hero.id);
        Sprite cardRes = CardData.Instance.GetOnBoardSprite(hero.id);
        if (cardRes != null)
            m_ImageCard.sprite = cardRes;

        //Sprite frameRes = Resources.Load<Sprite>("Pack/Images/Frame/ShopItemGodFrame_" + Database.GetHero(hero.id).color);
        Sprite frameRes = CardData.Instance.GetShopItemGodFrame(hero.id);
        if (frameRes != null)
            m_Frame.sprite = frameRes;

        //Sprite frameUlti = Resources.Load<Sprite>("Pack/Images/Frame/ShopItemGodUlti_" + Database.GetHero(hero.id).color);
        //Sprite frameUlti = CardData.Instance.GetShopItemGodUlti(hero.id);
        //if (frameUlti != null)
        //    m_Ulti.sprite = frameUlti;

        //CardSkillDataInfo cardSkill = CardData.Instance.GetCardSkillDataInfo(hero.id);

        //if(cardSkill != null && cardSkill.skillIds.Count >= 2)
        //{
        //    //Sprite skill1 = Resources.Load<Sprite>("Pack/Images/UltimateSkillNew/" + cardSkill.skillIds[0]);
        //    Sprite skill1 = CardData.Instance.GetUltiSprite(cardSkill.skillIds[0].ToString());
        //    if (skill1 != null)
        //    {
        //        m_Skill1.sprite = skill1;
        //    }

        //    //Sprite skill2 = Resources.Load<Sprite>("Pack/Images/UltimateSkillNew/" + cardSkill.skillIds[1]);
        //    Sprite skill2 = CardData.Instance.GetUltiSprite(cardSkill.skillIds[1].ToString());
        //    if (skill2 != null)
        //    {
        //        m_Skill2.sprite = skill2;
        //    }
        //}

        m_Mana.text = hero.mana + "";
        m_CardName.text = hero.name;
        m_Count.text = "X" + data.count;
    }
}
