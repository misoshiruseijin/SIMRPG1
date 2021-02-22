using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : ScriptableObject
{
    public string id, engName, jpName;
    public int hp, atk, def, spd;
    public List<int> skills;
}