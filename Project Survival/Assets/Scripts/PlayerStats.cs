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
    public Slider healthBar;
    public Image healthBarImage;
    public bool isDead;
    [Header("Base Stats")]
    public float baseMoveSpeed;
    public float baseMaxHealth;
    public float baseDefense;
    public float baseRegen, baseDegen;
    public float baseMagnetRange;
    [Header("Current Stats")]
    public float moveSpeed;
    public float currentHealth;
    public float maxHealth;
    public float defense;
    public float regen, degen;
    public float magnetRange;
    //I-Frames
    public float iFrameDuration;
    float iFrameTimer;
    public bool isInvincible;
    private void Start()
    {
        InvokeRepeating(nameof(Regenerate), 1f, 1f);
        UpdatePlayerStats();
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        UpdateHealthBar(maxHealth);
    }

    private void Update()
    {
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
        if(collision.gameObject.CompareTag("Player Skill"))
        {
            return;
        }
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Rare Enemy"))
        {
            EnemyStats enemy = collision.GetComponent<EnemyStats>();
            TakeDamage(enemy.damage, true, false); //Do damage to player
            return;
        }
        if (collision.gameObject.CompareTag("Enemy Projectile"))
        {
            EnemyProjectile enemyProj = collision.GetComponent<EnemyProjectile>();
            TakeDamage(enemyProj.enemyStats.damage, false, false); //Do damage to player
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
    public void TakeDamage(float dmg, bool triggerIframe, bool isDotDamage)
    {
        if (isDead) return;
        if (isDotDamage) //Dot damage doesn't calculate defense
        {
            if (dmg <= 0) dmg = 1;
            else dmg = Mathf.Round(dmg);
        }
        else
        {
            if (dmg <= 0 || defense >= dmg)
                dmg = 1;
            else
                dmg = Mathf.Round(dmg - defense);
        }
        if (!triggerIframe) //hit w/ projectile will not trigger IFrame
        {
            currentHealth -= dmg;
            UpdateHealthBar(-dmg);
        }
        else
        {
            if (!isInvincible)
            {
                currentHealth -= dmg;
                UpdateHealthBar(-dmg);
                iFrameTimer = iFrameDuration;
                isInvincible = true;
            }
        }
        floatingTextController.DisplayPlayerText(transform, "-" + (dmg).ToString(), Color.red);
        GameManager.totalDamageTaken += dmg;
        foreach (SkillController sc in gameplayManager.skillList) //Check trigger skill condition
        {
            if (sc.skillTrigger != null)
            {
                if (sc.skillTrigger.useDamageTakenTrigger)
                {
                    sc.skillTrigger.currentCounter += dmg;
                    if (sc.currentCooldown <= 0f)
                        sc.UseSkill();
                }
            }
        }
        if (currentHealth <= 0)
        {
            Die();
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
        isDead = true;
        //healthBarImage.gameObject.SetActive(false);
        //gameObject.SetActive(false);
    }

    public void Heal(float amt)
    {
        if (currentHealth == maxHealth || isDead) return;
        if (currentHealth + amt > maxHealth)
        {
            floatingTextController.DisplayPlayerText(transform, "+" + (maxHealth - currentHealth).ToString(), Color.green);
            currentHealth = maxHealth;
            GameManager.totalHealing += maxHealth - currentHealth;
            foreach (SkillController sc in gameplayManager.skillList) //Check trigger skill condition
            {
                if (sc.skillTrigger != null)
                {
                    if (sc.skillTrigger.useHealTrigger)
                    {
                        sc.skillTrigger.currentCounter += maxHealth - currentHealth;
                        if (sc.currentCooldown <= 0f)
                            sc.UseSkill();
                    }
                }
            }
        }
        else
        {
            floatingTextController.DisplayPlayerText(transform, "+" + (amt).ToString(), Color.green);
            currentHealth += amt;
            GameManager.totalHealing += amt;
            foreach (SkillController sc in gameplayManager.skillList) //Check trigger skill condition
            {
                if (sc.skillTrigger != null)
                {
                    if (sc.skillTrigger.useHealTrigger)
                    {
                        sc.skillTrigger.currentCounter += amt;
                        if (sc.currentCooldown <= 0f)
                            sc.UseSkill();
                    }
                }
            }
        }
        UpdateHealthBar(amt);
    }

    public void Regenerate()
    {
        if (isDead) return;
        float amt = regen - degen;
        if (amt > 0) //regen
        {
            if (amt + currentHealth > maxHealth)
            {
                Heal(maxHealth - currentHealth);
                GameManager.totalRegen += maxHealth - currentHealth;
            }
            else
            {
                Heal(amt);
                GameManager.totalRegen += amt;
            }
            GameManager.totalDegen += degen;
            foreach (SkillController sc in gameplayManager.skillList) //Check trigger skill condition
            {
                if (sc.skillTrigger != null)
                {
                    if (sc.skillTrigger.useBloodTrigger)
                    {
                        sc.skillTrigger.currentCounter += degen;
                        if (sc.currentCooldown <= 0f)
                            sc.UseSkill();
                    }
                }
            }
        }
        else if (amt < 0 && currentHealth != 1) //Degen. Do not degen after 1 hp
        {
            amt *= -1; //make it positive
            if (amt >= currentHealth)
            {
                TakeDamage(currentHealth - 1, false, true);
                GameManager.totalDegen += currentHealth - 1;
                foreach (SkillController sc in gameplayManager.skillList) //Check trigger skill condition
                {
                    if (sc.skillTrigger != null)
                    {
                        if (sc.skillTrigger.useBloodTrigger)
                        {
                            sc.skillTrigger.currentCounter += currentHealth - 1;
                            if (sc.currentCooldown <= 0f)
                                sc.UseSkill();
                        }
                    }
                }
            }
            else
            { 
                TakeDamage(amt, false, true);
                GameManager.totalDegen += amt;
                foreach (SkillController sc in gameplayManager.skillList) //Check trigger skill condition
                {
                    if (sc.skillTrigger != null)
                    {
                        if (sc.skillTrigger.useBloodTrigger)
                        {
                            sc.skillTrigger.currentCounter += amt;
                            if (sc.currentCooldown <= 0f)
                                sc.UseSkill();
                        }
                    }
                }
            }
        }
    }

    public void CheckHPBarColor()
    {
        if (currentHealth <= 0)
        {
            if (regen - degen == 0) gameplayManager.hpText.text = "<color=red>" + currentHealth.ToString();
            else if (regen - degen < 0) gameplayManager.hpText.text = "<color=red>" + currentHealth.ToString() + "<color=white> (" + (regen - degen).ToString() + "/s)";
            else gameplayManager.hpText.text = "<color=red>" + currentHealth.ToString() + "<color=white> (+" + (regen - degen).ToString() + "/s)";
            healthBarImage.gameObject.SetActive(false);
            return;
        }
        float hpPercent = currentHealth * 100 / maxHealth;
        if (hpPercent <= 30)
        {
            healthBarImage.color = Color.red;
            if (regen - degen == 0) gameplayManager.hpText.text = "<color=red>" + currentHealth.ToString();
            else if (regen - degen < 0) gameplayManager.hpText.text = "<color=red>" + currentHealth.ToString() + "<color=white> (" + (regen - degen).ToString() + "/s)";
            else gameplayManager.hpText.text = "<color=red>" + currentHealth.ToString() + "<color=white> (+" + (regen - degen).ToString() + "/s)";
        }
        else if (hpPercent <= 60)
        {
            healthBarImage.color = Color.yellow;
            if (regen - degen == 0) gameplayManager.hpText.text = "<color=yellow>" + currentHealth.ToString();
            else if (regen - degen < 0) gameplayManager.hpText.text = "<color=yellow>" + currentHealth.ToString() + "<color=white> (" + (regen - degen).ToString() + "/s)";
            else gameplayManager.hpText.text = "<color=yellow>" + currentHealth.ToString() + "<color=white> (+" + (regen - degen).ToString() + "/s)";
        }
        else
        {
            healthBarImage.color = Color.green;
            if (regen - degen == 0) gameplayManager.hpText.text = "<color=green>" + currentHealth.ToString();
            else if (regen - degen < 0) gameplayManager.hpText.text = "<color=green>" + currentHealth.ToString() + "<color=white> (" + (regen - degen).ToString() + "/s)";
            else gameplayManager.hpText.text = "<color=green>" + currentHealth.ToString() + "<color=white> (+" + (regen - degen).ToString() + "/s)";
        }
    }

    public void UpdatePlayerStats()
    {
        moveSpeed = baseMoveSpeed * (1 + gameplayManager.moveSpeedMultiplier / 100);
        maxHealth = baseMaxHealth * (1 + gameplayManager.maxHealthMultiplier / 100);
        defense = baseDefense * (1 + gameplayManager.defenseMultiplier / 100);
        regen = baseRegen + gameplayManager.regenAdditive;
        degen = baseDegen + gameplayManager.degenAdditive;
        magnetRange = baseMagnetRange * (1 + gameplayManager.magnetRangeMultiplier / 100); 
        playerCollector.SetMagnetRange(magnetRange);
    }
}
