using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

	public int ysize,xsize;
    public int lvlnum;
    public ArrayLayout data;
    

    [ContextMenu("writetojson")]
    public void writetojson()
    {
        int[,] myarry = new int[xsize, ysize];
        for (var i = 0; i < xsize; i++)
        {
            for (var j = 0; j < ysize; j++)
            {
                myarry[i, j] = data.rows[i].row[j];
            }
        }
        FileManager fileManager = new FileManager();
        fileManager.writeLevel(myarry, lvlnum);
    }

    [ContextMenu("getjson")]
    public void readJson()
    {
        FileManager fm = new FileManager();
        string levelData = fm.ReadLevel(lvlnum);
        if (levelData.Length <= 0 || string.IsNullOrEmpty(levelData))
        {
            return;
        }
        else
        {
            fillArry(fm.getArry(levelData));
        }

    }
    public void fillArry(int[,] arry_)
    {
        xsize = arry_.GetLength(0);
        ysize = arry_.GetLength(1);
        data.rows = new ArrayLayout.rowData[xsize];
        for (var i = 0; i < data.rows.Length; i++)
        {
            data.rows[i].row = new int[ysize];
            for (var j = 0; j < data.rows[i].row.Length; j++)
            {
                data.rows[i].row[j] = arry_[i,j];
            }
        }
        
    }

    [ContextMenu("changesize")]
    public void changesize()
    {
        data.changesize(ysize, xsize);
    }
}
