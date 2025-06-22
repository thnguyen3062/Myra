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
using Unity.Mathematics;

public class CellHeroCard : MonoBehaviour
{
    // Fields

    [SerializeField] private GameObject god, minion, spell;
    [SerializeField] private UIDragDrop m_UIDragDrop;
    [SerializeField] private GameObject cardCropUInormal;
    [SerializeField] private GameObject cardCropUI;
    [SerializeField] private GameObject highLightBlue;
    [SerializeField] private Transform lstCardCustom;
    // Values
    private ICallback.CallFunc onPointerExitCB = null;
    private ICallback.CallFunc onDragCB = null;
    private ICallback.CallFunc onBeginDragCB = null;
    private ICallback.CallFunc2<CellHeroCardUser> onClickCB = null;
    private ICallback.CallFunc4<CellHeroCardUser, GameObject, bool> onEndDragCB = null;
    private CellHeroCardUser cardData = new CellHeroCardUser();
    public int countHC;
    bool isDrag = false;
    public Vector3 initPosisiotn, curPos;
    private GameObject go;
    private long cardType;
    float angle;
    Vector2 line;
    private ScrollRect scroll;
    public long CurrentCardID
    {
        get;
        private set;
    }
    public static CellHeroCard main { get; private set; } = null;

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


    // Methods
    public void DoClick()
    {
        if (cardData != null && onClickCB != null && isDrag == false)
            onClickCB(cardData);
    }

    public void SetOnClickCallback(ICallback.CallFunc2<CellHeroCardUser> func)
    {
        onClickCB = func;
    }
    public void SetOnBeginDragCallBack(ICallback.CallFunc func) { onBeginDragCB = func; }
    public void SetOnDragCallBack(ICallback.CallFunc func) { onDragCB = func; }
    public void SetOnPointerExitCallBack(ICallback.CallFunc func) { onPointerExitCB = func; }
    public void SetOnEndDragCallback(ICallback.CallFunc4<CellHeroCardUser, GameObject, bool> func) { onEndDragCB = func; }
    public void SetData(CellHeroCardUser data, int index, ScrollRect sr = null)
    {
        highLightBlue.SetActive(false);
        scroll = sr;
        cardData = data;
        cardData.heroId = (int)data.lst[0].heroId;
        //DBHero db = data.lst.OrderByDescending(x => x.GetDatabase().rarity).FirstOrDefault().GetDatabase();

        DBHero db = data.lst[0].GetDatabase();
        if (db != null)
        {
            if (db.disable > 0)
            {
                return;
            }
            countHC = data.lst.Count - data.lstHeroCardIDNow.Count;
            int idx = countHC != 0 ? data.lst.Count - countHC : 0;
            //Debug.Log("HeroId: " + db.name + " Count: " + data.lst.Count + " Left: " + countHC);
            cardType = db.type;
            if (db.type == DBHero.TYPE_GOD)
            {
                god.SetActive(true);
                god.GetComponent<CardUserInfor>().SetInfoCard(db, countHC, (int)data.lst[idx].frame);
                minion.SetActive(false);
                spell.SetActive(false);
            }
            if (db.type == DBHero.TYPE_TROOPER_MAGIC || db.type == DBHero.TYPE_BUFF_MAGIC)
            {
                god.SetActive(false);
                minion.SetActive(false);
                spell.SetActive(true);
                spell.GetComponent<CardUserInfor>().SetInfoCard(db, countHC, (int)data.lst[idx].frame);
            }
            if (db.type == DBHero.TYPE_TROOPER_NORMAL)
            {
                god.SetActive(false);
                minion.SetActive(true);
                spell.SetActive(false);
                minion.GetComponent<CardUserInfor>().SetInfoCard(db, countHC, (int)data.lst[idx].frame);
            }
            if (countHC <= 0)
            {
                allowDrag = false;
            }
            else
            {
                allowDrag = true;
            }
        }
    }
    private void Awake()
    {
        CellHeroCard.main = this;
    }
    void Start()
    {
        Event.SetOnBeginDragCallback((go) => { OnBeginDragCard(); });
        Event.SetOnDragCallback((go) => { OnDrag(); });
        Event.SetOnEndDragCallback((go) => { OnDropCard(go); });
        Event.SetOnPointerDownCallback((go) => { OnPointerDown(); });
        Event.SetOnPointerExitCallback((go) => { OnPointerExit(); });
    }

