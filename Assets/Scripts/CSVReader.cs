using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class CSVReader
{

    public static List<string[]> csvData = new List<string[]>();
    private static string csvLine;

    public static void ReadCSV(string filename)
    {
        // read csv file in Assets/Resources/ and store content in csvData
        TextAsset csvFile = Resources.Load(filename) as TextAsset;
        StringReader reader = new StringReader(csvFile.text);

        while (reader.Peek() != -1)
        {
            // read csv line by line
            // csvData = [ [line 1], [line 2], ...]
            csvLine = reader.ReadLine();

            byte[] bytes = Encoding.Default.GetBytes(csvLine);
            csvLine = Encoding.UTF8.GetString(bytes);
            Debug.Log("[!!!!!] " + csvLine);

            csvData.Add(csvLine.Split(','));
        }

    }
}
