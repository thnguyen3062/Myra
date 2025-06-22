using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardDataInfo
{
    public long id;
    public string name;
    public List<string> skillName= new List<string>();
    public List<string> description = new List<string>();
}
