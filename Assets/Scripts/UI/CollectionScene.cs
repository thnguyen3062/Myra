using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GIKCore;
using GIKCore.Net;
using GIKCore.Lang;
using GIKCore.DB;
using GIKCore.Utilities;
using GIKCore.UI;
using GIKCore.Sound;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using pbdson;
using DG.Tweening;
using UIEngine.UIPool;
using UIEngine.Extensions;
using SimpleJSON;
using System;
public class CollectionScene : GameListener
{
    // Fields
    [SerializeField] private List<GameObject> m_ListBackground;
    [SerializeField] private List<SidebarTabOption> m_SidebarTabOptions;
    [SerializeField] private List<GameObject> m_ListOptionContents;
    [SerializeField] private GridPoolGroup m_ListDeckPool, m_ListItemPool;
    [SerializeField] private VerticalPoolGroup m_ListPacks;
    [SerializeField] private TextMeshProUGUI m_Gods, m_AllCards, m_MainGod;
    [SerializeField] private GoOnOff m_TabPackList;

    [SerializeField] private GameObject m_PackClone;
    [SerializeField] private RewardSceneUI m_RewardPopup;
    [SerializeField] private CustomDeckScene m_CustomDeckPopup;
    [SerializeField] private NewCustomDeckPopup m_NewCustomDeckPopup;
    [SerializeField] private CollectionDeckDetail m_CollectionDeckDetail;

    [SerializeField] private GameObject m_CardPreview, m_MinionCardPreview, m_GodCardPreview, m_SpellCardPreview;
    [SerializeField] private GameObject m_PopupSelectMode, m_ButtonNormalDeck, m_ButtonEventDeck;
    [SerializeField] private List<GameObject> m_HighLightDeck = new List<GameObject>();

    // Open Pack
    [SerializeField] private GameObject m_VfxPackDefault, m_VfxPackDragIn, m_VfxPackExplodeNormal, m_VfxPackExplodeLegendary, m_VfxPackExpldeImmortal;
    [SerializeField] private GameObject m_VfxPackExplodePremiumNormal, m_VfxPackExplodePremiumLegendary, m_VfxPackExpldePremiumImmortal;
    [SerializeField] private RectTransform m_PackMainContent;
    [SerializeField] private List<Transform> m_ListVfxRectTransforms;
    [SerializeField] private GameObject m_TabPackBlock;
    [SerializeField] private GameObject m_RewardPremiumSpinPopup;
    [SerializeField] private RewardPremiumCardPopup m_RewardPremiumCardPopup;


    [SerializeField] private GameObject m_CollectionCardPreview;
    [SerializeField] private CardPreviewInfo m_CollectionGodCardPreview, m_CollectionTroopCardPreview, m_CollectionSpellCardPreview;

    // Panel Upgrade
    [SerializeField] private RecycleLayoutGroup m_ListUpgradePoolAvaiable;
    [SerializeField] private RecycleLayoutGroup m_ListUpgradePoolInProgress;
    [SerializeField] private RecycleLayoutGroup m_ListUpgradePoolDark;
    [SerializeField] private RecycleLayoutGroup m_ListUpgradePoolToBeFound;
    [SerializeField] private TextMeshProUGUI m_TextCountUpgradeAvailable;
    [SerializeField] private TextMeshProUGUI m_TxtCardOwned;
    [SerializeField] private TextMeshProUGUI m_TxtCountDark;
    [SerializeField] private TextMeshProUGUI m_TxtCountDiamond;
    [SerializeField] private TextMeshProUGUI m_TxtCountGold;
    [SerializeField] private TextMeshProUGUI m_TxtCountSilver;
    [SerializeField] private TextMeshProUGUI m_TxtCountIron;
    [SerializeField] private RectTransform m_UpgradeLayoutPoolAvailable;
    [SerializeField] private RectTransform m_UpgradeLayoutPoolInProgress;
    [SerializeField] private RectTransform m_UpgradeLayoutPoolDark;
    [SerializeField] private RectTransform m_UpgradeLayoutPoolToBeFound;
    [SerializeField] private RectTransform m_UpgradeLayoutAvailable;
    [SerializeField] private RectTransform m_UpgradeLayoutInProgress;
    [SerializeField] private RectTransform m_UpgradeLayoutDark;
    [SerializeField] private RectTransform m_UpgradeLayoutToBeFound;
    [SerializeField] private RectTransform m_UpgradeLayout;

    // Values
    public static CollectionScene instance;
    private int deckDetailId;
    private List<UserItem> lstUserItemPack = new List<UserItem>();
    private List<UserItem> lstUserItemUse = new List<UserItem>();
    private int openingPack = 0;

