using GIKCore.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardReward : MonoBehaviour
{
    
    [SerializeField] private GameObject god, minion, spell;
    [SerializeField] private TextMeshProUGUI countTxt;
    public void SetData(DBHero db,long count)
    {
        if (db != null)
        {
            if (db.disable > 0)
            {
                return;
            }
            if (db.type == DBHero.TYPE_GOD)
            {
                god.SetActive(true);
                god.GetComponent<CardUserInfor>().SetInfoCard(db, 0, 1);
                minion.SetActive(false);
                spell.SetActive(false);
            }
            if (db.type == DBHero.TYPE_TROOPER_MAGIC || db.type == DBHero.TYPE_BUFF_MAGIC)
            {
                god.SetActive(false);
                minion.SetActive(false);
                spell.SetActive(true);
                spell.GetComponent<CardUserInfor>().SetInfoCard(db, 0, 1);
            }
            if (db.type == DBHero.TYPE_TROOPER_NORMAL)
            {
                god.SetActive(false);
                minion.SetActive(true);
                spell.SetActive(false);
                minion.GetComponent<CardUserInfor>().SetInfoCard(db, 0, 1);
            }
            countTxt.text = count.ToString();
           
        }
    }
}
