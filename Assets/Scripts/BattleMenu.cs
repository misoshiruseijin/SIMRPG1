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

    public GameObject modePanel, actionPanel, skillPanel, targetPanel; // 戦闘メニューパネル
    public GameObject battleLogPanel; // バトルログパネル
    public GameObject backBtnObj;
    public BackButton backBtn;

    [HideInInspector] public BattleController battleController;
    [HideInInspector] public Text skillDescText;

    public bool guardFlg;

    public List<int> skillTargetTypes;
    public List<string> skillDescStrings;

    public int skillNumber; // プレイヤーが選んだスキルが何番目のスキルか
    public int targetNumber; // プレイヤーが選んだターゲットが何番目のターゲットか

    private EventSystem eventSystem;
    private GameObject button;
    private GameObject prevButton; // 1回前に押されていたボタン
    private ButtonResponse btnResp;
    private int btnID, prevBtnID;
    private List<GameObject> menuPanelList;
    private List<int> panelStates;
    private int n_modes, n_actions, n_skills, n_targets, n_btns;
    private Text logText;

    private void Start()
    {
        battleController = GetComponent<BattleController>();
        logText = battleLogPanel.GetComponentInChildren<Text>();
        backBtn = backBtnObj.GetComponent<BackButton>();

        // initialize btnList
        btnList = btnListMode.Concat(btnListAction).Concat(btnListSkill).Concat(btnListTarget).ToList();

        menuPanelList = new List<GameObject>() { modePanel, actionPanel, skillPanel, targetPanel };
        panelStates = new List<int>() { 0, 0, 0, 0 };

        n_modes = btnListMode.Count;
        n_actions = btnListAction.Count;
        n_skills = btnListSkill.Count;
        n_targets = btnListTarget.Count;
        n_btns = btnList.Count;

        guardFlg = false;
    }
    
    public void SetMenuNewTurn()
    {
        PanelController.DisablePanel(modePanel);
        PanelController.DisablePanel(actionPanel);
        PanelController.DisablePanel(skillPanel);
        PanelController.DisablePanel(targetPanel);
        PanelController.DisablePanel(battleLogPanel);

        PanelController.EnablePanel(modePanel);

        skillDescText.text = "";
    }

    public void SetMenuNextUnit()
    {
        PanelController.DisablePanel(modePanel);
        PanelController.DisablePanel(skillPanel);
        PanelController.DisablePanel(targetPanel);
        PanelController.DisablePanel(battleLogPanel);

        PanelController.EnablePanel(actionPanel);

        skillDescText.text = "";
    }

    public void SetMenuStartAction()
    {
        PanelController.DisablePanel(modePanel);
        PanelController.DisablePanel(actionPanel);
        PanelController.DisablePanel(skillPanel);
        PanelController.DisablePanel(targetPanel);
        
        PanelController.EnablePanel(battleLogPanel);
    }

    public void ClearLogPanel()
    {
        //Debug.Log("Clearing log panel");
        TextController.ClearLog();
    }

    public void ResetAllButtonStates()
    {
        foreach (GameObject btn in btnList)
        {
            btn.GetComponent<ButtonResponse>().ResetButton();
            //Debug.Log(btn.GetComponent<ButtonResponse>());
        }
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
            //btnFlgList[prevBtnID] = false;
            prevButton.GetComponent<ButtonResponse>().ResetButton();    
        }

        if (!btnResp.btnReady)
        {
            //Debug.Log("初めてクリックされた");
            if (GetButtonType(btnID) == 2)
            {
                // スキルボタンならスキル解説文を表示
                skillDescText.text = skillDescStrings[btnID - n_modes - n_actions];
            }

            btnResp.btnReady = true;
        }

        else if (btnResp.btnReady && !btnResp.btnActive)
        {
            //Debug.Log("二回目のクリック");

            //btnResp.btnReady = false;
            //btnResp.btnActive = true;
            btnResp.SetButton();
            TakeButtonAction(btnID);
        }
    }

    public void SetMenuObjectText(Text textObj, string text)
    {
        textObj.GetComponent<Text>().text = text;
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
                StartCoroutine(battleController.StartManualTurn());
                break;

            case 1:
                //Debug.Log("オートモード");
                foreach (GameObject panel in menuPanelList)
                {
                    PanelController.DisablePanel(panel);
                }
                PanelController.EnablePanel(battleLogPanel);
                battleController.StartAutoTurn();
                break;

            case 2:
                //Debug.Log("スキル");
                PanelController.DisableButtons(actionPanel);
                PanelController.EnablePanel(skillPanel);
                guardFlg = false;
                break;

            case 3:
                //Debug.Log("防御");
                guardFlg = true;
                SetMenuNextUnit();
                battleController.ActionSelected();
                // 次のキャラへ
                break;

            case 4:
                //Debug.Log("スキル1");
                // 単体攻撃ならターゲットパネルへ
                if (skillTargetTypes[0] == 0)
                {
                    PanelController.DisableButtons(skillPanel);
                    PanelController.EnablePanel(targetPanel);
                }
                // 全体攻撃なら次のキャラへ
                else
                {
                    targetNumber = 20;
                    battleController.ActionSelected();
                }
                skillNumber = 0;
                break;

            case 5:
                //Debug.Log("スキル2");
                // 単体攻撃ならターゲットパネルへ
                if (skillTargetTypes[1] == 0)
                {
                    PanelController.DisableButtons(skillPanel);
                    PanelController.EnablePanel(targetPanel);
                }
                // 全体攻撃なら次のキャラへ
                else
                {
                    targetNumber = 20;
                    battleController.ActionSelected();
                }
                skillNumber = 1;
                break;

            case 6:
                //Debug.Log("スキル3");
                // 単体攻撃ならターゲットパネルへ
                if (skillTargetTypes[2] == 0)
                {
                    PanelController.DisableButtons(skillPanel);
                    PanelController.EnablePanel(targetPanel);
                }
                // 全体攻撃なら次のキャラへ
                else
                {
                    targetNumber = 20;
                    battleController.ActionSelected();
                }
                skillNumber = 2;
                break;

            case 7:
                //Debug.Log("スキル4");
                // 単体攻撃ならターゲットパネルへ
                if (skillTargetTypes[3] == 0)
                {
                    PanelController.DisableButtons(skillPanel);
                    PanelController.EnablePanel(targetPanel);
                }
                // 全体攻撃なら次のキャラへ
                else
                {
                    targetNumber = 20;
                    battleController.ActionSelected();
                }
                skillNumber = 3;
                break;

            case 8:
                //Debug.Log("ターゲット1");
                battleController.ActionSelected();
                targetNumber = 0;                
                // 次のキャラへ
                break;

            case 9:
                //Debug.Log("ターゲット2");
                battleController.ActionSelected();
                targetNumber = 1;
                // 次のキャラへ
                break;

            case 10:
                //Debug.Log("ターゲット3");
                battleController.ActionSelected();
                targetNumber = 2;
                // 次のキャラへ
                break;
        }

        // BackButtonにメニューパネルのアクティブステートを渡す
        for (int i = 0; i < menuPanelList.Count; i++)
        {
            if (menuPanelList[i].activeInHierarchy)
            {
                panelStates[i] = 1;
            }

            else
            {
                panelStates[i] = 0;
            }
        }

        backBtn.panelState = panelStates;

    }

}
