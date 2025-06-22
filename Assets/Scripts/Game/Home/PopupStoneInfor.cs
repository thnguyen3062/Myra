using GIKCore.Lang;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupStoneInforProps
{
    public int level;
    public int matchCoin;
    public int matchCup;
    public int matchEssence;
    public int chestCoin;
    public int chestCup;
    public int chestEssence;
    public string levelDesc;
}

public class PopupStoneInfor : MonoBehaviour
{
    // Fields
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_MatchCoin;
    [SerializeField] private TextMeshProUGUI m_MatchCup;
    [SerializeField] private TextMeshProUGUI m_MatchEssence;

    [SerializeField] private TextMeshProUGUI m_ChestCoin;
    [SerializeField] private TextMeshProUGUI m_ChestCup;
    [SerializeField] private TextMeshProUGUI m_ChestEssence;
    [SerializeField] private TextMeshProUGUI m_TextLevelDesc;

    // Values
    private PopupStoneInforProps dataProps;

    // Methods
    public void DoShow(PopupStoneInforProps data)
    {
        dataProps = data;
        m_Title.text = LangHandler.Get("761", "LEVEL") + " " + data.level + " " + LangHandler.Get("762", "DETAILS");
        m_MatchCoin.text = string.Format("{1}: <color=#00FF29>{0}</color>", dataProps.matchCoin, LangHandler.Get("643", "COINS"));
        m_MatchCup.text = string.Format("{1}: <color=#00FF29>{0}</color>", dataProps.matchCup, LangHandler.Get("644", "CUPS"));
        m_MatchEssence.text = string.Format("{1}: <color=#00FF29>{0}</color>", dataProps.matchEssence, LangHandler.Get("645", "ESSENCE"));
        m_ChestCoin.text = string.Format("{1}: <color=#00FF29>{0}</color>", dataProps.chestCoin, LangHandler.Get("643", "COINS"));
        m_ChestCup.text = string.Format("{1}: <color=#00FF29>{0}</color>", dataProps.chestCup, LangHandler.Get("644", "CUPS"));
        m_ChestEssence.text = string.Format("{1}: <color=#00FF29>{0}</color>", dataProps.chestEssence, LangHandler.Get("645", "ESSENCE"));
        m_TextLevelDesc.text = data.levelDesc;
        gameObject.SetActive(true);
    }
    public void DoClose()
    {
        gameObject.SetActive(false);
    }
}
