using DG.Tweening;
using GIKCore.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardPremiumCardPopup : MonoBehaviour
{
    // Fields
    [SerializeField] private List<CardRewardUI> m_LstRewards;

    // Values

    // Methods
    public void SetData(List<HeroCard> lstCard)
    {
        for (int i = 0; i < m_LstRewards.Count; i++)
        {
            m_LstRewards[i].SetData(lstCard[i]);
        }
    }
    public void DoFlipCard()
    {
        StartCoroutine(IUtil.Delay(() =>
        {
            m_LstRewards[0].FlipCard();
        }, 0.3f));
        StartCoroutine(IUtil.Delay(() =>
        {
            m_LstRewards[1].FlipCard();
        }, 0.6f));
        StartCoroutine(IUtil.Delay(() =>
        {
            m_LstRewards[2].FlipCard();
        }, 0.9f));
    }

    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    //void Update()
    //{

    //}
}
