using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public Item item;
   // string sItemContainerID;
    structItem structThisItem;
    public Canvas uiCanvas;
    private void Start()
    {
        #region Non Usable Code
        // item.sID = System.Guid.NewGuid().ToString();
        // sItemContainerID = "Item_C" + gameObject.GetInstanceID().ToString();

        //item.sItemDescription = item.sItemDescription.Replace("&value", item.fEffectValue.ToString());

        //if(item.iQuantity == 0)
        //{
        //    item.iQuantity = 1;
        //}
        //if (item.bIsStackable)
        //{
        //    if(item.iStackLimit == 0)
        //    {
        //        item.iStackLimit = 5;
        //    }
        //}
        #endregion
        CheckIfEquipable();
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
        item.SetQuantity(1); // to make the amount 1
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
    public void CheckIfEquipable()
    {
        if (item.bIsEquipable)
        {
            item.bIsStackable = false;
            item.iQuantity = 1;

            Weapon _weapon = GetComponent<Weapon>();
            DamageTarget _dmgTargetScript = GetComponent<DamageTarget>();
            if (_weapon)
                _weapon.enabled = false;
            if (_dmgTargetScript)
                _dmgTargetScript.enabled = false;
        }
    }
    public void SetItemEquipable()
    {
        Weapon _weapon = GetComponent<Weapon>();
        DamageTarget _dmgTargetScript = GetComponent<DamageTarget>();
        if (_weapon)
            _weapon.enabled = true;
        if (_dmgTargetScript)
        {
            _dmgTargetScript.enabled = true;
            _dmgTargetScript.InitializeStats(this);
        }
        Destroy(uiCanvas.gameObject);
        Destroy(this);
    }
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.blue;
    }
}
