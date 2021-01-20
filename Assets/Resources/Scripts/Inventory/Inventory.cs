using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory 
{
    public int iInventorySize = 20;
    public List<Item> lstItems;

    structInventory structInventory;

    public Inventory(int _iInventoryStartSize)
    {
        structInventory = new structInventory();
        structInventory.iInventorySize = _iInventoryStartSize;
        lstItems = new List<Item>();
        iInventorySize = structInventory.iInventorySize;
    }

    public void AddItem(Item _item)
    {
        if(lstItems.Count < iInventorySize)
        {
            lstItems.Add(_item);
        }
        else
        {
            Debug.Log("Your Inventory is full.");
        }
    }
    public void RemoveItem(Item _item)
    {
        for (int i = 0; i < lstItems.Count; i++)
        {
            if (lstItems[i].sID == _item.sID)
            {
                lstItems.Remove(lstItems[i]);
            }
        }
    }
    public void UpdateItemAmount(Item _item)
    {
        for (int i = 0; i < lstItems.Count; i++)
        {
            if (lstItems[i].sID == _item.sID)
            {
                lstItems[i].UpdateAmount(-1);
            }
        }
    }
    public structInventory GetInventory()
    {
        structInventory.itemsLst = new List<structItem>();

        for (int i = 0; i < lstItems.Count; i++)
        {
            structInventory.itemsLst.Add(lstItems[i].GetItem());
        }
        return structInventory;
    }
    public void SetInventory(structInventory _structInventory)
    {
        structInventory = _structInventory;
        iInventorySize = structInventory.iInventorySize;

        lstItems = new List<Item>();

        for (int i = 0; i < structInventory.itemsLst.Count; i++)
        {
            Item _newItem = new Item(structInventory.itemsLst[i]);
            lstItems.Add(_newItem);
        }

    }
}
