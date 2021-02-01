using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    Queue<Action> battleQueue;
    public GameObject attackUnit; // 攻撃を行うユニット
    public GameObject receiveUnit; // 攻撃されるユニット
    public GameObject attackEffect; // 攻撃エフェクト（詠唱）
    public GameObject damageEffect; // 被ダメージエフェクト（爆発）

    // Start is called before the first frame update
    void Start()
    {
        CSVReader.ReadCSV("TestCSV");

        battleQueue = new Queue<Action>();

        Action action1 = new Action()
        {
            p = new Performance { character = attackUnit, effect = attackEffect },
            log = new BattleLog { logString = "味方の攻撃！" }
        };
        battleQueue.Enqueue(action1);

        Action action2 = new Action()
        {
            p = new Performance { character = receiveUnit, effect = damageEffect },
            log = new BattleLog { logString = "爆発が敵に襲いかかる！"}
        };
        battleQueue.Enqueue(action2);

        Action action3 = new Action()
        {
            d = new Damage { attackCharacter = attackUnit, receiveCharacter = receiveUnit, damage = 30 },
            log = new BattleLog { logString = "敵に30のダメージ！"}
        };
        battleQueue.Enqueue(action3);

    }

    IEnumerator ActionCoroutine()
    {
        //Debug.Log(battleQueue.Count);
        while (battleQueue.Count > 0)
        {
            Action action = battleQueue.Dequeue();
            Debug.Log(action);
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
