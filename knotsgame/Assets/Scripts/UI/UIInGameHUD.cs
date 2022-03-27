using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIInGameHUD : UIBase
{
    [SerializeField] Button playBtn;
    [SerializeField] TextMeshProUGUI CompletedText,levelNumText;
    private void Awake()
    {
        playBtn.onClick.AddListener(() => OnPlayButtonClick());
    }
    
    void OnPlayButtonClick()
    {
        levelManager.Instance.gameState = levelManager.GameState.PAUSED;
        UISceneController.Instance.ShowUIScreen(ScreenManager.UIScreens.PAUSE);
    }

    public void setLevelNumText()
    {
        levelNumText.text = "Level "+(gameManager.Instance.getCurrentLevel+1).ToString();
    }
    public void setCompletedTextState(bool state_)
    {
        CompletedText.gameObject.SetActive(state_);
    }
}
