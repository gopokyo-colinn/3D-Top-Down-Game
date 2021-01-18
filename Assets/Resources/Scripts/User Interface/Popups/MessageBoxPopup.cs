using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageBoxPopup : Popup
{
    public TMPro.TextMeshProUGUI txtMessage;
    public float fTimeToShow;
    public void SendTextMessage(string _message,  float _fTimeToShow = 2, float _bgScale = 1f)
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
