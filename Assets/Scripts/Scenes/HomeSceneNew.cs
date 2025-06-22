using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GIKCore;
using GIKCore.Net;
using GIKCore.Lang;
using GIKCore.DB;
using GIKCore.Utilities;
using GIKCore.UI;
using TMPro;
using pbdson;
using UnityEngine.SceneManagement;
using Net = UnityEngine.Networking.UnityWebRequest;
using UnityEngine.UI;
using UIEngine.UIPool;
using System.Xml;
using static HomeChestProps;
using GIKCore.Bundle;
using GIKCore.Sound;
#if UNITY_EDITOR || UNITY_STANDALONE
using ZenFulcrum.EmbeddedBrowser;
#endif
using System.Text.RegularExpressions;

public class HomeSceneNew : GameListener
{
    [SerializeField] private TextMeshProUGUI versionName, m_TxtName;
    [SerializeField] private GameObject HomeUI;
    List<long> lstTrooper = new List<long>();
    List<long> lstGod = new List<long>();
    [SerializeField] private GameObject videPlayer, videoBackground, loginBtn, loginFBBtn, popupTermOfService;
    [SerializeField] private List<GoOnOff> m_ListTermCheckmarks;
    [SerializeField] private Image m_UserLevelProgress;
    [SerializeField] private NavHeader m_NavHeader;
    [SerializeField] private GameObject progressionPref;
    [SerializeField] private Image m_Avatar;
    [SerializeField] private GameObject rankImgCND;
    [SerializeField] private SwipeRewardPopup m_SwipeRewardPopup;
    [SerializeField] private EventSliderContainer m_EventSliderContainer;
    [SerializeField] private EventBannerContainer m_EventBannerContainer;
    [SerializeField] private GameObject m_RewardNewbie;
    [SerializeField] private GameObject m_PointClickPlay;
    [SerializeField] private Image m_BGHomescene;
    [SerializeField] private PopupReward m_PopupRward;
    [SerializeField] private List<HomeChest> m_ListHomeChest;
    [SerializeField] private TextMeshProUGUI m_TextLevel;
    [SerializeField] private Image m_LevelFillSmall;
    [SerializeField] private Image m_LevelFillBig;
    [SerializeField] private TextMeshProUGUI m_TextLevelBig;
    [SerializeField] private GameObject m_PopupBaseHp;
    [SerializeField] private List<HomeMapLevel> m_ListHomeMapLevel;
    [SerializeField] private Transform m_AvatarItem;
    [SerializeField] private Image m_MapLineFill;
    [SerializeField] private SC_BounceOnVisible m_BounceScript;
#if UNITY_EDITOR || UNITY_STANDALONE
    [SerializeField] private Browser currentBrowser;
#endif

