using GIKCore.Bundle;
using GIKCore.Net;
using GIKCore.UI;
using GIKCore.Utilities;
using pbdson;
using Spine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UIEngine.UIPool;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class HomeRewardItemProps
{
    public int id;
    public int state; // 0 => chua mo; 1 => da unlock, chua nhan; 2 => da nhan
    public int matchAlwaysCoin;
    public int matchAlwaysCup;
    public int matchAlwaysEssence;
    public int matchTimeChestCoin;
    public int matchTimeChestCup;
    public int matchTimeChestEssence;
    public string desc;
    public bool hasCardUnlock;
    public List<int> lstHeroCardIds = new List<int>();
    public string itemUrl;
}

public class HomeRewardItem : MonoBehaviour
{
    // Fields
    [SerializeField] private TextMeshProUGUI m_ItemName;
    [SerializeField] private SC_StoneAnim SC_StoneAnim;
    [SerializeField] private Image m_ImgIcon;
    [SerializeField] private Image m_ImgItem;
    [SerializeField] private GameObject m_InforStone;
    [SerializeField] private RecycleLayoutGroup m_ListUnlockCard;
    [SerializeField] private GoOnOff m_BoxLevel;
    [SerializeField] private TextMeshProUGUI m_TextLevelOn;
    [SerializeField] private TextMeshProUGUI m_TextLevelOff;
    [SerializeField] private GameObject m_CheckTick;
    [SerializeField] private SC_StoneAnim m_StoneAnim;
    [SerializeField] private SC_StoneAnim m_GlowAnim;
    [SerializeField] private GameObject m_Blur;
    [SerializeField] private GameObject m_BlurStone;

    // Values
    private HomeRewardItemProps dataProps;
    private ICallback.CallFunc2<HomeRewardItemProps> OnClickCB = null;

    // Methods
    public HomeRewardItem SetData(HomeRewardItemProps data)
    {
        dataProps = data;
        if (GameData.main.DictRewardSprite.ContainsKey(data.itemUrl) && GameData.main.DictRewardSprite[data.itemUrl] != null)
        {
            m_ImgItem.sprite = GameData.main.DictRewardSprite[data.itemUrl];
        }
        string sprite = "" + (dataProps.id % 36 == 0 ? 36 : dataProps.id % 36);
        m_ImgIcon.sprite = BundleHandler.LoadSprite("rewarddecor", sprite);
        SC_StoneAnim.gameObject.SetActive(dataProps.state == 1);
        if(dataProps.state == 1)
        {
            //m_StoneAnim.PlayAnim();
            //m_GlowAnim.PlayAnim();
        } else
        {
            //m_StoneAnim.StopAnim();
            //m_GlowAnim.StopAnim();
        }
        m_Blur.SetActive(dataProps.state == 0);
        if (m_BlurStone != null)
            m_BlurStone.SetActive(dataProps.state == 0);
        m_BoxLevel.Turn(dataProps.state == 1 || dataProps.state == 2);
        m_CheckTick.SetActive(dataProps.state == 2);
        m_TextLevelOn.text = dataProps.id + "";
        m_TextLevelOff.text = dataProps.id + "";
        m_ItemName.text = dataProps.desc;

        if (data.hasCardUnlock && m_InforStone != null)
        {
            m_ListUnlockCard.SetCellDataCallback((GameObject go, int data, int index) =>
            {
                HomeRewardStoneCard script = go.GetComponent<HomeRewardStoneCard>();
                script.SetData(data);
            });
            m_ListUnlockCard.SetAdapter(dataProps.lstHeroCardIds);

            m_InforStone.SetActive(true);
        }
        return this;
    }
    public HomeRewardItem SetOnclickCB(ICallback.CallFunc2<HomeRewardItemProps> func)
    {
        OnClickCB = func;
        return this;
    }
    public void DoClickShowInfor()
    {
        if (OnClickCB != null)
            OnClickCB(dataProps);
    }
}
