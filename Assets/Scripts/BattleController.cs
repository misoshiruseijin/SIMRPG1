using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    public Character player;
    public Character enemy;
    public GameObject ResultPanel;

    private bool IsPlayerTurn;
    private bool IsGameOver;
    private float time = 0f;

    // Start is called before the first frame update
    void Start()
    {
        IsPlayerTurn = true;
        IsGameOver = false;
        ResultPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGameOver)
        {
            ShowResultPanel();
            return;
        }

        if (!IsPlayerTurn)
        {
            time += Time.deltaTime;
            if(time >= 1f)
            {
                player.ReceiveDamage(enemy.atk);
                IsPlayerTurn = true;
                time = 0f;
            }
        }

        if (player.hp == 0 || enemy.hp == 0)
        {
            IsGameOver = true;
        }
    }

    public void PushAttackButton()
    {
        enemy.ReceiveDamage(player.atk);
        IsPlayerTurn = false;
    }

    void ShowResultPanel()
    {
        ResultPanel.SetActive(true);
    }

}