    public static HomeSceneNew instance;
    // Values
    private long currentChestId;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }
    //Methods
    public void DoClickPlay()
    {
        if (PopupLogin.NeedLoginFirst()) return;
        HomeUI.SetActive(true);
        m_BounceScript.SetScaleZero();
        StartCoroutine(m_BounceScript.BeginBounce());
        //DoSetBattleDeckAuto();
    }
    public void DoClickPlayOnHomeScene()
    {
        //chuyen scene set deck
        Game.main.socket.GetUserDeck();
        Game.main.socket.GetRank();
        Game.main.LoadScene("SelectDeckScene", delay: 0.3f, curtain: true);
        //DoSetBattleDeckAuto();
    }
    public void DoClickLogout()
    {
        PopupConfirm.Show(content: LangHandler.Get("cf-3", "SELECT OK TO LOG OUT."),
            title: LangHandler.Get("but-24", "LOG OUT"),
            action1: LangHandler.Get("but-20", "CANCEL"), action2: "OK",
            action2Callback: go =>
            {
                Game.main.socket.Logout();
                if (ProgressionController.instance != null)
                {
                    Destroy(ProgressionController.instance.gameObject);
                    ProgressionController.instance = null;
                }
                Game.main.LoadScene("HomeSceneNew", delay: 0.3f, curtain: true);
            });
    }
    public void DoClickFriend()
    {
        // Toast.Show("Coming Soon!");
    }
    public void DoClickTopUpGem()
    {
        HandleNetData.QueueNetData(NetData.ACTION_OPEN_SCENE);
        Game.main.LoadScene("ShopScene", delay: 0.3f, curtain: true);
    }
    public void DoClickCollectionButton()
    {
        //Game.main.socket.GetUserDeckDetail(id);
        //Game.main.socket.GetUserDeck();
        Game.main.socket.GetUserPack();
        Game.main.LoadScene("CollectionScene", delay: 0.3f, curtain: true);
    }
    public void DoClickLoadPolicy()
    {
        Application.OpenURL("https://mytheria.io/policies");
    }

    public void DoClickLoadinforService()
    {
        Application.OpenURL("https://mytheria.io/terms");
    }

    public void DoClickDisableButton()
    {
        SoundHandler.main.PlaySFX("900_click_4", "sounds");
        Toast.Show(LangHandler.Get("toast-8", "Coming Soon!"));
    }
    public void DoClickAvatar()
    {
        SoundHandler.main.PlaySFX("900_click_4", "sounds");
        Game.main.socket.GetProfile();
        Game.main.LoadScene("ProfileScene", delay: 0.3f, curtain: true);
    }
    public void DoClickBaseHpPopup()
    {
        SoundHandler.main.PlaySFX("900_click_4", "sounds");
        m_PopupBaseHp.SetActive(true);
    }
    public void DoClickShowPopupReward()
    {
        SoundHandler.main.PlaySFX("900_click_4", "sounds");
        if (GameData.main.userProgressionState >= 10)
        {
            if (GameData.main.userProgressionState == 10)
            {
                Game.main.socket.UpdateProgression(10);
                GameData.main.userProgressionState = 11;
            }
            if (GameData.main.userProgressionState == 12)
            {
                Game.main.socket.UpdateProgression(12);
                GameData.main.userProgressionState = 13;
            }
            if (ProgressionController.instance != null)
                ProgressionController.instance.HideState();
            m_PopupRward.Show().SetData();
        }
        //PopupOpenReward.Show();
    }
    private void SetProfileData()
    {
        //m_TxtName.text = GameData.main.profile.displayName;
        m_NavHeader.SetData();
    }
    private void InitData()
    {
        m_Avatar.sprite = CardData.Instance.GetGodIconSprite("1");
        versionName.text = Application.version;
        //PopupLogin.Hide();
        videPlayer.SetActive(false);
        videoBackground.SetActive(false);
        HomeUI.SetActive(true);
        loginBtn.SetActive(false);
        loginFBBtn.SetActive(false);
        //m_BGHomescene.material = CardData.Instance.GetAnimatedMaterial("HomeScreen") as Material;
        //m_BGHomescene.material.shader = Shader.Find("ReadyPrefab/Shader3.14");
        if (GameData.main.lstSystemEvent.Count > 0)
        {
            List<SystemEvent> eventSlider = new List<SystemEvent>();
            List<SystemEvent> eventBanner = new List<SystemEvent>();
            for (int i = 0; i < GameData.main.lstSystemEvent.Count; i++)
            {
                if (GameData.main.lstSystemEvent[i].slide != "")
                    eventSlider.Add(GameData.main.lstSystemEvent[i]);

                if (GameData.main.lstSystemEvent[i].banner != "")
                    eventBanner.Add(GameData.main.lstSystemEvent[i]);

                m_EventSliderContainer.SetData(eventSlider);
                //m_EventBannerContainer.gameObject.SetActive(eventSlider.Count > 0);
                m_EventBannerContainer.gameObject.SetActive(eventBanner.Count > 0);
                //if (eventSlider.Count > 0) 
                //    m_EventBannerContainer.SetData(eventSlider);
                if (eventBanner.Count > 0)
                    m_EventBannerContainer.SetData(eventBanner);
            }
        }
        m_EventSliderContainer.gameObject.SetActive(GameData.main.lstSystemEvent.Count > 0);
        if (!GamePrefs.isLoggedIn)//not login yet
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            currentBrowser.onLoadComplete += OnLoadBrowserComplete;
#endif
            videPlayer.SetActive(true);
            videoBackground.SetActive(true);
            loginBtn.SetActive(true);
#if (UNITY_IOS || UNITY_ANDROID)
            loginFBBtn.SetActive(true);
#else
            loginFBBtn.SetActive(false);
#endif

        }
        else
        {
            if (!PlayerPrefs.HasKey("TRACK_FIRST_LOGIN_SUCCESS"))
            {
                PlayerPrefs.SetInt("TRACK_FIRST_LOGIN_SUCCESS", 1);
                ITrackingParameter pr = new ITrackingParameter() { name = "first_login", value = "true" };
                ITracking.LogEventFirebase(ITracking.TRACK_FIRST_LOGIN_SUCCESS, pr);
            }
            //if (GameData.main.isNewbie)
            //{
            //    Game.main.socket.NewbieEvent();
            //}
            //else
            //{
            //    Debug.Log("Lỗi ở đâyy");
            //}
            GamePrefs.TOS = true;
            if (GameData.main.userProgressionState < 18 && ProgressionController.instance == null)
            {
                Instantiate(progressionPref);
            }


            SetProfileData();
            Game.main.socket.GetUserHeroCard();
            Game.main.socket.GetLeaderboard();
            Game.main.socket.GetUserDeck();
            Game.main.socket.GetUserLevel();
            Game.main.socket.GetUserMap();
            if(GameData.main.userProgressionState >=14)
                Game.main.socket.GetUserTray();
            LogWriterHandle.WriteLog("LoginComplete____________________");
            //if(!string.IsNullOrEmpty(GameData.main.rankImg))
            //{
            //    rankImgCND.SetActive(true);
            //    LoadHttpRankImg(GameData.main.rankImg);
            //    GameData.main.rankImg = "";
            //    rankImgCND.transform.GetComponent<Button>().onClick.AddListener(delegate
            //    {
            //        Debug.Log("add listener to rank img");
            //        rankImgCND.SetActive(false);
            //    });
            //}    
#if UNITY_EDITOR || UNITY_STANDALONE
            currentBrowser.gameObject.SetActive(false);
#endif
        }

    }

    IEnumerator coroutine = null;
    public void LoadHttpRankImg(string url, ICallback.CallFunc2<Sprite> onLoaded = null)
    {
        if (coroutine != null)
            Game.main.StopCoroutine(coroutine);

        if (string.IsNullOrEmpty(url)) return;
        coroutine = IUtil.LoadTexture2DFromUrl(url, (Texture2D tex) =>
        {
            if (tex == null) return;
            Sprite sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            rankImgCND.transform.GetChild(0).GetComponent<Image>().sprite = sprite;

            if (onLoaded != null) onLoaded(sprite);
        });
        Game.main.StartCoroutine(coroutine);
    }
    private void OnLoadBrowserComplete(string message)
    {
        string testMessage = message.Split('{', '}')[1];
        string[] schemeSplit = testMessage.Split(new string[] { "://" }, System.StringSplitOptions.None);
        if (schemeSplit.Length == 1)
        {
            // `://` not existing. Try `:/` instead.
            schemeSplit = testMessage.Split(new string[] { ":/" }, System.StringSplitOptions.None);
        }
        if (schemeSplit.Length == 1)
        {
            // `:/` not existing. Try `:` instead.
            schemeSplit = testMessage.Split(new string[] { ":" }, System.StringSplitOptions.None);
        }

        if (schemeSplit.Length >= 2)
        {
            string Scheme = schemeSplit[0];
            UniWebViewLogger.Instance.Debug("Get scheme: " + Scheme);

            string pathAndArgsString = "";
            int index = 1;
            while (index < schemeSplit.Length)
            {
                pathAndArgsString = string.Concat(pathAndArgsString, schemeSplit[index]);
                index++;
            }
            UniWebViewLogger.Instance.Verbose("Build path and args string: " + pathAndArgsString);

            string[] split = pathAndArgsString.Split("?"[0]);

            string Path = Net.UnEscapeURL(split[0].TrimEnd('/'));
            Dictionary<string, string> Args = new Dictionary<string, string>();
            if (split.Length > 1)
            {
                foreach (string pair in split[1].Split("&"[0]))
                {
                    string[] elems = pair.Split("="[0]);
                    if (elems.Length > 1)
                    {
                        var key = Net.UnEscapeURL(elems[0]);
                        if (Args.ContainsKey(key))
                        {
                            var existingValue = Args[key];
                            Args[key] = existingValue + "," + Net.UnEscapeURL(elems[1]);
                        }
                        else
                        {
                            Args[key] = Net.UnEscapeURL(elems[1]);
                        }
                    }
                }
            }

            if (Args.ContainsKey("at"))
            {
                string rt = Args["rt"].Split('\"')[0];
                string at = Args["at"];
                //PlayerPrefs.SetString("rt", rt);
                //PlayerPrefs.SetString("at", at);
                Game.main.socket.LoginBlockchain(at, rt);
            }
        }
        else
        {
            Toast.Show(LangHandler.Get("67", "Bad url scheme"));
        }
    }

    public void DoClickLogin()
    {
        SoundHandler.main.PlaySFX("900_click_3", "sounds");
        if (!PlayerPrefs.HasKey("TRACK_FIRST_LOGIN_MYRA"))
        {
            PlayerPrefs.SetInt("TRACK_FIRST_LOGIN_MYRA", 1);
            ITrackingParameter pr = new ITrackingParameter() { name = "first_login_mytheria_account", value = "true" };
            ITracking.LogEventFirebase(ITracking.TRACK_FIRST_LOGIN_MYRA, pr);
        }
        Game.main.netBlock.Show();
#if UNITY_EDITOR
        if (GamePrefs.Username != null && !GamePrefs.Username.Equals(""))
            Game.main.socket.AutoLogin(GamePrefs.Username);
        else
            currentBrowser.gameObject.SetActive(true);
#elif UNITY_ANDROID || UNITY_IOS
        if (GamePrefs.Username != null && !GamePrefs.Username.Equals(""))
            Game.main.socket.AutoLogin(GamePrefs.Username);
        else
        {
            PopupWebview.Show("https://openid.helitech-solutions.com/", true);
            //PopupWebview.Show("https://myraopenid.helistudio.vn/", true);

        }
#else
       if (GamePrefs.Username != null && !GamePrefs.Username.Equals(""))
            Game.main.socket.AutoLogin(GamePrefs.Username);
        else
            currentBrowser.gameObject.SetActive(true);
#endif
    }

    public void DoClickAcceptTermOfService()
    {
        foreach (GoOnOff go in m_ListTermCheckmarks)
        {
            if (!go.online)
            {
                Toast.Show(LangHandler.Get("158", "YOU NEED TO AGREE TO ALL THE FOLLOWING"));
                return;
            }
        }
        InitData();
    }
    public void DoClickCancelTermOfService()
    {
        Game.main.socket.Logout();

        if (ProgressionController.instance != null)
        {
            Destroy(ProgressionController.instance.gameObject);
            ProgressionController.instance = null;
        }
        Game.main.LoadScene("HomeSceneNew", delay: 0.3f, curtain: true);
    }
    public void DoClickCheckmark(int index)
    {
        if (index == 0)
        {
            m_ListTermCheckmarks[0].Turn(!m_ListTermCheckmarks[0].online);
            m_ListTermCheckmarks[1].Turn(m_ListTermCheckmarks[0].online);
            m_ListTermCheckmarks[2].Turn(m_ListTermCheckmarks[0].online);
        }
        else
        {
            m_ListTermCheckmarks[index].Turn(!m_ListTermCheckmarks[index].online);
            m_ListTermCheckmarks[0].Turn(m_ListTermCheckmarks[1].online && m_ListTermCheckmarks[2].online);
        }
    }
    public void DoClickLoginFacebook()
    {
#if UNITY_ANDROID || UNITY_IOS

        Game.main.netBlock.Show();
        Game.main.FB.Login(LoginFacebookSuccess, LoginFacebookFailed);
#else
        Toast.Show(LangHandler.Get("toast-8", "Coming Soon!"));
#endif
    }
