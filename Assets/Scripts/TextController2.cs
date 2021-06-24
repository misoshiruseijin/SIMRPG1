using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class TextController2 : MonoBehaviour
{
    ///<summary>
    ///Hiearchy:
    ///このスクリプトがアタッチされているパネル
    ///-> Button: テキストが表示される部分
    ///-> Text
    ///</summary> 
    
    public string message; // 1会話内の全文章

    private Text messageText;
    private CanvasGroup cg;
    
    private string[] segments; // 一度に表示する文章のArray
    private string splitPattern; // セグメントを区切るのに使うキー
    private float elapsedTime; // 1文字表示してから経過した時間
    private float textSpeed; // 次の文字を表示するまでの時間（小さいほうが早い）
    private int segmentIndex; // 何個目のセグメントを表示中か
    private int charIndex; // 一度に表示する文章の何文字目まで表示されているか
    private bool isSegmentDone; // 1セグメントを表示し終わったか
    private bool isMsgDone; // messageを全て表示し終わったか
    
    public bool isControllerActive;
    public bool hidePanel; // メッセージを表示しきったらパネルを消すか？

    private bool isMsgBoxPressed;
    
    void Start()
    {
        messageText = this.GetComponentInChildren<Text>();
        splitPattern = "<>";
        textSpeed = GameController.instance.messaegSpeed;
        cg = this.GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (isControllerActive)
        {
            if (isMsgDone || message == null)
            {
                //Debug.Log("メッセージの表示が終わっているか、メッセージがない場合");
                return;
            }

            if (!isSegmentDone)
            {
                //Debug.Log("1セグメントの表示が終わっていない");
                if (elapsedTime >= textSpeed)
                {
                    messageText.text += segments[segmentIndex][charIndex];

                    charIndex++;
                    elapsedTime = 0f;

                    if (charIndex >= segments[segmentIndex].Length)
                    {
                        //Debug.Log("1セグメント表示し終わった");
                        isSegmentDone = true;
                    }

                    elapsedTime += Time.deltaTime;
                }

                elapsedTime += Time.deltaTime;

                if (isMsgBoxPressed)
                {
                    messageText.text = segments[segmentIndex];
                    isSegmentDone = true;
                    isMsgBoxPressed = false;
                }
            }

            else
            {
                // セグメントの表示が終わっている
                if (isMsgBoxPressed)
                {
                    // クリックで次のセグメントの表示を開始
                    //Debug.Log("クリックを確認");
                    charIndex = 0;
                    segmentIndex++;
                    elapsedTime = 0f;
                    isSegmentDone = false;
                    isMsgBoxPressed = false;

                    if (segmentIndex >= segments.Length)
                    {
                        Debug.Log("メッセージを表示しきった");
                        isMsgDone = true;
                        
                        if (hidePanel)
                        {
                            PanelController.DisablePanel(this.gameObject);
                        }

                        isControllerActive = false;
                    }

                    else
                    {
                        // 全文表示が終わっていたら最後のセグメントを残す
                        messageText.text = "";
                    }

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

    public void MessageBoxPressed()
    {
        // ボタンのOnClick
        //Debug.Log("Message window pressed");
        isMsgBoxPressed = true;
    }

    public void StartText(string fullMessage, bool hideWhenDone = true)
    {
        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
        message = fullMessage;
        SetMessage(message);
        hidePanel = hideWhenDone;
        isControllerActive = true;
    }
}
