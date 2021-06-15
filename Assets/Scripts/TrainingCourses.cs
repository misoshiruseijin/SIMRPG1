using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

public static class TrainingCourses
{
    #region ORIGINAL
    //private static int n_choices = 4; // 選択肢の数

    //public static string[] courseNameData = new string[] { "戦闘訓練・初級", "精神強化・初級", "座学・初級" };
    //public static string[] courseDescData = new string[] { "模擬戦闘を行い生物の攻撃能力および俊敏性を高める", "生物の精神力を鍛え体力、強靭さを強化する", "講義を行い生物の知性を高める" };

    ///// <summary>
    ///// 各コースのステータス上昇
    ///// statusChangeData = { course1{hp, atk, def...}, course2{hp, atk, def...}, course3... }
    ///// </summary>
    //public static int[][] statusChangeData = new int[][]
    //{
    //    new int[]{ 0, 3, 0, 2},
    //    new int[]{ 5, 0, 2, 0 },
    //    new int[] { 0, 0, 0, 0 }
    //};


    //// 選択肢の前に表示する文章
    //public static string[] eventText = new string[] { "は訓練に苦戦しているようだ", "は順調に訓練をこなしている", "は訓練に集中できていないようだ" };
    //// 選択肢の内容
    //public static string[] choiceText = new string[] { "慰める", "具体的な助言をする", "褒める", "放っておく", "叱る", "まだまだだと言う" };
    //// 選択肢に対応した性格ステータスの変化
    //public static int[][] statusChange_choice = new int[][]
    //{
    //    new int[] {1, -1, 1, 0}, // 選択肢１のステータス変化
    //    new int[] {0, 1, -1, 1},
    //    new int[] {1, -1, 1, 1},
    //    new int[] {0, 1, -1, 0},
    //    new int[] {-1, 1, 1, -1},
    //    new int[] {-1, 1, -1, -1}
    //};
    //// 各イベントで発生する可能性のある選択肢
    //public static int[][] possibleChoices = new int[][]
    //{
    //    new int[] {0, 1, 3, 4}, // イベント1で発生する選択肢
    //    new int[] {1, 2, 3, 5},
    //    new int[] {1, 3, 4, 5},
    //};



    //public static (string[] courseNames, string[] courseDescs, int[][] statusChange) GetCourseInfo()
    //{
    //    return (courseNameData, courseDescData, statusChangeData);
    //}

    //public static (string eventMessage, string[] choicesMsgs, int[][] statusChange) GetRandomEvent()
    //{
    //    string eventMessage;
    //    List<string> choiceMsgs = new List<string>();
    //    int[][] status = new int[n_choices][];

    //    // ランダムにイベントを1つ選ぶ
    //    int eventID = Random.Range(0, eventText.Length);
    //    eventMessage = eventText[eventID];

    //    // イベントに対応した選択肢を取得
    //    List<int> choiceIDList = possibleChoices[eventID].ToList();

    //    // 選択肢のメッセージとステータス変化をリストに入れる
    //    for (int i = 0; i < choiceIDList.Count; i++)
    //    {
    //        int choiceID = choiceIDList[i];

    //        choiceMsgs.Add(choiceText[choiceID]);
    //        status[i] = statusChange_choice[choiceID];
    //    }

    //    return (eventMessage, choiceMsgs.ToArray(), status);
    //}
    #endregion

    private static string courseCSVPath = "/TextData/TrainingCourses.csv";
    private static string eventCSVPath = "/TextData/TrainingEvents.csv";
    private static string choicesCSVPath = "/TextData/TrainingChoices.csv";
    private static int n_events; // イベントの種類数

    public static (string eventMsg, string[] choiceTitles, string[] choiceMsgs, int[][] status) GetRandomEvent()
    {
        n_events = File.ReadLines(Application.dataPath + eventCSVPath).Count() - 1; // subtract header line

        // ランダムなイベントを取得
        int eventID = Random.Range(0, n_events);
        Debug.Log($"EventID: {eventID}");
        string eventString = File.ReadLines(Application.dataPath + eventCSVPath).Skip(eventID+1).First();
        string[] eventSplitData = eventString.Split(',');
        string eventMessage = eventSplitData[1];
        
        // 選ばれたイベントの4つの選択肢を取得
        string[] choiceMessages = new string[4];
        string[] choiceTitles = new string[4];
        int[][] statusChange = new int[4][];
        for (int i = 0; i < 4; i++)
        {
            int choiceID = int.Parse(eventSplitData[i + 2]);
            string choiceString = File.ReadLines(Application.dataPath + choicesCSVPath).Skip(choiceID+1).First();
            string[] choiceSplitData = choiceString.Split(',');
            choiceTitles[i] = choiceSplitData[1];
            choiceMessages[i] = choiceSplitData[2];
            string[] tempArray = new string[] { choiceSplitData[3], choiceSplitData[4], choiceSplitData[5], choiceSplitData[6] };
            statusChange[i] = System.Array.ConvertAll(tempArray, int.Parse);
        }

        return (eventMessage, choiceTitles, choiceMessages, statusChange);
    }

    public static (string[] courseNames, string[] courseDescs, int[][] statusChange) GetCourseInfo()
    {
        List<string> courseNames = new List<string>();
        List<string> courseDescs = new List<string>();
        List<List<int>> statusList = new List<List<int>>();

        StreamReader reader = new StreamReader(Application.dataPath + courseCSVPath);
        reader.ReadLine(); // skip header
        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            string[] splitLine = line.Split(',');
            courseNames.Add(splitLine[1]);
            courseDescs.Add(splitLine[2]);

            List<string> tempList = new List<string>();
            for (int i = 3; i < splitLine.Length; i++)
            {
                tempList.Add(splitLine[i]);
            }
            statusList.Add(tempList.Select(int.Parse).ToList());
        }

        int[][] statusChange = statusList.Select(x => x.ToArray()).ToArray();
        return (courseNames.ToArray(), courseDescs.ToArray(), statusChange);
    }
}
