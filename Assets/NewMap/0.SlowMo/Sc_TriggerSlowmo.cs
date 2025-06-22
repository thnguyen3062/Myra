using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_TriggerSlowmo : MonoBehaviour

{
    public Sc_Slowmo slow_mo;


    public void Shoot ()
    {
            slow_mo.DoSlowmotion();
    }
}
