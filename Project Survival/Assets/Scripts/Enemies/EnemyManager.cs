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
        public Enemy enemyStat;
    }
    public int enemiesAlive;
    public List<Round> rounds;
    public PlayerStats player;
    public EnemyController enemyController;
    public EnemyDetector enemyDetector;
    public GameplayManager gameplayManager;
    public Enemy basePrefab;
    public SpawnMarks spawnMarks;
    public DropRate dropRate;
    public RectTransform backGroundRect;
    public float spawnTimer;
    public int enemiesAliveCap;
    public bool maxEnemiesReached;
    public List<Enemy> enemyList = new();
    public EnemyStats baseEnemy;
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
            Enemy es = Instantiate(basePrefab, enemyController.transform);    //Spawn, add to list, and initialize prefabs
            es.gameObject.SetActive(false);
            es.transform.position = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            es.enemyStats = baseEnemy;
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
        //if (spawnPosX > -100 && spawnPosX < 100)
        //{
        //    if (spawnPosX > 0)
        //        spawnPosX += 100;
        //    else spawnPosX -= 100;
        //}
        //if (spawnPosY > -100 && spawnPosY < 100)
        //{
        //    if (spawnPosY > 0)
        //        spawnPosY += 100;
        //    else spawnPosY -= 100;
        //}
        //if (player.transform.localPosition.x + spawnPosX > 1950 || player.transform.localPosition.x + spawnPosX < -1950)
        //    spawnPosX = player.transform.localPosition.x + (-1 * spawnPosX);
        //else
        //    spawnPosX = player.transform.localPosition.x + spawnPosX;
        //if (player.transform.localPosition.y + spawnPosY > 1400 || player.transform.localPosition.y + spawnPosY < -1400)
        //    spawnPosY = player.transform.localPosition.y + (-1 * spawnPosY);
        //else
        //    spawnPosY = player.transform.localPosition.y + spawnPosY;
        spawnPos = new Vector2(spawnPosX, spawnPosY);
        enemy.transform.localPosition = spawnPos;    //set starting position when spawn
        enemy.spriteRenderer.sprite = enemyGroup.enemyPrefab.spriteRenderer.sprite;
        enemy.spriteRenderer.transform.localScale = enemyGroup.enemyPrefab.spriteRenderer.transform.localScale;
        enemy.boxCollider.offset = enemyGroup.enemyPrefab.boxCollider.offset;
        enemy.boxCollider.size = enemyGroup.enemyPrefab.boxCollider.size;
        //enemy.knockBackImmune = enemyGroup.enemyPrefab.knockBackImmune;
        enemy.chilled = false; enemy.burned = false; enemy.shocked = false; enemy.bleeding = false;
        for (int i = 0; i < enemy.topAilmentsEffect.Count; i++)
        {
            enemy.topAilmentsEffect[i] = 0;
        }
        //enemy.SetStats(enemyGroup.enemyStat);   //Set new stats to enemy
        spawnMarks.Spawn(spawnPos, enemy);
        if (enemy.burned)
            Debug.Log(enemy + "is spawned Burned");
    }
    public void UpdateAllEnemyStats()
    {
        //for (int i = 0; i < rounds.Count; ++i)
        //{
        //    foreach (var eGroup in rounds[i].enemyGroups)
        //    {
        //        for (int j = 0; j < gameplayManager.damageTypeMultiplier.Count; j++)
        //        {
        //            if (eGroup.enemyPrefab.damageTypes[j] > 0)
        //                eGroup.enemyStat.damageTypes[j] = eGroup.enemyPrefab.damageTypes[j] * (1 + (gameplayManager.enemyDamageMultiplier + gameplayManager.enemyDamageTypeMultiplier[j]) / 100);
        //            eGroup.enemyStat.reductions[j] = eGroup.enemyPrefab.reductions[j] + gameplayManager.enemyReductions[j];
        //        }
        //        eGroup.enemyStat.moveSpeed = eGroup.enemyPrefab.baseMoveSpeed * (1 + gameplayManager.enemyMoveSpeedMultiplier / 100);
        //        eGroup.enemyStat.maxHealth = eGroup.enemyPrefab.maxHealth * (1 + gameplayManager.enemyMaxHealthMultiplier / 100);
        //        eGroup.enemyStat.exp = eGroup.enemyPrefab.exp * (1 + gameplayManager.expMultiplier / 100);
        //        eGroup.enemyStat.attackCooldown = eGroup.enemyPrefab.attackCooldown * (1 + gameplayManager.enemyAttackCooldownMultiplier / 100);
        //        if (eGroup.enemyPrefab.canAttack == true)
        //        {
        //            eGroup.enemyStat.projectile = eGroup.enemyPrefab.projectile + gameplayManager.enemyProjectileAdditive;
        //            eGroup.enemyStat.projectileSpeed = eGroup.enemyPrefab.projectileSpeed * (1 + gameplayManager.enemyProjectileTravelSpeedMultiplier / 100);
        //            //eGroup.enemyStat.projectileSize = eGroup.enemyPrefab.projectileSize * (1 + gameplayManager.enemyProjectileSizeMultiplier / 100);
        //        }
        //        else
        //        {
        //            eGroup.enemyStat.projectile = 0;
        //            eGroup.enemyStat.projectileSpeed = 0;
        //            eGroup.enemyStat.projectileSize = 0;
        //        }
        //    }
        //}
    }

}
