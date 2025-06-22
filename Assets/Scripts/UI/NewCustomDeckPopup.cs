using DG.Tweening;
using GIKCore;
using GIKCore.Lang;
using GIKCore.Net;
using GIKCore.UI;
using GIKCore.Utilities;
using pbdson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UIEngine.UIPool;
using UnityEngine;
using static UniWebViewLogger;

public class NewCustomDeckPopup : GameListener
{
    // Fields
    [SerializeField] private TextMeshProUGUI m_DeckName;
    [SerializeField] private VerticalPoolGroup m_PoolLstCardCustomdeck;
    [SerializeField] private GameObject m_CardPreview;
    [SerializeField] private CardPreviewInfo m_PreviewGod, m_PreviewMinion, m_PreviewSpell;
    [SerializeField] private BoxPage m_BoxPage;
    [SerializeField] private RecycleLayoutGroup m_RecycleLayoutGroup;
    [SerializeField] private GameObject m_CurrentCardCrop = null;


    // Values
    private int currentDeckId;
    private int page = 0;
    private int totalPage = 0;
    private string currentDeckName;
    private const int perPage = 8;

    private List<HeroCard> lstCardUser = new List<HeroCard>();
    private List<CellHeroCardUser> lstCardUserGroup = new List<CellHeroCardUser>();

    private List<HeroCard> lstCardCustomDeck = new List<HeroCard>();
    private List<CellDeckCard> lstCardCustomDeckGroup = new List<CellDeckCard>();


