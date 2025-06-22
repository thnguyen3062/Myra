using DG.Tweening;
using GIKCore.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SidebarTabOption : MonoBehaviour
{
    // Fields
    [SerializeField] private GoOnOff m_TopLine, m_BottomLine, m_Background, m_Icon;
    [SerializeField] private Image m_SelectedHighlight;
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] public int m_TabId;

    // Values
    [HideInInspector]
    private bool isActive;
    // Methods
    public SidebarTabOption SetActive(bool active)
    { 
        isActive = active;
        if (active)
        {
            m_TopLine.TurnOn();
            m_BottomLine.TurnOn();
            m_Background.TurnOn();
            m_Icon.TurnOn();
            Color color;
            ColorUtility.TryParseHtmlString("#ffffff", out color);
            m_SelectedHighlight.DOFillAmount(1f, 0.2f);
            m_Title.color = color;
        }
        else
        {
            m_SelectedHighlight.DOFillAmount(0f, 0.2f).OnComplete(() =>
            {
                m_Background.TurnOff();
            });
            m_TopLine.TurnOff();
            m_BottomLine.TurnOff();
            m_Icon.TurnOff();
            Color color;
            ColorUtility.TryParseHtmlString("#757471", out color);
            m_Title.color = color;
        }
        return this;
    }
}
