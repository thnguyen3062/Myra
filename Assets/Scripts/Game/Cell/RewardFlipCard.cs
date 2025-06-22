using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardFlipCard : MonoBehaviour
{
    [SerializeField] private CardRewardUI m_CardReward;
    [SerializeField] private ITween m_Blur;

    public void SetData(HeroCard hc)
    {
        m_CardReward.SetData(hc, true);
    }
    public void Blur()
    {
        m_Blur.Play();
    }
}
