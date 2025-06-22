using GIKCore.Utilities;
using GIKCore;
using System.Collections;
using System.Collections.Generic;
using UIEngine.UIPool;
using UnityEngine;
using GIKCore.DB;
using GIKCore.Net;
using pbdson;

public class EventSliderContainer : GameListener
{
    // Fields
    [SerializeField] private RecycleLayoutGroup m_ListEvent;
    // Values
    private string currentLink = "";

    // Methods
    public void SetData(List<SystemEvent> lstData)
    {
        m_ListEvent.SetCellDataCallback((GameObject go, SystemEvent data, int idx) =>
        {
            EventSlider script = go.GetComponent<EventSlider>();
            script.Setdata(data).SetOnClckCB(() =>
            {
                if (data.position == 0)
                {
                    currentLink = data.link;
                    Game.main.socket.GetAccessTokenWebview();
                }
                else
                {
                    DoOpenScene(data.position);
                }
            });
        });

        m_ListEvent.SetAdapter(lstData);
    }
    public override bool ProcessSocketData(int serviceId, byte[] data)
    {
        if (base.ProcessSocketData(serviceId, data)) return true;
        switch (serviceId)
        {
            case IService.GET_ACCESSTOKEN:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    IUtil.OpenURL(currentLink + "?token=" + cv.aString[0]);
                    //Application.OpenURL(currentLink + "?access_token=" + cv.aString[0]);
                    break;
                }
        }
        return false;
    }
    public void DoOpenScene(int position)
    {
        switch (position)
        {
            case OpenScene.HOME_SCENE:
                {
                    Game.main.LoadScene("HomeSceneNew", delay: 0.3f, curtain: true);
                    break;
                }
            case OpenScene.SELECT_MODE_SCENE:
                {
                    Game.main.socket.GetMode();
                    Game.main.LoadScene("SelectModeScene", delay: 0.3f, curtain: true);
                    break;
                }
            case OpenScene.SELECT_MODE_SCENE_REWARDS:
                {
                    Game.main.socket.GetMode();
                    HandleNetData.QueueNetData(NetData.ACTION_OPEN_SCENE, OpenScene.SELECT_MODE_SCENE_REWARDS);
                    Game.main.LoadScene("SelectModeScene", delay: 0.3f, curtain: true);
                    break;
                }
            case OpenScene.SELECT_MODE_SCENE_LEADERBOARD:
                {
                    Game.main.socket.GetMode();
                    HandleNetData.QueueNetData(NetData.ACTION_OPEN_SCENE, OpenScene.SELECT_MODE_SCENE_REWARDS);
                    Game.main.LoadScene("SelectModeScene", delay: 0.3f, curtain: true);
                    break;
                }
            case OpenScene.SELECT_DECK_SCENE_NORMAL:
                {
                    Game.main.socket.GetUserDeck();
                    Game.main.socket.GetRank();
                    HandleNetData.QueueNetData(NetData.ACTION_OPEN_SCENE, OpenScene.SELECT_DECK_SCENE_NORMAL);
                    Game.main.LoadScene("SelectDeckScene", delay: 0.3f, curtain: true);
                    break;
                }
            case OpenScene.SELECT_DECK_SCENE_RANK:
                {
                    break;
                }
            case OpenScene.COLLECTION_SCENE_DECKS:
                {
                    Game.main.socket.GetUserPack();
                    HandleNetData.QueueNetData(NetData.ACTION_OPEN_SCENE, OpenScene.COLLECTION_SCENE_DECKS);
                    Game.main.LoadScene("CollectionScene", delay: 0.3f, curtain: true);
                    break;
                }
            case OpenScene.COLLECTION_SCENE_PACKS:
                {
                    Game.main.socket.GetUserPack();
                    HandleNetData.QueueNetData(NetData.ACTION_OPEN_SCENE, OpenScene.COLLECTION_SCENE_PACKS);
                    Game.main.LoadScene("CollectionScene", delay: 0.3f, curtain: true);
                    break;
                }
            case OpenScene.COLLECTION_SCENE_VALUABLES:
                {
                    Game.main.socket.GetUserPack();
                    HandleNetData.QueueNetData(NetData.ACTION_OPEN_SCENE, OpenScene.COLLECTION_SCENE_VALUABLES);
                    Game.main.LoadScene("CollectionScene", delay: 0.3f, curtain: true);
                    break;
                }
            case OpenScene.COLLECTION_SCENE_COSMETICS:
                {
                    Game.main.socket.GetUserPack();
                    HandleNetData.QueueNetData(NetData.ACTION_OPEN_SCENE, OpenScene.COLLECTION_SCENE_COSMETICS);
                    Game.main.LoadScene("CollectionScene", delay: 0.3f, curtain: true);
                    break;
                }
            case OpenScene.QUEST_SCENE:
                {
                    Game.main.socket.GetQuests();
                    Game.main.LoadScene("QuestScene", delay: 0.3f, curtain: true);
                    break;
                }
            case OpenScene.SHOP_SCENE_PACKS:
                {
                    Game.main.socket.GetShopItem();
                    HandleNetData.QueueNetData(NetData.ACTION_OPEN_SCENE, OpenScene.SHOP_SCENE_PACKS);
                    Game.main.LoadScene("ShopScene", delay: 0.3f, curtain: true);
                    break;
                }
            case OpenScene.SHOP_SCENE_GEM_TOPUPS:
                {
                    Game.main.socket.GetShopItem();
                    HandleNetData.QueueNetData(NetData.ACTION_OPEN_SCENE, OpenScene.SHOP_SCENE_GEM_TOPUPS);
                    Game.main.LoadScene("ShopScene", delay: 0.3f, curtain: true);
                    break;
                }
            case OpenScene.SHOP_SCENE_VALUABLES:
                {
                    Game.main.socket.GetShopItem();
                    HandleNetData.QueueNetData(NetData.ACTION_OPEN_SCENE, OpenScene.SHOP_SCENE_VALUABLES);
                    Game.main.LoadScene("ShopScene", delay: 0.3f, curtain: true);
                    break;
                }
            case OpenScene.SHOP_SCENE_COSMETICS:
                {
                    Game.main.socket.GetShopItem();
                    HandleNetData.QueueNetData(NetData.ACTION_OPEN_SCENE, OpenScene.SHOP_SCENE_COSMETICS);
                    Game.main.LoadScene("ShopScene", delay: 0.3f, curtain: true);
                    break;
                }
        }
    }


    // Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
