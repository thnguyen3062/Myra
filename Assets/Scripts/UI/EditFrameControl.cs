using GIKCore;
using GIKCore.Lang;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EditFrameControl : MonoBehaviour
{
    [SerializeField] private Image previewImg;
    [SerializeField] private GameObject previewCard;
    [SerializeField] private TextMeshProUGUI deckName, noGod;
    [SerializeField] GameObject customDeckScene;
    [SerializeField] private Sprite noGodChosen;

    private long deckID;
    void Start()
    {

    }

    // Update is called once per frame
    public void InitData(string name, HeroCard hc,long deckId)
    {
        this.deckID = deckId;
        deckName.text = name;
        
        if (hc != null)
        {
            noGod.text = "";
            DBHero db = hc.GetDatabase();
            if (db != null)
            {
                previewCard.SetActive(true);
                previewCard.GetComponent<CardPreviewInfo>().SetCardPreview(db, hc.frame, false);
                //Sprite sprite = CardData.Instance.GetGodCardNewSprite(db.id);
                previewImg.gameObject.SetActive(false);
            }
        }
        else
        {
            previewCard.SetActive(false);
            previewImg.gameObject.SetActive(true);
            previewImg.sprite = noGodChosen;
            noGod.text = LangHandler.Get("140","NO GOD CHOSEN");
        }
        
    }
    public void DoClickDeleteDeck()
    {
        PopupConfirm.Show(
            content: LangHandler.Get("85","Confirm you want to delete this deck? This action cannot be reversed."),
            title: LangHandler.Get("but-23","CONFIRM"),
            action1: LangHandler.Get("but-20","CANCEL"),
            action2: "OK",
            action2Callback: go =>
            {
                Game.main.socket.DeleteUserDeck(this.deckID);
                go.SetActive(false);
                Game.main.socket.GetUserDeck();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            );

    }
    public void DoClickEditDeck()
    {
        customDeckScene.SetActive(true);
        Game.main.socket.GetUserDeckDetail(this.deckID);
    }
    public void DoClickPlayEven()
    {
        Game.main.socket.GetUserDeck();
        SceneManager.LoadScene("SelectDeckEventScene");
    }

}
