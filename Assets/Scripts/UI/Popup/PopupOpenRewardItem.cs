using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupOpenRewardItemProps
{
    public bool isGem = false;
    public bool isCoin = false;
    public bool isItem = false;
    public bool isChest3H = false;
    public bool isChest8H = false;
    public bool isChest12H = false;
    public bool isDeck = false;
    public bool isCard = false;
    public bool isExp = false;
    public bool isEssence = false;

    //deck
    public int deckId = 0;
    //hero card
    public DBHero hero = null;
    public int frameCard = 0;
    public int countCard = 0;
    //item
    public Sprite itemSprite = null;
    public string itemName = "";
    public int itemCount = 0;
    //exp
    public int exp = 0;
    //essence
    public int essence = 0;
    //gold
    public int gold = 0;
    // gem
    public int gem = 0;
}
public class PopupOpenRewardItem : MonoBehaviour
{
    // Fields
    [SerializeField] private GameObject m_Coin;
    [SerializeField] private GameObject m_Gem;
    [SerializeField] private GameObject m_Item;
    [SerializeField] private GameObject m_Chest3H;
    [SerializeField] private GameObject m_Chest8H;
    [SerializeField] private GameObject m_Chest12H;
    [SerializeField] private GameObject m_Deck;
    [SerializeField] private GameObject m_Card;
    [SerializeField] private GameObject m_Exp;
    [SerializeField] private GameObject m_Essence;
    [SerializeField] private List<GameObject> m_ListDeck;
    [SerializeField] private CardUserInfor m_CardGod;
    [SerializeField] private CardUserInfor m_CardSpell;
    [SerializeField] private CardUserInfor m_CardMinion;
    [SerializeField] private TextMeshProUGUI m_TextName;

    // Values
    private PopupOpenRewardItemProps dataProps = null;

    // Methods
    public PopupOpenRewardItem SetData(PopupOpenRewardItemProps props)
    {
        dataProps = props;
        if (dataProps != null)
        {
            m_Coin.SetActive(dataProps.isCoin);
            m_Gem.SetActive(dataProps.isGem);
            m_Item.SetActive(dataProps.isItem);
            m_Chest3H.SetActive(dataProps.isChest3H);
            m_Chest8H.SetActive(dataProps.isChest8H);
            m_Chest12H.SetActive(dataProps.isChest12H);
            m_Deck.SetActive(dataProps.isDeck);
            m_Card.SetActive(dataProps.isCard);
            m_Exp.SetActive(dataProps.isExp);
            m_Essence.SetActive(dataProps.isEssence);

            if (dataProps.isCoin)
            {
                m_TextName.text = "Coin X" + props.gold;
            }
            else if (dataProps.isGem)
            {
                m_TextName.text = "Gem X" + props.gem;
            }
            else if (dataProps.isItem)
            {
                m_TextName.text = dataProps.itemName + " X" + props.itemCount;
                if (dataProps.itemSprite != null)
                    m_Item.GetComponent<Image>().sprite = dataProps.itemSprite;
            }
            else if (dataProps.isChest3H)
            {
                m_TextName.text = "Standard Chest";
            }
            else if (dataProps.isChest8H)
            {
                m_TextName.text = "Premium Chest";
            }
            else if (dataProps.isChest12H)
            {
                m_TextName.text = "Godly Chest";
            }
            else if (dataProps.isDeck)
            {
                for(int i = 0; i < m_ListDeck.Count; i++)
                {
                    m_ListDeck[i].SetActive(i == dataProps.deckId - 1);
                }
                switch (dataProps.deckId)
                {
                    case 1:
                        {
                            m_TextName.text = "Green Deck";                            
                            break;
                        }
                    case 2:
                        {
                            m_TextName.text = "Red Deck";
                            break;
                        }
                    case 3:
                        {
                            m_TextName.text = "Yellow Deck";
                            break;
                        }
                    case 4:
                        {
                            m_TextName.text = "Purple Deck";
                            break;
                        }
                    default:
                        {
                            m_TextName.text = "Deck";
                            break;
                        }
                }
            }
            else if (dataProps.isCard)
            {
                switch (dataProps.hero.type)
                {
                    case DBHero.TYPE_GOD:
                        {
                            m_CardGod.gameObject.SetActive(true);
                            m_CardMinion.gameObject.SetActive(false);
                            m_CardSpell.gameObject.SetActive(false);

                            m_CardGod.SetInfoCard(dataProps.hero, 1, dataProps.frameCard);
                            break;
                        }
                    case DBHero.TYPE_TROOPER_NORMAL:
                        {
                            m_CardGod.gameObject.SetActive(false);
                            m_CardMinion.gameObject.SetActive(true);
                            m_CardSpell.gameObject.SetActive(false);

                            m_CardMinion.SetInfoCard(dataProps.hero, 1, dataProps.frameCard);
                            break;
                        }
                    case DBHero.TYPE_TROOPER_MAGIC:
                    case DBHero.TYPE_BUFF_MAGIC:
                        {
                            m_CardGod.gameObject.SetActive(false);
                            m_CardMinion.gameObject.SetActive(false);
                            m_CardSpell.gameObject.SetActive(true);

                            m_CardSpell.SetInfoCard(dataProps.hero, 1, dataProps.frameCard);
                            break;
                        }
                }
                m_TextName.text = dataProps.hero.name + " X" + dataProps.countCard;
            }
            else if (dataProps.isExp)
            {
                m_TextName.text = "EXP X" + dataProps.exp;
            }
            else if (dataProps.isEssence)
            {
                m_TextName.text = "Essence X" + dataProps.essence;
            }

        }
        return this;
    }
}
