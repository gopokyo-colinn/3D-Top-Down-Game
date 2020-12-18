using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class PopupButtonElement : MonoBehaviour
{
    public Button btnMain;
    public void SetButtonName(string _setName)
    {
        btnMain.gameObject.SetActive(true);
        btnMain.GetComponentInChildren<TextMeshProUGUI>().text = _setName;
    }
}