    // upgrade
    private bool isFirstUpgrade;
    private bool isInUpgrade = false;
    private int cardOwned, cardUnlockSize, darkCount, diamondCount, goldCount, silverCount, ironCount;
    private List<CellCardUpgradeProps> lstCardUpgradeAvailable = new List<CellCardUpgradeProps>();
    private List<CellCardUpgradeProps> lstCardUpgradeInProgress = new List<CellCardUpgradeProps>();
    private List<CellCardUpgradeProps> lstCardUpgradeBlack = new List<CellCardUpgradeProps>();
    private List<CellCardUpgradeProps> lstCardUpgradeToBeFound = new List<CellCardUpgradeProps>();

    // custom deck
    private bool isFirstDeck;
    private bool hadDeckNormal = false;
    private bool hadDeckEvent = true;
    private int currentDeck = -1;

    // Methods
    public void InitDataContent(int type)
    {
        //if progress thif clear 
        SoundHandler.main.PlaySFX("900_click_4", "sounds");
        if (ProgressionController.instance != null)
        {
            //lstCardUpgradeAvailable.Clear();
            //lstCardUpgradeInProgress.Clear();
            if (type != 1)
                return;

        }
        if (type == 0 || type == 1 || type == 3 || type == 2)
        {
            foreach (SidebarTabOption option in m_SidebarTabOptions)
            {
                option.SetActive(type == option.m_TabId);
            }
            foreach (GameObject optionContent in m_ListOptionContents)
            {
                optionContent.SetActive(false);
            }
            m_SidebarTabOptions[type].SetActive(true);
            m_ListOptionContents[type].SetActive(true);
            isInUpgrade = false;
            switch (type)
            {
                case 0:
                    {
                        if(isFirstDeck)
                        {
                            PopupFirstTime.Show("decks");
                            isFirstDeck = false;
                        }    
                        m_ListBackground[0].SetActive(true);
                        m_ListBackground[1].SetActive(false);
                        if (GameData.main.lstDeckInfo.Count != 0)
                            m_ListDeckPool.SetAdapter(GameData.main.lstDeckInfo);
                        break;
                    }
                case 1:
                    {
                        if (isFirstUpgrade)
                        {
                            PopupFirstTime.Show("upgrade");
                            isFirstUpgrade = false;
                        }
                        isInUpgrade = true;
                        m_ListBackground[0].SetActive(true);
                        m_ListBackground[1].SetActive(false);
                        InitDataListUpgrade();
                        break;
                    }
                case 3:
                    {
                        m_ListBackground[0].SetActive(false);
                        m_ListBackground[1].SetActive(true);
                        float MAIN_SCALE_X = 13.7f;
                        float MAIN_SCALE_Y = 16.5f;
                        float MAIN_WIDTH = 1236f;
                        float MAIN_HEIGHT = 980f;

                        float scaleX = MAIN_SCALE_X * (m_PackMainContent.rect.width / MAIN_WIDTH);
                        float scaleY = MAIN_SCALE_Y * (m_PackMainContent.rect.height / MAIN_HEIGHT);
                        foreach (Transform t in m_ListVfxRectTransforms)
                        {
                            t.localScale = new Vector3(scaleX, scaleY, 1);
                        }
                        m_VfxPackDefault.SetActive(true);
                        break;
                    }
                case 2:
                    {
                        m_ListBackground[0].SetActive(true);
                        m_ListBackground[1].SetActive(false);
                        m_ListItemPool.SetAdapter(lstUserItemUse);
                        break;
                    }
            }
        }
        else
            Toast.Show(LangHandler.Get("toast-8", "Coming Soon!"));
    }
    private void InitDataDeckDetail(int countGod, int countCard)
    {
        if (GameData.main.userProgressionState < 14)
            return;
        m_CollectionDeckDetail.SetData(deckDetailId, countGod, countCard);
    }
    private void InitDataListUpgrade()
    {
        if(ProgressionController.instance!= null)
        {
            if (GameData.main.userProgressionState == 4)
            {
                cardOwned = 30;
                cardUnlockSize = 160;
                darkCount = 10;
                diamondCount = 5;
                goldCount = 10;
                silverCount = 10;
                ironCount = 10;

                lstCardUpgradeAvailable.Clear();

                CellCardUpgradeProps ccu107 = new CellCardUpgradeProps();
                ccu107.heroId = 107;
                ccu107.frameId = 1;
                ccu107.copyAvailable = 5;
                ccu107.copyRequired = 3;
                ccu107.isPointClick = true;
                lstCardUpgradeAvailable.Add(ccu107);

                CellCardUpgradeProps ccu22 = new CellCardUpgradeProps();
                ccu22.heroId = 22;
                ccu22.frameId = 1;
                ccu22.copyAvailable = 3;
                ccu22.copyRequired = 3;

                lstCardUpgradeAvailable.Add(ccu22);
                m_ListUpgradePoolAvaiable.SetAdapter(lstCardUpgradeAvailable);
                lstCardUpgradeInProgress.Clear();
                lstCardUpgradeBlack.Clear();
                lstCardUpgradeToBeFound.Clear();
            }
            if (GameData.main.userProgressionState == 8)
            {
                cardOwned = 30;
                cardUnlockSize = 160;
                darkCount = 10;
                diamondCount = 5;
                goldCount = 10;
                silverCount = 10;
                ironCount = 10;

                lstCardUpgradeAvailable.Clear();

                CellCardUpgradeProps ccu107 = new CellCardUpgradeProps();
                ccu107.heroId = 107;
                ccu107.frameId = 2;
                ccu107.copyAvailable = 10;
                ccu107.copyRequired = 7;
                ccu107.isPointClick = true;
                lstCardUpgradeAvailable.Add(ccu107);

                CellCardUpgradeProps ccu22 = new CellCardUpgradeProps();
                ccu22.heroId = 22;
                ccu22.frameId = 1;
                ccu22.copyAvailable = 4;
                ccu22.copyRequired = 3;

                lstCardUpgradeAvailable.Add(ccu22);
                m_ListUpgradePoolAvaiable.SetAdapter(lstCardUpgradeAvailable);
                lstCardUpgradeInProgress.Clear();
                lstCardUpgradeBlack.Clear();
                lstCardUpgradeToBeFound.Clear();
            }


        }    
        m_TextCountUpgradeAvailable.text = LangHandler.Get("651", "UPGRADE AVAILABLE") + ": " + lstCardUpgradeAvailable.Count;
        m_TxtCardOwned.text = string.Format("{2}: {0}/{1}", cardOwned, cardUnlockSize, LangHandler.Get("650", "CARDS OWNED"));
        m_TxtCountDark.text = LangHandler.Get("652", "DARK") + ": " + darkCount;
        m_TxtCountDiamond.text = LangHandler.Get("653", "DIAMOND") + ": " + diamondCount;
        m_TxtCountGold.text = LangHandler.Get("654", "GOLD") + ": " + goldCount;
        m_TxtCountSilver.text = LangHandler.Get("655", "SILVER") + ": " + silverCount;
        m_TxtCountIron.text = LangHandler.Get("656", "IRON") + ": " + ironCount;
        m_ListUpgradePoolAvaiable.SetAdapter(lstCardUpgradeAvailable);
        m_ListUpgradePoolInProgress.SetAdapter(lstCardUpgradeInProgress);
        m_ListUpgradePoolDark.SetAdapter(lstCardUpgradeBlack);

        m_ListUpgradePoolToBeFound.SetAdapter(lstCardUpgradeToBeFound);

        LayoutRebuilder.ForceRebuildLayoutImmediate(m_UpgradeLayoutPoolAvailable);
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_UpgradeLayoutPoolInProgress);
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_UpgradeLayoutPoolDark);
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_UpgradeLayoutPoolToBeFound);
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_UpgradeLayoutAvailable);
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_UpgradeLayoutInProgress);
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_UpgradeLayoutDark);
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_UpgradeLayoutToBeFound);
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_UpgradeLayout);
    }
    private void OpenNormalPack()
    {

    }
    private void OpenPremiumPack()
    {

    }
    public override bool ProcessSocketData(int serviceId, byte[] data)
    {
        if (base.ProcessSocketData(serviceId, data)) return true;
        switch (serviceId)
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
                        CommonVector cv1 = listCommonVector.aVector[1];
                        CommonVector cv2 = listCommonVector.aVector[2];

                        int BLOCK_COUNT_1 = cv1.aLong.Count;
                        int BLOCK_COUNT_2 = cv2.aLong.Count;

                        InitDataDeckDetail(BLOCK_COUNT_1, BLOCK_COUNT_2 + BLOCK_COUNT_1);
                    }

                    break;
                }
            case IService.GET_USER_ITEMS:
                {
                    lstUserItemPack.Clear();
                    lstUserItemUse.Clear();
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    int START_LONG = 0;
                    int START_STRING = 0;
                    int BLOCK_LONG = 4;
                    int BLOCK_STRING = 2;

                    int COUNT_LONG = (cv.aLong.Count - START_LONG) / BLOCK_LONG;

                    for (int i = 0; i < COUNT_LONG; i++)
                    {
                        UserItem uip = new UserItem();
                        uip.itemId = (int)cv.aLong[START_LONG + i * BLOCK_LONG + 0];
                        uip.type = (int)cv.aLong[START_LONG + i * BLOCK_LONG + 1];
                        uip.expire = cv.aLong[START_LONG + i * BLOCK_LONG + 2];
                        uip.quantity = (int)cv.aLong[START_LONG + i * BLOCK_LONG + 3];

                        uip.name = cv.aString[START_STRING + i * BLOCK_STRING + 0];
                        uip.image = cv.aString[START_STRING + i * BLOCK_STRING + 1];
                        if (uip.quantity > 0)
                        {
                            if (uip.type == 1)
                            {
                                lstUserItemPack.Add(uip);
                            }
                            else
                            {
                                lstUserItemUse.Add(uip);
                            }
                        }

                    }
                    m_TabPackList.Turn(lstUserItemPack.Count > 0);

                    if (lstUserItemPack.Count > 0)
                    {
                        m_ListPacks.SetAdapter(lstUserItemPack);
                    }
                    break;
                }
            case IService.OPEN_CHEST:
                {
                    ListCommonVector lcv = ISocket.Parse<ListCommonVector>(data);
                    CommonVector cv0 = lcv.aVector[0];
                    if (cv0.aLong[0] == 0)
                    {
                        PopupConfirm.Show(content: cv0.aString[0]);
                    }
                    else
                    {
                        // Parse Data

                        // parse cardBuilder
                        #region parse cardbuilder
                        CommonVector cv1 = lcv.aVector[1];
                        int CV1_BLOCK_INT = 5;
                        int CV1_COUNT = cv1.aLong.Count / CV1_BLOCK_INT;
                        List<RewardCardBuilder> lstCardBuilder = new List<RewardCardBuilder>();
                        for (int i = 0; i < CV1_COUNT; i++)
                        {
                            RewardCardBuilder rcb = new RewardCardBuilder();
                            rcb.heroId = cv1.aLong[i * CV1_BLOCK_INT + 0];
                            rcb.frame = (int)cv1.aLong[i * CV1_BLOCK_INT + 1];
                            rcb.realRewardCount = (int)cv1.aLong[i * CV1_BLOCK_INT + 2];
                            rcb.realIronCopy = (int)cv1.aLong[i * CV1_BLOCK_INT + 3];
                            rcb.requireCopy = (int)cv1.aLong[i * CV1_BLOCK_INT + 4];
                            lstCardBuilder.Add(rcb);
                        }
                        #endregion

                        // parse duplicateGoldBuilder
                        #region parse duplicateGoldBuilder
                        CommonVector cv2 = lcv.aVector[2];
                        RewardDuplicateGoldBuilder rdgb = new RewardDuplicateGoldBuilder();
                        rdgb.duplicateGold = (int)cv2.aLong[0];
                        rdgb.curGold = (int)cv2.aLong[1];
                        #endregion

                        // parse balanceBuilder
                        #region parse balanceBuilder
                        CommonVector cv3 = lcv.aVector[3];
                        RewardBalanceBuilder rbb = new RewardBalanceBuilder();
                        rbb.lvGold = (int)cv3.aLong[0];
                        rbb.curGold = (int)cv3.aLong[1];
                        rbb.lvExp = (int)cv3.aLong[2];
                        rbb.curExp = (int)cv3.aLong[3];
                        rbb.lvEssence = (int)cv3.aLong[4];
                        rbb.curEssence = (int)cv3.aLong[5];
                        #endregion

                        // parse itemBuilder
                        #region parse itemBuilder
                        CommonVector cv4 = lcv.aVector[4];
                        List<RewardItemBuilder> listRewardItemBuilder = new List<RewardItemBuilder>();
                        int CV5_BLOCK_INT = 3;
                        int CV5_BLOCK_STRING = 2;
                        int CV5_COUNT = cv4.aLong.Count / CV5_BLOCK_INT;
                        for (int i = 0; i < CV5_COUNT; i++)
                        {
                            RewardItemBuilder rib = new RewardItemBuilder();
                            rib.id = cv4.aLong[i * CV5_BLOCK_INT + 0];
                            rib.count = (int)cv4.aLong[i * CV5_BLOCK_INT + 1];
                            rib.quantity = (int)cv4.aLong[i * CV5_BLOCK_INT + 2];

                            rib.name = cv4.aString[i * CV5_BLOCK_STRING + 0];
                            rib.image = cv4.aString[i * CV5_BLOCK_STRING + 1];
                            if (GameData.main.DictRewardSprite.ContainsKey(rib.image))
                            {
                                rib.sprite = GameData.main.DictRewardSprite[rib.image];
                            }
                            else
                            {
                                IUtil.SetMapRewardSprite(rib.image);
                                if (GameData.main.DictRewardSprite.ContainsKey(rib.image))
                                {
                                    rib.sprite = GameData.main.DictRewardSprite[rib.image];
                                }
                            }
                            listRewardItemBuilder.Add(rib);
                        }
                        #endregion

                        PopupOpenRewardProps props = new PopupOpenRewardProps();
                        props.listRewardCardBuilder = lstCardBuilder;
                        props.rewardDuplicateGoldBuilder = rdgb;
                        props.rewardBalanceBuilder = rbb;

                        m_VfxPackExplodePremiumLegendary.SetActive(true);
                        m_VfxPackDefault.SetActive(false);
                        m_VfxPackDragIn.SetActive(false);
                        m_TabPackBlock.SetActive(true);
                        StartCoroutine(IUtil.Delay(() =>
                        {
                            m_TabPackBlock.SetActive(false);
                            m_VfxPackExplodePremiumLegendary.SetActive(false);
                            m_VfxPackDefault.SetActive(true);
                            m_TabPackBlock.SetActive(false);
                            PopupOpenReward.Show(props);
                        }, 3f));
                        UserItem uip = lstUserItemPack.Find(x => x.itemId == openingPack);
                        if (uip != null)
                        {
                            if (uip.quantity == 1)
                            {
                                int idx = lstUserItemPack.FindIndex(x => x.itemId == openingPack);
                                lstUserItemPack.RemoveAt(idx);
                            }
                            else
                            {
                                uip.quantity--;
                            }
                            m_ListPacks.SetAdapter(lstUserItemPack, false);
                            m_TabPackList.Turn(lstUserItemPack.Count > 0);
                        }
                        Game.main.socket.GetUserCurrency();
                    }
                    Game.main.socket.GetUserHeroCard();
                    Game.main.socket.ViewUpgrade();
                    break;
                }
            case IService.GET_USER_DECK:
                {
                    m_CollectionDeckDetail.CloseDeckDetail();
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    GameData.main.lstDeckInfo = new List<DeckInfo>();
                    int START_LONG = 1;
                    int COUNT_LONG = cv.aString.Count;
                    int BLOCK = 0;
                    for (int i = 0; i < COUNT_LONG; i++)
                    {
                        DeckInfo info = new DeckInfo()
                        {
                            isLastDeck = cv.aLong[0] == cv.aLong[START_LONG + BLOCK + 0],
                            deckID = cv.aLong[START_LONG + BLOCK + 0],
                            deckStatus = cv.aLong[START_LONG + BLOCK + 1],
                            isDefaultDeck = cv.aLong[START_LONG + BLOCK + 2] == 1,
                            deckName = cv.aString[i]
                        };
                        int numberGod = (int)cv.aLong[START_LONG + 3 + BLOCK];
                        if (numberGod > 0)
                        {
                            info.godId = cv.aLong[START_LONG + BLOCK + 4];
                        }
                        else
                        {
                            info.godId = -1;
                        }
                        for (int j = 0; j < 3; j++)
                        {
                            if (j < numberGod)
                                info.lstGodIds.SetValue(cv.aLong[START_LONG + 4 + BLOCK + j], j);
                            else
                                info.lstGodIds.SetValue(-1, j);
                        }
                        BLOCK = BLOCK + 4 + numberGod;


                        GameData.main.lstDeckInfo.Add(info);
                    }
                    m_ListDeckPool.SetAdapter(GameData.main.lstDeckInfo);
                    break;
                }
            case IService.DELETE_USER_DECK:
                {
                    Toast.Show(LangHandler.Get("160", "Delete Deck Successfully!"));
                    hadDeckNormal = false;
                    break;
                }
            case IService.VIEW_UPGRADE:
                {
                    ListCommonVector lcv = ISocket.Parse<ListCommonVector>(data);

                    // first upgrade
                    CommonVector cv0 = lcv.aVector[0];
                    isFirstUpgrade = cv0.aLong[0] == 0;

                    //overview
                    CommonVector cv1 = lcv.aVector[1];
                    cardOwned = (int)cv1.aLong[0];
                    cardUnlockSize = (int)cv1.aLong[1];
                    darkCount = (int)cv1.aLong[2];
                    diamondCount = (int)cv1.aLong[3];
                    goldCount = (int)cv1.aLong[4];
                    silverCount = (int)cv1.aLong[5];
                    ironCount = (int)cv1.aLong[6];

                    // upgrade available
                    CommonVector cv2 = lcv.aVector[2];
                    int CV2_BLOCK_INT = 4;
                    int CV2_COUNT = cv2.aLong.Count / CV2_BLOCK_INT;
                    lstCardUpgradeAvailable.Clear();
                    for (int i = 0; i < CV2_COUNT; i++)
                    {
                        CellCardUpgradeProps ccu = new CellCardUpgradeProps();
                        ccu.heroId = cv2.aLong[i * CV2_BLOCK_INT + 0];
                        ccu.frameId = (int)cv2.aLong[i * CV2_BLOCK_INT + 1];
                        ccu.copyAvailable = (int)cv2.aLong[i * CV2_BLOCK_INT + 2];
                        ccu.copyRequired = (int)cv2.aLong[i * CV2_BLOCK_INT + 3];
                        lstCardUpgradeAvailable.Add(ccu);
                    }

                    // black frame
                    CommonVector cv3 = lcv.aVector[3];
                    lstCardUpgradeBlack.Clear();
                    for (int i = 0; i < cv3.aLong.Count; i++)
                    {
                        CellCardUpgradeProps ccu = new CellCardUpgradeProps();
                        ccu.heroId = cv3.aLong[i];
                        ccu.frameId = 5;
                        ccu.copyAvailable = 0;
                        ccu.copyRequired = 0;
                        lstCardUpgradeBlack.Add(ccu);
                    }

                    // in progress
                    CommonVector cv4 = lcv.aVector[4];
                    int CV4_BLOCK_INT = 4;
                    int CV4_COUNT = cv4.aLong.Count / CV4_BLOCK_INT;
                    lstCardUpgradeInProgress.Clear();
                    for (int i = 0; i < CV4_COUNT; i++)
                    {
                        CellCardUpgradeProps ccu = new CellCardUpgradeProps();
                        ccu.heroId = cv4.aLong[i * CV4_BLOCK_INT + 0];
                        ccu.frameId = (int)cv4.aLong[i * CV4_BLOCK_INT + 1];
                        ccu.copyAvailable = (int)cv4.aLong[i * CV4_BLOCK_INT + 2];
                        ccu.copyRequired = (int)cv4.aLong[i * CV4_BLOCK_INT + 3];
                        lstCardUpgradeInProgress.Add(ccu);
                    }

                    // card to be found
                    CommonVector cv5 = lcv.aVector[5];
                    lstCardUpgradeToBeFound.Clear();
                    for (int i = 0; i < cv5.aLong.Count; i++)
                    {
                        CellCardUpgradeProps ccu = new CellCardUpgradeProps();
                        ccu.heroId = cv5.aLong[i];
                        ccu.frameId = 1;
                        ccu.copyAvailable = 0;
                        ccu.copyRequired = 0;
                        lstCardUpgradeToBeFound.Add(ccu);
                    }

                    if(isInUpgrade)
                    {
                        InitDataListUpgrade();
                    }
                    break;
                }
            case IService.VIEW_UPGRADE_DETAIl:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    PopupUpgradeProps props = new PopupUpgradeProps();
                    for (int i = 0; i < cv.aLong.Count; i++)
                    {
                        if (i == 0)
                            props.status = (int)cv.aLong[i];
                        else if (i == 1)
                            props.heroId = cv.aLong[i];
                        else if (i == 2)
                            props.type = (int)cv.aLong[i];
                        else if (i == 3)
                            props.frameId = (int)cv.aLong[i];
                        else if (i == 4)
                            props.curAtk = (int)cv.aLong[i];
                        else if (i == 5)
                            props.curHp = (int)cv.aLong[i];
                        else if (i == 6)
                            props.curMana = (int)cv.aLong[i];
                        else if (i == 7)
                            props.gold = (int)cv.aLong[i];
                        else if (i == 8)
                            props.goldRequired = (int)cv.aLong[i];
                        else if (i == 9)
                            props.essence = (int)cv.aLong[i];
                        else if (i == 10)
                            props.essenceRequired = (int)cv.aLong[i];
                        else if (i == 11)
                            props.copyAvailable = (int)cv.aLong[i];
                        else if (i == 12)
                            props.copyRequired = (int)cv.aLong[i];
                        else if (i == 13)
                            props.canUpgrade = (int)cv.aLong[i] == 1;
                        else if (i == 14)
                            props.nextAtk = (int)cv.aLong[i];
                        else if (i == 15)
                            props.nextHp = (int)cv.aLong[i];
                        else if (i == 16)
                            props.nextMana = (int)cv.aLong[i];
                        else if (i == 17)
                            props.shardBonus = (int)cv.aLong[i];

                    }
                    DBHero h = Database.GetHero(props.heroId);
                    if(h != null)
                    {
                        if (props.curAtk == -1)
                            props.curAtk = (int)h.atk;
                        if (props.curHp == -1)
                            props.curHp = (int)h.hp;
                        if (props.curMana == -1)
                            props.curMana = (int)h.mana;
                    }
                    if (props.nextAtk == -1)
                        props.nextAtk = props.curAtk;
                    if (props.nextHp == -1)
                        props.nextHp = props.curHp;
                    if (props.nextMana == -1)
                        props.nextMana = props.curMana;
                    if (props.frameId <= 0)
                        props.frameId = 1;
                    PopupUpgrade.Show(props);
                    break;
                }
            case IService.FIRST_DECK:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    isFirstDeck = cv.aLong[0] == 0;
                    if(isFirstDeck)
                    {
                        PopupFirstTime.Show("decks");
                    }
                    break;
                }
        }
        return false;
    }
    void Start()
    {

    }
    private void OnDisable()
    {
        //NavFooter.instance.onChangeState -= DoCloseAllVfx;
    }
    protected override void Awake()
    {
        DoCloseAllVfx();
        instance = this;
        base.Awake();
        //NavFooter.instance.onChangeState += DoCloseAllVfx;
        m_ListDeckPool.SetCellDataCallback((GameObject go, DeckInfo data, int index) =>
        {
            CellCollectionDeck script = go.GetComponent<CellCollectionDeck>();
            script.SetData(data).SetOnClickCallback((script) =>
            {
                deckDetailId = script.deckId;
                Game.main.socket.GetUserDeckDetail(script.deckId);
                for (int i = 0; i < GameData.main.lstDeckInfo.Count; i++)
                {
                    GameData.main.lstDeckInfo[i].isSelected = GameData.main.lstDeckInfo[i].deckID == data.deckID;
                }
                m_ListDeckPool.SetAdapter(GameData.main.lstDeckInfo, false);
            });
        });
        m_ListPacks.SetCellDataCallback((GameObject go, UserItem data, int index) =>
        {
            UserItemPackPrefab script = go.GetComponent<UserItemPackPrefab>();
            script.SetData(data);

            UIDragDrop dragdrop = go.GetComponent<UIDragDrop>();
            dragdrop
            .SetOnDragCallback((x) =>
            {
                if (x.transform.position.x >= -5f && x.transform.position.x <= 8f && x.transform.position.y >= -3f && x.transform.position.y <= 6.5)
                {
                    m_VfxPackDefault.SetActive(false);
                    m_VfxPackDragIn.SetActive(true);
                }
                else
                {
                    m_VfxPackDefault.SetActive(true);
                    m_VfxPackDragIn.SetActive(false);
                }
            })
            .SetOnPointerDownCallback((x) =>
            {
                x.transform.position = go.transform.position;
                dragdrop.SetScrollRect(null);
            })
            .SetOnPointerExitCallback((x) =>
            {
                dragdrop.SetScrollRect(m_ListPacks.scrollRect);
            })
            .SetOnBeginDragCallback((x) =>
            {
                x.gameObject.SetActive(true);
                dragdrop.rectTransform.GetChild(0).GetComponent<Image>().sprite = data.sprite;
            })
            .SetOnEndDragCallback((x) =>
            {
                x.gameObject.SetActive(false);
                if (x.transform.position.x >= -5f && x.transform.position.x <= 8f && x.transform.position.y >= -3f && x.transform.position.y <= 6.5)
                {
                    openingPack = data.itemId;
                    Game.main.socket.OpenUserPack(data.itemId);
                }
            });
        });
        m_ListItemPool.SetCellDataCallback((GameObject go, UserItem data, int index) =>
        {
            go.GetComponent<UserItemPackPrefab>().SetData(data);
        });
        m_ListUpgradePoolAvaiable.SetCellDataCallback((GameObject go, CellCardUpgradeProps data, int index) =>
        {
            CellCardUpgrade script = go.GetComponent<CellCardUpgrade>();
            script.SetData(data).SetOnclickCallBack((heroId) =>
            {
                if(ProgressionController.instance == null)
                    Game.main.socket.ViewUpgradeDetail(heroId);
                else
                {
                    script.m_PointClick.SetActive(false);
                    if (heroId == 107)
                    {
                        ProgressionController.instance.NextState();
                    }    
                }    
            });
        });
        m_ListUpgradePoolInProgress.SetCellDataCallback((GameObject go, CellCardUpgradeProps data, int index) =>
        {
            CellCardUpgrade script = go.GetComponent<CellCardUpgrade>();
            script.SetData(data).SetOnclickCallBack((heroId) =>
            {
                Game.main.socket.ViewUpgradeDetail(heroId);
            });
        });
        m_ListUpgradePoolDark.SetCellDataCallback((GameObject go, CellCardUpgradeProps data, int index) =>
        {
            CellCardUpgrade script = go.GetComponent<CellCardUpgrade>();
            script.SetData(data).SetOnclickCallBack((heroId) =>
            {
                Game.main.socket.ViewUpgradeDetail(heroId);
            });
        });
        m_ListUpgradePoolToBeFound.SetCellDataCallback((GameObject go, CellCardUpgradeProps data, int index) =>
        {
            CellCardUpgrade script = go.GetComponent<CellCardUpgrade>();
            script.SetData(data).SetOnclickCallBack((heroId) =>
            {
                Game.main.socket.ViewUpgradeDetail(heroId);
            });
        });
    }
    public void DoClickFirstTimeDeck()
    {
        PopupFirstTime.Show("decks");
    }
    public void DoClickFirstTimeUpgrade()
    {
        PopupFirstTime.Show("upgrade");
    }
    public void DoClickButtonBack()
    {
        SoundHandler.main.PlaySFX("900_click_4", "sounds");
        if (ProgressionController.instance != null)
            return;
        Game.main.LoadScene("HomeSceneNew", delay: 0.3f, curtain: true);
    }
    public void DoClickButtonDisable()
    {
        Toast.Show(LangHandler.Get("toast-8", "Coming Soon!"));
    }
    private void OnBeginDragDeckCard()
    {

    }
    private void OnDropCard(GameObject go)
    {

    }
    public void ClosePreviewHandCard()
    {
        if (m_CardPreview.gameObject.activeSelf)
        {
            m_GodCardPreview.gameObject.SetActive(false);
            m_MinionCardPreview.gameObject.SetActive(false);
            m_CardPreview.gameObject.SetActive(false);
            m_SpellCardPreview.gameObject.SetActive(false);
        }
    }

    //custom deck
    public void DoClickCreatNewDeck()
    {
        SoundHandler.main.PlaySFX("900_click_3", "sounds");
        m_ButtonNormalDeck.transform.parent.gameObject.SetActive(!hadDeckNormal);
        m_ButtonEventDeck.transform.parent.gameObject.SetActive(!hadDeckEvent);
        if (hadDeckNormal)
        {
            m_ButtonNormalDeck.GetComponent<Button>().interactable = false;
            m_ButtonNormalDeck.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
        }
        else
        {
            m_ButtonNormalDeck.GetComponent<Button>().interactable = true;
            m_ButtonNormalDeck.GetComponent<Image>().color = new Color(1f, 1f, 1f);
        }
        if (hadDeckEvent)
        {
            m_ButtonEventDeck.GetComponent<Button>().interactable = false;
            m_ButtonEventDeck.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
        }
        else
        {
            m_ButtonEventDeck.GetComponent<Button>().interactable = true;
            m_ButtonEventDeck.GetComponent<Image>().color = new Color(1f, 1f, 1f);
        }
        m_PopupSelectMode.SetActive(true);
    }
    public void DoClickDonePickDeck()
    {
        DoClickCreatNormalDeck();

        currentDeck = -1;
        m_PopupSelectMode.SetActive(false);
    }
    public void DoPickDeck(int index)
    {
        currentDeck = index;
        foreach (GameObject hl in m_HighLightDeck)
        {
            hl.SetActive(false);
        }
        m_HighLightDeck[index].SetActive(true);
    }
    public void DoClickCreatNormalDeck()
    {
        if (GameData.main.lstDeckInfo.Count < 14)
            m_CustomDeckPopup.gameObject.SetActive(true);
        else
            Toast.Show(LangHandler.Get("161", "Can't create more deck"));
        //if (customDeckScene == null)
        //{
        //    customDeckScene = Instantiate(customDeckPrefab, Game.main.canvas.panelPopup);
        //}
        //customDeckScene.SetActive(true);
    }
    private void DoCloseAllVfx()
    {
        m_VfxPackDefault.SetActive(false);
        m_VfxPackDragIn.SetActive(false);
        //m_VfxPackExpldeImmortal.SetActive(false);
        //m_VfxPackExplodeLegendary.SetActive(false);
        //m_VfxPackExplodeNormal.SetActive(false);

        m_VfxPackExpldePremiumImmortal.SetActive(false);
        m_VfxPackExplodePremiumLegendary.SetActive(false);
        m_VfxPackExplodePremiumNormal.SetActive(false);
    }
    public void DoClickGetMorePack()
    {
        SoundHandler.main.PlaySFX("900_click_3", "sounds");
        Game.main.socket.GetShopItem();
        Game.main.LoadScene("ShopScene", delay: 0.3f, curtain: true, onLoadSceneSuccess: () =>
        {
            ShopScene.instance.DoClickButton(2);
        });
    }
    private void ShowPreviewHandCard(long id, long frame)
    {
        if (m_CollectionCardPreview.activeSelf)
            return;

        m_CollectionCardPreview.SetActive(true);
        DBHero hero = Database.GetHero(id);
        if (hero.type == DBHero.TYPE_GOD)
        {
            m_CollectionGodCardPreview.gameObject.SetActive(true);
            m_CollectionGodCardPreview.SetCardPreview(hero, frame, true);
        }
        if (hero.type == DBHero.TYPE_TROOPER_MAGIC || hero.type == DBHero.TYPE_BUFF_MAGIC)
        {
            m_CollectionSpellCardPreview.gameObject.SetActive(true);
            m_CollectionSpellCardPreview.SetCardPreview(hero, frame, true);
        }
        if (hero.type == DBHero.TYPE_TROOPER_NORMAL)
        {
            m_CollectionTroopCardPreview.gameObject.SetActive(true);
            m_CollectionTroopCardPreview.SetCardPreview(hero, frame, true);
        }
    }
}
