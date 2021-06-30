using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData
{
    #region 初期設定はCSVから
    public int Maxhp, atk, def, spd;
    public string jpName;
    public List<SkillStatus> skillList;
    public Sprite unitSprite;
    #endregion

    public int[] personaArray; // 性格を決める隠しパラメータ
    public int trainingCnt; // 訓練回数。性格付与条件の一つ

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
