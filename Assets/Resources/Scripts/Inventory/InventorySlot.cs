using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;
    private Item item;
    Outline outline;
    private void Awake()
    {
        outline = GetComponent<Outline>();
      
    }
    public void UpdateSlot(Item _item)
    {
       item = _item;
       icon.gameObject.SetActive(true);
       icon.sprite = item.GetSprite();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(item != null)
        {
            ClickItem();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        outline.enabled = true;
       // icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outline.enabled = false;
        //icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0.92f);
    }

    public void ClickItem()
    {
        List<structSubMenu> _lstSubMenu = new List<structSubMenu>();
        structSubMenu _subMenu = new structSubMenu();
        _subMenu.sName = "Use";
        _subMenu.action = delegate () { ClickUse(); };
        _lstSubMenu.Add(_subMenu);

        _subMenu = new structSubMenu();
        _subMenu.sName = "Discard";
        _subMenu.action = delegate () { ClickDiscard(); };
        _lstSubMenu.Add(_subMenu);
        PopupUIManager.Instance.subMenuPopup.openMenu(_lstSubMenu);

        _subMenu = new structSubMenu();
        _subMenu.sName = "Details";
        _subMenu.action = delegate () { ClickDetails(); };
        _lstSubMenu.Add(_subMenu);
        PopupUIManager.Instance.subMenuPopup.openMenu(_lstSubMenu);
    }
    public void ClickUse()
    {
        Debug.Log(item.sItemName + " used");
        if (item.UseItem(GameController.Instance.player))
        {
            RemoveItem(item);
        }
        else
        {
            Debug.Log("Can't use this item right now");
        }
    }
    public void ClickDiscard()
    {
        Debug.Log(item.sItemName + " Discarded");
        Vector3 _itemDropPosition = GameController.Instance.player.transform.position + new Vector3(Random.Range(-1f, 1f), 1.5f, Random.Range(-1f, 1f));
        Instantiate<ItemContainer>(ItemsAssets.Instance.GetPrefab(item), _itemDropPosition , Quaternion.identity);
        RemoveItem(item);
    }
    public void ClickDetails()
    {
        Debug.Log(item.sItemDescription);
    }

    public void RemoveItem(Item _item)
    {
        Inventory _updatedUIInventory = GameController.Instance.player.GetInventory();
        _updatedUIInventory.RemoveItem(item);
        EmptySlot();
        GameController.Instance.player.UpdateInventory(_updatedUIInventory);
        //PopupUIManager.Instance.inventoryPopup.UpdateInventoryUI(_updatedUIInventory);
    }
    public void EmptySlot()
    {
        item = null;
        icon.sprite = null;
        icon.gameObject.SetActive(false);
    }
}
