using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class PanelController
{
    public static void DisablePanel(GameObject panel)
    {
        // パネルを表示し、ボタンをアクティブにする
        DisableButtons(panel);
        panel.SetActive(false);        
    }

    public static void EnablePanel(GameObject panel)
    {
        // パネルを非表示にし、ボタンを非アクティブにする
        EnableButtons(panel);
        panel.SetActive(true);
    }

    public static void DisableButtons(GameObject panel)
    {
        // パネルのすべてのボタンを非アクティブにする
        Button[] btns = panel.gameObject.GetComponents<Button>();
        foreach (Button btn in btns)
        {
            btn.interactable = false;
        }
    }

    public static void EnableButtons(GameObject panel)
    {
        // パネルのすべてのボタンをアクティブにする
        Button[] btns = panel.gameObject.GetComponents<Button>();
        foreach (Button btn in btns)
        {
            btn.interactable = true;
        }
    }

    
}
