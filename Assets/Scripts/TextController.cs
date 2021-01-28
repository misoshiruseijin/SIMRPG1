using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
    // テキストオブジェクトにアタッチする
    // テキストを更新。テキストstringはBattleControllerからもらう。
    [HideInInspector] public string textString = "";

    private int n_lines = 3; // 表示できる最大の行数
    //private string[] logText;
    private List<string> logText = new List<string>();

    void Start()
    {
        // initialize logText arary
        //List<string> logText = new List<string>();
        //for (int i = 0; i < n_lines; i++)
        //{
        //    logText[i] = "";
        //}

        //this.GetComponent<Text>().text = string.Join("\n", logText);
    }

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
        //for (int i = 0; i < n_lines-1; i++)
        //{
        //    logText[i] = logText[i + 1];
        //}

        //logText[n_lines] = textString;

        //this.GetComponent<Text>().text = string.Join("\n", logText);
    }

}
