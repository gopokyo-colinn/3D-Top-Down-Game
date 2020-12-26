using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public Item item;
    public Canvas uiCanvas;
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
        uiCanvas.gameObject.SetActive(false);
    }
    public void Update()
    {
        UIActivator();
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
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.blue;
    }
}
