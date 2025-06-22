using System.Collections;
using System.Collections.Generic;
using TMPro;
using UIEngine.Extensions;
using UnityEngine;
using GIKCore;
using UnityEngine.UI;
using System.Linq;
using GIKCore.UI;
using GIKCore.Utilities;
using DG.Tweening;
using UnityEngine.EventSystems;
using GIKCore.Lang;

public class UserCardUI : MonoBehaviour
{
    // Fields
    // Old Feature
    [SerializeField] private Image m_GodIcon;
    [SerializeField] private Image m_filter;
    [SerializeField] private UIDragDrop m_UIDragDrop;
    [SerializeField] private GameObject checkTrooperOkBut;
    [SerializeField] private GameObject cardCropUI;
    [SerializeField] private GameObject NoneGod, God, highLightError, tabError;

    // New Feature
    [SerializeField] private PackItemReward m_ItemRewardGod, m_ItemRewardNonGod;

    //[SerializeField] public Image[] groupImageFade;
    //[SerializeField] public TextMeshProUGUI[] groupTextFade;
    // [SerializeField] private Transform POpp;

    // Values
    public Vector3 initPosisiotn, curPos;
    float angle;
    Vector2 line;

    public ICallback.CallFunc onDropCard;
    public LayerMask layerMask;
    private CardSlot currentSelectedCardSlot;
    private ICallback.CallFunc2<CellDeckCard> onClickCB = null;
    private ICallback.CallFunc2<CellDeckCard> onEndDragCB = null;
    private CellDeckCard data = new CellDeckCard();
    public int count = 1;
    public List<long> cardID;
    bool isDrag = false;
    private GameObject go;
    private ScrollRect scroll;

    // Methods
    public void SetOnClickCallback(ICallback.CallFunc2<CellDeckCard> func) { onClickCB = func; }
    public void SetOnEndDragCallback(ICallback.CallFunc2<CellDeckCard> func) { onEndDragCB = func; }
    public long CurrentCardID
    {
        get;
        private set;
    }

    public long CurrentCardHeroID
    {
        get;
        private set;
    }
    public UIDragDrop Event
    {
        get
        {
            return m_UIDragDrop;
        }
    }

    public bool allowDrag
    {
        get
        {
            return m_UIDragDrop.allowDrag;
        }
        set
        {
            m_UIDragDrop.allowDrag = value;
        }
    }

