using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupUIManager : MonoBehaviour
{
    public DialogBoxPopup dialogBoxPopup;
	public PauseMenuPopup pauseMenuPopup;

	private static PopupUIManager instance;
	public static PopupUIManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<PopupUIManager>();
				if (instance == null)
				{
					GameObject obj = new GameObject();
					obj.name = typeof(PopupUIManager).Name;
					instance = obj.AddComponent<PopupUIManager>();
				}
			}
			return instance;
		}
	}
	protected virtual void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			PauseGame();
		}
	}

	void PauseGame()
    {

        if (!GameController.bGamePaused)
        {
            GameController.bGamePaused = true;
            GameController.inPlayMode = false;
            Time.timeScale = 0f;
            pauseMenuPopup.open();
        }
        else
        {
            pauseMenuPopup.ResumeButton();
        }
    }
	
}
