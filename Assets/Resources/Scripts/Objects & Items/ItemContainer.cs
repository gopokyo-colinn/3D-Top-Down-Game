using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public Item item;
    private void Start()
    {
        item.sItemDescription = item.sItemDescription.Replace("&value", item.fEffectValue.ToString());
        if(item.iAmount == 0)
        {
            item.iAmount = 1;
        }
        if (item.isStackable)
        {
            if(item.iStackLimit == 0)
            {
                item.iStackLimit = 5;
            }
        }
    }
    public void SetItem(Item _item)
    {
        item = new Item(_item);
        item.iAmount = 1;
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
