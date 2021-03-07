using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageBoxPopup : Popup
{
    public TMPro.TextMeshProUGUI txtMessage;
    public float fTimeToShow;
    Queue<string> msgList = new Queue<string>();

    public void ShowMessageAfterDialog(string _message, float _fTimeToShow = 2, float _bgScale = 1f) // Its normally for the quests...
    {
        StartCoroutine(MessagePopup(_message, _fTimeToShow, _bgScale));
    }

    IEnumerator MessagePopup(string _message, float _fTimeToShow = 2, float _bgScale = 1f)
    {
        yield return new WaitForSeconds(1f); // waiting to check if dialog box is getting active within next few frames or not.
        
        yield return new WaitUntil(() => !PopupUIManager.Instance.GetDialogBoxIsActive());

        msgList.Enqueue(_message);
        fBgScale += _bgScale / 1.5f;
        yield return new WaitForSeconds(0.5f);
        ShowTextMessage(_message, _fTimeToShow, fBgScale);
       // StopCoroutine(MessagePopup());
    }
    float fBgScale = 0;
    public void ShowTextMessage(string _message,  float _fTimeToShow = 2, float _bgScale = 1f)
    {
        open();
        if (_bgScale < 1)
            _bgScale = 1;
       // PopupUIManager.Instance.SetDialogBoxIsActive(true);
        bgImage.transform.localScale = new Vector3(1, _bgScale, 1);
        for (int i = 0; i < msgList.Count; i++)
        {
            if(msgList.Count <= 1)
                txtMessage.text += msgList.Dequeue();
            else
                txtMessage.text += msgList.Dequeue() + "\n \n";
        }
        
        StartCoroutine(DisablePopupAfter(_fTimeToShow));
    }
    IEnumerator DisablePopupAfter(float _fWaitTime)
    {
        yield return new WaitForSecondsRealtime(_fWaitTime);
        fBgScale = 0;
        txtMessage.text = "";
       // PopupUIManager.Instance.SetDialogBoxIsActive(false);
        close();
        StopAllCoroutines();
    }
}
