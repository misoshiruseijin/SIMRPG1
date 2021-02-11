using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class BattleController : MonoBehaviour
{
    Queue<Action> battleQueue;
    public GameObject playerUnit1; // 味方ユニット
    //public GameObject playerUnit2;
    //public GameObject playerUnit3;
    public GameObject enemyUnit1; // 敵ユニット
    //public GameObject enemyUnit2;
    //public GameObject enemyUnit3;
    public GameObject attackEffect; // 攻撃エフェクト（詠唱）
    public GameObject damageEffect; // 被ダメージエフェクト（爆発）

    private List<string> players = new List<string> { "nezumi" }; // 戦闘に参加する味方ユニット
    private List<string> enemies = new List<string> { "nezumiM" }; // 戦闘に参加する敵ユニット

    private string playerSOpath = "Assets/SO/PlayerUnits/";
    //private string enemySOpath = "/Editor/EnemyUnits/";
    private string imgPath = "Assets/Textures/UnitImages/";

    void Start()
    {
        SetPlayerUnits(players);

        battleQueue = new Queue<Action>();

        Action action1 = new Action()
        {
            p = new Performance { character = playerUnit1, effect = attackEffect },
            log = new BattleLog { logString = "味方の攻撃！" }
        };
        battleQueue.Enqueue(action1);

        Action action2 = new Action()
        {
            p = new Performance { character = enemyUnit1, effect = damageEffect },
            log = new BattleLog { logString = "爆発が敵に襲いかかる！" }
        };
        battleQueue.Enqueue(action2);

        Action action3 = new Action()
        {
            d = new Damage { attackCharacter = playerUnit1, receiveCharacter = enemyUnit1, damage = 30 },
            log = new BattleLog { logString = "敵に30のダメージ！" }
        };
        battleQueue.Enqueue(action3);

    }


    public void SetPlayerUnits(List<string> playersInBattle)
    {
        string name1 = playersInBattle[0];
        Object SO1 = AssetDatabase.LoadAssetAtPath<Object>(playerSOpath + name1 + ".asset");
        Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(imgPath + name1 + ".png");
        Image img = playerUnit1.transform.Find("CharacterImage").GetComponent<Image>();
        img.sprite = sp;        
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
