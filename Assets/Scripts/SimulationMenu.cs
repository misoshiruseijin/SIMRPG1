using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class SimulationMenu : MonoBehaviour
{
    #region Variable Definitions

    #region インスペクターで設定する項目
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

    public GameObject HUDPanel;

    public ChangeScene sceneChanger;
    public Text scDateText; // 1日の初めに表示する日付テキスト
    #endregion

    #region スクリプトで生成する項目
    public List<GameObject> allyMenuToggleList, partyMenuToggleList; // 味方管理画面のトグルリスト、パーティー編成画面のトグルリスト
    public List<GameObject> evolveMenuToggleList; // 育成画面の遺伝子アイテムトグルリスト

    public List<CharacterData> allyDataList; // 味方キャラのステータスデータ
    public List<CharacterData> partyDataList; // バトルシーンに渡すパーティーメンバーのデータ
    public List<GameObject> skillBtnList; // 各キャラのスキル名オブジェクトリスト
    public List<GeneData> geneDataList; // 所持している遺伝子アイテムのリスト

    private int day, food, survivors, phase;
    private int evolveDays; // 変異完了までにかかる日数
    private int evolvingUnitID; // 変異中のユニットのID (evolveUnitIDはAllyMenuが更新されると更新される。evolvingUnitIDは次の変異を開始するまで変化しない)
    private bool isEvolving; // 変異中フラグ

    private EventSystem eventSystem;
    private List<GameObject> btnList;
    private GameObject button, prevButton;
    private ButtonResponseSize btnResp;
    private int btnID, prevBtnID;
    private int nPanels; // number of panels in popupPanelList
    private int nMainBtns;
    private ToggleGroup allyToggleGroup, partyToggleGroup, evolveToggleGroup;
    private int activeToggleID, prevToggleID;
    private int evolveUnitID, useGeneID; // 育成されるユニットID、使用する遺伝子アイテムID
    private List<int> partyMemberID;
    private GameObject skillDescObj;
    private DialogBox dialog;
    #endregion
    #endregion


    private void Awake()
    {
        // GameControllerからキャラデータをロードする
        allyDataList = ManageData.LoadCharacterData();
        geneDataList = ManageData.LoadGeneData();

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

        scDateText.text = $"{GameController.instance.day}日目";
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
        dialog = DialogBox.Instance();

        activeToggleID = -1; // default value

        // HUDの設定
        day = GameController.instance.day;
        food = GameController.instance.food;
        survivors = GameController.instance.survivors;

        Text infoText = HUDPanel.transform.Find("InfoText").gameObject.GetComponent<Text>();
        Text foodText = HUDPanel.transform.Find("FoodText").gameObject.GetComponent<Text>();
        Text survivorText = HUDPanel.transform.Find("SurvivorText").gameObject.GetComponent<Text>();
        Text dateText = HUDPanel.transform.Find("DateText").gameObject.GetComponent<Text>();

        infoText.text = $"食料消費： {allyDataList.Count + survivors} /日";
        foodText.text = $"X {food}";
        survivorText.text = $"X {survivors}";
        dateText.text = $"{day} 日目";

        isEvolving = GameController.instance.isEvolving;

        // 変異中なら、変異が完了したか確認
        if (isEvolving && GameController.instance.daysFromEvolve >= GameController.instance.evolveDays)
        {
            EvolveComplete();
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

        // NEW // 
        // いったん全部非表示
        if (btnID < nMainBtns)
        {
            for (int i = 0; i < nPanels; i++)
            {
                PanelController.DisablePanel(popupPanelList[i]);
            }
        }
        PanelController.EnablePanel(popupPanelList[btnID]);

        switch (btnID)
        {
            case 0:
                // 味方管理画面へ
                // トグルオブジェクトを作成 (初期状態では全部オフ)
                foreach(GameObject toggleObj in allyMenuToggleList)
                {
                    Destroy(toggleObj, 0f);
                }
                allyMenuToggleList = ToggleListFromAllyData(allyMenuToggleParent, allyToggleGroup);
                foreach (GameObject toggleObj in allyMenuToggleList)
                {
                    toggleObj.GetComponent<Toggle>().onValueChanged.AddListener((bool value) => AllyMenuToggleStateChange());
                }
                break;

            case 1:
                // パーティー編成へ
                // トグルオブジェクトを作成 (初期状態では全部オフ)
                foreach (GameObject toggleObj in partyMenuToggleList)
                {
                    Destroy(toggleObj, 0f);
                }
                partyMenuToggleList = ToggleListFromAllyData(partyMenuToggleParent, partyToggleGroup);
                foreach (GameObject toggleObj in partyMenuToggleList)
                {
                    toggleObj.GetComponent<Toggle>().onValueChanged.AddListener((bool value) => PartyMenuToggleStateChange());
                }
                break;

            case 5:
                // 変異画面を重ねて表示
                // トグルオブジェクトを作成 (初期状態では全部オフ)
                foreach (GameObject toggleObj in evolveMenuToggleList)
                {
                    Destroy(toggleObj, 0f);
                }
                evolveMenuToggleList = ToggleListFromGeneData(evolveMenuToggleParent, evolveToggleGroup);
                foreach (GameObject toggleObj in evolveMenuToggleList)
                {
                    toggleObj.GetComponent<Toggle>().onValueChanged.AddListener((bool value) => EvolveMenuToggleStateChange());
                }
                break;
            
            default:
                Debug.Log("会話、マップ、設定のどれかを表示");
                break;
        }
        // NEW END // 
    }

    public void AllyMenuToggleStateChange()
    {
        // アクティブなToggleに対応したキャラのステータスを表示する
        //Debug.Log("Ally Menu Toggle State Changed");
        GameObject activeToggle = allyToggleGroup.ActiveToggles().FirstOrDefault().gameObject;

        prevToggleID = activeToggleID;
        activeToggleID = allyMenuToggleList.IndexOf(activeToggle);
        evolveUnitID = activeToggleID;
        bool toggleChanged = prevToggleID != activeToggleID;

        if (toggleChanged)
        {
            //Debug.Log("ToggleState Changed");
            UpdateAllyMenu();
        }
    }

    public void PartyMenuToggleStateChange()
    {
        //Debug.Log("Party Menu Toggle State Changed");
        GameObject activeToggle = partyToggleGroup.ActiveToggles().FirstOrDefault().gameObject;
        
        prevToggleID = activeToggleID;
        activeToggleID = partyMenuToggleList.IndexOf(activeToggle);
        bool toggleChanged = prevToggleID != activeToggleID;

        if (toggleChanged)
        {
            UpdatePartyMenu();
        }
    }

    public void EvolveMenuToggleStateChange()
    {
        //Debug.Log("Evolve Menu Toggle State Change");
        GameObject activeToggle = evolveToggleGroup.ActiveToggles().FirstOrDefault().gameObject;
        
        prevToggleID = activeToggleID;
        activeToggleID = evolveMenuToggleList.IndexOf(activeToggle);
        bool toggleChanged = prevToggleID != activeToggleID;

        if (toggleChanged)
        {
            UpdateEvolveMenu();
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
            dialog.SingleButtonMode(true);
            dialog.SetMessage("味方生物が選択されていない").SetOnOK("了解", () => { dialog.Hide(); });
            dialog.Show();
            //ShowMessagePanel1(true, "味方生物が択されていない");
            return;
        }

        int toggleID = partyMenuToggleList.IndexOf(activeToggle);
        
        if (partyMemberID.IndexOf(toggleID) != -1)
        {
            //Debug.Log("そのキャラは既に追加されている");
            dialog.SingleButtonMode(true);
            dialog.SetMessage("その生物は既に編成されている").SetOnOK("了解", ()=> { dialog.Hide(); });
            dialog.Show();
            //ShowMessagePanel1(true, "その生物は既に編成されている");
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
        //ShowMessagePanel2(true, "本当に向かうか？");
        dialog.SingleButtonMode(false);
        dialog.SetMessage("本当に向かうか？");
        dialog.SetOnAccept("はい", () => { StartBattle(); }).SetOnDecline("いいえ", () => { dialog.Hide(); });
        dialog.Show();
    }

    public void StartBattle()
    {
        //Debug.Log("はいが押された。戦闘を開始する");
        foreach (int i in partyMemberID)
        {
            partyDataList.Add(allyDataList[i]);
        }

        ManageData.SavePartyData(partyDataList);
        ManageData.SaveCharacterData(allyDataList);
        ManageData.SaveGeneData(geneDataList);
        SceneController.ToBattleScene();
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

    public void StartEvolveButtonPressed()
    {
        Debug.Log("変異開始ボタンが押された");
        CloseAllPanels();
        // 代わりに進化演出を入れる

        // NEW //
        evolvingUnitID = evolveUnitID;
        GameController.instance.daysFromEvolve = 0;
        evolveDays = 2; // 本来はgenedataから取得
        isEvolving = true;
        GameController.instance.usedGeneID = useGeneID;
        GameController.instance.evolveDays = evolveDays;

        activeToggleID = -1; // 初期化
        // NEW END //

        #region 原文
        //CharacterData newData = allyDataList[evolveUnitID];
        //GeneData gene = geneDataList[useGeneID];
        //int riskPercent = gene.risk;

        //// ステータス変化を反映
        //if (riskPercent < 10)
        //{
        //    // リスク値10未満ならランダム要素なし
        //    newData.Maxhp += gene.hp;
        //    newData.atk += gene.atk;
        //    newData.def += gene.def;
        //    newData.spd += gene.spd;
        //}

        //else
        //{
        //    // ランダム要素あり。定数を計算
        //    int[] paramArray = new int[] { newData.Maxhp, newData.atk, newData.def, newData.spd };
        //    int[] growthArray = new int[] { gene.hp, gene.atk, gene.def, gene.spd };
        //    for (int i = 0; i < growthArray.Length; i++)
        //    {
        //        int factor = ProbabilityCalculator.GrowthFactor(riskPercent);
        //        //Debug.Log("Factor" + i + ": " + factor);
        //        paramArray[i] = Mathf.RoundToInt((1 + (factor / 100f)) * paramArray[i]);
        //    }

        //    newData.Maxhp = paramArray[0];
        //    newData.atk = paramArray[1];
        //    newData.def = paramArray[2];
        //    newData.spd = paramArray[3];
        //}

        //newData.skillList.Add(gene.skill);

        //allyDataList[evolveUnitID] = newData; // 進化前の情報を上書きする
        //geneDataList.RemoveAt(useGeneID); // アイテムリストから消費したアイテムを削除
        //Destroy(evolveMenuToggleList[useGeneID]); // 消費されたアイテムのトグルを削除
        //evolveMenuToggleList.RemoveAt(useGeneID); // トグルリストからトグルを削除

        //if (evolveMenuToggleList.Count > 0)
        //{
        //    updateEvolveMenuFlg = true;
        //}

        //activeToggleID = -1; // 初期化
        //updateStatusFlg = true;
        #endregion
    }

    public void NextDayButtonPressed()
    {
        dialog.SetMessage("今日はもう休みますか？");
        dialog.SetOnAccept("はい", () => { ToNextDay(); dialog.Hide(); });
        dialog.SetOnDecline("いいえ", () => { dialog.Hide(); });
        dialog.Show();
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

    private void ShowSkillDesc()
    {
        //Debug.Log("ShowSkillDesc");
        GameObject clickedBtnObj = EventSystem.current.currentSelectedGameObject;
        int skillTextID = skillBtnList.IndexOf(clickedBtnObj);
        string skillName = allyDataList[activeToggleID].skillList[skillTextID].jpName;
        string skillDesc = allyDataList[activeToggleID].skillList[skillTextID].desc;
        skillDescObj.GetComponent<Text>().text = skillName + "\n" + skillDesc;
    }

    private void UpdateAllyMenu()
    {
        //Debug.Log("UpdateAllyMenuCalled");
        GameObject unitName, unitImage, unitParam, unitDesc;
        Button evolveBtn;

        evolveBtn = allyStatusPanel.transform.Find("EvolveButton").gameObject.GetComponent<Button>(); // NEW
        unitName = allyStatusPanel.transform.Find("UnitNameText").gameObject;
        unitImage = allyStatusPanel.transform.Find("UnitImage").gameObject;
        unitParam = allyStatusPanel.transform.Find("UnitParam").transform.Find("UnitParamValue").gameObject;
        unitDesc = allyStatusPanel.transform.Find("UnitDescText").gameObject;

        // ステータスパネルを設定
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

        // 変異中のキャラがいる場合、変異ボタンをロック
        if (isEvolving && activeToggleID == evolvingUnitID)
        {
            evolveBtn.interactable = false;
        }        
    }

    private void UpdatePartyMenu()
    {
        GameObject paramObj, skillObj;

        paramObj = partyStatusPanel.transform.Find("UnitParam").transform.Find("UnitParamValue").gameObject;
        skillObj = partyStatusPanel.transform.Find("UnitSkillText").gameObject;

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

    private void UpdateEvolveMenu()
    {
        //Debug.Log("UpdateEvolveMenu Called");
        GameObject paramChange, newSkillText, riskText;

        paramChange = evolveStatusPanel.transform.Find("UnitParam").transform.Find("ChangeValue").gameObject;
        newSkillText = evolveStatusPanel.transform.Find("NewSkillText").gameObject;
        riskText = evolveStatusPanel.transform.Find("RiskText").gameObject;

        useGeneID = activeToggleID;
        GeneData activeGeneData = geneDataList[activeToggleID];

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

    private void CloseAllPanels()
    {
        foreach (GameObject panel in popupPanelList)
        {
            PanelController.DisablePanel(panel);
        }
    }

    private void ToNextDay()
    {
        //Debug.Log("ToNextDay Called");
        // シーン更新前にデータを保存
        ManageData.SaveCharacterData(allyDataList);
        ManageData.SaveGeneData(geneDataList);
        day++;

        // 食料の残量更新
        if (food < survivors + allyDataList.Count)
        {
            food = 0;
        }
        else
        {
            food -= (survivors + allyDataList.Count);
        }

        // 変異中の場合
        if (isEvolving)
        {
            GameController.instance.daysFromEvolve++;
        }

        // GameControllerに保存
        GameController.instance.day = day;
        GameController.instance.food = food;
        GameController.instance.isEvolving = isEvolving;

        sceneChanger.NewDay();
    }

    private void EvolveComplete()
    {
        Debug.Log("変異が完了した");

        int geneID = GameController.instance.usedGeneID;
        isEvolving = false;
        GameController.instance.daysFromEvolve = 0;

        CharacterData newData = allyDataList[evolvingUnitID];
        GeneData gene = geneDataList[geneID];
        int riskPercent = gene.risk;

        // ステータス変化を反映
        if (riskPercent < 10)
        {
            // リスク値10未満ならランダム要素なし
            newData.Maxhp += gene.hp;
            newData.atk += gene.atk;
            newData.def += gene.def;
            newData.spd += gene.spd;
        }

        else
        {
            // ランダム要素あり。定数を計算
            int[] paramArray = new int[] { newData.Maxhp, newData.atk, newData.def, newData.spd };
            int[] growthArray = new int[] { gene.hp, gene.atk, gene.def, gene.spd };
            for (int i = 0; i < growthArray.Length; i++)
            {
                int factor = ProbabilityCalculator.GrowthFactor(riskPercent);
                //Debug.Log("Factor" + i + ": " + factor);
                paramArray[i] = Mathf.RoundToInt((1 + (factor / 100f)) * paramArray[i]);
            }

            newData.Maxhp = paramArray[0];
            newData.atk = paramArray[1];
            newData.def = paramArray[2];
            newData.spd = paramArray[3];
        }

        newData.skillList.Add(gene.skill);

        allyDataList[evolveUnitID] = newData; // 進化前の情報を上書きする
        geneDataList.RemoveAt(geneID); // アイテムリストから消費したアイテムを削除
        //Destroy(evolveMenuToggleList[geneID]); // 消費されたアイテムのトグルを削除
        //evolveMenuToggleList.RemoveAt(geneID); // トグルリストからトグルを削除

        activeToggleID = -1; // 初期化
    }

    public void TestOnClick()
    {
        Debug.Log("Object clicked");
    }
}
