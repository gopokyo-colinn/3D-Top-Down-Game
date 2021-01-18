using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum QuestType { MAINQUEST = 0, SIDEQUEST = 1 }
[Serializable]
public class Quest
{
    public string sQuestTitle;
    [TextArea(2, 4)]
    public string sQuestDescription;
    public QuestType eQuestType;
    public string sQuestID;
    public bool bQuestInAnyOrder;
    // public QuestGoal qGoal;
   
    private bool bAllGoalsCompleted = false;
    public QuestGoal[] qGoals;

    [HideInInspector]
    public bool bIsActive;
    private bool bIsCompleted;

    public string sRewards; /// TODO: Replace this with type of reward such as GOLD, EXP, ITEMS. Make it a array to choose a number of rewards.

    PlayerController player;

    public void Initialize(PlayerController _player)
    {
        player = _player;
        InitializeKillGoals();
        AddQuestToManager();
        ActivateNextGoal();   
    }

    public void Refresh()
    {
        if (!bIsCompleted)
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
        if (bAllGoalsCompleted)
        {
            Debug.Log(sRewards);
            RemoveQuest();
        }
    }

    public void CheckGoalProgress()
    {
        foreach (var qGoal in qGoals)
        {
            if (!qGoal.bIsFinished)
            {
                switch (qGoal.eGoalType)
                {
                    case QuestGoalType.DELIVER:
                        DeliverQuest(qGoal);
                        break;
                    case QuestGoalType.GATHER:
                        GatherQuest(qGoal);
                        break;
                    case QuestGoalType.KILL:
                        KillQuest(qGoal);
                        break;
                    case QuestGoalType.GOTOLOCATION:
                        GoToLocationQuest(qGoal);
                        break;

                }
            }
        }
    }
    public bool CheckQuestProgress()
    {
        for (int i = 0; i < qGoals.Length; i++)
        {
            if (!qGoals[i].bIsFinished)
            {
                bAllGoalsCompleted = false;
                bIsActive = false;
                return bAllGoalsCompleted;
            }
        }
        bAllGoalsCompleted = true;
        return bAllGoalsCompleted;
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

    public void KillQuest(QuestGoal _qGoal)
    {
        if (bQuestInAnyOrder)
        {
            if (_qGoal.enemiesSpawner.CheckIfAllEnemiesDead())
            {
                _qGoal.enemiesSpawner.SetActive(false);
                _qGoal.bIsFinished = true;
                PopupUIManager.Instance.msgBoxPopup.SendTextMessage("Quest Updated...", 1);
            }
        }
        else
        {
            if (_qGoal.bIsActive)
            {
                if (_qGoal.enemiesSpawner.CheckIfAllEnemiesDead())
                {
                    _qGoal.enemiesSpawner.SetActive(false);
                    _qGoal.bIsFinished = true;
                    PopupUIManager.Instance.msgBoxPopup.SendTextMessage("Quest Updated...", 1);
                    ActivateNextGoal();
                }
            }
        }
    }
    public void GoToLocationQuest(QuestGoal _qGoal)
    {
        if (bQuestInAnyOrder)
        {
            if ((player.transform.position - _qGoal.tLocationToReach.position).sqrMagnitude <= 1f)
            {
                _qGoal.bIsFinished = true;
                PopupUIManager.Instance.msgBoxPopup.SendTextMessage("Quest Updated...", 1);
            }
        }
        else
        {
            if (_qGoal.bIsActive)
            {
                if ((player.transform.position - _qGoal.tLocationToReach.position).sqrMagnitude <= 1f)
                {
                    _qGoal.bIsFinished = true;
                    PopupUIManager.Instance.msgBoxPopup.SendTextMessage("Quest Updated...", 1);
                    ActivateNextGoal();
                }
            }
        }

    }
    public void GoToNPCCompleted(QuestGoal _qGoal, bool _isCompleted) // THis will be directly used by the NPC's
    {
        if (bQuestInAnyOrder)
        {
            if (_isCompleted)
            {
                if(!_qGoal.bIsFinished) // this is to show the popup message only once
                    PopupUIManager.Instance.msgBoxPopup.SendTextMessage("Quest Updated...", 1);
                _qGoal.bIsFinished = true;
            }
        }
        else
        {
            if (_isCompleted)
            {
                if (!_qGoal.bIsFinished) // this is to show the popup message only once
                    PopupUIManager.Instance.msgBoxPopup.SendTextMessage("Quest Updated...", 1);
                _qGoal.bIsFinished = true;
                ActivateNextGoal();
            }
        }

    }
    public void GatherQuest(QuestGoal _qGoal)
    {

    }// Not yet Done
    public void DeliverQuest(QuestGoal _qGoal)
    {

    } // Not Yet Done
    public bool AllGoalsCompleted()
    {
        return bAllGoalsCompleted;
    }
    public void AddQuestToManager()
    {
        if (eQuestType == QuestType.MAINQUEST)
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
    public void InitializeKillGoals()
    {
        // Right now only kill objectives is needed to initialize
        for (int i = 0; i < qGoals.Length; i++)
        {
            if (qGoals[i].eGoalType == QuestGoalType.KILL)
            {
                if (bQuestInAnyOrder)
                {
                    qGoals[i].enemiesSpawner.SetID(sQuestID);
                    qGoals[i].enemiesSpawner.SetActive(true);
                    qGoals[i].enemiesSpawner.SpawnEnemies();
                }
                else
                {
                    if (qGoals[i].bIsActive)
                    {
                        qGoals[i].enemiesSpawner.SetID(sQuestID);
                        qGoals[i].enemiesSpawner.SetActive(true);
                        qGoals[i].enemiesSpawner.SpawnEnemies();
                    }
                }
            }
        }

    }
    public void ActivateNextGoal()
    {
        if (bQuestInAnyOrder)
        {
            for (int i = 0; i < qGoals.Length; i++)
            {
                qGoals[i].bIsActive = true;
            }
        }
        else
        {
            for (int i = 0; i < qGoals.Length; i++)
            {
                if (!qGoals[i].bIsActive)
                {
                    qGoals[i].bIsActive = true;
                    if(qGoals[i].eGoalType == QuestGoalType.KILL)
                    {
                        InitializeKillGoals();
                    }
                    break;
                }
            }
        }
    }
    public bool IsCompleted()
    {
        return bIsCompleted;
    }
}