    public void DoClick()
    {
        if (data != null && onClickCB != null && isDrag == false)
        {
            onClickCB(data);
        }

    }
    public void DoClickErrorBut()
    {
        if (!data.check4God)
        {
            GameObject tab = Instantiate(tabError);
            tab.SetActive(true);
            tab.transform.SetParent(this.transform.parent.parent.parent);
            tab.GetComponent<RectTransform>().anchoredPosition = new Vector2(-268, -61);
            tab.GetComponent<RectTransform>().sizeDelta = new Vector2(527, 100);
            tab.GetComponent<RectTransform>().localScale = Vector3.one;
            Destroy(tab, 2);
        }
        if (!data.checkBuffCard && !data.checkMaxCard)
        {
            Toast.Show(string.Format("This card requires {0} god card in the deck.", Database.GetHero(data.lst[0].GetDatabase().ownerGodID).name));
        }
        else if (!data.checkTrooper && !data.checkMaxCard)
        {
            Toast.Show(LangHandler.Get("71", "Need at least 1 ") + data.lst[0].GetDatabase().GetColorName() + LangHandler.Get("72", " God Card same color to use this card.") + "\n" + LangHandler.Get("70", "Maximum 3 Gods or 2 NonGods allowed per deck"));
        }
        else if (!data.checkBuffCard)
            Toast.Show(string.Format("This card requires {0} god card in the deck.", Database.GetHero(data.lst[0].GetDatabase().ownerGodID).name));
        else if (!data.checkTrooper)
            Toast.Show(LangHandler.Get("71", "Need at least 1 ") + data.lst[0].GetDatabase().GetColorName() + LangHandler.Get("72", " God Card same color to use this card."));
        else if (!data.checkMaxCard)
            Toast.Show(LangHandler.Get("70", "Maximum 3 God copies or 2 NonGod copies allowed per deck."));

    }
    //private void OnEnable()
    //{
    //    foreach (Image img in groupImageFade)
    //    {
    //        img.color = new Color(1, 1, 1, 1);
    //    }
    //    foreach (TextMeshProUGUI txt in groupTextFade)
    //    {
    //        txt.color = new Color(1, 1, 1, 1);
    //    }
    //}
    void Start()
    {
        Event
            .SetOnBeginDragCallback((go) => { OnBeginDragDeckCard(); })
            .SetOnDragCallback((go) => { OnDrag(); })
            .SetOnEndDragCallback((go) => { OnDropCard(go); });
    }
    public void InitData(CellDeckCard data, ScrollRect sr = null)
    {
        scroll = sr;
        this.data = data;
        if (data.lst.Count <= 0)
            return;
        if (data.lst[0].GetDatabase().disable > 0)
        {
            return;
        }

        HeroCard hc = data.lst[0];
        CurrentCardID = hc.id;
        CurrentCardHeroID = hc.heroId;
        DBHero db = hc.GetDatabase();
        // gameObject.name = godName;
        // count so hero cung loai co cung Id/ so ban copy cua mot hero /card id khac nhau nhung hero id giong nhau 

        if (!data.check4God && data.lst[0].GetDatabase().type == DBHero.TYPE_GOD && !checkTrooperOkBut.activeInHierarchy)
        {
            GameObject tab = Instantiate(tabError);
            tab.SetActive(true);
            tab.transform.SetParent(this.transform.parent.parent.parent);
            tab.GetComponent<RectTransform>().anchoredPosition = new Vector2(-268, -61);
            tab.GetComponent<RectTransform>().sizeDelta = new Vector2(527, 100);
            tab.GetComponent<RectTransform>().localScale = Vector3.one;
            Destroy(tab, 2);
        }
        if (!data.checkTrooper || !data.checkMaxCard || !data.check4God)
            checkTrooperOkBut.SetActive(true);
        else
            checkTrooperOkBut.SetActive(false);
        if (db != null)
        {
            m_filter.sprite = CardData.Instance.GetFilterCardSprite(hc.GetDatabase().color);
            m_GodIcon.sprite = CardData.Instance.GetOnBoardSprite(db.id);
            ItemReward itemReward = new ItemReward
            {
                count = data.lst.Count,   
                kind = 0,
                id = (int)db.id,
                frame = (int)hc.GetDatabase().color
            };

            if (db.type == DBHero.TYPE_GOD)
            {
                // old feature
                //God.SetActive(true);
                //NoneGod.SetActive(false);
                //God.GetComponent<CardCropInfor>().SetInfo(db, data.lst.Count);

                // new feature
                m_ItemRewardGod.gameObject.SetActive(true);
                m_ItemRewardNonGod.gameObject.SetActive(false);
                m_ItemRewardGod.SetData(itemReward);
            }
            else if (db.type == DBHero.TYPE_TROOPER_MAGIC || db.type == DBHero.TYPE_BUFF_MAGIC)
            {
                // old feature
                //God.SetActive(false);
                //NoneGod.SetActive(true);
                //NoneGod.GetComponent<CardCropInfor>().SetInfo(db, data.lst.Count);

                // new feature
                m_ItemRewardGod.gameObject.SetActive(false);
                m_ItemRewardNonGod.gameObject.SetActive(true);
                m_ItemRewardNonGod.SetData(itemReward);
            }
            else if (db.type == DBHero.TYPE_TROOPER_NORMAL)
            {
                // old feature
                //God.SetActive(false);
                //NoneGod.SetActive(true);
                //NoneGod.GetComponent<CardCropInfor>().SetInfo(db, data.lst.Count);

                // new feature
                m_ItemRewardGod.gameObject.SetActive(false);
                m_ItemRewardNonGod.gameObject.SetActive(true);
                m_ItemRewardNonGod.SetData(itemReward);
            }
        }
        initPosisiotn = transform.position;
    }

    private void OnBeginDragDeckCard()
    {
        isDrag = true;
        go = Instantiate(cardCropUI, transform.position, Quaternion.identity, Game.main.canvas.panelPopup);
        go.SetActive(false);
        UserCardUI card = go.GetComponent<UserCardUI>();
        CellDeckCard newDeckCard = new CellDeckCard();
        newDeckCard.lst.Add(data.lst[0]);
        CanvasGroup cvGroup = go.GetComponent<CanvasGroup>();
        cvGroup.blocksRaycasts = false;
        card.InitData(newDeckCard);
        Event.rectTransform = go.GetComponent<RectTransform>();
        Event.rectTransform.sizeDelta = new Vector2(550, 90);
        initPosisiotn = Event.rectTransform.position;
    }
    private void OnDrag()
    {
        curPos = Event.rectTransform.position;
        line = (Vector2)(curPos - initPosisiotn);
        angle = Vector3.Angle(line, transform.right);
        if (angle < 20 && line.magnitude > 1f)
        {
            go.SetActive(true);
            Event.SetScrollRect(null);
        }
        else if (angle > 20 && !go.activeInHierarchy)
        {
            go.SetActive(false);
            Event.SetScrollRect(scroll);
        }
    }
    private void OnDropCard(GameObject go)
    {
        //tao tween 
        CanvasGroup cvGroup = go.GetComponent<CanvasGroup>();
        cvGroup.blocksRaycasts = true;
        if (angle < 20 && line.magnitude > 1f)
        {
            if (onEndDragCB != null)
            {
                onEndDragCB(data);
            }
            go.GetComponent<RectTransform>().DOAnchorPosX(600f, 0.5f);
            Destroy(go, 0.5f);
        }
        else
        {
            Destroy(go);
        }
        Event.rectTransform = this.GetComponent<RectTransform>();
        isDrag = false;
    }
}

