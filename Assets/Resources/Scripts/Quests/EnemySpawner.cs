using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    float fDIST_TO_PLAYER = 650f;
    const float fFirstEnemyRespawnTime = 40f;
    const float fOtherEnemiesRespawnTime = 20f;


    [SerializeField]
    string sID = System.Guid.NewGuid().ToString();//"KQ-";
    public bool bForQuestsOnly;
    public bool bRandomizeLocations;
    public Transform[] spawnLocations;

    public List<Enemy> enemiesToSpawnList;

    List<Enemy> aliveEnemiesLst;
    Queue<Enemy> deadEnemiesLst;
    bool bIsActive;

    public Collider spawnerBoundaryCol;
    PlayerController player;
    bool bEnemiesActivated;
    float fRespawnTimeCounter = 0f;

    private void Start()
    {
        player = PlayerController.Instance;
        aliveEnemiesLst = new List<Enemy>();
        deadEnemiesLst = new Queue<Enemy>();
        InvokeRepeating("CheckDistanceToPlayer", 1, 1);

        if (bForQuestsOnly)
            SetActive(false);
        else
            SpawnEnemies();

        bEnemiesActivated = true;
        fDIST_TO_PLAYER = (spawnerBoundaryCol.bounds.size.x * spawnerBoundaryCol.bounds.size.z) / 1.5f;
    }

    public void Update()
    {
       // Debug.Log(spawnerBoundaryCol.bounds.SqrDistance(spawnerBoundaryCol.bounds.size));
        if (bIsActive)
        {
            if (!bEnemiesActivated)
            {
                ActivateDeactivateEnemies(true);
                bEnemiesActivated = true;
            }

            CheckAllEnemiesStats();
        }
        else
        {
            if (bEnemiesActivated)
            {
                ActivateDeactivateEnemies(false);
                bEnemiesActivated = false;
            }
        }
        RespawnEnemies();
    }
    public void CheckAllEnemiesStats()
    {
        for (int i = 0; i < aliveEnemiesLst.Count; i++)
        {
            if (aliveEnemiesLst[i].IsEnemyDead())
            {
                if (bForQuestsOnly)// if enemy is for quest then destroy it
                    aliveEnemiesLst[i].DestroyEnemy();
                else // else they are set to false by default
                    deadEnemiesLst.Enqueue(aliveEnemiesLst[i]);

                aliveEnemiesLst.Remove(aliveEnemiesLst[i]);
            }
        }
    }
    public void ActivateDeactivateEnemies(bool _bSetActivate)
    {
        for (int i = 0; i < aliveEnemiesLst.Count; i++)
        {
            aliveEnemiesLst[i].gameObject.SetActive(_bSetActivate);
            if(_bSetActivate)
                aliveEnemiesLst[i].ResetBools();
        }
    }
    public void RespawnEnemies()
    {
        // TODO: Spawn Dead Enemies After Some Time 
        if(deadEnemiesLst.Count > 0)
        {
            fRespawnTimeCounter += Time.deltaTime;
            if(fRespawnTimeCounter >= fFirstEnemyRespawnTime && !bIsActive)
            {
                Enemy _revivedEnemy = deadEnemiesLst.Dequeue();
                _revivedEnemy.Revive(RandomPositionInArea());
                //_revivedEnemy.gameObject.SetActive(true);
                aliveEnemiesLst.Add(_revivedEnemy);
                fRespawnTimeCounter = 0;
            }
        }
    }
    public void CheckDistanceToPlayer()
    {
        if ((spawnerBoundaryCol.bounds.center - player.transform.position).sqrMagnitude <= fDIST_TO_PLAYER)
            bIsActive = true;
        else
            bIsActive = false;

        //if (spawnerBoundaryCol.bounds.Contains(player.transform.position))
        //    bIsActive = true;
        //else
        //    bIsActive = false;
    }
    public void SpawnEnemies()
    {
        for (int i = 0; i < enemiesToSpawnList.Count; i++)
        {
            if (bRandomizeLocations)
                SpawnEnemyAtRandonLocation(enemiesToSpawnList[i]);
            else
            {
                if(i < spawnLocations.Length)
                    SpawnEnemy(enemiesToSpawnList[i], spawnLocations[i].position);
                else
                    SpawnEnemyAtRandonLocation(enemiesToSpawnList[i]);
            }
        }
    }
    public void SpawnEnemy(Enemy _enemy, Vector3 _position)
    {
        // TODO: Add prefabs according to enemy etype (right now only plant enemiesprefab)        
        Enemy _newEnemy = Instantiate(_enemy, _position, Quaternion.identity);
        _newEnemy.SetBoundary(spawnerBoundaryCol);
        aliveEnemiesLst.Add(_newEnemy);
    }
    public void SpawnEnemyAtRandonLocation(Enemy _enemy)
    {
        Vector3 _randPosition = RandomPositionInArea();
        Enemy _newEnemy = Instantiate(_enemy, _randPosition, Quaternion.identity);
        _newEnemy.SetBoundary(spawnerBoundaryCol);
        aliveEnemiesLst.Add(_newEnemy);
    }
    public Vector3 RandomPositionInArea()
    {
        return new Vector3(
            Random.Range(spawnerBoundaryCol.bounds.min.x + 2, spawnerBoundaryCol.bounds.max.x - 2),
            Random.Range(spawnerBoundaryCol.bounds.max.y - 1, spawnerBoundaryCol.bounds.max.y),
            Random.Range(spawnerBoundaryCol.bounds.min.z + 2, spawnerBoundaryCol.bounds.max.z - 2)
        );
    }
    public bool CheckIfAllEnemiesDead()
    {
        return (aliveEnemiesLst.Count <= 0);
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
