using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UIEngine.UIPool;
using UIEngine.Extensions;
using GIKCore;
using GIKCore.UI;
using GIKCore.Utilities;

public class PopupHeroCard : MonoBehaviour
{
    // Fields
    [SerializeField] private HorizontalPoolGroup m_PoolBattleDeck, m_PoolUserCard;

    // Values
    private List<HeroCard> adapterUserCard = new List<HeroCard>();
    private List<HeroCard> adapterBattleDeck = new List<HeroCard>();

    private const int MAX_LENGTH = 40;
    private const int MAX_GOD = 6;
    private const int MAX_GOD_CLONE = 3;
    private const int MAX_TROOPER_CLONE = 2;

    // Methods
    public void DoSetBattleDeck()
    {       
        void AddToDict(Dictionary<long, long> target, long id)
        {
            if (target.ContainsKey(id))
                target[id] += 1;
            else target.Add(id, 1);
        }
        void GetFromDict(Dictionary<long, long> target, List<long> output)
        {
            foreach(KeyValuePair<long, long> pair in target)
            {
                output.Add(pair.Key);
                output.Add(pair.Value);
            }
        }

        Dictionary<long, long> dictGod = new Dictionary<long, long>();
        Dictionary<long, long> dictTrooper = new Dictionary<long, long>();
        List<long> lstGod = new List<long>();
        List<long> lstTrooper = new List<long>();

        int max = 4, count = 0;

        foreach (HeroCard hc in adapterBattleDeck)
        {
            DBHero db = hc.GetDatabase();
            if (db != null)
            {
                if (db.type == DBHero.TYPE_GOD)
                {
                    if (count < max)
                    {
                        AddToDict(dictGod, hc.id);
                        count++;
                    }
                }
                else AddToDict(dictTrooper, hc.id);
            }
        }

        GetFromDict(dictGod, lstGod);
        GetFromDict(dictTrooper, lstTrooper);

        if (lstGod.Count > 0 && lstTrooper.Count > 0)
        {
            Game.main.socket.SetUserBattleDeck(lstGod, lstTrooper);
        }
        else Toast.Show("Deck successfully loaded.");
    }
    public void DoSetBattleDeckAuto(int type)
    {
        List<long> lstTrooper = new List<long>();
        List<long> lstGod = new List<long>();

        // Set Green Deck
        if (type == 0)
        {
            foreach (HeroCard hc in GameData.main.lstHeroCard)
            {
                DBHero db = hc.GetDatabase();
                if (db != null)
                {
                    if (db.color == DBHero.COLOR_GREEN)
                    {
                        if (db.type == DBHero.TYPE_GOD)
                        {
                            lstGod.Add(hc.id);
                            lstGod.Add(1);
                        }
                        else
                        {
                            lstTrooper.Add(hc.id);
                            lstTrooper.Add(1);
                        }
                    }
                }
            }
        }
        // Set Red Deck
        else if(type == 1)
        {
            foreach (HeroCard hc in GameData.main.lstHeroCard)
            {
                DBHero db = hc.GetDatabase();
                if (db != null)
                {
                    if (db.color == DBHero.COLOR_RED)
                    {
                        if (db.type == DBHero.TYPE_GOD)
                        {
                            lstGod.Add(hc.id);
                            lstGod.Add(1);
                        }
                        else
                        {
                            lstTrooper.Add(hc.id);
                            lstTrooper.Add(1);
                        }
                    }
                }
            }
        }

        if (lstGod.Count > 0 && lstTrooper.Count > 0)
        {
            Game.main.socket.SetUserBattleDeck(lstGod, lstTrooper);
        }
        else Toast.Show("Deck successfully loaded.");
        //lang
    }

