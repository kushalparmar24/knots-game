using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

	public int ysize,xsize;
	public ArrayLayout data;

    private void OnValidate()
    {
      //  data.changesize(size);
    }
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
        fileManager.writeLevel(myarry);
    }

    [ContextMenu("changesize")]
    public void changesize()
    {
        data.changesize(ysize, xsize);
    }
}
