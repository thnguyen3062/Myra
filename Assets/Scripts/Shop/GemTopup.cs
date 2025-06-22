using GIKCore;
using GIKCore.Sound;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GemTopup : MonoBehaviour
{
    // Fields
    [SerializeField] private TextMeshProUGUI m_Price, m_Gem;
    [SerializeField] private List<GameObject> m_ListGemIcon = new List<GameObject>();

    // Values
    private string skuId;

    // Methods
    public void SetData(IPurchaserProduct data)
    {
        skuId = data.storeSpecificId;
        foreach (GameObject go in m_ListGemIcon)
        {
            go.SetActive(false);
        }
        JSONNode jN = JSON.Parse(data.localizedDescription);
        JSONObject jO = jN.AsObject;
        if (jO != null)
        {
            m_Gem.text = "x" + jO["desc"].Value;
        }
        else
        {
            m_Gem.text = "";
        }
        int iconIdx = 0;
        if (jO["icon"] != null)
            iconIdx = jO["icon"].AsInt;

        Debug.Log("iconIdx: " + iconIdx);
        m_ListGemIcon[iconIdx].SetActive(true);
        m_Price.text = data.localizedPriceString;
    }
    public void DoClickBuy()
    {
        SoundHandler.main.PlaySFX("900_click_3", "sounds");
        Game.main.IAP.BuyProduct(skuId);
    }
}
