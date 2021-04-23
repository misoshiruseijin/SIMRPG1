using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class SimulationMenu : MonoBehaviour
{
    #region Variable Definitions
    public List<CharacterData> allyDataList; // 味方キャラのステータスデータ

    // インスペクターで設定
    public List<GameObject> menuBtnList;
    public List<GameObject> popupPanelList;
    public GameObject togglePrefab;
    public GameObject allyMenuToggleParent;
    public GameObject allyStatusPanel; 

    // スクリプトで生成
    public List<GameObject> allyMenuToggleList; // 味方管理画面のトグル
    public GameObject unitName, unitImage, unitParam, unitSkill, unitDesc; // 味方ステータス画面のコンポーネント

    private EventSystem eventSystem;
    public List<GameObject> btnList;
    private GameObject button, prevButton;
    private ButtonResponseSize btnResp;
    private int btnID, prevBtnID;
    private int nPanels; // number of panels in popupPanelList
    private int nMenuBtns;
    private ToggleGroup allyToggleGroup;
    private int activeToggleID, prevToggleID;
    #endregion


    private void Awake()
    {
        // GameControllerからキャラデータをロードする
        allyDataList = ManageCharacterData.LoadCharacterData();


        // below is for testing purposes. use above line
        //allyDataList = new List<CharacterData>();
        //allyDataList.Add(ManageCharacterData.DataFromSO("nezumi", true));
        //allyDataList.Add(ManageCharacterData.DataFromSO("ka", true));
    }


    private void Start()
    {
        btnList = menuBtnList;
        nPanels = popupPanelList.Count;
        nMenuBtns = menuBtnList.Count;
        allyToggleGroup = allyMenuToggleParent.GetComponent<ToggleGroup>();
        activeToggleID = -1; // default value

        // 味方管理画面の設定
        // トグルオブジェクトを作成 (初期状態では全部オフ)
        for (int i = 0; i < allyDataList.Count; i++)
        {
            GameObject toggleObj = Instantiate(togglePrefab, allyMenuToggleParent.transform) as GameObject;
            toggleObj.GetComponent<Toggle>().group = allyToggleGroup;
            toggleObj.GetComponent<Toggle>().GetComponentInChildren<Text>().text = allyDataList[i].jpName;
            toggleObj.GetComponent<Toggle>().onValueChanged.AddListener((bool value) => AllyMenuToggleStateChange());

            allyMenuToggleList.Add(toggleObj);          
        }

        // ステータスパネルのコンポーネントを獲得
        unitName = allyStatusPanel.transform.Find("UnitNameText").gameObject;
        unitImage = allyStatusPanel.transform.Find("UnitImage").gameObject;
        unitParam = allyStatusPanel.transform.Find("UnitParamText").gameObject;
        unitSkill = allyStatusPanel.transform.Find("UnitSkillText").gameObject;
        unitDesc = allyStatusPanel.transform.Find("UnitDescText").gameObject;
    }

    public void ButtonPressed()
    {
        eventSystem = EventSystem.current;
        prevButton = button;
        button = eventSystem.currentSelectedGameObject;
        btnResp = button.GetComponent<ButtonResponseSize>();

        prevBtnID = btnID;
        btnID = btnList.IndexOf(button);

        bool btnChanged = btnID != prevBtnID; // 前と違うボタンを押した
        bool noPrevBtn = prevButton == null; // 前に押したボタンがない (今回初めてボタンを押した)

        //Debug.Log(button);

        // 同じパネル内で前と違うボタンを押した
        if (!noPrevBtn && btnChanged)
        {
            //Debug.Log(noPrevBtn + ", " + btnChanged);
            prevButton.GetComponent<ButtonResponseSize>().btnReady = false;
        }

        if (!btnResp.btnReady)
        {
            //Debug.Log("初めてクリックされた");
            btnResp.btnReady = true;
        }

        else if (btnResp.btnReady)
        {
            //Debug.Log("二回目のクリック");
            btnResp.btnReady = false;
            TakeButtonAction(btnID);
        }
    }

    private void TakeButtonAction(int _btnID)
    {
        switch (_btnID)
        {
            case int i when (i < nMenuBtns):
                //Debug.Log("メニューバーのボタンが押された。対応したパネルを表示する");
                for (int j = 0; j < nPanels; j++)
                {
                    if (i == btnID)
                    {
                        PanelController.EnablePanel(popupPanelList[i]);
                    }

                    else
                    {
                        PanelController.DisablePanel(popupPanelList[i]);
                    }
                }
                break;
        }
    }

    public void CloseButtonPressed()
    {
        // 閉じるボタンを押したら全てのパネルを閉じる
        foreach (GameObject panel in popupPanelList)
        {
            PanelController.DisablePanel(panel);
        }
    }

    public void BattleButtonPressed()
    {
        // VVV FOR TESTING VVV
        ManageCharacterData.SavePartyData(allyDataList); // 一時的に全味方とパーティーメンバーが同じことにしてる
        // ^^^

        ManageCharacterData.SaveCharacterData(allyDataList);
        SceneController.ToBattleScene();
    }

    public void AllyMenuToggleStateChange()
    {
        // アクティブなToggleに対応したキャラのステータスを表示する
        //Debug.Log("Ally Menu Toggle State Changed");
        GameObject activeToggle = allyToggleGroup.ActiveToggles().FirstOrDefault().gameObject;
        
        prevToggleID = activeToggleID;
        activeToggleID = allyMenuToggleList.IndexOf(activeToggle);

        bool toggleChanged = prevToggleID != activeToggleID;

        if (toggleChanged)
        {
            // ステータスパネルを設定
            //Debug.Log("Toggle changed from " + prevToggleID + " to " + activeToggleID);
            unitName.GetComponent<Text>().text = allyDataList[activeToggleID].jpName;
            unitImage.GetComponent<Image>().sprite = allyDataList[activeToggleID].unitSprite;
            unitParam.GetComponent<Text>().text = string.Join("\n",
                new string[]{"能力値", "体力: " + allyDataList[activeToggleID].Maxhp, "力: " + allyDataList[activeToggleID].atk,
                "強靭さ: " + allyDataList[activeToggleID].def, "俊敏性: " + allyDataList[activeToggleID].spd});
            List<string> skillNames = new List<string>() { "特殊技能" };
            foreach (SkillStatus skill in allyDataList[activeToggleID].skillList)
            {
                skillNames.Add(skill.jpName);
            }
            unitSkill.GetComponent<Text>().text = string.Join("\n", skillNames.ToArray());
            unitDesc.GetComponent<Text>().text = "キャラ説明文";
        }
    }
}
