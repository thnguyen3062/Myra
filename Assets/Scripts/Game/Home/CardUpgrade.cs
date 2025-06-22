using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUpgrade : MonoBehaviour
{
    // Fields
    [SerializeField] private List<GameObject> m_ListFrame;
    [SerializeField] private List<GameObject> m_ListTriangleColor;
    [SerializeField] private GameObject m_Mask;
    [SerializeField] private GameObject m_Glow;
    [SerializeField] private Image m_Image;

    // Values

    // Methods
    public CardUpgrade SetFrame(int frame)
    {
        for (int i = 0; i < m_ListFrame.Count; i++)
            m_ListFrame[i].SetActive(i == frame - 1);
        return this;
    }
    public CardUpgrade SetMask(bool on)
    {
        m_Mask.SetActive(on);
        return this;
    }
    public CardUpgrade SetGlow(bool on)
    {
        m_Glow.SetActive(on);
        return this;
    }
    public CardUpgrade SetCardColor(int color)
    {
        for(int i = 0; i < m_ListTriangleColor.Count; i++)
        {
            m_ListTriangleColor[i].SetActive(i == color);
        }
        return this;
    }
    public CardUpgrade SetImage(long heroId)
    {
        DBHero hero = Database.GetHero(heroId);
        if (hero != null)
        {
            m_Image.sprite = CardData.Instance.GetOnBoardSprite(heroId);
            SetCardColor((int)hero.color);
        }
        return this;
    }
}
