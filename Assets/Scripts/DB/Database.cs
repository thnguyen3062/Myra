using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using GIKCore.Bundle;
using GIKCore.Utilities;
using System;

public class Database
{
    // Values
    public static List<DBHero> lstHero = new List<DBHero>();
    public static List<DBRank> lstRank = new List<DBRank>();
    public static List<DBLevel> lstLevel = new List<DBLevel>();
    public static List<DBKeyword> lstKeyword = new List<DBKeyword>();
    public static List<DBBaseHp> lstBaseHp = new List<DBBaseHp>();
    public static List<DBHeroSkill> lstSkillAuction = new List<DBHeroSkill>();
    public static Dictionary<string, string> dictFirstTime = new Dictionary<string, string>();
    // Methods
    public static DBHero GetHero(long id)
    {
        foreach (DBHero hero in lstHero)
        {
            if (hero.id == id)
                return hero;
        }
        return null;
    }

    public static List<DBHero> GetHeroByType(long type)
    {
        List<DBHero> lst = new List<DBHero>();
        foreach (DBHero hero in lstHero)
        {
            if (hero.type == type)
                lst.Add(hero);
        }
        return lst;
    }
    public static List<DBRank> GetListRank()
    {
        return lstRank;
    }
    public static DBRank GetRank(long id)
    {
        foreach (DBRank rank in lstRank)
        {
            if (rank.id == id)
                return rank;
        }
        return null;
    }
    public static DBRank GetRankByElo(long elo)
    {
        long tmpCompare = elo;
        
        for(int i = 0; i < lstRank.Count; i++)
        {
            if(elo >= lstRank[i].elo )
            {
                long tmp = elo - lstRank[i].elo ;
                if(tmp<tmpCompare)
                {
                    tmpCompare = tmp;
                }    
            }    
        }
        long minElo = elo - tmpCompare;
        foreach(DBRank rank in lstRank)
        {
            if(minElo == rank.elo)
                return rank;
        }    
            
        return null;
    }
    public static List<DBHero> GetHeroByHeroNumber(long heroNumber)
    {
        List<DBHero> lst = new List<DBHero>();
        foreach (DBHero hero in lstHero)
        {
            if (hero.heroNumber == heroNumber)
                lst.Add(hero);
        }

        return lst;
    }
    public static DBLevel GetUserLevelInfo(long curExp)
    {
        DBLevel userLevel = new DBLevel();
        for (int i = 0; i < lstLevel.Count; i++)
        {
            DBLevel level = lstLevel[i];
            userLevel = level;
            if (i < lstLevel.Count - 1)
            {
                DBLevel nextLevel = lstLevel[i + 1];
                if (curExp >= level.exp && curExp < nextLevel.exp)
                {
                    long expRequire = nextLevel.exp - lstLevel[i].exp;

                    userLevel.expToUpLevel = expRequire;
                    userLevel.expCurrent = curExp - lstLevel[i].exp;
                    break;
                }
            }
            else
            {
                userLevel.expToUpLevel = 1;
                userLevel.expCurrent = 1;
            }

        }
        return userLevel;
    }
    public static DBKeyword GetKeywordInfo(string keywordID)
    {
        DBKeyword keyword = new DBKeyword();
        for (int i = 0; i < lstKeyword.Count; i++)
        {
            if (lstKeyword[i].id.Equals(keywordID))
            {
                keyword = lstKeyword[i];
            }
        }
        return keyword;
    }
    public static DBBaseHp GetBaseHp(int shard)
    {
        DBBaseHp hp = new DBBaseHp();
        if (shard >= lstBaseHp[lstBaseHp.Count - 1].shard)
        {
            lstBaseHp[lstBaseHp.Count - 1].shardToUpLevel = lstBaseHp[lstBaseHp.Count - 1].shard;
            lstBaseHp[lstBaseHp.Count - 1].shardCurrent = lstBaseHp[lstBaseHp.Count - 1].shard;
            return lstBaseHp[lstBaseHp.Count - 1];
        }
            
        for (int i = 0; i < lstBaseHp.Count; i++)
        {
            DBBaseHp cur = lstBaseHp[i];
            DBBaseHp next = lstBaseHp[i + 1];
            if (shard >= cur.shard && shard < next.shard)
            {
                long shardRequire = next.shard - cur.shard;

                lstBaseHp[i].shardToUpLevel = shardRequire;
                lstBaseHp[i].shardCurrent = shard - lstBaseHp[i].shard;
                return lstBaseHp[i];
            }
        }
        return hp;
    }
    public static DBHeroSkill GetDBSkillAuction(long skillID)
    {
        DBHeroSkill skill = new DBHeroSkill();
        for (int i = 0; i < lstSkillAuction.Count; i++)
        {
            if (lstSkillAuction[i].id.Equals(skillID))
            {
                skill = lstSkillAuction[i];
            }
        }
        return skill;
    }
    public static string GetContentFirstTime(string key)
    {
        string content = "";
        dictFirstTime.TryGetValue(key, out content);
        return content;
    }
    public static void ParseAll()
    {
        ParseHero();
        ParseHeroSkill();
        ParseRank();
        ParseLevel();
        ParseKeywordData();
        ParseBaseHp();
        ParseFirstTime();
    }
    public static void ParseHero()
    {
        string aJSON = BundleHandler.LoadDatabase("db_hero");
        JSONNode N = JSON.Parse(aJSON);
        JSONArray jArray = N.AsArray;

        for (int i = 0; i < jArray.Count; i++)
        {
            JSONObject o = jArray[i].AsObject;
            DBHero hero = new DBHero();
            hero.id = o["id"].AsLong;
            hero.heroNumber = o["hero_number"].AsLong;
            hero.variation = o["variation"].AsLong;
            hero.name = o["name"].Value;
            hero.type = o["type"].AsLong;
            hero.rarity = o["rarity"].AsLong;
            hero.color = o["color"].AsLong;
            hero.speciesId = o["species_id"].AsLong;
            hero.speciesName = o["species_name"].Value;
            hero.mana = o["mana"].AsLong;
            hero.atk = o["atk"].AsLong;
            hero.hp = o["hp"].AsLong;
            hero.ownerGodID = o["owner_god_id"].AsLong;
            hero.deathCost = o["death_cost"].AsLong;
            hero.collectible = o["collectible"].AsLong;
            hero.cleave = o["cleave"].AsLong;
            hero.breaker = o["breaker"].AsLong;
            hero.overrun = o["overrun"].AsLong;
            hero.combo = o["combo"].AsLong;
            hero.pierce = o["pierce"].AsLong;
            hero.godSlayer = o["god_slayer"].AsLong;
            hero.disable = o["disable"].AsLong;
            hero.virtualHero = o["virtual"].AsLong;
            hero.isFragile = o["fragile"].AsBool;
            string buffSkills = o["buff_skills"].Value;
            JSONNode NB = JSON.Parse(buffSkills);
            if (NB != null)
            {
                JSONArray jBuffShillsArray = NB.AsArray;
                
                if (jBuffShillsArray != null)
                {
                    for (int j = 0; j < jBuffShillsArray.Count; j++)
                    {
                        long oB = jBuffShillsArray[j].AsLong;
                        hero.lstBuffSkillID.Add(oB);
                    }
                }
                else
                {
                }    
            }
            lstHero.Add(hero);
        }
    }
    public static void ParseHeroSkill()
    {

        string aJSON = BundleHandler.LoadDatabase("heroSkill");
        JSONNode N = JSON.Parse(aJSON);
        JSONArray jArray = N.AsArray;

        //Debug.Log(aJSON);
        for (int i = 0; i < jArray.Count; i++)
        {
            JSONObject o = jArray[i].AsObject;
            DBHeroSkill skill = new DBHeroSkill();
            skill.id = o["id"].AsInt;
            skill.hero_id = o["hero_id"].AsInt;
            skill.isBaseSkill= o["base_skill"].AsInt == 1;
            skill.timing = o["timing"].AsInt;
            skill.eventSkill = o["event"].AsInt;
            skill.max_turn = o["max_turn"].AsInt;
            skill.sark_god = o["sark_god"].AsInt;
            //skill.min_shard = o["min_shard"].AsInt;
            //skill.max_shard = o["max_shard"].AsInt;
            skill.skill_type = o["skill_type"].AsInt;
            skill.enable = o["enable"].AsInt;
            skill.isUltiType = o["util_type"].AsInt == 1;
            skill.name_skill = o["name_skill"].Value;
            skill.note = o["note"].Value;
            string effects = o["effects"].Value.Replace("\\", "");
            JSONNode NE = JSON.Parse(effects);
            if (NE != null)
            {
                JSONArray jEffectArray = NE.AsArray;
                for (int j = 0; j < jEffectArray.Count; j++)
                {

                    //JSONObject obj = jEffectArray[j].AsObject;
                    //EffectSkill effectSkill = new EffectSkill();
                    //effectSkill.type = obj["type"].AsInt;
                    //effectSkill.target = obj["target"].AsInt;
                    //if (effectSkill.target >= 1000)
                    //{
                    //    skill.target = effectSkill.target;
                    //    skill.effect_type = effectSkill.type;
                    //}

                    //effectSkill.heroId = obj["heroId"].AsInt;
                    //effectSkill.desc = obj["desc"].Value;
                    //effectSkill.isFragile = obj["max_turn"].AsBool;
                    //skill.lstEffectSkill.Add(effectSkill);

                    //"effects" : "[{\"type\":11,\"target\":18,\"desc\":\"trieu hoi hero\",\"heroId\":43,\"isFragile\":true}]",

                    // "effects": "[{\"info\":0,\"effect\":[{\"type\":3,\"target\":6,\"desc\":\"comsume 2 shard\",\"manaDegre\":0,\"shardIncre\":-2}]},{\"info\":0,\"effect\":[{\"type\":11,\"target\":1026,\"desc\":\"trieu hoi đại kraken\",\"heroId\":45,\"lstHeroId\":[43,44,48],\"typeValueAtk\":4,\"typeValueHP\":4}]}]"

                    JSONObject obj = jEffectArray[j].AsObject;
                    ListEffectsSkill lstEffectsSkill = new ListEffectsSkill();
                    lstEffectsSkill.info = obj["info"].AsInt;
                    JSONNode effect = obj["effect"];
                    //Debug.Log(obj["effect"]);
                    //JSONNode E= JSON.Parse(effect);
                    if (effect != null)
                    {
                        JSONArray jEffectArrayInfo = effect.AsArray;
                        for (int z = 0; z < jEffectArrayInfo.Count; z++)
                        {
                            JSONObject eff = jEffectArrayInfo[z].AsObject;
                            EffectSkill effectSkill = new EffectSkill();
                            effectSkill.type = eff["type"].AsInt;
                            effectSkill.target = eff["target"].AsInt;
                            effectSkill.heroId = eff["heroId"].AsInt;
                            effectSkill.desc = eff["desc"].Value;
                            effectSkill.isFragile = eff["max_turn"].AsBool;
                            lstEffectsSkill.lstEffect.Add(effectSkill);
                        }
                    }
                    skill.lstEffectsSkills.Add(lstEffectsSkill);
                }
            }
            string conditions = o["conditions"].Value.Replace("\\", "");
            JSONNode NC = JSON.Parse(conditions);
            if (NC != null)
            {
                JSONArray jConditionArray = NC.AsArray;
                for (int j = 0; j < jConditionArray.Count; j++)
                {
                    ConditionSkill conditionSkill = new ConditionSkill();
                    conditionSkill.desc = jConditionArray[j]["desc"].Value;
                    conditionSkill.number = jConditionArray[j]["number"].AsInt;
                    conditionSkill.species = jConditionArray[j]["species"].AsInt;
                    conditionSkill.pos = jConditionArray[j]["pos"].AsInt;
                    conditionSkill.type = jConditionArray[j]["type"].AsInt;
                    skill.lstConditionSkill.Add(conditionSkill);
                    //Debug.Log("CONDITION ="+ conditionSkill.type+" "+ conditionSkill.species+" "+ conditionSkill.number);
                    //"conditions" : "[{\"type\":1000,\"desc\":\"không có diều kiện\"}]",

                }
            }
            if(skill.hero_id <=0)
            {
                lstSkillAuction.Add(skill);
            }    
            //Debug.Log("EFF=" + skill.lstEffectSkill.Count);
            //Debug.Log("CON=" + skill.lstConditionSkill.Count);
            foreach (DBHero hero in lstHero)
                if (hero.id == skill.hero_id && skill.enable == 1)
                    hero.lstHeroSkill.Add(skill);

        }
    }
    public static void ParseRank()
    {
        string aJSON = BundleHandler.LoadDatabase("rank");
        //Debug.Log(aJSON);
        JSONNode N = JSON.Parse(aJSON);
        JSONArray jArray = N.AsArray;

        for (int i = 0; i < jArray.Count; i++)
        {
            JSONObject o = jArray[i].AsObject;
            DBRank rank = new DBRank();
            rank.id = o["id"].AsLong;
            rank.name = o["name"].Value;
            rank.elo = o["elo"].AsLong;
            rank.eloReset = o["elo_reset"].AsLong;
            rank.eloParamReset = o["elo_param_reset"].AsLong;
            //hero.skills = o["skills"].Value;
            //Debug.Log("---------------Hero =" + hero.name + " " + hero.color);

            lstRank.Add(rank);
        }
    }
    public static void ParseLevel()
    {
        string aJSON = BundleHandler.LoadDatabase("level");
        //Debug.Log(aJSON);
        JSONNode N = JSON.Parse(aJSON);
        JSONArray jArray = N.AsArray;

        for (int i = 0; i < jArray.Count; i++)
        {
            JSONObject o = jArray[i].AsObject;
            DBLevel level = new DBLevel();
            level.id = o["id"].AsLong;
            level.exp = o["exp"].AsLong;

            lstLevel.Add(level);
        }
    }
    public static void ParseKeywordData()
    {
        string aJSON = BundleHandler.LoadDatabase("keyword");
        //Debug.Log(aJSON);
        JSONNode N = JSON.Parse(aJSON);
        JSONArray jArray = N.AsArray;

        for (int i = 0; i < jArray.Count; i++)
        {
            JSONObject o = jArray[i].AsObject;
            DBKeyword key = new DBKeyword();
            key.id = o["id"].Value;
            key.descriptionEN = o["en"].Value;
            key.descriptionKR = o["kr"].Value;
            key.descriptionJP = o["jp"].Value;
            key.descriptionCN = o["cn"].Value;
            key.descriptionTW = o["tw"].Value;

            lstKeyword.Add(key);
        }
    }
    public static void ParseBaseHp()
    {
        string aJSON = BundleHandler.LoadDatabase("db_base_hp");
        //Debug.Log(aJSON);
        JSONNode N = JSON.Parse(aJSON);
        JSONArray jArray = N.AsArray;

        lstBaseHp.Clear();
        for (int i = 0; i < jArray.Count; i++)
        {
            JSONObject o = jArray[i].AsObject;
            DBBaseHp key = new DBBaseHp();
            key.hp = o["hp"].AsInt;
            key.shard = o["shard"].AsInt;
            lstBaseHp.Add(key);
        }
    }
    public static void ParseFirstTime()
    {
        string aJSON = BundleHandler.LoadDatabase("db_first_time");
        //Debug.Log(aJSON);
        JSONNode N = JSON.Parse(aJSON);
        JSONArray jArray = N.AsArray;
        dictFirstTime.Clear();
        for (int i = 0; i < jArray.Count; i++)
        {
            JSONObject o = jArray[i].AsObject;
            if (!dictFirstTime.ContainsKey(o["key"]))
            {
                dictFirstTime.TryAdd(o["key"], o["value"]);
            }
        }
    }
}

