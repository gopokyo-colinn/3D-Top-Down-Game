using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryPopup : Popup
{
    public Transform detailsContainer;
    public TextMeshProUGUI txtDetailItemName;
    public TextMeshProUGUI txtDetailItemDescription;

    public Sprite lockImage;

    PlayerController player;
    Inventory inventory;
    InventorySlot[] inventorySlots;
    private void Start()
    {
        open();
        player = GameController.Instance.player;

        inventory = player.GetInventory();

        inventorySlots = GetComponentsInChildren<InventorySlot>();

        InitializeInventoryUI();

        close();
    }

    public override void open()
    {
        base.open();
        PopupUIManager.Instance.menuBarPopup.open();
    }
    public override void close()
    {
        base.close();
        PopupUIManager.Instance.menuBarPopup.close();
    }

    public void UpdateInventoryUI(Inventory _updatedInventory)
    {
        inventory = _updatedInventory;
        InitializeInventoryUI();
    }
    public void InitializeInventoryUI()
    {
        // foreach (InventorySlot inSlot in inventorySlots)
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].EmptySlot();
        }

        LockSlots();

        if (inventory.lstItems.Count <= inventory.iInventorySize)
        {
            for (int i = 0; i < inventory.lstItems.Count; i++)
            {
                inventorySlots[i].UpdateSlot(inventory.lstItems[i]);
            }
        }
    }
    public Inventory GetInventory()
    {
        return inventory;
    }

    public void LockSlots()
    {
        for (int i = inventory.iInventorySize; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].icon.gameObject.SetActive(true);
            inventorySlots[i].icon.sprite = lockImage;
        }
    }
}
