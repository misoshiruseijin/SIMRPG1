using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class BattleController : MonoBehaviour
{
    Queue<Action> battleQueue;
    public List<GameObject> playerObjList; // 味方ユニット（最大数3）
    public List<GameObject> enemyObjList; // 敵ユニット（最大数3）
    //public GameObject playerUnit1; // 味方ユニット
    //public GameObject playerUnit2;
    //public GameObject playerUnit3;
    //public GameObject enemyUnit1; // 敵ユニット
    //public GameObject enemyUnit2;
    //public GameObject enemyUnit3;
    public GameObject attackEffect; // 攻撃エフェクト（詠唱）
    public GameObject damageEffect; // 被ダメージエフェクト（爆発）

    private List<string> players = new List<string> { "nezumi", "nezumi" }; // 戦闘に参加する味方ユニット
    private List<string> enemies = new List<string> { "nezumiM" }; // 戦闘に参加する敵ユニット

    private string playerSOpath = "Assets/SO/PlayerUnits/";
    private string enemySOpath = "Assets/SO/EnemyUnits/";
    private string imgPath = "Assets/Textures/UnitImages/";

    void Start()
    {
        InitializeBattleUnits(players, playerObjList, "Player");
        InitializeBattleUnits(enemies, enemyObjList, "Enemy");

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
            
            // ScriptableObjectを読み込む
            if (unitType.Equals("Player"))
            {
                PlayerStatus SO = AssetDatabase.LoadAssetAtPath<PlayerStatus>(playerSOpath + unitName + ".asset");
                character.Maxhp = SO.hp;
                character.jpName = SO.jpName;
                character.SetStatus();

            }
            
            if (unitType.Equals("Enemy"))
            {
                EnemyStatus SO = AssetDatabase.LoadAssetAtPath<EnemyStatus>(enemySOpath + unitName + ".asset");
                character.Maxhp = SO.hp;
                character.jpName = SO.jpName;
                character.SetStatus();
            }

            Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(imgPath + unitName + ".png"); // キャラ画像を取得
            unitObjList[i].transform.Find("CharacterImage").GetComponent<Image>().sprite = sp; // キャラ画像を設定
            unitObjList[i].SetActive(true);                      
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
