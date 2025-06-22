using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using GIKCore;
using GIKCore.Net;
using GIKCore.Lang;
using GIKCore.UI;
using pbdson;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using UIEngine.UIPool;
using TMPro;
using PathologicalGames;
using GIKCore.Utilities;
using SimpleJSON;
using UnityEngine.UIElements.Experimental;
using GIKCore.Sound;

public class ShopScene : GameListener
{
    // Fields
    [SerializeField] private List<SidebarTabOption> m_ListSidebarOption = new List<SidebarTabOption>(); // 0 => Recommended, 1 => Gem Topups, 2 => Packs, 3 => Valuables, 4 => Cosmetics
    [SerializeField] private VerticalPoolGroup m_Pool;
    [SerializeField] private VerticalPoolGroup m_ListPackage, m_ListReward;
    [SerializeField] private HorizontalPoolGroup m_ListTopup;
    [SerializeField] private GameObject m_TabGrid, m_TabTopupGems, m_TabRecommended, m_MainContent, m_PackItemDetail, m_PackDetailExpireTag, m_PopupPurchaseComplete, m_PurchaseCompleteNextBtn;
    [SerializeField] private TextMeshProUGUI m_PackDetailTitle, m_PackDetailDesc, m_PackDetailExpire, m_PackDetailCost, m_PopupPurchaseCompleteTitle;
    [SerializeField] private Image m_PackDetailIcon;
    [SerializeField] private GoOnOff m_PackDetailCurrency;
    [SerializeField] private Transform m_PoppupLoadingPrefab;
    [SerializeField] private PopupPurchaseItemValuable m_PopupPurchaseItemValuable;
    [SerializeField] private RectTransform m_slidebaroptionfrom, m_slidebaroptionto;
    [SerializeField] private RectTransform m_SidetabOptions;
    [SerializeField] private List<GameObject> m_LstShopGemVfx = new List<GameObject>();
    [SerializeField] private Image m_BackGround;
    // Values
    private Transform popupLoading;
    private List<ItemInfo> lstShopItemPack = new List<ItemInfo>();
    private List<ItemInfo> lstShopItemValuable = new List<ItemInfo>();
    private List<ItemPackage> lstItemPackages = new List<ItemPackage>();
    private List<ItemReward> lstItemRewards = new List<ItemReward>();
    private bool isFirstShop;

    private bool isShowPackDetail = false;
    private int currency;
    private int itemPrice;
    private int shopItemId = -1;
    private int packageIdx = -1;
    private bool isLoading = false;
    private float timeCount = 5f;
    public static ShopScene instance;

    IEnumerator coroutine = null;

