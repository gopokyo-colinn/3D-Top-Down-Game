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

    void Start()
    {
        lstPopupButtonElement = new List<PopupButtonElement>();
        rtTransform = GetComponent<RectTransform>();
        containerAll.gameObject.SetActive(false);
    }
    public override void open()
    {
        base.open();
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

        PopupButtonElement _buttonElement;
        lstSubMenu = _lstSubMenu;
        for (int i = 0; i < _lstSubMenu.Count; i++)
        {
            _buttonElement = Instantiate<PopupButtonElement>(prefabButtonElement, tFieldContainer);
            
            _buttonElement.btnMain.onClick.AddListener(_lstSubMenu[i].action);
            _buttonElement.btnMain.onClick.AddListener(delegate () { close(); });
            _buttonElement.SetButtonName(_lstSubMenu[i].sName);

            lstPopupButtonElement.Add(_buttonElement);
        }
    }
}
