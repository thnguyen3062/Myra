using GIKCore;
using GIKCore.UI;
using GIKCore.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static HomeChestProps;

public class HomeChestProps
{
    public enum ChestState
    {
        EMPTY,
        NOT_ACTIVATED,
        ACTIVATED
    }
    public enum TimeChest
    {
        TIME_3H = 3,
        TIME_8H = 8,
        TIME_12H = 12
    }
    public enum ChestStatus
    {
        CHEST_EMPTY,
        CHEST_OPENING,
        CHEST_OPEN_NOW,
        CHEST_CLAIM
    }

    public int id;
    public TimeChest timeChest;
    public long remainTime;
    public int gemPrice;
    public ChestState state;
    public ChestStatus status;
    public bool isNew;
    public bool canOpen;
    public bool turnOnPointClick = false;
}
public class HomeChest : MonoBehaviour
{
    // Fields
    [SerializeField] private List<GameObject> m_ListStatus;
    [SerializeField] private List<GameObject> m_ListState;
    [SerializeField] private List<GameObject> m_ListChest;
    [SerializeField] private TextMeshProUGUI m_TextPriceOpen;
    [SerializeField] private TextMeshProUGUI m_TextTimeLeft;
    [SerializeField] private GameObject m_Chest;
    [SerializeField] private GameObject m_ChestAppear3;
    [SerializeField] private GameObject m_ChestAppear8;
    [SerializeField] private GameObject m_ChestAppear12;
    [SerializeField] private GameObject m_UISmoke;
    [SerializeField] private GameObject m_PointClick;
    [SerializeField] private GameObject m_ParticleChestReady;

    // Values
    private ICallback.CallFunc2<int> OnclickCB = null;
    public HomeChestProps dataHomeChest = null;
    private const float CHEST_ANIM = 1.17f;
    private ITimeDelta timeChestRemain = 0;
    private bool isSetTimeChest = false;


    // Methods
    public HomeChest SetData(HomeChestProps props)
    {
        dataHomeChest = props;
        timeChestRemain = dataHomeChest.remainTime / 1000;
        isSetTimeChest = dataHomeChest.remainTime > 0;

        for (int i = 0; i < m_ListStatus.Count; i++)
            m_ListStatus[i].SetActive(false);

        for (int i = 0; i < m_ListState.Count; i++)
            m_ListState[i].gameObject.SetActive(false);

        m_ListStatus[(int)dataHomeChest.status].SetActive(true);
        m_ListState[(int)dataHomeChest.status].SetActive(true);

        switch (dataHomeChest.status)
        {
            case ChestStatus.CHEST_EMPTY:
                {
                    break;
                }
            case ChestStatus.CHEST_OPENING:
                {
                    m_TextPriceOpen.text = dataHomeChest.gemPrice + "";
                    break;
                }
            case ChestStatus.CHEST_OPEN_NOW:
                {   
                    break;
                }
            case ChestStatus.CHEST_CLAIM:
                {
                    break;
                }
        }


        switch (dataHomeChest.timeChest)
        {
            case TimeChest.TIME_3H:
                {
                    if (dataHomeChest.isNew)
                    {
                        m_ListChest[0].SetActive(false);
                        m_ListChest[1].SetActive(false);
                        m_ListChest[2].SetActive(false);
                        m_ChestAppear3.SetActive(true);
                        StartCoroutine(IUtil.Delay(() =>
                        {
                            m_ChestAppear3.SetActive(false);
                            m_ListChest[0].SetActive(true);
                            m_UISmoke.SetActive(true);
                            StartCoroutine(IUtil.Delay(() =>
                            {
                                m_UISmoke.SetActive(false);
                            }, 0.5f));
                        }, CHEST_ANIM));
                    }
                    else
                    {
                        m_ListChest[0].SetActive(true);
                        m_ListChest[1].SetActive(false);
                        m_ListChest[2].SetActive(false);
                    }
                    if (dataHomeChest.status == ChestStatus.CHEST_CLAIM)
                    {
                        SC_Wiggle script = m_ListChest[0].GetComponent<SC_Wiggle>();
                        if (script != null)
                            script.enabled = true;
                    }
                    break;
                }
            case TimeChest.TIME_8H:
                {
                    if (dataHomeChest.isNew)
                    {
                        m_ListChest[0].SetActive(false);
                        m_ListChest[1].SetActive(false);
                        m_ListChest[2].SetActive(false);
                        m_ChestAppear8.SetActive(true);
                        StartCoroutine(IUtil.Delay(() =>
                        {
                            m_ChestAppear8.SetActive(false);
                            m_ListChest[1].SetActive(true);
                            m_UISmoke.SetActive(true);
                            StartCoroutine(IUtil.Delay(() =>
                            {
                                m_UISmoke.SetActive(false);
                            }, 0.5f));
                        }, CHEST_ANIM));
                    }
                    else
                    {
                        m_ListChest[0].SetActive(false);
                        m_ListChest[1].SetActive(true);
                        m_ListChest[2].SetActive(false);
                    }
                    if (dataHomeChest.status == ChestStatus.CHEST_CLAIM)
                    {
                        SC_Wiggle script = m_ListChest[1].GetComponent<SC_Wiggle>();
                        if (script != null)
                            script.enabled = true;
                    }
                    break;
                }
            case TimeChest.TIME_12H:
                {
                    if (dataHomeChest.isNew)
                    {
                        m_ListChest[0].SetActive(false);
                        m_ListChest[1].SetActive(false);
                        m_ListChest[2].SetActive(false);
                        m_ChestAppear12.SetActive(dataHomeChest.isNew);
                        StartCoroutine(IUtil.Delay(() =>
                        {
                            m_ChestAppear12.SetActive(false);
                            m_ListChest[2].SetActive(true);
                            m_UISmoke.SetActive(true);
                            StartCoroutine(IUtil.Delay(() =>
                            {
                                m_UISmoke.SetActive(false);
                            }, 0.5f));
                        }, CHEST_ANIM));
                    }
                    else
                    {
                        m_ListChest[0].SetActive(false);
                        m_ListChest[1].SetActive(false);
                        m_ListChest[2].SetActive(true);
                    }
                    if (dataHomeChest.status == ChestStatus.CHEST_CLAIM)
                    {
                        SC_Wiggle script = m_ListChest[2].GetComponent<SC_Wiggle>();
                        if (script != null)
                            script.enabled = true;
                    }
                    break;
                }
            default:
                {
                    m_ListChest[0].SetActive(false);
                    m_ListChest[1].SetActive(false);
                    m_ListChest[2].SetActive(false);
                    break;
                }
        }
        if(m_PointClick != null)
        {
            m_PointClick.SetActive(props.turnOnPointClick);
        }    

        return this;
    }

