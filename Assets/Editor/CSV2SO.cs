using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class CSV2SO : MonoBehaviour
{
    public static List<string> csvData = new List<string>();
    private static string csvLine;

    private static string PlayerCSVPath = "/Editor/CSVs/PlayerUnit.csv";
    
    [MenuItem("Utilities/Generate PlayerData")]
    public static void GeneratePlayerData()
    {
        //TextAsset csvFile = Resources.Load(filename) as TextAsset;
        //StringReader reader = new StringReader(csvFile.text);
        StreamReader reader = new StreamReader(Application.dataPath + PlayerCSVPath);

        // skip header
        reader.ReadLine();

        while (reader.Peek() != -1)
        {
            // csvData = list containing arrays "csvLine"
            csvLine = reader.ReadLine();

            byte[] bytes = Encoding.Default.GetBytes(csvLine);
            csvLine = Encoding.UTF8.GetString(bytes);

            csvData.Add(csvLine);
        }

        foreach (string csvLine in csvData)
        {
            string[] splitData = csvLine.Split(',');
            PlayerStatus p = ScriptableObject.CreateInstance<PlayerStatus>();
            p.id = splitData[0];
            p.engName = splitData[1];
            p.jpName = splitData[2];
            p.hp = int.Parse(splitData[3]);
            p.atk = int.Parse(splitData[4]);
            p.def = int.Parse(splitData[5]);
            p.spd = int.Parse(splitData[6]);
            p.skill1 = splitData[7];
            p.skill2 = splitData[8];
            p.skill3 = splitData[9];
            p.skill4 = splitData[10];

            AssetDatabase.CreateAsset(p, $"Assets/Editor/PlayerUnits/{p.engName}.asset");
        }

        AssetDatabase.SaveAssets();
    }
}
