using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillQuestsManager : MonoBehaviour
{
    #region singleton
    private static KillQuestsManager instance;

    public static KillQuestsManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<KillQuestsManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(KillQuestsManager).Name;
                    instance = obj.AddComponent<KillQuestsManager>();
                }
            }
            return instance;
        }
    }
    #endregion

    EnemySpawner[] questSpawners;
    EnemySpawner currentQuestSpawner;
    void Start()
    {
        questSpawners = GetComponentsInChildren<EnemySpawner>();
    }
    public void InitializeQuestEnemies(string _sQuestId, EnemyType[] _enemiesList)
    {
        currentQuestSpawner = GetCurrentQuestSpawner(_sQuestId);
        currentQuestSpawner.SpawnEnemies(_enemiesList);
    }

    public EnemySpawner GetCurrentQuestSpawner(string _sQuestId)
    {
        for (int i = 0; i < questSpawners.Length; i++)
        {
            if(questSpawners[i].sQuestID == _sQuestId)
            {
                return questSpawners[i];
            }
        }
        return null;
    }
}
