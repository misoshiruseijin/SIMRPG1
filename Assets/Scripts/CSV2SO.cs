using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

public class CSV2SO : MonoBehaviour
{
    public static List<string> csvData = new List<string>();
    private static string csvLine;

    private static string PlayerCSVPath = "/SO/CSVs/PlayerUnit.csv";
    private static string EnemyCSVPath = "/SO/CSVs/EnemyUnit.csv";
    private static string SkillsCSVPath = "/SO/CSVs/Skills.csv";

    private static string imgPath = "Assets/Textures/UnitImages/"; // ADDED
    private static string skillSOpath = "Assets/SO/Skills/"; // ADDED


    [MenuItem("Utilities/Generate AllData")]
    public static void GenerateAllData()
    {
        GenerateSkillData();
        GeneratePlayerData();
        GenerateEnemyData();
    }

    [MenuItem("Utilities/Generate PlayerData")]
    public static void GeneratePlayerData()
    {
        csvData.Clear();
        csvLine = string.Empty;
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
            string unitName;
            
            // 名前とステータスを獲得
            p.id = splitData[0];
            unitName = splitData[1];
            p.engName = unitName;
            p.jpName = splitData[2];
            p.hp = int.Parse(splitData[3]);
            p.atk = int.Parse(splitData[4]);
            p.def = int.Parse(splitData[5]);
            p.spd = int.Parse(splitData[6]);

            // スキルIDを獲得
            List<string> skillData = new List<string> { splitData[7], splitData[8], splitData[9], splitData[10] };
            p.skillList = new List<SkillStatus>();

            foreach (string s in skillData)
            {
                string stringID;

                if (String.IsNullOrEmpty(s))
                {
                    break;
                }

                else
                {
                    stringID = s.Substring(s.IndexOf("_") + 1);
                }

                // skillIDに対応したスキルアッセットをCharacterに設定
                IEnumerable<string> assetfiles = Directory.GetFiles(skillSOpath, "*.asset").Where(name => name.Contains(stringID));
                foreach (string ast in assetfiles)
                {
                    SkillStatus skillStatus = AssetDatabase.LoadAssetAtPath<SkillStatus>(ast);
                    Debug.Log(skillStatus);
                    p.skillList.Add(skillStatus);
                }
            }

            // スプライトを設定
            Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(imgPath + unitName + ".png"); // キャラ画像を取得
            p.unitImg = sp;

            AssetDatabase.CreateAsset(p, $"Assets/SO/PlayerUnits/{p.engName}.asset");
        }

