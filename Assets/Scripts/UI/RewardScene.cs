using GIKCore.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PathologicalGames;
using GIKCore;
using UnityEngine.SceneManagement;
using GIKCore.Utilities;
using pbdson;
using UnityEngine.UI;

public class RewardScene : GameListener

{
    [SerializeField] public Transform contentListReward;
    [SerializeField] private GameObject coinPref, cardPref,timeChestPref,expPref,deckPref,essencePref;
    [SerializeField] private GameObject cardPreview;
    [SerializeField] private CardPreviewInfo godCardPreview;
    [SerializeField] private CardPreviewInfo minionCardPreview;
    [SerializeField] private CardPreviewInfo spellCardPreview;
    [SerializeField] private GameObject moreItem;
    [SerializeField] private GameObject buttonNormal;
    [SerializeField] private GameObject buttonProgression , btnProgressionText;
    [SerializeField] private TextMeshProUGUI userLevel;
    [SerializeField] private Image fillImageLevel;
    [SerializeField] private GameObject rewardNotification;
    [SerializeField] private SC_BounceOnVisible animation;
    [SerializeField] private GameObject m_TimeChestNotification;
    private List<GameObject> lstItem = new List<GameObject>();    
    public static RewardScene instance;
    public event ICallback.CallFunc onRewardComplete;

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
    public void InitData(RewardModel model)
    {
        if (GameData.main.userProgressionState < 14)
        {
            buttonNormal.SetActive(false);
            buttonProgression.SetActive(true);
        }
        else
        {
            buttonNormal.SetActive(true);
            buttonProgression.SetActive(false);
        }
        if(model.rewardNewbie)
        {
            GameData.main.statusNewbie = model.rewardNewbieEffect;
            GameData.main.imgRewardNewbie = model.rewardNewbieImg;
            rewardNotification.SetActive(true);
        }    
        int count = 0;
        // model.currentExp
        DBLevel level = Database.GetUserLevelInfo(model.currentExp);
        if (level != null)
        {
            if (userLevel != null)
                userLevel.text = level.id.ToString();
            if (fillImageLevel != null)
                fillImageLevel.fillAmount = (float)level.expCurrent / (float)level.expToUpLevel;
        }
        if (model.gold>0)
        {
            GameObject gold = Instantiate(coinPref, contentListReward);
            gold.transform.localScale = new Vector3(.8f, .8f, 1);
            gold.GetComponent<ItemRewardInfo>().InitData("COIN REWARD", model.gold + " " +"Coin");
            gold.SetActive(false);
            lstItem.Add(gold);
            count++;
        }
        if (model.exp > 0)
        {
            GameObject exp = Instantiate(expPref, contentListReward);
            exp.transform.localScale = new Vector3(.8f, .8f, 1);
            exp.GetComponent<ItemRewardInfo>().InitData("EXP REWARD", "+"+model.exp + " "+"EXP");
            exp.SetActive(false);
            lstItem.Add(exp);
            count++;
        }
        if (model.essence > 0)
        {
            GameObject ess = Instantiate(essencePref, contentListReward);
            ess.transform.localScale = new Vector3(.8f, .8f, 1);
            ess.GetComponent<ItemRewardInfo>().InitData("ESSENCE", model.essence + " "+"ESSENCE");
            ess.SetActive(false);
            lstItem.Add(ess);
            count++;
        }
        if (model.timeChest > 0)
        {
            GameObject timeChest = Instantiate(timeChestPref, contentListReward);
            timeChest.transform.localScale = new Vector3(.8f, .8f, 1);
            timeChest.GetComponent<ItemRewardInfo>().InitData("TIMECHEST", "", 1 , model.timeChest);
            timeChest.SetActive(false);
            lstItem.Add(timeChest);
            count++;
            //if (m_TimeChestNotification != null )
            //{
            //    m_TimeChestNotification.SetActive(!string.IsNullOrEmpty(model.timeChestText));
            //    m_TimeChestNotification.GetComponent<TimeChestNotificationInfo>().InitData(model.timeChest, model.hasTimeChest, model.timeChestText);
            //}
            if (m_TimeChestNotification != null)
            {
                m_TimeChestNotification.SetActive(false);
            }

        }  
        else
        {
            if (m_TimeChestNotification != null)
            {
                m_TimeChestNotification.SetActive(false);
            }
        }
        //elo
        if(count>4)
        { 
            moreItem.gameObject.SetActive(true);
        }
        
        StartCoroutine(SpawnItem());
        //gameObject.SetActive(true);
    }
    IEnumerator SpawnItem()
    {
        yield return new WaitForSeconds(1);
        for(int i = 0; i< lstItem.Count;i++)
        {
            animation.objectsToBounce.Add(lstItem[i]);
            lstItem[i].SetActive(true);
            if(i==lstItem.Count-1)
            {
                lstItem[i].GetComponent<ItemRewardInfo>().line.SetActive(false);
            }    
        }    
    }    
    // Update is called once per frame
    void Update()
    {
        
    }
    public void DoClickBackHome()
    {
        //PoolManager.Pools["RewardPopup"].Despawn(this.transform);
        //onRewardComplete?.Invoke();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameObject.SetActive(false);
        Game.main.LoadScene("HomeSceneNew", delay: 0.3f, curtain: true);

    }
    public void DoClickPlayAgain()
    {
        //PoolManager.Pools["RewardPopup"].Despawn(this.transform);
        //onRewardComplete?.Invoke();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameObject.SetActive(false);
        Game.main.socket.GetRank();
        Game.main.socket.GetUserDeck();
        if (GameData.main.isNewbie)
        {
            GameData.main.currentPlayMode = "unrank";
            Game.main.socket.GetUserDeck();
            Game.main.socket.GetRank();
            Game.main.LoadScene("SelectDeckScene", delay: 0.3f, curtain: true);
        }
        else
        {

            if (GameData.main.isOutToModeSelection)
            {
                GameData.main.isOutToModeSelection = false;
                Game.main.socket.GetMode();
                Game.main.LoadScene("SelectModeScene", delay: 0.3f, curtain: true);
            }
            else
            {
                Game.main.socket.GetUserDeck();
                Game.main.socket.GetRank();
                if (GameData.main.currentPlayMode == "unrank")
                    Game.main.LoadScene("SelectDeckScene", delay: 0.3f, curtain: true);
                else
                    Game.main.LoadScene("SelectDeckRankScene", delay: 0.3f, curtain: true);
            }
        }
    }
    public void DoClickButtonProgression()
    {
        if(GameData.main.userProgressionState<14)
        {
            gameObject.SetActive(false);
            if (GameData.main.userProgressionState == 1)
            {
                HomeChestProps hc = new HomeChestProps();
                hc.state = HomeChestProps.ChestState.NOT_ACTIVATED;
                hc.status = HomeChestProps.ChestStatus.CHEST_OPEN_NOW;
                hc.timeChest = HomeChestProps.TimeChest.TIME_3H;
                hc.remainTime = 0;
                hc.isNew = true;
                hc.canOpen = true;
                hc.turnOnPointClick = true;
                hc.id = -1;
                HomeChestProps hcEmpty = new HomeChestProps();
                hcEmpty.state = HomeChestProps.ChestState.EMPTY;
                hcEmpty.status = HomeChestProps.ChestStatus.CHEST_EMPTY;
                hcEmpty.id = 0;
                GameData.main.listHomeChestProps.Clear();
                GameData.main.listHomeChestProps.Add(hc);
                GameData.main.listHomeChestProps.Add(hcEmpty);
                GameData.main.listHomeChestProps.Add(hcEmpty);
                GameData.main.listHomeChestProps.Add(hcEmpty);
                Game.main.LoadScene("BattleSceneTutorial", () =>
                {
                    BattleSceneTutorial.instance.SetTutorial(1);
                }, delay: 0.3f, curtain: true);
            }
            
            if (GameData.main.userProgressionState == 6)
            {
                HomeChestProps hc = new HomeChestProps();
                hc.state = HomeChestProps.ChestState.NOT_ACTIVATED;
                hc.status = HomeChestProps.ChestStatus.CHEST_OPEN_NOW;
                hc.timeChest = HomeChestProps.TimeChest.TIME_8H;
                hc.remainTime = 0;
                hc.isNew = true;
                hc.canOpen = true;
                hc.turnOnPointClick = true;
                hc.id = -1;
                HomeChestProps hcEmpty = new HomeChestProps();
                hcEmpty.state = HomeChestProps.ChestState.EMPTY;
                hcEmpty.status = HomeChestProps.ChestStatus.CHEST_EMPTY;
                hcEmpty.id = 0;
                GameData.main.listHomeChestProps.Clear();
                GameData.main.listHomeChestProps.Add(hc);
                GameData.main.listHomeChestProps.Add(hcEmpty);
                GameData.main.listHomeChestProps.Add(hcEmpty);
                GameData.main.listHomeChestProps.Add(hcEmpty);
                Game.main.LoadScene("HomeSceneNew",
               () => {
                   HomeSceneNew.instance.UpdateHomeChestTut();
               }, delay: 0.3f, curtain: true);
            }
            if(GameData.main.userProgressionState == 9)
            {
                Game.main.socket.UpdateProgression(9);
                GameData.main.userProgressionState = 10;
                ProgressionController.instance.DoActionInState();
            }    
            if(GameData.main.userProgressionState ==11)
            {
                Game.main.socket.UpdateProgression(11);
                GameData.main.userProgressionState = 12;
                ProgressionController.instance.DoActionInState();
            }    

        }    
    }    
    public void GetCoin(long coinReward)
    {
       GameObject go= Instantiate(coinPref, contentListReward);
        go.SetActive(true);
        go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "x"+coinReward.ToString();

    }
    public void ShowPreviewHandCard(long id,long frame)
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
    private void OnDisable()
    {
        foreach(Transform child in contentListReward.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
