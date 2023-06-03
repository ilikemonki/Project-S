using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public EnemyManager enemyManager;
    public FloatingTextController floatingTextController;
    public float moveSpeed;
    public float currentHealth;
    public float maxHealth;
    public float damageRate;
    public float defense;
    public float criticalChance, criticalChanceRate;
    public float regen, regenRate;
    public float currentMoveSpeed;
    public float magnetRange;
    public Slider healthBar;

    //I-Frames
    public float iFrameDuration;
    float iFrameTimer;
    public bool isInvincible;
    public bool onRegen;

    private void Awake()
    {
        currentHealth = maxHealth;
        currentMoveSpeed = moveSpeed;
    }
    private void Start()
    {
        healthBar.maxValue = maxHealth;
        UpdateHealthBar(maxHealth);
    }

    private void Update()
    {
        if(currentHealth < maxHealth && !onRegen && regen > 0)  //Start regen if health below 100%
        {
            onRegen = true;
            StartCoroutine(Regenerate(regen));
        }
        if(iFrameTimer > 0)
        {
            iFrameTimer -= Time.deltaTime;
        }
        else if (isInvincible)
        {
            isInvincible = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isInvincible)
        {
            return;
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyStats enemy = collision.GetComponent<EnemyStats>();
            TakeDamage(enemy.damage); //Do damage to player
            //Debug.Log(enemy.damage);
        }
    }

    public void TakeDamage(float dmg)
    {
        if (!isInvincible)
        {
            currentHealth -= dmg;
            UpdateHealthBar(-dmg);
            floatingTextController.DisplayFloatingText(transform, dmg, Color.red);
            iFrameTimer = iFrameDuration;
            isInvincible = true;
            if (currentHealth <= 0)
            {
                //Die();
            }
        }
    }
    public void CheckHealthBarVisibility()  //Show health if life is below max, else dont show
    {
        if (currentHealth >= maxHealth)
        {
            healthBar.gameObject.SetActive(false);
        }
        else
        {
            healthBar.gameObject.SetActive(true);
        }
    }
    public void ChangeMaxHealthBar(float maxAmt)   //set health bar to current max
    {
        healthBar.maxValue = maxAmt;
        UpdateHealthBar(maxAmt - currentHealth);
    }
    public void UpdateHealthBar(float amt)
    {
        healthBar.value += amt;
        CheckHealthBarVisibility();
    }

    void Die()
    {
        gameObject.SetActive(false);
    }

    public void Heal(float amt)
    {
        if (currentHealth == maxHealth) return;
        if (currentHealth + amt > maxHealth)
        {
            floatingTextController.DisplayFloatingText(transform, maxHealth - currentHealth, Color.green);
            currentHealth = maxHealth;
        }
        else
        {
            floatingTextController.DisplayFloatingText(transform, amt, Color.green);
            currentHealth += amt;
        }
        UpdateHealthBar(amt);
    }

    IEnumerator Regenerate(float amt)
    {
        Heal(amt);
        yield return new WaitForSeconds(1);
        onRegen = false;
    }

}
