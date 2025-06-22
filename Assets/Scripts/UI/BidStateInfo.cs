using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BidStateInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_PlayerShardText, m_OpponentShardText, m_PlayerBidShardText, m_OppBidShardText;
    [SerializeField] private SkillAuctionInfo skillDesc;
    [SerializeField] private TextMeshProUGUI m_TimeCountDown;
    public int countdown = 15;
    bool isTouch = false;
    // Start is called before the first frame update
     public void SetData(long playerShard, long oppShard, long playerBid,long oppBid,bool startcCount = false, long timeRemain =-1)
    {
        if(m_PlayerShardText!= null)
            m_PlayerShardText.text = playerShard.ToString();
        if (m_OpponentShardText != null)
            m_OpponentShardText.text = oppShard.ToString();
        if (m_OppBidShardText != null)
            m_OppBidShardText.text = oppBid.ToString();
        if (m_PlayerBidShardText != null)
            m_PlayerBidShardText.text = playerBid.ToString();
        skillDesc.InitData(GameData.main.skillIDBid);
        if(timeRemain != -1)
        {
            countdown = (int)timeRemain;
        }
        if (m_TimeCountDown != null)
        {
            if (startcCount)
            {

                m_TimeCountDown.DOCounter(countdown, 0, countdown).SetEase(Ease.Linear).onComplete += delegate
                {
                };
            }
            else
            {
                m_TimeCountDown.text = "0";
            }    
        }
     }    

    // Update is called once per frame
    void Update()
    {
    }
}
