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
    public List<GeneData> geneDataList; // 所持している遺伝子アイテムのリスト

    public int daysFromEvolve; // 変異開始から経過した日数
    public int evolveDays; // 変異完了までに必要な日数
    public int evolvingUnitID; // 変異中のキャラID
    public int usedGeneID; // 変異中のキャラに使用された遺伝子ID
    public bool isEvolving; // 変異中のキャラがいるか
    public int phase; // ゲームの進行状況
    public float messaegSpeed; // 文字表示速度
    public int day; // 開始からの日数
    public int survivors; // 人間の数
    public int food; // 食料の残量


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
        geneDataList = new List<GeneData>();
        phase = 0;
        messaegSpeed = def_messageSpeed;
        day = 1;
        survivors = 10;
        food = 50;
    }

}
