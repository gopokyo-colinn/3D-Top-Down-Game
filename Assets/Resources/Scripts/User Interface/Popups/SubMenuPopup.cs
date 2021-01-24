using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public struct structSubMenu
{
    public string sName;
    public UnityAction action;
}
public class SubMenuPopup : Popup
{
    public List<structSubMenu> lstSubMenu;

    public Transform tFieldContainer;

    public PopupButtonElement prefabButtonElement;

    private List<PopupButtonElement> lstPopupButtonElement;

    private RectTransform rtTransform;

    public Transform containerAll;

    int iSelectedElement;

    private PopupButtonElement selectedButtonElement;
    void Start()
    {
        lstPopupButtonElement = new List<PopupButtonElement>();
        rtTransform = GetComponent<RectTransform>();
        containerAll.gameObject.SetActive(false);
        
    }
    private void Update()
    {
        if (container.gameObject.activeSelf)
        {
            SelectElementWithInput();
        }
    }
    public override void open()
    {
        base.open();
        iSelectedElement = 0;
        Debug.Log("I am opened");
        containerAll.gameObject.SetActive(true);
    }
    public override void close()
    {
        base.close();
        containerAll.gameObject.SetActive(false);
    }
    public void openMenu(List<structSubMenu> _lstSubMenu)
    {
        open();
        Vector2 _v2Position = Input.mousePosition;
        if (_v2Position.x > Screen.width / 2f)
            _v2Position.x -= (rtTransform.sizeDelta.x) + 10f;
        else
            _v2Position.x += 10f;

        if (_v2Position.y > Screen.height / 2f)
            _v2Position.y -= 10f;
        else
            _v2Position.y += (rtTransform.sizeDelta.y) + 10f;

        rtTransform.position = _v2Position;

        PopupButtonElement[] _buttonElements = tFieldContainer.GetComponentsInChildren<PopupButtonElement>();

        for (int i = 0; i < lstPopupButtonElement.Count; i++)
        {
            Destroy(lstPopupButtonElement[i].gameObject);
        }

        lstPopupButtonElement = new List<PopupButtonElement>();

       // PopupButtonElement _buttonElement;
        lstSubMenu = _lstSubMenu;
        for (int i = 0; i < _lstSubMenu.Count; i++)
        {
            PopupButtonElement _buttonElement = Instantiate<PopupButtonElement>(prefabButtonElement, tFieldContainer);

            _buttonElement.SetButtonElement(_lstSubMenu[i].sName, _lstSubMenu[i].action, delegate () { SetSelected(_buttonElement); });

            lstPopupButtonElement.Add(_buttonElement);
        }
        selectedButtonElement = null;
    }
    public void openMenu(List<structSubMenu> _lstSubMenu, Vector2 _position)
    {
        open();

        rtTransform.position = _position;

        PopupButtonElement[] _buttonElements = tFieldContainer.GetComponentsInChildren<PopupButtonElement>();

        for (int i = 0; i < lstPopupButtonElement.Count; i++)
        {
            Destroy(lstPopupButtonElement[i].gameObject);
        }

        lstPopupButtonElement = new List<PopupButtonElement>();

        lstSubMenu = _lstSubMenu;
        for (int i = 0; i < _lstSubMenu.Count; i++)
        {
            PopupButtonElement _buttonElement = Instantiate<PopupButtonElement>(prefabButtonElement, tFieldContainer);

            _buttonElement.SetButtonElement(_lstSubMenu[i].sName, _lstSubMenu[i].action, delegate () { SetSelected(_buttonElement); });

            lstPopupButtonElement.Add(_buttonElement);
        }
        selectedButtonElement = null;
    }
    public void SetSelected(PopupButtonElement _element)
    {
        if (selectedButtonElement != null)
            selectedButtonElement.SetSelectedElement(false);

        selectedButtonElement = _element;

        if (selectedButtonElement)
        {
            selectedButtonElement.SetSelectedElement(true);
        }
        for (int i = 0; i < lstPopupButtonElement.Count; i++)
        {
            if(lstPopupButtonElement[i] == selectedButtonElement)
            {
                iSelectedElement = i;
            }
        }
    }

    public void SelectElementWithInput()
    {
        if (Input.GetAxisRaw("Vertical") > 0 && Input.anyKeyDown)
        {
            if (iSelectedElement > 0)
                SetSelected(lstPopupButtonElement[iSelectedElement - 1]);
            else if (iSelectedElement == 0)
                SetSelected(lstPopupButtonElement[lstPopupButtonElement.Count - 1]);
        }
        else if (Input.GetAxisRaw("Vertical") < 0 && Input.anyKeyDown)
        {
            if (iSelectedElement < lstPopupButtonElement.Count - 1)
                SetSelected(lstPopupButtonElement[iSelectedElement + 1]);
            else if (iSelectedElement == lstPopupButtonElement.Count - 1)
                SetSelected(lstPopupButtonElement[0]);
            else if (iSelectedElement == 0)
                SetSelected(lstPopupButtonElement[1]);
        }
        if (Input.GetButtonDown("Interact"))
        {
            if (selectedButtonElement != null)
            {
                selectedButtonElement.clickAction.Invoke();
                close();
            }
        }
    }
}
