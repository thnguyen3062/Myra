using System.Collections;
using System.Collections.Generic;
using GIKCore.Sound;
using GIKCore;
using GIKCore.Net;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GIKCore.UI;
using pbdson;
using TMPro;
using GIKCore.Lang;

public class ProfileScene : GameListener
{
    [SerializeField] private GameObject SettingUI;
    [SerializeField] private TextMeshProUGUI playTimeText;
    [SerializeField] private TextMeshProUGUI totalMatchText;
    [SerializeField] private TextMeshProUGUI totalWinText;
    [SerializeField] private TextMeshProUGUI winrateText;
    [SerializeField] private TextMeshProUGUI userNickName;

    public override bool ProcessSocketData(int id, byte[] data)
    {
        if (base.ProcessSocketData(id, data))
            return true;

        switch (id)
        {
            case IService.GET_PROFILE:
                CommonVector cv = ISocket.Parse<CommonVector>(data);
                ProcessProfile(cv);
                break;
        }

        return false;
    }

    void ProcessProfile(CommonVector cv)
    {
        //Debug.LogError(string.Join(",", cv.aLong));
        //Debug.LogError(string.Join(",", cv.aString));
        userNickName.text = GameData.main.profile.screenname;

        totalMatchText.text = cv.aLong[0].ToString();
        totalWinText.text = cv.aLong[1].ToString();
        if (cv.aLong[0] > 0)
            winrateText.text = Mathf.Round((float)cv.aLong[1] / (float)cv.aLong[0] * 100) + "%";
        else
            winrateText.text = "0%";
    }

    public void DoClickPlay()
    {
        Game.main.socket.GetUserDeck();
        Game.main.socket.GetRank();
        Game.main.LoadScene("SelectDeckScene", delay: 0.3f, curtain: true);
    }
    public void DoClickBack()
    {
        Game.main.LoadScene("HomeSceneNew", delay: 0.3f, curtain: true);
    }
    public void DoClickSetting()
    {
       SettingUI.SetActive(true);
    }
    public void DoClickDisableButton()
    {
        Toast.Show(LangHandler.Get("toast-8", "Coming Soon!"));
    }
    public void DoClickLeaderBoard()
    {
        Game.main.socket.GetLeaderboard();
        Game.main.LoadScene("LeaderBoardScene", delay: 0.5f, curtain: true);
    }
}
