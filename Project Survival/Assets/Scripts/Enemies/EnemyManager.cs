using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

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
        public EnemyStats enemyStats;
        public int numberToSpawn;
        public int whenToSpawn;   //start spawning when spawner reach the amount of mobs spawned
        public int currentSpawned;
        //public Enemy enemy;
    }
    public int enemiesAlive;
    public List<Round> rounds;
    public PlayerStats player;
    public EnemyController enemyController;
    public EnemyDetector enemyDetector;
    public GameplayManager gameplayManager;
    public Enemy baseEnemyPrefab;
    public SpawnMarks spawnMarks;
    public DropRate dropRate;
    public RectTransform backGroundRect;
    public float spawnTimer;
    public int enemiesAliveCap;
    public bool maxEnemiesReached;
    public List<Enemy> enemyList = new();
    float spawnPosX, spawnPosY;
    Vector2 spawnPos;
    private void Start()
    {
        
        PopulatePool(40);
        CalculateTotalEnemiesInRound();
        UpdateAllEnemyStats();
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if(spawnTimer >= rounds[gameplayManager.waveCounter].spawnInterval)
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
    public void PopulatePool(int spawnAmount)
    {
        for (int i = 0; i < 100; i++)
        {
            Enemy es = Instantiate(baseEnemyPrefab, enemyController.transform);    //Spawn, add to list, and initialize prefabs
            es.gameObject.SetActive(false);
            enemyList.Add(es);
        }
    }
    public void SpawnEnemies()
    {
        if (rounds[gameplayManager.waveCounter].currentTotalSpawned < rounds[gameplayManager.waveCounter].totalEnemiesInRound && !maxEnemiesReached)   //Check if there is still mobs left to spawn
        {
            foreach(var eGroup in rounds[gameplayManager.waveCounter].enemyGroups)     //For each enemy groups in a round, spawn the groups
            {
                if(eGroup.currentSpawned < eGroup.numberToSpawn && (eGroup.whenToSpawn <= rounds[gameplayManager.waveCounter].currentTotalSpawned))      //Check if min number of mobs of this type have been spawned
                {
                    if(enemiesAlive >= enemiesAliveCap) //Stop spawning if cap is reached
                    {
                        maxEnemiesReached = true;
                        return;
                    }
                    for (int i = 0; i < enemyList.Count; i++)   //Find an inactive enemy to spawn
                    {
                        if (i > enemyList.Count - 2)    //Check pool, add more if neccessary
                        {
                            PopulatePool(5);
                        }
                        if (!enemyList[i].gameObject.activeSelf && !enemyList[i].isSpawning)
                        {
                            SpawnMarkAndEnemy(enemyList[i], eGroup, false);
                            eGroup.currentSpawned++;
                            rounds[gameplayManager.waveCounter].currentTotalSpawned++;
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
    //Spawns the mark, waits a sec, then spawns the enemy on top of it.
    public void SpawnMarkAndEnemy(Enemy enemy, EnemyGroup enemyGroup, bool isRare)
    {
        enemy.isSpawning = true;
        spawnPosX = Random.Range(-backGroundRect.rect.width * 0.5f, backGroundRect.rect.width * 0.5f);
        spawnPosY = Random.Range(-backGroundRect.rect.height * 0.5f, backGroundRect.rect.height * 0.5f);
        spawnPos = new Vector2(spawnPosX, spawnPosY);
        enemy.transform.localPosition = spawnPos;    //set starting position when spawn
        enemy.spriteRenderer.sprite = enemyGroup.enemyStats.spriteRenderer.sprite;
        enemy.spriteRenderer.transform.localScale = enemyGroup.enemyStats.spriteRenderer.transform.localScale;
        enemy.boxCollider.offset = enemyGroup.enemyStats.boxCollider.offset;
        enemy.boxCollider.size = enemyGroup.enemyStats.boxCollider.size;

        enemy.SetStats(enemyGroup.enemyStats);   //Set new stats to enemy
        spawnMarks.Spawn(spawnPos, enemy);
    }
    public void UpdateAllEnemyStats()
    {
        for (int i = 0; i < rounds.Count; ++i)
        {
            foreach (var eGroup in rounds[i].enemyGroups)
            {
                eGroup.enemyStats.UpdateStats();
            }
        }
    }

}