    private void DoSetToBattleDeck(HeroCard hc)
    {
        if (hc == null) return;
        if (adapterBattleDeck.Count >= MAX_LENGTH)
        {
            Toast.Show(string.Format("Tối đa {0} lá bài tham chiến", MAX_LENGTH));
            return;
        }

        bool Check(List<HeroCard> target, long heroNumber, long maxClone, string label)
        {
            List<HeroCard> lstSame = HeroCard.GetHeroCardByHeroNumber(target, heroNumber);
            if (lstSame.Count >= maxClone)
            {
                Toast.Show(string.Format("Không đủ nguyên liệu. Bạn chỉ được nhân bản tối đa {0} lá bài {1}", maxClone, label), 3);
                return false;
            }

            foreach (HeroCard god in lstSame)
            {
                if (hc.id != god.id)
                {
                    Toast.Show("Chỉ được nhân bản lá bài cùng loại");
                    return false;
                }
            }
            return true;
        }

        DBHero db = hc.GetDatabase();
        if (db != null)
        {
            List<HeroCard> lstSameType = HeroCard.GetHeroCardByType(adapterBattleDeck, db.type);            
            int maxClone = HeroCard.CountHeroCardSameHeroNumber(GameData.main.lstHeroCard, db.heroNumber);

            if (db.type == DBHero.TYPE_GOD)
            {
                if (lstSameType.Count >= MAX_GOD)
                {
                    Toast.Show(string.Format("Tối đa {0} lá bài {1}", MAX_GOD, /*db.name*/CardData.Instance.GetCardDataInfo(db.id).name));
                    return;
                }
                
                if (Check(lstSameType, db.heroNumber, Mathf.Min(maxClone, MAX_GOD_CLONE), db.name))
                    adapterBattleDeck.Add(hc);
            }
            else
            {
                if (Check(lstSameType, db.heroNumber, Mathf.Min(maxClone, MAX_TROOPER_CLONE), db.name))
                    adapterBattleDeck.Add(hc);
            }
            m_PoolBattleDeck.SetAdapter(adapterBattleDeck, false);
            m_PoolBattleDeck.ScrollToLast(0.1f);
        }
    }
    private void DoRemoveFromBattleDeck(HeroCard hc)
    {        
        for (int i = adapterBattleDeck.Count - 1; i >= 0; i--)
        {
            if (hc.id == adapterBattleDeck[i].id)
            {                
                adapterBattleDeck.RemoveAt(i);
                break;
            }
        }
        
        m_PoolBattleDeck.SetAdapter(adapterBattleDeck, false);
    }

    void Awake()
    {
        m_PoolBattleDeck.SetCellDataCallback((GameObject go, HeroCard data, int index) =>
        {
            CellHeroCard script = go.GetComponent<CellHeroCard>();
            //script.SetData(data, index);
           // script.SetOnClickCallback(hc => { DoRemoveFromBattleDeck(hc); });
        });
        m_PoolUserCard.SetCellDataCallback((GameObject go, HeroCard data, int index) =>
        {
            CellHeroCard script = go.GetComponent<CellHeroCard>();
           // script.SetData(data, index);
           // script.SetOnClickCallback(hc => { DoSetToBattleDeck(hc); });
        });
    }

    // Start is called before the first frame update
    //void Start() { }

    // Update is called once per frame
    //void Update() { }

    public static void Show()
    {
        string assetName = "PopupHeroCard";
        IUtil.LoadPrefabWithParent("Prefabs/Home/" + assetName, Game.main.canvas.panelPopup);
    }

    void OnEnable()
    {
        adapterUserCard.AddRange(HeroCard.GetHeroCardByType(GameData.main.lstHeroCard, DBHero.TYPE_GOD));
        adapterUserCard.AddRange(HeroCard.GetHeroCardByType(GameData.main.lstHeroCard, DBHero.TYPE_TROOPER_MAGIC));
        adapterUserCard.AddRange(HeroCard.GetHeroCardByType(GameData.main.lstHeroCard, DBHero.TYPE_TROOPER_NORMAL));
        m_PoolUserCard.SetAdapter(adapterUserCard);

        adapterBattleDeck = GameData.main.battleDeck.GetAllClone();
        m_PoolBattleDeck.SetAdapter(adapterBattleDeck);
    }
}
