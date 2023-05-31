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
        public EnemyStats enemyPrefab;
        public int numberToSpawn;
        public int whenToSpawn;   //start spawning when spawner reach the amount of mobs spawned
        public int currentSpawned;
    }
    public int enemiesAlive;
    public List<Round> rounds;
    public PlayerStats player;
    public EnemyController enemyController;
    public GameplayManager gameplayMananger;
    public GameObject basePrefab;
    public SpawnMarks spawnMarks;
    public float spawnTimer;
    public int enemiesAliveCap;
    public bool maxEnemiesReached;

    private void Start()
    {
        PopulatePool(40);
        CalculateTotalEnemiesInRound();
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if(spawnTimer >= rounds[gameplayMananger.roundCounter].spawnInterval)
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

    protected virtual void CheckPoolAmount()
    {
        int inactive = 0;
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
        if (rounds[gameplayMananger.roundCounter].currentTotalSpawned < rounds[gameplayMananger.roundCounter].totalEnemiesInRound && !maxEnemiesReached)   //Check if there is still mobs left to spawn
        {
            foreach(var eGroup in rounds[gameplayMananger.roundCounter].enemyGroups)     //For each enemy groups in a round, spawn the groups
            {
                if(eGroup.currentSpawned < eGroup.numberToSpawn && (eGroup.whenToSpawn <= rounds[gameplayMananger.roundCounter].currentTotalSpawned))      //Check if min number of mobs of this type have been spawned
                {
                    if(enemiesAlive >= enemiesAliveCap) //Stop spawning if cap is reached
                    {
                        maxEnemiesReached = true;
                        return;
                    }

                    for (int i = 0; i < enemyController.enemyList.Count; i++)   //Find an inactive enemy to spawn
                    {
                        if (!enemyController.enemyList[i].isActiveAndEnabled && !enemyController.enemyList[i].isSpawning)
                        {
                            StartCoroutine(SpawnMarkAndEnemy(i, eGroup.enemyPrefab));
                            eGroup.currentSpawned++;
                            rounds[gameplayMananger.roundCounter].currentTotalSpawned++;
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

    IEnumerator SpawnMarkAndEnemy(int indexEnemyToSpawn, EnemyStats prefab )
    {
        enemyController.enemyList[indexEnemyToSpawn].isSpawning = true;
        Vector2 spawnPos = new Vector2(player.transform.position.x + Random.Range(-10f, 10f), player.transform.position.y + Random.Range(-10f, 10f));
        enemyController.enemyList[indexEnemyToSpawn].transform.position = spawnPos;    //set starting position when spawn
        enemyController.enemyList[indexEnemyToSpawn].spriteRenderer.sprite = prefab.spriteRenderer.sprite;
        enemyController.enemyList[indexEnemyToSpawn].transform.localScale = prefab.transform.localScale;
        enemyController.enemyList[indexEnemyToSpawn].SetStats(prefab.moveSpeed, prefab.maxHealth, prefab.damage, prefab.exp);   //Set new stats to enemy
        int indexToDespawn = spawnMarks.Spawn(spawnPos);
        yield return new WaitForSeconds(1f);
        spawnMarks.Despawn(indexToDespawn);
        enemyController.enemyList[indexEnemyToSpawn].gameObject.SetActive(true);
        enemyController.enemyList[indexEnemyToSpawn].isSpawning = false;
        enemiesAlive++;


    }

}
