using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum QuestType { MAINQUEST = 0, SIDEQUEST = 1 }
[Serializable]
public class Quest
{
    [UniqueID]
    public string sQuestID;
    public string sQuestTitle;
    [TextArea(2, 4)]
    public string sQuestDescription;
    public QuestType eQuestType;
    public bool bQuestInAnyOrder;
    // public QuestGoal qGoal;
   
    private bool bAllGoalsCompleted = false;
    public QuestGoal[] qGoals;

    [SerializeField]
    private bool bIsActive;
    [SerializeField]
    private bool bIsCompleted;

    public string sRewards; //TODO: Replace this with type of reward such as GOLD, EXP, ITEMS. Make it a array to choose a number of rewards.\\

    PlayerController player;

    public void Initialize()
    {
        if (!GetQuestCompleted())
        {
            player = GameController.Instance.player;
            SetQuestActive(true);
            AddQuestToManager();
            ActivateNextGoal();
        }
        else
        {
            RemoveQuest();
        }
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
            if (!qGoal.GetIsFinished())
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
            if (!qGoals[i].GetIsFinished())
            {
                bAllGoalsCompleted = false;
                //bIsActive = false;
                return bAllGoalsCompleted;
            }
        }
        bAllGoalsCompleted = true;
        return bAllGoalsCompleted;
    }

    public void RemoveQuest()
    {
        bIsCompleted = true;

        if (!QuestManager.Instance.dictCompletedQuests.ContainsKey(sQuestID))
        {
            QuestManager.Instance.dictCompletedQuests.Add(sQuestID, this);
        }

        if (eQuestType == QuestType.MAINQUEST)
        {
            if (QuestManager.Instance.dictMainQuests.ContainsKey(sQuestID))
            {
                QuestManager.Instance.dictMainQuests.Remove(sQuestID);
            }
        }
        else
        {
            if (QuestManager.Instance.dictSideQuests.ContainsKey(sQuestID))
            {
                QuestManager.Instance.dictSideQuests.Remove(sQuestID);
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
                _qGoal.SetIsFinished(true);
                PopupUIManager.Instance.msgBoxPopup.SendTextMessage("Quest Updated...", 1);
            }
        }
        else
        {
            if (_qGoal.GetIsActive())
            {
                if (_qGoal.enemiesSpawner.CheckIfAllEnemiesDead())
                {
                    _qGoal.enemiesSpawner.SetActive(false);
                    _qGoal.SetIsFinished(true);
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
                _qGoal.SetIsFinished(true);
                PopupUIManager.Instance.msgBoxPopup.SendTextMessage("Quest Updated...", 1);
            }
        }
        else
        {
            if (_qGoal.GetIsActive())
            {
                if ((player.transform.position - _qGoal.tLocationToReach.position).sqrMagnitude <= 1f)
                {
                    _qGoal.SetIsFinished(true);
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
                if(!_qGoal.GetIsFinished()) // this is to show the popup message only once
                    PopupUIManager.Instance.msgBoxPopup.SendTextMessage("Quest Updated...", 1);
                _qGoal.SetIsFinished(true);
            }
        }
        else
        {
            if (_isCompleted)
            {
                if (!_qGoal.GetIsFinished()) // this is to show the popup message only once
                    PopupUIManager.Instance.msgBoxPopup.SendTextMessage("Quest Updated...", 1);
                _qGoal.SetIsFinished(true);
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
            if (!QuestManager.Instance.dictMainQuests.ContainsKey(sQuestID))
            {
                QuestManager.Instance.dictMainQuests.Add(sQuestID, this);
            }
        }
        else if (eQuestType == QuestType.SIDEQUEST)
        {
            if (!QuestManager.Instance.dictSideQuests.ContainsKey(sQuestID))
            {
                QuestManager.Instance.dictSideQuests.Add(sQuestID, this);
            }
        }
    }
   
    public void ActivateNextGoal()
    {
        if (bQuestInAnyOrder)
        {
            for (int i = 0; i < qGoals.Length; i++)
            {
                qGoals[i].SetIsActive(true);
                qGoals[i].InitializeGoal(sQuestID);
            }
        }
        else
        {
            for (int i = 0; i < qGoals.Length; i++)
            {
                if (!qGoals[i].GetIsActive())
                {
                    qGoals[i].SetIsActive(true);
                    qGoals[i].InitializeGoal(sQuestID);
                    break;
                }
                else
                {
                    if (qGoals[i].GetIsFinished())
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
    public bool GetQuestCompleted()
    {
        return bIsCompleted;
    }
    public bool GetQuestActive()
    {
        return bIsActive;
    }
    public void SetQuestCompleted(bool _bIsComplete)
    {
        bIsCompleted = _bIsComplete;
    }
    public void SetQuestActive(bool _bIsActive)
    {
        bIsActive = _bIsActive;
    }
}