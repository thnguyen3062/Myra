using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening.Core;
using DG.Tweening;
using GIKCore.Lang;

public class RankInfo : MonoBehaviour
{
    [SerializeField] private Image rankImg;
    [SerializeField] private TextMeshProUGUI txtRank ;
    [SerializeField] private Image fillImg;
    [SerializeField] private Image progressImg;
    [SerializeField] private GameObject m_ProcessBar;
    [SerializeField] private Image m_ProcessBarImg;

    public void InitData(long id,long userElo =-1,bool doTween = false,int type =1,long afterAddElo = -1)
    {
        DBRank rank = Database.GetRank(id);
        DBRank nextRank = Database.GetRank(id-1);
        if (rank != null)
        {
            rankImg.sprite = CardData.Instance.GetRankSprite(id.ToString());
            txtRank.text = rank.name;
            if (id <= 0)
                id = 1;
            if (id > 18)
                id = 18;
            if (fillImg != null)
            {
                fillImg.sprite = CardData.Instance.GetRankSprite("f_" + id);
            }
        }
        else
        {
            rankImg.sprite = CardData.Instance.GetRankSprite("18");
            txtRank.text = LangHandler.Get("115", "Pending new season");
            if (fillImg != null)
            {
                fillImg.sprite = CardData.Instance.GetRankSprite("f_" + "18");
            }
        }
        if (userElo != -1 && progressImg != null && rank!=null)
        {
            if (id != 18 && id != 1)
            {
                float amt = (float)(userElo - rank.elo) / (float)(nextRank.elo - rank.elo);
                Debug.Log(amt);
                progressImg.gameObject.SetActive(true);
                progressImg.type = Image.Type.Filled;

                if (m_ProcessBar != null && m_ProcessBarImg != null)
                {
                    m_ProcessBar.SetActive(true);
                }
                if (!doTween)
                    progressImg.fillAmount = amt;
                else
                {
                    switch (type)
                    {
                        case 1:
                            {
                                // t?ng -> max
                                progressImg.fillAmount = amt;
                                DOTween.To(() => progressImg.fillAmount, x => progressImg.fillAmount = x, 1, 1f);
                                if (m_ProcessBar != null && m_ProcessBarImg != null)
                                {
                                    m_ProcessBarImg.fillAmount = amt;
                                    DOTween.To(() => m_ProcessBarImg.fillAmount, x => m_ProcessBarImg.fillAmount = x, 1, 1f);
                                }
                                break;
                            }
                        case 2:
                            {
                                // t?ng 0 -> current
                                progressImg.fillAmount = 0;
                                DOTween.To(() => progressImg.fillAmount, x => progressImg.fillAmount = x, amt, 1f);
                                if (m_ProcessBar != null && m_ProcessBarImg != null)
                                {
                                    m_ProcessBarImg.fillAmount = 0;
                                    DOTween.To(() => m_ProcessBarImg.fillAmount, x => m_ProcessBarImg.fillAmount = x, amt, 1f);
                                }
                                break;
                            }
                        case 3:
                            {
                                //giam -> 0
                                progressImg.fillAmount = amt;
                                DOTween.To(() => progressImg.fillAmount, x => progressImg.fillAmount = x, 0, 1f);
                                if (m_ProcessBar != null && m_ProcessBarImg != null)
                                {
                                    m_ProcessBarImg.fillAmount = amt;
                                    DOTween.To(() => m_ProcessBarImg.fillAmount, x => m_ProcessBarImg.fillAmount = x, 0, 1f);
                                }
                                break;
                            }
                        case 4:
                            {
                                // giam - cur
                                progressImg.fillAmount = 1;
                                DOTween.To(() => progressImg.fillAmount, x => progressImg.fillAmount = x, amt, 1f);
                                if (m_ProcessBar != null && m_ProcessBarImg != null)
                                {
                                    progressImg.fillAmount = 1;
                                    DOTween.To(() => m_ProcessBarImg.fillAmount, x => m_ProcessBarImg.fillAmount = x, amt, 1f);
                                }
                                break;
                            }
                        case 5:
                            {
                                //tu a - b
                                float amtAfter = (float)(afterAddElo - rank.elo) / (float)(nextRank.elo - rank.elo);
                                progressImg.fillAmount = amt;
                                DOTween.To(() => progressImg.fillAmount, x => progressImg.fillAmount = x, amtAfter, 1f);

                                if (m_ProcessBar != null && m_ProcessBarImg != null)
                                {
                                    m_ProcessBarImg.fillAmount = amt;
                                    DOTween.To(() => m_ProcessBarImg.fillAmount, x => m_ProcessBarImg.fillAmount = x, amtAfter, 1f);
                                }
                                break;
                            }
                    }

                }
            }
            else
            {
                progressImg.gameObject.SetActive(false);
                if (m_ProcessBar != null)
                {
                    m_ProcessBar.SetActive(false);
                }
            }
        }
        else
            {
                if (progressImg != null)
                    progressImg.gameObject.SetActive(false);
                if (m_ProcessBar != null )
                {
                    m_ProcessBar.SetActive(false);
                }
            }
           
    }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          
}
