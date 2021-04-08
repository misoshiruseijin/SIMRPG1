using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayText : MonoBehaviour
{
    // 会話、バトルログなど、スクロールするタイプのテキストオブジェクトにアタッチする
    Text textObj;
    void Start()
    {
        textObj = this.GetComponent<Text>();
        textObj.text = "";
    }

    void Update()
    {
        if (TextController.updateFlg == true)
        {
            // if new string is available from TextController --> get new string and set it to Text object, deactivate flag
            string logText = TextController.UpdateLog2();
            textObj.text = logText;
        }
    }

}
