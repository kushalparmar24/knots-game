using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// User interface screen. 
/// attach this to all ui screens and select the appropriate screen id
/// </summary>
public class UIBase : MonoBehaviour 
{
	public enum ScreenType
	{
		FullScreen
	}
	public ScreenType	screenType;
	public ScreenManager.UIScreens	thisScreen;
	public bool canGoBackToPreviousScreen;

	public virtual void GoBack ()
    {
		UISceneController.Instance.GoToPreviousScreen();
	}
}
