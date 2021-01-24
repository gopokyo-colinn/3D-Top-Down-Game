using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class PopupButtonElement : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    public Image imgSelected;
    public TextMeshProUGUI labelText;
    bool bSelected;
    public UnityAction clickAction;
    UnityAction hoverAction;

    public void SetButtonElement(string _sButtonName, UnityAction _clickAction, UnityAction _hoverAction = null)
    {
        clickAction = _clickAction;
        hoverAction = _hoverAction;
        labelText.text = _sButtonName;
    }
    public void SetButtonElement(UnityAction _clickAction, UnityAction _hoverAction = null)
    {
        clickAction = _clickAction;
        hoverAction = _hoverAction;
    }
    public void SetSelectedElement(bool _bSelected)
    {
        bSelected = _bSelected;
        imgSelected.gameObject.SetActive(bSelected);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        clickAction.Invoke();
        if(PopupUIManager.Instance)
            PopupUIManager.Instance.subMenuPopup.close();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverAction.Invoke();
    }

}
