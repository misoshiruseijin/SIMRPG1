using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
    // テキストオブジェクトにアタッチする
    // テキストを更新。テキストstringはBattleControllerなどの別クラスからもらう
    [HideInInspector] public string textString = "";

    private int n_lines = 3; // 表示できる最大の行数
    private List<string> logText = new List<string>();

    public void UpdateLog()
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

        this.GetComponent<Text>().text = string.Join("\n", logText.ToArray());
        
    }

}
