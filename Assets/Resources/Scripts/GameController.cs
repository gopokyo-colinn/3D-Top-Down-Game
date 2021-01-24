using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameController : MonoBehaviour
{
    public static bool inPlayMode;

	public static bool bGamePaused;

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

		LoadSceneInfo();

	}
	// Start is called before the first frame update
	void Start()
    {
        inPlayMode = true;
		if (SaveGameManager.gameLoaded)
		{
			LoadGame();
		}
	}
    private void Update()
    {
		if(QuestManager.Instance)
			QuestManager.Instance.Refresh();

		/// Saving and Loading Testing
		if (Input.GetKeyDown(KeyCode.F5))
        {
			SaveGame();
        }
		if (Input.GetKeyDown(KeyCode.F9))
        {
			LoadGame();
        }
	}
	public void SaveGame()
    {
		SaveGameManager.SaveGame(saveablesList);
	}
	public void LoadGame()
    {
		UnityEngine.SceneManagement.SceneManager.LoadScene(1); 
		StartCoroutine(LoadAfter());
	}
	IEnumerator LoadAfter()
    {
		yield return new WaitForSeconds(0f);
		//LoadSceneInfo();
		SaveGameManager.LoadGame(saveablesList);
		StopAllCoroutines();
	}
    public void LoadSceneInfo()
    {
		saveablesList = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>().ToArray();
		saveData = new SaveData();
	}
}
