using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class levelElement : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI levelText;
    [SerializeField] Button lvlBtn;
    int levelNumer;

    private void Awake()
    {
        lvlBtn.onClick.AddListener(() => onLevelClick());
    }

    public void setUI(int lvlnum_)
    {
        levelNumer = lvlnum_;
        levelText.text = (levelNumer + 1).ToString();
    }

    void onLevelClick()
    {
        gameManager.Instance.setCurrentLevel(levelNumer);
        UISceneController.Instance.ShowUIScreen("game", ScreenManager.UIScreens.INGAMEHUD);
    }
   
}
