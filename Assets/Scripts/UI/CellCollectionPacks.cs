using GIKCore.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellCollectionPacks : MonoBehaviour
{
    [SerializeField] private Image m_FramePacks, m_YangPacks, mhidetop, mhideunder;
    [SerializeField] private TextMeshProUGUI m_PackName;
    [SerializeField] int m_price;
    //[SerializeField] private GoOnOff m_CellDeck;

    public int idPacks;
    public delegate void Callback(CellCollectionPacks ccd);

    private Callback onClickCB = null;

    public CellCollectionPacks Setdatainfor(Packsinfor infor)
    {
        if (infor != null)
        {
            m_PackName.text = infor.txt;
        //    m_FramePacks = infor.img.GetComponent<>;
        }
        return this;
    }
}
