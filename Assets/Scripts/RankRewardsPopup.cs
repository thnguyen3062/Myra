using GIKCore;
using GIKCore.Net;
using GIKCore.Utilities;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankRewardsPopup : GameListener
{
    [SerializeField] private GameObject rankRewardsPrefab, preIcon, nextIcon;
    [SerializeField] private TextMeshProUGUI timeUpdate;
    [SerializeField] Transform lstRank;
    public void InitData(List<RewardRank> data)
    {
        this.gameObject.SetActive(true);
        for(int i = 0; i < data.Count; i++)
        {
            Transform trans = PoolManager.Pools["RankRewardItem"].Spawn(rankRewardsPrefab);
            trans.GetComponent<RankRewardItem>().SetData(data[i]);
            trans.gameObject.SetActive(true);
            trans.SetParent(lstRank);
            trans.localScale = Vector3.one;
        }    
    }
    public void DoClickQuestion()
    {
        PopupConfirm.Show(content: "Higher ranks will unlock more valuable rewards, so try your best" + "'\n" +
            " Each season has a pool of rewards. Every players when reaching a specific rank the first time can claim them once per season.",
            action1: "Back");
    }
    public void DoClickBack()
    {
        this.gameObject.SetActive(false);
    }
    public void DoClickLeaderBoard()
    {
        Game.main.socket.GetLeaderboard();
    }

}
