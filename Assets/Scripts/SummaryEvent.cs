using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SummaryEvent : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalMatch;
    [SerializeField] private TextMeshProUGUI totalWin;

    public void OnInitSummary(long match, long win)
    {
        totalMatch.text = match.ToString();
        totalWin.text = win.ToString();
    }
}
