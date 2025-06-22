using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellRankDetail : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Rank;
    [SerializeField] private TextMeshProUGUI m_ScreenName;
    [SerializeField] private TextMeshProUGUI m_Elo;
    [SerializeField] private List<GameObject> m_ListBackground;
    [SerializeField] private List<GameObject> m_ListRankIcon;
    [SerializeField] private RankTierLeaderboard m_RankTierLeaderboard;
    // Values

    // Methods
    public CellRankDetail SetData(CellRankProps data)
    {
        m_Rank.text = (data.rankIndex + 1) + "";
        m_Elo.text = data.eloPoint + "";
        m_ScreenName.text = data.screenName;

        foreach (GameObject bg in m_ListBackground)
        {
            bg.SetActive(false);
        }
       
        foreach (GameObject icon in m_ListRankIcon)
        {
            icon.SetActive(false);
        }
        if (data.rankIndex < 3)
        {
            m_ListBackground[data.rankIndex].SetActive(true);
            m_ListRankIcon[data.rankIndex].SetActive(true);
        }
        m_RankTierLeaderboard.SetData(data.rankId);
        return this;
    }
}
