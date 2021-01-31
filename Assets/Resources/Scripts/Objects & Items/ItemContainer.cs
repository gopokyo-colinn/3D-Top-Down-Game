using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public Item item;
   // string sItemContainerID;
    structItem structThisItem;
    public Canvas uiCanvas;
    Rigidbody rb;

    PlayerController player;
    private void Start()
    {
        CheckIfEquipable();
        InitializeItem();
        
        uiCanvas.gameObject.SetActive(false);
        player = PlayerController.Instance;
        gameObject.layer = LayerMask.NameToLayer("Item");
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
        //Collider[] _hitColliders = Physics.OverlapSphere(transform.position, 3f, LayerMask.GetMask("Player"));

       // if (_hitColliders.Length > 0)
        if((transform.position - player.transform.position).sqrMagnitude < 12)
        {
            uiCanvas.gameObject.SetActive(true);
        }
        else
        {
            uiCanvas.gameObject.SetActive(false);
        }
        uiCanvas.GetComponent<RectTransform>().rotation = Quaternion.Euler(45, 180, 0);//startRotation;
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

            rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
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
        Destroy(GetComponent<Rigidbody>());
        Destroy(uiCanvas.gameObject);
        Destroy(this);
    }
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.blue;
    }
}
