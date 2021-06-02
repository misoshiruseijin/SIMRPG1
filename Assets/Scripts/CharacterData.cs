using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData
{
    public int Maxhp, atk, def, spd;
    public string jpName;
    public List<SkillStatus> skillList;
    public Sprite unitSprite;

    public List<string> GetStatusList()
    {
        List<int> intList = new List<int> { Maxhp, atk, def, spd };
        List<string> statusList = new List<string>();

        foreach (int status in intList)
        {
            statusList.Add(status.ToString());
        }

        return statusList;
    }
}
