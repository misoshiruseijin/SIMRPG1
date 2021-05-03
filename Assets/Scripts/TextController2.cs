using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class TextController2 : MonoBehaviour
{
    //public GameObject textObj;
    public string message; // 1会話内の全文章

    private Text messageText;
    
    private string[] segments; // 一度に表示する文章のArray
    private string splitPattern;
    private float elapsedTime; // 1文字表示してから経過した時間
    private float textSpeed; // 次の文字を表示するまでの時間（小さいほうが早い）
    private int segmentIndex; // 何個目のセグメントを表示中か
    private int charIndex; // 一度に表示する文章の何文字目まで表示されているか
    private bool isSegmentDone; // 1セグメントを表示し終わったか
    private bool isMsgDone; // messageを全て表示し終わったか
    
    void Start()
    {
        messageText = this.GetComponent<Text>();

        message = "1行目だあああああああああああああああああああああああああああああああああああ。\n"
            + "改行して2行目ええええええええええええ\n"
            + "3行目ええええええええええええええええええええええええええええええええええええええええええええええええええええええええええ\n<>"
            + "うわあああああああああああああああああああああああああああああああああああああ";
        splitPattern = "<>";
        textSpeed = 0.05f;

        SetMessage(message);
    }

    void Update()
    {
        if (isMsgDone || message == null)
        {
            // メッセージの表示が終わっているか、メッセージがない場合
            return;
        }

        if (!isSegmentDone)
        {
            // 1セグメントの表示が終わっていない
            if (elapsedTime >= textSpeed)
            {
                messageText.text += segments[segmentIndex][charIndex];

                charIndex++;
                elapsedTime = 0f;

                if (charIndex >= segments[segmentIndex].Length)
                {
                    // 1セグメント表示し終わった
                    isSegmentDone = true;
                }

                elapsedTime += Time.deltaTime;
            }

            elapsedTime += Time.deltaTime;
        }

        else
        {
            // 1セグメント表示しきった
            if (Input.GetMouseButtonDown(0))
            {
                // クリックで次のセグメントの表示を開始
                Debug.Log("クリックを確認");
                charIndex = 0;
                segmentIndex++;
                messageText.text = "";
                elapsedTime = 0f;
                isSegmentDone = false;

                if (segmentIndex >= segments.Length)
                {
                    // メッセージをすべて表示しきった
                    isMsgDone = true;
                    Debug.Log("メッセージを表示しきった");
                }
            }
        }
    }

    private void SetMessage(string message)
    {
        // 新しいメッセージを設定
        segments = Regex.Split(message, @"\s*" + splitPattern + @"\s*", RegexOptions.IgnorePatternWhitespace);
        charIndex = 0;
        segmentIndex = 0;
        messageText.text = "";
        isSegmentDone = false;
        isMsgDone = false;
    }
}
