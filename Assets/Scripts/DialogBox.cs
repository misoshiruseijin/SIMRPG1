using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent (typeof(CanvasGroup))]
public class DialogBox : MonoBehaviour
{
    public Text messageText;
    public Text acceptText, declineText, okText;
    public Button acceptBtn, declineBtn, okBtn;

    private CanvasGroup cg;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
    }
    public DialogBox SetOnAccept(string text, UnityAction action)
    {
        // 「はい」ボタンのテキストとコールバックを設定
        acceptText.text = text;
        acceptBtn.onClick.RemoveAllListeners();
        acceptBtn.onClick.AddListener(action);
        return this;
    }

    public DialogBox SetOnDecline(string text, UnityAction action)
    {
        // 「いいえ」ボタンのテキストとコールバックを設定
        declineText.text = text;
        declineBtn.onClick.RemoveAllListeners();
        declineBtn.onClick.AddListener(action);
        return this;
    }

    public DialogBox SetOnOK(string text, UnityAction action)
    {
        // 「OK」ボタンのテキストとコールバックを設定
        okText.text = text;
        okBtn.onClick.RemoveAllListeners();
        okBtn.onClick.AddListener(action);
        return this;
    }

    public DialogBox SetMessage(string messageString)
    {
        // 本文を設定
        messageText.text = messageString;
        return this;
    }

    public DialogBox SingleButtonMode(bool isOn)
    {
        // ボタン1つか2つか設定
        if (isOn)
        {
            acceptBtn.gameObject.SetActive(false);
            declineBtn.gameObject.SetActive(false);
            okBtn.gameObject.SetActive(true);
        }

        else
        {
            acceptBtn.gameObject.SetActive(true);
            declineBtn.gameObject.SetActive(true);
            okBtn.gameObject.SetActive(false);
        }

        return this;
    }

    public void Show()
    {
        // ダイアログボックスを表示
        this.transform.SetAsLastSibling();
        cg.interactable = true;
        cg.alpha = 1f;
        cg.blocksRaycasts = true;
    }

    public void Hide()
    {
        // ダイアログボックスを非表示
        this.transform.SetAsFirstSibling();
        cg.interactable = false;
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
    }

    private static DialogBox instance;
    public static DialogBox Instance()
    {
        if (!instance)
        {
            instance = FindObjectOfType(typeof(DialogBox)) as DialogBox;
            if (!instance)
            {
                Debug.Log("シーン上にアクティブなDialogBoxが存在しません");
            }
        }

        return instance;
    }
}
