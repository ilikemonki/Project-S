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
        public float moveSpeed;
        public int exp;
        public float attackCooldown;
        public float projectileSpeed;
    }
    public int enemiesAlive;
    public List<Round> rounds;
    public PlayerStats player;
    public EnemyController enemyController;
    public GameplayManager gameplayManager;
    public FloatingTextController floatingTextController;
    public EnemyStats basePrefab;
    public SpawnMarks spawnMarks;
    public DropRate dropRate;
    public float spawnTimer;
    public int enemiesAliveCap;
    public bool maxEnemiesReached;
    public List<EnemyStats> enemyList = new();
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
        for (int i = 0; i < spawnAmount; i++)
        {
            EnemyStats es = Instantiate(basePrefab, enemyController.transform);    //Spawn, add to list, and initialize prefabs
            es.gameObject.SetActive(false);
            es.enemyManager = this;
            es.dropRate = dropRate;
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
                        if (!enemyList[i].isActiveAndEnabled && !enemyList[i].isSpawning)
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
    public void SpawnMarkAndEnemy(EnemyStats enemy, EnemyGroup enemyGroup, bool isRare)
    {
        enemy.isSpawning = true;
        spawnPosX = Random.Range(-1000f, 1001f);
        spawnPosY = Random.Range(-1000f, 1001f);
        if (spawnPosX > -100 && spawnPosX < 100)
        {
            if (spawnPosX > 0)
                spawnPosX += 100;
            else spawnPosX -= 100;
        }
        if (spawnPosY > -100 && spawnPosY < 100)
        {
            if (spawnPosY > 0)
                spawnPosY += 100;
            else spawnPosY -= 100;
        }
        if (player.transform.localPosition.x + spawnPosX > 1950 || player.transform.localPosition.x + spawnPosX < -1950)
            spawnPosX = player.transform.localPosition.x + (-1 * spawnPosX);
        else
            spawnPosX = player.transform.localPosition.x + spawnPosX;
        if (player.transform.localPosition.y + spawnPosY > 1400 || player.transform.localPosition.y + spawnPosY < -1400)
            spawnPosY = player.transform.localPosition.y + (-1 * spawnPosY);
        else
            spawnPosY = player.transform.localPosition.y + spawnPosY;
        spawnPos = new Vector2(spawnPosX, spawnPosY);
        enemy.transform.localPosition = spawnPos;    //set starting position when spawn
        enemy.spriteRenderer.sprite = enemyGroup.enemyPrefab.spriteRenderer.sprite;
        enemy.spriteRenderer.transform.localScale = enemyGroup.enemyPrefab.spriteRenderer.transform.localScale;
        enemy.boxCollider.offset = enemyGroup.enemyPrefab.boxCollider.offset;
        enemy.boxCollider.size = enemyGroup.enemyPrefab.boxCollider.size;
        enemy.knockBackImmune = enemyGroup.enemyPrefab.knockBackImmune;
        enemy.SetNonModifiedStats(enemyGroup.enemyPrefab.attackRange,
            enemyGroup.enemyPrefab.projectiles, enemyGroup.enemyPrefab.spreadAttack,
            enemyGroup.enemyPrefab.circleAttack, enemyGroup.enemyPrefab.burstAttack,
            enemyGroup.enemyPrefab.projectileRange);
        enemy.SetStats(enemyGroup.moveSpeed, enemyGroup.maxHealth, enemyGroup.damage, enemyGroup.exp, enemyGroup.attackCooldown, enemyGroup.projectileSpeed);   //Set new stats to enemy
        spawnMarks.Spawn(spawnPos, enemy);
    }
    public void UpdateAllEnemyStats()
    {
        for (int i = 0; i < rounds.Count; ++i)
        {
            foreach (var eGroup in rounds[i].enemyGroups)
            {
                eGroup.damage = eGroup.enemyPrefab.damage * (1 + gameplayManager.enemyDamageMultiplier / 100);
                eGroup.moveSpeed = eGroup.enemyPrefab.baseMoveSpeed * (1 + gameplayManager.enemyMoveSpeedMultiplier / 100);
                eGroup.maxHealth = eGroup.enemyPrefab.maxHealth * (1 + gameplayManager.enemyMaxHealthMultiplier / 100);
                eGroup.exp = eGroup.enemyPrefab.exp * (1 + gameplayManager.enemyExpMultiplier / 100);
                eGroup.attackCooldown = eGroup.enemyPrefab.attackCooldown * (1 + gameplayManager.enemyAttackCooldownMultiplier / 100);
                eGroup.projectileSpeed = eGroup.enemyPrefab.projectileSpeed * (1 + gameplayManager.enemyProjectileSpeedMultiplier / 100);
            }
        }
    }

}
