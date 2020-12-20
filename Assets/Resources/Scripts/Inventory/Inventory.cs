using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory 
{
    public int iInventoryLimit = 20;
    public List<Item> lstItems;

    public Inventory(int _size)
    {
        lstItems = new List<Item>();
        iInventoryLimit = _size;
    }

    public void AddItem(Item _item)
    {
        if(lstItems.Count < iInventoryLimit)
        {
            _item.sID = System.Guid.NewGuid().ToString();
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
                lstItems[i].iAmount--;
            }
        }
    }
}
