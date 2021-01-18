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
	public QuestPopupUI questPopupUI;
	public MenuBarPopup menuBarPopup;


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
		EscapeButtonFunction();
		InventoryInput();
		QuestUIInput();
    }
	void EscapeButtonFunction()
    {
		if (Input.GetKeyDown(KeyCode.Escape))
		{
            if (GameController.inPlayMode)
            {
                menuBarPopup.OpenPauseMenuUI();
            }
            else
            {
                menuBarPopup.CloseAllPopups();
            }
        }
	} // Escape for Closing any open popup and opening pause menu
	void InventoryInput() // I for Inventory
    {
		if (Input.GetKeyDown(KeyCode.I))
		{
			if (!inventoryPopup.IsActive())
			{
				menuBarPopup.OpenInventoryUI();
			}
			else
			{
				menuBarPopup.CloseAllPopups();
			}
		}
	}
	void QuestUIInput() // Tab for opening quest manager UI
    {
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			if (!questPopupUI.IsActive())
			{
				menuBarPopup.OpenQuestUI();
			}
			else
			{
				menuBarPopup.CloseAllPopups();
			}
		}
	}
	
}
