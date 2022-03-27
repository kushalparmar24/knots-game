using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPauseScreen : UIBase
{
    [SerializeField] Button levelBtn,menuBtn, backBtn;

    private void Awake()
    {
        levelBtn.onClick.AddListener(() => OnLevelButtonClick());
        menuBtn.onClick.AddListener(() => OnMenuButtonClick());
        backBtn.onClick.AddListener(() => OnBackButtonClick());
    }

    void OnLevelButtonClick()
    {
        levelManager.Instance.resetLevel();
        UISceneController.Instance.ShowUIScreen("Menu",ScreenManager.UIScreens.LEVELSELECTION);
    }
    void OnMenuButtonClick()
    {
        levelManager.Instance.resetLevel();
        UISceneController.Instance.ShowUIScreen("Menu",ScreenManager.UIScreens.MENU);
    }
    void OnBackButtonClick()
    {
        GoBack();
        levelManager.Instance.gameState = levelManager.GameState.PLAYING;
    }

    
}
