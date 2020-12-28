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
    PlayerController player;

    EnemySpawner killQuest;

    public void Initialize(PlayerController _player)
    {
        //qGoal = new QuestGoal();
        player = _player;// FindObjectOfType<PlayerController>();
        if (qGoal.eGoalType == QuestGoalType.KILL)
        {
            killQuest = KillQuestsManager.Instance.GetCurrentQuestSpawner(sQuestID);
            KillQuestsManager.Instance.InitializeQuestEnemies(sQuestID, qGoal.enemiesToKill);
        }

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
        if(killQuest.enemiesLst.Count <= 0)
        {
            qGoal.isFinished = true;
            killQuest.gameObject.SetActive(false);
        }
    }
    public void GoToLocationQuest()
    {
        if ((player.transform.position - qGoal.tLocationToReach.position).sqrMagnitude <= 1f)
        {
            qGoal.isFinished = true;
        }
    }
    public void GatherQuest()
    {

    }
    public void DeliverQuest()
    {

    }
}
