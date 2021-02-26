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
	public MapPopup mapPopup;

	bool bDialogBoxActive;

	#region Singleton
	protected static PopupUIManager instance;
	public static PopupUIManager Instance { get { return instance; } }
	#endregion
	private void Awake()
	{
		instance = this;
	}

    private void Update()
	{
        if (!GetDialogBoxIsActive())
        {
			EscapeButtonFunction();
			InventoryInput();
			QuestUIInput();
			MapInput();
        }
    }
	void EscapeButtonFunction()
    {
		if (Input.GetButtonDown("Escape"))
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
		if (Input.GetButtonDown("Inventory"))
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
	void MapInput()
    {
		if (Input.GetButtonDown("Map"))
		{
			if (!mapPopup.IsActive())
			{
				menuBarPopup.OpenMapUI();
			}
			else
			{
				menuBarPopup.CloseAllPopups();
			}
		}
	}
	void QuestUIInput() // Tab for opening quest manager UI
    {
		if (Input.GetButtonDown("QuestMenu"))
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

	public bool GetDialogBoxIsActive()
    {
		return bDialogBoxActive;
    }
	public void SetDialogBoxIsActive(bool _bActive)
    {
		bDialogBoxActive = _bActive;
    }
	
}
