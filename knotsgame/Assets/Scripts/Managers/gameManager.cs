using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    static gameManager instance;
    public const string GAMESCENE = "Game";
    public const string MENUSCENE = "Menu";
    List<ArrayLayout> gameLevels;
    FileManager fileManager = new FileManager();
    int currentLevel;
    public int getCurrentLevel { get { return currentLevel; } }
    public static gameManager Instance { get { return instance; } }
    public string GameScene { get { return GAMESCENE; } }
    public string MenuScene { get { return MENUSCENE; } }
    public List<ArrayLayout> getlevelDatas()
    {
        return gameLevels;
    }
    public void setCurrentLevel(int lvl_)
    {
        currentLevel = lvl_;
    }

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
    
    /// <summary>
    /// loads all the level at start of the the game and keep is stored in the list.
    /// </summary>
    void fetchLevels()
    {
        bool DataLoaded = true;
        gameLevels = new List<ArrayLayout>();
        currentLevel = 0;
        
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
