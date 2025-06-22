using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIEngine.UIPool;
using GIKCore.UI;
using GIKCore.Net;
using System.Linq;
using TMPro;
using GIKCore;
using pbdson;
using DG.Tweening;
using UnityEngine.SceneManagement;
using GIKCore.Lang;
using System;

public class CustomDeckScene : GameListener
{
    // Fields
    [SerializeField] private RecycleLayoutGroup m_PoolListUserCard;
    [SerializeField] private BoxPage m_BoxPage;
    [SerializeField] private Image iconMode;
    [SerializeField] private Sprite[] modes;
    [SerializeField] private TextMeshProUGUI txtMode;
    [SerializeField] private GridPoolGroup poolLstUserCard;
    [SerializeField] private VerticalPoolGroup poolLstCardCustomDeck;
    [SerializeField] private TextMeshProUGUI numberGod, numberCard, chooseMainGod;
    [SerializeField] private TMP_InputField deckName;
    [SerializeField] private GameObject iconDeckNameOk;
    [SerializeField] private GameObject mainGodButton;
    [SerializeField] private GameObject cardPreview;
    [SerializeField] private CardPreviewInfo godCardPreview;
    [SerializeField] private CardPreviewInfo minionCardPreview;
    [SerializeField] private CardPreviewInfo spellCardPreview;
    [SerializeField] private Toggle greenToggle, redToggle, yellowToggle, purpleToggle, whiteToggle, nftToggle, nonNftToggle, godToggle, mortalToggle, spellToggle;
    [SerializeField] private GameObject filterUse, filterNotUse;
    [SerializeField] private GameObject greenView, redView, yellowView, purpleView, whiteView;
    [SerializeField] private GameObject nftView, nonNftView;
    [SerializeField] private GameObject godView, mortalView, spellView;
    [SerializeField] private GameObject filterFrameUI;
    [SerializeField] private GameObject searchField, searchButton;
    [SerializeField] private TMP_InputField searchText;
    [SerializeField] private List<GoOnOff> m_ListSideTabButtons;
    [SerializeField] private RectTransform m_PopupMainGodBackground;
    [SerializeField] private SwipeRewardPopup m_SwipeRewardPopup;
    [SerializeField] private ITween m_Blur;
    [SerializeField] private GameObject m_ListCardHightlight;

    // Values
    //  [SerializeField] private InputField searchName;
    //  [SerializeField] private RectTransform frameUserCard, frameCustomDeck;
    private List<HeroCard> listCardCrop = new List<HeroCard>();
    private List<HeroCard> lstUserCard = new List<HeroCard>();
    private List<HeroCard> lstCardCustomDeck = new List<HeroCard>();
    private List<CellDeckCard> lstCardDeckGroup = new List<CellDeckCard>();
    private List<HeroCard> lstGod = new List<HeroCard>();
    private List<HeroCard> lstTrooper = new List<HeroCard>();
    private List<HeroCard> lstMainGod = new List<HeroCard>();
    private List<HeroCard> lstGreen = new List<HeroCard>();
    private List<HeroCard> lstRed = new List<HeroCard>();
    private List<HeroCard> lstYellow = new List<HeroCard>();
    private List<HeroCard> lstPurple = new List<HeroCard>();
    private List<HeroCard> lstWhite = new List<HeroCard>();
    private List<HeroCard> lstNFT = new List<HeroCard>();
    private List<HeroCard> lstNonNFT = new List<HeroCard>();
    private List<HeroCard> lstGodFilter = new List<HeroCard>();
    private List<HeroCard> lstMortalFilter = new List<HeroCard>();
    private List<HeroCard> lstSpellFilter = new List<HeroCard>();
    private List<HeroCard> lstCardFilterColor = new List<HeroCard>();
    private List<HeroCard> lstCardFilterType = new List<HeroCard>();
    private List<HeroCard> lstCardFilterTypeHero = new List<HeroCard>();
    private List<HeroCard> lstCardFilterName = new List<HeroCard>();
    //private List<HeroCard> listUserCardGroup = new List<HeroCard>();
    private List<CellHeroCardUser> lstUserCardGroup = new List<CellHeroCardUser>();
    private List<HeroCard> lstMainGodChoice = new List<HeroCard>();
    private const int MAX_TROOPER = 21;
    private const int MAX_GOD = 6;
    private const int MAX_GOD_CLONE = 3;
    private const int MAX_TROOPER_CLONE = 3;
    private const int MAX_LENGTH = 21;
    // public GameObject currentCardCrop;
    public bool isDrop = false;
    int countGod, countCard;
    string deckNameChecked;
    bool godOk = false, carkOk = false, mainGodOk = true;
    bool isEdit = false, isSuccess = false;
    bool canSaveMainGodAuto = false;
    bool useFilter = false, useSearch = false;
    public GameObject CurrentCardCrop = null;
    public static CustomDeckScene main;
    private string currentDeckName = null;
    int currentDeckId = 0;

    private int page = 0;
    private int totalPage = 0;
    private const int perPage = 8;

