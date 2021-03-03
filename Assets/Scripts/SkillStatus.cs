using UnityEngine;
using System.Collections.Generic;

public class SkillStatus : ScriptableObject
{
    //public string stringID;
    public int id;
    public string engName, jpName;
    public string desc;
    public string message;
    public List<int> targetList;
    public List<int> actionTypeList;
    public List<int> multList;
    public List<GameObject> effectList;
    //public string dmgTarget, healTarget, buffTarget;
    //public float dmgMult, healMult, buffMult;
    //public GameObject effect1, effect2;
}
