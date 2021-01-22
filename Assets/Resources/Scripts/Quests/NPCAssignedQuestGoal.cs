using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAssignedQuestGoal : MonoBehaviour
{
    private string sQuestID;
    public string[] sQuestDialog;
    bool bHasFinished;

    public bool QuestGoalCheck()
    {
        // can add another dialog to use if this quest is completed to make it more real.
        foreach (var _quest in QuestManager.Instance.dictMainQuests)
        {
            if (_quest.Value.sQuestID == sQuestID)
            {
                for (int i = 0; i < _quest.Value.qGoals.Length; i++)
                {
                    if (_quest.Value.qGoals[i].GetIsActive())
                    {
                        if (_quest.Value.qGoals[i].eGoalType == QuestGoalType.GOTONPC)
                        {
                            if (_quest.Value.sQuestID == this.sQuestID)
                            {
                                _quest.Value.GoToNPCCompleted(_quest.Value.qGoals[i], true);
                                return true;
                            }
                        }
                    }
                }
            }
        }

        foreach (var _quest in QuestManager.Instance.dictSideQuests)
        {
            if (_quest.Value.sQuestID == sQuestID)
            {
                for (int i = 0; i < _quest.Value.qGoals.Length; i++)
                {
                    if (_quest.Value.qGoals[i].GetIsActive())
                    {
                        if (_quest.Value.qGoals[i].eGoalType == QuestGoalType.GOTONPC)
                        {
                            if (_quest.Value.sQuestID == this.sQuestID)
                            {
                                _quest.Value.GoToNPCCompleted(_quest.Value.qGoals[i], true);
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;

    }
    public void SetIsFinished(bool _bHasFinished)
    {
        bHasFinished = _bHasFinished;
    }
    public bool IsFinished()
    {
        return bHasFinished;
    }
    public void SetQuestID(string _sID)
    {
        sQuestID = _sID;
    }
}
