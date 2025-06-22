using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GIKCore.Net;

public class PackSlot : MonoBehaviour,IDropHandler
{
   
    public void OnDrop( PointerEventData eventData)
    {
       
        if (eventData.pointerDrag != null)
        {
            UserPackController.main.isDrop = true;
        }
    }

}
