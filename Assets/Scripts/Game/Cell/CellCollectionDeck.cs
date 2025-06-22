using GIKCore.Sound;
using GIKCore.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellCollectionDeck : MonoBehaviour
{
    // Fields
    [SerializeField] private TextMeshProUGUI m_DeckName, m_DeckNameNoGod;
    [SerializeField] private GoOnOff m_CellDeck;
    [SerializeField] private List<GameObject> m_MainGods = new List<GameObject>();
    [SerializeField] private List<GameObject> m_Crystals = new List<GameObject>();
    [SerializeField] private GameObject m_IconWarning;
    [SerializeField] private GameObject m_PointClick;
    [SerializeField] private GameObject m_FrameHighlight;

    // Values
    public int deckId;

    // Methods
    public delegate void Callback(CellCollectionDeck ccd);
    private Callback onClickCB = null;
    public CellCollectionDeck SetOnClickCallback(Callback func) { onClickCB = func; return this; }
    public void OnClick()
    {
        SoundHandler.main.PlaySFX("900_click_4", "sounds");
        if (m_PointClick != null)
            m_PointClick.SetActive(false);
        if (onClickCB != null)
            onClickCB(this);
    }
    public CellCollectionDeck SetData(DeckInfo deckInfo, bool isShowPointClick = false)
    {
        deckId = (int)deckInfo.deckID;
        int count = 0;
        for (int i = 0; i < deckInfo.lstGodIds.Length; i++)
        {
            if (deckInfo.lstGodIds[i] != -1)
                count++;
        }
        if (count != 0)
        {
            m_CellDeck.TurnOn();
            m_DeckName.text = deckInfo.deckName;
            DBHero heroInfo = Database.GetHero((long)deckInfo.godId);

            for (int i = 0; i < m_MainGods.Count; i++)
            {
                m_MainGods[i].SetActive(i == (count - 1));
            }
            for (int i = 0; i < m_MainGods[count - 1].transform.childCount; i++)
            {
                CellCollectionDeckGod script = m_MainGods[count - 1].transform.GetChild(i).GetComponent<CellCollectionDeckGod>();
                if (script != null)
                {
                    DBHero hero = Database.GetHero((long)deckInfo.lstGodIds[i]);
                    if (hero != null)
                        script.SetData(hero, count);
                }
            }

            for (int i = 0; i < m_Crystals.Count; i++)
            {
                m_Crystals[i].SetActive(i < count);
            }
            m_IconWarning.SetActive(deckInfo.deckStatus == 0);
            //m_FrameCrystal.sprite = CardData.Instance.GetDeckCrystalSprite(heroInfo.color);

        }
        else
        {
            m_CellDeck.TurnOff();
            m_DeckNameNoGod.text = deckInfo.deckName;
        }
        if (m_PointClick != null)
            m_PointClick.SetActive(isShowPointClick);

        if (m_FrameHighlight != null)
            m_FrameHighlight.SetActive(deckInfo.isSelected);
        return this;
    }
}
