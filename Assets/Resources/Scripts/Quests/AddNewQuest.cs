using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddNewQuest : MonoBehaviour
{
    public Quest quest;
    [TextArea(2, 3)]
    [Tooltip("Use '&response' for adding user response for Side Quests for accepting or declining the quest.")]
    public string[] sQuestStartDialogLines;
    [TextArea(2, 3)]
    [Tooltip("Use '&response' for adding user response for Side Quests for accepting or declining the quest.")]
    public string[] sQuestInProgressDialogLines;
    [TextArea(2, 3)]
    [Tooltip("Use '&response' for adding user response for Side Quests for accepting or declining the quest.")]
    public string[] sQuestEndDialogLines;

    public void Start()
    {
        // Check if this quest is there in the completed quests, if it is then make the quest = null;
        //quest.QuestCompleted(true);
        //this.enabled = false;

        if (!QuestManager.Instance.dictAllQuests.ContainsKey(quest.sQuestID))
        {
            QuestManager.Instance.dictAllQuests.Add(quest.sQuestID, quest);
        }
    }
    public string[] QuestStartDialog()
    {
        //if (quest.eQuestType == QuestType.MAINQUEST)
        //{
        //    if (!QuestManager.Instance.dictMainQuests.ContainsValue(quest))
        //    {
        //        _quest.SetQuestActive(true);
        //        _quest.Initialize(GameController.Instance.player);
        //    }
        //}
        // else is its a side quest it asks for user response below
        return sQuestStartDialogLines;
    }
    public string[] QuestInProgressDialog()
    {
        return sQuestInProgressDialogLines;
    }
    public string[] QuestFinishedDialog()
    {
       // quest.GiveRewardAndFinish();
       // quest.qGoals = null;
        return sQuestEndDialogLines;
    }
}
