﻿using System.Collections;
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
    public GameObject allyMenuToggleParent, partyMenuToggleParent; // トグルの親オブジェクト。ToggleGroupを持つ
    public GameObject allyStatusPanel, partyStatusPanel;
    public List<GameObject> memberImageList; // パーティーメンバー画像オブジェクト
    public GameObject addUnitButton, redoPartyButton, finalizePartyButton; // パーティー編成画面のボタン

    // スクリプトで生成
    public List<GameObject> allyMenuToggleList, partyMenuToggleList; // 味方管理画面のトグルリスト、パーティー編成画面のトグルリスト
    public List<CharacterData> partyDataList; // バトルシーンに渡すパーティーメンバーのデータ

    private EventSystem eventSystem;
    public List<GameObject> btnList;
    private GameObject button, prevButton;
    private ButtonResponseSize btnResp;
    private int btnID, prevBtnID;
    private int nPanels; // number of panels in popupPanelList
    private int nMenuBtns;
    private ToggleGroup allyToggleGroup, partyToggleGroup;
    private int activeToggleID, prevToggleID;
    private List<int> partyMemberID; 
    #endregion


    private void Awake()
    {
        // GameControllerからキャラデータをロードする
        allyDataList = ManageCharacterData.LoadCharacterData();


        // FOR TESTING PURPOSES. DELETE WHEN UNNEEDED //
        if (allyDataList.Count == 0)
        {
            allyDataList = new List<CharacterData>();
            allyDataList.Add(ManageCharacterData.DataFromSO("nezumi", true));
            allyDataList.Add(ManageCharacterData.DataFromSO("ka", true));
            allyDataList.Add(ManageCharacterData.DataFromSO("ka", true));
            allyDataList.Add(ManageCharacterData.DataFromSO("ka", true));
        }
        
    }


    private void Start()
    {
        btnList = menuBtnList;
        nPanels = popupPanelList.Count;
        nMenuBtns = menuBtnList.Count;
        partyDataList = new List<CharacterData>();
        partyMemberID = new List<int>();
        allyToggleGroup = allyMenuToggleParent.GetComponent<ToggleGroup>();
        partyToggleGroup = partyMenuToggleParent.GetComponent<ToggleGroup>();
        
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
        switch (btnID)
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

    public void AllyMenuToggleStateChange()
    {
        // アクティブなToggleに対応したキャラのステータスを表示する
        //Debug.Log("Ally Menu Toggle State Changed");
        GameObject activeToggle = allyToggleGroup.ActiveToggles().FirstOrDefault().gameObject;
        GameObject unitName, unitImage, unitParam, unitSkill, unitDesc;

        unitName = allyStatusPanel.transform.Find("UnitNameText").gameObject;
        unitImage = allyStatusPanel.transform.Find("UnitImage").gameObject;
        unitParam = allyStatusPanel.transform.Find("UnitParam").transform.Find("UnitParamValue").gameObject;
        unitSkill = allyStatusPanel.transform.Find("UnitSkillText").gameObject;
        unitDesc = allyStatusPanel.transform.Find("UnitDescText").gameObject;

        prevToggleID = activeToggleID;
        activeToggleID = allyMenuToggleList.IndexOf(activeToggle);

        bool toggleChanged = prevToggleID != activeToggleID;

        if (toggleChanged)
        {
            // ステータスパネルを設定
            //Debug.Log("Toggle changed from " + prevToggleID + " to " + activeToggleID);
            unitName.GetComponent<Text>().text = allyDataList[activeToggleID].jpName;
            unitImage.GetComponent<Image>().sprite = allyDataList[activeToggleID].unitSprite;

            List<int> statusList = new List<int> { allyDataList[activeToggleID].Maxhp, allyDataList[activeToggleID].atk, allyDataList[activeToggleID].def, allyDataList[activeToggleID].spd };
            unitParam.GetComponent<Text>().text = "\n" + string.Join("\n", statusList.ConvertAll<string>(x => x.ToString()));

            List<string> skillNames = new List<string>() { "特殊技能" };

            foreach (SkillStatus skill in allyDataList[activeToggleID].skillList)
            {
                skillNames.Add(skill.jpName);
            }

            unitSkill.GetComponent<Text>().text = string.Join("\n", skillNames.ToArray());
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


    #region パーティー編成画面のボタンコールバック
    public void AddToPartyButtonPressed()
    {
        // パーティー編成画面のパーティーに追加ボタンのコールバック
        GameObject activeToggle = partyToggleGroup.ActiveToggles().FirstOrDefault().gameObject;

        if (activeToggle == null)
        {
            Debug.Log("キャラが選択されていない");
            return;
        }

        int toggleID = partyMenuToggleList.IndexOf(activeToggle);
        
        if (partyMemberID.IndexOf(toggleID) != -1)
        {
            Debug.Log("そのキャラは既に追加されている");
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
        //Debug.Log("パーティーを確定して戦闘を開始");
        foreach (int i in partyMemberID)
        {
            partyDataList.Add(allyDataList[i]);
        }

        ManageCharacterData.SavePartyData(partyDataList);
        ManageCharacterData.SaveCharacterData(allyDataList);
        SceneController.ToBattleScene();
    }
    #endregion

    public List<GameObject> ToggleListFromAllyData(GameObject toggleParentObj, ToggleGroup toggleGroup)
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
}
