﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData: GenericData
{
    public int Maxhp, atk, def, spd;
    public string jpName;
    public List<SkillStatus> skillList;
    public Sprite unitSprite;
}
