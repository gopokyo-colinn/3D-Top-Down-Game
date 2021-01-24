using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    public Image icon;
    public Image imgSelected;
    public TextMeshProUGUI stackText;
    public RectTransform tMenuPosition;
    private Item item;
    private bool bSelected;
    private InventoryPopup inventoryPopup;
    private void Start()
    {
        inventoryPopup = PopupUIManager.Instance.inventoryPopup;
    }
    public void UpdateSlot(Item _item)
    {
        item = new Item(_item);

        icon.gameObject.SetActive(true);
        icon.sprite = item.GetSprite();

        if (item.iQuantity <= 1)
        {
            stackText.text = "";
        }
        else
        {
           stackText.text = item.iQuantity.ToString();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        OpenItemMenu();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryPopup.SetSelected(this);
    }
    public void OpenItemMenu()
    {
        if(item != null)
        {
            inventoryPopup.SetItemMenuOpenBool(true);
            List<structSubMenu> _lstSubMenu = new List<structSubMenu>();
            structSubMenu _subMenu = new structSubMenu();
            _subMenu.sName = "Use";
            _subMenu.action = delegate () { ClickUse(); };
            _lstSubMenu.Add(_subMenu);

            _subMenu = new structSubMenu();
            _subMenu.sName = "Discard";
            _subMenu.action = delegate () { ClickDiscard(); };
            _lstSubMenu.Add(_subMenu);

            if (item.isStackable)
            {
                _subMenu = new structSubMenu();
                _subMenu.sName = "Discard All";
                _subMenu.action = delegate () { ClickDiscardAll(); };
                _lstSubMenu.Add(_subMenu);
            }

            _subMenu = new structSubMenu();
            _subMenu.sName = "Cancel";
            _subMenu.action = delegate () { ClickCancel(); };
            _lstSubMenu.Add(_subMenu);
            PopupUIManager.Instance.subMenuPopup.openMenu(_lstSubMenu, tMenuPosition.position);
        }
    }
    public void ClickUse()
    {
        if (item.UseItem(PlayerController.Instance))
        {
            Debug.Log(item.sItemName + " used");
            if (item.isStackable)
                item.UpdateQuantity(item.iQuantity - 1);

            RemoveItem(item);
        }
        else
        {
            PopupUIManager.Instance.msgBoxPopup.ShowTextMessage("Can't use this item right now..... ");
        }
        inventoryPopup.SetItemMenuOpenBool(false);
    }
    public void ClickDiscard()
    {
        Debug.Log(item.sItemName + " Discarded");
        Vector3 _itemDropPosition = PlayerController.Instance.transform.position + new Vector3(Random.Range(-1f, 1f), 1.5f, Random.Range(-1f, 1f));
        ItemContainer _newDroppedItem = Instantiate(item.GetItemPrefab(), _itemDropPosition , Quaternion.identity);

        _newDroppedItem.SetItem(item);

        if (item.isStackable)
        {
            item.UpdateQuantity(item.iQuantity - 1);
        }
        RemoveItem(item);
        inventoryPopup.SetItemMenuOpenBool(false);
    }
    public void ClickDiscardAll()
    {
        Debug.Log(item.sItemName + " Discarded");
        for (int i = 0; i < item.iQuantity; i++)
        {
            Vector3 _itemDropPosition = PlayerController.Instance.transform.position + new Vector3(Random.Range(-1f, 1f), 1.5f, Random.Range(-1f, 1f));
            ItemContainer _newDroppedItem = Instantiate(item.GetItemPrefab(), _itemDropPosition, Quaternion.identity);

            _newDroppedItem.SetItem(item);
        }
        //if (item.isStackable)
        // item.iAmount--;
        item.UpdateQuantity(0); // that will make it equal to 0
        RemoveItem(item);
        inventoryPopup.SetItemMenuOpenBool(false);
    }
    public void ClickCancel()
    {
        inventoryPopup.SetItemMenuOpenBool(false);
    }
    public void RemoveItem(Item _item)
    {
        if (item.isStackable)
        {
            if(_item.iQuantity > 0)
            {
                Inventory _updatedUIInventory = PlayerController.Instance.GetInventory();
                _updatedUIInventory.UpdateItemAmount(item); /// aaa hun chlana pena tenu
                //EmptySlot();
                PlayerController.Instance.UpdateInventory(_updatedUIInventory);
            }
            else
            {
                Inventory _updatedUIInventory = PlayerController.Instance.GetInventory();
                _updatedUIInventory.RemoveItem(item);
                EmptySlot();
                PlayerController.Instance.UpdateInventory(_updatedUIInventory);
            }
        }
        else
        {
            Inventory _updatedUIInventory = PlayerController.Instance.GetInventory();
            _updatedUIInventory.RemoveItem(item);
            EmptySlot();
            PlayerController.Instance.UpdateInventory(_updatedUIInventory);
        }
    }
    public void EmptySlot()
    {
        item = null;
        icon.sprite = null;
        icon.gameObject.SetActive(false);
        stackText.text = "";
    }
    public void SetSelectedElement(bool _bSelected)
    {
        bSelected = _bSelected;
        imgSelected.gameObject.SetActive(bSelected);
        SetItemDetails();
    }
    void SetItemDetails()
    {
        if(inventoryPopup != null)
        {
            if (item != null)
            {
                inventoryPopup.txtDetailItemName.text = item.sItemName;
                inventoryPopup.txtDetailItemDescription.text = item.sItemDescription;
            }
            else
            {
                inventoryPopup.txtDetailItemName.text = "No Item Selected";
                inventoryPopup.txtDetailItemDescription.text = "";
            }
        }
    }
}
