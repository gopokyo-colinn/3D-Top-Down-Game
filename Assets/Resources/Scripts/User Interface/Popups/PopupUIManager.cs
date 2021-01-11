using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupUIManager : MonoBehaviour
{
    public DialogBoxPopup dialogBoxPopup;
	public PauseMenuPopup pauseMenuPopup;
	public InventoryPopup inventoryPopup;
	public SubMenuPopup subMenuPopup;
	public MessageBoxPopup msgBoxPopup;

	PlayerController player;

	bool bInventoryIsOpen;

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
    private void Start()
    {
		player = GameController.Instance.player;
	}

    private void Update()
	{
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (bInventoryIsOpen)
            {
                CloseInventory();
            }
            else
            {
                if (GameController.inPlayMode)
                {
					PauseGame();
                }
                else
                {
					pauseMenuPopup.ResumeButton();
				}
            }
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!bInventoryIsOpen)
            {
				if(GameController.inPlayMode)
					OpenInventory();
            }
            else
            {
                CloseInventory();
            }
        }
    }
	void OpenInventory()
    {
		GameController.inPlayMode = false;
		bInventoryIsOpen = true;
		inventoryPopup.open();
    }
	void CloseInventory()
    {
		GameController.inPlayMode = true;
		bInventoryIsOpen = false;
		inventoryPopup.close();
		subMenuPopup.close();
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
