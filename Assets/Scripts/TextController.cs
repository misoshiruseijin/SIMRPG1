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
    private string[] logText;

    void Start()
    {
        // initialize logText arary
        logText = new string[n_lines];
        for (int i = 0; i < n_lines; i++)
        {
            logText[i] = "";
        }

        this.GetComponent<Text>().text = string.Join("\n", logText);
    }

    public void UpdateLog()
    {
        for (int i = n_lines-1; i > 0; i--)
        {
            logText[i] = logText[i-1];
        }

        logText[0] = textString;

        this.GetComponent<Text>().text = string.Join("\n", logText);
    }

}
