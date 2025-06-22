//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;

//public class MinionHandCardHandler : HandCardHandler
//{
//    [SerializeField] private TextMeshPro healthOnHandText;
//    [SerializeField] private TextMeshPro damageOnHandText;
//    [SerializeField] private MeshRenderer minionMeshRenderer;

//    public override void SetCardData(long battleID, long id, CardOwner owner, CardSlot slot = null)
//    {
//        base.SetCardData(battleID, id, owner, slot);
//        healthOnHandText.text = heroInfo.hp.ToString();
//        damageOnHandText.text = heroInfo.atk.ToString();
//    }
//}
