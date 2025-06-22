using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankModel
{
    public long rankSeasonID = 0;
    public long rankSeasonName;
    public long timeRemain = 0;

    public bool isFirstTime = false;
    public string seasonImage = "";
    public bool lastElo;
    public long lastRank;
    public long curUserRank;
    public long curUserElo;

}
public class CardRewardRank
{
    public long heroID;
    public long frame;
    public string cardImg="";
    public int count;
}
public class ItemRewardRank
{
    public long itemID;
    public string itemImg ="";
    public long count;
}

public class RewardRank
{
    public long rankSeasonID = 0;
    public long rankID = 0;
    public long gold = 0;
    public CardRewardRank card = null;
    public ItemRewardRank item = null;
    public bool isAchieved = false;
    public bool isReceive = false;
}
