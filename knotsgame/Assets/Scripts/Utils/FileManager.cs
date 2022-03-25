using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Text;


public class FileManager
{
    //private readonly string levelPath = $"{Application.streamingAssetsPath}/Levels/";
    private readonly string levelPath = Application.streamingAssetsPath + "/Levels/";

    public string ReadLevel(int levelNum)
    {
        var _path = levelPath + levelNum+".json";
        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(_path);
        www.SendWebRequest();
        while (!www.isDone)
        {
        }
        if (www.isNetworkError)
        {
            Debug.Log(www.isNetworkError);
        }
        else
        {
            return www.downloadHandler.text;
        }
        return null;
    }

    public int[,] getArry(string text_)
    {
        int[,] level2D;
        string[] a = text_.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        string[] b = a[0].Split(' ');
        level2D = new int[a.Count(), b.Count()];
        for (int row = 0; row < a.Count(); row++)
        {
            b = a[row].Split(' ');
            for (int column = 0; column < b.Count(); column++)
            {
                level2D[row, column] = Convert.ToInt16(b[column]);
            }
        }
        return level2D;
    }
    //public int[,] ReadLevel(int levelNum)
    //{
    //    //var rows = Resources.Load<TextAsset>("Levels/" + levelNum);
    //    var rows = File.ReadAllLines(levelPath + levelNum+".json"); 
    //    var columns = rows[0].Split(' ');
    //    int[,] level2D = new int[rows.Count(), columns.Count()];

    //    for (int row = 0; row < rows.Count(); row++)
    //    {
    //         columns = rows[row].Split(' ');
    //        for (int column = 0; column < columns.Count(); column++)
    //        {
    //            level2D[row, column] = Convert.ToInt16(columns[column]);
    //        }
    //    }
        
    //    return level2D;
    //}
    public void writeLevel(int[,] myarry)
    {
       // int[,] myarry = new int[5, 5];

        var sb = new StringBuilder(string.Empty);
        var col = myarry.GetLength(1);
        for (int i = 0; i < myarry.GetLength(0); i++)
        {
            for (int j = 0; j < col; j++)
            {
                sb.Append($"{myarry[i, j]}");
                if (j != col - 1)
                    sb.Append(' ');
            }
            if (i != myarry.GetLength(0) - 1)
                sb.AppendLine();
        }

        string result = sb.ToString();
        File.WriteAllText(levelPath + "10.json", result);

    }
}
