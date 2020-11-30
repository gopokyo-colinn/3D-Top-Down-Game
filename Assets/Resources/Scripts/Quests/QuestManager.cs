using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestManager 
{
	public Dictionary<string, Quest> dictMainQuests;
	public Dictionary<string, Quest> dictSideQuests;
	public Quest activeQuest;
	public QuestGoal activeGoal;


	#region Singleton
	private static QuestManager instance = null;
	public QuestManager() 
	{
		dictMainQuests = new Dictionary<string, Quest>();
		dictSideQuests = new Dictionary<string, Quest>();
	}
	public static QuestManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new QuestManager();
			}
			return instance;
		}
	}
	#endregion

	public void Initialize(PlayerController _player)
    {
		for (int i = 0; i < dictMainQuests.Count; i++)
		{
			dictMainQuests.ElementAt(i).Value.Initialize(_player);
		}
		for (int i = 0; i < dictSideQuests.Count; i++)
		{
			dictSideQuests.ElementAt(i).Value.Initialize(_player);
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
}