public class DBHero : ICloneable
{
    public const long TYPE_GOD = 0;
    public const long TYPE_TROOPER_NORMAL = 1;
    public const long TYPE_TROOPER_MAGIC = 2;
    public const long TYPE_BUFF_MAGIC = 3;

    public const long COLOR_WHITE = 0;
    public const long COLOR_GREEN = 1;
    public const long COLOR_RED = 2;
    public const long COLOR_YELLOW = 3;
    public const long COLOR_PURPLE = 4;

    private const string REGION_NONE = "";
    private const string REGION_AQUATIC = "AQUATIC";
    private const string REGION_BEAST = "BEAST";
    private const string REGION_NATURE = "NATURE";
    private const string REGION_GIANT = "GIANT";
    private const string REGION_ZODIAC = "ZODIAC";

    public const long KEYWORD_CLEAVE = 1;
    public const long KEYWORD_PIERCE = 2;
    public const long KEYWORD_OVERRUN = 3;
    public const long KEYWORD_BREAKER = 4;
    public const long KEYWORD_GODSLAYER = 5;
    public const long KEYWORD_COMBO = 6;
    public const long KEYWORD_DEFENDER = 7;

    public Dictionary<long, string> cardRegionDict = new Dictionary<long, string>()
    {
        {0, REGION_NONE },
        {1, REGION_AQUATIC },
        {2, REGION_BEAST },
        {3, REGION_NATURE },
        {4, REGION_GIANT },
        {5, REGION_ZODIAC }
    };

