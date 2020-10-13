using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogBoxPopup : Popup
{
    const float TEXT_SPEED = 0.01f;
    public TextMeshProUGUI dialogText;
    private string[] dialogLines;
    private int dialogLineNumber = 0;
    private bool isTyping = false;

    private static bool dialogInProgress;

    private void Update()
    {
        if (container.gameObject.activeSelf)
        {
            if (Input.GetButtonDown("Interact"))
            {
                if(!isTyping)
                    NextLine();
                else
                {
                    StopAllCoroutines();
                    dialogText.text = dialogLines[dialogLineNumber];
                    isTyping = false;
                }
            }
        }
    }
    public void setDialogText(string[] _dialogLines)
    {
        base.open();
        dialogLineNumber = -1;
        dialogLines = _dialogLines;
        NextLine();
        //dialogText.text = dialogLines[dialogLineNumber];
    }

    public void NextLine()
    {
        dialogLineNumber++;
        if (!dialogInProgress)
            dialogInProgress = true;

        if (dialogLineNumber > dialogLines.Length - 1)
        {
            base.close();
            dialogLineNumber = -1;
            dialogInProgress = false;
            GameController.Instance.inPlayMode = true;
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(TypeSentence(dialogLines[dialogLineNumber]));
        }
    }
    IEnumerator TypeSentence(string _sentence)
    {
        isTyping = true;
        char[] _dialogChars = _sentence.ToCharArray();
        dialogText.text = "";
        for (int i = 0; i < _dialogChars.Length; i++)
        {
            dialogText.text += _dialogChars[i];
            yield return new WaitForSeconds(TEXT_SPEED);
        }
        isTyping = false;
    }

    public bool GetDialogInProgress()
    {
        return dialogInProgress;
    }
    
}
