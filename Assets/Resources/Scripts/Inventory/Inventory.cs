﻿using System.Collections;
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
    public void RemoveItemInSlot(Item _item, int _iNumber)
    {
        
         lstItems.Remove(lstItems[_iNumber]);
    }
    public void UpdateItemAmount(Item _item)
    {
        for (int i = 0; i < lstItems.Count; i++)
        {
            if (lstItems[i].sID == _item.sID)
            {
                lstItems[i].SetQuantity(lstItems[i].iQuantity - 1);
            }
        }
    }
    public void UpdateItem(Item _item)
    {
        for (int i = 0; i < lstItems.Count; i++)
        {
            if (lstItems[i].sID == _item.sID)
            {
                lstItems[i] = new Item(_item);
                break;
            }
        }
    }
    public void UpdateItemInSlot(Item _item, int _iNumber)
    {
        lstItems[_iNumber] = new Item(_item);
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

        List<structItem> _savedItemsLst = structInventory.itemsLst;

        for (int i = 0; i < _savedItemsLst.Count; i++)
        {
            Item _newItem = new Item(ItemDatabaseManager.Instance.GetItemByID(_savedItemsLst[i].sID));
            if(_newItem != null)
            {
                _newItem.SetQuantity(_savedItemsLst[i].iQuantity);
                lstItems.Add(_newItem);
            }
        }

    }
}
