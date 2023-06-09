using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;
public class PlayerStats : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public EnemyManager enemyManager;
    public GameplayManager gameplayManager;
    public PlayerCollector playerCollector;
    public FloatingTextController floatingTextController;
    public float baseMoveSpeed;
    public float baseMaxHealth;
    public float baseDefense;
    public float baseRegen;
    public float baseMagnetRange;
    public Slider healthBar;
    public Image healthBarImage; 
    public float moveSpeed;
    public float currentHealth;
    public float maxHealth;
    public float defense;
    public float regen;
    public float magnetRange;
    public bool onRegen;
    float regenTemp;

    //I-Frames
    public float iFrameDuration;
    float iFrameTimer;
    public bool isInvincible;
    private void Start()
    {
        UpdatePlayerStats();
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        UpdateHealthBar(maxHealth);
    }

    private void Update()
    {
        if(currentHealth < maxHealth && !onRegen && regen > 0)  //Start regen if health below 100%
        {
            onRegen = true;
            Timing.RunCoroutine(Regenerate(regen));
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
        if (isInvincible || playerMovement.isDashing)
        {
            return;
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyStats enemy = collision.GetComponent<EnemyStats>();
            TakeDamage(enemy.damage, false); //Do damage to player
            return;
        }
        if (collision.gameObject.CompareTag("Enemy Projectile"))
        {
            EnemyProjectile enemyProj = collision.GetComponent<EnemyProjectile>();
            TakeDamage(enemyProj.enemyStats.damage, true); //Do damage to player
            enemyProj.gameObject.SetActive(false);
            return;
        }
        ICollectibles collectible = collision.GetComponent<ICollectibles>();
        if (collision.tag.Contains("Coin"))
        {
            collision.gameObject.SetActive(false);
            gameplayManager.GainCoins(int.Parse(collision.tag.Substring(5)));
        }
        else if (collision.CompareTag("Health Potion"))
        {
            collision.gameObject.SetActive(false);
            Heal(maxHealth * 0.15f);
        }
        else if (collision.CompareTag("Magnet"))
        {
            collision.gameObject.SetActive(false);
            playerCollector.MagnetCollectible();
        }
        else if (collision.CompareTag("Class Star"))
        {
            collision.gameObject.SetActive(false);
            gameplayManager.GainClassStars(1);
        }
        playerCollector.collectibles.Remove(collectible);
    }
    public void TakeDamage(float dmg, bool isProjectileDamage)
    {
        if (dmg <= 0) dmg = 1;
        dmg = Mathf.Round(dmg);
        if (isProjectileDamage)
        {
            currentHealth -= dmg;
            UpdateHealthBar(-dmg);
            floatingTextController.DisplayFloatingText(transform, dmg, Color.red);
        }
        else
        {
            if (!isInvincible)
            {
                currentHealth -= dmg;
                UpdateHealthBar(-dmg);
                floatingTextController.DisplayFloatingText(transform, dmg, Color.red);
                iFrameTimer = iFrameDuration;
                isInvincible = true;
            }
        }
        if (currentHealth <= 0)
        {
            //Die();
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
    public void UpdateMaxHealthBar(float maxAmt)   //set health bar to current max
    {
        healthBar.maxValue = maxAmt;
        CheckHPBarColor();
        UpdateHealthBar(maxAmt - currentHealth);
    }
    public void UpdateHealthBar(float amt)
    {
        healthBar.value += amt;
        CheckHPBarColor();
        CheckHealthBarVisibility();
    }

    void Die()
    {
        healthBarImage.gameObject.SetActive(false);
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

    IEnumerator<float> Regenerate(float amt)
    {
        regenTemp += amt;
        if (regenTemp >= 1)
        {
            Heal(Mathf.Floor(regenTemp));
            regenTemp -= Mathf.Floor(regenTemp);
        }
        yield return Timing.WaitForSeconds(1);
        onRegen = false;
    }

    public void CheckHPBarColor()
    {
        if (currentHealth <= 0)
        {
            gameplayManager.hpText.text = "<color=red>" + currentHealth.ToString();
            healthBarImage.gameObject.SetActive(false);
            return;
        }
        float hpPercent = currentHealth * 100 / maxHealth;
        if (hpPercent <= 30)
        {
            healthBarImage.color = Color.red;
            gameplayManager.hpText.text = "<color=red>" + currentHealth.ToString();
        }
        else if (hpPercent <= 60)
        {
            healthBarImage.color = Color.yellow;
            gameplayManager.hpText.text = "<color=yellow>" + currentHealth.ToString();
        }
        else
        {
            healthBarImage.color = Color.green;
            gameplayManager.hpText.text = "<color=green>" + currentHealth.ToString();
        }
    }

    public void UpdatePlayerStats()
    {
        moveSpeed = baseMoveSpeed * (1 + gameplayManager.moveSpeedMultiplier / 100);
        maxHealth = baseMaxHealth * (1 + gameplayManager.maxHealthMultiplier / 100);
        defense = gameplayManager.defenseAdditive;
        regen = gameplayManager.regenAdditive;
        magnetRange = baseMagnetRange * (1 + gameplayManager.magnetRangeMultiplier / 100); 
        playerCollector.SetMagnetRange(magnetRange);
    }
}
