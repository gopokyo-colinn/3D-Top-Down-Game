using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { HealthPotion, Weapon, QuestItem, Valuable }
[Serializable]
public class Item
{
    public string sItemName;
    public string sItemDescription;
    public ItemType eType;
    public float fEffectValue;
    public float fPrice;
    public Sprite itemIcon;

    public bool UseItem(PlayerController _player)
    {
        switch (eType)
        {
            case ItemType.HealthPotion:
                if(_player.iCurrentHitPoints < _player.iMaxHitPoints)
                {
                    _player.iCurrentHitPoints += (int)fEffectValue;
                    _player.HealthCheck();
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
