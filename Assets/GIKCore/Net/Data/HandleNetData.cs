using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pbdson;
using GIKCore.DB;
using GIKCore.Lang;
using GIKCore.UI;
using System;
using GIKCore.Utilities;
using UnityEngine.Purchasing;

namespace GIKCore.Net
{
    public class HandleNetData
    {
        public static void HandleQueue()
        {
            // process net data 
            while (lstQueueNetData.Count > 0)
            {
                try
                {
                    NetData nd = lstQueueNetData[0];
                    ProcessNetData(nd.id, nd.data);
                }
                catch (System.Exception e)
                {
                    Game.main.netBlock.Hide();//force hide
                    LogWriterHandle.WriteLogError(e.StackTrace);
                    LogWriterHandle.WriteLog("Net Stack Trace ::: " + lstQueueNetData[0].id);
                }
                lstQueueNetData.RemoveAt(0);
            }
            // process http response data
            while (lstQueueHttpResponseData.Count > 0)
            {
                try
                {
                    HttpResponseData hrd = lstQueueHttpResponseData[0];
                    ProcessHttpResponseData(hrd.id, hrd.data);
                }
                catch (System.Exception e)
                {
                    Game.main.netBlock.Hide();//force hide
                    LogWriterHandle.WriteLogError(e.StackTrace);
                    LogWriterHandle.WriteLog("HTTP Stack Trace ::: " + lstQueueHttpResponseData[0].id);
                }
                lstQueueHttpResponseData.RemoveAt(0);
            }

            //if (!Util.IsActiveSceneSameAs("SplashScene"))
            //{
            // process socket data
            while (lstQueueSocketData.Count > 0)
            {
                try
                {
                    SocketData sd = lstQueueSocketData[0];
                    ProcessSocketData(sd.id, sd.data);
                }
                catch (System.Exception e)
                {
                    Game.main.netBlock.Hide();//force hide
                    LogWriterHandle.WriteLogError(e.StackTrace);
                    LogWriterHandle.WriteLog("Socket Stack Trace ::: " + lstQueueSocketData[0].id);
                }
                lstQueueSocketData.RemoveAt(0);
            }
            //}
        }

        // --------------------------------------------------
        // -------------------- NET DATA --------------------
        // --------------------------------------------------
        private static List<NetData> lstQueueNetData = new List<NetData>();
        public static void QueueNetData(int id, object data = null)
        {
            lstQueueNetData.Add(new NetData(id, data));
        }
        public static void SendNetDataToListener(int id, object data)
        {
            for (int i = Game.main.listeners.Count - 1; i >= 0; i--)
            {
                GameListener tmp = Game.main.listeners[i];
                if (tmp != null && tmp.isActiveAndEnabled)
                    tmp.ProcessNetData(id, data);
            }
        }
        private static void ProcessNetData(int id, object data)
        {
            switch (id)
            {
                case NetData.SOCKET_CONNECT_SUCCESS:
                    {
                        Game.main.socket.tryConnect = false;
                        Game.main.netBehavior.InternetConnected();

                        GamePrefs.isLoggedIn = false;
                        Game.main.SetActiveBlockSocket(false);
                        PopupLogin.OAuthToken();

                        SendNetDataToListener(id, data);
                        break;
                    }
                case NetData.SOCKET_ERROR:
                    {
                        if (Game.main.socket.LimitCount())
                        {
                            Game.main.socket.tryConnect = false;

                            string msg = LangHandler.Get("host-1", "Không thể kết nối tới máy chủ đích") + ".";
                            if (!Game.main.netBehavior.IsConnected())
                                msg += "\n" + LangHandler.Get("host-3", "Không có internet. Vui lòng kiểm tra Wi-Fi hoặc dữ liệu di động của bạn");
                            string action1 = "";
#if (UNITY_ANDROID || UNITY_IOS)
                            action1 = LangHandler.Get("confirm-8", "Thoát game");
#endif
                            PopupConfirm.Show(content: msg, action1: action1, action2: LangHandler.Get("confirm-3", "Thử lại"),
                                action1Callback: go => { Application.Quit(); },
                                action2Callback: go => { Game.main.socket.tryConnect = !Game.main.socket.CheckSocketConnected(); }, parent: Game.main.netBehavior.panelBlock);
                        }
                        else Game.main.socket.tryConnect = true;
                        break;
                    }
                case NetData.SOCKET_DISCONNECT_READ:
                    {
                        Game.main.socket.Connect();
                        break;
                    }
                case NetData.SOCKET_SERVER_NULL:
                    {
                        string action1 = "";
#if (UNITY_ANDROID || UNITY_IOS)
                        action1 = LangHandler.Get("confirm-8", "Thoát game");
#endif
                        PopupConfirm.Show(LangHandler.Get("host-2", "Không thể phân giải máy chủ đích"),
                            action1,
                            action1Callback: go => { Application.Quit(); },
                            parent: Game.main.netBehavior.panelBlock);
                        break;
                    }
                case NetData.IAP_PURCHASE_SUCCESS:
                    {
                        string[] tmp = (string[])data;
                        string productId = tmp[0];
                        string token = tmp[1];
                        Game.main.socket.GetVerifyIAP(productId, Constants.PKN, token);
                        break;
                    }
                default:
                    {
                        SendNetDataToListener(id, data);
                        break;
                    }
            }
        }

