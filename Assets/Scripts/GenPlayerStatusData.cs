using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class GenPlayerStatusData
{
   [MenuItem("Create/PlayerStatusData")]
   private static void Create()
    {
        // Instantiate ScriptableObject
        PlayerStatusData playerStatusData = ScriptableObject.CreateInstance<PlayerStatusData>();

        // Set Data
        PlayerStatus status1 = new PlayerStatus();
        status1.name = "リオレウス";

        playerStatusData.PlayerStatusList.Add(status1);

        // Write file to Assets
        AssetDatabase.CreateAsset(playerStatusData, "Assets/Resources/PlayerStatusData.asset");
    }
}
