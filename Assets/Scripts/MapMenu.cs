using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapMenu : MonoBehaviour
{
    public GameObject valley, hills, desert, lake, forest, swamp,
        island, grassland, mountain, river, shore, glacier;
    public GameObject mapBG; // ベースとなるモノクロ地図オブジェクト

    public GameObject[] areaObjArray; // 各エリア画像のリスト
    private Image flashImg; // 点滅させるエリア

    private float time, maxAlpha;
    private bool isFlash; // 点滅させるか

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("START");
        areaObjArray = new GameObject[] { island, desert, hills, forest, swamp, 
            lake, valley, grassland, river, mountain, shore, glacier};

        time = 0f;
        maxAlpha = 1f;
        isFlash = false;
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
        Debug.Log("SHOW");
        // 進行度に合わせたエリア画像を表示
        int phase = GameController.instance.phase;
        for (int i = 0; i <= phase; i++)
        {
            GameObject areaObj = Instantiate(areaObjArray[i], mapBG.transform);

            if (i == phase)
            {
                flashImg = areaObj.GetComponent<Image>();
            }
        }

        isFlash = true;
    }

    private Color GetAlpha(Color color)
    {
        time += Time.deltaTime;
        color.a = (maxAlpha * 0.5f) * Mathf.Sin(3f * time) + (maxAlpha * 0.5f);

        return color;
    }
}
