using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageBoxPopup : Popup
{
    public TMPro.TextMeshProUGUI txtMessage;
    public float fTimeToShow;
    public void SendTextMessage(string _message, float _bgScale = 1f, float _fTimeToShow = 2)
    {
        open();
        bgImage.transform.localScale = new Vector3(1, _bgScale, 1);
        txtMessage.text = _message;
        StartCoroutine(DisableItemAfter(_fTimeToShow));
    }

    IEnumerator DisableItemAfter(float _fWaitTime)
    {
        yield return new WaitForSeconds(_fWaitTime);
        close();
        StopAllCoroutines();
    }
}