    // Methods
    public void DoClickButton(int type)
    {
        SoundHandler.main.PlaySFX("900_click_4", "sounds");
        if (isShowPackDetail)
        {
            DOTween.Kill(m_slidebaroptionfrom.transform, true);
            DOTween.Kill(m_slidebaroptionto.transform, true);

            m_slidebaroptionfrom.DOAnchorPos(new Vector2(0, 0), 0.5f)
              .OnComplete(() => m_slidebaroptionfrom.gameObject.SetActive(true));
            m_slidebaroptionto.gameObject.SetActive(true);
            // m_slidebaroptionto.anchoredPosition = new Vector2(0, 0);
            m_slidebaroptionto.DOAnchorPos(new Vector2(0, 0), 0f);


            m_PackItemDetail.SetActive(false);
            m_MainContent.SetActive(true);
        }
        foreach (SidebarTabOption sto in m_ListSidebarOption)
        {
            sto.SetActive(type == sto.m_TabId);
        }
        switch (type)
        {
            case 0:
                {
                    //Toast.Show(LangHandler.Get("toast-8", "Coming Soon!"));
                    m_TabGrid.SetActive(false);
                    m_TabTopupGems.SetActive(false);
                    m_TabRecommended.SetActive(true);
                    break;
                }
            case 1:
                {
                    m_TabGrid.SetActive(false);
                    m_TabRecommended.SetActive(false);
                    m_TabTopupGems.SetActive(true);
                    break;
                }
            case 2:
                {
                    List<List<ItemInfo>> lstAdapter = GenerateListItem(GameData.main.lstItemShopPack);
                    m_Pool.SetAdapter(lstAdapter);
                    m_TabRecommended.SetActive(false);
                    m_TabTopupGems.SetActive(false);
                    m_TabGrid.SetActive(true);
                    break;
                }
            case 3:
                {
                    List<List<ItemInfo>> lstAdapter = GenerateListItem(GameData.main.lstItemShopValuable);
                    m_Pool.SetAdapter(lstAdapter);
                    m_TabRecommended.SetActive(false);
                    m_TabTopupGems.SetActive(false);
                    m_TabGrid.SetActive(true);
                    break;
                }
            case 4:
                {
                    Toast.Show(LangHandler.Get("toast-8", "Coming Soon!"));
                    break;
                }
        }
    }
    public void DoOpenTab(int type)
    {
        switch (type)
        {
            case OpenScene.SHOP_SCENE_PACKS:
                {
                    DoClickButton(2);
                    break;
                }
            case OpenScene.SHOP_SCENE_GEM_TOPUPS:
                {
                    DoClickButton(1);
                    break;
                }
            case OpenScene.SHOP_SCENE_VALUABLES:
                {
                    DoClickButton(3);
                    break;
                }
            case OpenScene.SHOP_SCENE_COSMETICS:
                {
                    DoClickButton(4);
                    break;
                }
        }
    }
    public override bool ProcessSocketData(int id, byte[] data)
    {
        if (base.ProcessSocketData(id, data))
            return true;

        switch (id)
        {
            case IService.GET_PACK_DETAIL:
                {
                    ListCommonVector lcv = ISocket.Parse<ListCommonVector>(data);
                    CommonVector res = lcv.aVector[0];
                    if (res.aLong[0] == 1)
                    {
                        lstItemPackages.Clear();
                        lstItemRewards.Clear();
                        CommonVector lstPackages = lcv.aVector[1];
                        CommonVector lstRewards = lcv.aVector[2];

                        int BLOCK_ALONG_PACKAGE = 4;
                        int START_ALONG_PACKAGE = 3;
                        int COUNT_PACKAGE = (lstPackages.aLong.Count - START_ALONG_PACKAGE) / BLOCK_ALONG_PACKAGE;
                        for (int i = 0; i < COUNT_PACKAGE; i++)
                        {
                            ItemPackage ip = new ItemPackage();
                            ip.idx = i;
                            ip.count = (int)lstPackages.aLong[START_ALONG_PACKAGE + i * BLOCK_ALONG_PACKAGE + 0];
                            ip.currency = (int)lstPackages.aLong[START_ALONG_PACKAGE + i * BLOCK_ALONG_PACKAGE + 1];
                            ip.price = (int)lstPackages.aLong[START_ALONG_PACKAGE + i * BLOCK_ALONG_PACKAGE + 2];
                            ip.limit = (int)lstPackages.aLong[START_ALONG_PACKAGE + i * BLOCK_ALONG_PACKAGE + 3];
                            ip.isSelected = (i == 0);
                            lstItemPackages.Add(ip);
                            if (i == 0)
                            {
                                // set default value when pick
                                m_PackDetailCost.text = ip.price + "";
                                m_PackDetailCurrency.Turn(ip.currency == 1);
                                currency = ip.currency;
                                itemPrice = ip.price;
                                packageIdx = ip.idx;
                            }
                        }

                        int BLOCK_ALONG_REWARD = 4;
                        int START_ALONG_REWARD = 0;
                        int COUNT_REWARD = (lstRewards.aLong.Count - START_ALONG_REWARD) / BLOCK_ALONG_REWARD;
                        for (int i = 0; i < COUNT_REWARD; i++)
                        {
                            ItemReward ir = new ItemReward();
                            ir.kind = (int)lstRewards.aLong[START_ALONG_REWARD + i * BLOCK_ALONG_REWARD + 0];
                            ir.id = (int)lstRewards.aLong[START_ALONG_REWARD + i * BLOCK_ALONG_REWARD + 1];
                            ir.frame = (int)lstRewards.aLong[START_ALONG_REWARD + i * BLOCK_ALONG_REWARD + 2];
                            ir.count = (int)lstRewards.aLong[START_ALONG_REWARD + i * BLOCK_ALONG_REWARD + 3];
                            lstItemRewards.Add(ir);
                        }
                    }
                    else
                    {
                        Toast.Show(res.aString[0]);
                    }
                    m_ListPackage.SetAdapter(lstItemPackages);
                    m_ListReward.SetAdapter(lstItemRewards);
                    break;
                }
            case IService.BUY_ITEM:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    int status = (int)cv.aLong[0];
                    if (status == 1)
                    {
                        m_PopupPurchaseCompleteTitle.text = LangHandler.Get("143", "Purchase Complete!");
                    }
                    else
                    {
                        m_PopupPurchaseCompleteTitle.text = LangHandler.Get("144", "Purchase Failed!");
                    }
                    SoundHandler.main.PlaySFX("900_purchase_1", "sounds");
                    m_PopupPurchaseItemValuable.gameObject.SetActive(false);
                    m_PopupPurchaseComplete.SetActive(true);
                    Game.main.socket.GetUserCurrency();
                    break;
                }
            case IService.RECHARGE_GEM:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    int status = (int)cv.aLong[0];
                    if (status == 1)
                    {
                        Game.main.socket.GetUserCurrency();
                        m_PopupPurchaseCompleteTitle.text = LangHandler.Get("143", "Purchase Complete!");
                    }
                    else
                    {
                        m_PopupPurchaseCompleteTitle.text = LangHandler.Get("144", "Purchase Failed!");
                    }
                    m_PopupPurchaseComplete.SetActive(true);
                    break;
                }
        }

        return false;
    }
    public override bool ProcessNetData(int id, object data)
    {
        if (base.ProcessNetData(id, data)) return true;
        switch (id)
        {
            case NetData.RECEIVE_SHOP_PACK_DATA:
                {
                    List<List<ItemInfo>> lstAdapter = GenerateListItem(GameData.main.lstItemShopPack);
                    m_Pool.SetAdapter(lstAdapter);
                    if (GameData.main.isFirstShop)
                    {
                        PopupFirstTime.Show("shop");
                        GameData.main.isFirstShop = false;
                    }
                    break;
                }
            case NetData.ACTION_OPEN_SCENE:
                {
                    int type = OpenScene.SHOP_SCENE_GEM_TOPUPS;
                    DoOpenTab(type);
                    break;
                }
            case NetData.SHOP_CLICK_ITEM:
                {
                    ItemInfo ii = (ItemInfo)data;
                    if (ii.type == TypeItems.TYPE_PACK_SHOP)
                    {
                        Game.main.socket.GetPackDetail(ii.shopItemId);
                        maincontentFadein();
                        // m_MainContent.SetActive(false);
                        m_PackItemDetail.SetActive(true);
                        isShowPackDetail = true;
                        m_PackDetailExpireTag.SetActive(ii.endOffset <= 259200000); // < 3 days
                        m_PackDetailTitle.text = ii.name;
                        m_PackDetailDesc.text = ii.desc;

                        if (ii.sprite != null) SetAvatarSprite(ii.sprite);
                        else LoadHttpAvatar(ii.image, (s) => { ii.sprite = s; });

                        shopItemId = ii.shopItemId;
                    }
                    else if (ii.type == TypeItems.TYPE_VALUABLE_SHOP)
                    {
                        m_PopupPurchaseItemValuable.SetData(ii);
                    }
                    break;
                }
                //case NetData.RECEIVE_SHOP_VALUE_DATA:
                //    {
                //        m_Pool.SetAdapter(GameData.main.lstItemShopValuable);
                //        break;
                //    }
        }
        return false;
    }
    public void DoClickBackCollection()
    {
        Game.main.socket.GetUserPack();
        Game.main.LoadScene("CollectionScene", delay: 0.3f, curtain: true);
    }
    public void DoClickFirstTimeShop()
    {
        PopupFirstTime.Show("shop");
    }

    private void DoCloseAllVfx()
    {
        foreach (GameObject go in m_LstShopGemVfx)
        {
            go.SetActive(false);
        }
    }
    private List<List<ItemInfo>> GenerateListItem(List<ItemInfo> data)
    {
        List<List<ItemInfo>> listItem = new List<List<ItemInfo>>();
        int count = -1;
        for (int i = 0; i < data.Count; i++)
        {
            if (i % 4 == 0)
            {
                List<ItemInfo> lst = new List<ItemInfo>();
                lst.Add(data[i]);

                listItem.Add(lst);
                count++;
            }
            else
            {
                listItem[count].Add(data[i]);
            }
        }
        listItem.Add(new List<ItemInfo>());
        if (listItem.Count == 1)
            listItem.Add(new List<ItemInfo>());

        return listItem;
    }
    // Start is called before the first frame update
    void Start()
    {
        //NavFooter.instance.onChangeState += DoCloseAllVfx;
        m_ListTopup.SetAdapter(Game.main.IAP.GetAllProduct());
        m_BackGround.sprite = CardData.Instance.GetSpriteShopUI("wood-bg-pattern-dark");
        m_BackGround.material.SetTexture("_MainTex", CardData.Instance.GetTextureShopUI("wood-bg-pattern-dark"));
    }

    private void OnDisable()
    {
        //NavFooter.instance.onChangeState -= DoCloseAllVfx;
    }

    public void maincontentFadein()
    {
        DOTween.Kill(m_slidebaroptionfrom.transform, true);
        DOTween.Kill(m_slidebaroptionto.transform, true);
        DOTween.Kill(m_SidetabOptions.transform, true);
        m_SidetabOptions.localScale = new Vector3(1, 0);
        m_slidebaroptionfrom.DOAnchorPos(new Vector2(-700, 0), 1f)
          .OnComplete(() =>
          m_SidetabOptions.DOScale(new Vector3(1, 1), 0.5f)
          .OnComplete(() =>
          {
              m_slidebaroptionfrom.gameObject.SetActive(false);
          })
          );
        //m_slidebaroption.DOAnchorPos(new Vector2(f, 0), 1.5f, false).SetEase(Ease.OutElastic);

        m_slidebaroptionto.gameObject.SetActive(true);
        m_slidebaroptionto.anchoredPosition = new Vector2(-700, 0);
        m_slidebaroptionto.DOAnchorPos(new Vector2(0, 0), 1f);
        m_MainContent.SetActive(false);
    }
    protected override void Awake()
    {
        base.Awake();
        instance = this;
        m_Pool.SetCellDataCallback((GameObject go, List<ItemInfo> data, int index) =>
        {
            UserTrayShop script = go.GetComponent<UserTrayShop>();
            script.SetData(data);
        });
        m_ListPackage.SetCellDataCallback((GameObject go, ItemPackage data, int index) =>
        {
            ShopOptionPack script = go.GetComponent<ShopOptionPack>();
            script.SetData(data);

            go.GetComponent<ButtonClickEvent>().SetOnClickCallback(() =>
            {
                foreach (ItemPackage ip in lstItemPackages)
                {
                    ip.isSelected = (ip.idx == data.idx);
                }
                m_ListPackage.ReloadDataToVisibleCell();
                m_PackDetailCost.text = data.price + "";
                m_PackDetailCurrency.Turn(data.currency == 1);
                currency = data.currency;
                itemPrice = data.price;
                packageIdx = data.idx;
            });
        });
        m_ListReward
            .SetCellPrefabCallback((index) =>
            {
                ItemReward data = m_ListReward.GetCellDataAt<ItemReward>(index);
                if (data.kind == 0)
                {
                    DBHero hero = Database.GetHero(data.id);
                    if (hero.type == DBHero.TYPE_GOD)
                    {
                        return m_ListReward.GetDeclarePrefab(1);
                    }
                }
                return m_ListReward.GetDeclarePrefab(0);
            })
            .SetCellDataCallback((GameObject go, ItemReward data, int index) =>
        {
            PackItemReward script = go.GetComponent<PackItemReward>();
            script.SetData(data);
        });
        m_ListReward.SetCellSizeCallback((index) =>
        {
            ItemReward data = m_ListReward.GetCellDataAt<ItemReward>(index);
            if (data.kind == 0)
            {
                DBHero hero = Database.GetHero(data.id);
                if (hero.type == DBHero.TYPE_GOD)
                {
                    return new Vector2(540, 113);
                }
            }
            return new Vector2(530, 90);
        });



        m_ListTopup.SetCellPrefabCallback((index) =>
        {
            IPurchaserProduct data = m_ListTopup.GetCellDataAt<IPurchaserProduct>(index);
            JSONNode jN = JSON.Parse(data.localizedDescription);
            JSONObject jO = jN.AsObject;
            if (jO != null)
            {
                bool isSpecial = jO["is_special"].AsInt == 1;
                if (isSpecial)
                {
                    return m_ListTopup.GetDeclarePrefab(1);
                }
            }
            return m_ListTopup.GetDeclarePrefab(0);
        }).SetCellDataCallback((GameObject go, IPurchaserProduct data, int index) =>
        {
            GemTopup script = go.GetComponent<GemTopup>();
            if (script != null)
            {
                script.SetData(data);
            }
        });

        //m_ListTopup.SetCellSizeCallback((index) =>
        //{
        //    IPurchaserProduct data = m_ListTopup.GetCellDataAt<IPurchaserProduct>(index);
        //    JSONNode jN = JSON.Parse(data.localizedDescription);
        //    JSONObject jO = jN.AsObject;
        //    if (jO != null)
        //    {
        //        bool isSpecial = jO["is_special"].AsInt == 1;
        //        if (isSpecial)
        //        {
        //            return new Vector2(325, 706);
        //        }
        //    }
        //    return new Vector2(305, 686);
        //});
    }

    // Update is called once per frame
    //void Update()
    //{

    //}
    public void DoClickButtonBack()
    {
        if (isShowPackDetail)
        {
            m_slidebaroptionfrom.DOAnchorPos(new Vector2(0, 0), 0.5f)
             .OnComplete(() => m_slidebaroptionfrom.gameObject.SetActive(true));
            m_slidebaroptionto.gameObject.SetActive(true);
            m_slidebaroptionto.anchoredPosition = new Vector2(0, 0);
            // m_slidebaroptionto.DOAnchorPos(new Vector2(0, 0), 0f);


            m_PackItemDetail.SetActive(false);
            m_MainContent.SetActive(true);
            isShowPackDetail = false;
        }
        else
        {
            // DoCloseAllVfx();
            Game.main.LoadScene("HomeSceneNew", delay: 0.3f, curtain: true);
        }
    }
    public void DoClickDisableButton()
    {
        Toast.Show(LangHandler.Get("toast-8", "Coming Soon!"));
    }
    public void DoClickBuy()
    {
        //if (isLoading)
        //    return;
        //if (shopItemId != -1 && countPackage != 0)
        //{

        //    //Hien thi loading
        //    popupLoading = PoolManager.Pools["PopupLoading"].Spawn(m_PoppupLoadingPrefab);
        //    popupLoading.SetParent(Game.main.canvas.panelPopup);
        //    popupLoading.localScale = Vector3.one;
        //    popupLoading.localPosition = Vector3.zero;
        //    popupLoading.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        //    timeCount = 5;
        //    isLoading = true;
        //    //5s thì hienr thi fail;

        //}
        //else
        //    Toast.Show("Choose one package to buy");
        SoundHandler.main.PlaySFX("900_click_3", "sounds");
        if (currency == 0)
        {
            if (GameData.main.userCurrency.gold >= itemPrice)
            {
                Game.main.socket.BuyItem(shopItemId, packageIdx);
            }
            else
            {
                Toast.Show(LangHandler.Get("165", "Not Enough Gold!"));
            }
        }
        else if (currency == 1)
        {
            if (GameData.main.userCurrency.gem >= itemPrice)
            {
                Game.main.socket.BuyItem(shopItemId, packageIdx);
            }
            else
            {
                Toast.Show(LangHandler.Get("166", "Not Enough Gem!"));
            }
        }
    }
    public void DoClickClosePopupPurchase()
    {
        SoundHandler.main.PlaySFX("900_click_3", "sounds");
        m_PopupPurchaseComplete.SetActive(false);
    }
    public void DoClickOpenPack()
    {
        SoundHandler.main.PlaySFX("900_click_3", "sounds");
        Game.main.socket.GetUserPack();
        Game.main.socket.GetUserDeck();
        //HandleNetData.QueueNetData(NetData.GO_TO_OPEN_PACK);
        Game.main.LoadScene("CollectionScene", onLoadSceneSuccess: () =>
        {
            CollectionScene.instance.InitDataContent(3);
        }, delay: 0.3f, curtain: true);
        NavFooter.instance.onChangeState?.Invoke();
        //NavFooter.instance.DoClickCollection();
    }
    public void DoClickBuyGem(int skuId)
    {
        Game.main.socket.RechargeGem(skuId);
    }
    public void LoadHttpAvatar(string url, ICallback.CallFunc2<Sprite> onLoaded = null)
    {
        if (coroutine != null)
            Game.main.StopCoroutine(coroutine);

        if (string.IsNullOrEmpty(url)) return;

        coroutine = IUtil.LoadTexture2DFromUrl(url, (Texture2D tex) =>
        {
            if (tex == null) return;
            Sprite sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            SetAvatarSprite(sprite);

            if (onLoaded != null) onLoaded(sprite);
        });
        Game.main.StartCoroutine(coroutine);
    }
    private void SetAvatarSprite(Sprite sprite)
    {
        m_PackDetailIcon.sprite = sprite;
    }
}
