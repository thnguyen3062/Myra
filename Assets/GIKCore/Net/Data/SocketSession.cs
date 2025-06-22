using System.Collections.Generic;
using UnityEngine;
using GIKCore.DB;
using pbdson;
using GIKCore.Utilities;
using GIKCore.Lang;

namespace GIKCore.Net
{
    public class SocketSession
    {
        // Values
        public bool tryConnect = false;
        public bool allowConnect = false;

        private ISocket socket = null;

        // Methods    
        public SocketSession()
        {
            socket = new ISocket();
        }

        public void Connect()
        {
            if (!allowConnect) return;

            ServerInfo sv = Config.GetServer();
            if (sv == null)
            {
                DisconnectSocket("::: Socket Server Null");
                HandleNetData.QueueNetData(NetData.SOCKET_SERVER_NULL, null);
                return;
            }

            Game.main.SetActiveBlockSocket(true);
            socket.Connect(sv.host, sv.port);
        }
        public void DisconnectSocket(string log = "") { socket.DisconnectSocket(log); }
        public bool CheckSocketConnected() { return socket.CheckSocketConnected(); }
        public bool LimitCount(bool reset = true) { return socket.LimitCount(reset); }

        public void SendProto(int id, object proto = null) { socket.SendProto(id, proto); }

        // ----------------------------------------------------------------------
        // ----------------------------------------------------------------------
        // --------------------- DEFINE SEND FUNCTION BELOW ---------------------
        // ----------------------------------------------------------------------
        // ----------------------------------------------------------------------

        public void SendPong() { SendProto(IService.PONG); }
        public void MultiLanguage(int type)
        {
            CommonVector proto = new CommonVector();
            proto.aLong.Add(type);
            SendProto(IService.MULTI_LANGUAGE, proto);
        }

        public void Logout()
        {
            GamePrefs.Logout();
            SendProto(IService.LOGOUT);
        }
        public void CheckVersion(string ver)
        {
            CommonVector commonVector = new CommonVector();
            commonVector.aString.Add(ver);
            SendProto(IService.CHECK_VERSION, commonVector);
        }
        public void NewbieEvent()
        {
            SendProto(IService.NEWBIE_EVENT);
        }
        public void Register(string username, string password, string email)
        {
            CommonVector proto = new CommonVector();
            proto.aString.Add(username);
            proto.aString.Add(password);
            proto.aString.Add(Constants.VERSION);
            proto.aString.Add(Constants.OS_TYPE);
            proto.aString.Add(Constants.CP_CODE);
            proto.aString.Add(Constants.DEVICE_ID);
            proto.aString.Add(Constants.CLIENT_IP);
            proto.aString.Add(email);

            SendProto(IService.REGISTER, proto);
        }
        public void TransmitAuth(long seedCode)
        {
            string pass = ICrypto.EncryptNewPwd(GameData.main.loginPassword, "" + seedCode);

            CommonVector authRq = new CommonVector();
            authRq.aString.Add(GameData.main.loginUsername);
            authRq.aString.Add(pass);
            authRq.aString.Add(Constants.VERSION);
            authRq.aString.Add(Constants.PLATFORM);
            authRq.aString.Add(Constants.OS_TYPE);
            authRq.aString.Add(Constants.CP_CODE);
            authRq.aString.Add(Constants.DEVICE_ID);//imei            
            authRq.aString.Add(Constants.CLIENT_IP);
            authRq.aString.Add(Constants.CLIENT_MCC);

            SendProto(IService.LOGIN, authRq);
        }

        public void LoginBlockchain(string accessToken, string refreshToken)
        {
            //string pass = ICrypto.EncryptNewPwd(GameData.main.loginPassword, "" + seedCode);

            CommonVector authRq = new CommonVector();
            authRq.aLong.Add(GamePrefs.LastLang);
            // authRq.aLong.Add(LangHandler.lastType);
            authRq.aString.Add(accessToken); // asset token
            authRq.aString.Add(refreshToken); // refresh token
            authRq.aString.Add(Constants.VERSION);
            authRq.aString.Add(Constants.PLATFORM);
            authRq.aString.Add(Constants.OS_TYPE);
            authRq.aString.Add(Constants.CP_CODE);
            authRq.aString.Add(Constants.DEVICE_ID);//imei            
            authRq.aString.Add(Constants.CLIENT_IP);
            authRq.aString.Add(Constants.CLIENT_MCC);

            SendProto(IService.LOGIN_WEB, authRq);
        }

