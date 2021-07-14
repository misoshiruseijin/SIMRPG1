using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Globalization;

public static class PersonalityData
{
    // 性格付与関連

    ///<summary>
    /// 1.性格付与の条件を満たしているか検証
    /// 2.どの性格になるか
    /// 3.性格をCharacterDataに保存
    /// 4.性格ごとのステータス補正 
    ///</summary> 

    private static string personaCSVPath = "/TextData/Persona.csv";

    private static int[] n_training = new int[] { 1, 5 }; // 性格付与に必要な訓練回数

    // 初回性格付与時に使う条件
    private static int[] minStatusTypes = new int[] { 1, 0, 3, 2 };
    private static int[] maxStatusTypes = new int[] { 0, 1, 2, 3 };

    private static int[] statusArray; // personaArrayを一時保存

    public static bool ChangePersona(CharacterData character)
    {
        // 訓練回数の条件を満たしていない
        if(character.trainingCnt < n_training[character.personaStep])
        {
            Debug.Log("性格付与の条件を満たしていない");
            return false;
        }

        // 訓練回数の条件を満たしている
        else
        {
            statusArray = character.personaArray;
            
            // どの性格ステータスが最大、最小か確認
            int maxIndex = 0;
            int minIndex = 0;
            int maxValue = statusArray[0];
            int minValue = statusArray[0];
            int id = -1;
             
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

            // 性格IDを決める
            switch (character.personaStep)
            {
                // 1段階目
                case 0:
                    // 最大値と最小値、どちらが顕著かチェック            
                    if (Mathf.Abs(maxValue) >= Mathf.Abs(minValue))
                    {
                        // 最大値条件を適用
                        id = maxStatusTypes[maxIndex];
                    }

                    else
                    {
                        // 最小値条件を適用
                        id = minStatusTypes[minIndex];
                    }

                    break;
            }

            // CSVから決まった性格の情報を読み込む
            StreamReader reader = new StreamReader(Application.dataPath + personaCSVPath);
            for (int i = 0; i < id + 2; i++)
            {
                reader.ReadLine();
            }
            string line = reader.ReadLine(); // ほしい情報が書かれている行
            Debug.Log(line);
            string[] splitline = line.Split(',');

            string name = splitline[1];
            //int skillID = int.Parse(splitline[splitline.Length - 1]);

            // 倍率だけfloat[]に変換
            List<string> splitlineList = splitline.ToList();
            List<float> tempList = new List<float>();
            for (int i = 2; i < splitlineList.Count -1; i++ )
            {
                float f = float.Parse(splitlineList[i]);
                tempList.Add(f);
            }

            float[] multArray = tempList.ToArray();

            // CharacterDataを更新
            character.personaID = id;
            character.personaName = name;
            character.statusMults = multArray;
            character.personaStep++;
            character.trainingCnt = 0;

            // 性格倍率を適用
            int[] newStatus = new int[multArray.Length];
            int[] oldStatus = character.GetStatusIntArray();
            for (int i = 0; i < multArray.Length; i++)
            {
                newStatus[i] = oldStatus[i] + Mathf.RoundToInt(oldStatus[i] * multArray[i]);
            }
            character.UpdateStatus(newStatus);

            Debug.Log($"性格：{splitline[1]}が付与された");

            return true;
        }

    }

}
