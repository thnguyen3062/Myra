using DG.Tweening;
using GIKCore;
using GIKCore.Lang;
using GIKCore.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectionDeckDetail : MonoBehaviour
{
    // Fields
    [SerializeField] private CellCollectionDeck m_CellDeckDetail;
    [SerializeField] private RectTransform m_DeckDetail;
    [SerializeField] private GameObject m_ButtonDelete, m_ButtonEdit;
    [SerializeField] private TextMeshProUGUI m_DeckType;
    [SerializeField] private GameObject m_CustomDeckScene;
    [SerializeField] private NewCustomDeckPopup m_NewCustomDeckPopup;
    [SerializeField] private TextMeshProUGUI m_CountGod, m_CountCard;

    // Values
    private int collectionDeckId;

    // Methods
    public void SetData(int deckId, int countGod, int countCard)
    {
        DeckInfo deck = GameData.main.lstDeckInfo.Find(x => x.deckID == deckId);
        if (deck != null)
        {
            collectionDeckId = deckId;
            m_CellDeckDetail.SetData(deck);
            gameObject.SetActive(true);
            m_DeckDetail.DOAnchorPosX(0f, 0.3f);

            m_DeckType.text = LangHandler.Get("201","NORMAL DECK");
            m_ButtonDelete.SetActive(!deck.isDefaultDeck);
            //m_ButtonEdit.SetActive(!deck.isDefaultDeck);
            int MAX_GOD = 6;
            int MAX_CARD = 21;
            if (countGod > MAX_GOD || countGod == 0)
            {
                m_CountGod.text = "<color=#FF0000>" + countGod + "</color> / " + MAX_GOD;
            }
            else
            {
                m_CountGod.text = "<color=#19B000>" + countGod + "</color> / " + MAX_GOD;
            }

            if (countCard > MAX_CARD || countCard < MAX_CARD)
            {
                m_CountCard.text = "<color=#FF0000>" + countCard + "</color> / " + MAX_CARD;
            }
            else
            {
                m_CountCard.text = "<color=#19B000>" + countCard + "</color> / " + MAX_CARD;
            }
        }
        else
        {
            collectionDeckId = 0;
        }
    }
    public void CloseDeckDetail()
    {
        m_DeckDetail.DOAnchorPosX(706f, 0.3f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
    public void DoClickEdit()
    {
        
        //m_NewCustomDeckPopup.ShowEdit(collectionDeckId);
        m_CustomDeckScene.SetActive(true);
        Game.main.socket.GetUserDeckDetail(collectionDeckId);
    }
    public void DoClickDeleteDeck()
    {
        PopupConfirm.Show(
            content: LangHandler.Get("85", "Confirm you want to delete this deck? This action cannot be reversed."),
            title: LangHandler.Get("but-23", "CONFIRM"),
            action1: LangHandler.Get("but-20", "CANCEL"),
            action2: "OK",
            action2Callback: go =>
            {
                Game.main.socket.DeleteUserDeck(collectionDeckId);
                go.SetActive(false);
                Game.main.socket.GetUserDeck();
            }
            );
    }
    public void DoClickPlay()
    {
        DeckInfo deck = GameData.main.lstDeckInfo.Find(x => x.deckID == collectionDeckId);
        if (deck != null)
        {
            GameData.main.selectedDeck = collectionDeckId;
            if (GameData.main.passFirst10Match)
            {
                Game.main.socket.GetMode();
                Game.main.LoadScene("SelectModeScene", curtain: true);
            }
            else
            {
                Game.main.socket.GetUserDeck();
                Game.main.socket.GetRank();
                Game.main.LoadScene("SelectDeckScene", delay: 0.3f, curtain: true);
            }    
        } else
        {
            Toast.Show("No Deck Found");
        }
    }
}
