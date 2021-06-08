using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ボタン数を自由に設定できる
/// ストーリー上の選択や訓練での選択など、フラグ管理や隠しパラメーター管理を行う選択を扱う
/// </summary>

[RequireComponent (typeof(CanvasGroup))]
public class DialogBox_Choices : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject buttonParent;
    public Text eventMsgText;

    private CanvasGroup cg;
    private List<Choice> choiceList;

    private void Awake()
    {
        cg = this.GetComponent<CanvasGroup>();
        
    }

    public class Choice
    {
        public string btnText;
        public Action btnAction;
    }


    public void NewButtons(string[] textArray, Action[] actionArray)
    {
        choiceList = new List<Choice>();

        for (int i = 0; i < textArray.Length; i++)
        {
            Choice choice = new Choice();
            choice.btnText = textArray[i];
            choice.btnAction = actionArray[i];
            choiceList.Add(choice);
        }        
    }

    public void SetButtons()
    {
        foreach (Transform child in buttonParent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Choice choice in choiceList)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, buttonParent.transform);
            buttonObj.GetComponentInChildren<Text>().text = choice.btnText;
            Button button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(() => choice.btnAction());
            button.onClick.AddListener(() => Hide());            
        }
    }

    public void SetMessage(string msgString)
    {
        eventMsgText.text = msgString;
    }


    public void Show()
    {
        cg.interactable = true;
        cg.alpha = 1f;
        cg.blocksRaycasts = true;
    }

    public void Hide()
    {
        cg.interactable = false;
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
    }


    private static DialogBox_Choices instance;
    public static DialogBox_Choices Instance()
    {
        if (!instance)
        {
            instance = FindObjectOfType(typeof(DialogBox_Choices)) as DialogBox_Choices;
            if (!instance)
            {
                Debug.Log("シーン上にアクティブなDialogBox_Choicesが存在しません");
            }
        }

        return instance;
    }
}
