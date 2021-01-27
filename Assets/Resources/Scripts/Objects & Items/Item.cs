using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum ItemType { HealthPotion = 0, PrimaryWeapon = 1, SecondaryWeapon = 2, QuestItem = 3, Valuable = 4, Shield = 5 }
[Serializable][CreateAssetMenu(fileName ="New Item", menuName = "Assets/Item")]
public class Item: ScriptableObject
{
    //[HideInInspector]
#if UNITY_EDITOR
    [UniqueID]
#endif
    public string sID;
    public string sItemName;
    [TextArea(3,5)]
    public string sItemDescription;
    public ItemType eType;
    public float fEffectValue;
    [Tooltip("Only Applicable for Weapons")]
    public float fWeaponKnockback;
    public float fPrice;
    public Sprite itemIcon;
    public bool bIsStackable;
    public int iQuantity = 1;
    public int iStackLimit;
    public bool bIsEquipable;
    public bool bIsEquipped { get; private set; }
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
        fWeaponKnockback = _item.fWeaponKnockback;
        fPrice = _item.fPrice;
        itemIcon = _item.itemIcon;
        bIsStackable = _item.bIsStackable;
        iStackLimit = _item.iStackLimit;
        prefabItem = _item.prefabItem;
        bIsEquipable = _item.bIsEquipable;
        bIsEquipped = _item.bIsEquipped;

        structThisItem = new structItem();
        structThisItem.sID = sID;
        structThisItem.iQuantity = iQuantity;
    }
    public void SetQuantity(int _amount)
    {
        iQuantity = _amount;
        structThisItem.iQuantity = iQuantity;
    }
    public bool UseItem(PlayerController _player)
    {
        switch (eType)
        {
            case ItemType.HealthPotion:
                 return UseHealthPotion(_player);              
            case ItemType.PrimaryWeapon:
                return EquipPrimaryWeapon(_player);
            case ItemType.Shield:
                return EquipShield(_player);
            default:
                return false;
        }
    }
    public float ItemUseValue()
    {
        return fEffectValue;
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
    public void SetEquipped(bool _bEquipped)
    {
        bIsEquipped = _bEquipped;
    }
    
    public ItemContainer GetItemPrefab()
    {
        return prefabItem.GetComponent<ItemContainer>();
    }

    /// /Items Use Functions
    public bool UseHealthPotion(PlayerController _player)
    {
        if (_player.fCurrentHitPoints < _player.fMaxHitPoints)
        {
            _player.fCurrentHitPoints += (int)fEffectValue;
            _player.HealthCheck();
            _player.OnReciveDamageUI.Invoke();
            return true;
        }
        return false;
    }
    public bool EquipPrimaryWeapon(PlayerController _player)
    {
        if (!bIsEquipped)
        {
            if (!_player.IsAttacking())
            {
                bIsEquipped = true;
                _player.SetPrimaryWeaponEquipped(this);
                return true;
            }
        }
        else
        {
            if (!_player.IsAttacking())
            {
                bIsEquipped = false;
                _player.SetPrimaryWeaponEquipped(null);
                return true;
            }
        }
        return false;
    }
    public bool EquipSecondaryWeapon(PlayerController _player)
    {
        if (bIsEquipped)
        {
            bIsEquipped = false;
            _player.SetSecondaryWeaponEquipped(this);
            return true;
        }
        else
        {
            bIsEquipped = true;
            _player.SetSecondaryWeaponEquipped(this);
            return true;
        }
    }
    public bool EquipShield(PlayerController _player)
    {
        if (!_player.IsSwordEquipped())
        {
            _player.SetShieldEquipped(this);
            return true;
        }
        return false;
    }
}
