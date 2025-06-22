using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeChestNotificationInfo : MonoBehaviour
{
    [SerializeField] private Image[] m_TypeChest;
    [SerializeField] private TextMeshProUGUI m_Detail;
    // Start is called before the first frame update
    public void InitData(long typeTimeChest, bool hasTimeChest, string text)
    {
        foreach (Image i in m_TypeChest)
            i.gameObject.SetActive(false);
        switch (typeTimeChest)
        {
            case 3:
                {
                    m_TypeChest[0].gameObject.SetActive(true);
                    m_TypeChest[0].color = hasTimeChest ? new Color(1,1,1,1):new Color(1,1,1,0.5f);
                    m_Detail.text = text;
                    break;
                }
            case 8:
                {
                    m_TypeChest[1].gameObject.SetActive(true);
                    m_TypeChest[1].color = hasTimeChest ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0.5f);
                    m_Detail.text = text;
                    break;
                }
            case 12:
                {
                    m_TypeChest[2].gameObject.SetActive(true);
                    m_TypeChest[2].color = hasTimeChest ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0.5f);
                    m_Detail.text = text;
                    break;
                }
        }
    }    
}
