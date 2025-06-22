using GIKCore.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardOnPack : MonoBehaviour
{
    [SerializeField] private Image m_GodIcon;
    [SerializeField] private Image m_filter;
    [SerializeField] private TextMeshProUGUI m_NameCard,m_Mana;
    DBHero data;
    private long frameC;
    private ICallback.CallFunc3<long,long> onClickCB = null;
    public void SetOnClickCallback(ICallback.CallFunc3<long,long> func) { onClickCB = func; }
    public void DoClick()
    {
        if (data != null && onClickCB != null)
            onClickCB(data.id,frameC);
        else return;
    }
    public void InitData(long id,long frame)
    {
        DBHero db = Database.GetHero(id);
        if (db != null)
        {
            data = db;
            this.frameC = frame;
            m_filter.sprite = CardData.Instance.GetFilterCardSprite(db.color);
            m_GodIcon.sprite = CardData.Instance.GetOnBoardSprite(db.id);
            m_NameCard.text = db.name.ToString();
            m_Mana.text = db.mana.ToString();
        }
    }
}
