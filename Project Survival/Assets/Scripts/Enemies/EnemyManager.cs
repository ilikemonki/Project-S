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
        public EnemyStats enemyPrefab;
        public int numberToSpawn;
        public int whenToSpawn;   //start spawning when spawner reach the amount of mobs spawned
        public int currentSpawned;
        //Make changes to these stats only when enemy stat changes happen. This way we don't need to keep calculating their stats everytime an enemy spawns.
        public float damage;
        public float maxHealth;
        public float baseMoveSpeed;
        public int exp;
    }
    public int enemiesAlive;
    public List<Round> rounds;
    public PlayerStats player;
    public EnemyController enemyController;
    public GameplayManager gameplayMananger;
    public FloatingTextController floatingTextController;
    public GameObject basePrefab;
    public SpawnMarks spawnMarks;
    public DropRate dropRate;
    public float spawnTimer;
    public int enemiesAliveCap;
    public bool maxEnemiesReached;
    private void Start()
    {
        PopulatePool(40);
        CalculateTotalEnemiesInRound();
        UpdateAllEnemyStats();
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

    protected virtual void PopulatePool(int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject enemy = Instantiate(basePrefab, enemyController.transform);    //Spawn, add to list, and initialize prefabs
            enemy.SetActive(false);
            EnemyStats es = enemy.GetComponent<EnemyStats>();
            es.enemyManager = this;
            es.dropRate = dropRate;
            enemyController.AddToList(es);
        }
    }
        public void SpawnEnemies()
    {
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
                        if (i > enemyController.enemyList.Count - 5)    //Check pool, add more if neccessary
                        {
                            PopulatePool(5);
                        }
                        if (!enemyController.enemyList[i].isActiveAndEnabled && !enemyController.enemyList[i].isSpawning)
                        {
                            Timing.RunCoroutine(SpawnMarkAndEnemy(i, eGroup));
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
    //Spawns the mark, waits a sec, then spawns the enemy on top of it.
    IEnumerator<float> SpawnMarkAndEnemy(int indexEnemyToSpawn, EnemyGroup enemyGroup )
    {
        enemyController.enemyList[indexEnemyToSpawn].isSpawning = true;
        Vector2 spawnPos = new Vector2(player.transform.position.x + Random.Range(-10f, 10f), player.transform.position.y + Random.Range(-10f, 10f));
        enemyController.enemyList[indexEnemyToSpawn].transform.position = spawnPos;    //set starting position when spawn
        enemyController.enemyList[indexEnemyToSpawn].spriteRenderer.sprite = enemyGroup.enemyPrefab.spriteRenderer.sprite;
        enemyController.enemyList[indexEnemyToSpawn].spriteRenderer.transform.localScale = enemyGroup.enemyPrefab.spriteRenderer.transform.localScale;
        enemyController.enemyList[indexEnemyToSpawn].boxCollider.offset = enemyGroup.enemyPrefab.boxCollider.offset;
        enemyController.enemyList[indexEnemyToSpawn].boxCollider.size = enemyGroup.enemyPrefab.boxCollider.size;
        enemyController.enemyList[indexEnemyToSpawn].SetStats(enemyGroup.baseMoveSpeed, enemyGroup.maxHealth, enemyGroup.damage, enemyGroup.exp);   //Set new stats to enemy
        int indexToDespawn = spawnMarks.Spawn(spawnPos);
        yield return Timing.WaitForSeconds(1f);
        spawnMarks.Despawn(indexToDespawn);
        enemyController.enemyList[indexEnemyToSpawn].gameObject.SetActive(true);
        enemyController.enemyList[indexEnemyToSpawn].isSpawning = false;
        enemiesAlive++;
    }

    public void UpdateAllEnemyStats()
    {
        for (int i = 0; i < rounds.Count; ++i)
        {
            foreach (var eGroup in rounds[i].enemyGroups)
            {
                eGroup.damage = eGroup.enemyPrefab.damage * (1 + gameplayMananger.enemyDamageMultiplier / 100);
                eGroup.baseMoveSpeed = eGroup.enemyPrefab.moveSpeed * (1 + gameplayMananger.enemyMoveSpeedMultiplier / 100);
                eGroup.maxHealth = eGroup.enemyPrefab.maxHealth * (1 + gameplayMananger.enemyMaxHealthMultiplier / 100);
                eGroup.exp = eGroup.enemyPrefab.exp * (1 + gameplayMananger.enemyExpMultiplier / 100);
            }
        }
    }

}
