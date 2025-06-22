//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;

//public class HandCardHandler : Card
//{
//    public TextMeshPro manaOnHandText;
//    public TextMeshPro nameOnHandText;
//    public TextMeshPro descriptionOnHandText;

//    public override void SetCardData(long battleID, long id, CardOwner owner, CardSlot slot = null)
//    {
//        base.SetCardData(battleID, id, owner, slot);
//        CardDataInfo inf = CardData.Instance.GetCardDataInfo(heroInfo.id);
//        if (inf != null)
//        {
//            if (descriptionOnHandText != null)
//            {
//                string txt = "";
//                inf.description.ForEach(x =>
//                {
//                    if (x.Contains("overrun") || x.Contains("fragile") || x.Contains("combo") || x.Contains("breaker") || x.Contains("cleave"))
//                    {
//                        if (inf.keyword.Count > 1)
//                            x = "<size=6>" + x + "</size>";
//                        else
//                            x = "<size=3>" + x + "</size>";
//                    }
//                    txt += x;
//                });
//                txt = txt.Replace("\\n", "\n");
//                descriptionOnHandText.text = txt;
//            }
//        }

//        manaOnHandText.text = heroInfo.mana.ToString();
//        nameOnHandText.text = heroInfo.name;
//    }
//}
