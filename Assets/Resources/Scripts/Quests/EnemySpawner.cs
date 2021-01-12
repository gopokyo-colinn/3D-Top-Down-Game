using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    string sID = "KQ-";
    public Transform[] spawnLoactions;

    public List<EnemyType> enemiesToSpawnList;
    List<Enemy> enemiesLst;

    public Enemy plantEnemyPrefab;

    bool isActive = false;

    private void Start()
    {
        enemiesLst = new List<Enemy>();
        SetActive(false);
    }

    public void Update()
    {
        if (isActive)
        {
            for (int i = 0; i < enemiesLst.Count; i++)
            {
                if (enemiesLst[i].EnemyDied())
                {
                    enemiesLst.Remove(enemiesLst[i]);
                }
            }
        }
    }

    public void SpawnEnemies()
    {
        for (int i = 0; i < enemiesToSpawnList.Count; i++)
        {
            SpawnEnemy(enemiesToSpawnList[i], spawnLoactions[i].position);
        }

        isActive = true;
    }
    public void SpawnEnemy(EnemyType _eType, Vector3 _position)
    {
        // TODO: Add prefabs according to enemy etype (right now only plant enemiesprefab)
        Enemy _newEnemy = Instantiate(plantEnemyPrefab, _position, Quaternion.identity);
        
        enemiesLst.Add(_newEnemy);
    }
    public bool CheckIfAllEnemiesDead()
    {
        return (enemiesLst.Count <= 0);
    }
    public void SetActive(bool _b)
    {
        gameObject.SetActive(_b);
    }
    public void SetID(string _ID)
    {
        sID = _ID;
    }
}
