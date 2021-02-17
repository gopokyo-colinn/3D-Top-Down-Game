using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum QuestType { MAINQUEST = 0, SIDEQUEST = 1 }
[Serializable]
public class Quest
{
    const string sQuestUpdated = "Quest Updated...";
    const string sMainQuestAdded = "New Main Quest Added...";
    const string sSideQuestAdded = "New Side Quest Added...";
    const string sQuestCompleted = "Quest Completed...";

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
            player = PlayerController.Instance;
            SetQuestActive(true);
            AddQuestToManager(); // Quest Added Popup Msg is in here
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
            PopupUIManager.Instance.msgBoxPopup.ShowMessageAfterDialog(sQuestCompleted + " \n" + sRewards, 1);
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
                    case QuestGoalType.GATHER:
                        GatherItemGoal(qGoal);
                        break;
                    case QuestGoalType.KILL:
                        KillEnemiesGoal(qGoal);
                        break;
                    case QuestGoalType.GO_TO_LOCATION:
                        GoToLocationGoal(qGoal);
                        break;
                }
                if (qGoal.GetIsFinished())
                {
                    PopupUIManager.Instance.msgBoxPopup.ShowMessageAfterDialog(sQuestUpdated, 1);
                }
            }
        }
    }
    public bool CheckQuestProgress()
    {
        for (int i = 0; i < qGoals.Length; i++)
        {
            if(qGoals[i].eGoalType == QuestGoalType.RETURN_TO_QUEST_GIVER)
            {
                qGoals[i].SetIsFinished(true);
            }
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

    public void KillEnemiesGoal(QuestGoal _qGoal)
    {
        if (bQuestInAnyOrder)
        {
            if (_qGoal.enemiesSpawner.CheckIfAllEnemiesDead())
            {
                _qGoal.enemiesSpawner.SetActive(false);
                _qGoal.SetIsFinished(true);
                //PopupUIManager.Instance.msgBoxPopup.ShowTextMessage("Quest Updated...", 1);
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
                    //PopupUIManager.Instance.msgBoxPopup.ShowTextMessage("Quest Updated...", 1);
                    ActivateNextGoal();
                }
            }
        }
    }
    public void GoToLocationGoal(QuestGoal _qGoal)
    {
        if (bQuestInAnyOrder)
        {
            if ((player.transform.position - _qGoal.tLocationToReach.position).sqrMagnitude <= 1f)
            {
                _qGoal.SetIsFinished(true);
                //PopupUIManager.Instance.msgBoxPopup.ShowTextMessage("Quest Updated...", 1);
            }
        }
        else
        {
            if (_qGoal.GetIsActive())
            {
                if ((player.transform.position - _qGoal.tLocationToReach.position).sqrMagnitude <= 1f)
                {
                    _qGoal.SetIsFinished(true);
                   // PopupUIManager.Instance.msgBoxPopup.ShowTextMessage("Quest Updated...", 1);
                    ActivateNextGoal();
                }
            }
        }
    }
    public void SetGoToNPCGoal(QuestGoal _qGoal, bool _isCompleted) // This will be directly used by the NPC's
    {
        if (bQuestInAnyOrder)
        {
            if (_isCompleted)
            {
                if(!_qGoal.GetIsFinished()) // this is to show the popup message only once
                    PopupUIManager.Instance.msgBoxPopup.ShowMessageAfterDialog(sQuestUpdated, 1);
                _qGoal.SetIsFinished(_isCompleted);
            }
        }
        else
        {
            if (_isCompleted)
            {
                if (!_qGoal.GetIsFinished()) // this is to show the popup message only once
                    PopupUIManager.Instance.msgBoxPopup.ShowMessageAfterDialog(sQuestUpdated, 1);
                _qGoal.SetIsFinished(_isCompleted);
                ActivateNextGoal();
            }
        }
    }
    public void GatherItemGoal(QuestGoal _qGoal)
    {
        if (bQuestInAnyOrder)
        {
            if (player.GetInventory().HasItem(_qGoal.itemToGatherOrDeliver.item.sID))
            {
                _qGoal.SetIsFinished(true);
            }
        }
        else
        {
            if (_qGoal.GetIsActive())
            {
                if (player.GetInventory().HasItem(_qGoal.itemToGatherOrDeliver.item.sID))
                {
                    _qGoal.SetIsFinished(true);
                    ActivateNextGoal();
                }
            }
        }
    }// Not yet Done
    public void DeliverItemGoal(QuestGoal _qGoal, bool _bIsCompleted)
    {
        if (bQuestInAnyOrder)
        {
            if (_bIsCompleted)
            {
                if (!_qGoal.GetIsFinished()) // this is to show the popup message only once
                {
                    Item _itemToRemove = new Item(_qGoal.itemToGatherOrDeliver.item);
                    _itemToRemove.eType = ItemType.QuestItem;
                    player.GetInventory().RemoveQuestItem(_itemToRemove);
                    PopupUIManager.Instance.msgBoxPopup.ShowMessageAfterDialog(sQuestUpdated, 1);
                }
                _qGoal.SetIsFinished(_bIsCompleted);
            }
        }
        else
        {
            if (_bIsCompleted)
            {
                if (!_qGoal.GetIsFinished()) // this is to show the popup message only once
                {
                    Item _itemToRemove = new Item(_qGoal.itemToGatherOrDeliver.item);
                    _itemToRemove.eType = ItemType.QuestItem;
                    player.GetInventory().RemoveQuestItem(_itemToRemove);
                    PopupUIManager.Instance.msgBoxPopup.ShowMessageAfterDialog(sQuestUpdated, 1);
                }
                _qGoal.SetIsFinished(_bIsCompleted);
                ActivateNextGoal();
            }
        }
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
                PopupUIManager.Instance.msgBoxPopup.ShowMessageAfterDialog(sMainQuestAdded, 1);
                QuestManager.Instance.dictMainQuests.Add(sQuestID, this);
            }
        }
        else if (eQuestType == QuestType.SIDEQUEST)
        {
            if (!QuestManager.Instance.dictSideQuests.ContainsKey(sQuestID))
            {
                PopupUIManager.Instance.msgBoxPopup.ShowMessageAfterDialog(sSideQuestAdded, 1);
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