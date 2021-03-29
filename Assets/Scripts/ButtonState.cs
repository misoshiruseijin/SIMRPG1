using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonState : MonoBehaviour
{
    // 押されたボタンを見つけて、そのボタンについているスクリプトのメソッドを実行する
    public List<GameObject> btnListMode;
    public List<GameObject> btnListAction;
    public List<GameObject> btnListSkill;
    public List<GameObject> btnListTarget;
    public List<GameObject> btnList; // 全ボタンのリスト
    public List<bool> btnFlgList; // 全ボタンの状況

    private EventSystem eventSystem;
    private GameObject button;
    private GameObject prevButton; // 1回前に押されていたボタン
    private ButtonResponse btnResp;
    private int btnID, prevBtnID;

    private void Start()
    {
        btnList = btnListMode.Concat(btnListAction).Concat(btnListSkill).Concat(btnListTarget).ToList();
        btnFlgList = new List<bool>();
        for (int i=0; i<btnList.Count; i++)
        {
            btnFlgList.Add(false);
        }
    }
    public void ButtonPressed()
    {
        eventSystem = EventSystem.current;
        prevButton = button;
        button = eventSystem.currentSelectedGameObject;
        btnResp = button.GetComponent<ButtonResponse>();
        
        bool btnChanged = !GameObject.ReferenceEquals(button, prevButton); // 前と違うボタンを押した
        bool prevNull = GameObject.ReferenceEquals(prevButton, null); // 前に押したボタンがnull (今回初めてボタンを押した)

        prevBtnID = btnID;
        btnID = btnList.IndexOf(button);
        Debug.Log(button);

        // 違うボタンを押したら前に押したボタンの点滅を解除
        if(btnChanged && !prevNull)
        {
            btnFlgList[prevBtnID] = false;
            prevButton.GetComponent<ButtonResponse>().ResetButton();
        }

        if (!btnResp.btnReady)
        {
            btnResp.btnReady = true;
            //Debug.Log("初めてクリックされた");
        }

        else if (btnResp.btnReady && !btnResp.btnActive)
        {
            btnResp.btnReady = false;
            btnResp.btnActive = true;
            btnFlgList[btnID] = true;
            //Debug.Log("二回目のクリック");
        }
    }


}
