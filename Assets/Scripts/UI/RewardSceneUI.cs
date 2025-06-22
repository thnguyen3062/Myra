using GIKCore;
using GIKCore.Lang;
using GIKCore.Net;
using GIKCore.Utilities;
using pbdson;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UIEngine.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class RewardSceneUI : GameListener
{
    // Fields
    [SerializeField] private Transform contentListReward;
    [SerializeField] private GameObject coinPref, cardPref;
    [SerializeField] private GameObject cardPreview;
    [SerializeField] private CardPreviewInfo godCardPreview;
    [SerializeField] private CardPreviewInfo minionCardPreview;
    [SerializeField] private CardPreviewInfo spellCardPreview;
    [SerializeField] private RewardFlipCard m_Flipcard;
    [SerializeField] private ScrollRect m_ScrollRect;
    [SerializeField] private ITween m_Blur;
    [SerializeField] GameObject buttonBack;
    // Values
    public static RewardSceneUI instance;
    public event ICallback.CallFunc onRewardComplete;
    public List<GameObject> lstCardReward = new List<GameObject>();
    public List<HeroCard> lstHeroCard = new List<HeroCard>();
    public List<UserItem> lstUserItem = new List<UserItem>();
    public int countGold = 0;

    IEnumerator coroutine = null;

    // Methods
    public override bool ProcessSocketData(int id, byte[] data)
    {
        if (base.ProcessSocketData(id, data))
            return true;

        switch (id)
        {
            case IService.GET_BALANCE:
                {
                    CommonVector cv = ISocket.Parse<CommonVector>(data);
                    GameData.main.userCurrency.gold = cv.aLong[0];
                    GameData.main.userCurrency.gem = cv.aLong[1];
                    GameData.main.userCurrency.exp = cv.aLong[2];
                    break;
                }
        }

        return false;
    }

    // Start is called before the first frame update
    protected override void Awake()
    {
        instance = this;
    }
    public void InitData(RewardUIModel model)
    {
        if (ProgressionController.instance != null)
        {
            buttonBack.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = LangHandler.Get("203","GO TO COLECTION");
        }
        else
        {
            buttonBack.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = LangHandler.Get("204","BACK");
        }    
        lstCardReward.Clear();
        lstHeroCard.Clear();
        lstUserItem.Clear();
        if (model.lstCard.Count > 0)
        {
            foreach (HeroCard hc in model.lstCard)
            {
                lstHeroCard.Add(hc);
                GameObject card = Instantiate(cardPref, contentListReward);
                card.SetActive(true);
                CardRewardUI script = card.GetComponent<CardRewardUI>();
                script.SetData(hc);
            }
        }
        if(model.lstItem.Count > 0)
        {
            foreach (UserItem ui in model.lstItem)
            {
                lstUserItem.Add(ui);
                GameObject go = Instantiate(coinPref, contentListReward);
                Image img = go.GetComponent<Image>();
                if (ui.sprite != null) SetAvatarSprite(ui.sprite, img);
                else LoadHttpAvatar(ui.image, img, (s) => { ui.sprite = s; });
                go.SetActive(true);
                go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "x" + 1;
            }
        }
        if (model.goldNumber > 0)
        {
            countGold = 1;
            GetCoin(model.goldNumber);
        }
        else
        {
            countGold = 0;
        }

        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void DoClickBack()
    {
        //PoolManager.Pools["RewardPopup"].Despawn(this.transform);
        //onRewardComplete?.Invoke();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        m_Blur.Play();
        if(GameData.main.userProgressionState==8)
        {
            if (ProgressionController.instance != null)
                ProgressionController.instance.DoActionInState();
        }    
        //gameObject.SetActive(false);
    }
    public void GetCoin(long coinReward)
    {
        GameObject go = Instantiate(coinPref, contentListReward);
        go.SetActive(true);
        go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "x" + coinReward.ToString();

    }
    public void ShowPreviewHandCard(long id, long frame)
    {
        if (cardPreview.activeSelf)
            return;

        cardPreview.SetActive(true);
        DBHero hero = Database.GetHero(id);
        if (hero.type == DBHero.TYPE_GOD)
        {
            godCardPreview.gameObject.SetActive(true);
            godCardPreview.SetCardPreview(hero, frame, true);
        }
        if (hero.type == DBHero.TYPE_TROOPER_MAGIC)
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
    public void LoadHttpAvatar(string url, Image image, ICallback.CallFunc2<Sprite> onLoaded = null)
    {
        if (coroutine != null)
            Game.main.StopCoroutine(coroutine);

        if (string.IsNullOrEmpty(url)) return;

        coroutine = IUtil.LoadTexture2DFromUrl(url, (Texture2D tex) =>
        {
            if (tex == null) return;
            Sprite sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            SetAvatarSprite(sprite, image);

            if (onLoaded != null) onLoaded(sprite);
        });
        Game.main.StartCoroutine(coroutine);
    }
    private void SetAvatarSprite(Sprite sprite, Image image)
    {
        image.sprite = sprite;
    }
    private void OnDisable()
    {
        foreach (Transform child in contentListReward.transform)
        {
            Destroy(child.gameObject);
        }
    }
    private void OnEnable()
    {
        m_ScrollRect.content.anchoredPosition = Vector2.zero;
        contentListReward.GetComponent<SwipeRewardPopup>().SetOnSwipeStopCallback((int index) =>
        {
            if (index <= contentListReward.childCount - countGold)
            {
                CardRewardUI script = contentListReward.GetChild(index).GetComponent<CardRewardUI>();

                if (script != null && !script.isOpen)
                {
                    HeroCard hc = lstHeroCard[index];
                    script.FlipCard();
                    //m_Flipcard.gameObject.SetActive(true);
                    //m_Flipcard.SetData(hc);

                    //script.SetData(hc, true);
                    //script.SetInvisible();
                    
                }
            }
        });
    }
}