#if !UNITY_STANDALONE
    private void LoginFacebookFailed()
    {
        LogWriterHandle.WriteLog("Facebook Login Failed");
    }

    private void LoginFacebookSuccess()
    {
        LogWriterHandle.WriteLog("Facebook Login Success");
    }

    public void DoClickLoginGoogle()
    {
        Game.main.google.OnSignIn(LoginGoogleSuccess, LoginGoogleCanceled, LoginGoogleFailed);
        SoundHandler.main.PlaySFX("900_click_3", "sounds");
        if (!PlayerPrefs.HasKey("TRACK_FIRST_LOGIN_GG"))
        {
            PlayerPrefs.SetInt("TRACK_FIRST_LOGIN_GG", 1);
            ITrackingParameter pr = new ITrackingParameter() { name = "first_login_google", value = "true" };
            ITracking.LogEventFirebase(ITracking.TRACK_FIRST_LOGIN_GG, pr);
        }
    }
    private void LoginGoogleSuccess(string token)
    {
        LogWriterHandle.WriteLog("Google Login Success");
        Game.main.socket.LoginGoogle(token, 1);
    }

    private void LoginGoogleFailed(string e)
    {
        LogWriterHandle.WriteLog("Google Login Failed: " + e);
    }

    private void LoginGoogleCanceled()
    {
        LogWriterHandle.WriteLog("Google Login Canceled");
    }
