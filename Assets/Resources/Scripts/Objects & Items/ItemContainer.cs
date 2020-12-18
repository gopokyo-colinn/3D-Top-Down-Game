using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public Item item;
    private void Start()
    {
        item.sItemDescription = item.sItemDescription.Replace("&value", item.fEffectValue.ToString());
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
