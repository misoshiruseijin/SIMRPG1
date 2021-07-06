using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PersonalityData
{
    // 性格付与関連

    ///<summary>
    /// 1.性格付与の条件を満たしているか検証
    /// 2.どの性格になるか
    /// 3.性格をCharacterDataに保存
    /// 4.性格ごとのステータス補正 
    ///</summary> 

    private static int[] n_training = new int[]{ 2, 5 }; // 性格付与に必要な訓練回数

    // 初回性格付与時に使う条件
    private static int[] minStatusTypes = new int[] { 1, 0, 3, 2 };
    private static int[] maxStatusTypes = new int[] { 0, 1, 2, 3 };

    private static int[] statusArray; // personaArrayを一時保存
    private static int step;
    
    public static void CheckPersona(int[] personaArray, int trainingStep, int trainingCnt)
    {
        // stepは現在何段階目の性格を持っているか（0は性格なし）

        // 訓練回数の条件を満たしているか
        if (trainingCnt >= n_training[trainingStep])
        {
            statusArray = personaArray;
            step = trainingStep;
            //GetPersona();
        }
    }

    private static void GetPersona()
    {
        // どの性格ステータスが最大、最小か確認
        int maxIndex = 0;
        int minIndex = 0;
        int maxValue = statusArray[0];
        int minValue = statusArray[0];
        int personaID; // 性格タイプID

        for (int i = 1; i < statusArray.Length; i++)
        {
            if (statusArray[i] > maxValue)
            {
                maxIndex = i;
                maxValue = statusArray[i];
            }

            if (statusArray[i] < minValue)
            {
                minIndex = i;
                minValue = statusArray[i];
            }
        }

        switch (step)
        {
            // 1段階目
            case 0:
                // 最大値と最小値、どちらが顕著かチェック            
                if (Mathf.Abs(maxValue) >= Mathf.Abs(minValue))
                {
                    // 最大値条件を適用
                    personaID = maxStatusTypes[maxIndex];
                }

                else
                {
                    // 最小値条件を適用
                    personaID = minStatusTypes[minIndex];
                }

                break;
        }
    }

}
