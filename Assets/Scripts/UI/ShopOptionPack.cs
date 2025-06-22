using DG.Tweening;
using GIKCore.Lang;
using GIKCore.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopOptionPack : MonoBehaviour
{
    // Fields
    [SerializeField] private TextMeshProUGUI m_Count, m_Cost;
    [SerializeField] private GoOnOff m_Currency, m_Background, m_Topline, m_Botline;
    [SerializeField] private GameObject m_Block;
    [SerializeField] private Image m_SelectedHighlight;

    // Values
    public bool isSelected;

    // Methods
    public void SetData(ItemPackage data)
    {
        m_Count.text = data.count + LangHandler.Get("104","Packs");
        m_Cost.text = data.price + "";
        m_Currency.Turn(data.currency == 1);
        isSelected = data.isSelected;
        m_Topline.Turn(isSelected);
        m_Botline.Turn(isSelected);
        if (isSelected)
        {
            m_Background.TurnOn();
            m_SelectedHighlight.DOFillAmount(1f, 0.2f);
        }
        else
        {
            m_SelectedHighlight.DOFillAmount(0f, 0.2f).OnComplete(() =>
            {
                m_Background.TurnOff();
            });
        }
    }
}
