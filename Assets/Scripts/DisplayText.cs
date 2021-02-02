using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Text>().text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (TextController.updateFlg == true)
        {
            string logText = TextController.UpdateLog2();
            this.GetComponent<Text>().text = logText;
        }

        if (CSVReader.returnLineFlg == true)
        {
            string csvText = CSVReader.ReturnLine();
            this.GetComponent<Text>().text = csvText;
        }
    }
}
