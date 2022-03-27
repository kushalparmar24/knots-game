using System.IO;
using UnityEngine;

public class FileManager
{
    private readonly string levelPath = Application.streamingAssetsPath + "/Levels/";

    /// <summary>
    /// sending unitywebrequest on streaming assets path to get json file
    /// </summary>
    public string ReadLevel(int levelNum)
    {
        var _path = levelPath + levelNum+".json";
        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(_path);
        www.SendWebRequest();
        while (!www.isDone)
        {
            //
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

    public void writeToFile(int lvlnum, string str)
    {
        File.WriteAllText(levelPath + lvlnum + ".json", str);
    }

    //public string readFromFile(int lvlnum)
    //{
    //    if (File.Exists(levelPath + lvlnum + ".json"))
    //    {
    //        return File.ReadAllText(levelPath + lvlnum + ".json");
    //    }
    //    return null;
    //}
}
