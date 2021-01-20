using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { HealthPotion, Weapon, QuestItem, Valuable }
[Serializable]
public class Item
{
    [HideInInspector]
    public string sID;
    public string sItemName;
    public string sItemDescription;
    public ItemType eType;
    public float fEffectValue;
    public float fPrice;
    public Sprite itemIcon;
    public bool isStackable;
    public int iAmount = 1;
    public int iStackLimit;

    structItem structThisItem;

    public Item(structItem _structItem)
    {
        structThisItem = new structItem();
        structThisItem = _structItem;

        sID = structThisItem.sID;
        sItemName = structThisItem.sItemName;
        sItemDescription = structThisItem.sItemDescription;
        eType = structThisItem.eType;
        fEffectValue = structThisItem.fEffectValue;
        fPrice = structThisItem.fPrice;
        itemIcon = structThisItem.itemIcon;
        isStackable = structThisItem.isStackable;
        iAmount = structThisItem.iAmount;
        iStackLimit = structThisItem.iStackLimit;
    }
    public void UpdateAmount(int _amount)
    {
        iAmount += _amount;
        structThisItem.iAmount = iAmount;
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
}
