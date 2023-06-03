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
    public Rigidbody2D rb;
    Material defaultMaterial;
    public Material damageFlashMaterial;
    public DropRate dropRate;
    public bool knockedBack;
    public bool isSpawning; //check if enemy is beginning to spawn

    private void Awake()
    {
        defaultMaterial = spriteRenderer.material;
    }
    private void Start()
    {
        enemyMovement.SetMoveSpeed(currentMoveSpeed);
    }

    public void TakeDamage(float dmg)
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(DamageFlash());
        }
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
        enemyMovement.SetMoveSpeed(currentMoveSpeed);
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        this.damage = damage;
        this.exp = exp;
    }


    public void Die()
    {
        dropRate.DropLoot(transform);
        gameObject.SetActive(false);
    }

    //Becareful if destorying gameobject, this may get called.
    private void OnDisable()
    {
        StopAllCoroutines();
        spriteRenderer.material = defaultMaterial;
        if (enemyManager != null)
        {
            enemyManager.gameplayMananger.GainExp(exp);
            enemyManager.gameplayMananger.CalculateKillCounter();
            enemyManager.enemiesAlive--;
        }
        knockedBack = false;
        isSpawning = false;
    }
    public IEnumerator ResetVelocity()
    {
        yield return new WaitForSeconds(0.15f);
        rb.velocity = Vector2.zero;
        knockedBack = false;
    }

    public void KnockBack(Vector2 power)
    {
        if (gameObject.activeSelf)
        {
            knockedBack = true;
            rb.AddForce(power, ForceMode2D.Impulse);
            StartCoroutine(ResetVelocity());
        }
    }

    public IEnumerator DamageFlash()
    {
        spriteRenderer.material = damageFlashMaterial;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.material = defaultMaterial;
    }
    
}
