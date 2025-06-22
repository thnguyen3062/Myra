using GIKCore.Utilities;
using GIKCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using DG.Tweening;
using UIEngine.UIPool;
using TMPro;
using UnityEngine.UI;

public class PopupOpenRewardProps
{
    // cac chi so balance 
    public RewardBalanceBuilder rewardBalanceBuilder = new RewardBalanceBuilder();
    // bai thua  doi sang vang cho ng choi 
    public RewardDuplicateGoldBuilder rewardDuplicateGoldBuilder = new RewardDuplicateGoldBuilder();
    // deck 
    public RewardDeckBuilder rewardDeckBuilder = new RewardDeckBuilder();
    // timechest
    public RewardTimeChestBuilder rewardTimeChestBuilder = new RewardTimeChestBuilder();
    //card
    public List<RewardCardBuilder> listRewardCardBuilder = new List<RewardCardBuilder>();
    // item 
    public List<RewardItemBuilder> listRewardItemBuilder = new List<RewardItemBuilder>();

    // Callback
    public ICallback.CallFunc onClose = null;
    public bool isShowButtonUpgrade = false;
    public bool isShowListReward = true;
}
public class RewardCardBuilder
{
    public long heroId;
    public int frame;
    public int realRewardCount; // tong so card 
    public int realIronCopy; // so card dc nhan
    public int requireCopy; // dieu kien de len level tiep 
}
public class RewardDuplicateGoldBuilder
{
    public int duplicateGold;  // vang nhan do doi card duplicate
    public int curGold;
}
public class RewardBalanceBuilder
{
    public int lvGold; // so vang duoc nhan
    public int curGold; // so vang truoc day
    public int lvExp;
    public int curExp;
    public int lvEssence;
    public int curEssence;
    public int lvlGem;
    public int curGem;
}
public class RewardItemBuilder
{
    public long id;
    public int count;
    public int quantity;
    public string name;
    public string image;
    public Sprite sprite = null;
}
public class RewardDeckBuilder
{
    public int deckColor;
}
public class RewardTimeChestBuilder
{
    public int timeChest = 0; // neu ko co thi truyen = 0
}
public class PopupOpenReward : MonoBehaviour
{
    // Fields
    [SerializeField] private SC_SingleRewardAnim m_RewardManager;
    [SerializeField] private SC_Reward_Info m_InfoManager;
    [SerializeField] private CardUserInfor m_GodCard;
    [SerializeField] private CardUserInfor m_SpellCard;
    [SerializeField] private CardUserInfor m_MinionCard;
    [SerializeField] private GameObject m_CellCard;
    [SerializeField] private GameObject m_CellCollection;
    [SerializeField] private List<GameObject> m_ListDeck;

    [SerializeField] private GameObject m_RewardCoin;
    [SerializeField] private GameObject m_RewardGem;
    [SerializeField] private GameObject m_RewardItem;
    [SerializeField] private GameObject m_RewardEssence;
    [SerializeField] private GameObject m_RewardExp;
    [SerializeField] private GameObject m_RewardChest3H;
    [SerializeField] private GameObject m_RewardChest8H;
    [SerializeField] private GameObject m_RewardChest12H;

    [SerializeField] private GameObject m_ListOpenAnim;
    [SerializeField] private GameObject m_ListOpenShow;
    [SerializeField] private HorizontalPoolGroup m_PoolItemShow;
    [SerializeField] private GameObject m_ButtonUpgrade;
    [SerializeField] private GameObject m_ButtonClose;
    [SerializeField] private TextMeshProUGUI m_ButtonText;

