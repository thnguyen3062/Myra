using GIKCore;
using GIKCore.Net;
using GIKCore.Sound;
using GIKCore.UI;
using GIKCore.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserPackShop : MonoBehaviour
{
    // Fields
    [SerializeField] private TextMeshProUGUI m_ItemName, m_ItemCost, m_ItemSale, m_ItemExpire;
    [SerializeField] private Image m_PackImage, m_PackCurrency;
    [SerializeField] private GameObject m_BannerSale, m_BannerExpire, m_BannerNew, m_IconInfo;
    [SerializeField] private GoOnOff m_Currency;

    // Values
    IEnumerator coroutine = null;
    public ItemInfo itemInfo;

    // Methods
    public void SetData(ItemInfo data)
    {
        if (m_PackImage != null)
            m_PackImage.GetComponent<CanvasGroup>().alpha = 0.3f;
        itemInfo = data;

        if (m_ItemName != null)
            m_ItemName.text = data.name;

        if (m_ItemCost != null)
            m_ItemCost.text = data.price + "";
        if (m_ItemSale != null)
            m_ItemSale.text = "-" + data.percent + "%";
        if (m_ItemExpire != null)
            m_ItemExpire.text = "Expires in <color=#FFC968>" + ITimer.GetTimeDisplay(data.endOffset / 1000, ITimeSpanFormat.D) + "</color> days";
        if (m_Currency != null)
            m_Currency.Turn(data.currency == 1);

        if (m_BannerExpire != null)
            m_BannerExpire.SetActive(data.endOffset <= 259200000); // < 3 days
        if (m_BannerSale != null)
            m_BannerSale.SetActive(data.isDiscount);
        if (m_BannerNew != null)
            m_BannerNew.SetActive(data.isNew);
        if (m_IconInfo != null)
            m_IconInfo.SetActive(false);

        if (data.sprite != null) SetAvatarSprite(data.sprite);
        else LoadHttpAvatar(data.image, (s) => { data.sprite = s; });
    }
    public void LoadHttpAvatar(string url, ICallback.CallFunc2<Sprite> onLoaded = null)
    {
        if (coroutine != null)
            Game.main.StopCoroutine(coroutine);

        if (string.IsNullOrEmpty(url)) return;

        coroutine = IUtil.LoadTexture2DFromUrl(url, (Texture2D tex) =>
        {
            if (tex == null) return;
            Sprite sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            SetAvatarSprite(sprite);

            if (onLoaded != null) onLoaded(sprite);
        });
        Game.main.StartCoroutine(coroutine);
    }
    private void SetAvatarSprite(Sprite sprite)
    {
        m_PackImage.sprite = sprite;
        m_PackImage.GetComponent<CanvasGroup>().alpha = 1f;
    }
    public void DoClickItem()
    {
        SoundHandler.main.PlaySFX("900_click_6", "sounds");
        HandleNetData.QueueNetData(NetData.SHOP_CLICK_ITEM, itemInfo);
    }
}