    // Methods
    public override bool ProcessSocketData(int id, byte[] data)
    {
        if (base.ProcessSocketData(id, data))
            return true;

        switch (id)
        {
            case IService.GET_USER_DECK_DETAIL:
                {
                    ListCommonVector listCommonVector = ISocket.Parse<ListCommonVector>(data);
                    ProcessGetUserDeckDetail(listCommonVector);
                    break;
                }
            case IService.SET_USER_DECK:
                {
                    string mess = "";
                    CommonVector commonVector = ISocket.Parse<CommonVector>(data);
                    isSuccess = (commonVector.aLong[0] == 1) ? true : false;
                    LogWriterHandle.WriteLog("Setuserdeck" + string.Join(",", commonVector.aLong));
                    LogWriterHandle.WriteLog("SetUserDeck" + "," + string.Join(",", commonVector.aString));
                    if (!isSuccess)
                    {
                        mess = commonVector.aString[0];
                    }
                    CheckSetUserDeck(isSuccess, mess);
                    break;
                }
            case IService.UPDATE_USER_DECK:
                {
                    string mess = "";
                    CommonVector commonVector = ISocket.Parse<CommonVector>(data);
                    LogWriterHandle.WriteLog("UpdateUserDeck" + string.Join(",", commonVector.aLong));
                    LogWriterHandle.WriteLog("UpdateUserDeck" + string.Join(",", commonVector.aString));
                    isSuccess = (commonVector.aLong[0] == 1) ? true : false;
                    if (!isSuccess)
                    {
                        mess = commonVector.aString[0];
                    }
                    CheckUpdateUserDeck(isSuccess, mess);
                    break;
                }
        }

        return false;
    }
    private void CheckSetUserDeck(bool isSuccess, string mess)
    {
        if (isSuccess)
        {
            if (string.IsNullOrEmpty(mess))
            {
                Toast.Show(LangHandler.Get("63", "Save successfully"));
            }
            else
            {
                Toast.Show(mess);
            }
            this.gameObject.SetActive(false);
            Game.main.socket.GetUserDeck();
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            //Game.main.LoadScene("CollectionScene", delay: 0.3f, curtain: true);
            m_Blur.Play();
        }
        else
        {
            if (string.IsNullOrEmpty(mess))
            {
                Toast.Show(LangHandler.Get("62", "Save failed"));
            }
            else
            {
                Toast.Show(mess);
            }

            // this.gameObject.SetActive(false);
        }
    }
    private void CheckUpdateUserDeck(bool isSuccess, string mess)
    {
        if (isSuccess)
        {
            if (string.IsNullOrEmpty(mess))
            {
                Toast.Show(LangHandler.Get("66", "Update successfully"));
            }
            else
            {
                Toast.Show(mess);
            }
            this.gameObject.SetActive(false);
            Game.main.socket.GetUserDeck();
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            //Game.main.LoadScene("CollectionScene", delay: 0.3f, curtain: true);
            m_Blur.Play();
        }
        else
        {
            if (string.IsNullOrEmpty(mess))
            {
                Toast.Show(LangHandler.Get("65", "Update failed"));
            }
            else
            {
                Toast.Show(mess);
            }
            // this.gameObject.SetActive(false);
        }
    }
    private void ProcessGetUserDeckDetail(ListCommonVector listCommonVector)
    {
        //lstUserCardGroup.Clear();
        CommonVector cv0 = listCommonVector.aVector[0];
        if (cv0.aLong[0] == 0)
        {
            Toast.Show(LangHandler.Get("59", "Can't get deck detail!"));
            //InitNewData();
        }
        else
        {
            currentDeckId = (int)cv0.aLong[1];
        }
        for (int i = 4; i < GameData.main.lstDeckInfo.Count; i++)
        {
            if (GameData.main.lstDeckInfo[i].deckID == cv0.aLong[1])
            {
                currentDeckName = GameData.main.lstDeckInfo[i].deckName;
                deckName.text = GameData.main.lstDeckInfo[i].deckName;
                break;
            }
        }
        GameData.main.lstCardCustomDeckEdit.Clear();

        CommonVector cv1 = listCommonVector.aVector[1];
        for (int i = 0; i < cv1.aLong.Count; i += 1)
        {
            // hc.id = cv1.aLong[i];
            HeroCard hc = GameData.main.lstHeroCard.FirstOrDefault(x => x.id == cv1.aLong[i]);
            GameData.main.lstCardCustomDeckEdit.Add(hc);
        }

        CommonVector cv2 = listCommonVector.aVector[2];
        for (int i = 0; i < cv2.aLong.Count; i += 1)
        {
            HeroCard hc = GameData.main.lstHeroCard.FirstOrDefault(x => x.id == cv2.aLong[i]);
            GameData.main.lstCardCustomDeckEdit.Add(hc);
        }
        InitData();
    }
    public void CheckConditionCustomDeck()
    {
        //check deck name
        if (string.IsNullOrWhiteSpace(deckName.text))
        {
            iconDeckNameOk.SetActive(false);
        }
        else
        {
            var lstDeckSameName = GameData.main.lstDeckInfo.Where(w => w.deckName == deckName.text).ToList();
            if (lstDeckSameName.Count == 0)
            {
                //ten deck hop le 
                deckNameChecked = deckName.text;
                iconDeckNameOk.SetActive(true);
            }
            else if (lstDeckSameName.Count > 0)
            {
                if (currentDeckName != null)
                {
                    if (isEdit && deckName.text == currentDeckName)
                    {
                        iconDeckNameOk.SetActive(true);
                        deckNameChecked = deckName.text;
                    }
                    else
                    {
                        iconDeckNameOk.SetActive(false);
                        deckNameChecked = deckName.text;
                    }
                }
                else
                {
                    iconDeckNameOk.SetActive(false);
                    deckNameChecked = deckName.text;
                }
            }

        }

        //check number god card 

        lstGod = lstCardCustomDeck.Where(w => w.GetDatabase().type == DBHero.TYPE_GOD).ToList();
        var lstGodGroupByName = lstGod.GroupBy(g => g.GetDatabase().heroNumber).Select(grp => grp.ToList()).ToList();
        countGod = lstGod.Count;
        numberGod.text = countGod.ToString();

        if (countGod > MAX_GOD || countGod == 0)
        {
            numberGod.color = Color.red;
            godOk = false;
        }
        else
        {
            godOk = true;
            numberGod.color = Color.green;
        }
        if (lstGodGroupByName.Count > 2)
        {
            var lst = lstCardDeckGroup.Where(w => w.lst[0].GetDatabase().type == DBHero.TYPE_GOD).ToList();

            foreach (CellDeckCard cellDeckCard in lstCardDeckGroup)
            {
                if (cellDeckCard.lst[0].GetDatabase().type == DBHero.TYPE_GOD)
                {
                    cellDeckCard.check4God = false;
                }
                else
                    cellDeckCard.check4God = true;
            }
            Toast.Show(LangHandler.Get("84", "Max 2 distinct gods"));
        }
        else
        {
            foreach (CellDeckCard cellDeckCard in lstCardDeckGroup)
            {
                cellDeckCard.check4God = true;
            }
        }

        //check number trooper card

        lstTrooper = lstCardCustomDeck.Where(w => w.GetDatabase().type != DBHero.TYPE_GOD).ToList();
        countCard = lstTrooper.Count + lstGod.Count;
        numberCard.text = countCard.ToString();
        if (countCard > MAX_TROOPER || countCard < 21)
        {
            carkOk = false;
            numberCard.color = Color.red;
        }
        else
        {
            carkOk = true;
            numberCard.color = Color.green;
        }
        //check max clone card 
        foreach (CellDeckCard cellDeckCard in lstCardDeckGroup)
        {
            if (cellDeckCard.lst[0].GetDatabase().type != DBHero.TYPE_GOD)
            {
                if (cellDeckCard.lst.Count > 2)
                    cellDeckCard.checkMaxCard = false;
                else
                    cellDeckCard.checkMaxCard = true;
            }
        }
        //check main god
        //Check da chon main god chua
        var groupHeroNumber = lstGod.GroupBy(g => g.GetDatabase().heroNumber).Select(grp => grp.ToList()).ToList();
        groupHeroNumber.ForEach(g =>
        {
            if (g.Count > 3)
            {
                foreach (CellDeckCard cellDeckCard in lstCardDeckGroup)
                {
                    if (cellDeckCard.lst[0].GetDatabase().heroNumber == g[0].GetDatabase().heroNumber && cellDeckCard.lst[0].GetDatabase().type == DBHero.TYPE_GOD)
                    {
                        cellDeckCard.checkMaxCard = false;
                    }
                }
            }
            else
            {
                foreach (CellDeckCard cellDeckCard in lstCardDeckGroup)
                {
                    if (cellDeckCard.lst[0].GetDatabase().heroNumber == g[0].GetDatabase().heroNumber && cellDeckCard.lst[0].GetDatabase().type == DBHero.TYPE_GOD)
                    {
                        cellDeckCard.checkMaxCard = true;
                    }
                }
            }
        });

        //check buff card da co than tuong ung chua 
        foreach (CellDeckCard cellDeckCard in lstCardDeckGroup)
        {
            if (cellDeckCard.lst[0].GetDatabase().type == DBHero.TYPE_BUFF_MAGIC)
            {
                cellDeckCard.checkBuffCard = false;
                foreach (HeroCard hc in lstGod)
                {
                    if (hc.heroId == cellDeckCard.lst[0].GetDatabase().ownerGodID)
                        cellDeckCard.checkBuffCard = true;
                    
                }    
            }    
            else
            {
                cellDeckCard.checkBuffCard = true;
            }    
        }

            //CheckMainGodOk();
            if (lstCardCustomDeck.Count > MAX_LENGTH)
        {
            Toast.Show(LangHandler.Get("61", "Max 21 battle cards in one deck"));
        }
    }
    private void CheckConditionCard()
    {
        foreach (HeroCard hc in lstCardCustomDeck)
        {
            if (hc.GetDatabase().type != DBHero.TYPE_GOD)
            {

                var findGodColor = lstGod.FirstOrDefault(x => x.GetDatabase().color == hc.GetDatabase().color);
                if (findGodColor == null)
                    lstCardDeckGroup.ForEach(g =>
                    {
                        if (g.lst[0].GetDatabase().type != DBHero.TYPE_GOD && g.lst[0].GetDatabase().color == hc.GetDatabase().color)
                        {
                            g.checkTrooper = false;
                        }
                    });
                if (hc.GetDatabase().color == DBHero.COLOR_WHITE)
                {
                    lstCardDeckGroup.ForEach(g =>
                    {
                        if (g.lst[0].GetDatabase().color == DBHero.COLOR_WHITE)
                        {
                            g.checkTrooper = true;
                        }
                    });
                }
            }
            else
            {
                if (hc.heroId == 153)
                {
                    lstCardDeckGroup.ForEach(g =>
                    {
                        if (g.lst[0].GetDatabase().speciesId == 5)
                        {
                            g.checkTrooper = true;
                        }
                    });
                }
                lstCardDeckGroup.ForEach(g =>
                {
                    if (g.lst[0].GetDatabase().type != DBHero.TYPE_GOD && g.lst[0].GetDatabase().color == hc.GetDatabase().color)
                    {
                        g.checkTrooper = true;
                    }
                });
            }
        }

    }
    private void SortCardCustomDeck()
    {
        lstCardDeckGroup = lstCardDeckGroup.OrderBy(g => g.lst[0].GetDatabase().type).ToList();
    }
    public void DoClickSave()
    {
        if (string.IsNullOrWhiteSpace(deckName.text))
        {
            Toast.Show(LangHandler.Get("83", "Deck name is empty! Please fill your deck name!"));
            return;
        }

        Color godColor = godOk ? Color.green : Color.red;
        Color cardColor = carkOk ? Color.green : Color.red;
        //Color mainGodColor = mainGodOk ? Color.green : Color.red;
        if (godOk && carkOk)
        {
            PopUpCustomDeckConfirm.Show(
                content: "\"" + deckName.text.ToString() + "\"" + LangHandler.Get("73", "meets all requirements. Confirm save this deck?"),
                title: LangHandler.Get("stt-38", "NOTICE"),
                countGod: numberGod.text,
                god: godColor,
                countCard: numberCard.text,
                card: cardColor,
                action1: LangHandler.Get("but-23", "CONFIRM"),
                action2: "",
                false,
                action1Callback: go =>
                {
                    DoSaveCustomDeck();
                    go.SetActive(false);
                }
                );
        }
        else
        {
            PopUpCustomDeckConfirm.Show(
            content: "\"" + deckName.text.ToString() + "\"" + LangHandler.Get("74", " does not meets all requirements and will not be playable. Confirm save this deck?"),
            title: LangHandler.Get("stt-38", "NOTICE"),
            countGod: numberGod.text,
            god: godColor,
            countCard: numberCard.text,
            card: cardColor,
            action1: LangHandler.Get("but-13", "SAVE"),
            action2: LangHandler.Get("87", "RESUME EDITING"),
            false,
            action1Callback: go =>
            {
                DoSaveCustomDeck();
            },
            action2Callback: go =>
             {
                 go.SetActive(false);
             }
            );

        }
    }
    public void DoClickClose()
    {
        PopupConfirm.Show(
        content: "\"" + deckName.text.ToString() + "\"" + " " + LangHandler.Get("86", " is editing. Confirm to quit?"),
        title: LangHandler.Get("stt-38", "NOTICE"),
        action1: LangHandler.Get("87", "RESUME"),
        action2: LangHandler.Get("88", "QUIT"),
        false,
        action1Callback: go =>
        {
            go.SetActive(false);
        },
        action2Callback: go =>
        {
            Game.main.socket.GetUserDeck();

            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            m_Blur.Play();
        }
        );
    }

