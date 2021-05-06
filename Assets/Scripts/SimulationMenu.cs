using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class SimulationMenu : MonoBehaviour
{
    #region Variable Definitions

    // インスペクターで設定
    public List<GameObject> menuBtnList;
    public List<GameObject> popupPanelList;

    public GameObject togglePrefab;
    public GameObject skillNameBtnPrefab;

    public GameObject allyMenuToggleParent, partyMenuToggleParent; // トグルの親オブジェクト。ToggleGroupを持つ
    public GameObject evolveMenuToggleParent;
    public GameObject skillNameParent; // スキル名テキストオブジェクトの親。VerticalLayout持ち
    public GameObject allyStatusPanel, partyStatusPanel, evolveStatusPanel;
    public List<GameObject> memberImageList; // パーティーメンバー画像オブジェクト
    public GameObject addUnitButton, redoPartyButton, finalizePartyButton; // パーティー編成画面のボタン
    public GameObject msgPanel1, msgPanel2; // ポップアップメッセージパネル

    // スクリプトで生成
    public List<GameObject> allyMenuToggleList, partyMenuToggleList; // 味方管理画面のトグルリスト、パーティー編成画面のトグルリスト
    public List<GameObject> evolveMenuToggleList; // 育成画面の遺伝子アイテムトグルリスト

    public List<CharacterData> allyDataList; // 味方キャラのステータスデータ
    public List<CharacterData> partyDataList; // バトルシーンに渡すパーティーメンバーのデータ
    public List<GameObject> skillBtnList; // 各キャラのスキル名オブジェクトリスト
    public List<GeneData> geneDataList; // 所持している遺伝子アイテムのリスト

    private EventSystem eventSystem;
    private List<GameObject> btnList;
    private GameObject button, prevButton;
    private ButtonResponseSize btnResp;
    private int btnID, prevBtnID;
    private int nPanels; // number of panels in popupPanelList
    private int nMainBtns;
    private ToggleGroup allyToggleGroup, partyToggleGroup, evolveToggleGroup;
    private int activeToggleID, prevToggleID;
    private int evolveUnitID; // 育成されるユニットID
    private List<int> partyMemberID;
    private GameObject skillDescObj;
    #endregion


    private void Awake()
    {
        // GameControllerからキャラデータをロードする
        allyDataList = ManageData.LoadCharacterData();


        // FOR TESTING PURPOSES. DELETE WHEN UNNEEDED //
        if (allyDataList.Count == 0)
        {
            allyDataList = new List<CharacterData>();
            allyDataList.Add(ManageData.CharacterDataFromSO("nezumi", true));
            allyDataList.Add(ManageData.CharacterDataFromSO("ka", true));
            allyDataList.Add(ManageData.CharacterDataFromSO("ka", true));
            allyDataList.Add(ManageData.CharacterDataFromSO("ka", true));
        }

        geneDataList = new List<GeneData>();
        geneDataList.Add(ManageData.GeneDataFromSO("koumori"));
        geneDataList.Add(ManageData.GeneDataFromSO("usagi"));
        // FOR TESTING PURPOSES. DELETE WHEN UNNEEDED //

    }

    private void Start()
    {
        btnList = menuBtnList;
        nPanels = popupPanelList.Count;
        nMainBtns = 5; // 大元のメニューボタン数
        partyDataList = new List<CharacterData>();
        partyMemberID = new List<int>();
        allyToggleGroup = allyMenuToggleParent.GetComponent<ToggleGroup>();
        partyToggleGroup = partyMenuToggleParent.GetComponent<ToggleGroup>();
        evolveToggleGroup = evolveMenuToggleParent.GetComponent<ToggleGroup>();
        skillDescObj = allyStatusPanel.transform.Find("SkillDescText").gameObject;
        
        activeToggleID = -1; // default value

        // 味方管理画面の設定
        // トグルオブジェクトを作成 (初期状態では全部オフ)
        allyMenuToggleList = ToggleListFromAllyData(allyMenuToggleParent, allyToggleGroup);
        foreach (GameObject toggleObj in allyMenuToggleList)
        {
            toggleObj.GetComponent<Toggle>().onValueChanged.AddListener((bool value) => AllyMenuToggleStateChange());
        }

        // パーティー編成画面の設定
        // トグルオブジェクトを作成 (初期状態では全部オフ)
        partyMenuToggleList = ToggleListFromAllyData(partyMenuToggleParent, partyToggleGroup);
        foreach (GameObject toggleObj in partyMenuToggleList)
        {
            toggleObj.GetComponent<Toggle>().onValueChanged.AddListener((bool value) => PartyMenuToggleStateChange());
        }

        // 育成画面の設定
        // トグルオブジェクトを作成 (初期状態では全部オフ)
        evolveMenuToggleList = ToggleListFromGeneData(evolveMenuToggleParent, evolveToggleGroup);
        foreach (GameObject toggleObj in evolveMenuToggleList)
        {
            toggleObj.GetComponent<Toggle>().onValueChanged.AddListener((bool value) => EvolveMenuToggleStateChange());
        }

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

    private void TakeButtonAction(int btnID)
    {
        activeToggleID = -1;
        prevButton = null;
        button = null;

        if (btnID < nMainBtns)
        {
            //Debug.Log("メインメニューのボタンが押された。対応したパネルを表示する");
            for (int i = 0; i < nPanels; i++)
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
        }

        else
        {
            //Debug.Log("メインメニュー以外のボタンが押された。メインパネルに重ねてパネルを表示する");
            PanelController.EnablePanel(popupPanelList[btnID]);
        }
        
    }

    public void AllyMenuToggleStateChange()
    {
        // アクティブなToggleに対応したキャラのステータスを表示する
        //Debug.Log("Ally Menu Toggle State Changed");
        GameObject activeToggle = allyToggleGroup.ActiveToggles().FirstOrDefault().gameObject;
        GameObject unitName, unitImage, unitParam, unitDesc;

        unitName = allyStatusPanel.transform.Find("UnitNameText").gameObject;
        unitImage = allyStatusPanel.transform.Find("UnitImage").gameObject;
        unitParam = allyStatusPanel.transform.Find("UnitParam").transform.Find("UnitParamValue").gameObject;
        unitDesc = allyStatusPanel.transform.Find("UnitDescText").gameObject;

        prevToggleID = activeToggleID;
        activeToggleID = allyMenuToggleList.IndexOf(activeToggle);
        evolveUnitID = activeToggleID;

        bool toggleChanged = prevToggleID != activeToggleID;

        if (toggleChanged)
        {
            // ステータスパネルを設定
            //Debug.Log("Toggle changed from " + prevToggleID + " to " + activeToggleID);
            unitName.GetComponent<Text>().text = allyDataList[activeToggleID].jpName;
            unitImage.GetComponent<Image>().sprite = allyDataList[activeToggleID].unitSprite;

            List<int> statusList = new List<int> { allyDataList[activeToggleID].Maxhp, allyDataList[activeToggleID].atk, allyDataList[activeToggleID].def, allyDataList[activeToggleID].spd };
            unitParam.GetComponent<Text>().text = "\n" + string.Join("\n", statusList.ConvertAll<string>(x => x.ToString()));

            // 前のキャラのスキルボタンを消去
            foreach (GameObject obj in skillBtnList)
            {
                Destroy(obj, 0f);
            }

            List<string> skillNames = new List<string>();
            foreach (SkillStatus skill in allyDataList[activeToggleID].skillList)
            {
                skillNames.Add(skill.jpName);
            }

            skillBtnList = GenerateSkillBtnList(skillNames);

            unitDesc.GetComponent<Text>().text = "キャラ説明文";
        }
    }

    public void PartyMenuToggleStateChange()
    {
        //Debug.Log("Party Menu Toggle State Changed");
        GameObject activeToggle = partyToggleGroup.ActiveToggles().FirstOrDefault().gameObject;
        GameObject paramObj, skillObj;

        paramObj = partyStatusPanel.transform.Find("UnitParam").transform.Find("UnitParamValue").gameObject;
        skillObj = partyStatusPanel.transform.Find("UnitSkillText").gameObject;

        prevToggleID = activeToggleID;
        activeToggleID = partyMenuToggleList.IndexOf(activeToggle);

        bool toggleChanged = prevToggleID != activeToggleID;

        if (toggleChanged)
        {
            // ステータスパネルを設定
            //Debug.Log("Toggle changed from " + prevToggleID + " to " + activeToggleID);

            List<int> statusList = new List<int> { allyDataList[activeToggleID].Maxhp, allyDataList[activeToggleID].atk, allyDataList[activeToggleID].def, allyDataList[activeToggleID].spd };
            paramObj.GetComponent<Text>().text = "\n" + string.Join("\n", statusList.ConvertAll<string>(x => x.ToString()));

            List<string> skillNames = new List<string>() { "特殊技能" };

            foreach (SkillStatus skill in allyDataList[activeToggleID].skillList)
            {
                skillNames.Add(skill.jpName);
            }

            skillObj.GetComponent<Text>().text = string.Join("\n", skillNames.ToArray());
        }
    }

    public void EvolveMenuToggleStateChange()
    {
        //Debug.Log("Evolve Menu Toggle State Change");
        GameObject activeToggle = evolveToggleGroup.ActiveToggles().FirstOrDefault().gameObject;
        GameObject paramChange, newSkillText, riskText;

        paramChange = evolveStatusPanel.transform.Find("UnitParam").transform.Find("ChangeValue").gameObject;
        newSkillText = evolveStatusPanel.transform.Find("NewSkillText").gameObject;
        riskText = evolveStatusPanel.transform.Find("RiskText").gameObject;

        prevToggleID = activeToggleID;
        activeToggleID = evolveMenuToggleList.IndexOf(activeToggle);

        bool toggleChanged = prevToggleID != activeToggleID;
        GeneData activeGeneData = geneDataList[activeToggleID];

        if (toggleChanged)
        {
            // ステータスパネルを設定
            List<int> statusList = new List<int> { activeGeneData.hp, activeGeneData.atk, activeGeneData.def, activeGeneData.spd };
            paramChange.GetComponent<Text>().text = "\n" + string.Join("\n", statusList.ConvertAll<string>(x => x.ToString()));

            // 追加スキルの表記を更新
            newSkillText.GetComponent<Text>().text = "追加される能力\n" + activeGeneData.skill.jpName + "\n" + activeGeneData.skill.desc;

            // リスクレベルを更新
            (string riskString, Color riskTextColor) = RiskLevelString(activeGeneData.risk);
            riskText.GetComponent<Text>().text = riskString;
            riskText.GetComponent<Text>().color = riskTextColor;
        }
    }

    public void CloseButtonPressed()
    {
        // 閉じるボタンを押したら対応したパネルを閉じる。閉じるボタンはパネルの子オブジェクトでなくてはいけない
        GameObject panelObj = eventSystem.currentSelectedGameObject.transform.parent.gameObject;
        PanelController.DisablePanel(panelObj);
    }

    #region パーティー編成画面のボタンコールバック
    public void AddToPartyButtonPressed()
    {
        // パーティー編成画面のパーティーに追加ボタンのコールバック
        GameObject activeToggle = partyToggleGroup.ActiveToggles().FirstOrDefault().gameObject;

        if (activeToggle == null)
        {
            //Debug.Log("キャラが選択されていない");
            ShowMessagePanel1(true, "味方生物が択されていない");
            return;
        }

        int toggleID = partyMenuToggleList.IndexOf(activeToggle);
        
        if (partyMemberID.IndexOf(toggleID) != -1)
        {
            Debug.Log("そのキャラは既に追加されている");
            ShowMessagePanel1(true, "その生物は既に編成されている");
            return;
        }

        CharacterData addedUnitData = allyDataList[toggleID];
        memberImageList[partyMemberID.Count].GetComponent<Image>().sprite = addedUnitData.unitSprite;
        
        partyMemberID.Add(toggleID); // 何番目のキャラがパーティーに追加されたか保存
        
        if (partyMemberID.Count == 1)
        {
            //Debug.Log("最初のメンバーが追加された。出撃ボタンのロックを解除");
            finalizePartyButton.GetComponent<Button>().interactable = true;
        }

        if (partyMemberID.Count == memberImageList.Count)
        {
            //Debug.Log("編成可能数の上限に到達。追加ボタンをロック");
            addUnitButton.GetComponent<Button>().interactable = false;
        }
    }

    public void RedoPartyButtonPressed()
    {
        //Debug.Log("パーティーメンバーの選択をやり直す");
        // リストをクリア
        partyMemberID = new List<int>();

        // 画像をクリア
        foreach (GameObject img in memberImageList)
        {
            img.GetComponent<Image>().sprite = null;
        }

        // ボタンを初期化
        addUnitButton.GetComponent<Button>().interactable = true;
        finalizePartyButton.GetComponent<Button>().interactable = false;
    }

    public void FinalizePartyButtonPressed()
    {
        //Debug.Log("確認メッセージを表示");
        ShowMessagePanel2(true, "本当に向かうか？");
    }

    public void OKButtonPressed()
    {
        //Debug.Log("OKボタンが押された");
        ShowMessagePanel1(false);
    }

    public void YesButtonPressed()
    {
        //Debug.Log("はいが押された。戦闘を開始する");
        foreach (int i in partyMemberID)
        {
            partyDataList.Add(allyDataList[i]);
        }

        ManageData.SavePartyData(partyDataList);
        ManageData.SaveCharacterData(allyDataList);
        SceneController.ToBattleScene();
    }

    public void NoButtonPressed()
    {
        //Debug.Log("いいえが押された。メッセージダイアログを閉じる");
        ShowMessagePanel2(false);
    }
    #endregion

    public void EvolveButtonPressed()
    {
        // 育成画面を開く準備
        eventSystem = EventSystem.current;
        button = eventSystem.currentSelectedGameObject;
        btnID = btnList.IndexOf(button);

        CharacterData evolveUnitData = allyDataList[evolveUnitID]; // 育成されるキャラのデータ
        GameObject unitName, originalParam;
        GameObject skillTextObj = evolveStatusPanel.transform.Find("SkillText").gameObject;

        // 育成画面のキャラクターステータスを設定
        originalParam = evolveStatusPanel.transform.Find("UnitParam").transform.Find("BeforeValue").gameObject;
        unitName = evolveStatusPanel.transform.Find("UnitNameText").gameObject;

        unitName.GetComponent<Text>().text = evolveUnitData.jpName;
        List<int> statusList = new List<int> { evolveUnitData.Maxhp, evolveUnitData.atk, evolveUnitData.def, evolveUnitData.spd };
        originalParam.GetComponent<Text>().text = "\n" + string.Join("\n", statusList.ConvertAll<string>(x => x.ToString()));

        // 現在所持しているスキルを表示
        List<string> skillNameList = new List<string>();
        foreach (SkillStatus skillStatus in evolveUnitData.skillList)
        {
            skillNameList.Add(skillStatus.jpName);
        }

        skillTextObj.GetComponent<Text>().text = "特殊能力\n" + string.Join("\n", skillNameList);

        TakeButtonAction(btnID);

    }

    #region トグルやボタンを作成
    private List<GameObject> ToggleListFromAllyData(GameObject toggleParentObj, ToggleGroup toggleGroup)
    {
        List<GameObject> toggleList = new List<GameObject>();

        for (int i = 0; i < allyDataList.Count; i++)
        {
            GameObject toggleObj = Instantiate(togglePrefab, toggleParentObj.transform) as GameObject;
            toggleObj.GetComponent<Toggle>().group = toggleGroup;
            toggleObj.GetComponent<Toggle>().GetComponentInChildren<Text>().text = allyDataList[i].jpName;
            toggleList.Add(toggleObj);
        }

        return toggleList;
    }

    private List<GameObject> ToggleListFromGeneData(GameObject toggleParentObj, ToggleGroup toggleGroup)
    {
        List<GameObject> toggleList = new List<GameObject>();

        for (int i = 0; i < geneDataList.Count; i++)
        {
            GameObject toggleObj = Instantiate(togglePrefab, toggleParentObj.transform) as GameObject;
            toggleObj.GetComponent<Toggle>().group = toggleGroup;
            toggleObj.GetComponent<Toggle>().GetComponentInChildren<Text>().text = geneDataList[i].jpName;
            toggleList.Add(toggleObj);
        }

        return toggleList;
    }

    private List<GameObject> GenerateSkillBtnList(List<string> nameList)
    {
        List<GameObject> btnObjList = new List<GameObject>();

        for (int i = 0; i < nameList.Count; i++)
        {
            GameObject btnObj = Instantiate(skillNameBtnPrefab, skillNameParent.transform) as GameObject;
            btnObj.GetComponent<Button>().onClick.AddListener(delegate { ShowSkillDesc(); });

            btnObj.GetComponentInChildren<Text>().text = nameList[i];
            btnObjList.Add(btnObj);
        }

        return btnObjList;
    }
    #endregion

    private void ShowMessagePanel1(bool show, string msgString = "")
    {
        if (show)
        {
            //Debug.Log("パネルを表示する");
            GameObject textObj = msgPanel1.transform.Find("TextPanel").Find("MsgText").gameObject;
            textObj.GetComponent<Text>().text = msgString;
            msgPanel1.SetActive(true);
        }

        else
        {
            msgPanel1.SetActive(false);
        }
    }

    private void ShowMessagePanel2(bool show, string msgString = "")
    {
        if (show)
        {
            //Debug.Log("パネルを表示する");
            GameObject textObj = msgPanel2.transform.Find("TextPanel").Find("MsgText").gameObject;
            textObj.GetComponent<Text>().text = msgString;
            msgPanel2.SetActive(true);
        }

        else
        {
            msgPanel2.SetActive(false);
        }
    }

    private void ShowSkillDesc()
    {
        Debug.Log("ShowSkillDesc");

        GameObject clickedBtnObj = EventSystem.current.currentSelectedGameObject;
        Debug.Log("Clicked Object: " + clickedBtnObj.name);
        int skillTextID = skillBtnList.IndexOf(clickedBtnObj);
        string skillName = allyDataList[activeToggleID].skillList[skillTextID].jpName;
        string skillDesc = allyDataList[activeToggleID].skillList[skillTextID].desc;
        skillDescObj.GetComponent<Text>().text = skillName + "\n" + skillDesc;
    }

    private (string riskString, Color textColor) RiskLevelString(int riskPercent)
    {
        Color color = Color.black;

        string riskString = "リスクレベル：";
        switch (riskPercent)
        {
            case int i when (i <= 5):
                riskString += "Minimum\n" + "危険度は非常に低い。想定外の変異が起こる可能性は極めて低いだろう。";
                color = new Color(0f, 0f, 1f, 1f);
                break;

            case int i when (i > 5 && i <= 20):
                riskString += "Minor\n" + "危険度は低いと言える。予想通りの結果になることは十分期待できる。";
                color = new Color(0f, 1f, 0f, 1f);
                break;

            case int i when (i > 20 && i <= 30):
                riskString += "Medium\n" + "危険度は中程度。想定外の結果に注意が必要だろう。";
                color = new Color(0.7f, 0.8f, 0.1f, 1f);
                break;

            case int i when (i > 30 && i <= 50):
                riskString += "Warning\n" + "やや危険度の高い変異だろう。無視できない確率で想定外な結果となるだろう。";
                color = new Color(0.9f, 0.6f, 0.1f, 1f);
                break;

            case int i when (i > 50 && i <= 75):
                riskString += "Caution\n" + "危険度が高い。想定外の結果を覚悟しておく必要があるだろう。";
                color = new Color(1.0f, 0f, 0f, 1f);
                break;

            case int i when (i > 75):
                riskString += "Danger\n" + "極めて危険度な変異となるだろう。結果を予測することは不可能に近い。";
                color = new Color(0.7f, 0f, 1f, 1f);
                break;

        }

        return (riskString, color);
    }

    public void TestOnClick()
    {
        Debug.Log("Object clicked");
    }
}