        AssetDatabase.SaveAssets();
    }


    [MenuItem("Utilities/Generate EnemyData")]
    public static void GenerateEnemyData()
    {
        csvData.Clear();
        csvLine = string.Empty;
        StreamReader reader = new StreamReader(Application.dataPath + EnemyCSVPath);

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
            EnemyStatus e = ScriptableObject.CreateInstance<EnemyStatus>();
            string unitName;

            // 名前とステータスを獲得
            e.id = splitData[0];
            unitName = splitData[1];
            e.engName = unitName;
            e.jpName = splitData[2];
            e.hp = int.Parse(splitData[3]);
            e.atk = int.Parse(splitData[4]);
            e.def = int.Parse(splitData[5]);
            e.spd = int.Parse(splitData[6]);

            // スキルIDを獲得
            List<string> skillData = new List<string> { splitData[7], splitData[8], splitData[9], splitData[10] };
            e.skillList = new List<SkillStatus>();

            foreach (string s in skillData)
            {
                string stringID;

                if (String.IsNullOrEmpty(s))
                {
                    break;
                }

                else
                {
                    stringID = s.Substring(s.IndexOf("_") + 1);
                }

                // skillIDに対応したスキルアッセットをCharacterに設定
                IEnumerable<string> assetfiles = Directory.GetFiles(skillSOpath, "*.asset").Where(name => name.Contains(stringID));
                foreach (string ast in assetfiles)
                {
                    SkillStatus skillStatus = AssetDatabase.LoadAssetAtPath<SkillStatus>(ast);
                    Debug.Log(skillStatus);
                    e.skillList.Add(skillStatus);
                }
            }

            // スプライトを設定
            Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(imgPath + unitName + ".png"); // キャラ画像を取得
            e.unitImg = sp;
            
            AssetDatabase.CreateAsset(e, $"Assets/SO/EnemyUnits/{e.engName}.asset");
        }

        AssetDatabase.SaveAssets();
    }


    [MenuItem("Utilities/Generate SkillData")]
    public static void GenerateSkillData()
    {
        csvData.Clear();
        csvLine = string.Empty;
        StreamReader reader = new StreamReader(Application.dataPath + SkillsCSVPath);

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
            SkillStatus s = ScriptableObject.CreateInstance<SkillStatus>();

            // get integer ver of ID
            string id = splitData[0];
            if (id[id.IndexOf("_") + 1].ToString().Equals("0"))
            {
                s.id = int.Parse(id[id.Length - 1].ToString());
            }

            else
            {
                s.id = int.Parse(id.Substring(id.IndexOf("_")));
            }

            // get name
            s.engName = splitData[1];
            s.jpName = splitData[2];

            // get skill content
            s.targetList = new List<int>();
            s.actionTypeList = new List<int>();
            s.multList = new List<int>();
            for (int i = 1; i < 4; i++)
            {
                if (!String.IsNullOrEmpty(splitData[3 * i]))
                {
                    s.targetList.Add(int.Parse(splitData[3 * i]));
                    s.actionTypeList.Add(int.Parse(splitData[3 * i + 1]));
                    s.multList.Add(int.Parse(splitData[3 * i + 2]));
                }
                else
                {
                    break;
                }
            }

            // get skill effects
            s.effectList = new List<GameObject>();
            for (int i = 0; i < 4; i++)
            {
                if (!String.IsNullOrEmpty(splitData[i+12]))
                {
                    GameObject effectPrefab = Resources.Load<GameObject>("Effects/" + splitData[i + 12]);
                    s.effectList.Add(effectPrefab);
                }
                else
                {
                    break;
                }
            }
            //GameObject effectPrefab1 = Resources.Load<GameObject>("Effects/" + splitData[12]);
            //s.effect1 = effectPrefab1;
            //GameObject effectPrefab2 = Resources.Load<GameObject>("Effects/" + splitData[13]);
            //s.effect2 = effectPrefab2;

            // get battle message and skill description
            s.message = splitData[16];
            s.desc = splitData[17];

            AssetDatabase.CreateAsset(s, $"Assets/SO/Skills/{s.engName}.asset");
        }

        AssetDatabase.SaveAssets();
    }
    
    
    [MenuItem("Utilities/Generate PlayerData2")]
    public static void GeneratePlayerData2()
    {
        csvData.Clear();
        csvLine = string.Empty;
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
            string unitName;
            // 名前とステータスを獲得
            p.id = splitData[0];
            unitName = splitData[1];
            p.engName = unitName;
            p.jpName = splitData[2];
            p.hp = int.Parse(splitData[3]);
            p.atk = int.Parse(splitData[4]);
            p.def = int.Parse(splitData[5]);
            p.spd = int.Parse(splitData[6]);

            // スキルIDを獲得
            List<string> skillData = new List<string> { splitData[7], splitData[8], splitData[9], splitData[10] };
            p.skillList = new List<SkillStatus>();

            foreach (string s in skillData)
            {
                string stringID;

                if (String.IsNullOrEmpty(s))
                {
                    break;
                }

                else
                {
                    stringID = s.Substring(s.IndexOf("_") + 1);
                }

                // skillIDに対応したスキルアッセットをCharacterに設定
                IEnumerable<string> assetfiles = Directory.GetFiles(skillSOpath, "*.asset").Where(name => name.Contains(stringID));
                foreach (string ast in assetfiles)
                {
                    SkillStatus skillStatus = AssetDatabase.LoadAssetAtPath<SkillStatus>(ast);
                    Debug.Log(skillStatus);
                    p.skillList.Add(skillStatus);
                }
            }

            // スプライトを設定
            Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(imgPath + unitName + ".png"); // キャラ画像を取得
            p.unitImg = sp;

            AssetDatabase.CreateAsset(p, $"Assets/SO/PlayerUnits/{p.engName}.asset");
        }

        AssetDatabase.SaveAssets();
    }
}
