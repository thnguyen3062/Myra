using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellCollectionDeckGod : MonoBehaviour
{
    // Fields
    [SerializeField] private Image m_God, m_Frame;

    // Values

    // Methods
    public void SetData(DBHero heroInfo, int count)
    {
        m_God.sprite = CardData.Instance.GetOnBoardSprite(heroInfo.id);
        m_Frame.sprite = CardData.Instance.GetDeckFrameSprite(count, (int)heroInfo.color);
    }
}
