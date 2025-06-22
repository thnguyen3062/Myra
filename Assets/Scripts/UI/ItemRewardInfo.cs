using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemRewardInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TypeItem;
    [SerializeField] private TextMeshProUGUI m_Detail;
    [SerializeField] private TextMeshProUGUI m_Quantity;
    public GameObject line;
    [Header("Deck")]
    [SerializeField] private Image m_ItemImage;
    [SerializeField] private Image m_FrameImage;
    [Header("TimeChest")]
    [SerializeField] private GameObject[] m_TypeChest;
    // Start is called before the first frame update
    public void InitData(string type, string detail,long quantity=0,long typeTimeChest=0, long heroId = -1)
    {
        m_TypeItem.text = type;
        m_Detail.text = detail;
        if(m_Quantity!= null)
        {
            if (quantity > 1)
            {
                m_Quantity.text = quantity.ToString();
                m_Quantity.transform.parent.gameObject.SetActive(true);
            }
            else
                m_Quantity.transform.parent.gameObject.SetActive(false);

        }    
        if(typeTimeChest!=0)
        {
            switch (typeTimeChest)
            {
                case 3:
                    {
                        m_TypeChest[0].SetActive(true);
                        m_Detail.text = "Standard Chest";
                        break;
                    }
                case 8:
                    {
                        m_TypeChest[1].SetActive(true);
                        m_Detail.text = "Premium Chest";
                        break;
                    }
                case 12:
                    {
                        m_TypeChest[2].SetActive(true);
                        m_Detail.text = "Godly Chest";
                        break;
                    }
                case 1000:
                    {
                        m_TypeChest[0].SetActive(true);
                        m_Detail.text = "5s Chest";
                        break;
                    }
            }
        }    
    }     
}

