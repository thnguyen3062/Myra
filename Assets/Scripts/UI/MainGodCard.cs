using GIKCore.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainGodCard : MonoBehaviour
{
    [SerializeField] private Image m_Img;
  //  [SerializeField] private TextMeshProUGUI m_TxtHP, m_TxtAtk, m_TxtMana;
    public HeroCard data;
    private ICallback.CallFunc2<HeroCard> onClickCB = null;
    public void SetOnClickCallback(ICallback.CallFunc2<HeroCard> func) { onClickCB = func; }
    public void InitData(HeroCard hc)
    {
        this.data = hc;
        DBHero db= hc.GetDatabase();
        if(db!=null)
        {
            m_Img.sprite = CardData.Instance.GetGodCardNewSprite(db.id);
            //m_TxtHP.text = db.hp.ToString();
            //m_TxtAtk.text = db.atk.ToString();
            //m_TxtMana.text = db.mana.ToString();
            //if (data.isMain)
            //{
            //    m_Img.color = new Color(1, 1, 1);
            //}
            //else
            //{
            //    m_Img.color = new Color(0.5f, 0.5f, 0.5f);
            //}

        }
        

    }
    public void DoClick()
    {
        if (data != null && onClickCB != null)
            onClickCB(data);
    }
    public void CheckImg()
    {
            m_Img.color = new Color(1, 1, 1);
    }
}
