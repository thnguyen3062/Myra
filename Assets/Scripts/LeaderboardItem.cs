using GIKCore.Lang;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankIndex;
    [SerializeField] private TextMeshProUGUI userName;
    [SerializeField] private TextMeshProUGUI winCount;

    public void InitItem(int index, string username, long count)
    {
        if (rankIndex != null)
        {
            if (index == 0)
                rankIndex.text = LangHandler.Get("123", "Unranked");
            else
                rankIndex.text = index.ToString();
        }
        userName.text = username;
        winCount.text = count.ToString();
    }
}
