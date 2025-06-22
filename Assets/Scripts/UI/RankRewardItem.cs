using GIKCore;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankRewardItem : MonoBehaviour
{

    [SerializeField] private Image rankIcon;
    [SerializeField] private Image rankFrame, rankFrameHL,rankFrameDisable;
    [SerializeField] private Transform iteamRewardContent;
    [SerializeField] private TextMeshProUGUI rankName;
    [SerializeField] private GameObject[] listItems;
    [SerializeField] private GameObject m_PopupClaim;
    private long rankID;
    private long rankSeasonID;
    private int count;
    private RewardRank data;
    public void SetData(RewardRank model )
    {
        this.gameObject.SetActive(true);
        data = model;
        rankID = model.rankID;
        rankSeasonID = model.rankSeasonID;
        rankIcon.sprite = CardData.Instance.GetRankSprite(model.rankID.ToString());
        rankFrame.sprite = CardData.Instance.GetRankSprite("fs_" + model.rankID.ToString());
        rankFrameHL.sprite = CardData.Instance.GetRankSprite("fs_hl_" + model.rankID.ToString());
        rankFrameDisable.sprite = CardData.Instance.GetRankSprite("fs_d_" + model.rankID.ToString());
        if(model.isAchieved)
        {
            if(!model.isReceive)
            {
                rankFrameHL.gameObject.SetActive(true);
                rankFrameDisable.gameObject.SetActive(false);
                rankFrame.gameObject.SetActive(false);
            } 
            else
            {
                rankFrameHL.gameObject.SetActive(false);
                rankFrameDisable.gameObject.SetActive(false);
                rankFrame.gameObject.SetActive(true);
            }    
        }  
        else
        {
            rankFrameHL.gameObject.SetActive(false);
            rankFrameDisable.gameObject.SetActive(true);
            rankFrame.gameObject.SetActive(false);
        }    
        DBRank rankInfo = Database.GetRank(rankID);
        rankName.text = rankInfo.name;
        if(model.gold>0)
        {
            listItems[0].SetActive(true);
            listItems[0].GetComponent<CellItemRankReward>().InitData(model.isAchieved,model.isReceive,model.gold);
            count++;
        }   
        if(model.card!=null)
        {
            listItems[1].SetActive(true);
            listItems[1].GetComponent<CellItemRankReward>().InitData(model.isAchieved, model.isReceive, 0,model.card);
            count++;
        } 
        if(model.item!=null)
        {
            listItems[2].SetActive(true);
            listItems[2].GetComponent<CellItemRankReward>().InitData(model.isAchieved, model.isReceive, 0,null, model.item);
            count++;
        }    

        if(count==1)
        {
            foreach (Transform item in iteamRewardContent)
                item.localScale = Vector3.one;
        }
        else
        {
            foreach(Transform item in iteamRewardContent)
                item.GetChild(0).localScale = new Vector3( 0.8f,0.8f,1);
        }
        if(!model.isReceive&& model.isAchieved)
            this.gameObject.GetComponent<Button>().interactable = true;
        else
            this.gameObject.GetComponent<Button>().interactable = false;
        this.gameObject.GetComponent<Button>().onClick.AddListener(OnClickReward) ;
    } 
    private void OnClickReward()
    {
        //gui goi reward
        Game.main.socket.GetReward(this.rankSeasonID, this.rankID);
        ModeSelection.instance.onReceiveSuccess += GetReward;
    }    
    private void GetReward()
    {
        this.gameObject.GetComponent<Button>().interactable = false;
        foreach(GameObject item in listItems)
            item.GetComponent<CellItemRankReward>().OnRecieveSuccess();
        m_PopupClaim.SetActive(true);
        foreach (Transform c in m_PopupClaim.transform.GetChild(0))
            c.gameObject.SetActive(false);
        if (data.gold > 0)
        {
            m_PopupClaim.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            m_PopupClaim.transform.GetChild(0).GetChild(0).GetComponent<CellItemRankReward>().InitData(false, false, data.gold);
            count++;
        }
        if (data.card != null)
        {
            m_PopupClaim.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
            m_PopupClaim.transform.GetChild(0).GetChild(1).GetComponent<CellItemRankReward>().InitData(false, false, 0, data.card);
            count++;
        }
        if (data.item != null)
        {
            m_PopupClaim.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
            m_PopupClaim.transform.GetChild(0).GetChild(2).GetComponent<CellItemRankReward>().InitData(false, false, 0, null, data.item);
        }
        rankFrameHL.gameObject.SetActive(false);
        rankFrameDisable.gameObject.SetActive(false);
        rankFrame.gameObject.SetActive(true);
        ModeSelection.instance.onReceiveSuccess -= GetReward;
    }    
}
