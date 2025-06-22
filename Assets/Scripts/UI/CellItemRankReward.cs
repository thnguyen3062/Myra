using GIKCore;
using GIKCore.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellItemRankReward : MonoBehaviour
{
    //[SerializeField] private Image m_ItemImg;
    
    [SerializeField] private TextMeshProUGUI m_Quantity;
    [SerializeField] private List<GameObject> m_Items;
    [SerializeField] private GameObject checkBox, tickRecieved;
    public void InitData(bool isAchieved, bool isReceive,long gold =0 , CardRewardRank card = null, ItemRewardRank item = null)
    {
        if(isAchieved && isReceive)
        {
            if(checkBox != null&& tickRecieved!= null)
            {
                checkBox.SetActive(true);
                tickRecieved.SetActive(true);
            }   
        }  
        else if(!isAchieved)
        {
            if(checkBox!= null)
                checkBox.SetActive(true);
        }    
        foreach(GameObject o in m_Items)
            o.SetActive(false);
        if(gold != 0)
        {
            m_Items[0].SetActive(true);
            m_Quantity.text = gold.ToString();

        }    
        else if( card != null)
        {
            m_Items[1].SetActive(true);
            m_Items[1].GetComponent<Image>().sprite = CardData.Instance.GetOnBoardSprite(card.heroID);
            if(!string.IsNullOrEmpty(card.cardImg))
            {
                LoadHttpItemImg(card.cardImg, m_Items[1].GetComponent<Image>());
            }    
            m_Quantity.text = card.count.ToString();
        }    
        else
        {
            m_Items[2].SetActive(true);
            if (!string.IsNullOrEmpty(item.itemImg))
            {
                LoadHttpItemImg(item.itemImg, m_Items[2].GetComponent<Image>());
            }
            m_Quantity.text = item.count.ToString();
        }    
    }
    IEnumerator coroutine;
    public void LoadHttpItemImg(string url,Image target)
    {
        if (coroutine != null)
            Game.main.StopCoroutine(coroutine);

        if (string.IsNullOrEmpty(url)) return;
        coroutine = IUtil.LoadTexture2DFromUrl(url, (Texture2D tex) =>
        {
            if (tex == null) return;
            Sprite sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            target.sprite = sprite;
        });
        Game.main.StartCoroutine(coroutine);
    }
    public void OnRecieveSuccess()
    {
        checkBox.SetActive(true);
        tickRecieved.SetActive(true);
    }    
}