        public void AutoLogin(string username)
        {
            CommonVector authRq = new CommonVector();
            authRq.aLong.Add(GamePrefs.LastLang);
            //authRq.aLong.Add(LangHandler.lastType);
            authRq.aString.Add(username);
            authRq.aString.Add(Constants.VERSION);
            authRq.aString.Add(Constants.PLATFORM);
            authRq.aString.Add(Constants.OS_TYPE);
            authRq.aString.Add(Constants.CP_CODE);
            authRq.aString.Add(Constants.DEVICE_ID);//imei            
            authRq.aString.Add(Constants.CLIENT_IP);
            authRq.aString.Add(Constants.CLIENT_MCC);

            SendProto(IService.AUTO_LOGIN, authRq);
        }

        public void LoginFacebook(string token, long type)
        {
            // 0 facebook, 1 google
            CommonVector authRq = new CommonVector();
            authRq.aLong.Add(type);
            authRq.aLong.Add(GamePrefs.LastLang);
            //authRq.aLong.Add(LangHandler.lastType);
            authRq.aString.Add(token);
            authRq.aString.Add(Constants.VERSION);
            authRq.aString.Add(Constants.PLATFORM);
            authRq.aString.Add(Constants.OS_TYPE);
            authRq.aString.Add(Constants.CP_CODE);
            authRq.aString.Add(Constants.DEVICE_ID);//imei            
            authRq.aString.Add(Constants.CLIENT_IP);
            authRq.aString.Add(Constants.CLIENT_MCC);

            SendProto(IService.LOGIN_3RD, authRq);
        }
        public void LoginGoogle(string token, long type)
        {
            // 0 facebook, 1 google
            CommonVector authRq = new CommonVector();
            authRq.aLong.Add(type);
            authRq.aLong.Add(GamePrefs.LastLang);
            //authRq.aLong.Add(LangHandler.lastType);
            authRq.aString.Add(token);
            authRq.aString.Add(Constants.VERSION);
            authRq.aString.Add(Constants.PLATFORM);
            authRq.aString.Add(Constants.OS_TYPE);
            authRq.aString.Add(Constants.CP_CODE);
            authRq.aString.Add(Constants.DEVICE_ID);//imei            
            authRq.aString.Add(Constants.CLIENT_IP);
            authRq.aString.Add(Constants.CLIENT_MCC);

            SendProto(IService.LOGIN_GOOGLE, authRq);
        }
        public void SetLanguage(int typeLang)
        {
            CommonVector commonVector = new CommonVector();
            commonVector.aLong.Add(typeLang);
            SendProto(IService.SET_LANGUAGE, commonVector);
        }
        public void LoginNormal(string username, string password)
        {
            GameData.main.loginUsername = username;
            GameData.main.loginPassword = password;
            SendProto(IService.LOGIN);
        }

        public void GetUserHeroCard() { SendProto(IService.GET_USER_HERO_CARD); }
        public void GetUserBattleDeck() { SendProto(IService.GET_USER_BATTLE_DECK); }
        public void DeleteUserDeck(long id)
        {
            CommonVector cv = new CommonVector();
            cv.aLong.Add(id);
            SendProto(IService.DELETE_USER_DECK, cv);
        }
        public void SetUserBattleDeck(List<long> lstGod, List<long> lstTrooper)
        {
            // [id, number]
            CommonVector cvTrooper = new CommonVector();
            cvTrooper.aLong.AddRange(lstTrooper);

            CommonVector cvGod = new CommonVector();
            cvGod.aLong.AddRange(lstGod);

            ListCommonVector lcv = new ListCommonVector();
            lcv.aVector.Add(cvTrooper);
            lcv.aVector.Add(cvGod);

            SendProto(IService.SET_USER_BATTLE_DECK, lcv);
        }

        public void SetUserBattleDeck(long deckId, string mode)
        {
            CommonVector cv = new CommonVector();
            cv.aLong.Add(deckId);
            cv.aString.Add(mode);
            SendProto(IService.SET_USER_BATTLE_DECK, cv);
        }

        public void GetUserDeck()
        {
            SendProto(IService.GET_USER_DECK);
        }

