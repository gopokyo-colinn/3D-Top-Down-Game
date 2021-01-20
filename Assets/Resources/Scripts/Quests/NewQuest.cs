using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewQuest : MonoBehaviour
{
    public Quest quest;

    [TextArea(2, 3)]
    [Tooltip("Use '&response' for adding user response for accepting or declining the quest." +
    "\n Use '&questAdded' to show quest added popup message. \n Use '&questCompleted' to show quest completed popup message")]
    public string[] sQuestStartDialogLines;
    [TextArea(2, 3)]
    [Tooltip("Use '&response' for adding user response for accepting or declining the quest." +
        "\n Use '&questAdded' to show quest added popup message. \n Use '&questCompleted' to show quest completed popup message")]
    public string[] sQuestInProgressDialogLines;
    [TextArea(2, 3)]
    [Tooltip("Use '&response' for adding user response for accepting or declining the quest." +
        "\n Use '&questAdded' to show quest added popup message. \n Use '&questCompleted' to show quest completed popup message")]
    public string[] sQuestEndDialogLines;


    public string[] QuestStartDialog()
    {
        if (quest.eQuestType == QuestType.MAINQUEST)
        {
            if (!QuestManager.Instance.dictMainQuests.ContainsValue(quest))
            {
                quest.bIsActive = true;
                quest.Initialize(GameController.Instance.player);
            }
        }
        // else is its a side quest it asks for user response below
        return sQuestStartDialogLines;
    }
    public string[] QuestInProgressDialog()
    {
        return sQuestInProgressDialogLines;
    }
    public string[] QuestFinishedDialog()
    {
        quest.GiveReward();
        quest.qGoals = null;
        return sQuestEndDialogLines;
    }
}
