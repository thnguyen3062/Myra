using EZCameraShake;
using GIKCore.Utilities;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGameRank : MonoBehaviour
{
    [SerializeField] private RankInfo rankBefore;
    [SerializeField] private RankInfo rankAfter;
    [SerializeField] private Transform effectUprank;
    [SerializeField] private Transform effectFlareUprank;
    [SerializeField] private TextMeshProUGUI pointEloTxt;
    //[SerializeField] private Image progressImg;
    [SerializeField] private Button continueButton;

    private ICallback.CallFunc onComplete = null;
    public EndGameRank SetOnComplete(ICallback.CallFunc func) { onComplete = func; return this; }
    public void InitData(long eloAdd, long curElo, long curRank)
    {
        Debug.Log("elo add:" + eloAdd + "/cur:" + curElo + "/curRank:" + curRank);
        continueButton.interactable = false;
        // rankBefore.InitData()
        effectFlareUprank.gameObject.SetActive(false);
        rankAfter.gameObject.SetActive(false);
        long eloBefore = curElo - eloAdd;
        DBRank rankBeforeAdd = Database.GetRankByElo(eloBefore);
        if (rankBeforeAdd != null)
        {
            if (curElo > eloBefore)
            {
                // t?ng
                if (curRank != rankBeforeAdd.id)
                {
                    // hien thi progretion t?ng 2 rank
                    StartCoroutine(DisplayIncreaseTwoRankDifferent(rankBeforeAdd.id, eloBefore, curRank, curElo));

                }
                else
                {
                    StartCoroutine(DisplayChangeOnOneRank(curRank, eloBefore, curElo));
                }
            }
            else
            {
                if (curRank != rankBeforeAdd.id)
                {
                    StartCoroutine(DisplayDecreaseTwoRankDifferent(rankBeforeAdd.id, eloBefore, curRank, curElo));
                }
                else
                {
                    StartCoroutine(DisplayChangeOnOneRank(curRank, eloBefore, curElo));
                }

            }
            //rankBefore.InitData(rankBeforeAdd.id, curElo - eloAdd);
        }
        if(eloAdd!=0)
        {
            pointEloTxt.gameObject.SetActive(true);
            if(eloAdd>0)
            {
                string txt = "";
                txt = string.Format("YOU EARN {0} RANK POINTS", eloAdd);
                pointEloTxt.text = txt;
            }  
            else
            {
                string txt = "";
                txt = string.Format("YOU LOSE {0} RANK POINTS", Mathf.Abs(eloAdd));
                pointEloTxt.text = txt;
            }  
            
        } 
        else
            pointEloTxt.gameObject.SetActive(false);

    }

    IEnumerator DisplayChangeOnOneRank(long rankID, long eloBefore, long eloAfter)
    {
        rankBefore.InitData(rankID, eloBefore, true, 5, eloAfter);
        yield return new WaitForSeconds(1f);
        continueButton.interactable = true;
    }
    IEnumerator DisplayIncreaseTwoRankDifferent(long rankBeforeID,long eloBefore, long rankAfterID, long eloAfter)
    {
        rankBefore.InitData(rankBeforeID, eloBefore, true, 1);
        yield return new WaitForSeconds(0.5f);
        Transform target = IUtil.LoadPrefabRecycle("Prefabs/EndGameRank/","U_"+rankBeforeID, effectUprank);

        if (target != null)
        {
            target.GetComponent<RectTransform>().position = Vector3.zero;
            target.localPosition = Vector3.zero;
            target.localScale = Vector3.one;
            yield return new WaitForSeconds(0.5f);
            effectFlareUprank.gameObject.SetActive(true);
            yield return new WaitForSeconds(3.5f);
        }
        
        rankAfter.InitData(rankAfterID, eloAfter, true, 2);
        rankAfter.gameObject.SetActive(true);
        rankBefore.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        continueButton.interactable = true;
    }

    IEnumerator DisplayDecreaseTwoRankDifferent(long rankBeforeID, long eloBefore, long rankAfterID, long eloAfter)
    {
        rankBefore.InitData(rankBeforeID, eloBefore, true, 3);
        yield return new WaitForSeconds(1f);
        Transform target = IUtil.LoadPrefabRecycle("Prefabs/EndGameRank/", "D_" + rankBeforeID, effectUprank);

        if (target != null)
        {
            target.GetComponent<RectTransform>().position = Vector3.zero;
            target.localPosition = Vector3.zero;
            target.localScale = Vector3.one;
            yield return new WaitForSeconds(4);
        }

        rankAfter.InitData(rankAfterID, eloAfter, true, 4);
        rankAfter.gameObject.SetActive(true);
        rankBefore.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        continueButton.interactable = true;
    }
    public void TapToContinue()
    {
        onComplete?.Invoke();
        this.gameObject.SetActive(false);
        
    }
    public void DoDelayAllVFX()
    {
        rankAfter.GetComponent<Animator>().enabled = false;
        continueButton.interactable = true;

    }    
}
