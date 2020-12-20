using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuPopup : Popup
{
    public void ResumeButton()
    {
        if (GameController.bGamePaused)
        {
            GameController.bGamePaused = false;
            GameController.inPlayMode = true;
            Time.timeScale = 1f;
            close();
        }
    }
    public void SaveGameButton()
    {

    }
    public void LoadGameButton()
    {

    }
    public void QuitGameButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
