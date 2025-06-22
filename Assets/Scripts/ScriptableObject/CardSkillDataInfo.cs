using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CardSkillDataInfo
{
    public string godName;
    public long heroID;
    public List<long> skillIds;
}
[Serializable]
public class SkillSound
{
    public string nameSound;
    public string soundID;
    public List<long> skillIds;
}