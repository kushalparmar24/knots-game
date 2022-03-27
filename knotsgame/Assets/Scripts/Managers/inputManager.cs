using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inputManager : MonoBehaviour
{
    public enum InputType { NONE, SWIPING, FIRSTTOUCH };
    Vector3 point;
    InputType inputType; 
    public static inputManager instance;
    private void Awake()
    {
        instance = this;
        inputType = InputType.NONE;
    }

    /// <summary>
    /// sends the user input to levelmanager class for it to procceed with tile placements.
    /// </summary>
    private void Update()
    {
        if (levelManager.Instance.gameState != levelManager.GameState.PLAYING)
        {
            inputType = InputType.NONE;
            return;
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            inputType = InputType.FIRSTTOUCH;
            levelManager.Instance.inputRecieved(inputType, point);
        }
        if (Input.GetMouseButtonUp(0))
        {
            inputType = InputType.NONE;
            levelManager.Instance.inputRecieved(inputType, point);
        }

        if (inputType == InputType.FIRSTTOUCH || inputType == InputType.SWIPING)
        {
            inputType = InputType.SWIPING;
            levelManager.Instance.inputRecieved(inputType, point);
        }

#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 _touchPos; _touchPos = touch.position;
            point = Camera.main.ScreenToWorldPoint(_touchPos);
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
            {
                inputType = InputType.FIRSTTOUCH;
                levelManager.Instance.inputRecieved(inputType, point);
            } 
            else if (touch.phase == TouchPhase.Ended)
            {
                inputType = InputType.NONE;
                levelManager.Instance.inputRecieved(inputType, point);
            }
            if (inputType == InputType.FIRSTTOUCH || inputType == InputType.SWIPING)
            {
                inputType = InputType.SWIPING;
                levelManager.Instance.inputRecieved(inputType, point);
            }
        }
#endif
    }
}
