using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
    // テキストオブジェクトにアタッチする
    // テキストを更新。テキストstringはBattleControllerからもらう。
    [HideInInspector] public string textString = "";

    void Start()
    {
        this.GetComponent<Text>().text = textString;
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<Text>().text = textString;
    }

}
