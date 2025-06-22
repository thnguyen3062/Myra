using System.Collections;
using System.Collections.Generic;
using TMPro;
using UIEngine.Extensions;
using UnityEngine;
using GIKCore;
using UnityEngine.UI;
using System.Linq;
using GIKCore.UI;
using GIKCore.Utilities;


public class CellCollectionCard : MonoBehaviour
{
    // Fields

    [SerializeField] private GameObject god, minion, spell;
    // Values
    private ICallback.CallFunc2<CellHeroCardUser> onClickCB = null;
    private ICallback.CallFunc3<CellHeroCardUser, GameObject> onEndDragCB = null;
    private CellHeroCardUser data;
    public int countHC;
    public long CurrentCardID
    {
        get;
        private set;
    }
    public static CellCollectionCard main { get; private set; } = null;

    // Methods
    public void DoClick()
    {
        if (data != null && onClickCB != null)
            onClickCB(data);
    }

    public void SetOnClickCallback(ICallback.CallFunc2<CellHeroCardUser> func) { onClickCB = func; }
    public void SetData(CellHeroCardUser data)
    {
        this.data = data;
        DBHero db = data.lst[0].GetDatabase();
        if (db != null)
        {
            if (db.disable > 0)
            {
                return;
            }
            countHC = data.lst.Count - data.lstHeroCardIDNow.Count;
            if (db.type == DBHero.TYPE_GOD)
            {
                god.GetComponent<CardUserInfor>().SetInfoCard(db, countHC, (int)data.lst[0].frame, true);
                god.SetActive(true);
                minion.SetActive(false);
                spell.SetActive(false);
            }
            if (db.type == DBHero.TYPE_TROOPER_MAGIC || db.type == DBHero.TYPE_BUFF_MAGIC)
            {
                spell.GetComponent<CardUserInfor>().SetInfoCard(db, countHC, (int)data.lst[0].frame, true);
                god.SetActive(false);
                minion.SetActive(false);
                spell.SetActive(true);
            }
            if (db.type == DBHero.TYPE_TROOPER_NORMAL)
            {
                minion.GetComponent<CardUserInfor>().SetInfoCard(db, countHC, (int)data.lst[0].frame, true);
                god.SetActive(false);
                minion.SetActive(true);
                spell.SetActive(false);
            }
        }
    }
    private void Awake()
    {
        CellCollectionCard.main = this;
    }
}
