using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public enum QuestType { MAINQUEST = 0, SIDEQUEST = 1 }
[Serializable]
public class Quest
{
    const string sQuestUpdated = "Quest Updated...";
    const string sMainQuestAdded = "New Main Quest Added...";
    const string sSideQuestAdded = "New Side Quest Added...";
    const string sQuestCompleted = "Quest Completed...";
    const float fMsgTime = 2f;

    [UniqueID]
    public string sQuestID;
    public string sQuestTitle;
    [TextArea(2, 4)]
    public string sQuestDescription;
    public QuestType eQuestType;
    [Tooltip("Only check the box if the quest is not given my any npc (if its auto add quest).")]
    public bool bSelfAssignedQuest;
    public bool bQuestInAnyOrder;
    // public QuestGoal qGoal;
   
    private bool bAllGoalsCompleted = false;
    public QuestGoal[] qGoals;

    [SerializeField]
    private bool bIsActive;
    [SerializeField]
    private bool bIsCompleted;

    public string sRewards; //TODO: Replace this with type of reward such as GOLD, EXP, ITEMS. Make it a array to choose a number of rewards.\\
    [SerializeField]
    public UnityEvent activateNextQuestUponCompletion;
    PlayerController player;

    public void Initialize()
    {
        if (!IsCompleted())
        {
            player = PlayerController.Instance;
            SetQuestActive(true);
            AddQuestToManager(); // Quest Added Popup Msg is in here
            if (bQuestInAnyOrder)
                ActivateAllGoals();
            else
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
   
    public void GiveRewardAndFinish()
    {
        if (bAllGoalsCompleted)
        {
            PopupUIManager.Instance.msgBoxPopup.ShowMessageAfterDialog(sQuestCompleted + " \n" + sRewards, fMsgTime);
            Debug.Log(sRewards);
            RemoveQuest();
            if(activateNextQuestUponCompletion != null)
            {
                activateNextQuestUponCompletion.Invoke();
            }
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

        GiveRewardAndFinish();

        return bAllGoalsCompleted;
    }
    public bool CheckQuestComplete()
    {
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
        //if (bQuestInAnyOrder)
        //{
        //    if (_qGoal.enemiesSpawner.CheckIfAllEnemiesDead())
        //    {
        //        _qGoal.enemiesSpawner.SetActive(false);
        //        _qGoal.SetIsFinished(true);
        //        //PopupUIManager.Instance.msgBoxPopup.ShowTextMessage("Quest Updated...", 1);
        //    }
        //}
        //else
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
        if (_qGoal.GetIsActive())
            {
                if ((player.transform.position - _qGoal.tLocationToReach.position).sqrMagnitude <= 2f)
                {
                    _qGoal.SetIsFinished(true);
                    ActivateNextGoal();
                }
            }
      // }
    }
    public void SetGoToNPCGoal(QuestGoal _qGoal, bool _isCompleted) // This will be directly used by the NPC's
    {
        {
            if (_isCompleted)
            {
                _qGoal.SetIsFinished(_isCompleted);
                ActivateNextGoal();
            }
        }
    }
    public void GatherItemGoal(QuestGoal _qGoal)
    {
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
    public bool DeliverItemGoal(QuestGoal _qGoal)
    {
        {
            if (player.GetInventory().HasItem(_qGoal.itemToGatherOrDeliver.item.sID))
            {
                if (!_qGoal.GetIsFinished()) // this is to show the popup message only once
                {
                    player.GetInventory().RemoveQuestItem(_qGoal.itemToGatherOrDeliver.item);
                }
                _qGoal.SetIsFinished(true);
                ActivateNextGoal();
                return true;
            }
            return false;
        }
    } // Not Yet Done
    public void ReturnToQuestGiverGoal(QuestGoal _qGoal, bool _bIsCompleted)
    {
        if (_bIsCompleted)
        {
            _qGoal.SetIsFinished(_bIsCompleted);
            ActivateNextGoal();
        }
    }
    public void AddQuestToManager()
    {
        if (eQuestType == QuestType.MAINQUEST)
        {
            if (!QuestManager.Instance.dictMainQuests.ContainsKey(sQuestID))
            {
                PopupUIManager.Instance.msgBoxPopup.ShowMessageAfterDialog(sMainQuestAdded, fMsgTime);
                QuestManager.Instance.dictMainQuests.Add(sQuestID, this);
            }
        }
        else if (eQuestType == QuestType.SIDEQUEST)
        {
            if (!QuestManager.Instance.dictSideQuests.ContainsKey(sQuestID))
            {
                PopupUIManager.Instance.msgBoxPopup.ShowMessageAfterDialog(sSideQuestAdded, fMsgTime);
                QuestManager.Instance.dictSideQuests.Add(sQuestID, this);
            }
        }
    }
    public void ActivateAllGoals()
    {
        for (int i = 0; i < qGoals.Length; i++)
        {
            if (!qGoals[i].GetIsActive())
            {
                qGoals[i].SetIsActive(true);
                qGoals[i].InitializeGoal(sQuestID);
            }
        }
    }
   
    public void ActivateNextGoal()
    {
        if (bQuestInAnyOrder)
        {
            CheckQuestProgress();
            if(!IsCompleted()) // meaning if an goal is completed then show updated text
                    PopupUIManager.Instance.msgBoxPopup.ShowMessageAfterDialog(sQuestUpdated, fMsgTime);
        }
        else
        {
            for (int i = 0; i < qGoals.Length; i++)
            {
                if (!qGoals[i].GetIsActive())
                {
                    if(i != 0)
                        PopupUIManager.Instance.msgBoxPopup.ShowMessageAfterDialog(sQuestUpdated, fMsgTime); // To not show update text when new quest is just added
                    qGoals[i].SetIsActive(true);
                    qGoals[i].InitializeGoal(sQuestID);
                    break;
                }
                else
                {
                    if (qGoals[i].GetIsFinished())
                    {
                        //  if(bSelfAssignedQuest)
                        if (CheckQuestProgress())
                            break;
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
    public bool IsCompleted()
    {
        return bIsCompleted;
    }
    public bool IsActive()
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