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

    public Item(Item _item)
    {
        sID = _item.sID;
        sItemName = _item.sItemName;
        sItemDescription = _item.sItemDescription;
        eType = _item.eType;
        fEffectValue = _item.fEffectValue;
        fPrice = _item.fPrice;
        itemIcon = _item.itemIcon;
        isStackable = _item.isStackable;
        iAmount = _item.iAmount;
        iStackLimit = _item.iStackLimit;
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
}