    /// <summary>Đánh dấu các hero khác nhau</summary>
    public long id;
    /// <summary>Đánh dấu loại hero; 2 hero cùng loại (cùng heroNumber) nhưng kỹ năng (skill) khác nhau thì id khác nhau</summary>
    public long heroNumber;
    //
    public long variation;
    /// <summary>Đánh dấu độ hiếm của hero
    public long rarity;
    /// <summary>Đánh dấu gia tộc (clan) của hero: Thần, Lính hay Phép</summary>
    public long type;
    /// <summary>Đánh dấu màu sắc của hero: Trắng, Xanh, Đỏ,...</summary>
    public long color;
    /// <summary>Năng lượng cần để triệu hồi</summary>
    public long mana;
    /// <summary>Số TÍCH của Thần cùng loại để có thể ra trận; chỉ áp dụng với lính thường và lính phép</summary>
    //public long shardRequired;
    /// <summary>Số máu trụ bị mất khi hero bị đối phương tiêu diệt</summary>
    public long deathCost, collectible, cleave, breaker, overrun, combo, pierce, disable, virtualHero, godSlayer;
    public long speciesId, atk, hp;
    public string name, speciesName, skills;
    public bool isFragile;
    //danh cho bai buff
    //Than so huu skill
    public long ownerGodID;
    //skill id tuong ung 
    public List<long> lstBuffSkillID = new List<long>();

