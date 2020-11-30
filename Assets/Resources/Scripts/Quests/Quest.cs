using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum QuestType { MAINQUEST = 0, SIDEQUEST = 1 }
[Serializable]
public class Quest
{
    public string sQuestTitle;
    public string sQuestDescription;
    public QuestType eQuestType;
    public int iQuestNumber;
    public QuestGoal qGoal;
    PlayerController player;

    public void Initialize(PlayerController _player)
    {
        //qGoal = new QuestGoal();
        player = _player;// FindObjectOfType<PlayerController>();
        Debug.Log("Quest Initialized");
    }

    public void Refresh()
    {
        CheckGoalProgress();
        Reward();
       // Debug.Log("Quest Refreshing");
    }

    public void StartCondition()
    {

    }

    public void QuestTrigger()
    {

    }

    public void Reward()
    {
        if (qGoal.isFinished)
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
                break;
            case QuestGoalType.GATHER:
                break;
            case QuestGoalType.KILL:
                break;
            case QuestGoalType.GOTOLOCATION:
                GoToLocationQuest();
                break;
        }
    }

    public void RemoveQuest()
    {
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

    }
    public void GoToLocationQuest()
    {
        if ((player.transform.position - qGoal.tLocationToReach.position).sqrMagnitude <= 1f)
        {
            qGoal.isFinished = true;
        }
    }
}
