using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class GameController: MonoBehaviour
{
    // シーン間で保持されるデータを所有
    public static GameController instance;

    // キャラデータ
    public CharacterData[] allyDataArray; // 味方データを保存するArray
    public List<CharacterData> partyMemberData; // 戦闘に参加するユニットの情報

    public GeneData[] geneDataArray; // 所持している遺伝子アイテムのリスト

    public int phase; // ゲームの進行状況
    public float messaegSpeed; // 文字表示速度


    #region デフォルトの値
    private float def_messageSpeed = 0.05f; // 文字表示速度
    #endregion

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


        // データ初期化。本来はセーブデータをロードする
        allyDataArray = new CharacterData[10]; // 10は味方の最大数
        geneDataArray = new GeneData[10];
        phase = 0;
        messaegSpeed = def_messageSpeed;
    }

}
