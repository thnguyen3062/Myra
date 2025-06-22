using GIKCore.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeRewardStoneCard : MonoBehaviour
{
    // Fields
    [SerializeField] private CardUpgrade m_CardUpgrade;

    // Values
    private DBHero hero;

    // Methods
    public HomeRewardStoneCard SetData(long heroId)
    {
        hero = Database.GetHero(heroId);
        m_CardUpgrade.SetFrame(1).SetMask(false).SetGlow(false).SetImage(heroId);
        return this;
    }
    public void Onclick()
    {
        HandleNetData.QueueNetData(NetData.PREVIEW_CARD, hero);
    }
}
