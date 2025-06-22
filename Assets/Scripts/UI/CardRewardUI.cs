using DG.Tweening;
using GIKCore.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRewardUI : MonoBehaviour
{
    // Fields
    [SerializeField] private GameObject m_God, m_Minion, m_Spell;
    [SerializeField] private GameObject m_BackCard, m_Invisible, m_FlipCard;

    // Values
    HeroCard data;
    public bool isOpen = false;
    public int cardType = -1;

    private ICallback.CallFunc2<HeroCard> onClickCB = null;
    public void SetOnClickCallback(ICallback.CallFunc2<HeroCard> func) { onClickCB = func; }
    public void DoClick()
    {
        if (data != null && onClickCB != null)
            onClickCB(data);
    }
    public void SetData(HeroCard hc, bool open = false)
    {
        data = hc;
        DBHero db = hc.GetDatabase();
        cardType = (int)db.type;
        isOpen = open;
        if (db != null)
        {
            if (db.disable > 0)
            {
                return;
            }
            if (db.type == DBHero.TYPE_GOD)
            {
                m_God.SetActive(true);
                m_God.GetComponent<CardUserInfor>().SetInfoCard(db, 1, (int)hc.frame);
                m_Minion.SetActive(false);
                m_Spell.SetActive(false);
                m_BackCard.SetActive(false);
            }
            if (db.type == DBHero.TYPE_TROOPER_MAGIC || db.type == DBHero.TYPE_BUFF_MAGIC)
            {
                m_God.SetActive(false);
                m_Minion.SetActive(false);
                m_Spell.SetActive(true);
                m_Spell.GetComponent<CardUserInfor>().SetInfoCard(db, 1, (int)hc.frame);
                m_BackCard.SetActive(false);
            }
            if (db.type == DBHero.TYPE_TROOPER_NORMAL)
            {
                m_God.SetActive(false);
                m_Minion.SetActive(true);
                m_Spell.SetActive(false);
                m_Minion.GetComponent<CardUserInfor>().SetInfoCard(db, 1, (int)hc.frame);
                m_BackCard.SetActive(false);
            }
        }
        if (!isOpen)
        {
            m_God.SetActive(false);
            m_Minion.SetActive(false);
            m_Spell.SetActive(false);
            m_BackCard.SetActive(true);
        }
    }
    public void SetInvisible()
    {
        m_God.SetActive(false);
        m_Minion.SetActive(false);
        m_Spell.SetActive(false);
        m_BackCard.SetActive(false);
    }
    public void SetOpen()
    {
        //transform.Rotate(0, 0, 0);
        m_BackCard.SetActive(false);
        switch (cardType)
        {
            case (int)DBHero.TYPE_GOD:
                {
                    m_God.SetActive(true);
                    break;
                }
            case (int)DBHero.TYPE_TROOPER_MAGIC:
                {
                    m_Spell.SetActive(true);
                    break;
                }
            case (int)DBHero.TYPE_TROOPER_NORMAL:
                {
                    m_Minion.SetActive(true);
                    break;
                }
        }
    }
    public void FlipCard()
    {
        m_FlipCard.SetActive(true);
        StartCoroutine(IUtil.Delay(() =>
        {
            transform.DORotate(new Vector3(0, 180f, 0), 0.4f).SetEase(Ease.OutBounce);
            StartCoroutine(IUtil.Delay(() =>
            {
                SetOpen();
            }, 0.2f));
        }, 0.7f));
    }
    private void Start()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    private float currentRotation;

    private void Update()
    {
        //if(transform.rotation.y >= 90f)
        //{
        //    Debug.Log("Go here");
        //    SetOpen();
        //}
        //if (currentRotation <= 180f)
        //{
        //    currentRotation += 3;
        //    transform.localRotation = Quaternion.Euler(0, currentRotation, 0);
        //    if(currentRotation >= 90f)
        //    {
        //        SetOpen();
        //    }
        //}
        //else
        //{
            
        //}
        //transform.Rotate(0, 3, 0);
        //if (transform.rotation.y >= -93f && transform.rotation.y <= -87f)
        //{
        //    Debug.Log("Go here");
            
        //}
    }
}
