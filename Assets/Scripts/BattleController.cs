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

    private List<string> players = new List<string> { "nezumi" }; // 戦闘に参加する味方ユニット
    private List<string> enemies = new List<string> { "nezumiM" }; // 戦闘に参加する敵ユニット
    private List<GameObject> unitObjList;

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

        unitObjList = playerObjList.Concat(enemyObjList).ToList(); // 戦闘に参加する全ユニット

        //battleQueue = new Queue<Action>();

        //Action action1 = new Action()
        //{
        //    p = new Performance { character = playerUnit1, effect = attackEffect },
        //    log = new BattleLog { logString = "味方の攻撃！" }
        //};
        //battleQueue.Enqueue(action1);

        //Action action2 = new Action()
        //{
        //    p = new Performance { character = enemyUnit1, effect = damageEffect },
        //    log = new BattleLog { logString = "爆発が敵に襲いかかる！" }
        //};
        //battleQueue.Enqueue(action2);

        //Action action3 = new Action()
        //{
        //    d = new Damage { attackCharacter = playerUnit1, receiveCharacter = enemyUnit1, damage = 30 },
        //    log = new BattleLog { logString = "敵に30のダメージ！" }
        //};
        //battleQueue.Enqueue(action3);
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

    public void GenerateRandomQueue(List<GameObject> allUnits, List<GameObject> playerUnits, List<GameObject> enemyUnits)
    {
        // spdステータスから行動順を決定
        allUnits = allUnits.OrderByDescending(unit => unit.GetComponent<Character>().spd).ToList(); // spdが高い順に並べる

        foreach (GameObject unit in allUnits)
        {
            List<GameObject> receiveUnits = new List<GameObject>();
            GameObject effect1, effect2;
            int damage;
            Character character = unit.GetComponent<Character>();

            // スキルを選ぶ
            List<SkillStatus> skillList = character.skillList;
            SkillStatus skill = skillList[UnityEngine.Random.Range(0, skillList.Count)];

            // エフェクトを取得
            effect1 = skill.effect1;
            effect2 = skill.effect2;

            // 被攻撃ユニットを相手サイドのユニットの中から選ぶ
            if (unit.tag == allyTag)
            {
                receiveUnits.Add(enemyUnits[UnityEngine.Random.Range(0, enemyUnits.Count)]);
            }
            if (unit.tag == enemyTag)
            {
                receiveUnits.Add(playerUnits[UnityEngine.Random.Range(0, playerUnits.Count)]);
            }
            else
            {
                Debug.Log("インスペクターでユニットオブジェクトに設定されたタグを確認してください");
            }

            // ダメージ計算
            damage = (int)Math.Round(character.atk * skill.dmgMult);

            //--->>> ここから！！！！// Queueを作成　
        }
    }

    IEnumerator ActionCoroutine()
    {
        //Debug.Log(battleQueue.Count);
        while (battleQueue.Count > 0)
        {
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
        public GameObject character;
        public GameObject effect;

        public void PerformanceMethod()
        {
            GameObject tempParticle = Instantiate(effect, character.transform.position, effect.transform.rotation) as GameObject;

        }

    }

    public class Damage
    {
        // ダメージ制御
        public GameObject attackCharacter;
        public GameObject receiveCharacter;
        public int damage;

        public void DamageMethod()
        {
            receiveCharacter.GetComponent<Character>().hp -= damage;
            receiveCharacter.GetComponent<Character>().UpdateStatus();
        }
    }

    public class BattleLog
    {
        // バトルログテキスト制御
        public string logString;

        public void BattleLogMethod()
        {
            TextController.UpdateLog(logString);
        }
    }

    public void AttackButtonPressed()
    {
        StartCoroutine("ActionCoroutine");
    }

}
