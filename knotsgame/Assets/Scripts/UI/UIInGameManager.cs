using UnityEngine;

/// <summary>
/// static instance that provide the access to other UI screen in game. 
/// </summary>
public class UIInGameManager : MonoBehaviour
{
    [SerializeField] UIInGameHUD inGameHUD;
    static UIInGameManager instance;
    public static UIInGameManager Instance { get { return instance; } }
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void levelCompletedTextStat(bool state_)
    {
        inGameHUD.setCompletedTextState(state_);
    }
    public void setLevelText()
    {
        inGameHUD.setLevelNumText();
    }
}
