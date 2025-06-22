using GIKCore.Sound;
using GIKCore.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellCardUpgradeProps
{
    public long heroId;
    public int frameId;
    public int copyAvailable; //TOTAL CUA MK 
    public int copyRequired;  // DIEU KIEN 
    public bool isPointClick = false;
}
public class CellCardUpgrade : MonoBehaviour
{
    // Fields
    [SerializeField] private CardUpgrade m_CardUpgrade;
    [SerializeField] private Image m_FillBar;
    [SerializeField] private TextMeshProUGUI m_Count;
    [SerializeField] private GameObject m_BarGreen;
    [SerializeField] private GameObject m_BarBlue;
    [SerializeField] private GameObject m_ArrowUpgrade;
    [SerializeField] public GameObject m_PointClick;

    // Vales
    private CellCardUpgradeProps data;
    private ICallback.CallFunc2<long> onClickCB = null;

    // Methods
    public CellCardUpgrade SetData(CellCardUpgradeProps props)
    {
        data = props;
        m_CardUpgrade.SetFrame(data.frameId).SetMask(false).SetGlow(false).SetImage(data.heroId);
        if (m_FillBar != null)
            m_FillBar.fillAmount = data.copyAvailable > data.copyRequired ? 1 : (float)data.copyAvailable / data.copyRequired;
        if (m_Count != null)
            m_Count.text = data.copyAvailable + " / " + data.copyRequired;
        if(m_BarGreen != null && data.copyAvailable >= data.copyRequired)
        {
            m_BarGreen.SetActive(true);
            m_BarBlue.SetActive(false);
        }
        if (m_BarBlue != null && data.copyAvailable < data.copyRequired)
        {
            m_BarGreen.SetActive(false);
            m_BarBlue.SetActive(true);
        }
        if (m_ArrowUpgrade != null && data.copyAvailable >= data.copyRequired)
            m_ArrowUpgrade.SetActive(true);

        if (m_PointClick != null)
            m_PointClick.SetActive(data.isPointClick);
        return this;
    }
    public CellCardUpgrade SetOnclickCallBack(ICallback.CallFunc2<long> func)
    {
        onClickCB = func;
        return this;
    }
    public void DoClick()
    {
        SoundHandler.main.PlaySFX("900_click_4", "sounds");
        if (onClickCB != null)
            onClickCB(data.heroId);
    }
}