    public void DoClickShowStats()
    {
        Toast.Show(LangHandler.Get("toast-8", "Coming Soon!"));
    }
    public void DoClickShowOwned()
    {
        Toast.Show(LangHandler.Get("toast-8", "Coming Soon!"));
    }
    private void DoSaveCustomDeck()
    {
        void AddToDict(Dictionary<long, long> target, long id, bool mainGod = false)
        {
            long isMain = (mainGod == true) ? 1 : 0;
            if (target.ContainsKey(id))
            {
                LogWriterHandle.WriteLog("Wrong Data");
            }
            else
                target.Add(id, isMain);

        }
        void GetFromDict(Dictionary<long, long> target, List<long> output)
        {
            foreach (KeyValuePair<long, long> pair in target)
            {
                output.Add(pair.Key);
                output.Add(pair.Value);
            }
        }
        Dictionary<long, long> dictGod = new Dictionary<long, long>();
        List<long> lstGod = new List<long>();
        List<long> lstTrooper = new List<long>();
        string deckName = deckNameChecked;
        int max = 6, count = 0;
        lstTrooper.Clear();
        foreach (CellHeroCardUser cell in lstUserCardGroup)
        {
            foreach (HeroCard hc in cell.lstHeroCardIDNow)
            {
                DBHero db = hc.GetDatabase();
                if (db != null)
                {
                    if (db.type == DBHero.TYPE_GOD)
                    {
                        lstGod.Add(hc.id);
                    }
                    else
                    {
                        lstTrooper.Add(hc.id);
                    }
                }
            }

        }
        if (lstGod.Count > 0 || lstTrooper.Count > 0)
        {
            if (isEdit)
            {
                //LogWriterHandle.WriteLog(GameData.main.lstDeckInfo[PlayerPrefs.GetInt("CurrentDeck")].deckID);
                if (currentDeckId != 0)
                    Game.main.socket.UpdateUserDeck(lstGod, lstTrooper, deckName, currentDeckId);
                else
                    Toast.Show("Deck id null");
            }
            else if (isEdit == false)
            {
                Game.main.socket.SetUserCustomDeck(lstGod, lstTrooper, deckName);
            }

        }
        else if (lstTrooper.Count == 0 && lstGod.Count == 0)
        {
            Toast.Show(LangHandler.Get("60", "Need at least 1 card to save customdeck"));
        }
    }
    private void DoAddCardDeck(HeroCard hc, GameObject go)
    {
        CellDeckCard targetCell = new CellDeckCard();
        if (hc == null)
            return;
        DBHero db = hc.GetDatabase();
        if (db != null)
        {
            lstUserCard.Remove(hc);
            lstCardCustomDeck.Add(hc);
            SetCardFilter();

            bool add = false;

            lstCardDeckGroup.ForEach(g =>
            {
                if (hc.heroId == g.lst[0].heroId)
                {
                    g.lst.Add(hc);
                    add = true;
                    targetCell = g;

                }

            });
            if (add == false)
            {
                CellDeckCard cell = new CellDeckCard();
                cell.lst.Add(hc);
                lstCardDeckGroup.Add(cell);
                targetCell = cell;
            }
            lstUserCardGroup.ForEach(g =>
            {
                if (hc.heroId == g.lst[0].heroId && hc.cardType == g.lst[0].cardType)
                {
                    g.lstHeroCardIDNow.Add(hc);
                }

            });

            if (hc.GetDatabase().type != DBHero.TYPE_GOD)
            {

                var findGodColor = lstGod.FirstOrDefault(x => x.GetDatabase().color == hc.GetDatabase().color);
                if (findGodColor == null)
                    lstCardDeckGroup.ForEach(g =>
                    {
                        if (g.lst[0].GetDatabase().type != DBHero.TYPE_GOD && g.lst[0].GetDatabase().color == hc.GetDatabase().color)
                        {
                            g.checkTrooper = false;
                        }
                        // else if(g.lst[0].GetDatabase().color == DBHero.COLOR_WHITE) g.checkTrooper = true;

                    });
                if (hc.GetDatabase().color == DBHero.COLOR_WHITE)
                {
                    lstCardDeckGroup.ForEach(g =>
                    {
                        if (g.lst[0].GetDatabase().color == DBHero.COLOR_WHITE)
                        {
                            g.checkTrooper = true;
                        }
                    });
                }
                if (hc.GetDatabase().speciesId == 5)
                {
                    var findZodiagDog = lstGod.FirstOrDefault(x => x.heroId == 153);
                    if (findZodiagDog != null)
                    {
                        lstCardDeckGroup.ForEach(g =>
                        {
                            if (g.lst[0].GetDatabase().speciesId == 5)
                            {
                                g.checkTrooper = true;
                            }

                        });
                    }
                    else
                    {
                        lstCardDeckGroup.ForEach(g =>
                        {
                            if (g.lst[0].GetDatabase().speciesId == 5)
                            {
                                g.checkTrooper = false;
                            }

                        });
                    }
                }


            }
            else
            {
                if (hc.heroId == 153)
                {
                    lstCardDeckGroup.ForEach(g =>
                   {
                       if (g.lst[0].GetDatabase().speciesId == 5)
                       {
                           g.checkTrooper = true;
                       }
                   });
                }
                lstCardDeckGroup.ForEach(g =>
                {
                    if (g.lst[0].GetDatabase().type != DBHero.TYPE_GOD && g.lst[0].GetDatabase().color == hc.GetDatabase().color)
                    {
                        g.checkTrooper = true;
                    }
                });

            }
        }

        CheckConditionCustomDeck();
        //if (useFilter)
        //{
        //    poolLstUserCard.SetAdapter(DoFilter(), false);
        //}
        //else if (useSearch)
        //    poolLstUserCard.SetAdapter(FilterCardName(searchText), false);
        //else
        //    poolLstUserCard.SetAdapter(lstUserCardGroup, false);
        SetPageData(page);
        if (targetCell != null)
        {
            int index = poolLstCardCustomDeck.GetDataIndex(targetCell);
            GameObject card = poolLstCardCustomDeck.GetGameObject(index);
            if (card != null)
            {
                CurrentCardCrop = card;
                if (card.activeSelf == true)
                {
                    go.GetComponent<RectTransform>().DOMove(CurrentCardCrop.GetComponent<RectTransform>().transform.position, 0.5f, false).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        CurrentCardCrop = null;
                        SortCardCustomDeck();
                        poolLstCardCustomDeck.SetAdapter(lstCardDeckGroup, false);

                    });
                }
                else
                {
                    go.GetComponent<RectTransform>().DOMove(CurrentCardCrop.GetComponent<RectTransform>().transform.position, 0.5f, false).OnComplete(() =>
                    {
                        CurrentCardCrop = null;
                        SortCardCustomDeck();
                        poolLstCardCustomDeck.SetAdapter(lstCardDeckGroup, false);
                    });
                }

            }
            else
            {
                SortCardCustomDeck();
                poolLstCardCustomDeck.SetAdapter(lstCardDeckGroup, false);
                int index2 = poolLstCardCustomDeck.GetDataIndex(targetCell);
                GameObject card2 = poolLstCardCustomDeck.GetGameObject(index2);
                if (card2 != null)
                {
                    CurrentCardCrop = card2;
                    if (card2.activeSelf)
                    {
                        card2.SetActive(false);
                        go.GetComponent<RectTransform>().DOMove(CurrentCardCrop.GetComponent<RectTransform>().transform.position, 0.5f, false).OnComplete(() =>

                        {
                            card2.SetActive(true);

                        });
                    }
                    CurrentCardCrop = null;
                }
                else
                {
                    go.GetComponent<RectTransform>().DOMoveX(poolLstCardCustomDeck.gameObject.transform.position.x, 0.5f, false);
                }
            }
        }
    }
    private void DoRemoveCardFromNewDeck(HeroCard hc)
    {
        lstUserCard.Add(hc);
        lstCardCustomDeck.Remove(hc);
        SetCardFilter();
        for (int i = lstCardDeckGroup.Count - 1; i >= 0; i--)
        {
            CellDeckCard data = lstCardDeckGroup[i];
            if (hc.heroId == data.lst[0].heroId)
            {
                if (data.lst.Count == 1)
                {
                    lstCardDeckGroup.RemoveAt(i);

                    SortCardCustomDeck();
                    CheckConditionCustomDeck();
                    poolLstCardCustomDeck.SetAdapter(lstCardDeckGroup, false);
                }

                else
                    data.lst.Remove(hc);
            }
        }

        lstUserCardGroup.ForEach(g =>
        {
            if (hc.heroId == g.lst[0].heroId && hc.cardType == g.lst[0].cardType)
            {
                g.lstHeroCardIDNow.Remove(hc);
            }
        });
        if (hc.GetDatabase().type == DBHero.TYPE_GOD)
        {
            var findGodColor = lstCardCustomDeck.FirstOrDefault(x => x.GetDatabase().color == hc.GetDatabase().color && x.GetDatabase().type == DBHero.TYPE_GOD);
            if (findGodColor == null)
            {
                lstCardDeckGroup.ForEach(g =>
                {
                    if (g.lst[0].GetDatabase().type != DBHero.TYPE_GOD && g.lst[0].GetDatabase().color == hc.GetDatabase().color)
                    {
                        g.checkTrooper = false;
                    }
                });
            }
            if (hc.heroId == 153)
            {
                lstCardDeckGroup.ForEach(g =>
                {
                    if (g.lst[0].GetDatabase().speciesId == 5)
                    {
                        g.checkTrooper = false;
                    }
                });
            }
        }
        CheckConditionCustomDeck();
        //if (useFilter)
        //    poolLstUserCard.SetAdapter(DoFilter(), false);
        //else if (useSearch)
        //    poolLstUserCard.SetAdapter(FilterCardName(searchText), false);
        //else
        //    poolLstUserCard.SetAdapter(lstUserCardGroup, false);
        SetPageData(page);

        SortCardCustomDeck();
        poolLstCardCustomDeck.SetAdapter(lstCardDeckGroup, false);
        //SetMainGodAuto();
        //CheckMainGodOk();
    }
    protected override void Awake()
    {
        CustomDeckScene.main = this;
        base.Awake();
        //InitNewData();
        CheckConditionCustomDeck();
        poolLstCardCustomDeck
            .SetCellDataCallback((GameObject go, CellDeckCard data, int index) =>
            {
                UserCardUI script = go.GetComponent<UserCardUI>();

                script.SetOnClickCallback(lstHc =>
                {
                    ShowPreviewHandCard(lstHc.lst[0].heroId, lstHc.lst[0].frame);
                });
                script.SetOnEndDragCallback(lstHc =>
                {
                    DoRemoveCardFromNewDeck(lstHc.lst[lstHc.lst.Count - 1]);
                });
                script.InitData(data, poolLstCardCustomDeck.scrollRect);
            }).SetCellPrefabCallback((index) =>
            {
                CellDeckCard data = poolLstCardCustomDeck.GetCellDataAt<CellDeckCard>(index);
                if (data.lst[0].GetDatabase().type == DBHero.TYPE_GOD)
                {
                    return poolLstCardCustomDeck.GetDeclarePrefab(1);
                }
                return poolLstCardCustomDeck.GetDeclarePrefab(0);
            });

        poolLstCardCustomDeck.SetCellSizeCallback((index) =>
        {
            CellDeckCard data = poolLstCardCustomDeck.GetCellDataAt<CellDeckCard>(index);
            if (data.lst[0].GetDatabase().type == DBHero.TYPE_GOD)
            {
                return new Vector2(530, 113);
            }
            return new Vector2(530, 90);
        });


        m_PoolListUserCard.SetCellDataCallback((GameObject go, CellHeroCardUser data, int index) =>
        {
            CellHeroCard script = go.GetComponent<CellHeroCard>();
            script.SetData(data, index);
            script.SetOnClickCallback(hc =>
            {
                int countHC = data.lst.Count - data.lstHeroCardIDNow.Count;
                int idx = countHC != 0 ? data.lst.Count - countHC : 0;
                ShowPreviewHandCard(hc.lst[0].heroId, hc.lst[idx].frame);
            });
            script.SetOnBeginDragCallBack(() =>
            {
                m_ListCardHightlight.SetActive(true);
            });
            script.SetOnEndDragCallback((lstHc, cardGo, move) =>
            {
                if (move)
                    DoAddCardDeck(lstHc.lst[lstHc.lstHeroCardIDNow.Count()], cardGo);
                m_ListCardHightlight.SetActive(false);
                //DoAddCardDeck(lstHc.lst[0], cardGo);
            });
        });

        m_BoxPage
            .SetOnNext(() => SetPageData(page + 1, true, true))
            .SetOnPrev(() => SetPageData(page - 1, false, true));

        m_SwipeRewardPopup.SetOnSwipeStopCallback((index) =>
        {

        });

    }

    public void SetPageData(int pageId, bool isNext = true, bool isSetAnimation = false)
    {
        page = pageId;
        List<CellHeroCardUser> lstAdapter = new List<CellHeroCardUser>();
        if (useFilter)
        {
            lstAdapter = DoFilter();
        }
        else if (useSearch)
            lstAdapter = FilterCardName(searchText);
        else
        {
            lstAdapter = lstUserCardGroup;
        }
        totalPage = (int)Math.Ceiling((double)lstAdapter.Count / perPage) - 1;
        m_BoxPage.Set(page, page < totalPage);
        CanvasGroup cg = m_PoolListUserCard.GetComponent<CanvasGroup>();
        RectTransform rtf = m_PoolListUserCard.GetComponent<RectTransform>();
        int range = (lstAdapter.Count - page * perPage) > 8 ? 8 : (lstAdapter.Count - page * perPage);
        if (isSetAnimation)
        {
            for (int i = 0; i < m_PoolListUserCard.group.childCount; i++)
            {
                m_PoolListUserCard.group.GetChild(i).GetComponent<CellHeroCard>().TurnCardPowerFrame(false);
            }
            if (isNext)
            {
                cg.DOFade(0f, 0.2f).OnComplete(() =>
                {
                    rtf.anchoredPosition = new Vector2(1560, 0);
                    cg.alpha = 1;
                    m_PoolListUserCard.SetAdapter(lstAdapter.GetRange(page * perPage, range));
                    rtf.DOAnchorPosX(0f, 0.2f).SetEase(Ease.OutCubic);
                });
            }
            else
            {
                rtf.DOAnchorPosX(1560f, 0.2f).SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    cg.alpha = 0;
                    rtf.anchoredPosition = new Vector2(0, 0);
                    m_PoolListUserCard.SetAdapter(lstAdapter.GetRange(page * perPage, perPage));
                    cg.DOFade(1f, 0.2f).SetEase(Ease.OutCubic);
                });
            }
        }
        else
        {
            m_PoolListUserCard.SetAdapter(lstAdapter.GetRange(page * perPage, range));
        }
    }

    private void OnEnable()
    {
        InitNewData();
        CheckConditionCustomDeck();
        ClearFilter();
    }

    private void InitData()
    {
        lstCardCustomDeck.Clear();
        lstCardDeckGroup.Clear();
        foreach (CellHeroCardUser cellHero in lstUserCardGroup)
        {
            cellHero.lstHeroCardIDNow.Clear();
        }
        isEdit = true;
        //deckName.text = GameData.main.lstDeckInfo[4].deckName;
        lstCardCustomDeck.AddRange(GameData.main.lstCardCustomDeckEdit);
        var groupCard1 = lstCardCustomDeck.GroupBy(g => g.heroId).Select(grp => grp.ToList()).ToList();
        groupCard1.ForEach(g =>
        {
            CellDeckCard cellDeck = new CellDeckCard();
            cellDeck.lst.AddRange(g);
            lstCardDeckGroup.Add(cellDeck);
            foreach (CellHeroCardUser cellHero in lstUserCardGroup)
            {
                foreach (HeroCard hc in g)
                {
                    if (cellHero.lst[0].heroId == hc.heroId && cellHero.lst[0].cardType == hc.cardType)
                    {
                        cellHero.lstHeroCardIDNow.Add(hc);
                        lstUserCard.Remove(hc);
                    }

                }
            }
        });
        SetCardFilter();
        CheckConditionCustomDeck();
        CheckConditionCard();
        //if (useFilter)
        //{
        //    poolLstUserCard.SetAdapter(DoFilter(), false);
        //}
        //else if (useSearch)
        //    poolLstUserCard.SetAdapter(FilterCardName(searchText), false);
        //else
        //    poolLstUserCard.SetAdapter(lstUserCardGroup, false);
        // poolLstUserCard.SetAdapter(lstUserCardGroup);
        SetPageData(0);

        SortCardCustomDeck();
        poolLstCardCustomDeck.SetAdapter(lstCardDeckGroup);
    }

    private void ResetData()
    {
        isEdit = false;
        isSuccess = false;
        godOk = false;
        carkOk = false;
        mainGodOk = true;
        currentDeckId = 0;
        countGod = 0;
        countCard = 0;
        isDrop = false;
        deckNameChecked = "";
        lstUserCardGroup.Clear();
        lstCardDeckGroup.Clear();
        lstGod.Clear();
        lstTrooper.Clear();
        lstMainGod.Clear();
        lstUserCard.Clear();
        lstCardCustomDeck.Clear();
        lstGodFilter.Clear();
        lstMortalFilter.Clear();
        lstSpellFilter.Clear();
        lstCardFilterColor.Clear();
        lstCardFilterType.Clear();
        lstCardFilterTypeHero.Clear();
        lstCardFilterName.Clear();
        deckName.text = "";

        page = 0;
        totalPage = 0;

        CheckConditionCustomDeck();
        CheckConditionCard();
    }
    private void InitNewData()
    {
        ResetData();
        //lang
        deckName.text = LangHandler.Get("111", "New Deck ") + (GameData.main.lstDeckInfo.Count - 4 + 1); // 4 default deck
        List<HeroCard> lstGod = new List<HeroCard>();
        List<HeroCard> lstTrooperMagic = new List<HeroCard>();
        List<HeroCard> lstBuffMagic= new List<HeroCard>();
        List<HeroCard> lstTrooperNormal = new List<HeroCard>();

        foreach (HeroCard hc in GameData.main.lstHeroCard)
        {
            if (hc.heroId != 117)
            {
                DBHero db = hc.GetDatabase();

                
                if (db != null)
                {
                    if (db.type == DBHero.TYPE_GOD) lstGod.Add(hc);
                    else if (db.type == DBHero.TYPE_TROOPER_MAGIC) lstTrooperMagic.Add(hc);
                    else if (db.type == DBHero.TYPE_BUFF_MAGIC) lstBuffMagic.Add(hc);
                    else if (db.type == DBHero.TYPE_TROOPER_NORMAL) lstTrooperNormal.Add(hc);

                }
            }
        }

        lstUserCard.Clear();
        lstUserCard.AddRange(lstGod);
        lstUserCard.AddRange(lstBuffMagic);
        lstUserCard.AddRange(lstTrooperMagic);
        lstUserCard.AddRange(lstTrooperNormal);

        //filter lst card
        SetCardFilter();
        //var groupCard = lstUserCard.GroupBy(g => g.heroId).Select(grp => grp.ToList()).ToList();
        var groupCard = lstUserCard.GroupBy(g => g.heroId).Select(grp => grp.ToList()).ToList();

        groupCard.ForEach(g =>
        {
            var groupFilter = g.GroupBy(x => x.cardType).Select(grp => grp.ToList()).ToList();
            groupFilter.ForEach(gFilter =>
            {
                CellHeroCardUser cell = new CellHeroCardUser();
                cell.lst.AddRange(gFilter);
                cell.lst.Sort((x, y) => y.frame.CompareTo(x.frame));
                cell.heroId = (int)cell.lst[0].heroId;
                lstUserCardGroup.Add(cell);
            });
        });
        //poolLstUserCard.SetAdapter(lstUserCardGroup);
        List<long> listOrder = new List<long>
            {
                DBHero.COLOR_RED,
                DBHero.COLOR_GREEN,
                DBHero.COLOR_PURPLE,
                DBHero.COLOR_YELLOW,
                DBHero.COLOR_WHITE
            };
        lstUserCardGroup = lstUserCardGroup.OrderBy(x => x.lst[0].GetDatabase().type)
            .ThenBy(x => listOrder.IndexOf(x.lst[0].GetDatabase().color))
            .ThenBy(x => x.lst[0].GetDatabase().mana)
            .ThenBy(x => x.lst[0].GetDatabase().name)
            .ToList();

        SortCardCustomDeck();
        poolLstCardCustomDeck.SetAdapter(lstCardDeckGroup);
        //SetPageData(0);

        //iconMode.sprite = modes[0];
        //txtMode.text = "Normal";
        //iconEvent.SetActive(false);
        //poolLstUserCard.SetAdapter(lstUserCardGroup);

        SetPageData(0);

    }
    private void SetCardFilter()
    {
        lstGreen = lstUserCard.Where(w => w.GetDatabase().color == DBHero.COLOR_GREEN).ToList();
        lstRed = lstUserCard.Where(w => w.GetDatabase().color == DBHero.COLOR_RED).ToList();
        lstYellow = lstUserCard.Where(w => w.GetDatabase().color == DBHero.COLOR_YELLOW).ToList();
        lstPurple = lstUserCard.Where(w => w.GetDatabase().color == DBHero.COLOR_PURPLE).ToList();
        lstWhite = lstUserCard.Where(w => w.GetDatabase().color == DBHero.COLOR_WHITE).ToList();
        lstNFT = lstUserCard.Where(w => w.cardType == HeroCard.CardType.nft).ToList();
        lstNonNFT = lstUserCard.Where(w => w.cardType == HeroCard.CardType.normal).ToList();
        lstGodFilter = lstUserCard.Where(w => w.GetDatabase().type == DBHero.TYPE_GOD).ToList();
        lstMortalFilter = lstUserCard.Where(w => w.GetDatabase().type == DBHero.TYPE_TROOPER_NORMAL).ToList();
        lstSpellFilter = lstUserCard.Where(w => w.GetDatabase().type == DBHero.TYPE_TROOPER_MAGIC || w.GetDatabase().type == DBHero.TYPE_BUFF_MAGIC).ToList();
    }
    public void ShowPreviewHandCard(long id, long frame)
    {
        if (cardPreview.activeSelf)
            return;

        cardPreview.SetActive(true);
        DBHero hero = Database.GetHero(id);
        godCardPreview.gameObject.SetActive(false);
        spellCardPreview.gameObject.SetActive(false);
        minionCardPreview.gameObject.SetActive(false);
        if (hero.type == DBHero.TYPE_GOD)
        {
            godCardPreview.gameObject.SetActive(true);
            godCardPreview.SetCardPreview(hero, frame, true);
        }
        if (hero.type == DBHero.TYPE_TROOPER_MAGIC|| hero.type == DBHero.TYPE_BUFF_MAGIC)
        {
            spellCardPreview.gameObject.SetActive(true);
            spellCardPreview.SetCardPreview(hero, frame, true);
        }
        if (hero.type == DBHero.TYPE_TROOPER_NORMAL)
        {
            minionCardPreview.gameObject.SetActive(true);
            minionCardPreview.SetCardPreview(hero, frame, true);
        }
    }

    public void DoClickSearchButton()
    {
        if (searchField.activeSelf == false)
        {
            searchField.SetActive(true);
            useFilter = false;
        }
        else
        {
            var lst = FilterCardName(searchText);
            if (lst == null)
            {
                useSearch = false;
                searchField.SetActive(true);
                //poolLstUserCard.SetAdapter(lstUserCardGroup);
                SetPageData(0);
            }
            else if (lst.Count() == 0)
            {
                SetPageData(0);
                //poolLstUserCard.SetAdapter(lst);
                Toast.Show(LangHandler.Get("64", "There are no cards matching your search"));
            }
            else
            {
                useSearch = true;
                //poolLstUserCard.SetAdapter(lst);
                SetPageData(0);
            }
        }
    }
    public void DoSearchByName()
    {
        if (searchText.text.Length > 2)
        {
            List<CellHeroCardUser> lst = FilterCardName(searchText);
            if (lst == null)
            {
                useSearch = false;
                searchField.SetActive(true);
                //poolLstUserCard.SetAdapter(lstUserCardGroup);
                SetPageData(0);
            }
            else if (lst.Count == 0)
            {
                //poolLstUserCard.SetAdapter(lst);
                SetPageData(0);
                Toast.Show(LangHandler.Get("64", "There are no cards matching your search"));
            }
            else
            {
                useSearch = true;
                //poolLstUserCard.SetAdapter(lst);
                SetPageData(0);
            }
        }
        else
        {
            useSearch = false;
            SetPageData(0);
        }
    }
    private List<CellHeroCardUser> FilterCardName(TMP_InputField searchName)
    {
        if (!string.IsNullOrEmpty(searchName.text))
        {
            List<CellHeroCardUser> lstReturn = new List<CellHeroCardUser>();
            lstReturn = lstUserCardGroup.Where(g => g.lst[0].GetDatabase().name.ToLower().Contains(searchName.text.ToLower())).ToList();
            List<long> listOrder = new List<long>
            {
                DBHero.COLOR_RED,
                DBHero.COLOR_GREEN,
                DBHero.COLOR_PURPLE,
                DBHero.COLOR_YELLOW,
                DBHero.COLOR_WHITE
            };
            lstReturn = lstReturn.OrderBy(x => x.lst[0].GetDatabase().type)
                .ThenBy(x => listOrder.IndexOf(x.lst[0].GetDatabase().color))
                .ThenBy(x => x.lst[0].GetDatabase().mana)
                .ThenBy(x => x.lst[0].GetDatabase().name)
                .ToList();
            return lstReturn;
        }
        else
            return lstUserCardGroup;
    }
    public void ClosePreviewHandCard()
    {
        if (cardPreview.gameObject.activeSelf)
        {
            godCardPreview.gameObject.SetActive(false);
            minionCardPreview.gameObject.SetActive(false);
            cardPreview.gameObject.SetActive(false);
            spellCardPreview.gameObject.SetActive(false);
        }
    }
    public void NftToggleValueChanged(Toggle toggle)
    {
        if (toggle.isOn)
        {
            nonNftToggle.isOn = false;
        }
        nftView.SetActive(toggle.isOn);
    }
    public void NonNftToggleValueChanged(Toggle toggle)
    {
        if (toggle.isOn)
        {
            nftToggle.isOn = false;
        }
        nonNftView.SetActive(toggle.isOn);
    }
    public void GreenToggleValueChanged(Toggle toggle)
    {
        if (toggle.isOn)
            greenView.SetActive(true);
        else
            greenView.SetActive(false);
    }
    public void RedToggleValueChanged(Toggle toggle)
    {

        if (toggle.isOn)
            redView.SetActive(true);
        else
            redView.SetActive(false);
    }
    public void YellowToggleValueChanged(Toggle toggle)
    {
        if (toggle.isOn)
            yellowView.SetActive(true);
        else
            yellowView.SetActive(false);
    }
    public void PurpleToggleValueChanged(Toggle toggle)
    {
        if (toggle.isOn)
            purpleView.SetActive(true);
        else
            purpleView.SetActive(false);
    }
    public void WhiteToggleValueChanged(Toggle toggle)
    {
        if (toggle.isOn)
            whiteView.SetActive(true);
        else
            whiteView.SetActive(false);
    }
    public void GodToggleValueChanged(Toggle toggle)
    {
        if (toggle.isOn)
        {
            mortalToggle.isOn = false;
            spellToggle.isOn = false;
        }
        godView.SetActive(toggle.isOn);
    }
    public void MortalToggleValueChanged(Toggle toggle)
    {
        if (toggle.isOn)
        {
            godToggle.isOn = false;
            spellToggle.isOn = false;
        }
        mortalView.SetActive(toggle.isOn);
    }
    public void SpellToggleValueChanged(Toggle toggle)
    {
        if (toggle.isOn)
        {
            mortalToggle.isOn = false;
            godToggle.isOn = false;
        }
        spellView.SetActive(toggle.isOn);
    }

    private void CheckUseFilter()
    {
        if (!greenToggle.isOn && !redToggle.isOn && !yellowToggle.isOn && !purpleToggle.isOn && !whiteToggle.isOn && !nftToggle.isOn && !nonNftToggle.isOn && !godToggle.isOn && !mortalToggle.isOn && !spellToggle.isOn)
        {
            useFilter = false;
            //searchButton.SetActive(true);
        }
        else
        {
            useFilter = true;
            searchButton.SetActive(false);
        }
    }
    //Filter
    public void DoClickUseFilter()
    {
        // mo popup
        // CheckUseFilter();
        filterUse.SetActive(true);
        filterNotUse.SetActive(false);
        searchButton.SetActive(false);
        filterFrameUI.SetActive(true);

    }
    private void ClearFilter()
    {
        useFilter = false;
        greenToggle.isOn = false;
        redToggle.isOn = false;
        yellowToggle.isOn = false;
        purpleToggle.isOn = false;
        whiteToggle.isOn = false;
        nftToggle.isOn = false;
        nonNftToggle.isOn = false;
        godToggle.isOn = false;
        mortalToggle.isOn = false;
        spellToggle.isOn = false;
        // poolLstUserCard.SetAdapter(lstUserCardGroup);
        //button filer ve not use
    }
    public void DoClickCleareFilter()
    {
        ClearFilter();
        CheckUseFilter();
        filterUse.SetActive(false);
        filterNotUse.SetActive(true);
        filterFrameUI.SetActive(false);
        //poolLstUserCard.SetAdapter(lstUserCardGroup);
        SetPageData(0);
    }

    public void DoClickOutFilterUI()
    {
        CheckUseFilter();
        if (useFilter)
        {
            //poolLstUserCard.SetAdapter(DoFilter());
            filterUse.SetActive(true);
            filterNotUse.SetActive(false);
        }
        else
        {
            filterUse.SetActive(false);
            filterNotUse.SetActive(true);
            //poolLstUserCard.SetAdapter(lstUserCardGroup);
        }
        SetPageData(0);
        filterFrameUI.SetActive(false);
    }
    public List<CellHeroCardUser> DoFilter()
    {
        List<CellHeroCardUser> lstReturn = new List<CellHeroCardUser>();

        List<CellHeroCardUser> lstGreen = new List<CellHeroCardUser>();
        List<CellHeroCardUser> lstRed = new List<CellHeroCardUser>();
        List<CellHeroCardUser> lstYellow = new List<CellHeroCardUser>();
        List<CellHeroCardUser> lstPurple = new List<CellHeroCardUser>();
        List<CellHeroCardUser> lstWhite = new List<CellHeroCardUser>();
        List<CellHeroCardUser> lstNft = new List<CellHeroCardUser>();
        List<CellHeroCardUser> lstNonNft = new List<CellHeroCardUser>();
        List<CellHeroCardUser> lstGod = new List<CellHeroCardUser>();
        List<CellHeroCardUser> lstMortal = new List<CellHeroCardUser>();
        List<CellHeroCardUser> lstSpell = new List<CellHeroCardUser>();

        List<CellHeroCardUser> lstCardByColor = new List<CellHeroCardUser>();
        List<CellHeroCardUser> lstCardByType = new List<CellHeroCardUser>();
        List<CellHeroCardUser> lstCardByNft = new List<CellHeroCardUser>();
        if (greenToggle.isOn)
        {
            lstGreen = lstUserCardGroup.Where(x => x.lst[0].GetDatabase().color == DBHero.COLOR_GREEN).ToList();
        }
        if (redToggle.isOn)
        {
            lstRed = lstUserCardGroup.Where(x => x.lst[0].GetDatabase().color == DBHero.COLOR_RED).ToList();
        }
        if (yellowToggle.isOn)
        {
            lstYellow = lstUserCardGroup.Where(x => x.lst[0].GetDatabase().color == DBHero.COLOR_YELLOW).ToList();
        }
        if (purpleToggle.isOn)
        {
            lstPurple = lstUserCardGroup.Where(x => x.lst[0].GetDatabase().color == DBHero.COLOR_PURPLE).ToList();
        }
        if (whiteToggle.isOn)
        {
            lstWhite = lstUserCardGroup.Where(x => x.lst[0].GetDatabase().color == DBHero.COLOR_WHITE).ToList();
        }

        if (!greenToggle.isOn && !redToggle.isOn && !yellowToggle.isOn && !purpleToggle.isOn && !whiteToggle.isOn)
        {
            lstCardByColor = lstUserCardGroup;
        }
        else
        {
            lstCardByColor.AddRange(lstGreen);
            lstCardByColor.AddRange(lstRed);
            lstCardByColor.AddRange(lstYellow);
            lstCardByColor.AddRange(lstPurple);
            lstCardByColor.AddRange(lstWhite);
        }

        if (godToggle.isOn)
        {
            lstGod = lstCardByColor.Where(x => x.lst[0].GetDatabase().type == DBHero.TYPE_GOD).ToList();
        }
        if (mortalToggle.isOn)
        {
            lstMortal = lstCardByColor.Where(x => x.lst[0].GetDatabase().type == DBHero.TYPE_TROOPER_NORMAL).ToList();
        }
        if (spellToggle.isOn)
        {
            lstSpell = lstCardByColor.Where(x => x.lst[0].GetDatabase().type == DBHero.TYPE_TROOPER_MAGIC || x.lst[0].GetDatabase().type == DBHero.TYPE_BUFF_MAGIC).ToList();
        }
        if (!godToggle.isOn && !mortalToggle.isOn && !spellToggle.isOn)
        {
            lstCardByType = lstCardByColor;
        }
        else
        {
            lstCardByType.AddRange(lstGod);
            lstCardByType.AddRange(lstMortal);
            lstCardByType.AddRange(lstSpell);
        }


        if (nftToggle.isOn)
        {
            lstNft = lstCardByType.Where(x => x.lst[0].cardType == HeroCard.CardType.nft).ToList();
        }
        if (nonNftToggle.isOn)
        {
            lstNonNft = lstCardByType.Where(x => x.lst[0].cardType == HeroCard.CardType.normal).ToList();
        }
        if (!nftToggle.isOn && !nonNftToggle.isOn)
        {
            lstCardByNft = lstCardByType;
        }
        else
        {
            lstCardByNft.AddRange(lstNft);
            lstCardByNft.AddRange(lstNonNft);
        }

        if (!greenToggle.isOn && !redToggle.isOn && !yellowToggle.isOn && !purpleToggle.isOn && !whiteToggle.isOn && !godToggle.isOn && !mortalToggle.isOn && !spellToggle.isOn && !nftToggle.isOn && !nonNftToggle.isOn)
        {
            lstReturn = lstUserCardGroup;
        }
        else
        {
            lstReturn = lstCardByNft.GroupBy(x => x.heroId).Select(x => x.First()).ToList();
        }
        List<long> listOrder = new List<long>
            {
                DBHero.COLOR_RED,
                DBHero.COLOR_GREEN,
                DBHero.COLOR_PURPLE,
                DBHero.COLOR_YELLOW,
                DBHero.COLOR_WHITE
            };
        lstReturn = lstReturn.OrderBy(x => x.lst[0].GetDatabase().type)
            .ThenBy(x => listOrder.IndexOf(x.lst[0].GetDatabase().color))
            .ThenBy(x => x.lst[0].GetDatabase().mana)
            .ThenBy(x => x.lst[0].GetDatabase().name)
            .ToList();
        return lstReturn;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        CustomDeckScene.main = null;
    }
    private void OnDisable()
    {
        useFilter = false;
        useSearch = false;
    }
    public void DoClickSidebarTab(int type) //0 => all, 1 => god, 2 => mortal, 3 => spell
    {
        foreach (GoOnOff goo in m_ListSideTabButtons)
        {
            if (goo.online)
            {
                goo.transform.GetChild(0).GetComponent<Image>().DOFillAmount(0f, 0.2f).OnComplete(() =>
                {
                    goo.TurnOff();
                });
            }
            if (goo.idInt == type)
            {
                goo.TurnOn();
                goo.transform.GetChild(0).GetComponent<Image>().DOFillAmount(1f, 0.2f);
            }
        }
    }
}
