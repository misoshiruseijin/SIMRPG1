using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyPartyData
{
    public static List<string> party1 = new List<string> { "nezumiM", "kaM"};

    public static List<List<string>> partiesList = new List<List<string>> { party1 };

    public static List<string> GetEnemyNameList()
    {
        List<string> names = partiesList[GameController.instance.phase];
        
        return names;
    }
}
