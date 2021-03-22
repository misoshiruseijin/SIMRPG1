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

    public void ButtonPressed()
    {
        eventSystem = EventSystem.current;
        button = eventSystem.currentSelectedGameObject;
        Debug.Log(button);
    }
}
