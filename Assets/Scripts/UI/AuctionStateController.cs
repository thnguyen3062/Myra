using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuctionStateController : MonoBehaviour
{
    [SerializeField] private GameObject m_CoinRandomUI;
    [SerializeField] private GameObject m_StartBidUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnRollCoin()
    {
        m_CoinRandomUI.SetActive(true);
    }
    public void OnStartBid()
    {
        // parse thong tin 2 ng choi vao day ;

    }    

}
