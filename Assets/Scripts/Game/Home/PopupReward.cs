using GIKCore;
using GIKCore.Net;
using GIKCore.UI;
using GIKCore.Utilities;
using pbdson;
using System.Collections;
using System.Collections.Generic;
using UIEngine.UIPool;
using UnityEngine;
using UnityEngine.UI;

public class PopupReward : GameListener
{
    // Fields
    [SerializeField] private HorizontalPoolGroup m_Pool;
    [SerializeField] private PopupStoneInfor m_PopupStoneInfor;
    [SerializeField] private RectTransform m_ProgressBar;
    [SerializeField] private RectTransform m_ProgressFill;
    [SerializeField] private Image m_ImageFill;
    [SerializeField] private GameObject m_CardPreview;
    [SerializeField] private CardPreviewInfo m_GodPreview;
    [SerializeField] private CardPreviewInfo m_MinionPreview;
    [SerializeField] private CardPreviewInfo m_SpellPreview;

    // Values
    private List<HomeRewardItemProps> listHomeRewardItemProps = new List<HomeRewardItemProps>();
    private int levelClaim;
    private HomeRewardItemProps receiveHomeRewardItem = null;

    // Methods
    public PopupReward Show()
    {
        gameObject.SetActive(true);
        return this;
    }
    public PopupReward SetData()
    {
        m_Pool.SetCellPrefabCallback((index) =>
        {
            HomeRewardItemProps data = m_Pool.GetCellDataAt<HomeRewardItemProps>(index);
            if (data.hasCardUnlock)
            {
                return m_Pool.GetDeclarePrefab(1);
            }
            return m_Pool.GetDeclarePrefab(0);
        });
        m_Pool.SetCellDataCallback((GameObject go, HomeRewardItemProps data, int index) =>
        {
            go.GetComponent<HomeRewardItem>()
            .SetData(data)
            .SetOnclickCB((props) =>
            {
                if (props.state == 0 || props.state == 2)
                {
                    PopupStoneInforProps stoneInforProps = new PopupStoneInforProps();
                    stoneInforProps.level = props.id;
                    stoneInforProps.matchCoin = props.matchAlwaysCoin;
                    stoneInforProps.matchCup = props.matchAlwaysCup;
                    stoneInforProps.matchEssence = props.matchAlwaysEssence;
                    stoneInforProps.chestCoin = props.matchTimeChestCoin;
                    stoneInforProps.chestCup = props.matchTimeChestCup;
                    stoneInforProps.chestEssence = props.matchTimeChestEssence;
                    stoneInforProps.levelDesc = props.desc;
                    m_PopupStoneInfor.DoShow(stoneInforProps);
                }
                else if (props.state == 1)
                {
                    levelClaim = props.id;
                    receiveHomeRewardItem = props;
                    Game.main.socket.ClaimLevelReward(props.id);
                }
            });
        });
        m_Pool.SetCellSizeCallback((index) =>
        {
            HomeRewardItemProps data = m_Pool.GetCellDataAt<HomeRewardItemProps>(index);
            if (data.hasCardUnlock)
            {
                return new Vector2(1130, 880);
            }
            return new Vector2(400, 880);
        });
        listHomeRewardItemProps = GameData.main.listHomeRewardItemProps;
        StartCoroutine(IUtil.Delay(() =>
        {
            DBLevel level = Database.GetUserLevelInfo(GameData.main.userCurrency.exp);
            m_Pool.SetAdapter(listHomeRewardItemProps).ScrollToChild(listHomeRewardItemProps.FindIndex(x => x.id == level.id), 0);
        }, 0.1f));

        int countLevel = 0;
        int countTotal = 0;
        int maxLevelClaim = 0;
        for (int i = 0; i < listHomeRewardItemProps.Count; i++)
        {
            if (i < listHomeRewardItemProps.Count - 1)
            {
                if (listHomeRewardItemProps[i].hasCardUnlock)
                {
                    countTotal += 3;
                }
                else
                {
                    countTotal += 1;
                }
            }
            if (listHomeRewardItemProps[i].state == 1 || listHomeRewardItemProps[i].state == 2)
                maxLevelClaim = maxLevelClaim < listHomeRewardItemProps[i].id ? listHomeRewardItemProps[i].id : maxLevelClaim;
        }
        for (int i = 0; i < maxLevelClaim - 1; i++)
        {
            if (listHomeRewardItemProps[i].hasCardUnlock)
                countLevel += 3;
            else
                countLevel += 1;
        }
        m_ImageFill.fillAmount = (float)countLevel / countTotal;
        return this;
    }
    public void Hide()
    {
        if (ProgressionController.instance != null)
        {
            if (GameData.main.userProgressionState == 11)
            {
                ProgressionController.instance.NextState();
            }
            else if (GameData.main.userProgressionState == 13)
            {
                ProgressionController.instance.NextState();
            }
        }
        gameObject.SetActive(false);
    }
    public override bool ProcessSocketData(int serviceId, byte[] data)
    {
        if (base.ProcessSocketData(serviceId, data)) return true;
        switch (serviceId)
        {
            case IService.CLAIM_LEVEL_REWARD:
                {
                    ListCommonVector lcv = ISocket.Parse<ListCommonVector>(data);
                    CommonVector cv0 = lcv.aVector[0];
                    if (cv0.aLong[0] == 1)
                    {
                        GameData.main.listHomeRewardItemProps.Find(x => x.id == levelClaim).state = 2;
                        listHomeRewardItemProps = GameData.main.listHomeRewardItemProps;
                        m_Pool.SetAdapter(listHomeRewardItemProps, false);

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
                        //                        Debug.Log("===== cv2: " + string.Join(", ", cv2.aLong));
                        RewardDuplicateGoldBuilder rdgb = new RewardDuplicateGoldBuilder();
                        rdgb.duplicateGold = (int)cv2.aLong[0];
                        rdgb.curGold = (int)cv2.aLong[1];
                        #endregion

                        // parse deckBuilder
                        #region parse deckBuilder
                        CommonVector cv3 = lcv.aVector[3];
                        //Debug.Log("===== cv3: " + string.Join(", ", cv3.aLong));
                        RewardDeckBuilder rdb = new RewardDeckBuilder();
                        rdb.deckColor = (int)cv3.aLong[0];
                        #endregion

                        // parse balanceBuilder
                        #region parse balanceBuilder
                        CommonVector cv4 = lcv.aVector[4];
                        Debug.Log("===== cv4: " + string.Join(", ", cv4.aLong));
                        RewardBalanceBuilder rbb = new RewardBalanceBuilder();
                        rbb.lvGold = (int)cv4.aLong[0];
                        rbb.curGold = (int)cv4.aLong[1];
                        rbb.lvExp = (int)cv4.aLong[2];
                        rbb.curExp = (int)cv4.aLong[3];
                        rbb.lvEssence = (int)cv4.aLong[4];
                        rbb.curEssence = (int)cv4.aLong[5];
                        rbb.lvlGem = (int)cv4.aLong[6];
                        rbb.curGem = (int)cv4.aLong[7];
                        #endregion

                        // parse itemBuilder
                        #region parse itemBuilder
                        CommonVector cv5 = lcv.aVector[5];
                        List<RewardItemBuilder> listRewardItemBuilder = new List<RewardItemBuilder>();
                        int CV5_BLOCK_INT = 3;
                        int CV5_BLOCK_STRING = 2;
                        int CV5_COUNT = cv5.aLong.Count / CV5_BLOCK_INT;
                        for (int i = 0; i < CV5_COUNT; i++)
                        {
                            RewardItemBuilder rib = new RewardItemBuilder();
                            rib.id = cv5.aLong[i * CV5_BLOCK_INT + 0];
                            rib.count = (int)cv5.aLong[i * CV5_BLOCK_INT + 1];
                            rib.quantity = (int)cv5.aLong[i * CV5_BLOCK_INT + 2];

                            rib.name = cv5.aString[i * CV5_BLOCK_STRING + 0];
                            rib.image = cv5.aString[i * CV5_BLOCK_STRING + 1];
                            if (receiveHomeRewardItem != null)
                            {
                                if (GameData.main.DictRewardSprite.ContainsKey(receiveHomeRewardItem.itemUrl) && GameData.main.DictRewardSprite[receiveHomeRewardItem.itemUrl] != null)
                                {
                                    rib.sprite = GameData.main.DictRewardSprite[receiveHomeRewardItem.itemUrl];
                                }
                            }
                            else
                            {
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
                            }
                            listRewardItemBuilder.Add(rib);
                        }
                        #endregion


                        PopupOpenRewardProps props = new PopupOpenRewardProps();
                        props.listRewardCardBuilder = lstCardBuilder;
                        props.rewardDuplicateGoldBuilder = rdgb;
                        props.rewardDeckBuilder = rdb;
                        props.rewardBalanceBuilder = rbb;
                        props.listRewardItemBuilder = listRewardItemBuilder;
                        PopupOpenReward.Show(props);

                        Game.main.socket.GetUserCurrency();
                    }
                    else
                    {
                        Toast.Show(cv0.aString[0]);
                    }
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
            case NetData.PREVIEW_CARD:
                {
                    DBHero hero = (DBHero)data;
                    m_CardPreview.SetActive(true);
                    switch (hero.type)
                    {
                        case DBHero.TYPE_GOD:
                            {
                                m_GodPreview.SetCardPreview(hero, 1, true);
                                m_MinionPreview.gameObject.SetActive(false);
                                m_SpellPreview.gameObject.SetActive(false);
                                m_GodPreview.gameObject.SetActive(true);
                                break;
                            }
                        case DBHero.TYPE_TROOPER_NORMAL:
                            {
                                m_MinionPreview.SetCardPreview(hero, 1, true);
                                m_GodPreview.gameObject.SetActive(false);
                                m_SpellPreview.gameObject.SetActive(false);
                                m_MinionPreview.gameObject.SetActive(true);
                                break;
                            }
                        case DBHero.TYPE_TROOPER_MAGIC:
                        case DBHero.TYPE_BUFF_MAGIC:
                            {
                                m_SpellPreview.SetCardPreview(hero, 1, true);
                                m_GodPreview.gameObject.SetActive(false);
                                m_MinionPreview.gameObject.SetActive(false);
                                m_SpellPreview.gameObject.SetActive(true);
                                break;
                            }
                    }
                    break;
                }
        }
        return false;
    }
}
