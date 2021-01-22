using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum QuestGoalType { KILL = 0, GATHER = 1, DELIVER = 2, GOTOLOCATION = 3, GOTONPC = 4}

[Serializable]
public class QuestGoal 
{
    public QuestGoalType eGoalType;
    public string sGoalObjective;
    public Transform tLocationToReach;
    [Tooltip("Set Spawn Location for these Enemies in KillQuestManager")]
    public EnemySpawner enemiesSpawner;
    public NPCAssignedQuestGoal taskNPC;
    [SerializeField]
    private bool bIsActive;
    [SerializeField]
    private bool bIsFinished;

    public void InitializeGoal(string _sID)
    {
        if(eGoalType == QuestGoalType.GOTONPC)
        {
            taskNPC.SetQuestID(_sID);
        }
        else if(eGoalType == QuestGoalType.KILL)
        {
            enemiesSpawner.SetID(_sID);
            enemiesSpawner.SetActive(true);
            enemiesSpawner.SpawnEnemies();
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
