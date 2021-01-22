using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameController : MonoBehaviour
{
    public static bool inPlayMode;

	public static bool bGamePaused;

	public PlayerController player;

	SaveData saveData;

	ISaveable[] saveablesList;

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

		saveablesList = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>().ToArray();

		player = FindObjectOfType<PlayerController>();
		saveData = new SaveData();
	}
	// Start is called before the first frame update
	void Start()
    {
        inPlayMode = true;
	}
    private void Update()
    {
		QuestManager.Instance.Refresh();

		/// Saving and Loading Testing
		if (Input.GetKeyDown(KeyCode.F5))
        {
			SaveGameManager.SaveGame(saveablesList);
        }
		if (Input.GetKeyDown(KeyCode.F9))
        {
			SaveGameManager.LoadGame(saveablesList);
        }
	}
}
