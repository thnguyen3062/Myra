using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GIKCore.Utilities;
using UnityEngine.UI;

public class GodCardHandler : MonoBehaviour
{
    [SerializeField] private GameObject godPrefab, ultiPrefab;
    [SerializeField] private List<Transform> playerGodPosition;
    [SerializeField] private List<Transform> enemyGodPosition;
    //[SerializeField] private List<Transform> ultiEnemy;
    public static GodCardHandler instance;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        if(GameBattleScene.instance != null)
        {
            GameBattleScene.instance.onSpawnRandomGod += SummonGodUI;
            GameBattleScene.instance.onSpawnRandomGodEnemy += SummonGodUIEnemy;
        }  
    }

    //public ICallback.CallFunc3<int, int> onUpdateGodCount;
    public void InitGodUI(List<long> godBattleID1, List<DBHero> godHero1, List<long> frame1, List<long> atk1, List<long> hp1, List<long> mana1, List<long> godBattleID2, List<DBHero> godHero2, List<long> frame2, List<long> atk2, List<long> hp2, List<long> mana2)
    {
        if (godHero1.Count > 6 || godHero2.Count > 6)
        {
            return;
        }

        // Player
        int indexPlayer = 0;
        int indexPlayerBattleID = 0;

        var groupGodPlayer = godHero1.GroupBy(g => g.heroNumber).Select(grp => grp.ToList()).ToList();
        groupGodPlayer.ForEach(x =>
        {
            playerGodPosition[indexPlayer].gameObject.SetActive(true);
            // init data ulti cho tung god 
            x.ForEach(c =>
            {
                GameObject go = Instantiate(godPrefab, Vector3.zero, Quaternion.identity, playerGodPosition[indexPlayer]);
                go.transform.localScale = new Vector3(.45f, .45f, .45f);
                GodCardUI card = go.GetComponent<GodCardUI>();
                card.InitData(godBattleID1[indexPlayerBattleID], godHero1[indexPlayerBattleID].id,frame1[indexPlayerBattleID],atk1[indexPlayerBattleID],hp1[indexPlayerBattleID],mana1[indexPlayerBattleID], CardData.Instance.GetCardDataInfo(godHero1[indexPlayerBattleID].id).name, CardOwner.Player); //Database.GetHero(godHero1[indexPlayerBattleID].id).name); ; ;
                indexPlayerBattleID += 1;
                card.onDropCard += OnDropCard;
                card.CheckCondition();
            });
            indexPlayer += 1;
            // x.First(x => x.id == 2);
        });

        // Enemy
        int indexEnemy = 0;
        int indexEnemyBattleID = 0;

        var groupGodEnemy = godHero2.GroupBy(g => g.heroNumber).Select(grp => grp.ToList()).ToList();
        groupGodEnemy.ForEach(x =>
        {
            enemyGodPosition[indexEnemy].gameObject.SetActive(true);
            x.ForEach(c =>
            {
                GameObject go = Instantiate(godPrefab, Vector3.zero, Quaternion.identity, enemyGodPosition[indexEnemy]);
                go.transform.localScale = new Vector3(.45f, .45f, .45f);
                GodCardUI card = go.GetComponent<GodCardUI>();
                card.InitData(godBattleID2[indexEnemyBattleID], godHero2[indexEnemyBattleID].id, frame2[indexEnemyBattleID],atk2[indexEnemyBattleID],hp2[indexEnemyBattleID],mana2[indexEnemyBattleID],/*Database.GetHero(godHero2[indexEnemyBattleID].id).name)*/CardData.Instance.GetCardDataInfo(godHero2[indexEnemyBattleID].id).name,CardOwner.Enemy);
                card.allowDrag = false;
                indexEnemyBattleID += 1;
                card.onDropCard += OnDropCard;
            });
            indexEnemy += 1;
        });
        OnDropCard();
    }

    private void OnDropCard()
    {
        int godPlayerCount = 0;
        playerGodPosition.ForEach(p =>
        {
            godPlayerCount += p.childCount;
        });

        int godEnemyCount = 0;
        enemyGodPosition.ForEach(p =>
        {
            godEnemyCount += p.childCount;
        });

        //onUpdateGodCount(godPlayerCount, godEnemyCount);
    }

    public void SummonGodUI(long battleId)
    {
        playerGodPosition.ForEach(x =>
        {
            if (x.childCount > 0)
            {
                foreach(Transform god in x.transform)
                    if(god.GetComponent<GodCardUI>().CurrentCardBattleID==battleId)
                    {
                        GodCardUI godUI = god.GetComponent<GodCardUI>();
                        godUI.ChangeState(1);
                        

                    }
            }
        });
    }
    public void GodDead(long battleId)
    {
        playerGodPosition.ForEach(x =>
        {
            if (x.childCount > 0)
            {
                for (int i = 0; i < x.childCount; i++)
                {
                    Transform god = x.GetChild(i).transform;
                    GodCardUI godUI = god.GetComponent<GodCardUI>();
                    if (godUI.CurrentCardBattleID == battleId)
                    {
                        godUI.ChangeState(0);
                        god.transform.SetAsFirstSibling();
                        if (!god.gameObject.activeSelf)
                            Destroy(god.gameObject);
                    }
                }
            }
        });
    }
      
    public void GodEnemyDead(long battleId)
    {
        enemyGodPosition.ForEach(x =>
        {
            if (x.childCount > 0)
            {
                foreach (Transform god in x.transform)
                    if (god.GetComponent<GodCardUI>().CurrentCardBattleID == battleId)
                    {
                        god.GetComponent<GodCardUI>().ChangeState(0);
                    }

            }
        });

    }
    private void SummonGodUIEnemy(long id)
    {
        enemyGodPosition.ForEach(x =>
        {
            if (x.childCount > 0)
            {

                foreach (Transform god in x.transform)
                    if (god.GetComponent<GodCardUI>().CurrentCardID == id)
                {
                    god.GetComponent<GodCardUI>().ChangeState(1);
                }
            }
        });
    }
}
