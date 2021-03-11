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
    Queue<Action> battleQueue;
    public List<GameObject> playerObjList; // 味方ユニット（最大数3）
    public List<GameObject> enemyObjList; // 敵ユニット（最大数3）
    public GameObject attackEffect; // 攻撃エフェクト（詠唱）
    public GameObject damageEffect; // 被ダメージエフェクト（爆発）

    private List<string> players = new List<string> { "nezumi", "nezumi" }; // 戦闘に参加する味方ユニット
    private List<string> enemies = new List<string> { "nezumiM" }; // 戦闘に参加する敵ユニット
    private List<GameObject> unitObjList = new List<GameObject>(); // 戦闘に参加する全ユニットオブジェクト

    private string playerSOpath = "Assets/SO/PlayerUnits/";
    private string enemySOpath = "Assets/SO/EnemyUnits/";
    private string skillSOpath = "Assets/SO/Skills/";
    private string imgPath = "Assets/Textures/UnitImages/";

    private string allyTag = "Ally";
    private string enemyTag = "Enemy";

    void Start()
    {
        InitializeBattleUnits(players, playerObjList, "Player");
        InitializeBattleUnits(enemies, enemyObjList, "Enemy");

        // initialize unit object lists to match number of units in battle
        playerObjList = playerObjList.Take(players.Count).ToList();
        enemyObjList = enemyObjList.Take(enemies.Count).ToList();
        for (int i = 0; i < players.Count; i++)
        {
            unitObjList.Add(playerObjList[i]);
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            unitObjList.Add(enemyObjList[i]);
        }

        //Debug.Log($"ユニット数は{unitObjList.Count}");

        //StartBattleTurn(unitObjList, playerObjList, enemyObjList);
    }


    public void InitializeBattleUnits(List<string> unitsInBattle, List<GameObject> unitObjList, string unitType)
    {
        int n_units = unitsInBattle.Count;
        int max_units = unitObjList.Count;

        if (n_units > max_units)
        {
            Debug.Log("バトルに参加できるのは" + max_units.ToString() + "体まで");
        }

        // 全ユニットを非表示
        foreach (GameObject unitObj in unitObjList)
        {
            unitObj.SetActive(false);
        }
        
        for (int i = 0; i < n_units; i++)
        {
            Character character = unitObjList[i].GetComponent<Character>();
            string unitName = unitsInBattle[i];

            // ScriptableObjectを読み込み、対応するGameObjectにHPと名前を設定
            if (unitType.Equals("Player"))
            {
                // プレイヤーユニットの場合
                PlayerStatus SO = AssetDatabase.LoadAssetAtPath<PlayerStatus>(playerSOpath + unitName + ".asset");
                character.Maxhp = SO.hp;
                character.jpName = SO.jpName;
                character.atk = SO.atk;
                character.def = SO.def;
                character.spd = SO.spd;
                character.SetStatus();

                // スキルを設定
                string stringID;
                foreach(int skillID in SO.skills)
                {
                    // foreach skill a character has
                    if (skillID < 10)
                    {
                        stringID = "0" + skillID.ToString();
                    }
                    else
                    {
                        stringID = skillID.ToString();
                    }

                    // skillIDに対応したスキルアッセットをCharacterに設定
                    IEnumerable<string> assetfiles = Directory.GetFiles(skillSOpath, "*.asset").Where(name => name.Contains(stringID));
                    foreach(string ast in assetfiles)
                    {
                        SkillStatus skillStatus = AssetDatabase.LoadAssetAtPath<SkillStatus>(ast);
                        character.skillList.Add(skillStatus);
                    }
                }                
            }
            
            if (unitType.Equals("Enemy"))
            {
                // 敵ユニットの場合
                EnemyStatus SO = AssetDatabase.LoadAssetAtPath<EnemyStatus>(enemySOpath + unitName + ".asset");
                character.Maxhp = SO.hp;
                character.jpName = SO.jpName;
                character.atk = SO.atk;
                character.def = SO.def;
                character.spd = SO.spd;
                character.SetStatus();

                // スキルを設定
                string stringID;
                foreach (int skillID in SO.skills)
                {
                    // foreach skill a character has
                    if (skillID < 10)
                    {
                        stringID = "0" + skillID.ToString();
                    }
                    else
                    {
                        stringID = skillID.ToString();
                    }

                    // skillIDに対応したスキルアッセットをCharacterに設定
                    IEnumerable<string> assetfiles = Directory.GetFiles(skillSOpath, "*.asset").Where(name => name.Contains(stringID));
                    foreach (string ast in assetfiles)
                    {
                        SkillStatus skillStatus = AssetDatabase.LoadAssetAtPath<SkillStatus>(ast);
                        character.skillList.Add(skillStatus);
                    }
                }
            }

            Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(imgPath + unitName + ".png"); // キャラ画像を取得
            unitObjList[i].transform.Find("CharacterImage").GetComponent<Image>().sprite = sp; // キャラ画像を設定
            unitObjList[i].SetActive(true); // 画像が設定されたら非表示を解除                  
        }
    }

    public void StartBattleTurn(List<GameObject> allUnits, List<GameObject> playerUnits, List<GameObject> enemyUnits)
    {
        // spdステータスから行動順を決定
        allUnits = allUnits.OrderByDescending(unit => unit.GetComponent<Character>().spd).ToList(); // spdが高い順に並べる
        //Debug.Log("行動順は" + String.Join(" ", allUnits));
        
        // バトルキューを作成
        battleQueue = new Queue<Action>();

        foreach (GameObject unit in allUnits)
        {
            List<List<GameObject>> receiveUnits = new List<List<GameObject>>(); // 行動を受けるユニットのリスト
            List<GameObject> actionUnit = new List<GameObject> { unit }; // 行動するユニット
            List<List<int>> damages = new List<List<int>>(); 
            List<List<string>> logStringList = new List<List<string>>();
            Character character = unit.GetComponent<Character>();
            string unitTag = unit.tag;

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
    }


    public List<GameObject> ChooseTargetUnits(int targetCode, GameObject unit)
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
                // 敵単体が対象。敵ユニットの中からランダムに1体選択
                targetUnits.Add(enemyTargets[UnityEngine.Random.Range(0, enemyTargets.Count)]);
                break;

            case 1:
                // 敵全体が対象
                targetUnits = enemyTargets;
                break;

            case 2:
                // 味方単体が対象。味方ユニットの中からランダムに1体選択
                targetUnits.Add(allyTargets[UnityEngine.Random.Range(0, allyTargets.Count)]);
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
                    }
                    break;

                case 1:
                    // 回復効果
                    foreach(GameObject targetUnit in receiveUnits[i])
                    {
                        Character targetCharacter = targetUnit.GetComponent<Character>();
                        int damage = Mathf.RoundToInt((float)(-atk * multList[i] / 10));
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
                        // バフ
                        msgString = "バフがかかった！";
                        break;

                    case 3:
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
            p = new Performance { unitList = actionUnit },
            log = new BattleLog { logList = new List<string> { msgString0} }
        };

        actionsList.Add(preAction);

        List<List<string>> msgList = new List<List<string>>();
        for (int i = 0; i < receiveUnits.Count; i++)
        {
            // スキルを受けたエフェクト
            Action action1 = new Action()
            {
                p = new Performance { unitList = receiveUnits[i], effect = effectsList[i+1]}
            };
            actionsList.Add(action1);

            // ダメージとログ
            Action action2 = new Action()
            {
                d = new Damage { receiveUnitList = receiveUnits[i], damageList = damageList[i]},
                log = new BattleLog { logList = logStringList[i]}
            };
            actionsList.Add(action2);
        }

        return actionsList;
    }

    IEnumerator ActionCoroutine()
    {
        while (battleQueue.Count > 0)
        {
            //Debug.Log(battleQueue.Count);
            Action action = battleQueue.Dequeue();
            //Debug.Log(action);
            if (action.log != null)
            {
                action.log.BattleLogMethod();
                yield return new WaitForSeconds(0.2f);
            }
            if (action.p != null)
            {
                action.p.PerformanceMethod();
                yield return new WaitForSeconds(1.0f);
            }
            if (action.d != null) 
            { 
                action.d.DamageMethod();
                yield return new WaitForSeconds(1.0f);
            }
            
        }
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
        //public string logString;
        public List<string> logList;

        public void BattleLogMethod()
        {
            //TextController.UpdateLog(logString);
            foreach (string logString in logList)
            {                
                TextController.UpdateLog(logString);
            }
        }
    }

    public void AttackButtonPressed()
    {
        StartBattleTurn(unitObjList, playerObjList, enemyObjList);
        StartCoroutine("ActionCoroutine");
    }

}
