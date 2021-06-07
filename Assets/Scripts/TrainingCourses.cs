using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TrainingCourses
{
    public static string[] courseNameData = new string[] { "戦闘訓練・初級", "精神強化・初級", "座学・初級" };
    public static string[] courseDescData = new string[] { "模擬戦闘を行い生物の攻撃能力および俊敏性を高める", "生物の精神力を鍛え体力、強靭さを強化する", "講義を行い生物の知性を高める" };

    /// <summary>
    /// 各コースのステータス上昇
    /// statusChangeData = { course1{hp, atk, def...}, course2{hp, atk, def...}, course3... }
    /// </summary>
    public static int[][] statusChangeData = new int[][]
    {
        new int[]{ 0, 3, 0, 2},
        new int[]{ 5, 0, 2, 0 },
        new int[] { 0, 0, 0, 0 }
    };


    // 選択肢の前に表示する文章
    public static string[] eventText = new string[] { "は訓練に苦戦しているようだ", "は順調に訓練をこなしている", "は訓練に集中できていないようだ" };
    // 選択肢の内容
    public static string[] choiceText = new string[] { "慰める", "具体的な助言をする", "褒める", "放っておく", "叱る", "まだまだだと言う" };
    // 選択肢に対応した性格ステータスの変化
    public static int[][] statusChange_choice = new int[][]
    {
        new int[] {1, -1, 1, 0}, // 選択肢１のステータス変化
        new int[] {0, 1, -1, 1},
        new int[] {1, -1, 1, 1},
        new int[] {0, 1, -1, 0},
        new int[] {-1, 1, 1, -1},
        new int[] {-1, 1, -1, -1}
    };
    // 各イベントで発生する可能性のある選択肢
    public static int[][] possibleChoices = new int[][]
    {
        new int[] {0, 1, 3, 4}, // イベント1で発生する選択肢
        new int[] {1, 2, 3, 5},
        new int[] {1, 3, 4, 5},
    };



    public static (string[] courseNames, string[] courseDescs, int[][] statusChange) GetCourseInfo()
    {
        return (courseNameData, courseDescData, statusChangeData);
    }

    public static (string eventMessage, string[] choicesMsgs, int[][] statusChange) GetChoicesAndEffects()
    {
        string eventMessage;
        List<string> choiceMsgs = new List<string>();
        int[][] status = new int[][] { };

        // ランダムにイベントを1つ選ぶ
        int eventID = Random.Range(0, eventText.Length);
        eventMessage = eventText[eventID];

        // イベントに対応した選択肢を取得
        List<int> choiceIDList = possibleChoices[eventID].ToList();

        // 選択肢のメッセージとステータス変化をリストに入れる
        for (int i = 0; i < choiceIDList.Count; i++)
        {
            int choiceID = choiceIDList[i];
            choiceMsgs.Add(choiceText[choiceID]);
            status[i] = statusChange_choice[choiceID];
        }

        return (eventMessage, choiceMsgs.ToArray(), status);
    }
}
