using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowChangeSprite : MonoBehaviour
{
    [SerializeField] private GameObject m_Imortal;
    [SerializeField] private GameObject m_Legend;
    [SerializeField] private GameObject m_Low;
    [SerializeField] private GameObject m_MortalHigh;
    [SerializeField] private GameObject m_MortalLow;

    public void SetShadow(bool isGod, long rarity)
    {
        m_Imortal.SetActive(false);
        m_Legend.SetActive(false);
        m_Low.SetActive(false);
        m_MortalHigh.SetActive(false);
        m_MortalLow.SetActive(false);

        if (isGod)
        {
            if (rarity == 5)
                m_Imortal.SetActive(true);
            else if (rarity == 4)
                m_Legend.SetActive(true);
            else
                m_Low.SetActive(true);
        }
        else
        {
            //if (rarity <= 3)
                m_MortalLow.SetActive(true);
            //else
                //m_MortalHigh.SetActive(true);
        }
    }
}
