using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SaveData 
{
    public structGameSavePlayer playerSaveData;

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
    public void LoadFromJson(string _json)
    {
        JsonUtility.FromJsonOverwrite(_json, this);
    }

}
[Serializable]
public struct structGameSavePlayer
{
    public float fCurrentHitPoints;
    public float fCurrentStamina;
    public float[] tPosition;
    public float[] tRotation;
    public structInventory playerInventory;
}
[Serializable]
public struct structInventory
{
    public int iInventorySize;
    public List<structItem> itemsLst;
}
[Serializable]
public struct structItem
{
    public string sID;
    public string sItemName;
    public string sItemDescription;
    public ItemType eType;
    public float fEffectValue;
    public float fPrice;
    public Sprite itemIcon;
    public bool isStackable;
    public int iAmount;
    public int iStackLimit;
}

