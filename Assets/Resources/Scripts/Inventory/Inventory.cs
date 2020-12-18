using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory 
{
    public int iInventoryLimit = 20;
    public List<Item> lstItems;

    public Inventory()
    {
        lstItems = new List<Item>();
    }

    public void AddItem(Item _item)
    {
        if(lstItems.Count <= iInventoryLimit)
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
        if (lstItems.Contains(_item))
        {
            lstItems.Remove(_item);
        }
    }
}
