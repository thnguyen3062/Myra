using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    private static ProjectileData instance;
    public static ProjectileData Instance
    {
        get
        {
            if (instance == null)
                return instance = Resources.Load<ProjectileData>("Data/ProjectileData");
            return instance;
        }
    }

    public List<ProjectileDataInfo> projectileInfo;
    public List<Projectile> projectile;
}