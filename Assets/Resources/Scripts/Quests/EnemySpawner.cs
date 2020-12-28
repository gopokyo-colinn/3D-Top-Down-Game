using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public string sQuestID = "KQ-";
    public Transform[] spawnLoactions;

    public List<Enemy> enemiesLst;

    public Enemy enemyPrefab;

    bool isActive = false;

    public void Update()
    {
        if (isActive)
        {
            for (int i = 0; i < enemiesLst.Count; i++)
            {
                if(enemiesLst[i].EnemyDied())
                {
                    enemiesLst.Remove(enemiesLst[i]);
                }
            }
            Debug.Log("I m runnung");
        }
    }

    public void SpawnEnemies(EnemyType[] _enemiesList)
    {
        isActive = true;

        for (int i = 0; i < _enemiesList.Length; i++)
        {
            SpawnEnemy(spawnLoactions[i].position);
        }
    }
    public void SpawnEnemy(Vector3 _position)
    {
        Enemy _newEnemy = Instantiate(enemyPrefab, _position, Quaternion.identity);
        if(enemiesLst == null)
        {
            enemiesLst = new List<Enemy>();
        }
        enemiesLst.Add(_newEnemy);
    }
}
