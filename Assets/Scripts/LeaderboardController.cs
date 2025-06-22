using GIKCore;
using GIKCore.Lang;
using GIKCore.Net;
using GIKCore.UI;
using pbdson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardController : GameListener
{
    [SerializeField] private List<LeaderboardItem> lstLeaderboardItem;
    [SerializeField] private Transform leaderItemParent;

    public override bool ProcessSocketData(int id, byte[] data)
    {
        if (base.ProcessSocketData(id, data))
            return true;

        switch (id)
        {
            case IService.GET_LEADER_BOARD:
                CommonVector cv = ISocket.Parse<CommonVector>(data);
                ProcessLeaderboardData(cv);
                break;
        }

        return false;
    }

    void ProcessLeaderboardData(CommonVector cv)
    {
        LogWriterHandle.WriteLog(string.Join(",", cv.aLong));
        LogWriterHandle.WriteLog(string.Join(",", cv.aString));

        var listName = cv.aString.GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).ToList();

        //int index = 0;
        for(int i = 0; i < cv.aLong.Count; i++)
        {
            lstLeaderboardItem[i].gameObject.SetActive(true);
            lstLeaderboardItem[i].InitItem(i, cv.aString[i * 2 + 1], cv.aLong[i]);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(leaderItemParent.GetComponent<RectTransform>());
    }

    public void OnBack()
    {
        Game.main.socket.GetProfile();
        Game.main.LoadScene("ProfileScene", delay: 0.3f, curtain: true);
    }
    public void DoClickDisableButton()
    {
        Toast.Show(LangHandler.Get("toast-8", "Coming Soon!"));
    }
}
