using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PackInfo
{
    public long packId;
    public string packName;
    public long quantity;
}
public class ItemInfo
{
    public int shopItemId;
    public int type;
    public int itemId;
    public bool isShow;
    public bool isNew;
    public long startOffset;
    public long endOffset;
    public bool isDiscount;
    public int currency;
    public int price;
    public int percent;
    public string name;
    public string desc;
    public string image;
    public Sprite sprite;
}
public class ItemPackage
{
    public int idx;
    public int count;
    public int currency;
    public int price;
    public int limit;
    public bool isSelected;
}
public class ItemReward
{
    public int kind;
    public int id;
    public int frame;
    public int count;
}
public class UserItem
{
    public int itemId;
    public long expire;
    public int quantity;
    public string name;
    public string image;
    public Sprite sprite;
    public int type; // 1 => pack, 2 => item
}
public class PackDetail
{
    public ItemInfo itemInfo;
    public List<PackagesBuy> lstPackage = new List<PackagesBuy>();
    public List<long> heroIds = new List<long>();
}

public class PackagesBuy
{
    public long packageBuyId;
    public long countItem;
    public long price;
    // currency : gold = 0; gem =1 
    public long currency;
}
public class TypeItems
{
    public const int TYPE_PACK_SHOP = 1;
    public const int TYPE_VALUABLE_SHOP = 2;
    public const int TYPE_PACK = 60;
}

