using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData
{
    #region 初期設定はCSVから
    public int Maxhp, atk, def, mat, mde, spd;
    public string jpName;
    public List<SkillStatus> skillList;
    public Sprite unitSprite;
    #endregion

    public int personaID; // 性格ID
    public string personaName; // 性格名
    public int[] personaArray; // 性格を決める隠しパラメータ
    public int trainingCnt; // 訓練回数。性格付与条件の一つ
    public float[] statusMults; // 性格によるステータス補正倍率
    public int personaStep; // 何段階目の性格を持っているか

    public List<string> GetStatusList()
    {
        List<int> intList = new List<int> { Maxhp, atk, def, mat, mde, spd };
        List<string> statusList = new List<string>();

        foreach (int status in intList)
        {
            statusList.Add(status.ToString());
        }

        return statusList;
    }

    public int[] GetStatusIntArray()
    {
        int[] intArray = new int[] { Maxhp, atk, def, mat, mde, spd };
        return intArray;
    }

    public void UpdateStatus(int[] newStatusArray)
    {
        Maxhp = newStatusArray[0];
        atk = newStatusArray[1];
        def = newStatusArray[2];
        mat = newStatusArray[3];
        mde = newStatusArray[4];
        spd = newStatusArray[5];
    }
}