        // --------------------------------------------------
        // --------------- HTTP RESPONSE DATA ---------------
        // -------------------------------------------------- 
        private static List<HttpResponseData> lstQueueHttpResponseData = new List<HttpResponseData>();
        public static void QueueHttpResponseData(int id, string data)
        {
            lstQueueHttpResponseData.Add(new HttpResponseData(id, data));
        }
        private static void SendHttpResponseDataToListener(int id, string data)
        {
            for (int i = Game.main.listeners.Count - 1; i >= 0; i--)
            {
                GameListener tmp = Game.main.listeners[i];
                if (tmp != null && tmp.isActiveAndEnabled)
                    tmp.ProcessHttpResponseData(id, data);
            }
        }
        private static void ProcessHttpResponseData(int id, string data)
        {
            Game.main.netBlock.Hide();
            switch (id)
            {
                default:
                    {
                        SendHttpResponseDataToListener(id, data);
                        break;
                    }
            }
        }

        // --------------------------------------------------
        // -------------- SOCKET PACKAGE DATA ---------------
        // --------------------------------------------------   
        private static List<SocketData> lstQueueSocketData = new List<SocketData>();
        public static void QueueSocketData(int id, byte[] data)
        {
            lstQueueSocketData.Add(new SocketData(id, data));
        }
        private static void SendSocketDataToListener(int id, byte[] data)
        {
            for (int i = Game.main.listeners.Count - 1; i >= 0; i--)
            {
                GameListener tmp = Game.main.listeners[i];
                if (tmp != null && tmp.isActiveAndEnabled)
                    tmp.ProcessSocketData(id, data);
            }
        }
        private static void ProcessSocketData(int id, byte[] data)
        {
            if (IService.NeedHideLoading(id))
                Game.main.netBlock.Hide();

            switch (id)
            {
                case IService.ERROR:
                    {
                        CommonVector cv = ISocket.Parse<CommonVector>(data);
                        LogWriterHandle.WriteLog(cv.aString[0]);
                        Toast.Show(cv.aString[0]);
                        break;
                    }
                case IService.PONG:
                    {
                        Game.main.socket.SendPong();
                        break;
                    }

                case IService.LOGIN:
                    {
                        CommonVector cv = ISocket.Parse<CommonVector>(data);
                        if (cv.aString == null || cv.aString.Count <= 0)
                        {
                            long seedCode = GameData.main.seedCode = cv.aLong[0];
                            Game.main.socket.TransmitAuth(seedCode);

                            if (Config.allowLog) LogWriterHandle.WriteLog("-------- Login with seed code: " + seedCode);
                        }
                        else
                        {// login ok
                            LogWriterHandle.WriteLog("LoginOk->SetLoginData");
                            GameData.main.SetLoginData(cv);
                            Debug.Log("Get user progression________" + GameData.main.userProgressionState);
                            SendSocketDataToListener(id, data);
                        }
                        break;
                    }
                case IService.LOGIN_FAIL:
                    {
                        CommonVector cv = ISocket.Parse<CommonVector>(data);
                        string msg = cv.aString[0];
                        if (!string.IsNullOrEmpty(msg))
                            PopupConfirm.Show(content: msg,
                                action1Callback: go =>
                                {
                                    Game.main.socket.Logout();
                                    if (ProgressionController.instance != null)
                                    {
                                        Destroy(ProgressionController.instance.gameObject);
                                        ProgressionController.instance = null;
                                    }
                                    Game.main.LoadScene("HomeSceneNew");

                                });
                        break;
                    }
                case IService.DUPLICATE_LOGIN:
                    {
                        GameData.main.isX2Login = true;
                        GamePrefs.Logout();
                        Game.main.LoadScene("HomeSceneNew");
                        break;
                    }
                case IService.GET_USER_HERO_CARD:
                    {
                        CommonVector cv = ISocket.Parse<CommonVector>(data);

                        LogWriterHandle.WriteLog(string.Join(",", cv.aLong));

                        GameData.main.lstHeroCard.Clear();
                        for (int i = 0; i < cv.aLong.Count; i += 3)
                        {
                            HeroCard hc = new HeroCard();
                            hc.id = cv.aLong[i];
                            hc.heroId = cv.aLong[i + 1];
                            hc.frame = cv.aLong[i + 2];
                            if (hc.id < 0)
                            {
                                hc.cardType = HeroCard.CardType.nft;
                            }
                            else
                                hc.cardType = HeroCard.CardType.normal;
                            GameData.main.lstHeroCard.Add(hc);
                        }

                        SendSocketDataToListener(id, data);
                        break;
                    }
                case IService.GET_USER_BATTLE_DECK:
                    {
                        ListCommonVector lcv = ISocket.Parse<ListCommonVector>(data);
                        GameData.main.battleDeck.Parse(lcv);

                        SendSocketDataToListener(id, data);
                        break;
                    }
                case IService.SET_USER_BATTLE_DECK:
                    {
                        CommonVector cv = ISocket.Parse<CommonVector>(data);
                        long status = cv.aLong[0];
                        //string msg = cv.aString[0];
                        LogWriterHandle.WriteLog("SET_USER_BATTLE_DECK==" + cv.aLong[0]);
                        LogWriterHandle.WriteLog(string.Join(",", cv.aString));

                        if (status == 1)
                        {
                            SendSocketDataToListener(id, data);
                        }
                        else
                        {
                            Toast.Show(cv.aString[0]);
                        }
                        //else if (!string.IsNullOrEmpty(msg)) Toast.Show(msg);
                        break;
                    }
                case IService.GET_SHOP:
                    {
                        ListCommonVector lcv = ISocket.Parse<ListCommonVector>(data);
                        CommonVector cv = lcv.aVector[0];
                        for (int i = 0; i < lcv.aVector.Count; i++)
                        {
                            Debug.Log(string.Join(",", lcv.aVector[i].aLong));
                            Debug.Log(string.Join(",", lcv.aVector[i].aString));
                        }
                        if (!GameData.main.isFirstShop)
                            GameData.main.isFirstShop = cv.aLong[0] == 0;
                        int START_LONG = 1;
                        int BLOCK_LONG = 9;
                        int COUNT_LONG = (cv.aLong.Count - START_LONG) / BLOCK_LONG;

                        int START_STRING = 0;
                        int BLOCK_STRING = 3;
                        GameData.main.lstItemShopPack.Clear();
                        GameData.main.lstItemShopValuable.Clear();
                        for (int i = 0; i < COUNT_LONG; i++)
                        {
                            ItemInfo item = new ItemInfo();
                            item.shopItemId = (int)cv.aLong[START_LONG + i * BLOCK_LONG + 0];
                            item.itemId = (int)cv.aLong[START_LONG + i * BLOCK_LONG + 1];
                            item.startOffset = cv.aLong[START_LONG + i * BLOCK_LONG + 2];
                            item.endOffset = cv.aLong[START_LONG + i * BLOCK_LONG + 3];
                            item.isDiscount = cv.aLong[START_LONG + i * BLOCK_LONG + 4] == 1;
                            item.currency = (int)cv.aLong[START_LONG + i * BLOCK_LONG + 5];
                            item.price = (int)cv.aLong[START_LONG + i * BLOCK_LONG + 6];
                            item.percent = (int)cv.aLong[START_LONG + i * BLOCK_LONG + 7];
                            item.type = (int)cv.aLong[START_LONG + i * BLOCK_LONG + 8];

                            item.name = cv.aString[START_STRING + i * BLOCK_STRING + 0];
                            item.desc = cv.aString[START_STRING + i * BLOCK_STRING + 1];
                            item.image = cv.aString[START_STRING + i * BLOCK_STRING + 2];
                            if (item.type == TypeItems.TYPE_PACK_SHOP)
                            {
                                GameData.main.lstItemShopPack.Add(item);
                            }
                            else if (item.type == TypeItems.TYPE_VALUABLE_SHOP)
                            {
                                GameData.main.lstItemShopValuable.Add(item);
                            }

                            // set default item 0
                        }
                        HandleNetData.QueueNetData(NetData.RECEIVE_SHOP_PACK_DATA);
                        HandleNetData.QueueNetData(NetData.RECEIVE_SHOP_VALUE_DATA);

                        break;
                    }
                case IService.GET_MAP:
                    {
                        CommonVector cv = ISocket.Parse<CommonVector>(data);
                        GameData.main.listHomeRewardItemProps.Clear();
                        int BLOCK_STRING = 2;
                        int START_STRING = 0;
                        int COUNT_BLOCK_STRING = cv.aString.Count / BLOCK_STRING;
                        int start = 0;
                        int end = 0;
                        for (int i = 0; i < cv.aLong.Count; i++)
                        {
                            if (cv.aLong[i] == -1)
                            {
                                end = i;
                                HomeRewardItemProps props = new HomeRewardItemProps();
                                props.id = (int)cv.aLong[start + 0];
                                props.state = (int)cv.aLong[start + 1];
                                props.matchAlwaysCoin = (int)cv.aLong[start + 2];
                                props.matchAlwaysCup = (int)cv.aLong[start + 3];
                                props.matchAlwaysEssence = (int)cv.aLong[start + 4];
                                props.matchTimeChestCoin = (int)cv.aLong[start + 5];
                                props.matchTimeChestCup = (int)cv.aLong[start + 6];
                                props.matchTimeChestEssence = (int)cv.aLong[start + 7];
                                for (int j = start + 8; j < end; j++)
                                {
                                    props.lstHeroCardIds.Add((int)cv.aLong[j]);
                                    props.hasCardUnlock = props.lstHeroCardIds.Count > 0;
                                }
                                if (GameData.main.listHomeRewardItemProps.Find(x => x.id == props.id) == null)
                                    GameData.main.listHomeRewardItemProps.Add(props);
                                start = i + 1;
                            }
                        }
                        for (int i = START_STRING; i < COUNT_BLOCK_STRING; i++)
                        {
                            GameData.main.listHomeRewardItemProps[i].itemUrl = cv.aString[i * BLOCK_STRING];
                            GameData.main.listHomeRewardItemProps[i].desc = cv.aString[i * BLOCK_STRING + 1];
                            IUtil.SetMapRewardSprite(cv.aString[i * BLOCK_STRING]);
                        }

                        if (GameData.main.listHomeRewardItemProps.Count > 0)
                            HandleNetData.QueueNetData(NetData.HOME_LOAD_MAP_LEVEL);
                        break;
                    }
                default:
                    {
                        SendSocketDataToListener(id, data);
                        break;
                    }
            }
        }

        private static void Destroy(GameObject gameObject)
        {
            throw new NotImplementedException();
        }

        // --------------------------------------------------
        // ---------------------- GAME ----------------------
        // --------------------------------------------------        
    }
}