    public List<DBHeroSkill> lstHeroSkill = new List<DBHeroSkill>();
    public List<DBHero> lstUnlockSkillCard= new List<DBHero>();
    public string GetTypeName()
    {
        if (type == TYPE_TROOPER_NORMAL)
            return "Lính thường";
        else if (type == TYPE_TROOPER_MAGIC)
            return "Lính phép";
        return "THẦN";
    }
    public string GetColorName()
    {
        string colorName = "";
        switch (color)
        {
            case 0:
                colorName = "WHITE";
                break;
            case 1:
                colorName = "GREEN";
                break;
            case 2:
                colorName = "RED";
                break;
            case 3:
                colorName = "YELLOW";
                break;
            case 4:
                colorName = "PURPLE";
                break;


        }
        return colorName;
    }

    public DBHero Clone()
    {
        DBHero clone = new DBHero();
        clone.id = id;
        clone.heroNumber = heroNumber;
        clone.rarity = rarity;
        clone.type = type;
        clone.color = color;
        clone.mana = mana;
        //clone.shardRequired = shardRequired;
        clone.deathCost = deathCost;
        clone.collectible = collectible;
        clone.cleave = cleave;
        clone.breaker = breaker;
        clone.overrun = overrun; clone.combo = combo; clone.pierce = pierce; clone.disable = disable; clone.virtualHero = virtualHero; clone.godSlayer = godSlayer;
        clone.speciesId = speciesId;
        clone.atk = atk;
        clone.hp = hp;
        clone.isFragile = isFragile;
        clone.lstHeroSkill = lstHeroSkill;
        clone.lstUnlockSkillCard = lstUnlockSkillCard;

        return clone;
    }