    // Methods
    public NewCustomDeckPopup DoSaveCustomDeck()
    {
        return this;
    }
    public NewCustomDeckPopup DoFilter()
    {
        return this;
    }
    public NewCustomDeckPopup DoCheckCondition()
    {
        return this;
    }
    public NewCustomDeckPopup DoResetData()
    {
        lstCardUser.Clear();
        lstCardCustomDeck.Clear();
        return this;
    }
    public NewCustomDeckPopup DoRemoveCard(HeroCard hc)
    {
        return this;
    }
    private void DoAddCardDeck(HeroCard hc, GameObject go)
    {
        CellDeckCard targetCell = new CellDeckCard();
        if (hc == null)
            return;
        DBHero db = hc.GetDatabase();
        if (db != null)
        {
            lstCardUser.Remove(hc);
            lstCardCustomDeck.Add(hc);

            bool add = false;
            for (int i = lstCardCustomDeckGroup.Count - 1; i >= 0; i--)
            {
                CellDeckCard cdc = lstCardCustomDeckGroup[i];
                if (hc.heroId == cdc.heroId)
                {
                    cdc.lst.Add(hc);
                    add = true;
                    targetCell = cdc;
                }
            }


            if (add == false)
            {
                CellDeckCard cell = new CellDeckCard();
                cell.lst.Add(hc);
                cell.heroId = (int)hc.heroId;
                cell.cardType = (int)hc.GetDatabase().type;
                lstCardCustomDeckGroup.Add(cell);
                targetCell = cell;
            }
            for (int i = lstCardUserGroup.Count - 1; i >= 0; i--)
            {
                CellHeroCardUser chcu = lstCardUserGroup[i];
                if (chcu.lst.Count > 0)
                {
                    if (hc.heroId == chcu.heroId && hc.cardType == chcu.lst[0].cardType)
                    {
                        chcu.lstHeroCardIDNow.Add(hc);
                    }
                }

            }
        }
        if (targetCell != null)
        {
            int index = m_PoolLstCardCustomdeck.GetDataIndex(targetCell);
            GameObject card = m_PoolLstCardCustomdeck.GetGameObject(index);
            if (card != null)
            {
                m_CurrentCardCrop = card;
                if (card.activeSelf == true)
                {
                    go.GetComponent<RectTransform>().DOMove(m_CurrentCardCrop.GetComponent<RectTransform>().transform.position, 0.5f, false).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        m_CurrentCardCrop = null;
                        m_PoolLstCardCustomdeck.SetAdapter(lstCardCustomDeckGroup, false);

                    });
                }
                else
                {
                    go.GetComponent<RectTransform>().DOMove(m_CurrentCardCrop.GetComponent<RectTransform>().transform.position, 0.5f, false).OnComplete(() =>
                    {
                        m_CurrentCardCrop = null;
                        m_PoolLstCardCustomdeck.SetAdapter(lstCardCustomDeckGroup, false);
                    });
                }

            }
            else
            {
                m_PoolLstCardCustomdeck.SetAdapter(lstCardCustomDeckGroup, false);
                int index2 = m_PoolLstCardCustomdeck.GetDataIndex(targetCell);
                GameObject card2 = m_PoolLstCardCustomdeck.GetGameObject(index2);
                if (card2 != null)
                {
                    m_CurrentCardCrop = card2;
                    if (card2.activeSelf)
                    {
                        card2.SetActive(false);
                        go.GetComponent<RectTransform>().DOMove(m_CurrentCardCrop.GetComponent<RectTransform>().transform.position, 0.5f, false).OnComplete(() =>

                        {
                            card2.SetActive(true);

                        });
                    }
                    m_CurrentCardCrop = null;
                }
                else
                {
                    go.GetComponent<RectTransform>().DOMoveX(m_PoolLstCardCustomdeck.gameObject.transform.position.x, 0.5f, false);
                }
            }
        }
    }
    public NewCustomDeckPopup DoSortListCardCustomDeck()
    {
        lstCardCustomDeckGroup.Sort(delegate (CellDeckCard x, CellDeckCard y)
        {
            int compareType = x.cardType.CompareTo(y.cardType);
            //if(compareType == 0)
            //{
            //    return x.heroId.CompareTo(y.heroId);
            //}
            return compareType;
        });
        return this;
    }
    public NewCustomDeckPopup DoSortListUserCard()
    {
        lstCardUserGroup.Sort(delegate (CellHeroCardUser x, CellHeroCardUser y)
        {
            int compareType = x.cardType.CompareTo(y.cardType);
            //if(compareType == 0)
            //{
            //    return x.heroId.CompareTo(y.heroId);
            //}
            return compareType;
        });
        return this;
    }

    public NewCustomDeckPopup InitData()
    {
        // list card user deck
        List<HeroCard> lstGod = new List<HeroCard>();
        List<HeroCard> lstMinion = new List<HeroCard>();
        List<HeroCard> lstMagic = new List<HeroCard>();
        foreach (HeroCard hc in GameData.main.lstHeroCard)
        {
            if (hc.heroId != 117)
            {
                DBHero db = hc.GetDatabase();
                if (db != null)
                {
                    if (db.type == DBHero.TYPE_GOD) lstGod.Add(hc);
                    else if (db.type == DBHero.TYPE_TROOPER_MAGIC) lstMagic.Add(hc);
                    else if (db.type == DBHero.TYPE_TROOPER_NORMAL) lstMinion.Add(hc);

                }
            }
        }

        lstCardUser.Clear();
        lstCardUser.AddRange(lstGod);
        lstCardUser.AddRange(lstMagic);
        lstCardUser.AddRange(lstMinion);
        Dictionary<int, List<HeroCard>> lstGroupUserHeroCard = new Dictionary<int, List<HeroCard>>();
        for (int i = lstCardUser.Count - 1; i >= 0; i--)
        {
            int heroId = (int)lstCardUser[i].heroId;
            if (lstGroupUserHeroCard.ContainsKey(heroId))
            {
                lstGroupUserHeroCard[heroId].Add(lstCardUser[i]);
            }
            else
            {
                List<HeroCard> lst = new List<HeroCard> { lstCardUser[i] };
                lstGroupUserHeroCard.Add(heroId, lst);
            }
        }
        foreach (KeyValuePair<int, List<HeroCard>> guhc in lstGroupUserHeroCard)
        {
            CellHeroCardUser chcu = new CellHeroCardUser();
            chcu.heroId = guhc.Key;
            chcu.cardType = (int)Database.GetHero((long)guhc.Key).type;
            chcu.lst.AddRange(guhc.Value);
            lstCardUserGroup.Add(chcu);
        }
        totalPage = lstCardUserGroup.Count / 8;


        // lst card custom deck
        Dictionary<int, List<HeroCard>> lstGroupHeroCard = new Dictionary<int, List<HeroCard>>();
        for (int i = lstCardCustomDeck.Count - 1; i >= 0; i--)
        {
            int heroId = (int)lstCardCustomDeck[i].heroId;
            if (lstGroupHeroCard.ContainsKey(heroId))
            {
                lstGroupHeroCard[heroId].Add(lstCardCustomDeck[i]);
            }
            else
            {
                List<HeroCard> lst = new List<HeroCard> { lstCardCustomDeck[i] };
                lstGroupHeroCard.Add(heroId, lst);
            }
        }
        foreach (KeyValuePair<int, List<HeroCard>> ghc in lstGroupHeroCard)
        {
            CellDeckCard cdc = new CellDeckCard();
            cdc.heroId = ghc.Key;
            cdc.cardType = (int)Database.GetHero((long)ghc.Key).type;
            cdc.lst.AddRange(ghc.Value);
            lstCardCustomDeckGroup.Add(cdc);
        }

        DoSortListCardCustomDeck().DoSortListUserCard().SetPageData(page);
        m_PoolLstCardCustomdeck.SetAdapter(lstCardCustomDeckGroup);
        return this;
    }
    public NewCustomDeckPopup ShowCreateNew(bool isEdit)
    {
        DoResetData().InitData();
        gameObject.SetActive(true);
        return this;
    }
    public NewCustomDeckPopup ShowEdit(int deckId)
    {
        DoResetData();
        gameObject.SetActive(true);
        Game.main.socket.GetUserDeckDetail(deckId);
        return this;
    }
    public void ShowPreviewHandCard(long id, long frame)
    {
        if (m_CardPreview.activeSelf)
            return;

        m_CardPreview.SetActive(true);
        DBHero hero = Database.GetHero(id);
        if (hero.type == DBHero.TYPE_GOD)
        {
            m_PreviewGod.gameObject.SetActive(true);
            m_PreviewGod.SetCardPreview(hero, frame, true);
        }
        if (hero.type == DBHero.TYPE_TROOPER_MAGIC)
        {
            m_PreviewSpell.gameObject.SetActive(true);
            m_PreviewSpell.SetCardPreview(hero, frame, true);
        }
        if (hero.type == DBHero.TYPE_TROOPER_NORMAL)
        {
            m_PreviewMinion.gameObject.SetActive(true);
            m_PreviewMinion.SetCardPreview(hero, frame, true);
        }
    }

    public NewCustomDeckPopup SetPageData(int pageId)
    {
        page = pageId;
        m_BoxPage.Set(page, page < totalPage);
        List<CellHeroCardUser> lstAdapter = new List<CellHeroCardUser>();
        lstAdapter = lstCardUserGroup.GetRange(page * perPage, perPage);
        m_RecycleLayoutGroup.SetAdapter(lstAdapter);
        return this;
    }
    protected override void Awake()
    {
        base.Awake();
        m_PoolLstCardCustomdeck.SetCellDataCallback((GameObject go, CellDeckCard data, int index) =>
            {
                UserCardUI script = go.GetComponent<UserCardUI>();

                script.SetOnClickCallback(lstHc =>
                {
                    ShowPreviewHandCard(lstHc.lst[0].heroId, lstHc.lst[0].frame);
                });
                script.SetOnEndDragCallback(lstHc =>
                {
                    DoRemoveCard(lstHc.lst[lstHc.lst.Count - 1]);
                });
                script.InitData(data, m_PoolLstCardCustomdeck.scrollRect);
            })
            .SetCellPrefabCallback((index) =>
            {
                CellDeckCard data = m_PoolLstCardCustomdeck.GetCellDataAt<CellDeckCard>(index);
                if (data.lst[0].GetDatabase().type == DBHero.TYPE_GOD)
                {
                    return m_PoolLstCardCustomdeck.GetDeclarePrefab(1);
                }
                return m_PoolLstCardCustomdeck.GetDeclarePrefab(0);
            });

        m_PoolLstCardCustomdeck.SetCellSizeCallback((index) =>
        {
            CellDeckCard data = m_PoolLstCardCustomdeck.GetCellDataAt<CellDeckCard>(index);
            if (data.lst[0].GetDatabase().type == DBHero.TYPE_GOD)
            {
                return new Vector2(530, 113);
            }
            return new Vector2(530, 90);
        });

        m_RecycleLayoutGroup.SetCellDataCallback((GameObject go, CellHeroCardUser data, int idx) =>
        {
            CellHeroCard script = go.GetComponent<CellHeroCard>();
            script.SetData(data, idx);
            script.SetOnClickCallback(hc =>
            {
                ShowPreviewHandCard(hc.lst[0].heroId, hc.lst[0].frame);
            });

            script.SetOnEndDragCallback((lstHc, cardGo, move) =>
            {
                if (move)
                    DoAddCardDeck(lstHc.lst[lstHc.lstHeroCardIDNow.Count()], cardGo);
                //DoAddCardDeck(lstHc.lst[0], cardGo);
            });
        });
        m_BoxPage
            .SetOnNext(() => SetPageData(page + 1))
            .SetOnPrev(() => SetPageData(page - 1));
    }
    public override bool ProcessSocketData(int id, byte[] data)
    {
        if (base.ProcessSocketData(id, data))
            return true;

        switch (id)
        {
            case IService.GET_USER_DECK_DETAIL:
                {
                    ListCommonVector listCommonVector = ISocket.Parse<ListCommonVector>(data);
                    CommonVector cv0 = listCommonVector.aVector[0];
                    if (cv0.aLong[0] == 0)
                    {
                        Toast.Show(LangHandler.Get("59", "Can't get deck detail!"));
                    }
                    else
                    {
                        currentDeckId = (int)cv0.aLong[1];
                    }
                    for (int i = 4; i < GameData.main.lstDeckInfo.Count; i++)
                    {
                        if (GameData.main.lstDeckInfo[i].deckID == cv0.aLong[1])
                        {
                            currentDeckName = GameData.main.lstDeckInfo[i].deckName;
                            m_DeckName.text = GameData.main.lstDeckInfo[i].deckName;
                            break;
                        }
                    }
                    lstCardCustomDeck.Clear();

                    CommonVector cv1 = listCommonVector.aVector[1];
                    for (int i = 0; i < cv1.aLong.Count; i += 1)
                    {
                        // hc.id = cv1.aLong[i];
                        HeroCard hc = GameData.main.lstHeroCard.FirstOrDefault(x => x.id == cv1.aLong[i]);
                        lstCardCustomDeck.Add(hc);
                    }

                    CommonVector cv2 = listCommonVector.aVector[2];
                    for (int i = 0; i < cv2.aLong.Count; i += 1)
                    {
                        HeroCard hc = GameData.main.lstHeroCard.FirstOrDefault(x => x.id == cv2.aLong[i]);
                        lstCardCustomDeck.Add(hc);
                    }
                    InitData();
                    break;
                }
        }
        return false;
    }
}