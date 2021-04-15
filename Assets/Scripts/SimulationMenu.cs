using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SimulationMenu : MonoBehaviour
{
    public List<GameObject> btnList;

    private EventSystem eventSystem;
    private GameObject button, prevButton;
    private ButtonResponseSize btnResp;
    private int btnID, prevBtnID;

    public void ButtonPressed()
    {
        eventSystem = EventSystem.current;
        prevButton = button;
        button = eventSystem.currentSelectedGameObject;
        btnResp = button.GetComponent<ButtonResponseSize>();

        prevBtnID = btnID;
        btnID = btnList.IndexOf(button);

        bool btnChanged = btnID != prevBtnID; // 前と違うボタンを押した
        bool noPrevBtn = prevButton == null; // 前に押したボタンがない (今回初めてボタンを押した)

        //Debug.Log(button);

        // 同じパネル内で前と違うボタンを押した
        if (!noPrevBtn && btnChanged)
        {
            //Debug.Log(noPrevBtn + ", " + btnChanged);
            prevButton.GetComponent<ButtonResponseSize>().btnReady = false;
        }

        if (!btnResp.btnReady)
        {
            //Debug.Log("初めてクリックされた");
            btnResp.btnReady = true;
        }

        else if (btnResp.btnReady)
        {
            //Debug.Log("二回目のクリック");
            btnResp.btnReady = false;
        }
    }

    private void TakeButtonAction(int btnID)
    {
        switch (btnID)
        {
            case 0:
                //Debug.Log("仲間管理ボタン");
                break;

            case 1:
                //Debug.Log("会話ボタン");
                break;

            case 2:
                //Debug.Log("マップボタン");
                break;

            case 3:
                //Debug.Log("設定ボタン");
                break;

            case 4:
                //Debug.Log("戦闘開始ボタン");
                break;
        }
    }
}
