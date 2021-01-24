using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBarPopup : Popup
{
    InventoryPopup inventoryPopup;
    QuestPopupUI questPopupUI;
    PauseMenuPopup pauseMenuPopup;
    SubMenuPopup subMenuPopup;
    List<Popup> lstPopup;

    private void Start()
    {
        open();
        lstPopup = new List<Popup>();
        inventoryPopup = PopupUIManager.Instance.inventoryPopup;
        lstPopup.Add(inventoryPopup);
        questPopupUI = PopupUIManager.Instance.questPopupUI;
        lstPopup.Add(questPopupUI);
        pauseMenuPopup = PopupUIManager.Instance.pauseMenuPopup;
        lstPopup.Add(pauseMenuPopup);
        subMenuPopup = PopupUIManager.Instance.subMenuPopup;
        lstPopup.Add(subMenuPopup);
        close();

    }
    public override void open()
    {
        base.open();
        GameController.bGamePaused = true;
        GameController.inPlayMode = false;
        Time.timeScale = 0f;
    }
    public override void close()
    {
        GameController.bGamePaused = false;
        GameController.inPlayMode = true;
        Time.timeScale = 1f;
        base.close();
    }
    public void OpenInventoryUI()
    {
        CloseAllPopups();
        if (GameController.inPlayMode)
        {
            GameController.inPlayMode = false;
            inventoryPopup.UpdateInventoryUI(PlayerController.Instance.GetInventory());
            inventoryPopup.open();
        }
    }
    public void OpenPauseMenuUI()
    {
        CloseAllPopups();
        if (GameController.inPlayMode)
        {
            GameController.inPlayMode = false;
            pauseMenuPopup.open();
        }
    }
    public void OpenQuestUI()
    {
        CloseAllPopups();
        if (GameController.inPlayMode)
        {
            GameController.inPlayMode = false;
            questPopupUI.open();
        }
    }
    public void CloseAllPopups()
    {
        for (int i = 0; i < lstPopup.Count; i++)
        {
            lstPopup[i].close();
        }
    }
}
