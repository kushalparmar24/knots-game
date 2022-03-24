using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Text;


public class FileManager
{
    private readonly string levelPath = $"{Application.streamingAssetsPath}/Levels/10_10.json";

    public int[,] ReadLevel()
    {
        var rows = File.ReadAllLines(levelPath); // TODO: Choose data path dynamic or preload them
        var columns = rows[0].Split(' ');
        int[,] level2D = new int[rows.Count(), columns.Count()];

        for (int row = 0; row < rows.Count(); row++)
        {
             columns = rows[row].Split(' ');
            for (int column = 0; column < columns.Count(); column++)
            {
                level2D[row, column] = Convert.ToInt16(columns[column]);
            }
        }
        
        return level2D;
    }
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
            sb.AppendLine();
        }

        string result = sb.ToString();
        File.WriteAllText(levelPath, result);

    }
}
