using GIKCore;
using GIKCore.Net;
using PathologicalGames;
using pbdson;
using System.Collections;
using System.Collections.Generic;
using UIEngine.UIPool;
using UnityEngine;

public class UserPackController : GameListener
{
    [SerializeField] private VerticalPoolGroup poolLstPack;

    [SerializeField] private Transform rewardPopupPrefab;
    [SerializeField] private GameObject NoChestText;
    private List<PackInfo> packList = new List<PackInfo>();
    public static UserPackController main;
    public bool isDrop;
    protected override void Awake()
    {
        base.Awake();
        main = this;
        poolLstPack.SetCellDataCallback((GameObject go, PackInfo data, int index) =>
        {
            UserPack script = go.GetComponent<UserPack>();
            script.SetOnEndDragCallback(pack =>
            {
                if (isDrop)
                {
                    //H??ng b?o là t?m th?i fix c?ng ch? này 
                    Game.main.socket.OpenUserPack(0);
                    //Game.main.socket.OpenUserPack(pack.packId);
                    isDrop = false;
                }
            });
            script.InitData(data);
        });
    }

    public void ClickGetPack()
    {
        Game.main.LoadScene("ShopScene", delay: 0.3f, curtain: true);
    }
    private void OnEnable()
    {
        UpdateListPack();
    }
    private void UpdateListPack()
    {
        packList.Clear();
        //packList.AddRange(GameData.main.lstUserPack);
        packList.AddRange(GameData.main.lstUserPack);
        if (packList.Count == 0)
        {
            NoChestText.SetActive(true);
        }
        else
        {
            NoChestText.SetActive(false);
            poolLstPack.SetAdapter(packList);
        }
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    public override bool ProcessSocketData(int serviceId, byte[] data)
    {
        if (base.ProcessSocketData(serviceId, data)) return true;
        switch (serviceId)
        {
            case IService.GET_USER_ITEMS:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    LogWriterHandle.WriteLog("GET_LIST_PACK: \nALong: " + string.Join(",", cv.aLong) + "\nAString: " + string.Join(",", cv.aString));
                    GameData.main.lstUserPack = new List<PackInfo>();
                    for (int i = 0; i < cv.aLong.Count; i += 2)
                    {
                        PackInfo info = new PackInfo()
                        {
                            packId = cv.aLong[i],
                            packName = cv.aString[i / 2],
                            quantity = cv.aLong[i + 1]
                        };
                        GameData.main.lstUserPack.Add(info);
                    }
                    UpdateListPack();
                    break;
                }
            case IService.OPEN_CHEST:
                {
                    ListCommonVector lst = ISocket.Parse<ListCommonVector>(data);
                    foreach (CommonVector cv in lst.aVector)
                    {
                        LogWriterHandle.WriteLog("OPEN_PACK: \nALong: " + string.Join(",", cv.aLong) + "\nAString: " + string.Join(",", cv.aString));
                    }
                    RewardUIModel model = new RewardUIModel();

                    CommonVector cv1 = lst.aVector[0];
                    if (cv1.aLong[0] == 0)
                    {
                        PopupConfirm.Show(content: cv1.aString[0]);
                    }
                    else
                    {
                        if (cv1.aLong.Count >= 3)
                        {
                            for (int i = 0; i < cv1.aLong.Count; i += 3)
                            {
                                HeroCard card = new HeroCard();
                                card.id = cv1.aLong[i];
                                card.heroId = cv1.aLong[i + 1];
                                card.frame = cv1.aLong[i + 2];
                                model.lstCard.Add(card);
                            }
                        }
                        CommonVector cv2 = lst.aVector[1];
                        model.goldNumber = cv2.aLong[0];
                        Transform trans = PoolManager.Pools["RewardPopup"].Spawn(rewardPopupPrefab);
                        trans.SetParent(Game.main.canvas.panelPopup);
                        trans.localScale = Vector3.one;
                        trans.localPosition = Vector3.zero;
                        trans.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
                        trans.gameObject.GetComponent<RewardSceneUI>().InitData(model);
                        Game.main.socket.GetUserPack();
                    }
                    break;
                }
        }
        return false;
    }
}
