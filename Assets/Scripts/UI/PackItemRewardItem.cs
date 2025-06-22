using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PackItemRewardItem : MonoBehaviour
{
    // Fields
    [SerializeField] private Image m_ImageCard, m_Frame;
    [SerializeField] private TextMeshProUGUI m_Mana, m_CardName, m_Count;

    // Values

    // Methods
    public void SetData(ItemReward data)
    {
        //m_CardName.text = data;
        //m_Count.text = "X" + data.count;
    }
}
