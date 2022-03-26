using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UISceneController : MonoBehaviour
{
    Dictionary<string, ScreenManager> screensInScenes;

	private static UISceneController instance;

	private string currentSceneName;
	private ScreenManager currentScreenManager;
	[SerializeField]
	private ScreenManager.UIScreens screenToShow = ScreenManager.UIScreens.NONE;
	Scene currentScene;

	public static UISceneController Instance
	{
		get { return instance; }
	}

    public ScreenManager CurrentScreenManager { get => currentScreenManager; }

    private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
			Destroy(gameObject);
		screensInScenes = new Dictionary<string, ScreenManager>();
	}

	void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneWasLoaded;
    }

	private void OnSceneWasLoaded(Scene arg0, LoadSceneMode arg1)
	{
		ScreenManager screenManager = null;
		currentSceneName = arg0.name;
		if (screensInScenes.ContainsKey(currentSceneName))
		{
			screenManager = screensInScenes[currentSceneName];
		}
		else
		{
			screenManager = new ScreenManager();
			screensInScenes.Add(currentSceneName, screenManager);
		}

		screenManager.FindAllScreens();
		
		currentScreenManager = screenManager;
		if (screenToShow != ScreenManager.UIScreens.NONE)
		{
			currentScreenManager.HideAllScreens(); // added to hide dont destroy on load screens on scene transitions..
			ShowUIScreen(screenToShow);
			screenToShow = ScreenManager.UIScreens.NONE;
		}
	}
	
	public void ShowUIScreen(ScreenManager.UIScreens uiScreen)
	{
		currentScreenManager.ShowUIScreen(uiScreen);
	}

	public void ShowUIScreen(in string sceneName,  ScreenManager.UIScreens uiScreen)
	{
        if (string.Equals(currentSceneName, sceneName))
            currentScreenManager.ShowUIScreen(uiScreen);
        else
        {
            screenToShow = uiScreen;
			StartCoroutine(LoadSceneAsync(sceneName));
		}
    }

	IEnumerator LoadSceneAsync(string Name)
	{
		AsyncOperation asyncOperation;
		asyncOperation = SceneManager.LoadSceneAsync(Name, LoadSceneMode.Single);
		while (!asyncOperation.isDone)
		{
			yield return null;
		}

		currentScene = SceneManager.GetSceneByName(Name);
	}

	public void OnBackButtonPressed()
    {
		currentScreenManager.BackButtonHandler();
	}

    public void GoToPreviousScreen()
    {
        currentScreenManager.GoBack();
    }
    // Update is called once per frame
    void Update()
    {
		if(currentScreenManager!= null)
			currentScreenManager.Update();
	}
}
