using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class PanelController
{
    //public static void DisablePanel(GameObject panel)
    //{
    //    // パネルを表示し、ボタンをアクティブにする
    //    DisableButtons(panel);
    //    panel.SetActive(false);        
    //}
    public static void DisablePanel(GameObject panel)
    {
        // パネルを非表示にし、インタラクト不能にする
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }
    //public static void EnablePanel(GameObject panel)
    //{
    //    // パネルを非表示にし、ボタンを非アクティブにする
    //    EnableButtons(panel);
    //    panel.SetActive(true);
    //}
    public static void EnablePanel(GameObject panel)
    {
        // パネルを表示し、インタラクト可能にする
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    public static void DisableButtons(GameObject panel)
    {
        // パネルのすべてのボタンを非アクティブにする
        Button[] btns = panel.gameObject.GetComponentsInChildren<Button>();
        foreach (Button btn in btns)
        {
            btn.enabled = false;
        }
    }

    public static void EnableButtons(GameObject panel)
    {
        // パネルのすべてのボタンをアクティブにする
        Button[] btns = panel.gameObject.GetComponentsInChildren<Button>();
        //Debug.Log("Number of buttons: " + btns.Length);
        foreach (Button btn in btns)
        {
            if (btn.GetComponentInChildren<Text>() != null && string.IsNullOrEmpty(btn.GetComponentInChildren<Text>().text))
            {
                btn.enabled = false;
            }
            else
            {
                btn.enabled = true;
            }
        }
    }

    
}
