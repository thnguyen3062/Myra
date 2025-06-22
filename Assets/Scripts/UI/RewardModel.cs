using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardModel 
{
   public long gold = 0;
   public long deck = 0;
   public long exp = 0;
   public long essence = 0;
   public bool hasTimeChest = false;
   public long timeChest = 0;
   public string timeChestText = ""; 
   public long eloAdd = 0;
   public long currentGold = 0;
   public long currentEssence = 0;
   public long currentElo = 0;
   public long currentRank = 0;
   public long currentExp = 0;
   public bool rewardNewbie = false;
   public bool rewardNewbieEffect = false;
   public string rewardNewbieImg = "";
}
public class RewardUIModel
{
    public List<HeroCard> lstCard = new List<HeroCard>();
    public List<UserItem> lstItem = new List<UserItem>();   
    public long goldNumber = 0;
}
public class CardRewardEndGame
{
    public long heroId;
    public long count;

}