    // Values
    private PopupOpenRewardProps data = new PopupOpenRewardProps();
    private bool isShowDuplicate;
    private bool isShowDeck;
    private bool isShowGem;
    private bool isShowGold;
    private bool isShowExp;
    private bool isShowEssence;
    private bool isShowTimeChest;
    private bool isShowListReward;
    private int countCard = 0;
    private int countItem = 0;
    private List<string> listDeckName = new List<string> { "Green Deck", "Red Deck", "Yellow Deck", "Purple Deck" };
    private List<PopupOpenRewardItemProps> listRewardItemProps = new List<PopupOpenRewardItemProps>();
    private ICallback.CallFunc onCloseCallback = null;
    // Methods
    public PopupOpenReward SetData(PopupOpenRewardProps props)
    {
        data = props;
        for (int i = 0; i < data.listRewardCardBuilder.Count; i++)
        {
            RewardCardBuilder card = data.listRewardCardBuilder[i];

            PopupOpenRewardItemProps p = new PopupOpenRewardItemProps();
            p.isCard = true;
            p.hero = Database.GetHero(card.heroId);
            p.countCard = card.realRewardCount;
            p.frameCard = card.frame;
            listRewardItemProps.Add(p);
        }
        if (data.rewardDuplicateGoldBuilder.duplicateGold > 0)
        {
            PopupOpenRewardItemProps p = new PopupOpenRewardItemProps();
            p.isCoin = true;
            p.gold = data.rewardDuplicateGoldBuilder.duplicateGold;
            listRewardItemProps.Add(p);
        }
        if (data.rewardDeckBuilder.deckColor == 1 || data.rewardDeckBuilder.deckColor == 2 || data.rewardDeckBuilder.deckColor == 3 || data.rewardDeckBuilder.deckColor == 4)
        {
            PopupOpenRewardItemProps p = new PopupOpenRewardItemProps();
            p.isDeck = true;
            p.deckId = data.rewardDeckBuilder.deckColor;
            listRewardItemProps.Add(p);
        }
        if (data.rewardBalanceBuilder.lvlGem > 0)
        {
            PopupOpenRewardItemProps p = new PopupOpenRewardItemProps();
            p.isGem = true;
            p.gem = data.rewardBalanceBuilder.lvlGem;
            listRewardItemProps.Add(p);
        }
        if (data.rewardBalanceBuilder.lvGold > 0)
        {
            PopupOpenRewardItemProps p = new PopupOpenRewardItemProps();
            p.isCoin = true;
            p.gold = data.rewardBalanceBuilder.lvGold;
            listRewardItemProps.Add(p);
        }
        if (data.rewardBalanceBuilder.lvExp > 0)
        {
            PopupOpenRewardItemProps p = new PopupOpenRewardItemProps();
            p.isExp = true;
            p.exp = data.rewardBalanceBuilder.lvExp;
            listRewardItemProps.Add(p);
        }
        if (data.rewardBalanceBuilder.lvEssence > 0)
        {
            PopupOpenRewardItemProps p = new PopupOpenRewardItemProps();
            p.isEssence = true;
            p.essence = data.rewardBalanceBuilder.lvEssence;
            listRewardItemProps.Add(p);
        }
        for (int i = 0; i < data.listRewardItemBuilder.Count; i++)
        {
            RewardItemBuilder item = data.listRewardItemBuilder[i];
            PopupOpenRewardItemProps p = new PopupOpenRewardItemProps();
            p.isItem = true;
            p.itemSprite = item.sprite;
            p.itemName = item.name;
            p.itemCount = item.count;
            listRewardItemProps.Add(p);
        }
        if (data.rewardTimeChestBuilder.timeChest > 0)
        {
            PopupOpenRewardItemProps p = new PopupOpenRewardItemProps();
            switch (data.rewardTimeChestBuilder.timeChest)
            {
                case 3:
                    {
                        p.isChest3H = true;
                        break;
                    }
                case 8:
                    {
                        p.isChest8H = true;
                        break;
                    }
                case 12:
                    {
                        p.isChest12H = true;
                        break;
                    }
                case 1000:
                    {
                        p.isChest8H = true;
                        break;
                    }
            }
            listRewardItemProps.Add(p);
        }
        onCloseCallback = data.onClose;
        StartCoroutine(IUtil.Delay(() =>
        {
            DoRunOpenReward();
        }, 0.1f));
        return this;
    }
    public void DoClose()
    {
        if (onCloseCallback != null)
            onCloseCallback?.Invoke();
        Destroy(gameObject);
    }
    private void DoRunOpenReward(bool showListReward = true)
    {
        m_InfoManager.gameObject.SetActive(false);
        m_InfoManager.gameObject.SetActive(true);
        if (data.listRewardCardBuilder.Count > 0 && countCard < data.listRewardCardBuilder.Count)
        {
            m_CellCollection.SetActive(false);
            m_CellCard.SetActive(true);
            RewardCardBuilder card = data.listRewardCardBuilder[countCard];
            DBHero hero = Database.GetHero(card.heroId);
            switch (hero.type)
            {
                case DBHero.TYPE_GOD:
                    {
                        m_GodCard.gameObject.SetActive(true);
                        m_SpellCard.gameObject.SetActive(false);
                        m_MinionCard.gameObject.SetActive(false);

                        m_GodCard.SetInfoCard(hero, card.realRewardCount, card.frame);
                        break;
                    }
                case DBHero.TYPE_TROOPER_MAGIC:
                case DBHero.TYPE_BUFF_MAGIC:
                    {
                        m_GodCard.gameObject.SetActive(false);
                        m_SpellCard.gameObject.SetActive(true);
                        m_MinionCard.gameObject.SetActive(false);

                        m_SpellCard.SetInfoCard(hero, card.realRewardCount, card.frame);
                        break;
                    }
                case DBHero.TYPE_TROOPER_NORMAL:
                    {
                        m_GodCard.gameObject.SetActive(false);
                        m_SpellCard.gameObject.SetActive(false);
                        m_MinionCard.gameObject.SetActive(true);

                        m_MinionCard.SetInfoCard(hero, card.realRewardCount, card.frame);
                        break;
                    }
            }
            m_InfoManager.rewardNameText.text = hero.name;
            m_RewardManager.SetCardReward(card.frame, card.requireCopy, card.realRewardCount, card.realIronCopy - card.realRewardCount).PlayCardAnim((int)hero.rarity);
            countCard++;
            return;
            //m_RewardManager.PlayCardAnim(Rarity);
        }
        if (data.rewardDuplicateGoldBuilder.duplicateGold > 0 && !isShowDuplicate)
        {
            SetRewardIcon(isCoin: true);
            m_RewardManager.SetGoldReward(data.rewardDuplicateGoldBuilder.duplicateGold, data.rewardDuplicateGoldBuilder.curGold - data.rewardDuplicateGoldBuilder.duplicateGold).PlayCardAnim(6);
            isShowDuplicate = true;
            return;
        }
        if ((data.rewardDeckBuilder.deckColor == 1 || data.rewardDeckBuilder.deckColor == 2 || data.rewardDeckBuilder.deckColor == 3 || data.rewardDeckBuilder.deckColor == 4) && !isShowDeck)
        {
            m_CellCard.SetActive(false);
            m_CellCollection.SetActive(true);
            for (int i = 0; i < m_ListDeck.Count; i++)
            {
                m_ListDeck[i].SetActive(i == data.rewardDeckBuilder.deckColor - 1);
            }
            m_InfoManager.rewardNameText.text = listDeckName[data.rewardDeckBuilder.deckColor - 1];
            m_RewardManager.SetDeckReward(1, 1, 0).PlayCardAnim(2);
            isShowDeck = true;
            return;
        }
        if (data.rewardBalanceBuilder.lvlGem > 0 && !isShowGem)
        {
            SetRewardIcon(isGem: true);
            m_InfoManager.rewardNameText.text = "GEM";
            m_RewardManager.SetGoldReward(data.rewardBalanceBuilder.lvlGem, data.rewardBalanceBuilder.curGem - data.rewardBalanceBuilder.lvlGem).PlayCardAnim(5);
            isShowGem = true;
            return;
        }
        if (data.rewardBalanceBuilder.lvGold > 0 && !isShowGold)
        {
            SetRewardIcon(isCoin: true);
            m_InfoManager.rewardNameText.text = "COIN";
            m_RewardManager.SetGoldReward(data.rewardBalanceBuilder.lvGold, data.rewardBalanceBuilder.curGold - data.rewardBalanceBuilder.lvGold).PlayCardAnim(5);
            isShowGold = true;
            return;
        }
        if (data.rewardBalanceBuilder.lvExp > 0 && !isShowExp)
        {
            SetRewardIcon(isExp: true);
            m_InfoManager.rewardNameText.text = "EXP";
            m_RewardManager.SetGoldReward(data.rewardBalanceBuilder.lvExp, data.rewardBalanceBuilder.curExp - data.rewardBalanceBuilder.lvExp).PlayCardAnim(5);
            isShowExp = true;
            return;
        }
        if (data.rewardBalanceBuilder.lvEssence > 0 && !isShowEssence)
        {
            SetRewardIcon(isEssence: true);
            m_InfoManager.rewardNameText.text = "ESSENCE";
            m_RewardManager.SetGoldReward(data.rewardBalanceBuilder.lvEssence, data.rewardBalanceBuilder.curEssence - data.rewardBalanceBuilder.lvEssence).PlayCardAnim(5);
            isShowEssence = true;
            return;
        }
        if (data.listRewardItemBuilder.Count > 0 && countItem < data.listRewardItemBuilder.Count)
        {
            SetRewardIcon(isItem: true);
            m_InfoManager.rewardNameText.text = data.listRewardItemBuilder[countItem].name;
            RewardItemBuilder item = data.listRewardItemBuilder[countItem];
            if (item.sprite != null)
                m_RewardItem.GetComponent<Image>().sprite = item.sprite;
            m_RewardManager.SetGoldReward(item.count, item.quantity - item.count).PlayCardAnim(5);
            countItem++;
            return;
        }
        if (data.rewardTimeChestBuilder.timeChest > 0 && !isShowTimeChest)
        {
            switch (data.rewardTimeChestBuilder.timeChest)
            {
                case 3:
                    {
                        m_InfoManager.rewardNameText.text = "Standard Chest";
                        SetRewardIcon(isChest3H: true);
                        break;
                    }
                case 8:
                    {
                        m_InfoManager.rewardNameText.text = "Premium Chest";
                        SetRewardIcon(isChest8H: true);
                        break;
                    }
                case 12:
                    {
                        m_InfoManager.rewardNameText.text = "Godly Chest";
                        SetRewardIcon(isChest12H: true);
                        break;
                    }
                case 1000:
                    {
                        m_InfoManager.rewardNameText.text = "5s Chest";
                        SetRewardIcon(isChest8H: true);
                        break;
                    }
            }
            m_RewardManager.SetGoldReward(1, 0).PlayCardAnim(5);
            isShowTimeChest = true;
            return;
        }

        if (!isShowListReward && data.isShowListReward)
        {
            m_ListOpenAnim.SetActive(false);
            m_ListOpenShow.SetActive(true);
            m_PoolItemShow.SetCellDataCallback((GameObject go, PopupOpenRewardItemProps data, int index) =>
            {
                PopupOpenRewardItem script = go.GetComponent<PopupOpenRewardItem>();
                script.SetData(data);
            });
            m_PoolItemShow.SetAdapter(listRewardItemProps);
            SetButtonProgressionTitle();
            m_ButtonUpgrade.SetActive(data.isShowButtonUpgrade);
            m_ButtonClose.SetActive(!data.isShowButtonUpgrade);
            isShowListReward = true;
            return;
        }

        //check iff progression
        if (onCloseCallback != null)
        {
            onCloseCallback?.Invoke();
            Destroy(gameObject);
        }
    }
    private void SetRewardIcon(bool isCoin = false, bool isGem = false, bool isItem = false, bool isEssence = false, bool isExp = false, bool isChest3H = false, bool isChest8H = false, bool isChest12H = false)
    {
        m_RewardCoin.SetActive(isCoin);
        m_RewardGem.SetActive(isGem);
        m_RewardItem.SetActive(isItem);
        m_RewardEssence.SetActive(isEssence);
        m_RewardExp.SetActive(isExp);
        m_RewardChest3H.SetActive(isChest3H);
        m_RewardChest8H.SetActive(isChest8H);
        m_RewardChest12H.SetActive(isChest12H);
    }
    public static void Show(PopupOpenRewardProps props)
    {
        string assetName = "PopupOpenReward";
        GameObject go = IUtil.LoadPrefabWithParent("Prefabs/Home/" + assetName, Game.main.canvas.panelPopup);

        PopupOpenReward script = go.GetComponent<PopupOpenReward>();
        script.SetData(props);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (m_RewardManager.isCardSeqPlaying)
            {
                // If the sequence is playing, complete it immediately
                m_RewardManager.CardSeq.Complete(withCallbacks: true);
                m_RewardManager.isCardSeqPlaying = false;
                return;
            }
            else
            {
                DoRunOpenReward();
                return;
            }
        }
    }

    // Hien progression
    public void DoClickButtonProgression()
    {
        // do code here
    }
    private void SetButtonProgressionTitle()
    {
        string title = "Title";
        m_ButtonText.text = title;
    }
}
