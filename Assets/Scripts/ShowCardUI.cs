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

public class ShowCardUI : MonoBehaviour
{
    [SerializeField] private Image showCardImg;
    public void InitData(long heroNumber)
    {
        Sprite godSprite = CardData.Instance.GetGodCardSprite(heroNumber);
        if (godSprite != null)
        {
            showCardImg.sprite = godSprite;
        }
        //initPosisiotn = transform.position;
    }
}