#endif

    public override bool ProcessSocketData(int serviceId, byte[] data)
    {
        if (base.ProcessSocketData(serviceId, data)) return true;
        switch (serviceId)
        {
            case IService.CHECK_VERSION:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    if (cv.aLong[0] == 0)
                    {
                        Toast.Show(cv.aString[0]);
                        LogWriterHandle.WriteLog(cv.aString[0]);
                    }
                    else if (cv.aLong[0] == 1)
                    {
                        InitData();
                    }
                    else if (cv.aLong[0] == 2)
                    {
                        InitData();
                        PopupConfirm.Show("Mytheria now has new version!", "NOTICE", "CLOSE");
                    }
                    else if (cv.aLong[0] == 3)
                    {
                        InitData();
                        //check lại luong vi van login duoc
                        PopupConfirm.Show(content: "Client outdated. Please download the latest client at Mytheria.io",
                                          title: "NOTICE",
                                          action1: LangHandler.Get("but-23", "CONFIRM"),
                                          action1Callback: go =>
                                          {
                                              go.SetActive(false);
                                              loginBtn.SetActive(false);
                                              loginFBBtn.SetActive(false);
                                          }
                                          );
                    }
                    break;
                }
            case IService.NEWBIE_EVENT:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    GameData.main.isNewbie = cv.aLong[0] == 0 ? true : false;
                    GameData.main.statusNewbie = cv.aLong[1] == 0 ? false : true;
                    GameData.main.imgRewardNewbie = cv.aString[0];
                    GameData.main.passFirst10Match = cv.aLong[2] == 1 ? true : false;
                    if (GameData.main.isNewbie)
                    {
                        m_RewardNewbie.SetActive(true);
                    }
                    if (!GameData.main.passFirst10Match)
                    {
                        m_PointClickPlay.SetActive(true);
                    }
                    break;

                }
            case IService.LOGIN:
                {
                    GameData.main.lstSystemEvent.Clear();
                    Game.main.netBlock.Hide();
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    int START_LONG = 3; // id, state, winstatus ?
                    int BLOCK_LONG = 3;
                    int START_STRING = 2;
                    int BLOCK_STRING = 3;
                    int COUNT_LONG = (cv.aLong.Count - START_LONG) / BLOCK_LONG;
                    for (int i = 0; i < COUNT_LONG; i++)
                    {
                        SystemEvent se = new SystemEvent();
                        se.id = (int)cv.aLong[START_LONG + i * BLOCK_LONG + 0];
                        se.end = cv.aLong[START_LONG + i * BLOCK_LONG + 1];
                        se.position = (int)cv.aLong[START_LONG + i * BLOCK_LONG + 2];

                        se.banner = cv.aString[START_STRING + i * BLOCK_STRING + 0];
                        se.slide = cv.aString[START_STRING + i * BLOCK_STRING + 1];
                        se.link = cv.aString[START_STRING + i * BLOCK_STRING + 2];
                        if (se.slide != "" || se.banner != "")
                            GameData.main.lstSystemEvent.Add(se);
                    }
                    if (GamePrefs.TOS)
                    {
                        InitData();
                    }
                    else
                    {
                        popupTermOfService.SetActive(true);
#if UNITY_EDITOR || UNITY_STANDALONE
                        currentBrowser.gameObject.SetActive(false);
#endif
                    }
                    Game.main.socket.GetUserCurrency();
                    break;
                }
            case IService.AUTO_LOGIN:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    if (cv.aLong[0] == 0)
                    {
                        Toast.Show(cv.aString[0]);
#if UNITY_EDITOR
                        currentBrowser.gameObject.SetActive(true);
#elif UNITY_ANDROID || UNITY_IOS
                        PopupWebview.Show("https://openid.helitech-solutions.com/", true);
                        //PopupWebview.Show("https://myraopenid.helistudio.vn/", true);
#else
                        currentBrowser.gameObject.SetActive(true);
#endif
                    }
                    //else
                    //{
                    //    Game.main.socket.LoginBlockchain(PlayerPrefs.GetString("at"), PlayerPrefs.GetString("rt"));
                    //}
                    Game.main.socket.GetUserCurrency();
                    break;
                }
            case IService.SET_USER_BATTLE_DECK:
                {
                    Game.main.LoadScene("FindMatchScene", delay: 0.3f, curtain: true);
                    break;
                }

            case IService.GAME_RESUME:
                {
                    ListCommonVector lcv = ISocket.Parse<ListCommonVector>(data);
                    ResumeGame(lcv);
                    break;
                }
            case IService.GET_USER_DECK:
                {
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

                    //for (int i = 2; i < cv.aLong.Count; i += 5)
                    //{
                    //    DeckInfo info = new DeckInfo()
                    //    {
                    //        deckID = cv.aLong[i],
                    //        deckStatus = cv.aLong[i + 1],
                    //        isDefaultDeck = cv.aLong[i + 2] == 1,
                    //        isLastDeck = cv.aLong[0] == cv.aLong[i],
                    //        deckName = cv.aString[(i - 2) / 5],
                    //        isDeckEven = cv.aLong[i + 3] == 1
                    //    };
                    //    GameData.main.lstDeckInfo.Add(info);
                    //}
                    break;
                }
            case IService.GET_USER_LEVEL:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    for (int i = 0; i < cv.aLong.Count; i++)
                    {
                        if (i == 0)
                        {
                            GameData.main.userCurrency.exp = cv.aLong[i];
                        }
                        else if (i == 1)
                        {

                        }
                        else if (i == 2)
                        {
                            GameData.main.isLevelUp = cv.aLong[i] == 1;
                        }
                    }
                    Debug.Log("GameData.main.userCurrency.exp: " + GameData.main.userCurrency.shard);
                    DBLevel level = Database.GetUserLevelInfo(GameData.main.userCurrency.exp);
                    DBBaseHp baseHp = Database.GetBaseHp((int)GameData.main.userCurrency.shard);
                    m_TextLevel.text = level.id + "";
                    GameData.main.userHasReachedLevel6 = (level.id >=6) ? true : false;
                    m_LevelFillSmall.fillAmount = (float)level.expCurrent / level.expToUpLevel;


                    m_LevelFillBig.fillAmount = (float)baseHp.shardCurrent / baseHp.shardToUpLevel;
                    m_TextLevelBig.text = baseHp.shardCurrent + "/" + baseHp.shardToUpLevel;
                    break;
                }
            case IService.GET_TRAY:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    int BLOCK_INT = 6;
                    int COUNT_BLOCK_INT = cv.aLong.Count / BLOCK_INT;
                    GameData.main.listHomeChestProps.Clear();
                    bool hasOpening = false;
                    for (int i = 0; i < COUNT_BLOCK_INT; i++)
                    {
                        HomeChestProps props = new HomeChestProps();
                        props.id = (int)cv.aLong[i * BLOCK_INT];
                        props.state = (ChestState)cv.aLong[i * BLOCK_INT + 1];
                        props.timeChest = (TimeChest)cv.aLong[i * BLOCK_INT + 2];
                        props.remainTime = cv.aLong[i * BLOCK_INT + 3];
                        props.gemPrice = (int)cv.aLong[i * BLOCK_INT + 4];
                        props.isNew = cv.aLong[i * BLOCK_INT + 5] == 1;
                        if (!hasOpening)
                            hasOpening = props.state == ChestState.ACTIVATED && props.remainTime > 0;
                        props.canOpen = !hasOpening;

                        switch (props.state)
                        {
                            case ChestState.EMPTY:
                                {
                                    props.status = ChestStatus.CHEST_EMPTY;
                                    break;
                                }
                            case ChestState.NOT_ACTIVATED:
                                {
                                    props.status = ChestStatus.CHEST_OPEN_NOW;
                                    break;
                                }
                            case ChestState.ACTIVATED:
                                {
                                    if (props.remainTime <= 0)
                                    {
                                        props.status = ChestStatus.CHEST_CLAIM;
                                    }
                                    else
                                    {
                                        props.status = ChestStatus.CHEST_OPENING;
                                    }
                                    break;
                                }
                        }

                        GameData.main.listHomeChestProps.Add(props);
                    }
                    for (int i = 0; i < GameData.main.listHomeChestProps.Count; i++)
                    {
                        m_ListHomeChest[i]
                            .SetData(GameData.main.listHomeChestProps[i])
                            .SetOnclickCB((id) =>
                            {
                                if (id != 0)
                                {
                                    currentChestId = id;
                                    Game.main.socket.GetTimeChest(id);
                                }
                                else
                                {
                                    //PopupChestDetail.Show()
                                }

                            });
                    }
                    break;
                }
            case IService.GET_TIME_CHEST:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    PopupChestDetailProps props = new PopupChestDetailProps();
                    props.chestId = currentChestId;
                    for (int i = 0; i < cv.aLong.Count; i++)
                    {
                        if (i == 0)
                        {

                        }
                        else if (i == 1)
                        {
                            props.chestType = (int)cv.aLong[i];
                        }
                        else if (i == 2)
                        {
                            props.numberCard = (int)cv.aLong[i];
                        }
                        else if (i == 3)
                        {
                            props.gold = (int)cv.aLong[i];
                        }
                        else if (i == 4)
                        {
                            props.exp = (int)cv.aLong[i];
                        }
                        else if (i == 5)
                        {
                            props.essence = (int)cv.aLong[i];
                        }
                        else if (i == 6)
                        {
                            props.sizeCommon = (int)cv.aLong[i];
                        }
                        else if (i == 7)
                        {
                            props.sizeRare = (int)cv.aLong[i];
                        }
                        else if (i == 8)
                        {
                            props.sizeEpic = (int)cv.aLong[i];
                        }
                        else if (i == 9)
                        {
                            props.sizeLegendary = (int)cv.aLong[i];
                        }
                        else if (i == 10)
                        {
                            props.isActivate = cv.aLong[i] == 2;
                        }
                        else if (i == 11)
                        {
                            props.remainTime = cv.aLong[i];
                        }
                        else if (i == 12)
                        {
                            props.haveOtherActivated = cv.aLong[i] == 1;
                        }
                        else if (i == 13)
                        {
                            props.gemPrice = (int)cv.aLong[i];
                        }
                    }
                    PopupChestDetail.Show(props);
                    break;
                }
            case IService.ACTIVATE_TIME_CHEST:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    if (cv.aLong[0] == 1)
                        Game.main.socket.GetUserTray();
                    break;
                }
            case IService.OPEN_TIME_CHEST:
                {
                    ListCommonVector lcv = ISocket.Parse<ListCommonVector>(data);
                    CommonVector cv0 = lcv.aVector[0];
                    if (cv0.aLong[0] == 1)
                    {
                        Game.main.socket.GetUserTray();
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

                        PopupOpenRewardProps props = new PopupOpenRewardProps();
                        props.listRewardCardBuilder = lstCardBuilder;
                        props.rewardDuplicateGoldBuilder = rdgb;
                        props.rewardBalanceBuilder = rbb;
                        PopupOpenReward.Show(props);
                        Game.main.socket.GetUserCurrency();
                    }
                    else
                    {
                        Toast.Show(cv0.aString[0]);
                    }
                    break;
                }
            case IService.GET_MODE:
                {
                    CommonVector cv1 = ISocket.Parse<CommonVector>(data);
                    LogWriterHandle.WriteLog("GET_MODE" + string.Join(",", cv1.aLong));
                    if (cv1.aLong.Count > 0)
                    {
                        GameData.main.haveRankSeason = cv1.aLong[0]==2 ? true: false;
                        if(GameData.main.haveRankSeason)
                            Game.main.LoadScene("SelectDeckRankScene", delay: 0.3f, curtain: true);
                        else
                            Game.main.LoadScene("SelectDeckScene", delay: 0.3f, curtain: true);
                    }
                    break;
                }
        }
        return false;
    }
    public override bool ProcessNetData(int id, object o)
    {
        if (base.ProcessNetData(id, o)) return true;
        switch (id)
        {
            case NetData.HOME_LOAD_MAP_LEVEL:
                {
                    SetMapData();
                    break;
                }
            case NetData.DO_CLICK_OPEN_HOME_LEVEL:
                {
                    DoClickShowPopupReward();
                    break;
                }
        }
        return false;
    }
    private void ResumeGame(ListCommonVector lcv)
    {
        foreach (CommonVector cv in lcv.aVector)
        {
            LogWriterHandle.WriteLog(string.Join(",", cv.aLong));
            LogWriterHandle.WriteLog(string.Join(",", cv.aString));
        }

        GameData.main.isResume = true;
        GameData.main.resumeData = lcv;
        GamePrefs.isLoggedIn = true;

        Game.main.socket.GetUserHeroCard();
        Game.main.socket.GetLeaderboard();
        Game.main.LoadScene("BattleScene", delay: 2f, curtain: true);
    }

    public void LoadHttpImage(string url)
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        if (!GamePrefs.isLoggedIn)
            videoBackground.SetActive(true);
        else
            HomeUI.SetActive(true);
        loginBtn.SetActive(false);
        loginFBBtn.SetActive(false);
        Game.main.socket.CheckVersion(Application.version);

        if (GameData.main.isX2Login)
        {
            GameData.main.isX2Login = false;
            PopupConfirm.Show(content: LangHandler.Get("toast-28", "DUPLICATE LOG IN."));
        }
    }

    // Update is called once per frame
    //void Update() { }

    public void LoadTutorial()
    {
        // Application.OpenURL("https://tutorial.mytheria.io/");
        SceneManager.LoadScene("BattleSceneTutorial");
    }
    public void UpdateHomeChestTut()
    {
        if (ProgressionController.instance.index == 0)
        {
            for (int i = 0; i < GameData.main.listHomeChestProps.Count; i++)
            {
                m_ListHomeChest[i]
                    .SetData(GameData.main.listHomeChestProps[i])
                    .SetOnclickCB((id) =>
                    {
                        if (id == -1)
                        {
                            if (GameData.main.listHomeChestProps.Find(x => x.id == id).status == ChestStatus.CHEST_OPEN_NOW)
                            {
                                //PopupChestDetail.Show()
                                //open to unlock chest
                                ProgressionController.instance.DoActionInState();



                            }
                            else
                                return;
                        }

                    });
            }
        }
        else if (ProgressionController.instance.index == 1)
        {
            for (int i = 0; i < GameData.main.listHomeChestProps.Count; i++)
            {
                m_ListHomeChest[i]
                    .SetData(GameData.main.listHomeChestProps[i])
                    .SetOnclickCB((id) =>
                    {
                        if (id == -1)
                        {
                            if (GameData.main.listHomeChestProps.Find(x => x.id == id).status == ChestStatus.CHEST_CLAIM)
                            {
                                //PopupChestDetail.Show()
                                //open to unlock chest
                                ProgressionController.instance.DoActionInState();
                            }
                            else
                                return;
                        }

                    });
            }
        }
        else if (ProgressionController.instance.index == 5)
        {
            for (int i = 0; i < GameData.main.listHomeChestProps.Count; i++)
            {
                m_ListHomeChest[i]
                    .SetData(GameData.main.listHomeChestProps[i])
                    .SetOnclickCB((id) =>
                    {
                        if (id == -1)
                        {
                            if (GameData.main.listHomeChestProps.Find(x => x.id == id).status == ChestStatus.CHEST_OPEN_NOW)
                            {
                                //PopupChestDetail.Show()
                                //open to unlock chest
                                ProgressionController.instance.canSkip = true;
                                ProgressionController.instance.NextState();
                            }
                            else
                                return;
                        }

                    });
            }
        }
        else if (ProgressionController.instance.index == 6)
        {
            for (int i = 0; i < GameData.main.listHomeChestProps.Count; i++)
            {
                m_ListHomeChest[i]
                    .SetData(GameData.main.listHomeChestProps[i])
                    .SetOnclickCB((id) =>
                    {
                        if (id == -1)
                        {
                            if (GameData.main.listHomeChestProps.Find(x => x.id == id).status == ChestStatus.CHEST_CLAIM)
                            {
                                ProgressionController.instance.DoActionInState();
                            }
                            else
                                return;
                        }

                    });
            }
        }
        else
        {
            for (int i = 0; i < GameData.main.listHomeChestProps.Count; i++)
            {
                m_ListHomeChest[i]
                    .SetData(GameData.main.listHomeChestProps[i])
                    .SetOnclickCB((id) =>
                    {
                    });
            }
        }

    }
    public void SetMapData()
    {
        DBLevel level = Database.GetUserLevelInfo(GameData.main.userCurrency.exp);
        int minLevel = 1;
        int maxLevel = 10;
        if (level.id % 10 == 0)
        {
            maxLevel = (int)level.id;
            minLevel = maxLevel - 9;
        }
        else
        {
            minLevel = ((int)level.id / 10) * 10 + 1;
            maxLevel = ((int)level.id / 10 + 1) * 10;
        }
        List<HomeMapLevelProps> lstHomeMapLevel = new List<HomeMapLevelProps>();
        for (int i = minLevel; i <= maxLevel; i++)
        {
            HomeRewardItemProps p = GameData.main.listHomeRewardItemProps.Find(x => x.id == i);
            if(p != null)
            {
                HomeMapLevelProps mapLevelProps = new HomeMapLevelProps();
                mapLevelProps.state = p.state;
                mapLevelProps.level = p.id;
                mapLevelProps.image = p.itemUrl;
                mapLevelProps.isHard = p.id % 10 == 4 || p.id % 10 == 9;
                mapLevelProps.isUserLevel = level.id == p.id || level.id + 1 < p.id;
                lstHomeMapLevel.Add(mapLevelProps);
            }
        }
        for (int i = 0; i < lstHomeMapLevel.Count; i++)
        {
            m_ListHomeMapLevel[i].SetData(lstHomeMapLevel[i]);
        }
        switch((int)level.id % 10)
        {
            case 1:
                {
                    m_MapLineFill.fillAmount = 0.08f;
                    break;
                }
            case 2:
                {
                    m_MapLineFill.fillAmount = 0.11f;
                    break;
                }
            case 3:
                {
                    m_MapLineFill.fillAmount = 0.19f;
                    break;
                }
            case 4:
                {
                    m_MapLineFill.fillAmount = 0.3f;
                    break;
                }
            case 5:
                {
                    m_MapLineFill.fillAmount = 0.37f;
                    break;
                }
            case 6:
                {
                    m_MapLineFill.fillAmount = 0.46f;
                    break;
                }
            case 7:
                {
                    m_MapLineFill.fillAmount = 0.56f;
                    break;
                }
            case 8:
                {
                    m_MapLineFill.fillAmount = 0.66f;
                    break;
                }
            case 9:
                {
                    m_MapLineFill.fillAmount = 0.76f;
                    break;
                }
            case 10:
                {
                    m_MapLineFill.fillAmount = 1f;
                    break;
                }
        }

        m_AvatarItem.position = m_ListHomeMapLevel[((int)level.id - 1) % 10].m_AvatarPosition.position;
    }
}
