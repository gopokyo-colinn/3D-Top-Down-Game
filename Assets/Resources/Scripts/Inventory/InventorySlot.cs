using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;
    public TextMeshProUGUI stackText;
    private Item item;
    Outline outline;
    private void Awake()
    {
        outline = GetComponent<Outline>();
        stackText.text = "";
    }
    public void UpdateSlot(Item _item)
    {
        item = new Item(_item);
        icon.gameObject.SetActive(true);
        icon.sprite = item.GetSprite();
        if(item.iAmount <= 1)
        {
            stackText.text = "";
        }
        else
        {
           stackText.text = item.iAmount.ToString();
        }
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


        if (item.isStackable)
        {
            _subMenu = new structSubMenu();
            _subMenu.sName = "Discard All";
            _subMenu.action = delegate () { ClickDiscardAll(); };
            _lstSubMenu.Add(_subMenu);
            PopupUIManager.Instance.subMenuPopup.openMenu(_lstSubMenu);
        }

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
            if(item.isStackable)
                item.iAmount--;

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
        Transform _droppedItem = Instantiate(ItemsAssets.Instance.GetPrefab(item), _itemDropPosition , Quaternion.identity).transform;

        ItemContainer _newSpawnedItem = _droppedItem.GetComponent<ItemContainer>();

        _newSpawnedItem.SetItem(item);

        if (item.isStackable)
            item.iAmount--;
        Debug.Log(item.iAmount);
        RemoveItem(item);

    }
    public void ClickDiscardAll()
    {
        Debug.Log(item.sItemName + " Discarded");
        for (int i = 0; i < item.iAmount; i++)
        {
            Vector3 _itemDropPosition = GameController.Instance.player.transform.position + new Vector3(Random.Range(-1f, 1f), 1.5f, Random.Range(-1f, 1f));
            Transform _droppedItem = Instantiate(ItemsAssets.Instance.GetPrefab(item), _itemDropPosition, Quaternion.identity).transform;

            ItemContainer _newSpawnedItem = _droppedItem.GetComponent<ItemContainer>();
            _newSpawnedItem.SetItem(item);
        }
        //if (item.isStackable)
        // item.iAmount--;
        item.iAmount = 0;
        RemoveItem(item);
    }
    public void ClickDetails()
    {
        Debug.Log(item.sItemDescription);
    }

    public void RemoveItem(Item _item)
    {
        if (item.isStackable)
        {
            if(_item.iAmount > 0)
            {
                Inventory _updatedUIInventory = GameController.Instance.player.GetInventory();
                _updatedUIInventory.UpdateItemAmount(item); /// aaa hun chlana pena tenu
                //EmptySlot();
                GameController.Instance.player.UpdateInventory(_updatedUIInventory);
            }
            else
            {
                Inventory _updatedUIInventory = GameController.Instance.player.GetInventory();
                _updatedUIInventory.RemoveItem(item);
                EmptySlot();
                GameController.Instance.player.UpdateInventory(_updatedUIInventory);
            }
        }
        else
        {
            Inventory _updatedUIInventory = GameController.Instance.player.GetInventory();
            _updatedUIInventory.RemoveItem(item);
            EmptySlot();
            GameController.Instance.player.UpdateInventory(_updatedUIInventory);
        }
    }
    public void EmptySlot()
    {
        item = null;
        icon.sprite = null;
        icon.gameObject.SetActive(false);
        stackText.text = "";
    }
}
