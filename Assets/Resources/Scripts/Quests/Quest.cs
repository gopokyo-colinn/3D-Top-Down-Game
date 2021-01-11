using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum QuestType { MAINQUEST = 0, SIDEQUEST = 1 }
[Serializable]
public class Quest
{
    public string sQuestTitle;
    [TextArea(2,4)]
    public string sQuestDescription;
    public QuestType eQuestType;
    public string sQuestID;
    public QuestGoal qGoal;

    [HideInInspector]
    public bool bIsActive;
    [HideInInspector]
    public bool bIsCompleted;

    PlayerController player;
    EnemySpawner killQuestEnemySpawner;

    public void Initialize(PlayerController _player)
    {
        player = _player;// FindObjectOfType<PlayerController>();
        if (qGoal.eGoalType == QuestGoalType.KILL)
        {
            killQuestEnemySpawner = KillQuestsManager.Instance.GetCurrentQuestSpawner(sQuestID);
            KillQuestsManager.Instance.InitializeQuestEnemies(sQuestID, qGoal.enemiesToKill);
        }
        if(eQuestType == QuestType.MAINQUEST)
        {
            if (!QuestManager.Instance.dictMainQuests.ContainsKey(sQuestTitle))
            {
                QuestManager.Instance.dictMainQuests.Add(sQuestTitle, this);
            }
        }
        else if (eQuestType == QuestType.SIDEQUEST)
        {
            if (!QuestManager.Instance.dictSideQuests.ContainsKey(sQuestTitle))
            {
                QuestManager.Instance.dictSideQuests.Add(sQuestTitle, this);
            }
        }
    }

    public void Refresh()
    {
        if(!bIsCompleted)
            CheckGoalProgress();
    }

    public void StartCondition()
    {

    }

    public void QuestTrigger()
    {

    }

    public void GiveReward()
    {
        if (qGoal.bIsFinished)
        {
            Debug.Log("Got 10 XP.");
            RemoveQuest();
        }
    }

    public void CheckGoalProgress()
    {
        switch (qGoal.eGoalType)
        {
            case QuestGoalType.DELIVER:
                DeliverQuest();
                break;
            case QuestGoalType.GATHER:
                GatherQuest();
                break;
            case QuestGoalType.KILL:
                KillQuest();
                break;
            case QuestGoalType.GOTOLOCATION:
                GoToLocationQuest();
                break;
        }
    }

    public void RemoveQuest()
    {
        bIsCompleted = true;
        if (!QuestManager.Instance.dictCompletedQuests.ContainsKey(sQuestTitle))
        {
            QuestManager.Instance.dictCompletedQuests.Add(sQuestTitle, this);
        }
        
        if (eQuestType == QuestType.MAINQUEST)
        {
            if (QuestManager.Instance.dictMainQuests.ContainsKey(sQuestTitle))
            {
                QuestManager.Instance.dictMainQuests.Remove(sQuestTitle);
            }
        }
        else
        {
            if (QuestManager.Instance.dictSideQuests.ContainsKey(sQuestTitle))
            {
                QuestManager.Instance.dictSideQuests.Remove(sQuestTitle);
            }
        }
    }

    public void KillQuest()
    {
        if(killQuestEnemySpawner.enemiesLst.Count <= 0)
        {
            qGoal.bIsFinished = true;
            killQuestEnemySpawner.gameObject.SetActive(false);
        }
    }
    public void GoToLocationQuest()
    {
        if ((player.transform.position - qGoal.tLocationToReach.position).sqrMagnitude <= 1f)
        {
            qGoal.bIsFinished = true;
        }
    }
    public void GatherQuest()
    {

    }
    public void DeliverQuest()
    {

    }
}
