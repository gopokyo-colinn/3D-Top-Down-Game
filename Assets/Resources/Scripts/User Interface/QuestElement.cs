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
    public void SetElement(string _text, float _width = 590, float _height = 58, bool _bNonClickable = true)
    {
        bNonClickable = _bNonClickable;
        questTitleTxt.fontSize = 22f;
        questTitleTxt.text = _text;
        rectTransform.sizeDelta = new Vector2(_width, _height);
    }
    public void SetSelectedElement(bool _bSelected)
    {
        bSelected = _bSelected;
        imgSelected.gameObject.SetActive(bSelected);

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