    public HomeChest SetOnclickCB(ICallback.CallFunc2<int> func)
    {
        OnclickCB = func;
        return this;
    }
    public void DoClick()
    {
        if(dataHomeChest != null)
        {
            switch (dataHomeChest.status)
            {
                case ChestStatus.CHEST_EMPTY:
                    {
                        break;
                    }
                case ChestStatus.CHEST_OPENING:
                case ChestStatus.CHEST_OPEN_NOW:
                    {
                        if (OnclickCB != null)
                            OnclickCB(dataHomeChest.id);
                        break;
                    }
                case ChestStatus.CHEST_CLAIM:
                    {
                        if (ProgressionController.instance == null)
                        {
                            Game.main.socket.OpenTimeChest(dataHomeChest.id);
                        }
                        else
                        {
                            ProgressionController.instance.DoActionInState();
                        }
                        break;
                    }
            }
        } else
        {
            return;
        }
    }
    private void Update()
    {
        if (isSetTimeChest)
        {
            if (timeChestRemain > 0)
            {
                timeChestRemain.MakeTimePassInSeconds();
                if (timeChestRemain <= 0)
                {
                    if (ProgressionController.instance != null)
                    {
                        if (GameData.main.userProgressionState < 6)
                        {
                            isSetTimeChest = false;
                            HomeChestProps hc = new HomeChestProps();
                            hc.state = HomeChestProps.ChestState.ACTIVATED;
                            hc.status = HomeChestProps.ChestStatus.CHEST_CLAIM;
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
                            HomeSceneNew.instance.UpdateHomeChestTut();
                        }
                       else
                        {
                            isSetTimeChest = false;
                            HomeChestProps hc = new HomeChestProps();
                            hc.state = HomeChestProps.ChestState.ACTIVATED;
                            hc.status = HomeChestProps.ChestStatus.CHEST_CLAIM;
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
                            HomeSceneNew.instance.UpdateHomeChestTut();
                        }    
                    }
                    else
                    {
                        Game.main.socket.GetUserTray();
                        isSetTimeChest = false;
                    }
                }
                TimeSpan ts = TimeSpan.FromSeconds(timeChestRemain.time);
                if(timeChestRemain.time <= 60)
                {
                    m_TextTimeLeft.text = string.Format("{0}s", ts.Seconds);
                } else
                {
                    m_TextTimeLeft.text = string.Format("{0}h {1}min", ts.Hours, ts.Minutes);
                }
                
            }
        }
    }
}