    object ICloneable.Clone()  
    {
        return this.MemberwiseClone() as DBHero;
    }
}


public class DBHeroSkill 
{

    public const long TYPE_PASSIVE_SKILL = 0;
    public const long TYPE_ACTIVE_SKILL = 1;
    public const long TYPE_SUMMON_SKILL = 2;
     
    public const long OPPOSITE_ENEMY = 1;
    public const long LANE_ENEMIES = 2;
    public const long RANDOM_LANE_ENEMY = 3;
    public const long RANDOM_ENEMY = 4;
    public const long ALL_ENEMIES = 5;
    public const long MY_SELF = 6;
    public const long FRONT_ALLY = 7;
    public const long BACK_ALLY = 8;
    public const long NEARBY_ALLY = 9;
    public const long LANE_ALLIES = 10;
    public const long ALL_ALLIES = 11;
    public const long ANY_OPOSITE_ENEMY_TARGET = 12;
    public const long LANE_ALLY_TOWER = 13;
    public const long LANE_ENEMY_TOWER = 14;
    public const long ALL_UNIT = 15;
    public const long ALL_ENEMY_UNIT_IN_RANDOM_LANE = 16;
    public const long RANDOM_ALLY = 17;
    public const long SELF_BLANK_NEXT = 18;
    public const long OPPOSITE_FOUNTAIN = 19;
    public const long RANDOM_FOUNTAIN = 20;
    public const long MY_HAND = 21;
    public const long LANE_UNIT = 22;
    public const long POS_MY_SELF_DEAD = 23;
    public const long OPPOSITE_TARGET = 24;
    public const long ME_AND_AQUA_ALL = 25;
    public const long LANE_ALLIES_AQUA = 26;
    public const long ALL_ALLY_HERO_AND_TOWER = 27;
    public const long SELF_BLANK_FRONT = 28;
    public const long SELF_BLANK_BACK = 29;
    public const long ALL_ALLIES_AQUA = 30; 
    public const long LANE_ALLIES_BUT_SELF = 31;
    public const long ALL_ALLIES_BUT_SELF = 32;
    public const long RANDOM_BLANK_ALLY = 33;
    public const long NEARBY_ALLY_BEAST = 34;
    public const long LANE_ALLIES_BEAST = 35;
    public const long ALL_ALLIES_BEAST = 36;
    public const long RANDOM_LANE_ALLY = 37;
    public const long LANE_TARGET = 38;
    public const long LANE_UNIT_BUT_SELF = 39;
    public const long ALL_UNIT_BUT_SELF = 40;
    public const long OPPOSITE_MORTAL_ENEMY = 41;
    public const long ALL_ALLIES_GIONG = 42;
    public const long ALL_GOD_ALLIES = 43;
    public const long FOUNTAIN = 44;
    public const long RANDOM_LANE_BLANK_ALLY = 45;
    public const long LANE_TARGET_BUT_SELF = 46;
    public const long LANE_FRONT_ALLY = 47;
    public const long ALL_ENEMY_GOD = 48;
    public const long ALL_ALLY_MORTAL = 49;
    public const long MAX_MANA_HAND = 50;
    public const long MIN_MANA_HAND = 51;
    public const long CURRENT_HERO_SUMMON = 52;

