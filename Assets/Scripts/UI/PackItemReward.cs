using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackItemReward : MonoBehaviour
{
    // Fields
    [SerializeField] private PackItemRewardGod m_God;
    [SerializeField] private PackItemRewardMortal m_Mortal;
    [SerializeField] private PackItemRewardSpell m_Spell;
    [SerializeField] private PackItemRewardItem m_Item;

    // Values

    // Methods
    public void SetData(ItemReward data)
    {
        // 0 => card, != 0 => not card
        if (data.kind == 0)
        {
            DBHero hero = Database.GetHero(data.id);
            switch (hero.type)
            {
                case DBHero.TYPE_GOD:
                    {
                        m_God.SetData(data);
                        m_God.gameObject.SetActive(true);
                        m_Spell.gameObject.SetActive(false);
                        m_Mortal.gameObject.SetActive(false);
                        m_Item.gameObject.SetActive(false);
                        break;
                    }
                case DBHero.TYPE_TROOPER_NORMAL:
                    {
                        m_Mortal.SetData(data);
                        m_Mortal.gameObject.SetActive(true);
                        m_God.gameObject.SetActive(false);
                        m_Spell.gameObject.SetActive(false);
                        m_Item.gameObject.SetActive(false);
                        break;
                    }
                case DBHero.TYPE_TROOPER_MAGIC:
                    {
                        m_Spell.SetData(data);
                        m_Spell.gameObject.SetActive(true);
                        m_Mortal.gameObject.SetActive(false);
                        m_God.gameObject.SetActive(false);
                        m_Item.gameObject.SetActive(false);
                        break;
                    }
                case DBHero.TYPE_BUFF_MAGIC:
                    {
                        m_Spell.SetData(data);
                        m_Spell.gameObject.SetActive(true);
                        m_Mortal.gameObject.SetActive(false);
                        m_God.gameObject.SetActive(false);
                        m_Item.gameObject.SetActive(false);
                        break;
                    }
            }
        }
        else
        {
            m_Item.SetData(data);
            m_Item.gameObject.SetActive(false);
            m_God.gameObject.SetActive(false);
            m_Spell.gameObject.SetActive(false);
            m_Mortal.gameObject.SetActive(false);
        }
    }
}
