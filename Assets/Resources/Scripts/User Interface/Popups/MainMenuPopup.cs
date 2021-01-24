using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuPopup : Popup
{
    public PopupButtonElement newGameElement;
    public PopupButtonElement loadGameElement;
    public PopupButtonElement optionsGameElement;
    public PopupButtonElement quitGameElement;
    List<PopupButtonElement> lstButtonsElement;
    PopupButtonElement selectedButtonElement;
    int iSelectedElement;
    private void Start()
    {
        SaveGameManager.gameLoaded = false;

        open();

        lstButtonsElement = new List<PopupButtonElement>();
        newGameElement.SetButtonElement(delegate () { NewGame(); }, delegate () { SetSelected(newGameElement);});
        lstButtonsElement.Add(newGameElement);
        loadGameElement.SetButtonElement(delegate () { LoadGame(); }, delegate () { SetSelected(loadGameElement); });
        lstButtonsElement.Add(loadGameElement);
        optionsGameElement.SetButtonElement(delegate () { Options(); }, delegate () { SetSelected(optionsGameElement); });
        lstButtonsElement.Add(optionsGameElement);
        quitGameElement.SetButtonElement(delegate () { Quit(); }, delegate () { SetSelected(quitGameElement); });
        lstButtonsElement.Add(quitGameElement);

    }
    private void Update()
    {
        if(container.gameObject.activeSelf)
            MenuKeysInput();
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

    public void NewGame()
    {
        //Debug.Log("New Game");
        SaveGameManager.gameLoaded = false;
        SceneManager.LoadScene(1);// Loads first scene for now
    }
    public void LoadGame()
    {
        SaveGameManager.gameLoaded = true;
        SceneManager.LoadScene(1);
    }
    public void Options()
    {
        Debug.Log("Options Not Applicable Yet..");
    }
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
          Application.Quit();
#endif
    }

    public void MenuKeysInput()
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
}
