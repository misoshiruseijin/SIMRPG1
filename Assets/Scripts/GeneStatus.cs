﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneStatus : ScriptableObject
{
    public string id, engName, jpName;
    public int hp, atk, def, spd;
    public SkillStatus skill;
    public int risk;
}
