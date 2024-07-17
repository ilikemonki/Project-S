using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStats enemyStats;

    //Stats
    public float currentHealth;
    public float moveSpeed; //baseMoveSpeed is the current movespeed, use to reset current movespeed back to normal when chilled.
    public bool chilled, burned, shocked, bleeding;
    public List<float> topAilmentsEffect;
    public SpriteRenderer spriteRenderer;
    public float totalBurnDamage, totalBleedDamage;
    public Rigidbody2D rb;
    public BoxCollider2D boxCollider;
    public bool knockedBack;
    public bool isSpawning; //check if enemy is beginning to spawn
    public bool isDead, showDamageFlash;
    [Header("Attack")]
    public float barrageTimer, barrageCounter; //For barrage
    public float attackTimer;
    public bool isAttacking;
    public GameObject attackImage; //Exclamation mark
    //Status Timers
    public float knockbackTimer, damageFlashTimer, chilledTimer, burnedTimer, shockedTimer, bleedingTimer;
    public int burnOneSecCounter, bleedOneSecCounter; //When timer hits 1 sec, increment counter. 5 counters mean 5 seconds

    public void SetStats(EnemyStats enemy)
    {
        enemyStats = enemy;
        moveSpeed = enemyStats.moveSpeed;
        currentHealth = enemyStats.maxHealth;
        for (int i = 0; i < topAilmentsEffect.Count; i++)
        {
            topAilmentsEffect[i] = 0;
        }
        chilled = false; burned = false; shocked = false; bleeding = false;
        totalBurnDamage = 0; totalBleedDamage = 0;
        attackTimer = 0;
        knockbackTimer = 0; damageFlashTimer = 0;
        burnedTimer = 0; chilledTimer = 0; shockedTimer = 0; bleedingTimer = 0;
        burnOneSecCounter = 0; bleedOneSecCounter = 0;
        if (enemyStats.barrageAttack)
        {
            barrageTimer = 0; barrageCounter = 0;
        }
    }
    private void OnDisable()
    {
        if (enemyStats != null)
        {
            spriteRenderer.material = enemyStats.defaultMaterial;
            try
            {
                enemyStats.enemyManager.enemyDetector.RemoveEnemyFromList(this);
            }
            catch { Debug.Log("Cannot remove enemy from detector list"); }
        }
        isAttacking = false;
        attackImage.SetActive(false);
        knockedBack = false;
        isSpawning = false;
    }
    protected virtual void OnEnable()
    {
        isDead = false;
    }
}
