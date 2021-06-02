using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneData
{
    public string jpName;
    public int hp, atk, def, spd;
    public SkillStatus skill;
    public int risk;

    public List<string> GetStatusList()
    {
        List<int> intList = new List<int> { hp, atk, def, spd };
        List<string> statusList = new List<string>();
        foreach (int status in intList)
        {
            statusList.Add(status.ToString());
        }
        return statusList;
    }
}
