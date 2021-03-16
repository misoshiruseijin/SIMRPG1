using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayText : MonoBehaviour
{
    // 会話、バトルログなど、スクロールするタイプのテキストオブジェクトにアタッチする
    void Start()
    {
        this.GetComponent<Text>().text = "";
    }

    void Update()
    {
        if (TextController.updateFlg == true)
        {
            // if new string is available from TextController --> get new string and set it to Text object, deactivate flag
            string logText = TextController.UpdateLog2();
            this.GetComponent<Text>().text = logText;
        }
    }
}
