using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour {

	public int ysize,xsize;//used to change the length of row data of ArrayLayout data.
    public int lvlnum;// read or writes the data on given level number.
    public ArrayLayout data;
   
    /// <summary>
    /// takes the array data filled on editor and sends it to file manager to create json file.
    /// methods can be called from the context menu of inspector editor.
    /// </summary>
    [ContextMenu("writetojson")]
    public void writetojson()
    {
        JsonUtility.ToJson(data);
        FileManager fileManager = new FileManager();
        fileManager.writeToFile(lvlnum, JsonUtility.ToJson(data));
       
    }
    /// <summary>
    /// reads json file based on the level number and passes it to array data that replicate on editor.
    /// methods can be called from the context menu of inspector editor.
    /// </summary>
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
            data = JsonUtility.FromJson<ArrayLayout>(levelData);
        }

    }
    /// <summary>
    /// refreshes the array length on custom editor on call. helps to create the levels.
    /// methods can be called from the context menu of inspector editor.
    /// </summary>
    [ContextMenu("changesize")]
    public void changesize()
    {
        data.changesize(ysize, xsize);
    }
}
