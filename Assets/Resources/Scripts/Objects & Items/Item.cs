using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum ItemType { HealthPotion, Weapon, QuestItem, Valuable }
[Serializable][CreateAssetMenu(fileName ="New Item", menuName = "Assets/Item")]
public class Item: ScriptableObject
{
    //[HideInInspector]
#if UNITY_EDITOR
    [UniqueID]
#endif
    public string sID;
    public string sItemName;
    public string sItemDescription;
    public ItemType eType;
    public float fEffectValue;
    public float fPrice;
    public Sprite itemIcon;
    public bool isStackable;
    public int iQuantity = 1;
    public int iStackLimit;

    structItem structThisItem;

    public GameObject prefabItem;

    public Item(Item _item)
    {
        sID = _item.sID;
        iQuantity = _item.iQuantity;
        sItemName = _item.sItemName;
        sItemDescription = _item.sItemDescription;
        eType = _item.eType;
        fEffectValue = _item.fEffectValue;
        fPrice = _item.fPrice;
        itemIcon = _item.itemIcon;
        isStackable = _item.isStackable;
        iStackLimit = _item.iStackLimit;
        prefabItem = _item.prefabItem;

        structThisItem = new structItem();
        structThisItem.sID = sID;
        structThisItem.iQuantity = iQuantity;
    }
    public void UpdateQuantity(int _amount)
    {
        iQuantity = _amount;
        structThisItem.iQuantity = iQuantity;
    }
    public bool UseItem(PlayerController _player)
    {
        switch (eType)
        {
            case ItemType.HealthPotion:
                if(_player.fCurrentHitPoints < _player.fMaxHitPoints)
                {
                    _player.fCurrentHitPoints += (int)fEffectValue;
                    _player.HealthCheck();
                    _player.OnReciveDamageUI.Invoke();
                    return true;
                }
                return false;
            default:
                return false;

        }
    }
    public float ItemUseValue()
    {
        return fEffectValue;
    }
    
    public void EquipItem()
    {
        Debug.Log("SwordEquipped");
    }

    public Sprite GetSprite()
    {
        return itemIcon;
    }
    public structItem GetItem()
    {
        return structThisItem;
    }
    public void SetItem(structItem _structItem)
    {
        structThisItem = _structItem;
    }
    public ItemContainer GetItemPrefab()
    {
        return prefabItem.GetComponent<ItemContainer>();
    }
}
