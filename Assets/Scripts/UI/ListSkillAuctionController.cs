using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ListSkillAuctionController : MonoBehaviour ,IUpdateSelectedHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Transform skillSmallPreview;
    [SerializeField] private GameObject skillFullPreview;
    [SerializeField] private SkillAuctionPreview m_FullPreviewContainer;
    private bool isHold = false;
    private float countTime = 0;
    private List<long> m_CurrentLstAuction;
    private bool isTouch =false, isBidTime;
    // Start is called before the first frame update
    void Start()
    {
        if (GameBattleScene.instance != null)
        {
            GameBattleScene.instance.onGameStartRound += OnGameStartRound;
            GameBattleScene.instance.onBidStateStart += OnBidState;
            GameBattleScene.instance.onBidEnd += OnBidEnd;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isBidTime)
            return;
        if (isTouch)
        {
            countTime += Time.deltaTime;
            if (countTime > 0.3f)
            {
                if (isHold == false)
                {
                    if (GameBattleScene.instance != null)
                    {
                        isHold = true;
                        m_FullPreviewContainer.SetData(m_CurrentLstAuction);
                        m_FullPreviewContainer.gameObject.SetActive(true);
                    }
                }
            }
        }
    }
    void OnGameStartRound()
    {
        OnDeselected();
    }    
    void OnBidState(BidState state ,long a, long b, long c, long d, long e)
    {
        isBidTime = state == BidState.StartBid;
        if (isHold)
            OnDeselected();
    }
    void OnBidEnd(BidState state, bool isMe)
    {
        isBidTime = false;
    }    
    public void InitData(List<long> lst)
    {
        //data list 2 skill
        OnUpdateListSkill(lst);

    }
    
    public void OnDeselected()
    {
        //close preview full
        isHold = false;
        m_FullPreviewContainer.gameObject.SetActive(false);
        
    }  
    public void OnUpdateListSkill(List<long> listSkillNew)
    {
        GameData.main.skillIDBid = listSkillNew[0];
        m_CurrentLstAuction = listSkillNew;
        for (int i = 0; i < this.transform.childCount; i++)
        {
            PoolManager.Pools["Skill"].Despawn(this.transform.GetChild(i));
        }
        Transform trans = PoolManager.Pools["Skill"].Spawn(skillSmallPreview);
        trans.GetComponent<SkillAuctionInfo>().InitData(listSkillNew[1]);
        trans.SetParent(this.transform);
        trans.localScale = Vector3.one;
        Transform trans2 = PoolManager.Pools["Skill"].Spawn(skillSmallPreview);
        trans2.GetComponent<SkillAuctionInfo>().InitData(listSkillNew[0]);
        trans2.SetParent(this.transform);
        trans2.localScale = Vector3.one;
    }

    public void OnUpdateSelected(BaseEventData eventData)
    {
       
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isTouch = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isTouch = false;
        countTime = 0;
        OnDeselected();
    }
}
public class SkillAuctionDataInfo
{
    public long skillID;
    public string descEN;
    public string descKR;
    public string descJP;
    public string descCN;
    public string descTW;
}
