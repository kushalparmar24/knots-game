using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inputManager : MonoBehaviour
{
    public enum InputType { NONE, SWIPING };

    InputType inputType; 
    public static inputManager instance;
    private void Awake()
    {
        instance = this;
        inputType = InputType.NONE;
    }

    private void Update()
    {
        if (levelManager.Instance.gameState != levelManager.GameState.PLAYING)
            return;
        Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            inputType = InputType.SWIPING;
            levelManager.Instance.inputRecieved(inputType, point);
        }
        if (Input.GetMouseButtonUp(0))
        {
            inputType = InputType.NONE;
            levelManager.Instance.inputRecieved(inputType, point);
        }

        if (inputType == InputType.SWIPING)
        {
            levelManager.Instance.inputRecieved(inputType, point);
        }
#if UNITY_ANDROID
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 _touchPos; _touchPos = touch.position;
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
            {
                inputType = InputType.SWIPING;
                levelManager.Instance.inputRecieved(inputType, _touchPos);
            } 
            else if (touch.phase == TouchPhase.Ended)
            {
                inputType = InputType.NONE;
                levelManager.Instance.inputRecieved(inputType, _touchPos);
            }
            if (inputType == InputType.SWIPING)
            {
                levelManager.Instance.inputRecieved(inputType, _touchPos);
            }
        }
#endif
    }
}
