using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GIKCore;
using GIKCore.UI;
using GIKCore.Lang;

public class CustomDeckPrefabControl : MonoBehaviour
{
   // [SerializeField] private Image previewImg;
    [SerializeField] private TextMeshProUGUI deckName;
    [SerializeField] private TextMeshProUGUI deckStatus;
    [SerializeField] private GameObject statusIcon,noGod;
    [SerializeField] private Image godDeck;
    [SerializeField] private Sprite noGodChosen;
    [SerializeField] private GameObject popupEditPrefab;
    GameObject popupEdit;
    private long deckID;
    private string m_name;
    private HeroCard card;
    void Start()
    {
        
    }

    // Update is called once per frame
    public void InitData(long deckID, string name, bool status,HeroCard hc)
    {
        this.deckID = deckID;
        this.m_name = name;
        this.card = hc;
        
        if(hc!= null)
        {
            DBHero db = hc.GetDatabase();
            if (godDeck != null)
            {
                if (db != null)
                    godDeck.sprite = CardData.Instance.GetGodDeckSprite(db.heroNumber);
            }
            if (noGod != null)
            {
                noGod.SetActive(false);
            }
        }
        else
        {
            if(godDeck != null)
            {
                if(noGodChosen!=null)
                godDeck.sprite = noGodChosen;
            }
            if(noGod!=null)
            {
                noGod.SetActive(true);
            }
        }
       
        deckName.text = name;
        if (status)
            statusIcon.SetActive(false);
        else
            statusIcon.SetActive(true);
        if(deckStatus!= null)
        {
            if (status)
                deckStatus.text = LangHandler.Get("101","Valid deck. You can use this to battle!");
            else
                deckStatus.text = LangHandler.Get("80","Invalid deck. Please revise your deck!");
        }    
        
       // Sprite sprite = CardData.Instance.GetOnBoardSprite(CurrentCardHeroID);
    }
    public void DoClickEdit()
    {
        
        if (!FindObjectOfType<EditFrameControl>())
        {
            popupEdit = Instantiate(popupEditPrefab, Game.main.canvas.panelPopup);
        }
        popupEdit.GetComponent<EditFrameControl>().InitData(this.m_name, this.card, this.deckID);
        popupEdit.SetActive(true);
    }
    public void ClickStatusIcon()
    {
        Toast.Show(LangHandler.Get("80", "Invalid deck. Please revise your deck!"));
    }
    public string GetDeckName()
    {
        return m_name;
    }
    public HeroCard GetPreviewDeckCard()
    {
        return card;
    }
    public long GetDeckID()
    {
        return deckID;
    }
    
}
