using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPopup : Popup
{
    PlayerController player;
    Inventory inventory;
    InventorySlot[] inventorySlots;
    private void Start()
    {
        open();
        inventorySlots = GetComponentsInChildren<InventorySlot>();
        close();
        player = GameController.Instance.player;
        inventory = new Inventory();

        inventory = player.GetInventory();
        InitializeInventoryUI();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            
        }
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

        if (inventory.lstItems.Count <= inventory.iInventoryLimit)
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
}
