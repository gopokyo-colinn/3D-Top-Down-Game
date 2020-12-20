using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    public bool isActive;
    public Quest quest;

    public void Awake()
    {
        QuestManager.Instance.dictSideQuests.Add(quest.sQuestTitle, quest);
        QuestManager.Instance.activeQuest = quest;
        QuestManager.Instance.activeGoal = quest.qGoal;
    }


}
