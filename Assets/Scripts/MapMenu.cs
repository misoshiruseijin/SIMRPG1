using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapMenu : MonoBehaviour
{
    #region インスペクターで設定
    public GameObject valley, hills, desert, lake, forest, swamp,
        island, grassland, mountain, river, shore, glacier;
    public GameObject mapParent; // 画像配置の基準になるパネル
    public Text descriptionText; // 次のエリアの解説を表示するテキストオブジェクト
    #endregion


    private GameObject[] areaObjArray; // 各エリア画像のリスト
    private Image flashImg; // 点滅させるエリア
    private string[] descStringArray; // 各エリアの解説文
    private float time, maxAlpha;
    private float flashSpeed;
    private bool isFlash; // 点滅させるか
    private Vector3 positionVector;

    // Start is called before the first frame update
    void Start()
    {
        areaObjArray = new GameObject[] { island, desert, hills, forest, swamp, 
            lake, valley, grassland, river, mountain, shore, glacier};

        time = 0f;
        maxAlpha = 1f;
        flashSpeed = 3f;
        isFlash = false;

        positionVector = mapParent.transform.position;
        descStringArray = new string[] 
        { 
            "島\n地下シェルターが位置する小さな島。変異種の被害はまだ比較的少ない。",
            "砂漠\n変異種たちによって植物が食い尽くされ、乾いた土地が広がっている。変異した草食動物との遭遇が予想される。",
            "丘\n",
            "森林\n",
            "沼地\n",
            "湖\n",
            "崖\n",
            "草原\n",
            "川\n",
            "山岳\n",
            "浜辺\n",
            "氷河\n比較的温暖なこの地域にこ本来存在しないはずの氷の島。周囲の気候を変えるほどの強力な力を得た変異種が生まれたというのか。",
        };
    }

    private void Update()
    {
        if (isFlash)
        {
            flashImg.color = GetAlpha(flashImg.color);
        }
    }

    public void ShowMap()
    {
        // 進行度に合わせたエリア画像を表示
        foreach (Transform child in mapParent.transform)
        {
            Destroy(child.gameObject);
        }

        int phase = GameController.instance.phase;
        for (int i = 0; i <= phase; i++)
        {
            GameObject areaObj = Instantiate(areaObjArray[i], positionVector, Quaternion.identity, mapParent.transform);

            if (i == phase)
            {
                flashImg = areaObj.GetComponent<Image>();
            }
        }

        descriptionText.text = $"次の目的地：{descStringArray[phase]}";
        isFlash = true;
    }

    private Color GetAlpha(Color color)
    {
        time += Time.deltaTime;
        color.a = (maxAlpha * 0.5f) * Mathf.Sin(flashSpeed * time) + (maxAlpha * 0.5f);

        return color;
    }
}
