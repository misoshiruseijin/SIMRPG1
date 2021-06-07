using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData
{
    public int Maxhp, atk, def, spd;
    public string jpName;
    public List<SkillStatus> skillList;
    public Sprite unitSprite;
    
    public int[] personaArray; // 性格を決める隠しパラメータ

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

    public int[] GetStatusIntArray()
    {
        int[] intArray = new int[] { Maxhp, atk, def, spd };
        return intArray;
    }

    public void UpdateStatus(int[] newStatusArray)
    {
        Maxhp = newStatusArray[0];
        atk = newStatusArray[1];
        def = newStatusArray[2];
        spd = newStatusArray[3];
    }
}
