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
    float totalBurnDamage, totalBleedDamage;
    public Rigidbody2D rb;
    public BoxCollider2D boxCollider;
    public bool knockedBack;
    public bool isSpawning; //check if enemy is beginning to spawn
    bool isDead, showDamageFlash;
    [Header("Attack")]
    public float barrageTimer, barrageCounter; //For barrage
    public float attackTimer;
    public bool isAttacking;
    public GameObject attackImage; //Exclamation mark
    //Status Timers
    float knockbackTimer, damageFlashTimer, chilledTimer, burnedTimer, shockedTimer, bleedingTimer;
    int burnOneSecCounter, bleedOneSecCounter; //When timer hits 1 sec, increment counter. 5 counters mean 5 seconds
}
