using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuPopup : Popup
{
    public PopupButtonElement resumeGameElement;
    public PopupButtonElement saveGameElement;
    public PopupButtonElement loadGameElement;
    public PopupButtonElement optionsGameElement;
    public PopupButtonElement quitGameElement;
    List<PopupButtonElement> lstButtonsElement;

    PopupButtonElement selectedButtonElement;
    int iSelectedElement;
    private void Start()
    {
        lstButtonsElement = new List<PopupButtonElement>();
        resumeGameElement.SetButtonElement(delegate () { ResumeButton(); }, delegate () { SetSelected(resumeGameElement); });
        lstButtonsElement.Add(resumeGameElement);
        saveGameElement.SetButtonElement(delegate () { SaveGameButton(); }, delegate () { SetSelected(saveGameElement); });
        lstButtonsElement.Add(saveGameElement);
        loadGameElement.SetButtonElement(delegate () { LoadGameButton(); }, delegate () { SetSelected(loadGameElement); });
        lstButtonsElement.Add(loadGameElement);
        optionsGameElement.SetButtonElement(delegate () { OptionsButton(); }, delegate () { SetSelected(optionsGameElement); });
        lstButtonsElement.Add(optionsGameElement);
        quitGameElement.SetButtonElement(delegate () { QuitGameButton(); }, delegate () { SetSelected(quitGameElement); });
        lstButtonsElement.Add(quitGameElement);
    }
    private void Update()
    {
        if(container.gameObject.activeSelf)
            MenuKeysInput();
    }
    public override void open()
    {
        base.open();
        SetSelected(lstButtonsElement[0]);
        PopupUIManager.Instance.menuBarPopup.open();
    }
    public override void close()
    {
        base.close();
        PopupUIManager.Instance.menuBarPopup.close();
    }
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
        GameController.Instance.SaveGame();
        close();
        PopupUIManager.Instance.msgBoxPopup.ShowTextMessage("Game Saved....");
    }
    public void LoadGameButton()
    {
        GameController.Instance.LoadGame();
        PopupUIManager.Instance.msgBoxPopup.ShowTextMessage("Game Loaded....");
    }
    public void OptionsButton()
    {
        Debug.Log("Not Applicable Yet.");
    }
    public void QuitGameButton()
    {
        //#if UNITY_EDITOR
        //        UnityEditor.EditorApplication.isPlaying = false;
        //#else
        //         Application.Quit();
        //#endif
        Destroy(GameController.Instance.gameObject);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    public override void MenuKeysInput()
    {
        if (Input.GetAxisRaw("Vertical") > 0f && Input.anyKeyDown)
        {
            if (iSelectedElement > 0)
                SetSelected(lstButtonsElement[iSelectedElement - 1]);
            else if (iSelectedElement == 0)
                SetSelected(lstButtonsElement[lstButtonsElement.Count - 1]);
        }
        else if (Input.GetAxisRaw("Vertical") < 0 && Input.anyKeyDown)
        {
            if (iSelectedElement < lstButtonsElement.Count - 1)
                SetSelected(lstButtonsElement[iSelectedElement + 1]);
            else if (iSelectedElement == lstButtonsElement.Count - 1)
                SetSelected(lstButtonsElement[0]);
            else if (iSelectedElement == 0)
                SetSelected(lstButtonsElement[1]);
        }
        if (Input.GetButtonDown("Interact"))
        {
            if (selectedButtonElement != null)
            {
                selectedButtonElement.clickAction.Invoke();
            }
        }
    }
    public void SetSelected(PopupButtonElement _element)
    {
        if (selectedButtonElement != null)
            selectedButtonElement.SetSelectedElement(false);

        selectedButtonElement = _element;

        if (selectedButtonElement)
        {
            selectedButtonElement.SetSelectedElement(true);
        }
        for (int i = 0; i < lstButtonsElement.Count; i++)
        {
            if (lstButtonsElement[i] == selectedButtonElement)
            {
                iSelectedElement = i;
            }
        }
    }
}
