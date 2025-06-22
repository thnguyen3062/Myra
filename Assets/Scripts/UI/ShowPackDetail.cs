using GIKCore;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UIEngine.UIPool;
using UnityEngine;
using UnityEngine.UI;

public class ShowPackDetail : MonoBehaviour
{
    [SerializeField] private Image packImg; 
    [SerializeField] private Transform cardOnPackPrefab;
    [SerializeField] private Transform cardContent;
    [SerializeField] private GameObject cardPreview;
    [SerializeField] private CardPreviewInfo godCardPreview;
    [SerializeField] private CardPreviewInfo minionCardPreview;
    [SerializeField] private CardPreviewInfo spellCardPreview;

    //[SerializeField] private GameObject lightSelected;

    PackDetail packInfo;
    
    public void Init(PackDetail packDetail)
    {
        packInfo = packDetail;
        foreach (long id in packDetail.heroIds)
        {
            Transform trans = PoolManager.Pools["CardPack"].Spawn(cardOnPackPrefab);
            trans.localPosition = Vector3.zero;
            CardOnPack script = trans.GetComponent<CardOnPack>();
            script.InitData(id,1);
            trans.SetParent(cardContent);
            trans.localScale = new Vector3(0.7f, 0.7f, 1);
            script.SetOnClickCallback((heroId,frame) =>
            {
                ShowPreviewHandCard(heroId,frame);
            });
        }
    }
    public void ShowPreviewHandCard(long id,long frame)
    {
        if (cardPreview.activeSelf)
            return;

        cardPreview.SetActive(true);
        DBHero hero = Database.GetHero(id);
        if (hero.type == DBHero.TYPE_GOD)
        {
            godCardPreview.gameObject.SetActive(true);
            godCardPreview.SetCardPreview(hero, frame, true);
        }
        if (hero.type == DBHero.TYPE_TROOPER_MAGIC)
        {
            spellCardPreview.gameObject.SetActive(true);
            spellCardPreview.SetCardPreview(hero, frame, true);
        }
        if (hero.type == DBHero.TYPE_TROOPER_NORMAL)
        {
            minionCardPreview.gameObject.SetActive(true);
            minionCardPreview.SetCardPreview(hero, frame, true);
        }
    }
    public void ClosePreviewHandCard()
    {
        if (cardPreview.gameObject.activeSelf)
        {
            godCardPreview.gameObject.SetActive(false);
            minionCardPreview.gameObject.SetActive(false);
            cardPreview.gameObject.SetActive(false);
            spellCardPreview.gameObject.SetActive(false);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

    private void OnDisable()
    {
        PoolManager.Pools["CardPack"].DespawnAll();
    }
}
