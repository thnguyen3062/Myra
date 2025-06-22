using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PackItemRewardMortal : MonoBehaviour
{
    // Fields
    [SerializeField] private Image m_ImageCard, m_Frame;
    [SerializeField] private TextMeshProUGUI m_Mana, m_CardName, m_Count;
    // Values

    // Methods
    public void SetData(ItemReward data)
    {
        DBHero hero = Database.GetHero(data.id);

        //Sprite dataNext = Resources.Load<Sprite>("Pack/Images/CardOnBoard_" + Database.GetHero(hero.id).color + "/" + hero.id);
        Sprite dataNext = CardData.Instance.GetOnBoardSprite(hero.id);
        if (dataNext != null)
            m_ImageCard.sprite = dataNext;

        //Sprite frameRes = Resources.Load<Sprite>("Pack/Images/Frame/ShopItemNormalFrame_" + Database.GetHero(hero.id).color);
        Sprite frameRes = CardData.Instance.GetShopItemNormalFrame(hero.id);
        if (frameRes != null)
            m_Frame.sprite = frameRes;


        //for(int i = 0; i < m_ListShardCrystal.Count; i++)
        //{
        //    //Sprite shardIcon = Resources.Load<Sprite>("Pack/Images/Shard/" + Database.GetHero(hero.id).color);
        //    Sprite shardIcon = CardData.Instance.GetShardSprite(Database.GetHero(hero.id).color);
        //    if (shardIcon != null)
        //        m_ListShardCrystal[i].sprite = shardIcon;
        //    m_ListShardCrystal[i].gameObject.SetActive(i < hero.shardRequired);
        //}

        m_Mana.text = hero.mana + "";
        m_CardName.text = hero.name;
        m_Count.text = "X" + data.count;
    }
}
