using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_CoinToss : MonoBehaviour
{
        public GameObject coin;
        public Rigidbody rb;
        public int forceY = 20;
        public int forceX = 0;
        public int forceZ = 0;
    // Start is called before the first frame update
    void Start()
    {
        // cointoss();
    }

    public void cointoss()
    {
        int jumpforce = forceY;
        rb.AddForce(0, jumpforce, 0);
        int torqx = forceX;
        int torqz = forceZ;
        rb.AddTorque(torqx, 0, torqz);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
