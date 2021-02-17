using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum QuestGoalType { KILL = 0, GATHER = 1, DELIVER = 2, GO_TO_LOCATION = 3, GO_TO_NPC = 4, RETURN_TO_QUEST_GIVER}

[Serializable]
public class QuestGoal 
{
    public QuestGoalType eGoalType;
    public string sGoalObjective;
    public Transform tLocationToReach;
    [Tooltip("Set Spawn Location for these Enemies in KillQuestManager")]
    public EnemySpawner enemiesSpawner;
    public NPCAssignedQuestGoal taskNPC;
    public ItemContainer itemToGatherOrDeliver;


    [SerializeField]
    private bool bIsActive;
    [SerializeField]
    private bool bIsFinished;

    public void InitializeGoal(string _sID)
    {
        if (bIsActive && !bIsFinished)
        {
            switch (eGoalType)
            {
                case QuestGoalType.GO_TO_NPC:
                    taskNPC.SetQuestID(_sID);
                    taskNPC.SetIsFinished(bIsFinished);
                    break;
                case QuestGoalType.KILL:
                    enemiesSpawner.SetID(_sID);
                    enemiesSpawner.SetActive(true);
                    enemiesSpawner.SpawnEnemies();
                    break;
                case QuestGoalType.GATHER:
                    itemToGatherOrDeliver.gameObject.SetActive(true);
                    break;
                case QuestGoalType.DELIVER:
                    taskNPC.SetQuestID(_sID); // this task npc is to whom you are delivering the item
                    taskNPC.SetIsFinished(bIsFinished);
                    Item _itemToAdd = new Item(itemToGatherOrDeliver.item);
                    _itemToAdd.eType = ItemType.QuestItem;
                    if(!PlayerController.Instance.GetInventory().HasQuestItem(_itemToAdd))
                        PlayerController.Instance.GetInventory().AddItem(_itemToAdd);
                    break;
            }
        }
    }

    public bool GetIsFinished()
    {
        return bIsFinished;
    }
    public bool GetIsActive()
    {
        return bIsActive;
    }
    public void SetIsFinished(bool _bIsFinished)
    {
        bIsFinished = _bIsFinished;
    }
    public void SetIsActive(bool _bIsActive)
    {
        bIsActive = _bIsActive;
    }
}
