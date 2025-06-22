using System.Collections;
using System.Collections.Generic;
using pbdson;
using GIKCore.DB;
using AppsFlyerSDK;
using GIKCore.Utilities;
using UnityEngine;

public class GameData
{
    private static GameData instance;
    public static GameData main
    {
        get
        {
            if (instance == null)
                instance = new GameData();
            return instance;
        }
    }
    public static void DoReset()
    {
        instance = null;
    }

    // Values
    public UserModel profile = new UserModel();

    public string loginUsername = "";
    public string loginPassword = "";
    public string accessToken = "";
    public string refreshToken = "";

    public long seedCode = 0;
    public long idGodFindMatch = 0;

    public bool isX2Login = false;
    public bool hadCustomDeck = false;
    public long selectedDeck = 0;

    public ITimeCache timerAutoEvent = new ITimeCache(3);

    public List<HeroCard> lstHeroCard = new List<HeroCard>();
    public List<HeroCard> lstCardCustomDeckEdit = new List<HeroCard>();
    public BattleDeck battleDeck = new BattleDeck();
    public List<DeckInfo> lstDeckInfo = new List<DeckInfo>();
    public List<PackInfo> lstUserPack = new List<PackInfo>();
    public List<ItemInfo> lstItemShopPack = new List<ItemInfo>();
    public List<ItemInfo> lstItemShopValuable = new List<ItemInfo>();
    public bool isFirstShop = false;
    public bool isFirstQuest = false;
    public List<SystemEvent> lstSystemEvent = new List<SystemEvent>();

    //battle scene
    public List<BattlePlayer> mLstBattlePlayer = new List<BattlePlayer>();
    public List<long> lstSkillAuction = new List<long>();
    public long eventTotalMatch;
    public long eventTotalWin;
    public long myraEvent;

    public bool isResume = false;
    public ListCommonVector resumeData = null;
    public bool isUsedUlti = false;
    public long ultiID = -1;
    public long skillIDBid=0;
    public List<long> lstGodDead = new List<long>();

    public string currentPlayMode = "unrank";
    public Currency userCurrency = new Currency();
    public RankModel userRankInfo = new RankModel();

    public long userProgressionState = 0;
    public bool isOutToModeSelection = false;
    public string rankImg = "";
    public bool isNewbie = true;
    public bool passFirst10Match = false;
    public bool userHasReachedLevel6 = false;
    public bool statusNewbie = false;
    public string imgRewardNewbie = "";
    public bool haveRankSeason = false;

    public Dictionary<string, Sprite> DictRewardSprite = new Dictionary<string, Sprite>();
    public List<HomeRewardItemProps> listHomeRewardItemProps = new List<HomeRewardItemProps>();
    public List<HomeChestProps> listHomeChestProps = new List<HomeChestProps>();

    public bool isLevelUp = false;

    // Methods
    public void SetLoginData(CommonVector cv)
    {
        long id = cv.aLong[0];
        long progressionState = cv.aLong[1];
        string username = cv.aString[0];
        string screenname = cv.aString[1];
        //string rankImg = cv.aString[2];

        GamePrefs.Username = username;
        AppsFlyer.setCustomerUserId(username);
        GamePrefs.isLoggedIn = true;
        if (GamePrefs.LastLoginType == (int)Constants.LoginType.Normal)
        {
            GamePrefs.Password = loginPassword;
        }
        GameData.main.userProgressionState = progressionState;
        //GameData.main.rankImg = rankImg;
        loginUsername = loginPassword = "";
        profile.SetData(id, username, screenname);
    }
}
