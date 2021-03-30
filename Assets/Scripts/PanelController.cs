using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class PanelController
{
    public static void DiablePanel(GameObject panel)
    {
        // パネルを表示し、ボタンをアクティブにする
        Button[] btns = panel.gameObject.GetComponents<Button>();
        foreach (Button btn in btns)
        {
            btn.enabled = false;
        }
        panel.SetActive(false);        
    }

    public static void EnablePanel(GameObject panel)
    {
        // パネルを非表示にし、ボタンを非アクティブにする
        Button[] btns = panel.gameObject.GetComponents<Button>();
        foreach (Button btn in btns)
        {
            btn.enabled = false;
        }
        panel.SetActive(false);
    }
}
