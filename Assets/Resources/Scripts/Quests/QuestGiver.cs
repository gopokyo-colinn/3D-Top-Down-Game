using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    public bool isActive;
    public Quest quest;

    PlayerController player;

    public void Start()
    {
        QuestManager.Instance.dictSideQuests.Add(quest.sQuestTitle, quest);
        QuestManager.Instance.activeQuest = quest;
        QuestManager.Instance.activeGoal = quest.qGoal;
    }


}
