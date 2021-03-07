using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestManager: MonoBehaviour, ISaveable
{
	//public Dictionary<string, Quest> dictMainQuests;
	//public Dictionary<string, Quest> dictSideQuests;
	//public Dictionary<string, Quest> dictCompletedQuests;

	public Dictionary<string, Quest> dictMainQuests;
	public Dictionary<string, Quest> dictSideQuests;
	public Dictionary<string, Quest> dictCompletedQuests;
	public Dictionary<string, Quest> dictAllQuests;

	public Quest activeQuest;
	public QuestGoal activeGoal;

	#region Singleton
	protected static QuestManager instance;
	public static QuestManager Instance { get { return instance; } }
    #endregion
    private void Awake()
    {
		instance = this;
		dictAllQuests = new Dictionary<string, Quest>();
		dictMainQuests = new Dictionary<string, Quest>();
		dictSideQuests = new Dictionary<string, Quest>();
		dictCompletedQuests = new Dictionary<string, Quest>();
    }
    public void Initialize()
    {
		for (int i = 0; i < dictMainQuests.Count; i++)
        {
            dictMainQuests.ElementAt(i).Value.Initialize();
        }
        for (int i = 0; i < dictSideQuests.Count; i++)
        {
            dictSideQuests.ElementAt(i).Value.Initialize();
        }
	}
	public void Refresh()
    {
        for (int i = 0; i < dictMainQuests.Count; i++)
        {
            dictMainQuests.ElementAt(i).Value.Refresh();
        }
        for (int i = 0; i < dictSideQuests.Count; i++)
        {
            dictSideQuests.ElementAt(i).Value.Refresh();
        }
    }

    public void SaveAllData(SaveData _saveData)
    {
		instance.SaveQuestsData(_saveData);
    }

    public void LoadSaveData(SaveData _saveData) 
	{   
		instance.LoadQuestsData(_saveData);
		instance.Initialize();
	}
	public void SaveQuestsData(SaveData _saveData)
    {
		structQuestsManager _structQuestsManager = new structQuestsManager();

		_structQuestsManager.activeMainQuestsLst = SaveQuests(dictMainQuests);
		_structQuestsManager.activeSideQuestsLst = SaveQuests(dictSideQuests);
		_structQuestsManager.allCompletedQuestsLst = SaveQuests(dictCompletedQuests);

		_saveData.questsSavedData = _structQuestsManager;
	}
	public void LoadQuestsData(SaveData _saveData)
    {
		dictMainQuests = new Dictionary<string, Quest>();
		dictSideQuests = new Dictionary<string, Quest>();
		dictCompletedQuests = new Dictionary<string, Quest>();

		structQuestsManager _structQuestsManager = new structQuestsManager();
		_structQuestsManager = _saveData.questsSavedData;

		dictMainQuests = LoadQuests(_structQuestsManager.activeMainQuestsLst);
		dictSideQuests = LoadQuests(_structQuestsManager.activeSideQuestsLst);
		dictCompletedQuests = LoadQuests(_structQuestsManager.allCompletedQuestsLst);
		
	}
	public List<structQuest> SaveQuests(Dictionary<string, Quest> _dict)
    {
		List<structQuest> _questsLst = new List<structQuest>();
		foreach (var _quest in _dict)
		{
			structQuest _structQuest = new structQuest();
			_structQuest.sQuestID = _quest.Value.sQuestID;
			_structQuest.bIsActive = _quest.Value.IsActive();
			_structQuest.bIsCompleted = _quest.Value.IsCompleted();
			_structQuest.qGoalsLst = GetGoals(_quest.Value);
			_questsLst.Add(_structQuest);
		}
		return _questsLst;
	}
	public Dictionary<string, Quest> LoadQuests(List<structQuest> _questsLst)
	{
		Dictionary<string, Quest> _questsDict = new Dictionary<string, Quest>();

        foreach (var _loadedQuest in _questsLst)
        {
            foreach (var _quest in dictAllQuests)
            {
				if(_loadedQuest.sQuestID == _quest.Key)
                {
					// add quest to the dict and set goals according to save file
					_quest.Value.qGoals = SetGoals(_quest.Value, _loadedQuest);
					_quest.Value.SetQuestActive(_loadedQuest.bIsActive);
					_quest.Value.SetQuestCompleted(_loadedQuest.bIsCompleted);
					
					if(!_questsDict.ContainsKey(_quest.Key))
						_questsDict.Add(_quest.Key, _quest.Value);
                }
            }
        }
		return _questsDict;
	}
	public List<structQuestGoal> GetGoals(Quest _quest)
    {
		List<structQuestGoal> _qGoalsLst = new List<structQuestGoal>();
        for (int i = 0; i < _quest.qGoals.Length; i++)
        {
			structQuestGoal _structQuestGoal = new structQuestGoal();
			_structQuestGoal.bIsActive = _quest.qGoals[i].GetIsActive();
			_structQuestGoal.bIsFinished = _quest.qGoals[i].GetIsFinished();
			_qGoalsLst.Add(_structQuestGoal);
		}
		return _qGoalsLst;
    }
	public QuestGoal[] SetGoals(Quest _quest, structQuest _loadedQuestsLst)
	{
		QuestGoal[]_qGoalsLst = _quest.qGoals.ToArray();

        for (int i = 0; i < _qGoalsLst.Length; i++)
        {
			_qGoalsLst[i].SetIsActive(_loadedQuestsLst.qGoalsLst[i].bIsActive);
			_qGoalsLst[i].SetIsFinished(_loadedQuestsLst.qGoalsLst[i].bIsFinished);
			//if(_qGoalsLst[i].GetIsActive() && !_qGoalsLst[i].GetIsFinished())
            {
				_qGoalsLst[i].InitializeGoal(_loadedQuestsLst.sQuestID);
            }
        }

		return _qGoalsLst;
	}

	public Quest GetQuestByID(Quest _questToFind)
	{
		if (_questToFind.eQuestType == QuestType.MAINQUEST)
		{
			foreach (var _quest in dictMainQuests)
			{
				if (_quest.Value.sQuestID == _questToFind.sQuestID)
				{
					return _quest.Value;
				}
			}
		}
		else
		{
			foreach (var _quest in dictSideQuests)
			{
				if (_quest.Value.sQuestID == _questToFind.sQuestID)
				{
					return _quest.Value;
				}
			}
		}

        foreach (var _quest in dictAllQuests)
        {
			if (_quest.Value.sQuestID == _questToFind.sQuestID)
			{
				if(!_quest.Value.IsActive())
					return _quest.Value;
			}
		}
		return null;
	}
}