        public void SetUserCustomDeck(List<long> lstGod, List<long> lstTrooper, string deckName)
        {
            // [id, number]
            CommonVector cvName = new CommonVector();
            cvName.aString.Add(deckName);
            CommonVector cvTrooper = new CommonVector();
            cvTrooper.aLong.AddRange(lstTrooper);

            CommonVector cvGod = new CommonVector();
            cvGod.aLong.AddRange(lstGod);

            ListCommonVector lcv = new ListCommonVector();
            lcv.aVector.Add(cvName);
            lcv.aVector.Add(cvGod);
            lcv.aVector.Add(cvTrooper);

            SendProto(IService.SET_USER_DECK, lcv);
        }
        public void UpdateUserDeck(List<long> lstGod, List<long> lstTrooper, string deckName, long deckId)
        {
            // [id, number]
            CommonVector cv = new CommonVector();
            cv.aString.Add(deckName);
            cv.aLong.Add(deckId);
            CommonVector cvTrooper = new CommonVector();
            cvTrooper.aLong.AddRange(lstTrooper);

            CommonVector cvGod = new CommonVector();
            cvGod.aLong.AddRange(lstGod);

            ListCommonVector lcv = new ListCommonVector();
            lcv.aVector.Add(cv);
            lcv.aVector.Add(cvGod);
            lcv.aVector.Add(cvTrooper);

            SendProto(IService.UPDATE_USER_DECK, lcv);
        }
        public void GetLeaderboard()
        {
            SendProto(IService.GET_LEADER_BOARD);
        }

        public void GetUserEventInfo(CommonVector cv)
        {
            SendProto(IService.GET_USER_EVENT_INFO, cv);
        }

        public void GetEvent()
        {
            SendProto(IService.GET_EVENT);
        }
        public void GetRank()
        {
            SendProto(IService.GET_RANK);
        }
        public void GetMode()
        {
            SendProto(IService.GET_MODE);
        }
        public void ViewReward()
        {
            SendProto(IService.VIEW_REWARD);
        }
        public void GetReward(long idSeason,long idRank)
        {
            CommonVector cv = new CommonVector();
            cv.aLong.Add(idSeason);
            cv.aLong.Add(idRank);
            SendProto(IService.GET_REWARD, cv);
        }
        public void GetProfile()
        {
            SendProto(IService.GET_PROFILE);
        }

        public void GetUserDeckDetail(long id)
        {
            CommonVector cv = new CommonVector();
            cv.aLong.Add(id);
            SendProto(IService.GET_USER_DECK_DETAIL, cv);
        }
        public void GameBattleLeave()
        {
            SendProto(IService.GAME_BATTLE_LEAVE);
        }

        public void GameBattleJoin()
        {
            CommonVector cv = new CommonVector();
            cv.aString.Add(GameData.main.currentPlayMode);
            SendProto(IService.GAME_BATTLE_AUTO_JOIN, cv);
        }

        public void GameMulligan(CommonVector commonVector)
        {
            SendProto(IService.GAME_MULLIGAN, commonVector);
        }

        public void GameFirstGodSummon(CommonVector commonVector)
        {
            SendProto(IService.GAME_FIRST_GOD_SUMMON, commonVector);
        }

        public void GameMoveGodSummon(CommonVector commonVector)
        {
            SendProto(IService.GAME_MOVE_GOD_SUMMON, commonVector);
        }

        public void GameStartupConfirm()
        {
            SendProto(IService.GAME_STARTUP_CONFIRM);
        }

        public void GameMoveCardInbattle(CommonVector commonVector)
        {
            SendProto(IService.GAME_MOVE_CARD_IN_BATTLE, commonVector);
        }

        public void GameSummonCardInBatttle(CommonVector commonVector)
        {

            LogWriterHandle.WriteLog(string.Join(",", commonVector.aLong));
            SendProto(IService.GAME_SUMMON_CARD_IN_BATTLE, commonVector);
        }

        public void GameConfirmStartBattle()
        {
            SendProto(IService.GAME_CONFIRM_STARTBATTLE);
        }

        public void GameChooseWayRequest(CommonVector commonVector)
        {
            SendProto(IService.GAME_CHOOSE_WAY_REQUEST, commonVector);
        }

        public void GameSimulateConfirm(CommonVector commonVector)
        {
            SendProto(IService.GAME_SIMULATE_CONFIRM, commonVector);
        }

        public void GameActiveSkill(CommonVector commonVector)
        {
            SendProto(IService.GAME_ACTIVE_SKILL, commonVector);
        }

        public void GameStartBid()
        {
            SendProto(IService.GAME_START_BID);
        }

