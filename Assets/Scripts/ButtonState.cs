using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonState : MonoBehaviour
{
    // 押されたボタンを見つけて、ボタン状況に対応したアクションを取る
    public List<GameObject> btnListMode;
    public List<GameObject> btnListAction;
    public List<GameObject> btnListSkill;
    public List<GameObject> btnListTarget;
    public List<GameObject> btnList; // 全ボタンのリスト
    public List<bool> btnFlgList; // 全ボタンの状況

    public GameObject modePanel, actionPanel, skillPanel, targetPanel; // 戦闘メニューパネル

    private EventSystem eventSystem;
    private GameObject button;
    private GameObject prevButton; // 1回前に押されていたボタン
    private ButtonResponse btnResp;
    private int btnID, prevBtnID;
    private int n_modes, n_actions, n_skills, n_targets, n_btns;

    private void Start()
    {
        // initialize btnList and btnFlgList
        btnList = btnListMode.Concat(btnListAction).Concat(btnListSkill).Concat(btnListTarget).ToList();
        btnFlgList = new List<bool>();
        for (int i=0; i<btnList.Count; i++)
        {
            btnFlgList.Add(false);
        }

        n_modes = btnListMode.Count;
        n_actions = btnListAction.Count;
        n_skills = btnListSkill.Count;
        n_targets = btnListTarget.Count;
        n_btns = btnList.Count;
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
        //Debug.Log(button);

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
            CheckPressedButtonType(button);
            //Debug.Log("二回目のクリック");
        }
    }

    private void CheckPressedButtonType(GameObject button)
    {
        switch (btnID)
        {
            case int i when (i < n_modes):
                Debug.Log("戦闘モードが選択された");
                break;

            case int i when (i >= n_modes && i < n_modes + n_actions):
                Debug.Log("アクションが選択された");
                break;

            case int i when (i >= n_modes + n_actions && i < n_modes + n_actions + n_skills):
                Debug.Log("スキルが選択された");
                break;

            case int i when (i >= n_modes + n_actions + n_skills && i < n_btns):
                Debug.Log("ターゲットが選択された");
                break;
        }

    }


}
