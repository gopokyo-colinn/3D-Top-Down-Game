using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageBoxPopup : Popup
{
    public TMPro.TextMeshProUGUI txtMessage;
    public float fTimeToShow;

    public void ShowMessageAfterDialog(string _message, float _fTimeToShow = 2, float _bgScale = 1f) // Its normally for the quests...
    {
        StartCoroutine(MessagePopup(_message, _fTimeToShow, _bgScale));
    }

    IEnumerator MessagePopup(string _message, float _fTimeToShow = 2, float _bgScale = 1f)
    {
        yield return new WaitForSeconds(1f); // waiting to check if dialog box is getting active within next few frames or not.

        yield return new WaitUntil(() => !PopupUIManager.Instance.GetDialogBoxIsActive());

        ShowTextMessage(_message, _fTimeToShow, _bgScale);
       // StopCoroutine(MessagePopup());
    }
    public void ShowTextMessage(string _message,  float _fTimeToShow = 2, float _bgScale = 1f)
    {
        open();
        bgImage.transform.localScale = new Vector3(1, _bgScale, 1);
        txtMessage.text = _message;
        StartCoroutine(DisablePopupAfter(_fTimeToShow));
    }
    IEnumerator DisablePopupAfter(float _fWaitTime)
    {
        yield return new WaitForSecondsRealtime(_fWaitTime);
        close();
        StopAllCoroutines();
    }
}