    //Dạng Target Selected mà Player gửi lên
    public const int MY_HAND_CARD = 1000;
    public const int ANY_UNIT = 1001;
    public const int ANY_LANE_ALLIES = 1002;
    public const int ONE_ALLY_ONE_ENEMY_UNIT = 1003;
    public const int TWO_ANY_ALLIES = 1004;
    public const int TWO_ANY_ENEMIES = 1005;
    public const int ANY_LANE_UNITS = 1006;
    public const int ANY_ALLY_MORTAL_UNIT = 1007;
    public const int ANY_ALLY_GOD_UNIT = 1008;
    public const int ANY_ALLY_BUT_SELF = 1009;
    public const int ANY_ALLY_UNIT = 1010;
    public const int ANY_ALLY_LANE_UNITS = 1011;
    public const int ANY_ENEMY_MORTAL_UNIT = 1012;
    public const int ANY_ENEMY_GOD_UNIT = 1013;
    public const int ANY_ENEMY_UNIT = 1014;
    public const int ANY_ENEMY_LANE_UNITS = 1015;
    public const int ANY_TARGET = 1016;
    public const int ANY_ALLY_TOWER = 1017;
    public const int ANY_ENEMY_TOWER = 1018;
    public const int RANDOM_ENEMY_IN_SELECTED_LANE = 1019;
    public const int CHOSE_SELF_BLANK_NEXT = 1020;
    public const int CHOSSE_FOUNTAIN = 1021;
    public const int CHOSSE_LANE = 1022;
    public const int ANY_ALL_UNIT = 1023;
    public const int ANY_MOTAL = 1025;
    public const int ANY_BLANK_ALLY = 1026;
    public const int ANY_LANE_ENEMY = 1027;
    public const int ANY_COL_ENEMY = 1028;
    public const int ANY_ALLY_TARGET = 1029;
    public const int RANDOM_UNIT_IN_SELECTED_LANE = 1030;
    public const int TWO_ANY_ALLIES_JUNGLE_LAW = 1032;
    public const int ANY_TARGET_BUT_SELF = 1033;
    public const int ANY_MORTAL_BUT_SELF = 1034;
    public const int ANY_ALLY_TARGET_BUT_SELF = 1035;
    public const int TWO_ANY_ALLIES_BUT_SELF = 1036;
    public const int ONE_CARD_IN_HAND_AND_CHOOSE_LANE_UNITS = 1037;
    public const int ANY_UNIT_BUT_SELF = 1039;


