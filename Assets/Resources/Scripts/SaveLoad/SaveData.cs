using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SaveData 
{
    public structGameSavePlayer playerSaveData;
    public structQuestsManager questsSavedData;

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
/// <summary>
/// Inventory Structs-------------------------------------------------------------------------------------
/// </summary>
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
    public int iQuantity;
}
/// <summary>
/// Quest Structs-------------------------------------------------------------------------------------------
/// </summary>
[Serializable]
public struct structQuestsManager
{
    public List<structQuest> activeMainQuestsLst;
    public List<structQuest> activeSideQuestsLst;
    public List<structQuest> allCompletedQuestsLst;
}
[Serializable]
public struct structQuest
{
    public string sQuestID;
    public bool bIsActive;
    public bool bIsCompleted;
    public List<structQuestGoal> qGoalsLst;
}
[Serializable]
public struct structQuestGoal
{
    public bool bIsActive;
    public bool bIsFinished;
}


