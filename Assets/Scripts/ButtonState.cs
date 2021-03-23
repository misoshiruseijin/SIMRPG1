using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonState : MonoBehaviour
{
    // 押されたボタンを見つけて、そのボタンについているスクリプトのメソッドを実行する
    private EventSystem eventSystem;
    private GameObject button;
    private ButtonResponse btnResp;

    public void ButtonPressed()
    {
        eventSystem = EventSystem.current;
        button = eventSystem.currentSelectedGameObject;
        btnResp = button.GetComponent<ButtonResponse>();
        Debug.Log(button);

        if (!btnResp.btnReady)
        {
            // 初めてクリックされた
            btnResp.btnReady = true;
            Debug.Log("初めてクリックされた");
        }

        else if (btnResp.btnReady && !btnResp.btnActive)
        {
            btnResp.btnReady = false;
            btnResp.btnActive = true;
            Debug.Log("二回目のクリック");
        }
    }


}
