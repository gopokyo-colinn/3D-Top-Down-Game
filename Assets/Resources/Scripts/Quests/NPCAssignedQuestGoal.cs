using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAssignedQuestGoal : MonoBehaviour
{
    [SerializeField]
    private string sQuestID;
    public string[] sQuestDialog;
    bool bIsFinished;

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
                        if (_quest.Value.qGoals[i].eGoalType == QuestGoalType.GO_TO_NPC)
                        {
                            if (_quest.Value.qGoals[i].taskNPC == this)
                            {
                                _quest.Value.SetGoToNPCGoal(_quest.Value.qGoals[i], true);
                                bIsFinished = true;
                                return bIsFinished;
                            }
                        }
                        else if(_quest.Value.qGoals[i].eGoalType == QuestGoalType.DELIVER)
                        {
                            if (_quest.Value.qGoals[i].taskNPC == this)
                            {
                                _quest.Value.DeliverItemGoal(_quest.Value.qGoals[i], true);
                                bIsFinished = true;
                                return bIsFinished;
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
                        if (_quest.Value.qGoals[i].eGoalType == QuestGoalType.GO_TO_NPC)
                        {
                            if (_quest.Value.qGoals[i].taskNPC == this)
                            {
                                _quest.Value.SetGoToNPCGoal(_quest.Value.qGoals[i], true);
                                bIsFinished = true;
                                return bIsFinished;
                            }
                        }
                        else if (_quest.Value.qGoals[i].eGoalType == QuestGoalType.DELIVER)
                        {
                            if (_quest.Value.qGoals[i].taskNPC == this)
                            {
                                _quest.Value.DeliverItemGoal(_quest.Value.qGoals[i], true);
                                bIsFinished = true;
                                return bIsFinished;
                            }
                        }
                    }
                }
            }
        }
        return false;

    }
    public bool IsFinished()
    {
        return bIsFinished;
    }
    public void SetIsFinished(bool _bIsFinished)
    {
        bIsFinished = _bIsFinished;
    }
    public void SetQuestID(string _sID)
    {
        sQuestID = _sID;
    }
}