    public const long EFFECT_BUFF_HP = 2;
    public const long EFFECT_TMP_INCREASE_ATK_AND_HP = 0;
    public const long EFFECT_INCREASE_ATK_AND_HP = 1;
    public const long EFFECT_MANA_CREATE_SHARD = 3;
    public const long EFFECT_MOVE_HERO = 4;
    public const long EFFECT_DRAW_CARD = 5;
    public const long EFFECT_DEAL_DAME = 6;
    public const long EFFECT_INCREASE_SPECIAL_PARAM = 7;
    public const long EFFECT_TMP_INCREASE_SPECIAL_PARAM = 8;
    public const long EFFECT_READY = 9;
    public const long EFFECT_FIGHT = 10;
    public const long EFFECT_SUMMON_VIRTUAL_HERO = 11;
    public const long EFFECT_SUMMON_CARD_IN_HAND = 12;
    public const long EFFECT_USER_MANA_MAX = 13;
    public const long EFFECT_LEAVE_CARD_IN_HAND = 14;
    public const long EFFECT_TMP_INCREASE_MANA_MAX = 16;
    public const long EFFECT_INCREASE_HERO_MANA = 18;
    public const long EFFECT_DEALDAMGE_TO_HEAL = 19;
    public const long EFFECT_PLAY_ALL_COLOR_CARD = 20;
    public const long EFFECT_TMP_INCREASE_HERO_MANA = 21;
    public const long EFFECT_PENATY = 25;
    public const long EFFECT_RETURN_HAND = 26;
    public const long EFFECT_X_TO_Y_TARGET = 28;
    public const long EFFECT_SWAP_MANA_ATK = 29;
    public const long EFFECT_DUPLICATE_CARD_IN_HAND = 30;
    public const long EFFECT_REPLACE_CARD_ON_BOARD = 32;
    public const long EFFECT_DEAL_DAME_ON_HAND = 33;

