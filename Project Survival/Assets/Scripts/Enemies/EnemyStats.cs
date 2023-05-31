using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public EnemyManager enemyManager;
    public EnemyMovement enemyMovement;
    public float damage;
    public float maxHealth;
    public float currentHealth;
    public float moveSpeed;
    public float currentMoveSpeed;
    public int exp;
    public SpriteRenderer spriteRenderer;
    public bool spawnInPool;    //Used in OnDisabled when populating pool as to not run code
    public bool isSpawning; //check if enemy is beginning to spawn
    // Start is called before the first frame update
    void Awake()
    {
        currentHealth = maxHealth;
        currentMoveSpeed = moveSpeed;
    }

    private void Start()
    {
        enemyMovement.SetMoveSpeed(currentMoveSpeed);
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }
    public void SetStats(float moveSpeed, float maxHealth, float damage, int exp)
    {
        this.moveSpeed = moveSpeed;
        currentMoveSpeed = moveSpeed;
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        this.damage = damage;
        this.exp = exp;
    }


    public void Die()
    {
        gameObject.SetActive(false);
    }

    //Becareful if destorying gameobject, this may get called.
    private void OnDisable()
    {
        if (!spawnInPool)
        {
            enemyManager.gameplayMananger.GainExp(exp);
            enemyManager.gameplayMananger.CalculateKillCounter();
            enemyManager.enemiesAlive--;
        }
        isSpawning = false;
    }
}