        public void GameUpBid()
        {
            SendProto(IService.GAME_UP_BID);
        }
        public void ConfirmBidState()
        {
            SendProto(IService.GAME_BID_COMFIRM);
        }
        //-----------------------method received
        public void GetCCU() { SendProto(63744); }
        public void GetUserPack()
        {
            SendProto(IService.GET_USER_ITEMS);
        }
        public void GetShopItem()
        {
            CommonVector cv = new CommonVector();
            SendProto(IService.GET_SHOP, cv);
        }

        public void GetPackDetail(long shopItemId)
        {
            CommonVector cv = new CommonVector();
            cv.aLong.Add(shopItemId);
            SendProto(IService.GET_PACK_DETAIL, cv);
        }
        public void BuyItem(long shopItemId, long count)
        {
            CommonVector cv = new CommonVector();
            cv.aLong.Add(shopItemId);
            cv.aLong.Add(count);
            SendProto(IService.BUY_ITEM, cv);
        }
        public void OpenUserPack(long itemId)
        {
            CommonVector cv = new CommonVector();
            cv.aLong.Add(itemId);
            SendProto(IService.OPEN_CHEST, cv);
        }
        public void GetUserCurrency()
        {
            SendProto(IService.GET_BALANCE);
        }
        public void GetQuests()
        {
            SendProto(IService.GET_QUESTS);
        }
        public void RechargeGem(long skuId)
        {
            CommonVector cv = new CommonVector();
            cv.aLong.Add(skuId);
            SendProto(IService.RECHARGE_GEM, cv);
        }
        public byte[] Serializer( object obj)
        {
            return socket.SerializerByte(obj);
        }
        public void UpdateProgression(long state, string nickname = "")
        {
            CommonVector cv = new CommonVector();
            cv.aLong.Add(state);
            cv.aString.Add(nickname);
            SendProto(IService.UPDATE_PROGRESS, cv);
        }
        public void GetVerifyIAP(string skuId, string packageName, string token)
        {
            CommonVector cv = new CommonVector();
            cv.aString.Add(skuId);
            cv.aString.Add(packageName);
            cv.aString.Add(token);
            SendProto(IService.GET_VERIFY_IAP, cv);
        }
        public void GetAccessTokenWebview()
        {
            CommonVector cv = new CommonVector();
            SendProto(IService.GET_ACCESSTOKEN, cv);
        }

        public void GetUserLevel()
        {
            SendProto(IService.GET_USER_LEVEL);
        }
        public void GetUserTray()
        {
            SendProto(IService.GET_TRAY);
        }
        public void GetTimeChest(int id)
        {
            CommonVector cv = new CommonVector();
            cv.aLong.Add(id);
            SendProto(IService.GET_TIME_CHEST, cv);
        }
        public void ActivateTimeChest(long id)
        {
            CommonVector cv = new CommonVector();
            cv.aLong.Add(id);
            SendProto(IService.ACTIVATE_TIME_CHEST, cv);
        }
        public void OpenTimeChest(long id)
        {
            CommonVector cv = new CommonVector();
            cv.aLong.Add(id);
            SendProto(IService.OPEN_TIME_CHEST, cv);
        }
        public void GetUserMap()
        {
            SendProto(IService.GET_MAP);
        }
        public void ClaimLevelReward(long id)
        {
            CommonVector cv = new CommonVector();
            cv.aLong.Add(id);
            SendProto(IService.CLAIM_LEVEL_REWARD, cv);
        }
        public void ViewUpgrade()
        {
            SendProto(IService.VIEW_UPGRADE);
        }
        public void ViewUpgradeDetail(long heroId)
        {
            CommonVector cv = new CommonVector();
            cv.aLong.Add(heroId);
            SendProto(IService.VIEW_UPGRADE_DETAIl, cv);
        }
        public void Upgrade(long heroId)
        {
            CommonVector cv = new CommonVector();
            cv.aLong.Add(heroId);
            SendProto(IService.UPGRADE, cv);
        }
        public void FirstDeck()
        {
            SendProto(IService.FIRST_DECK);
        }
        public void GetDefaultScreenName()
        {
            SendProto(IService.GET_RANDOM_SCREEN_NAME);
        }
        public void SetScreenName(string srName)
        {
            CommonVector cv = new CommonVector();
            cv.aString.Add(srName);
            SendProto(IService.SET_SCREEN_NAME, cv);
        }
    }
}