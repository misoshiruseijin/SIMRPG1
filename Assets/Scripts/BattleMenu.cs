using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleMenu : MonoBehaviour
{
    // 押されたボタンを見つけて、ボタン状況に対応したアクションを取る
    public List<GameObject> btnListMode;
    public List<GameObject> btnListAction;
    public List<GameObject> btnListSkill;
    public List<GameObject> btnListTarget;
    public List<GameObject> btnList; // 全ボタンのリスト
    public List<bool> btnFlgList; // 全ボタンの状況

    public GameObject modePanel, actionPanel, skillPanel, targetPanel; // 戦闘メニューパネル
    public GameObject battleLogPanel; // バトルログパネル

    public bool autoBattleFlg, manualBattleFlg;

    private EventSystem eventSystem;
    private GameObject button;
    private GameObject prevButton; // 1回前に押されていたボタン
    private ButtonResponse btnResp;
    private int btnID, prevBtnID;
    private List<GameObject> menuPanelList;
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

        menuPanelList = new List<GameObject>() { modePanel, actionPanel, skillPanel, targetPanel };

        n_modes = btnListMode.Count;
        n_actions = btnListAction.Count;
        n_skills = btnListSkill.Count;
        n_targets = btnListTarget.Count;
        n_btns = btnList.Count;

        autoBattleFlg = false;
    }
    public void ButtonPressed()
    {
        eventSystem = EventSystem.current;
        prevButton = button;
        button = eventSystem.currentSelectedGameObject;
        btnResp = button.GetComponent<ButtonResponse>();
        
        prevBtnID = btnID;
        btnID = btnList.IndexOf(button);

        bool btnChanged = btnID != prevBtnID; // 前と違うボタンを押した
        bool noPrevBtn = prevButton == null; // 前に押したボタンがない (今回初めてボタンを押した)

        //Debug.Log(button);

        // 同じパネル内で前と違うボタンを押した
        if (!noPrevBtn && !CheckButtonTypeChange(btnID, prevBtnID) && btnChanged)
        {
            Debug.Log(noPrevBtn + ", " + btnChanged);
            btnFlgList[prevBtnID] = false;
            prevButton.GetComponent<ButtonResponse>().ResetButton();    
        }

        if (!btnResp.btnReady)
        {
            //Debug.Log("初めてクリックされた");
            btnResp.btnReady = true;
        }

        else if (btnResp.btnReady && !btnResp.btnActive)
        {
            //Debug.Log("二回目のクリック");
            btnResp.btnReady = false;
            btnResp.btnActive = true;
            btnFlgList[btnID] = true;
            TakeButtonAction(btnID);
        }
    }


    private bool CheckButtonTypeChange(int btnID, int prevBtnID)
    {
        bool typeChanged = true;

        if (GetButtonType(btnID) == GetButtonType(prevBtnID))
        {
            typeChanged = false;
        }

        return typeChanged;
    }

    private int GetButtonType(int btnID)
    {
        int btnType = 10;

        switch (btnID)
        {
            case int i when (i < n_modes):
                //Debug.Log("モードが選択された");
                btnType = 0;
                break;

            case int i when (i >= n_modes && i < n_modes + n_actions):
                //Debug.Log("アクションが選択された");
                btnType = 1;
                break;

            case int i when (i >= n_modes + n_actions && i < n_modes + n_actions + n_skills):
                //Debug.Log("スキルが選択された");
                btnType = 2;
                break;

            case int i when (i >= n_modes + n_actions + n_skills && i < n_btns):
                //Debug.Log("ターゲットが選択された");
                btnType = 3;
                break;
            
            default:
                Debug.Log("ボタンIDに異常");
                break;
        }

        return btnType;
    }

    private void TakeButtonAction(int btnID)
    {
        switch (btnID)
        {
            case 0:
                Debug.Log("マニュアルモード");
                PanelController.DisablePanel(modePanel);
                PanelController.EnablePanel(actionPanel);
                manualBattleFlg = true;
                break;

            case 1:
                Debug.Log("オートモード");
                foreach (GameObject panel in menuPanelList)
                {
                    PanelController.DisablePanel(panel);
                }
                PanelController.EnablePanel(battleLogPanel);
                autoBattleFlg = true;
                break;

            case 2:
                Debug.Log("スキル");
                PanelController.DisableButtons(actionPanel);
                PanelController.EnablePanel(skillPanel);
                break;

            case 3:
                Debug.Log("防御");
                foreach (GameObject panel in menuPanelList)
                {
                    PanelController.DisablePanel(panel);
                }
                // 次のキャラへ
                break;

            case 4:
                Debug.Log("スキル1");
                // 単体攻撃ならターゲットパネルへ
                // 全体攻撃なら次のキャラへ
                break;

            case 5:
                Debug.Log("スキル2");
                // 単体攻撃ならターゲットパネルへ
                // 全体攻撃なら次のキャラへ
                break;

            case 6:
                Debug.Log("スキル3");
                // 単体攻撃ならターゲットパネルへ
                // 全体攻撃なら次のキャラへ
                break;

            case 7:
                Debug.Log("スキル4");
                // 単体攻撃ならターゲットパネルへ
                // 全体攻撃なら次のキャラへ
                break;

            case 8:
                Debug.Log("ターゲット1");
                // 次のキャラへ
                break;

            case 9:
                Debug.Log("ターゲット2");
                // 次のキャラへ
                break;

            case 10:
                Debug.Log("ターゲット3");
                // 次のキャラへ
                break;
        }

    }


}
