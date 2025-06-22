using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserTrayShop : MonoBehaviour
{
    // Fields
    [SerializeField] private List<UserPackShop> m_ListUserPackShop;

    // Values
    private List<ItemInfo> listItemInfoData = new List<ItemInfo>();

    // Methods
    public UserTrayShop SetData(List<ItemInfo> lstData)
    {
        listItemInfoData = lstData;
        for(int i = 0; i < m_ListUserPackShop.Count; i++)
        {
            m_ListUserPackShop[i].gameObject.SetActive(false);
        }
        for(int i = 0; i < listItemInfoData.Count; i++)
        {
            m_ListUserPackShop[i].gameObject.SetActive(true);
            m_ListUserPackShop[i].SetData(listItemInfoData[i]);
        }
        return this;
    }
}
