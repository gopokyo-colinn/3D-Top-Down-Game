using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogBoxPopup : Popup
{
    const float TEXT_SPEED = 0.01f;
    public TextMeshProUGUI dialogText;
    private string[] sDialogLines;
    private int iDialogLineNumber = 0;
    private bool bIsTyping = false;
    private bool bResponseSelecting;

    private bool bResponseFound;

    public RectTransform responsePopupPosition;

    private static bool dialogInProgress;
    NPCEntity questNPC;

    private void Update()
    {
        if (container.gameObject.activeSelf)
        {
            if(Input.GetButtonDown("Interact"))
            {
                if (!bResponseSelecting)
                {
                    if (!bIsTyping)
                        NextLine();
                    else
                    {
                        if (iDialogLineNumber > 0) // TODO: jgad laya first line skip nhi honi, try adding double tap interact for the forst line only
                        {
                            StopAllCoroutines();
                            dialogText.text = sDialogLines[iDialogLineNumber];
                            bIsTyping = false;
                        }
                    }
                }
            }
        }
    }
    public void setDialogText(string[] _dialogLines)
    {
        base.open();
        PopupUIManager.Instance.SetDialogBoxIsActive(true);
        iDialogLineNumber = -1;
        sDialogLines = new string[_dialogLines.Length];
        sDialogLines = _dialogLines;
        NextLine();
        //dialogText.text = dialogLines[dialogLineNumber];
    }

    public void NextLine()
    {
        iDialogLineNumber++;
        if (!dialogInProgress)
            dialogInProgress = true;

        if (iDialogLineNumber > sDialogLines.Length - 1)
        {
            base.close();
            PopupUIManager.Instance.SetDialogBoxIsActive(false);
            iDialogLineNumber = -1;
            dialogInProgress = false;
            GameController.inPlayMode = true;
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(TypeSentence());
        }
    }
    IEnumerator TypeSentence()
    {
        CheckForSpecialFunction();
        bIsTyping = true;

        char[] _dialogChars = sDialogLines[iDialogLineNumber].ToCharArray();

        dialogText.text = "";
        for (int i = 0; i < _dialogChars.Length; i++)
        {
            dialogText.text += _dialogChars[i];
            yield return new WaitForSeconds(TEXT_SPEED);
        }
        bIsTyping = false;
    }

    public void CheckForSpecialFunction()
    {
        if(questNPC.GetQuest() != null)
        {
            if(questNPC.GetQuest().eQuestType == QuestType.SIDEQUEST)
            {
                if (sDialogLines[iDialogLineNumber].Contains("&response"))
                {
                    sDialogLines[iDialogLineNumber] = sDialogLines[iDialogLineNumber].Replace("&response", ShowResponsePopup());
                    bResponseFound = true;
                }
                if(iDialogLineNumber == sDialogLines.Length - 1)
                {
                    if (!bResponseFound)
                    {
                        ResponseYES();
                        bResponseFound = false;
                    }
                }
            }
        }
    }
    public string MsgBoxPopup(string _sMessage)
    {
        // TODO: here select reward text from npc's quest
        PopupUIManager.Instance.msgBoxPopup.ShowTextMessage(_sMessage);
        return "";
    }
    public string ShowResponsePopup()
    {
        List<structSubMenu> _lstSubMenu = new List<structSubMenu>();
        structSubMenu _subMenu = new structSubMenu();
        _subMenu.sName = "Yes";
        _subMenu.action = delegate () { ResponseYES(); };
        _lstSubMenu.Add(_subMenu);

        _subMenu = new structSubMenu();
        _subMenu.sName = "No";
        _subMenu.action = delegate () { ResponseNO(); };
        _lstSubMenu.Add(_subMenu);

        bResponseSelecting = true;

        PopupUIManager.Instance.subMenuPopup.openMenu(_lstSubMenu, responsePopupPosition.position);// new Vector2(556, 126));

        return "";
    }
    void ResponseYES()
    {
        questNPC.ActivateQuest();
        if(iDialogLineNumber < sDialogLines.Length - 1)
            NextLine();
        bResponseSelecting = false;
    }
    void ResponseNO()
    {
        sDialogLines = new string[1];
        sDialogLines[0] = "Come back if you change your mind..."; // TODO: Add more lines.....
        iDialogLineNumber = -1;
        StopAllCoroutines();
        NextLine();

        bResponseSelecting = false;
    }
    public void SetQuestNPC(NPCEntity _npc)
    {
        questNPC = _npc;
    }
    public bool GetDialogInProgress()
    {
        return dialogInProgress;
    }
    
}
