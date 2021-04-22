using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class GameController: MonoBehaviour
{
    // シーン間で保持されるデータを所有
    public static GameController instance;
    public CharacterData[] allyDataArray; // 味方データを保存するArray


    private void Awake()
    {
        // GameControllerが必ず1つ存在するようにする
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }

        else if (instance != this)
        {
            Destroy(gameObject);
        }

        allyDataArray = new CharacterData[10]; // 10は味方の最大数

    }

    public void testmethod()
    {
        //Debug.Log("Name: " + ally1.jpName);
        foreach (CharacterData cd in allyDataArray)
        {
            Debug.Log(cd.jpName);
        }
    }
}
