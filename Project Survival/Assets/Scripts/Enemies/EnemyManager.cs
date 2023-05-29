using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
     
    [System.Serializable]
    public class Round
    {
        public List<EnemyGroup> enemyGroups;
        public int totalEnemiesInRound;
        public int currentTotalSpawned;
        public float spawnInterval;
    }
    [System.Serializable]
    public class EnemyGroup
    {
        public GameObject enemyPrefab;
        public int numberToSpawn;
        public int whenToSpawn;   //start spawning when spawner reach the amount of mobs spawned
        public int currentSpawned;
    }
    public int enemiesAlive;
    public List<Round> rounds;
    public PlayerStats player;
    public Transform enemyParent;
    public EnemyController enemyController;
    public GameplayManager gameplayMananger;
    public GameObject basePrefab;
    public int roundCounter;
    public float spawnTimer;
    public int enemiesAliveCap;
    public bool maxEnemiesReached;
    int inactive;

    private void Start()
    {
        PopulatePool(40);
        CalculateTotalEnemiesInRound();
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if(spawnTimer >= rounds[roundCounter].spawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemies();
        }
    }
    public void CalculateTotalEnemiesInRound()
    {
        for (int i = 0; i < rounds.Count; ++i)
        {
            int currentSpawnPerInterval = 0;
            foreach (var enemyGroup in rounds[i].enemyGroups)
            {
                currentSpawnPerInterval += enemyGroup.numberToSpawn;
            }
            rounds[i].totalEnemiesInRound = currentSpawnPerInterval;
        }

    }
    //Check if kill count meets requirement for next round, then go to next round.

    protected virtual void CheckPoolAmount()
    {
        inactive = 0;
        for (int i = 0; i < enemyController.enemyList.Count; i++)
        {
            if (!enemyController.enemyList[i].isActiveAndEnabled) inactive++;    //Calculate how many inactives there are
        }
        if (inactive <= 20) //if inactives are less than the number, spawn more. Change number if not enough.
        {
            PopulatePool(20);
        }
    }

    protected virtual void PopulatePool(int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject enemy = Instantiate(basePrefab, enemyController.transform);    //Spawn, add to list, and initialize prefabs
            EnemyStats es = enemy.GetComponent<EnemyStats>();
            es.spawnInPool = true;
            es.enemyManager = this;
            es.enemyMovement.playerTransform = player.transform;
            enemy.SetActive(false);
            es.spawnInPool = false;
            enemyController.AddToList(es);
        }
    }
        public void SpawnEnemies()
    {
        CheckPoolAmount();
        if (rounds[roundCounter].currentTotalSpawned < rounds[roundCounter].totalEnemiesInRound && !maxEnemiesReached)   //Check if there is still mobs left to spawn
        {
            foreach(var eGroup in rounds[roundCounter].enemyGroups)     //For each enemy groups in a round, spawn the groups
            {
                if(eGroup.currentSpawned < eGroup.numberToSpawn && (eGroup.whenToSpawn <= rounds[roundCounter].currentTotalSpawned))      //Check if min number of mobs of this type have been spawned
                {
                    if(enemiesAlive >= enemiesAliveCap) //Stop spawning if cap is reached
                    {
                        maxEnemiesReached = true;
                        return;
                    }
                    Vector2 spawnPos = new Vector2(player.transform.position.x + Random.Range(-10f, 10f), player.transform.position.y + Random.Range(-10f, 10f));

                    for (int i = 0; i < enemyController.enemyList.Count; i++)   //Find an inactive enemy to spawn
                    {
                        if (!enemyController.enemyList[i].isActiveAndEnabled)
                        {
                            enemyController.enemyList[i].transform.position = spawnPos;    //set starting position when spawn
                            enemyController.enemyList[i].gameObject.SetActive(true);
                            eGroup.currentSpawned++;
                            rounds[roundCounter].currentTotalSpawned++;
                            enemiesAlive++;
                            break;
                        }
                    }
                }
            }
        }
        if (enemiesAlive < enemiesAliveCap)
        {
            maxEnemiesReached = false;
        }
    }

    public void GoToNextRound()
    {
        if (roundCounter < rounds.Count - 1)
        {
            roundCounter++;
        }
    }
}
