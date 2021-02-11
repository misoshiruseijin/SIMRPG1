using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleInitialize
{
    ///<summary>
    ///戦闘に参加するユニットの情報を受け取る。
    ///CSVReaderから受け取ったデータから必要なデータだけをまとめてBattleControllerに渡す
    ///</summary>

    public static List<string> playerData = new List<string>();
    public static List<string> enemyData = new List<string>();

    public static void SetPlayerUnits(List<string> playersInBattle)
    {
        int n_players = playersInBattle.Count;
        for (int i = 0; i < n_players; i++)
        {

        }
    }

    public static void GetEnemyData(List<string> enemiesInBattle)
    {

    }

}
