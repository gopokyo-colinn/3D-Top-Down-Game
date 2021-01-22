using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public Item item;
    [SerializeField]
    string sItemContainerID;
    structItem structThisItem;
    public Canvas uiCanvas;
    private void Start()
    {
        // item.sID = System.Guid.NewGuid().ToString();
        sItemContainerID = "Item_C" + gameObject.GetInstanceID().ToString();

        item.sItemDescription = item.sItemDescription.Replace("&value", item.fEffectValue.ToString());

        if(item.iQuantity == 0)
        {
            item.iQuantity = 1;
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
    public void SetItem(Item _item)
    {
        item = new Item(_item);
        item.UpdateQuantity(1); // to make the amount 1
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
        structThisItem.iQuantity = item.iQuantity;

        item.SetItem(structThisItem);
    }
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.blue;
    }
}
