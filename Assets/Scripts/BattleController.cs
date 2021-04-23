using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class BattleController : MonoBehaviour
{
    #region Variable Definitions
    Queue<Action> battleQueue;
    public List<GameObject> playerObjList; // 味方ユニット
    public List<GameObject> enemyObjList; // 敵ユニット
    public List<GameObject> statusPanelList; // 味方のステータス表示用パネル

    public bool actionSelectedFlg;

    public BattleMenu battleMenu;

    private List<CharacterData> allyData; // 戦闘に参加する味方ユニット
    private List<CharacterData> enemyData; // 戦闘に参加する敵ユニット

    private List<GameObject> unitObjList = new List<GameObject>(); // 戦闘に参加する全ユニットオブジェクト（マスター）
    private List<GameObject> allUnits = new List<GameObject>(); // unitObjListのコピー

    private string allyTag = "Ally";
    private string enemyTag = "Enemy";

    private int prevDamage;
    #endregion

    void Start()
    {
        // GameControllerからパーティー情報をロード
        allyData = ManageCharacterData.LoadPartyData();
        enemyData = ManageCharacterData.GenerateEnemyParty();

        battleMenu = GetComponent<BattleMenu>();

        InitializeBattleUnits(allyData, playerObjList);
        InitializeBattleUnits(enemyData, enemyObjList);

        // シーン上のオブジェクトの数を戦闘に参加するユニットの数に合わせる
        playerObjList = playerObjList.Take(allyData.Count).ToList();
        enemyObjList = enemyObjList.Take(enemyData.Count).ToList();

        Button[] targetBtns = battleMenu.targetPanel.GetComponentsInChildren<Button>();

        for (int i = 0; i < allyData.Count; i++)
        {
            unitObjList.Add(playerObjList[i]);
            allUnits.Add(playerObjList[i]);
        }
        for (int i = 0; i < enemyData.Count; i++)
        {
            unitObjList.Add(enemyObjList[i]);
            allUnits.Add(enemyObjList[i]);            
        }

        // targetメニューのボタンテキスト設定
        for (int i = 0; i < targetBtns.Length; i++)
        {
            if (i >= enemyData.Count)
            {
                battleMenu.SetMenuObjectText(targetBtns[i].GetComponentInChildren<Text>(), "");
            }
            else
            {
                battleMenu.SetMenuObjectText(targetBtns[i].GetComponentInChildren<Text>(), enemyObjList[i].GetComponent<Character>().jpName);
            }
        }

        // フラグ初期化
        actionSelectedFlg = false;

    }

    public void InitializeBattleUnits(List<CharacterData> partyMemberData, List<GameObject> unitObjList)
    {
        int n_units = partyMemberData.Count;
        int max_units = unitObjList.Count;

        if (n_units > max_units)
        {
            Debug.Log("バトルに参加できるのは" + max_units.ToString() + "体まで");
        }

        // 全ユニットオブジェクトを非表示
        foreach (GameObject unitObj in unitObjList)
        {
            unitObj.SetActive(false);
        }

        for (int i = 0; i < n_units; i++)
        {
            // GameControllerから受け取った味方パーティーのデータをバトルシーンの味方オブジェクトに設定                       
            Character character = unitObjList[i].GetComponent<Character>();
            character.Maxhp = partyMemberData[i].Maxhp;
            character.jpName = partyMemberData[i].jpName;
            character.atk = partyMemberData[i].atk;
            character.def = partyMemberData[i].def;
            character.spd = partyMemberData[i].spd;
            character.skillList = partyMemberData[i].skillList;
            character.SetStatus();

            // ステータスパネルを表示
            statusPanelList[i].SetActive(true);

            // キャラ画像を設定
            unitObjList[i].transform.Find("CharacterImage").GetComponent<Image>().sprite = partyMemberData[i].unitSprite;
            
            unitObjList[i].SetActive(true); // 画像が設定されたら非表示を解除                  
        }
    }

    public void StartAutoTurn()
    {
        battleMenu.ResetAllButtonStates();
        // spdステータスから行動順を決定
        allUnits = allUnits.OrderByDescending(unit => unit.GetComponent<Character>().spd).ToList(); // spdが高い順に並べる
        //Debug.Log("行動順は" + String.Join(" ", allUnits));

        // バトルキューを作成
        battleQueue = new Queue<Action>();

        foreach (GameObject unit in allUnits)
        {
            EnqueueAuto(unit);
        }

        battleMenu.SetMenuStartAction();

        StartCoroutine("ActionCoroutine");
    }

    public IEnumerator StartManualTurn()
    {
        // バトルキューを作成
        battleQueue = new Queue<Action>();

        // spdステータスから行動順を決定
        allUnits = allUnits.OrderByDescending(unit => unit.GetComponent<Character>().spd).ToList(); // spdが高い順に並べる
        //Debug.Log("行動順は" + String.Join(" ", allUnits));

        // マニュアルモードが選択された場合
        foreach (GameObject unit in allUnits)
        {
            // ユニットが敵なら行動をランダム生成
            if (unit.tag == enemyTag)
            {
                //Debug.Log("敵: " + unit.GetComponent<Character>().jpName);
                EnqueueAuto(unit);
            }

            // ユニットが味方なら
            else if (unit.tag == allyTag)
            {
                //Debug.Log("味方: " + unit.GetComponent<Character>().jpName);
                
                ManualActionSelect(unit);

                battleMenu.SetMenuNextUnit();
                battleMenu.ResetAllButtonStates();

                yield return StartCoroutine("WaitCoroutine");
                //Debug.Log("[!] Resuming StartManual");

                EnqueueManual(unit);
            }
        }

        battleMenu.SetMenuStartAction();

        StartCoroutine("ActionCoroutine");
    }

    public void ManualActionSelect(GameObject unit)
    {
        // BattleMenuがアクションパネルが表示している
        List<List<GameObject>> receiveUnits = new List<List<GameObject>>(); // 行動を受けるユニットのリスト
        List<GameObject> actionUnit = new List<GameObject> { unit }; // 行動するユニット
        List<List<int>> damages = new List<List<int>>();
        List<List<string>> logStringList = new List<List<string>>();
        Character character = unit.GetComponent<Character>();

        List<SkillStatus> skillList = character.skillList; // ユニットのスキルリストを取得
        
        // BattleMenuにスキル対象リスト（アクション確定判定のため）と
        // スキル解説文リスト（スキルパネルに表示するため）をわたす
        List<int> targetTypes = new List<int>();
        List<string> skillDescs = new List<string>();
        foreach(SkillStatus skill in skillList)
        {
            targetTypes.Add(skill.targetList[0]);
            skillDescs.Add(skill.desc);
        }
        battleMenu.skillTargetTypes = targetTypes;
        battleMenu.skillDescStrings = skillDescs;

        // アクションパネルにキャラ名を表示
        battleMenu.actionPanel.GetComponentInChildren<Text>().text = character.jpName;

        // キャラのスキル情報をスキルパネルに表示
        Button[] skillBtns = battleMenu.skillPanel.GetComponentsInChildren<Button>();
        for (int i = 0; i < skillBtns.Length; i++)
        {
            if (i >= skillList.Count)
            {
                skillBtns[i].GetComponentInChildren<Text>().text = "";
            }
            else
            {
                skillBtns[i].GetComponentInChildren<Text>().text = skillList[i].jpName;
            }
        }

    }

    public void EnqueueManual(GameObject unit)
    {
        List<List<GameObject>> receiveUnits = new List<List<GameObject>>(); // 行動を受けるユニットのリスト
        List<GameObject> actionUnit = new List<GameObject> { unit }; // 行動するユニット
        List<List<int>> damages = new List<List<int>>();
        List<List<string>> logStringList = new List<List<string>>();
        Character character = unit.GetComponent<Character>();

        if (battleMenu.guardFlg)
        {
            // 防御行動が選択されている
        }

        else
        {
            // スキルが選択されている　→　receiveUnitsにターゲットを追加

            SkillStatus skill = character.skillList[battleMenu.skillNumber];

            // スキルの一つ目の効果とターゲット
            if (battleMenu.targetNumber == 20)
            {
                // 全体攻撃スキルが選択されている
                List<GameObject> tempList = ChooseTargetUnits(skill.targetList[0], unit);
                receiveUnits.Add(tempList);
            }
            else
            {
                // 単体攻撃とターゲットが選択されている
                List<GameObject> tempList = ChooseTargetUnits(skill.targetList[0], unit, battleMenu.targetNumber);
                receiveUnits.Add(tempList);
            }

            // スキルの二つ目以降の効果とターゲット　（注：2つ目以降の効果はターゲット選択の余地がないものとしている）
            bool firstElement = true;
            foreach (int targetType in skill.targetList)
            {
                if (firstElement)
                {
                    firstElement = false;
                    continue;
                }

                List<GameObject> tempList = ChooseTargetUnits(targetType, unit);
                receiveUnits.Add(tempList);
            }

            // ダメージリストとログリストを作成
            damages = CalculateDamage(character, receiveUnits, skill.actionTypeList, skill.multList);
            logStringList = GenerateLogStrings(receiveUnits, damages, skill);
            
            // バトルキューにアクションを追加
            List<Action> tempActionList = new List<Action>();
            tempActionList = GenerateActions(receiveUnits, damages, logStringList, skill.effectList, actionUnit, skill);
            foreach (Action action in tempActionList)
            {
                battleQueue.Enqueue(action);
            }
        }

    }

    public void EnqueueAuto(GameObject unit)
    {
        List<List<GameObject>> receiveUnits = new List<List<GameObject>>(); // 行動を受けるユニットのリスト
        List<GameObject> actionUnit = new List<GameObject> { unit }; // 行動するユニット
        List<List<int>> damages = new List<List<int>>();
        List<List<string>> logStringList = new List<List<string>>();
        Character character = unit.GetComponent<Character>();

        List<SkillStatus> skillList = character.skillList; // ユニットのスキルリストを取得
        // 使うスキルをランダムに選ぶ
        SkillStatus skill = skillList[0];
        if (skillList.Count > 1)
        {
            skill = skillList[UnityEngine.Random.Range(0, skillList.Count)];
        }

        //Debug.Log($"{character.jpName}が使うスキルは{skill.jpName}");

        // スキルを受けるユニットをリストに格納
        foreach (int targetType in skill.targetList)
        {
            List<GameObject> tempList = ChooseTargetUnits(targetType, unit);
            receiveUnits.Add(tempList);
            //Debug.Log("スキルを受けるのは" + String.Join(" ", tempList));
        }

        // ダメージリストを作成
        damages = CalculateDamage(character, receiveUnits, skill.actionTypeList, skill.multList);

        // バトルログテキストのリストを作成
        logStringList = GenerateLogStrings(receiveUnits, damages, skill);

        // バトルキューにアクションを追加
        List<Action> tempActionList = new List<Action>();
        tempActionList = GenerateActions(receiveUnits, damages, logStringList, skill.effectList, actionUnit, skill);
        foreach (Action action in tempActionList)
        {
            battleQueue.Enqueue(action);
        }

    }

    public List<GameObject> ChooseTargetUnits(int targetCode, GameObject unit, int targetNum = 10)
    {
        // unit = スキル使用者, targetCode = TargetListのintコード
        List<GameObject> targetUnits = new List<GameObject>(); // アウトプット
        List<GameObject> enemyTargets = new List<GameObject>(); // スキル使用者の敵ユニットリスト
        List<GameObject> allyTargets = new List<GameObject>(); // スキル使用者の味方ユニットリスト
        string unitTag = unit.tag; // ユニット判別用タグ

        // 敵味方を判断
        if (unitTag.Equals(allyTag))
        {
            allyTargets = playerObjList;
            enemyTargets = enemyObjList;
        }
        else if (unitTag.Equals(enemyTag))
        {
            allyTargets = enemyObjList;
            enemyTargets = playerObjList;
        }
        else
        {
            Debug.Log("インスペクターでユニットオブジェクトに設定されたタグを確認してください");
        }

        // TargetTypeからActionを受けるユニットのリストを作成
        switch (targetCode)
        {
            case 0:
                // 敵単体が対象
                if (targetNum == 10)
                {
                    // ターゲット指定のないオート戦闘の場合、敵ユニットの中からランダムに1体選択
                    targetUnits.Add(enemyTargets[UnityEngine.Random.Range(0, enemyTargets.Count)]);
                }
                else
                {
                    // マニュアル戦闘でターゲットが指定されている
                    targetUnits.Add(enemyTargets[targetNum]);
                }
                
                break;

            case 1:
                // 敵全体が対象
                targetUnits = enemyTargets;
                break;

            case 2:
                // 味方単体が対象
                if (targetNum == 10)
                {
                    // ターゲット指定のないオート戦闘の場合、味方ユニットの中からランダムに1体選択
                    targetUnits.Add(allyTargets[UnityEngine.Random.Range(0, allyTargets.Count)]);
                }
                else
                {
                    // マニュアル戦闘でターゲットが指定されている
                    targetUnits.Add(allyTargets[targetNum]);
                }
                break;

            case 3:
                // 味方全体が対象
                targetUnits = allyTargets;
                break;

            case 4:
                // スキル発動者が対象
                targetUnits.Add(unit);
                break;

            default:
                Debug.Log("TargetTypeが存在しません");
                break;
        }

        return targetUnits;
    }

    public List<List<int>> CalculateDamage(Character skillUser, List<List<GameObject>> receiveUnits, List<int> actionTypesList, List<int> multList)
    {
        // character = Character component of unit using the skill

        List<List<int>> damageList = new List<List<int>>();

        for (int i = 0; i < actionTypesList.Count; i++)
        {
            List<int> tempList = new List<int>();
            int atk = skillUser.atk; // スキル使用者の攻撃力を取得

            switch (actionTypesList[i])
            {
                case 0:
                    // ダメージを与える効果
                    foreach (GameObject targetUnit in receiveUnits[i])
                    {
                        Character targetCharacter = targetUnit.GetComponent<Character>();
                        int def = targetCharacter.def;
                        int damage = atk * multList[i] - def;
                        tempList.Add(damage);
                        prevDamage = damage;
                    }
                    break;

                case 1:
                    // 回復効果
                    foreach (GameObject targetUnit in receiveUnits[i])
                    {
                        Character targetCharacter = targetUnit.GetComponent<Character>();
                        int damage = Mathf.RoundToInt((float)(-atk * multList[i] / 10));
                        tempList.Add(damage);
                    }
                    break;

                case 2:
                    // 直前に与えたダメージ依存の回復
                    foreach (GameObject targetUnit in receiveUnits[i])
                    {
                        int damage = Mathf.RoundToInt((float)(-prevDamage * multList[i] / 10));
                        tempList.Add(damage);
                    }
                    break;

            }

            damageList.Add(tempList);
            //Debug.Log("ダメージは" + String.Join(" ", tempList));
        }

        return damageList;
    }

    public List<List<string>> GenerateLogStrings(List<List<GameObject>> receiveUnitList, List<List<int>> damageList, SkillStatus skill)
    {
        List<List<string>> logList = new List<List<string>>();
        for (int i = 0; i < receiveUnitList.Count; i++)
        {
            // for each action type
            string msgString = "";
            List<string> tempList = new List<string>();

            for (int j = 0; j < receiveUnitList[i].Count; j++)
            {
                // for each unit 
                string receiveCharaName = receiveUnitList[i][j].GetComponent<Character>().jpName;
                int damage = damageList[i][j];

                switch (skill.actionTypeList[i])
                {
                    // スキルタイプによって文章を変える
                    case 0:
                        // ダメージ
                        msgString = receiveCharaName + "に" + damage.ToString() + "のダメージ！";
                        break;

                    case 1:
                        // 回復
                        int heal = -damageList[i][0];
                        msgString = receiveCharaName + "の体力が" + heal.ToString() + "回復した！";
                        break;

                    case 2:
                        // 直前の攻撃依存の回復
                        int heal2 = -damageList[i][0];
                        msgString = receiveCharaName + "の体力が" + heal2.ToString() + "回復した！";
                        break;

                    case 3:
                        // バフ
                        msgString = "バフがかかった！";
                        break;

                    case 4:
                        // 状態異常付与
                        msgString = "状態異常効果！";
                        break;
                }

                tempList.Add(msgString);
            }

            logList.Add(tempList);
        }

        return logList;
    }
    
    public List<Action> GenerateActions(List<List<GameObject>> receiveUnits, List<List<int>> damageList, List<List<string>> logStringList, List<GameObject> effectsList, List<GameObject> actionUnit, SkillStatus skill)
    {
        // actionUnit = スキル使用者（リスト形式）
        Character skillUser = actionUnit[0].GetComponent<Character>();
        List<Action> actionsList = new List<Action>();

        // スキル発動エフェクト
        string msgString0 = skillUser.jpName + skill.message;
        Action preAction = new Action()
        {
            //p = new Performance { unitList = actionUnit, effect = effectsList[0]},
            p = new Performance { unitList = actionUnit, skillUser = skillUser },
            log = new BattleLog { logList = new List<string> { msgString0}, skillUser = skillUser}
        };

        actionsList.Add(preAction);

        List<List<string>> msgList = new List<List<string>>();
        for (int i = 0; i < receiveUnits.Count; i++)
        {
            // スキルを受けたエフェクト
            Action action1 = new Action()
            {
                p = new Performance { unitList = receiveUnits[i], effect = effectsList[i+1], skillUser = skillUser}
            };
            actionsList.Add(action1);

            // ダメージとログ
            Action action2 = new Action()
            {
                d = new Damage { receiveUnitList = receiveUnits[i], damageList = damageList[i], skillUser = skillUser},
                log = new BattleLog { logList = logStringList[i], skillUser = skillUser}
            };
            actionsList.Add(action2);
        }

        return actionsList;
    }

    public void CheckUnitLife()
    {
        // 戦闘不能のユニットをリストから削除
        allUnits.RemoveAll(i => !i.GetComponent<Character>().aliveFlg);
        playerObjList.RemoveAll(i => !i.GetComponent<Character>().aliveFlg);
        enemyObjList.RemoveAll(i => !i.GetComponent<Character>().aliveFlg);
        //Debug.Log("全ユニット " + String.Join(" ", allUnits));
        //Debug.Log("味方 " + String.Join(" ", playerObjList));
        //Debug.Log("敵 " + String.Join(" ", enemyObjList));

        // バトル終了判定
        if (playerObjList.Count == 0)
        {
            // プレイヤーサイドの敗北
            TextController.UpdateLog("戦いに敗れた...");
            Debug.Log("戦いに敗れた…");
        }

        else if (enemyObjList.Count == 0)
        {
            // 敵サイドの敗北
            TextController.UpdateLog("戦いに勝利した");
            Debug.Log("戦いに勝利した");
        }

        else
        {
            // 次のターンへ
            battleMenu.SetMenuNewTurn();
        }
    }

    IEnumerator ActionCoroutine()
    {
        while (battleQueue.Count > 0)
        {
            Action action = battleQueue.Dequeue();

            if (action.log != null && action.log.skillUser.aliveFlg == true)
            {
                action.log.BattleLogMethod();
                yield return new WaitForSeconds(0.2f);
            }
            if (action.p != null && action.p.skillUser.aliveFlg == true)
            {
                action.p.PerformanceMethod();
                yield return new WaitForSeconds(1.0f);
            }
            if (action.d != null && action.d.skillUser.aliveFlg == true) 
            { 
                action.d.DamageMethod();
                yield return new WaitForSeconds(1.0f);
            } 
        }

        battleMenu.ClearLogPanel();
        CheckUnitLife();
    }

    IEnumerator WaitCoroutine()
    {
        //Debug.Log("[!] Start Wait");
        yield return new WaitUntil(() => actionSelectedFlg);
        actionSelectedFlg = false;
        //Debug.Log("[!] End Wait");
    }
    public struct Action
    {
        public Performance p;
        public Damage d;
        public BattleLog log;
    }

    public class Performance
    {
        // エフェクト、アニメーション制御
        //public GameObject character;
        public List<GameObject> unitList;
        public GameObject effect;
        public Character skillUser;

        public void PerformanceMethod()
        {
            if (effect == null)
            {
                foreach (GameObject unit in unitList)
                {
                    // preAction effect
                    
                }
            }

            else
            {
                foreach (GameObject unit in unitList)
                {
                    GameObject tempParticle = Instantiate(effect, unit.transform.position, effect.transform.rotation) as GameObject;
                }
            }           
            
        }

    }

    public class Damage
    {
        // ダメージ制御
        public List<GameObject> receiveUnitList;
        public List<int> damageList;
        public Character skillUser;

        public void DamageMethod()
        {
            for (int i = 0; i < receiveUnitList.Count; i++)
            {
                Character receiveCharacter = receiveUnitList[i].GetComponent<Character>();
                receiveCharacter.hp -= damageList[i];
                receiveCharacter.UpdateStatus();
            }
        }
    }

    public class BattleLog
    {
        // バトルログテキスト制御
        public List<string> logList;
        public Character skillUser;

        public void BattleLogMethod()
        {
            foreach (string logString in logList)
            {                
                TextController.UpdateLog(logString);
            }
        }
    }

    public void ActionSelected()
    {
        actionSelectedFlg = true;
    }
}
