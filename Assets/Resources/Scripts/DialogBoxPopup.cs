using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogBoxPopup : Popup
{
    public TextMeshProUGUI dialogText;
    private string[] dialogLines;
    private int dialogLineNumber = 0;

    private void Update()
    {
        if (Input.GetButtonDown("Interact"))
            NextLine();
    }
    public void setDialogText(string[] _dialogLines)
    {
        base.open();
        dialogLineNumber = -1;
        dialogLines = _dialogLines;
        //dialogText.text = dialogLines[dialogLineNumber];
    }

    public void NextLine()
    {
        if (container.gameObject.activeSelf)
        {
            dialogLineNumber++;

            if (dialogLineNumber > dialogLines.Length - 1)
            {
                base.close();
                dialogLineNumber = -1;
                GameController.Instance.inPlayMode = true;
            }
            else
                dialogText.text = dialogLines[dialogLineNumber];
                
        }
    }
    
}
