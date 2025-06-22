using System;
using System.Collections.Generic;
using UnityEngine;

public class SC_Tower_Invoker : MonoBehaviour
{
    public static event Action OnAnyAllyHPChanged;
    public static event Action OnAnyEnemyHPChanged;

    private float hpAllyTower;
    private float hpEnemyTower;

    private void Start()
    {
        // hpAllyTower = SC_HP_Influence.hp_ally_public;
        // hpEnemyTower = SC_HP_Influence.hp_enemy_public;

    }

    private void Update()
    {
    //     if( hpAllyTower!= SC_HP_Influence.hp_ally_public)
    //     {
    //         OnAnyAllyHPChanged?Invoke();
    //         hpAllyTower = SC_HP_Influence.hp_ally_public;
    //     }

    //     if( hpEnemyTower!= SC_HP_Influence.hp_enemy_public)
    //     {
    //         OnAnyEnemyHPChanged?Invoke();
    //         hpEnemyTower = SC_HP_Influence.hp_enemy_public;
    //     }
    }
}