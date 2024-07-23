using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class EnemyManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemyGroup
    {
        public EnemyStats enemyStats;
        public int numberToSpawnAtOnce;
        public float spawnTimer, spawnCooldown;   //start spawning when spawner reach the amount of mobs spawned
        public int totalSpawned;
    }
    public int enemiesAlive;
    public List<EnemyStats> enemyStatsPrefabList;
    public List<EnemyGroup> enemyGroupList;
    public PlayerStats player;
    public EnemyController enemyController;
    public EnemyDetector enemyDetector;
    public GameplayManager gameplayManager;
    public Enemy baseEnemyPrefab;
    public SpawnMarks spawnMarks;
    public DropRate dropRate;
    public RectTransform backGroundRect; //spawn area
    public int enemiesAliveCap;
    public bool maxEnemiesReached;
    public List<Enemy> enemyList = new();
    float spawnPosX, spawnPosY;
    Vector2 spawnPos;
    private void Start()
    {
        PopulatePool(40);
        UpdateAllEnemyStats();
    }

    private void Update()
    {
        UpdateSpawnTimer();
    }
    public void UpdateSpawnTimer()
    {
        if (enemiesAlive < enemiesAliveCap)
        {
            for (int i = 0; i < enemyGroupList.Count; ++i)
            {
                enemyGroupList[i].spawnTimer += Time.deltaTime;
                if (enemyGroupList[i].spawnTimer >= enemyGroupList[i].spawnCooldown)
                {
                    enemyGroupList[i].spawnTimer = 0;
                    SpawnEnemies(enemyGroupList[i]);
                }
            }
        }
    }
    public void PopulatePool(int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            Enemy es = Instantiate(baseEnemyPrefab, enemyController.transform);    //Spawn, add to list, and initialize prefabs
            es.gameObject.SetActive(false);
            enemyList.Add(es);
        }
    }
    public void SpawnEnemies(EnemyGroup eGroup)
    {
        for (int i = 0; i < eGroup.numberToSpawnAtOnce; i++) //Amount to spawn at once.
        {
            for (int j = 0; j < enemyList.Count; j++)
            {
                if (j > enemyList.Count - 2)
                {
                    PopulatePool(20);
                }
                if (!enemyList[j].gameObject.activeSelf && !enemyList[j].isSpawning)
                {
                    SpawnMarkAndEnemy(enemyList[j], eGroup);
                    break;
                }
            }
        }
        if (enemiesAlive < enemiesAliveCap)
        {
            maxEnemiesReached = false;
        }
    }
    //Spawns the mark, waits a sec, then spawns the enemy on top of it.
    public void SpawnMarkAndEnemy(Enemy enemy, EnemyGroup enemyGroup)
    {
        enemy.isSpawning = true;
        enemy.rectTrans.sizeDelta = enemyGroup.enemyStats.rectTrans.sizeDelta;
        enemy.spriteRenderer.sprite = enemyGroup.enemyStats.spriteRenderer.sprite;
        enemy.spriteRenderer.transform.localScale = enemyGroup.enemyStats.spriteRenderer.transform.localScale;
        enemy.boxCollider.offset = enemyGroup.enemyStats.boxCollider.offset;
        enemy.boxCollider.size = enemyGroup.enemyStats.boxCollider.size;
        enemy.SetStats(enemyGroup.enemyStats);   //Set new stats to enemy

        spawnPosX = Random.Range(-backGroundRect.rect.width * 0.5f, backGroundRect.rect.width * 0.5f);
        spawnPosY = Random.Range(-backGroundRect.rect.height * 0.5f, backGroundRect.rect.height * 0.5f);
        spawnPos = new Vector2(spawnPosX, spawnPosY);
        enemy.transform.localPosition = spawnPos;    //set starting position when spawn

        spawnMarks.Spawn(spawnPos, enemy);
    }
    public void UpdateAllEnemyStats()
    {
        for (int i = 0; i < enemyGroupList.Count; ++i)
        {
            enemyGroupList[i].enemyStats.UpdateStats();
        }
    }

}
