using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIEngine.Extensions;
using UnityEngine.UI;
using TMPro;
using GIKCore.Utilities;
using GIKCore.UI;
using GIKCore.Lang;

public class PackShop : MonoBehaviour
{
    [SerializeField] private Image packImg;
    [SerializeField] private TextMeshProUGUI m_name;

    private ItemInfo data;
    private ICallback.CallFunc2<ItemInfo> onClickCB;

    public long CurrentPackID
    {
        get;
        private set;
    }
    public void SetOnClickCallback(ICallback.CallFunc2<ItemInfo> func) { onClickCB = func; }
    public void DoClick()
    {
        if (data != null && onClickCB != null)
            onClickCB(data);
        else
           Toast.Show(LangHandler.Get("162","DATA NOT FOUND"));
    }
    public void InitData(ItemInfo data)
    {
        if (data == null)
            return;
        if (data.type != 60)
            return;
        this.data = data;
        CurrentPackID = data.itemId;
        m_name.text = data.name;
    }
}
