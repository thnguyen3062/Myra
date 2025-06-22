using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProjectileDataInfo
{
    public string skillName;
    public long skillID;
    public long effectID;
    public ProjectileType type;
    public bool requireProjectile;
}

[Serializable]
public class Projectile
{
    public ProjectileType type;
    public Transform muzzle;
    public Transform projectile;
    public Transform impact;
}