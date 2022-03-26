using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuScreen : UIBase
{
    [SerializeField] Button playBtn;
    private void Awake()
    {
        playBtn.onClick.AddListener(() => OnPlayButtonClick());
    }
    void OnPlayButtonClick()
    {
        UISceneController.Instance.ShowUIScreen(ScreenManager.UIScreens.LEVELSELECTION);
    }
}
