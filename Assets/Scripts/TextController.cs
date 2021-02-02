using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class TextController
{
    // テキストオブジェクトにアタッチする
    // テキストを更新。テキストstringはBattleControllerなどの別クラスからもらう

    private static int n_lines = 3; // 表示できる最大の行数
    private static List<string> logText = new List<string>();
    
    public static bool updateFlg = false;
    public static string outputString;

    public static void UpdateLog(string textString)
    {
        if (logText.Count < n_lines)
        {
            logText.Add(textString);
        }

        else
        {
            for (int i = 0; i < n_lines-1; i++)
            {
                logText[i] = logText[i + 1];
            }

            logText[n_lines - 1] = textString;
        }

        outputString = string.Join("\n", logText.ToArray());
        updateFlg = true;
        
    }

    public static string UpdateLog2()
    {
        updateFlg = false;
        return outputString;
    }

}
