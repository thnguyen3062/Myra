using GIKCore;
using GIKCore.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.PostProcessing.SubpixelMorphologicalAntialiasing;

public class PopupPurchaseItemValuable : MonoBehaviour
{
    // Fields
    [SerializeField] private Image m_ItemSpite;
    [SerializeField] private TextMeshProUGUI m_ItemTitle, m_ItemDesc, m_ItemQuanlity, m_ItemPrice;
    [SerializeField] private GoOnOff m_Currency;

    [SerializeField] private TMP_InputField m_InputField;
    // Values
    private int quantity = 1;
    private ItemInfo itemInfo;

    // Methods
    public void SetData(ItemInfo data)
    {
        itemInfo = data;
        quantity = 1;
        m_InputField.text = quantity.ToString();
        m_ItemSpite.sprite = data.sprite;
        m_ItemTitle.text = data.name;
        m_ItemDesc.text = data.desc;
        m_ItemQuanlity.text = "1";
        m_ItemPrice.text = data.price + "";
        m_Currency.Turn(data.currency == 1);
        gameObject.SetActive(true);
    }

    private void Update()
    {
        SetDataPurchs();
    }

    public void SetDataPurchs()
    {
        bool check = true;
        try
        {
            quantity = int.Parse(m_InputField.text);
        }
        catch
        {
            quantity = 1;
            m_InputField.text = quantity.ToString();
            check = false;
        }
        if(quantity > 99 || quantity < 1 )
        {
            quantity = 1;
            m_InputField.text = quantity.ToString();
           
        }
        m_InputField.text = quantity.ToString();
        if (check)
        {
            m_ItemPrice.text = itemInfo.price * quantity + "";
        }
    }
    public void DoClickPurchase()
    {
        Game.main.socket.BuyItem(itemInfo.shopItemId, quantity);
    }
    public void DoClickClose()
    {
        gameObject.SetActive(false);
    }
    public void DoClickQuantity(int type) // 0 minus, 1 plus
    {
        if(type == 0)
        {
            quantity -= (quantity == 1 ? 0 : 1);
           
        } else
        {
            quantity += (quantity == 99 ? 0 : 1);
        }
        m_InputField.text = quantity.ToString();
    }

}
