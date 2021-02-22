using UnityEngine;
using System.Collections.Generic;

public class EnemyStatus : ScriptableObject
{
    public string id, engName, jpName;
    public int hp, atk, def, spd;
    public List<int> skills;
}
