using GIKCore;
using GIKCore.Lang;
using GIKCore.Net;
using GIKCore.UI;
using PathologicalGames;
using pbdson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PackShopController : GameListener
{
    [SerializeField] private Transform contentListPack,poppupLoadingPrefab;
    [SerializeField] private GameObject packShopPrefab;
    [SerializeField] private GameObject packDetailUI,packListUI, butBackShop, packageOptions;
    [SerializeField] private Animator animTab;
    Transform popupLoading;
    bool isLoading= false;
    float timeCount = 5f;
    public static PackShopController instance;
    public long selectedPackID = -1;
    private List<ItemInfo> packList = new List<ItemInfo>();
    bool canSelect = true;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }

    void Start()
    {
        packListUI.SetActive(true);
        packDetailUI.SetActive(false);
        foreach(ItemInfo item in GameData.main.lstItemShopPack)
        {
            if(item.type == 60)
            {
                packList.Add(item);
            }
        }
        foreach(ItemInfo pack in packList)
        {
            GameObject packToBuy = Instantiate(packShopPrefab, contentListPack);
            PackShop script = packToBuy.GetComponent<PackShop>();
            script.InitData(pack);
            script.SetOnClickCallback(pack =>
            {
                // gui goi get package
                if(canSelect)
                {
                   canSelect = false;
                   Game.main.socket.GetPackDetail(pack.itemId);
                }
               // packageOptions.GetComponent<PackagesOptionController>().UpdateListPackage(pack.lstPacKBuy);
               
            });

        }
    }
    public void DoClickBackOnPackDetail()
    {
        animTab.SetBool("open", true);
        packDetailUI.SetActive(false);
        packListUI.SetActive(true);
        butBackShop.SetActive(false);
    }
    public void DoClickBuy()
    {
        if (isLoading)
            return;
        if (selectedPackID != -1)
        {
            //Game.main.socket.BuyItem(selectedPackID);
            
            //Hien thi loading
            popupLoading = PoolManager.Pools["PopupLoading"].Spawn(poppupLoadingPrefab);
            popupLoading.SetParent(Game.main.canvas.panelPopup);
            popupLoading.localScale = Vector3.one;
            popupLoading.localPosition = Vector3.zero;
            popupLoading.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            timeCount = 5;
            isLoading = true;
            //5s thì hienr thi fail;

        }
        else
            Toast.Show(LangHandler.Get("164","Choose one package to buy"));
    }
    private void Update()
    {
        if(isLoading)
        {
            if(timeCount>0)
            {
                timeCount--;
            }
            else
            {
                timeCount = 5;
                Toast.Show(LangHandler.Get("163","Unstable network transmission line"));
                PoolManager.Pools["PopupLoading"].DespawnAll();
                isLoading = false;
            }
        }    
        
    }
    
    public void GoToPackDetail(PackDetail packDetail)
    {
        ItemInfo item = GameData.main.lstItemShopPack.FirstOrDefault(x => x== packDetail.itemInfo);
        if (item != null)
        {
            if (animTab.GetCurrentAnimatorStateInfo(0).IsName("Open"))
                animTab.SetBool("open", false);
            else
                animTab.SetBool("close", true);
            packListUI.SetActive(false);
            packDetailUI.SetActive(true);
            packDetailUI.GetComponent<ShowPackDetail>().Init(packDetail);
            packageOptions.GetComponent<PackagesOptionController>().UpdateListPackage(packDetail.lstPackage);
            butBackShop.SetActive(true);
            canSelect = true;
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
                    ListCommonVector lst = ISocket.Parse<ListCommonVector>(data);
                    foreach (CommonVector cv in lst.aVector)
                        LogWriterHandle.WriteLog("GET_ITEM_DETAIL: \nALong: " + string.Join(",", cv.aLong) + "\nAString: " + string.Join(",", cv.aString));
                    PackDetail packDetail = new PackDetail();
                    CommonVector cv1 = lst.aVector[0];
                    if (cv1.aLong[0] == 0)
                    {
                        Toast.Show(cv1.aString[0]);
                        LogWriterHandle.WriteLog(cv1.aString[0]);
                    }
                    //else
                    //{
                    //    if (cv1.aLong.Count > 0)
                    //    {
                    //        long itemId = cv1.aLong[0];
                    //        var item = GameData.main.lstItemShopPack.FirstOrDefault(x => x.itemId == itemId);
                    //        packDetail.itemInfo = item;
                    //        for (int i = 1; i < cv1.aLong.Count; i += 4)
                    //        {
                    //            PackagesBuy packages = new PackagesBuy()
                    //            {

                    //                packageBuyId = cv1.aLong[i],
                    //                countItem = cv1.aLong[i + 1],
                    //                currency = cv1.aLong[i + 2],
                    //                price = cv1.aLong[i + 3]
                    //            };
                    //            packDetail.lstPackage.Add(packages);
                    //        }
                    //    }
                    //}
                    //CommonVector cv2 = lst.aVector[1];
                    //if (cv2.aLong.Count > 0)
                    //{
                    //    List<long> heroIds = new List<long>();
                    //    for (int i = 0; i < cv2.aLong.Count; i++)
                    //    {
                    //        packDetail.heroIds.Add(cv2.aLong[i]);
                    //    }
                    //}
                    //GoToPackDetail(packDetail);
                    break;
                }
            case IService.BUY_ITEM:
                {
                    PoolManager.Pools["PopupLoading"].DespawnAll();
                    isLoading = false;
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    LogWriterHandle.WriteLog("BUY_ITEM: \nALong: " + string.Join(",", cv.aLong) + "\nAString: " + string.Join(",", cv.aString));
                    if (cv.aLong[0] == 0)
                    {
                        Toast.Show(cv.aString[0]);
                    }
                    else
                    {
                        //PopupConfirm.Show(content: LangHandler.Get("toast-28", "DUPLICATE LOG IN."));
                        PopupConfirm.Show(content: "PURCHASE COMPLETE!");
                    }
                    break;
                }
        }
        return false;
    }
}
