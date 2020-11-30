using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public bool inPlayMode;

	public PlayerController player;

	private static GameController instance;
	public static GameController Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<GameController>();
				if (instance == null)
				{
					GameObject obj = new GameObject();
					obj.name = typeof(GameController).Name;
					instance = obj.AddComponent<GameController>();
				}
			}
			return instance;
		}
	}
	protected virtual void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{ 
			Destroy(gameObject);
		}
	}
	// Start is called before the first frame update
	void Start()
    {
		player = FindObjectOfType<PlayerController>();
        inPlayMode = true;
		Debug.Log(QuestManager.Instance.activeQuest.sQuestTitle);
		QuestManager.Instance.Initialize(player);
    }
    private void LateUpdate()
    {
		QuestManager.Instance.Refresh();
    }
}
