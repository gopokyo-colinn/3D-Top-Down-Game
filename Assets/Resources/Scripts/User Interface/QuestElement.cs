using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
public class QuestElement : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector]
    public Quest myQuest; // change with structs for easy save load
    public Image imgSelected;
    public TextMeshProUGUI questTitleTxt;
    public UnityAction clickAction;
    public RectTransform rectTransform;
    bool bNonClickable;
    bool bSelected;

    public void SetElement(Quest _myQuest, UnityAction _action, float _width = 712, float _height = 68)
    {
        myQuest = _myQuest;
        clickAction = _action;
        questTitleTxt.text = myQuest.sQuestTitle;

        rectTransform.sizeDelta = new Vector2(_width, _height);
    }
    public void SetElement(string _text, bool _bResizeable = false, float _fontSize = 22, float _width = 590, float _height = 58, bool _bNonClickable = true)
    {
        bNonClickable = _bNonClickable;
        questTitleTxt.text = _text;
        if (_bResizeable)
        {
            questTitleTxt.fontSize = _fontSize;
            rectTransform.sizeDelta = new Vector2(_width, _height);
        }
    }
    public void SetSelectedElement(bool _bSelected)
    {
        bSelected = _bSelected;
        imgSelected.gameObject.SetActive(bSelected);
    }
    public void SetFontStrikethrough() 
    {
        questTitleTxt.fontStyle = FontStyles.Strikethrough;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!bNonClickable)
        {
            if (!bSelected)
            {
                clickAction.Invoke();
            }
        }
    }
}