    public int id, hero_id, timing, eventSkill, max_turn, sark_god, /*min_shard, max_shard,*/ skill_type, target, effect_type, enable;
    public string name_skill, note;
    public bool isUltiType = false;
    public bool isBaseSkill = false;
    public List<EffectSkill> lstEffectSkill = new List<EffectSkill>();
    public List<ListEffectsSkill> lstEffectsSkills = new List<ListEffectsSkill>();
    public List<ConditionSkill> lstConditionSkill = new List<ConditionSkill>();
}

public class EffectSkill
{
    //effect của 1 mệnh đề trong skill
    public int type, target, heroId;
    public string desc;
    public bool isFragile;
}
public class ListEffectsSkill
{
    //list effect của các mệnh đề có quan hệ và 
    //info=0 skill 1 menh de ->list ef =1 ,info =1 skill nhieu menh de co quan he AND
    public int info;
    public List<EffectSkill> lstEffect = new List<EffectSkill>();
}
public class SkillEffect
{
    public long typeEffect;
    public long defCount; // so luong quan bi anh huong;
    public List<BoardCard> lstCardImpact = new List<BoardCard>();
    public List<TowerController> lstTowerImpact = new List<TowerController>();
}

public class ConditionSkill
{

    public const long NON_CONDITION = 1000;
    public const long FULL_LANE = 0;
    public const long SELF_READY = 1;
    public const int SELF_IN_FRONT_ROW = 2;
    public const int SELF_IN_BACK_ROW = 3;
    public const long SELF_ATK_ENEMY = 4;
    public const long SELF_DEFEAT_ENEMY = 5;
    public const long SELF_MOVE_LANE = 6;
    public const long SELF_GET_SHARK = 7;
    public const long SELF_DEAD = 8;
    public const long SELF_GET_DAME = 9;
    public const long SELF_BEFOR_ATK = 10;

    public const long BLANK_FRONT = 11;
    public const long BLANK_NEXT = 12;
    public const long BLANK_IN_LANE = 13;
    public const long CALL_SPELL = 14;

    public const long NUMBER_CARDS_IN_HAND = 15;
    public const long ENEMY_DEAD = 16;
    public const long ALLY_AQUATIC_DEAD = 17;
    public const long ALLY_DEAD_FRONT = 18;

    public const int CONDITION_TYPE_SMALLER = 30;
    public const int CONDITION_TYPE_BIGGER = 31;

    public int type;
    public string desc;
    public int pos, species, number;

}
public class DBRank
{
    public long id;
    public string name;
    public long elo;
    public long eloReset;
    public long eloParamReset;
}
public class DBLevel
{
    //level
    public long id;
    // điểm đầu exp của level hiện tại 
    public long exp;
    // khoang cach diem exp giua level hien tai den level tiep 
    public long expToUpLevel = 0;
    // exp hiện tại tính từ level trước
    public long expCurrent = 0;
}
public class DBKeyword
{
    //level
    public string id;
    public string descriptionEN;
    public string descriptionKR;
    public string descriptionJP;
    public string descriptionCN;
    public string descriptionTW;
}

public class VoiceData
{
    //  khi trieu hoi than lan dau tien
    // random in 2
    public const int TYPE_SUMMON = 100;
    // khi hoi sinh than (trieu hoi than lan 2 tro di) , tra ve trong update hero matrix
    //random in 2
    public const int TYPE_REVIVAL = 200;
    // khi than tan cong dich (ke ca combat va prepare)
    // random in 4
    public const int TYPE_ATTACK = 300;
    // khi kich hoat ulti thanh cong
    // random in 2
    public const int TYPE_ULTIMATE_ACTIVATED =500;
    // khi doi thu con 10s truoc khi ket thuc luot nhung doi thu chua co hanh dong gi
    public const int TYPE_DEFY =600;
    public const int TYPE_DIE =700;

}
public class DBBaseHp
{
    public int hp = 16; // chi so be nhat
    public int shard = 0; // chi so be nhat
    public long shardToUpLevel = 0;// khoang cach diem exp giua level hien tai den level tiep 
    public long shardCurrent = 0;// exp hiện tại tính từ level trước
}