    private void OnPointerDown()
    {
        switch (cardType)
        {
            case (int)DBHero.TYPE_GOD:
                {
                    god.GetComponent<CardUserInfor>().SetFrameBackSelect(true);
                    break;
                }
            case (int)DBHero.TYPE_TROOPER_NORMAL:
                {
                    minion.GetComponent<CardUserInfor>().SetFrameBackSelect(true);
                    break;
                }
            case (int)DBHero.TYPE_TROOPER_MAGIC:
                {
                    spell.GetComponent<CardUserInfor>().SetFrameBackSelect(true);
                    break;
                }
        }
    }
    private void OnPointerExit()
    {
        if (onPointerExitCB != null)
            onPointerExitCB();

        switch (cardType)
        {
            case (int)DBHero.TYPE_GOD:
                {
                    god.GetComponent<CardUserInfor>().SetFrameBackSelect(false);
                    break;
                }
            case (int)DBHero.TYPE_TROOPER_NORMAL:
                {
                    minion.GetComponent<CardUserInfor>().SetFrameBackSelect(false);
                    break;
                }
            case (int)DBHero.TYPE_TROOPER_MAGIC:
                {
                    spell.GetComponent<CardUserInfor>().SetFrameBackSelect(false);
                    break;
                }
        }
    }
    private void OnBeginDragCard()
    {
        if (cardData.lst.Count > 0)
        {
            if (onBeginDragCB != null)
            {
                onBeginDragCB();
            }
            isDrag = true;
            Event.SetScrollRect(null);
            go = Instantiate(cardCropUI, transform.position, Quaternion.identity, Game.main.canvas.panelPopup);
            go.SetActive(false);
            UserCardUI card = go.GetComponent<UserCardUI>();
            CellDeckCard newDeckCard = new CellDeckCard();
            if (ProgressionController.instance != null)
            {
                HeroCard hc = GameData.main.lstHeroCard.FirstOrDefault(x => x.heroId == 46);
                newDeckCard.lst.Add(hc);
            }
            else
            {
                if (cardData.lst.Count > 0)
                    newDeckCard.lst.Add(cardData.lst[0]);
            }
            CanvasGroup cvGroup = go.GetComponent<CanvasGroup>();
            cvGroup.blocksRaycasts = false;
            card.InitData(newDeckCard);
            Event.rectTransform = go.GetComponent<RectTransform>();
            Event.rectTransform.sizeDelta = new Vector2(550, 90);
            initPosisiotn = Event.rectTransform.position;
        }
    }
    private void OnDrag()
    {
        if (onDragCB != null)
        {
            onDragCB();
        }
        curPos = Event.rectTransform.position;
        line = (Vector2)(curPos - initPosisiotn);
        angle = Vector2.Angle(line, -transform.right);

        if(go != null)
            go.SetActive(true);
        if (ProgressionController.instance == null)
        {
            highLightBlue.SetActive(true);
            highLightBlue.transform.SetParent(lstCardCustom);
            highLightBlue.GetComponent<RectTransform>().localPosition = Vector2.zero;
            highLightBlue.GetComponent<RectTransform>().sizeDelta = new Vector2(438, 580);
            highLightBlue.GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.05f, 1);
        }
        //Event.SetScrollRect(null);
        //if (angle < 30 && line.magnitude > 0.5f)
        //{
        //    go.SetActive(true);
        //    Event.SetScrollRect(null);
        //    if (ProgressionController.instance == null)
        //    {
        //        highLightBlue.SetActive(true);
        //        highLightBlue.transform.SetParent(lstCardCustom);
        //        highLightBlue.GetComponent<RectTransform>().localPosition = Vector2.zero;
        //        highLightBlue.GetComponent<RectTransform>().sizeDelta = new Vector2(438, 580);
        //        highLightBlue.GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.05f, 1);
        //    }
        //    else
        //    {
        //        highLightBlue.SetActive(true);
        //    }
        //}
        //else if (angle > 30 && !go.activeInHierarchy && line.magnitude > 0.5f)
        //{
        //    go.SetActive(false);
        //    Event.SetScrollRect(scroll);
        //    highLightBlue.SetActive(false);
        //}
        //else if (angle > 30 && go.activeInHierarchy)
        //{
        //    highLightBlue.SetActive(false);
        //}
    }
    private void OnDropCard(GameObject go)
    {
        if(cardData.lst.Count > 0)
        {
            CanvasGroup cvGroup = go.GetComponent<CanvasGroup>();
            cvGroup.blocksRaycasts = true;
            Event.SetScrollRect(scroll);
            bool move = false;
            if (angle < 90 && line.magnitude > 1f)
            {
                Destroy(go, 0.5f);
                if (cardData.lst[0].GetDatabase().type == DBHero.TYPE_GOD)
                {
                    god.GetComponent<CardUserInfor>().CreatScaleEffect(countHC);
                }
                if (cardData.lst[0].GetDatabase().type == DBHero.TYPE_TROOPER_MAGIC || cardData.lst[0].GetDatabase().type == DBHero.TYPE_BUFF_MAGIC)
                {
                    spell.GetComponent<CardUserInfor>().CreatScaleEffect(countHC);
                }
                if (cardData.lst[0].GetDatabase().type == DBHero.TYPE_TROOPER_NORMAL)
                {
                    minion.GetComponent<CardUserInfor>().CreatScaleEffect(countHC);
                }

                Event.rectTransform = this.GetComponent<RectTransform>();
                isDrag = false;
                move = true;
            }
            else
            {
                Destroy(go);
                Event.rectTransform = this.GetComponent<RectTransform>();
                isDrag = false;
            }
            highLightBlue.SetActive(false);
            Event.rectTransform = this.GetComponent<RectTransform>();
            isDrag = false;
            if (onEndDragCB != null)
            {
                onEndDragCB(cardData, go, move);
            }
        }
    }
    private void OnDestroy()
    {
        CellHeroCard.main = null;
    }

    public void TurnCardPowerFrame(bool turn)
    {
        switch (cardType)
        {
            case (int)DBHero.TYPE_GOD:
                {
                    god.GetComponent<CardUserInfor>().SetPowerFrame(turn);
                    break;
                }
            case (int)DBHero.TYPE_TROOPER_NORMAL:
                {
                    minion.GetComponent<CardUserInfor>().SetPowerFrame(turn);
                    break;
                }
            case (int)DBHero.TYPE_TROOPER_MAGIC:
                {
                    spell.GetComponent<CardUserInfor>().SetPowerFrame(turn);
                    break;
                }
            case (int)DBHero.TYPE_BUFF_MAGIC:
                {
                    spell.GetComponent<CardUserInfor>().SetPowerFrame(turn);
                    break;
                }
        }
    }

}
