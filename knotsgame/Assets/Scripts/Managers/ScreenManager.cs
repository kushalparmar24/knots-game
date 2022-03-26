using UnityEngine;
using System.Collections.Generic;

public class ScreenManager
{
    public enum UIScreens
    {
        NONE,
        LEVELSELECTION,
        MENU,
        INGAMEHUD,
        PAUSE
    };
    private Stack<UIScreens> queuedScreens;
    Stack<UIScreens> activeDisplayedScreens;
    private Dictionary<UIScreens, GameObject> screenDict;
    UIScreens lastFullScreen;
    private UIScreens activeUIScreen;
    public UIScreens ActiveUIScreen { get => activeUIScreen; }
    public ScreenManager()
    {
        queuedScreens = new Stack<UIScreens>();
        activeDisplayedScreens = new Stack<UIScreens>();
        screenDict = new Dictionary<UIScreens, GameObject>();
    }
    #region private methods
    T GetComponent<T>(UIScreens screen)
    {
        return screenDict[screen].GetComponent<T>();
    }

    /// <summary>
    /// Using to disable the active screens in the current scene when switching scenes.
    /// </summary>
    // disables all active displayed screens..
    void DisablePrevScreens()
    {
        for (int i = activeDisplayedScreens.Count - 1; i >= 0; i--)
        {
            UIBase prevScreen = GetComponent<UIBase>(activeDisplayedScreens.Pop());
            if (prevScreen != null)
                prevScreen.gameObject.SetActive(false);
        }
        activeDisplayedScreens.Clear();
    }
    /// <summary>
    /// if the current screen exists in the queue, Clears the queue till the current screen(remove enums behind the current screen)
    /// </summary>
    void CheckInQueue(UIScreens currentScreen)
    {
        if (queuedScreens.Contains(currentScreen))
        {
            int count = queuedScreens.Count;
            for (int i = 0; i < count; i++)
            {
                queuedScreens.Pop();
                if (queuedScreens.Peek() == lastFullScreen)
                {
                    queuedScreens.Pop(); // remove the current screen from the queue
                    break;
                }
            }
        }
    }
    #endregion

    #region public methods
    public void FindAllScreens()
    {
        GameObject[] canvas = GameObject.FindGameObjectsWithTag("Canvas");

        for (int i = 0; i < canvas.Length; i++)
        {
            UIBase[] screens = canvas[i].GetComponentsInChildren<UIBase>(true);
            for (int j = 0; j < screens.Length; j++)
            {
                AddScreen(screens[j].thisScreen, screens[j].gameObject);
            }
        }
    }

    public void FindAllScreensInGameObject(GameObject parentObj)
    {
        UIBase[] screens = parentObj.GetComponentsInChildren<UIBase>(true);
        for (int j = 0; j < screens.Length; j++)
        {
            AddScreen(screens[j].thisScreen, screens[j].gameObject);
        }
    }

    public void HideAllScreens()
    {
        foreach (KeyValuePair<UIScreens, GameObject> pair in screenDict)
            pair.Value.SetActive(false);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackButtonHandler();
        }
    }

    public void BackButtonHandler()
    {
        if (screenDict.ContainsKey(activeUIScreen))
            screenDict[activeUIScreen].GetComponent<UIBase>().GoBack();
    }

    // called when back is pressed.. on closing the current screen..
    public void GoBack()
    {
        GameObject activeObject = screenDict[activeDisplayedScreens.Peek()];
        if (activeObject == null)
            return;

        UIBase curr = activeObject.GetComponent<UIBase>();
        if (curr.screenType == UIBase.ScreenType.FullScreen)// if the current screen is fullscreen..
        {
            if (curr.canGoBackToPreviousScreen) // if goback is enabled, show the queued screen..
            {
                if (queuedScreens.Count > 0)
                {
                    ShowUIScreen(queuedScreens.Pop(), false, true);
                }
            }
           
        }
       
    }
    
    /// <summary>
    /// Call this method to show the screen
    /// </summary>
    /// <param name="screen"></param>
    /// <param name="clearList"></param>
    /// <param name="isClosing"></param>
    public void ShowUIScreen(UIScreens screen, bool clearList = false, bool isClosing = false)
    {
        if (!screenDict.ContainsKey(screen))
        {
            Debug.LogFormat("Screen with name {0} is not found.\n{1}", screen,"Check if Canvas is tagged with 'Canvas'");
            return;
        }


        if (screen == activeUIScreen) // if the screen is the active ui screen enable the screen and return.(used while coming back to previous scene screen from the next scene. 
        {
            screenDict[screen].SetActive(true);
            return;
        }

        if (clearList)
        {
            queuedScreens.Clear();
            activeUIScreen = UIScreens.NONE;
        }

        UIBase currScreen = screenDict[screen].GetComponent<UIBase>();
        UIBase prevScreen = null;
        if (activeUIScreen != UIScreens.NONE)
            prevScreen = screenDict[activeUIScreen].GetComponent<UIBase>();

        // if current screen type and prev screen type is same then disable the prev screen.. else display the current screen on top of prev screen..
        if (prevScreen != null && currScreen.screenType == prevScreen.screenType)
        {
            activeDisplayedScreens.Pop();
            screenDict[activeUIScreen].SetActive(false);
        }

        if (currScreen.screenType == UIBase.ScreenType.FullScreen)
        {
            DisablePrevScreens();

            if (currScreen.canGoBackToPreviousScreen && !isClosing) // if enabled push it to queue to enable it later..
            {
                queuedScreens.Push(lastFullScreen);
            }
            else if (!isClosing)// if full screen is shown and goback is disabled clear all stacks ..
            {
                queuedScreens.Clear();
            }

            lastFullScreen = screen;
        }
       
        if (screen != UIScreens.NONE)
        {
            screenDict[screen].SetActive(true);
        }

        // check if the screen is in queue.. if there then pop the before screens
        CheckInQueue(screen);

        activeUIScreen = screen;
        activeDisplayedScreens.Push(screen);
    }

    
    public void AddScreen(UIScreens scr, GameObject screenObj)
    {
        GameObject screenObject = null;

        if (screenDict.TryGetValue(scr, out screenObject))
            screenDict[scr] = screenObj;
        else
            screenDict.Add(scr, screenObj);
    }

    public GameObject GetScreenObject(UIScreens scr)
    {
        return screenDict[scr];
    }
    #endregion
}
