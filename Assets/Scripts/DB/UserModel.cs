using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pbdson;

public class UserModel
{
    public long userID { get; private set; } = 0;
    public string username { get; private set; } = "";
    public string screenname { get; private set; } = "";

    public void SetData(long id, string username, string screenname)
    {
        this.userID = id;
        this.username = username;
        this.screenname = screenname;
    }

    public string displayName
    {
        get { return string.IsNullOrEmpty(screenname) ? username : screenname; }
    }
}

public class HeroCard
{
    // Values
    public long id;
    /// <summary>id của hero trong database</summary>
    public long heroId;
    /// <summary>id của hero trong battle</summary>
    public long battleId;
    /// <summary>Số lượng trong battle</summary>
    public long number;
    /// <summary>Có mệt hay không?</summary>
    public bool isTired;
    /// <summary>Hiện có bao nhiêu shard</summary>
    public int currentShard;
    public long frame;
    public CardType cardType;
    public enum CardType
    {
        normal,
        nft,
    }

    // Methods
    /// <summary>Get Database of this hero card</summary>    
    public DBHero GetDatabase()
    {
        DBHero db = Database.GetHero(heroId);
        return db;
    }
    public HeroCard Clone()
    {
        HeroCard hc = new HeroCard();
        hc.id = id;
        hc.heroId = heroId;
        hc.battleId = battleId;
        hc.number = number;

        return hc;
    }

    /// <summary>Trả về hero card có id tương ứng</summary>
    public static HeroCard GetHeroCard(List<HeroCard> target, long id)
    {
        foreach (HeroCard hc in target)
        {
            if (hc.id == id)
                return hc;
        }
        return null;
    }
    /// <summary>Trả về hero card có battle id tương ứng</summary>
    public static HeroCard GetHeroCardBattle(List<HeroCard> target, long battleId)
    {
        foreach (HeroCard hc in target)
        {
            if (hc.battleId == battleId)
                return hc;
        }
        return null;
    }
    /// <summary>Trả về danh sách hero card có type (Thần, Lính thường, Lính phép) tương ứng</summary>    
    public static List<HeroCard> GetHeroCardByType(List<HeroCard> target, long type)
    {
        List<HeroCard> lst = new List<HeroCard>();
        foreach (HeroCard hc in target)
        {
            DBHero db = hc.GetDatabase();
            if (db != null && db.type == type)
                lst.Add(hc);
        }
        return lst;
    }
    /// <summary>Trả về danh sách hero card có heroNumber (xác định cùng 1 loại hero) tương ứng</summary>  
    public static List<HeroCard> GetHeroCardByHeroNumber(List<HeroCard> target, long heroNumber)
    {
        List<HeroCard> lst = new List<HeroCard>();
        foreach (HeroCard hc in target)
        {
            DBHero db = hc.GetDatabase();
            if (db != null && db.heroNumber == heroNumber)
                lst.Add(hc);
        }
        return lst;
    }
    public static List<HeroCard> GetHeroCardByHeroID(List<HeroCard> target, long heroID)
    {
        List<HeroCard> lst = new List<HeroCard>();
        foreach (HeroCard hc in target)
        {
            DBHero db = hc.GetDatabase();
            if (db != null && db.id == heroID)
                lst.Add(hc);
        }
        return lst;
    }
    public static int CountHeroCardSameHeroNumber(List<HeroCard> target, long heroNumber)
    {
        int count = 0;
        foreach (HeroCard hc in target)
        {
            DBHero db = hc.GetDatabase();
            if (db != null && db.heroNumber == heroNumber)
                count++;
        }
        return count;
    }
    public static int CountHeroCardSameHeroId(List<HeroCard> target, long heroId)
    {
        int count = 0;
        foreach (HeroCard hc in target)
        {
            DBHero db = hc.GetDatabase();
            if (db != null && db.id == heroId)
                count++;
        }
        return count;
    }
}

public class SystemEvent
{
    public int id;
    public long end;
    public int position;
    public string banner;
    public string slide;
    public string link;
    public Sprite spriteBanner;
    public Sprite spriteSlider;
}
public class BattleDeck
{
    // Values
    /// <summary> List of normal trooper or magic trooper on battle </summary>
    public List<HeroCard> lstTrooper = new List<HeroCard>();
    /// <summary> List of god on battle </summary>
    public List<HeroCard> lstGod = new List<HeroCard>();

