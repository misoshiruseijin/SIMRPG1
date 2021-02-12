using UnityEngine;

public class SkillStatus : ScriptableObject
{
    public string id, engName, jpName;
    public string desc, message1, message2, message3;
    public string dmgTarget, healTarget, buffTarget;
    public float dmgMult, healMult, buffMult;
    public GameObject effect1, effect2;
}
