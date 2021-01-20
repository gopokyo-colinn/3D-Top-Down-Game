using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public Item item;
    structItem structThisItem;
    public Canvas uiCanvas;
    private void Start()
    {
        item.sID = System.Guid.NewGuid().ToString();
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
        InitializeItem();
        uiCanvas.gameObject.SetActive(false);
    }
    public void Update()
    {
        UIActivator();
    }
    public void SetItem(structItem _structItem)
    {
        item = new Item(_structItem);
        item.UpdateAmount(-item.iAmount + 1); // to make the amount 1
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    private void UIActivator()
    {
        Collider[] _hitColliders = Physics.OverlapSphere(transform.position, 3f, LayerMask.GetMask("Player"));

        if(_hitColliders.Length > 0)
        {
            uiCanvas.gameObject.SetActive(true);
        }
        else
        {
            uiCanvas.gameObject.SetActive(false);
        }
    }
    void InitializeItem()
    {
        structThisItem = new structItem();
        structThisItem.sID = item.sID;
        structThisItem.sItemName = item.sItemName;
        structThisItem.sItemDescription = item.sItemDescription;
        structThisItem.eType = item.eType;
        structThisItem.fEffectValue = item.fEffectValue;
        structThisItem.fPrice = item.fPrice;
        structThisItem.itemIcon = item.itemIcon;
        structThisItem.isStackable = item.isStackable;
        structThisItem.iAmount = item.iAmount;
        structThisItem.iStackLimit = item.iStackLimit;

        item.SetItem(structThisItem);
    }
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.blue;
    }
}