    // Methods
    public void Parse(ListCommonVector lcv)
    {
        if (lcv == null || lcv.aVector.Count < 2) return;

        CommonVector cv1 = lcv.aVector[0];//trooper
        CommonVector cv2 = lcv.aVector[1];//god

        void Func(List<HeroCard> target, List<long> aLong)
        {
            target.Clear();
            for (int i = 0; i < aLong.Count; i += 3)
            {
                HeroCard hc = new HeroCard();
                hc.id = aLong[i];
                hc.heroId = aLong[i + 1];
                hc.number = aLong[i + 2];

                target.Add(hc);
            }
        }

        // #trooper        
        Func(lstTrooper, cv1.aLong);

        // #god        
        Func(lstGod, cv2.aLong);
    }

    /// <summary>Trả về toàn bộ lá bài trong battle deck. Nhân bản (clone) theo số lượng (number) tương ứng</summary>    
    public List<HeroCard> GetAllClone()
    {
        void Func(HeroCard target, List<HeroCard> output)
        {
            for (int i = 0; i < target.number; i++)
            {
                HeroCard clone = target.Clone();
                clone.number = 1;
                output.Add(clone);
            }
        }

        List<HeroCard> lst = new List<HeroCard>();
        foreach (HeroCard hc in lstGod)
        {
            Func(hc, lst);
        }
        foreach (HeroCard hc in lstTrooper)
        {
            Func(hc, lst);
        }
        return lst;
    }
    /// <summary>Trả về DBHero của toàn bộ lá bài trong battle deck</summary>    
    public List<DBHero> GetDatabases()
    {
        void Func(List<HeroCard> target, List<DBHero> output)
        {
            foreach (HeroCard hc in target)
            {
                DBHero db = hc.GetDatabase();
                if (db != null)
                {
                    for (int i = 0; i < hc.number; i++)
                    {
                        output.Add(db);
                    }
                }
            }
        }

        List<DBHero> lst = new List<DBHero>();
        Func(lstGod, lst);
        Func(lstTrooper, lst);

        return lst;
    }
}

public class DeckInfo
{
    public long deckID = 0;
    public long deckStatus = 0; // 0 - unavailable, 1 = available
    public bool isDefaultDeck;
    public bool isLastDeck;
    public bool isSelected = false;
    public string deckName = "";
    public long godId = 0;
    public long[] lstGodIds = new long[3];
}
public class Packsinfor
{
    public int Price;
    public string img;
    public string txt= "";
}

public class BattlePlayer
{
    public long id, position, clientPostion, state, mana, towerHealth;
    public string username, screenname;
    public bool isFirst = false;
}

public class CellHeroCardUser
{
    public int cardType;
    public int heroId;
    public List<HeroCard> lst = new List<HeroCard>();
    public List<HeroCard> lstHeroCardIDNow = new List<HeroCard>();

}

public class CellDeckCard
{
    public int cardType;
    public int heroId;
    public List<HeroCard> lst = new List<HeroCard>();
    public bool checkTrooper = true, checkMaxCard=true,check4God= true, checkBuffCard = true;
}

// Packs 
public class Currency
{
    public long gold;
    public long gem;
    public long myra;
    public long essence;
    public long shard;
    public long exp;
}
public class OpenScene
{
    public const int HOME_SCENE = 1;

    public const int SELECT_MODE_SCENE = 10;
    public const int SELECT_MODE_SCENE_REWARDS = 11;
    public const int SELECT_MODE_SCENE_LEADERBOARD = 12;

    public const int SELECT_DECK_SCENE_NORMAL = 20;
    public const int SELECT_DECK_SCENE_RANK = 21;

    public const int COLLECTION_SCENE_DECKS = 30;
    public const int COLLECTION_SCENE_PACKS = 31;
    public const int COLLECTION_SCENE_VALUABLES = 32;
    public const int COLLECTION_SCENE_COSMETICS = 33;

    public const int QUEST_SCENE = 40;

    public const int SHOP_SCENE_PACKS = 50;
    public const int SHOP_SCENE_GEM_TOPUPS = 51;
    public const int SHOP_SCENE_VALUABLES = 52;
    public const int SHOP_SCENE_COSMETICS = 53;
}