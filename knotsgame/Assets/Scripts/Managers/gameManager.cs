using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    static gameManager instance;
    List<ArrayLayout> gameLevels;
    FileManager fileManager = new FileManager();
    public int currentLevel;
    public static gameManager Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
       
    }
    private void Start()
    {
        fetchLevels();
    }
    public List<ArrayLayout> getlevelDatas()
    {
        return gameLevels;
    }

    /// <summary>
    /// loads all the level at start of the the game and keep is stored in the list.
    /// </summary>
    void fetchLevels()
    {
        bool DataLoaded = true;
        currentLevel = 10;
        
        while(DataLoaded)
        {
            string levelData = fileManager.ReadLevel(currentLevel);
            if(string.IsNullOrEmpty(levelData) || levelData.Length <= 0)
            {
                DataLoaded = false;
            }
            else
            {
                Debug.Log("levelloading");
                ArrayLayout myarry = JsonUtility.FromJson<ArrayLayout>(levelData);
                gameLevels.Add(myarry);
                currentLevel++;
            }
        }
        currentLevel = 0;
    }

    /// <summary>
    /// increases the level number and if the last level is reached, then goes back to index 0
    /// </summary>
    public void checkNextLevel()
    {
        if (currentLevel < (gameLevels.Count - 1))
            currentLevel++;
        else
            currentLevel = 0;
    }
}
