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
            // if logText list contain less than n_lines lines, append new string to logText list
            logText.Add(textString);
        }

        else
        {
            // else, discard oldest line and add new line (scroll text)
            for (int i = 0; i < n_lines-1; i++)
            {
                logText[i] = logText[i + 1];
            }

            logText[n_lines - 1] = textString;
        }

        // join all strings in logText list into single string
        outputString = string.Join("\n", logText.ToArray());
        updateFlg = true; // set flag to indicate that new text is ready
        
    }

    public static string UpdateLog2()
    {
        updateFlg = false;
        return outputString;
    }

    public static void ClearLog()
    {
        logText = new List<string>();
    }

}
