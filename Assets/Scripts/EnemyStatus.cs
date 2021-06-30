using UnityEngine;
using System.Collections.Generic;

public class EnemyStatus : ScriptableObject
{
    public string id, engName, jpName;
    public int hp, atk, def, mat, mde, spd;
    public Sprite unitImg;
    public List<SkillStatus> skillList;
}
