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
    public GameplayManager gameplayMananger;
    public FloatingTextController floatingTextController;
    public EnemyStats basePrefab;
    public SpawnMarks spawnMarks;
    public DropRate dropRate;
    public float spawnTimer;
    public int enemiesAliveCap;
    public bool maxEnemiesReached;
    public List<EnemyStats> enemyList = new();
    public List<EnemyStats> rareEnemyList = new();
    public EnemyStats baseRarePrefab;
    public GameObject rareEnemyParent;
    public int rareSpawnChance, rareSpawnChanceRange;
    float spawnPosX, spawnPosY;
    Vector2 spawnPos;
    private void Start()
    {
        PopulatePool(40);
        PopulateRarePool(10);
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
    public void PopulateRarePool(int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            EnemyStats es = Instantiate(baseRarePrefab, rareEnemyParent.transform);    //Spawn, add to list, and initialize prefabs
            es.gameObject.SetActive(false);
            es.enemyManager = this;
            es.dropRate = dropRate;
            rareEnemyList.Add(es);
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
                    if (Random.Range(1, rareSpawnChanceRange + 1) <= rareSpawnChance)   //Chance to spawn rare mob
                    {
                        for (int i = 0; i < rareEnemyList.Count; i++)   //Find an inactive enemy to spawn
                        {
                            if (i > rareEnemyList.Count - 2)    //Check pool, add more if neccessary
                            {
                                PopulateRarePool(5);
                            }
                            if (!rareEnemyList[i].isActiveAndEnabled && !rareEnemyList[i].isSpawning)
                            {
                                Timing.RunCoroutine(SpawnMarkAndEnemy(i, eGroup, rareEnemyList, true));
                                eGroup.currentSpawned++;
                                rounds[gameplayMananger.roundCounter].currentTotalSpawned++;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < enemyList.Count; i++)   //Find an inactive enemy to spawn
                        {
                            if (i > enemyList.Count - 2)    //Check pool, add more if neccessary
                            {
                                PopulatePool(5);
                            }
                            if (!enemyList[i].isActiveAndEnabled && !enemyList[i].isSpawning)
                            {
                                Timing.RunCoroutine(SpawnMarkAndEnemy(i, eGroup, enemyList, false));
                                eGroup.currentSpawned++;
                                rounds[gameplayMananger.roundCounter].currentTotalSpawned++;
                                break;
                            }
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
    IEnumerator<float> SpawnMarkAndEnemy(int indexEnemyToSpawn, EnemyGroup enemyGroup, List<EnemyStats> enemyList, bool isRare)
    {
        enemyList[indexEnemyToSpawn].isSpawning = true;
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
        enemyList[indexEnemyToSpawn].transform.localPosition = spawnPos;    //set starting position when spawn
        enemyList[indexEnemyToSpawn].spriteRenderer.sprite = enemyGroup.enemyPrefab.spriteRenderer.sprite;
        enemyList[indexEnemyToSpawn].spriteRenderer.transform.localScale = enemyGroup.enemyPrefab.spriteRenderer.transform.localScale;
        enemyList[indexEnemyToSpawn].boxCollider.offset = enemyGroup.enemyPrefab.boxCollider.offset;
        enemyList[indexEnemyToSpawn].boxCollider.size = enemyGroup.enemyPrefab.boxCollider.size;
        enemyList[indexEnemyToSpawn].knockBackImmune = enemyGroup.enemyPrefab.knockBackImmune;
        enemyList[indexEnemyToSpawn].SetNonModifiedStats(enemyGroup.enemyPrefab.attackRange, enemyGroup.enemyPrefab.projectiles, enemyGroup.enemyPrefab.spreadAttack, enemyGroup.enemyPrefab.circleAttack, enemyGroup.enemyPrefab.projectileDuration);
        if (isRare)
        {
            enemyList[indexEnemyToSpawn].SetStats(enemyGroup.enemyPrefab.baseMoveSpeed * (1 + gameplayMananger.rareMoveSpeedMultiplier / 100),
                Mathf.Round(enemyGroup.enemyPrefab.maxHealth * (1 + gameplayMananger.rareHealthMultiplier / 100)),
                Mathf.Round(enemyGroup.enemyPrefab.damage * (1 + gameplayMananger.rareDamageMultiplier / 100)),
                (int)Mathf.Round(enemyGroup.enemyPrefab.exp * (1 + gameplayMananger.rareExpMultiplier / 100)),
                Mathf.Round(enemyGroup.enemyPrefab.attackCooldown * (1 + gameplayMananger.rareAttackCooldownMultiplier / 100)),
                enemyGroup.projectileSpeed);
        }
        else
            enemyList[indexEnemyToSpawn].SetStats(enemyGroup.moveSpeed, enemyGroup.maxHealth, enemyGroup.damage, enemyGroup.exp, enemyGroup.attackCooldown, enemyGroup.projectileSpeed);   //Set new stats to enemy
        int indexToDespawn = spawnMarks.Spawn(spawnPos);
        yield return Timing.WaitForSeconds(1f);
        spawnMarks.Despawn(indexToDespawn);
        enemyList[indexEnemyToSpawn].gameObject.SetActive(true);
        enemyList[indexEnemyToSpawn].isSpawning = false;
        enemiesAlive++;
    }
    public void UpdateAllEnemyStats()
    {
        for (int i = 0; i < rounds.Count; ++i)
        {
            foreach (var eGroup in rounds[i].enemyGroups)
            {
                eGroup.damage = eGroup.enemyPrefab.damage * (1 + gameplayMananger.enemyDamageMultiplier / 100);
                eGroup.moveSpeed = eGroup.enemyPrefab.baseMoveSpeed * (1 + gameplayMananger.enemyMoveSpeedMultiplier / 100);
                eGroup.maxHealth = eGroup.enemyPrefab.maxHealth * (1 + gameplayMananger.enemyMaxHealthMultiplier / 100);
                eGroup.exp = eGroup.enemyPrefab.exp * (1 + gameplayMananger.enemyExpMultiplier / 100);
                eGroup.attackCooldown = eGroup.enemyPrefab.attackCooldown * (1 + gameplayMananger.enemyAttackCooldownMultiplier / 100);
                eGroup.projectileSpeed = eGroup.enemyPrefab.projectileSpeed * (1 + gameplayMananger.enemyProjectileSpeedMultiplier / 100);
            }
        }
    }

}
