using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankTierLeaderboard : MonoBehaviour
{
    // Fields
    [SerializeField] private Image m_RankIcon;
    [SerializeField] private Image m_RankBg;
    [SerializeField] private TextMeshProUGUI m_RankText;
    // Values
    // Methods
    public void SetData(int rankId)
    {
        m_RankIcon.sprite = CardData.Instance.GetRankSprite(rankId.ToString());
        m_RankText.gameObject.SetActive(true);
        Color rankBgColor;
        switch (rankId)
        {
            case 1:
                {
                    m_RankText.gameObject.SetActive(false);
                    ColorUtility.TryParseHtmlString("#1E1126", out rankBgColor);
                    break;
                }
            case 2:
                {
                    m_RankText.text = "V";
                    ColorUtility.TryParseHtmlString("#39240A", out rankBgColor);
                    break;
                }
            case 3:
                {
                    m_RankText.text = "IV";
                    ColorUtility.TryParseHtmlString("#39240A", out rankBgColor);
                    break;
                }
            case 4:
                {
                    m_RankText.text = "III";
                    ColorUtility.TryParseHtmlString("#39240A", out rankBgColor);
                    break;
                }
            case 5:
                {
                    m_RankText.text = "II";
                    ColorUtility.TryParseHtmlString("#39240A", out rankBgColor);
                    break;
                }
            case 6:
                {
                    m_RankText.text = "I";
                    ColorUtility.TryParseHtmlString("#39240A", out rankBgColor);
                    break;
                }
            case 7:
                {
                    m_RankText.text = "IV";
                    ColorUtility.TryParseHtmlString("#053120", out rankBgColor);
                    break;
                }
            case 8:
                {
                    m_RankText.text = "III";
                    ColorUtility.TryParseHtmlString("#053120", out rankBgColor);
                    break;
                }
            case 9:
                {
                    m_RankText.text = "II";
                    ColorUtility.TryParseHtmlString("#053120", out rankBgColor);
                    break;
                }
            case 10:
                {
                    m_RankText.text = "I";
                    ColorUtility.TryParseHtmlString("#053120", out rankBgColor);
                    break;
                }
            case 11:
                {
                    m_RankText.text = "III";
                    ColorUtility.TryParseHtmlString("#361D00", out rankBgColor);
                    break;
                }
            case 12:
                {
                    m_RankText.text = "II";
                    ColorUtility.TryParseHtmlString("#361D00", out rankBgColor);
                    break;
                }
            case 13:
                {
                    m_RankText.text = "I";
                    ColorUtility.TryParseHtmlString("#361D00", out rankBgColor);
                    break;
                }
            case 14:
                {
                    m_RankText.text = "III";
                    ColorUtility.TryParseHtmlString("#2A2C34", out rankBgColor);
                    break;
                }
            case 15:
                {
                    m_RankText.text = "II";
                    ColorUtility.TryParseHtmlString("#2A2C34", out rankBgColor);
                    break;
                }
            case 16:
                {
                    m_RankText.text = "I";
                    ColorUtility.TryParseHtmlString("#2A2C34", out rankBgColor);
                    break;
                }
            case 17:
                {
                    m_RankText.gameObject.SetActive(false);
                    ColorUtility.TryParseHtmlString("#322113", out rankBgColor);
                    break;
                }
            case 18:
                {
                    m_RankText.gameObject.SetActive(false);
                    ColorUtility.TryParseHtmlString("#7F7F7F", out rankBgColor);
                    break;
                }
            default:
                {
                    ColorUtility.TryParseHtmlString("#1E1126", out rankBgColor);
                    break;
                }
        }
        m_RankBg.color = rankBgColor;
    }
}
