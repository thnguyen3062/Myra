using GIKCore;
using GIKCore.Lang;
using GIKCore.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UIEngine.UIPool;
using UnityEngine;

public class CellRankProps
{
    public int rankIndex;
    public int rankId;
    public int eloPoint;
    public string screenName;
    public string userName;
}
public class PanelRankLeaderboard : MonoBehaviour
{
    // Fields
    [SerializeField] private VerticalPoolGroup m_Pool;
    [SerializeField] private List<GameObject> m_ListCurUserRankIcons;
    [SerializeField] private TextMeshProUGUI m_CurUserRankIndex;
    [SerializeField] private TextMeshProUGUI m_CurUserRankElo;
    [SerializeField] private TextMeshProUGUI m_CurUserScreenName;
    [SerializeField] private TextMeshProUGUI m_TimeUpdate;
    [SerializeField] private RankTierLeaderboard m_RankTierLeaderboard;
    // Values

    // Fields
    public PanelRankLeaderboard SetData(List<CellRankProps> cellRanks, CellRankProps curUserRank, int timeUpdate)
    {
        gameObject.SetActive(true);
        m_Pool.SetAdapter(cellRanks); 

        foreach(GameObject go in m_ListCurUserRankIcons)
        {
            go.SetActive(false);
        }
        if(curUserRank.rankIndex >= 1 && curUserRank.rankIndex <= 3)
        {
            m_ListCurUserRankIcons[curUserRank.rankIndex - 1].SetActive(true);
        }
        m_CurUserRankIndex.text = (curUserRank.rankIndex != 0 ? curUserRank.rankIndex + "" : "-");
        m_CurUserScreenName.text = curUserRank.screenName;
        m_CurUserRankElo.text = curUserRank.eloPoint + "";
        m_RankTierLeaderboard.SetData(curUserRank.rankId);
        DateTime timestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(timeUpdate);
        m_TimeUpdate.text = LangHandler.Get("202","Last Update") +": " + (timestamp.Hour < 10 ? "0" + timestamp.Hour : "" + timestamp.Hour) + (timestamp.Minute < 10 ? "0" + timestamp.Minute : "" + timestamp.Minute);
        return this;
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    public void DoClickDisableButton()
    {
        Toast.Show(LangHandler.Get("toast-8", "Coming Soon!"));
    } 
    public void DoClickReward()
    {
        Game.main.socket.ViewReward();
    }    
    void Awake()
    {
        m_Pool.SetCellPrefabCallback((index) =>
        {
            return index < 3 ? m_Pool.GetDeclarePrefab(0) : m_Pool.GetDeclarePrefab(1);
        });
        m_Pool.SetCellSizeCallback((index) =>
        {
            return index < 3 ? new Vector2(1260f, 107f) : new Vector2(1260f, 80f);
        });
        m_Pool.SetCellDataCallback((GameObject go, CellRankProps data, int index) =>
        {
            go.GetComponent<CellRankDetail>().SetData(data);
        });
    }
}
