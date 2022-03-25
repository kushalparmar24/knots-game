using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    static gameManager instance;
    public static gameManager Instance { get { return instance; } }

    [SerializeField]List<int[,]> gameLevels;
    FileManager fileManager = new FileManager();
    public int currentLevel;

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
        fetchLevels();
    }

    public List<int[,]> getlevelData()
    {
        return gameLevels;
    }

    void fetchLevels()
    {
        bool DataLoaded = true;
        currentLevel = 10;
        gameLevels = new List<int[,]>();
        
        while(DataLoaded)
        {
            string levelData = fileManager.ReadLevel(currentLevel);
            if(levelData.Length <=0 || string.IsNullOrEmpty(levelData))
            {
                DataLoaded = false;
            }
            else
            {
                Debug.Log("levelloading");
                int[,] myarry =  fileManager.getArry(levelData);
                gameLevels.Add(myarry);
                currentLevel++;
            }
        }
        currentLevel = 0;
        //SceneManager.LoadScene("game");
    }

    public void checkNextLevel()
    {
        if (currentLevel < (gameLevels.Count - 1))
            currentLevel++;
        else
            currentLevel = 0;
    }

    public void loadScene()
    {
        SceneManager.LoadScene("game");
    }
}
