using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using GIKCore.Lang;

public class PackageBuy : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI namePackage;
    [SerializeField] private TextMeshProUGUI price;
    [SerializeField] private Image currencyImg;
    [SerializeField] private Sprite[] currency;
    PackagesBuy package;
    public void InnitPackages(PackagesBuy data)
    {
        if (data == null)
            return;

        package = data;
        namePackage.text = LangHandler.Get("141", "PACK ") + package.countItem.ToString();
        price.text =  package.price.ToString();
        currencyImg.sprite = currency[package.currency];
    }
    public void DoClickPackage()
    {
        PackShopController.instance.selectedPackID = this.package.packageBuyId;
    }    
}
