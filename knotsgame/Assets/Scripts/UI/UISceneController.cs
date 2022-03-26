using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UISceneController : MonoBehaviour
{
    Dictionary<string, ScreenManager> screensInScenes;
	static UISceneController instance;
	string currentSceneName;
	ScreenManager currentScreenManager;
	[SerializeField]
	ScreenManager.UIScreens screenToShow = ScreenManager.UIScreens.NONE;
	Scene currentScene;
	#region getters
	public static UISceneController Instance
	{
		get { return instance; }
	}

    public ScreenManager CurrentScreenManager { get => currentScreenManager; }
	#endregion

	#region private methods
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
	void Update()
	{
		if (currentScreenManager != null)
			currentScreenManager.Update();
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
	#endregion

	/// <summary>
	/// enables the passed screentype from current scene.
	/// </summary>
	#region public methods
	public void ShowUIScreen(ScreenManager.UIScreens uiScreen)
	{
		currentScreenManager.ShowUIScreen(uiScreen);
	}

	/// <summary>
	/// loads the give scene and enables the passed screentype after scene is loaded
	/// </summary>
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

	public void OnBackButtonPressed()
    {
		currentScreenManager.BackButtonHandler();
	}

    public void GoToPreviousScreen()
    {
        currentScreenManager.GoBack();
    }
    #endregion

}
