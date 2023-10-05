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
    public SpriteRenderer spriteRenderer;
    Material defaultMaterial;
    public Material damageFlashMaterial;
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
    private void Awake()
    {
        defaultMaterial = spriteRenderer.material;
    }
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
            foreach (InventoryManager.Skill sc in gameplayManager.inventory.skillList) //Check damageTaken trigger skill condition
            {
                if (sc.skillController != null)
                {
                    if (sc.skillController.skillTrigger.useContactTrigger)
                    {
                        sc.skillController.skillTrigger.currentCounter++;
                        if (sc.skillController.currentCooldown <= 0f)
                            sc.skillController.UseSkill();
                    }
                }
            }
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
        else if (collision.CompareTag("Skill Orb"))
        {
            collision.gameObject.SetActive(false);
            gameplayManager.inventory.AddCollectibleIntoInventory(collision.name);
        }
        else if (collision.CompareTag("Skill Gem"))
        {
            collision.gameObject.SetActive(false);
            gameplayManager.inventory.AddCollectibleIntoInventory(collision.name);
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
            if (gameObject.activeSelf)
            {
                Timing.RunCoroutine(DamageFlash());
            }
            currentHealth -= dmg;
            UpdateHealthBar(-dmg);
        }
        else
        {
            if (!isInvincible)
            {
                if (gameObject.activeSelf)
                {
                    Timing.RunCoroutine(DamageFlash());
                }
                currentHealth -= dmg;
                UpdateHealthBar(-dmg);
                iFrameTimer = iFrameDuration;
                isInvincible = true;
            }
        }
        floatingTextController.DisplayPlayerText(transform, "-" + (dmg).ToString(), Color.red);
        GameManager.totalDamageTaken += dmg;
        foreach (InventoryManager.Skill sc in gameplayManager.inventory.skillList) //Check damageTaken trigger skill condition
        {
            if (sc.skillController != null)
            {
                if (sc.skillController.skillTrigger.useDamageTakenTrigger)
                {
                    sc.skillController.skillTrigger.currentCounter += dmg;
                    if (sc.skillController.currentCooldown <= 0f)
                        sc.skillController.UseSkill();
                }
            }
        }
        if (currentHealth <= 0)
        {
            Die();
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
            foreach (InventoryManager.Skill sc in gameplayManager.inventory.skillList) //Check trigger skill condition
            {
                if (sc.skillController != null)
                {
                    if (sc.skillController.skillTrigger.useHealTrigger)
                    {
                        sc.skillController.skillTrigger.currentCounter += maxHealth - currentHealth;
                        if (sc.skillController.currentCooldown <= 0f)
                            sc.skillController.UseSkill();
                    }
                }
            }
        }
        else
        {
            floatingTextController.DisplayPlayerText(transform, "+" + (amt).ToString(), Color.green);
            currentHealth += amt;
            GameManager.totalHealing += amt;
            foreach (InventoryManager.Skill sc in gameplayManager.inventory.skillList) //Check trigger skill condition
            {
                if (sc.skillController != null)
                {
                    if (sc.skillController.skillTrigger.useHealTrigger)
                    {
                        sc.skillController.skillTrigger.currentCounter += amt;
                        if (sc.skillController.currentCooldown <= 0f)
                            sc.skillController.UseSkill();
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
            foreach (InventoryManager.Skill sc in gameplayManager.inventory.skillList) //Check trigger skill condition
            {
                if (sc.skillController != null)
                {
                    if (sc.skillController.skillTrigger.useBloodTrigger)
                    {
                        sc.skillController.skillTrigger.currentCounter += degen;
                        if (sc.skillController.currentCooldown <= 0f)
                            sc.skillController.UseSkill();
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
                foreach (InventoryManager.Skill sc in gameplayManager.inventory.skillList) //Check trigger skill condition
                {
                    if (sc.skillController != null)
                    {
                        if (sc.skillController.skillTrigger.useBloodTrigger)
                        {
                            sc.skillController.skillTrigger.currentCounter += currentHealth - 1;
                            if (sc.skillController.currentCooldown <= 0f)
                                sc.skillController.UseSkill();
                        }
                    }
                }
            }
            else
            { 
                TakeDamage(amt, false, true);
                GameManager.totalDegen += amt;
                foreach (InventoryManager.Skill sc in gameplayManager.inventory.skillList) //Check trigger skill condition
                {
                    if (sc.skillController != null)
                    {
                        if (sc.skillController.skillTrigger.useBloodTrigger)
                        {
                            sc.skillController.skillTrigger.currentCounter += amt;
                            if (sc.skillController.currentCooldown <= 0f)
                                sc.skillController.UseSkill();
                        }
                    }
                }
            }
        }
    }

    public void CheckHPBarColor()
    {
        gameplayManager.hpText.text = currentHealth.ToString();
        if (currentHealth <= 0)
        {
            healthBarImage.gameObject.SetActive(false);
            return;
        }
        float hpPercent = currentHealth * 100 / maxHealth;
        if (hpPercent <= 30)
        {
            healthBarImage.color = Color.red;
        }
        else if (hpPercent <= 60)
        {
            healthBarImage.color = Color.yellow;
        }
        else
        {
            healthBarImage.color = Color.green;
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

        if (regen - degen == 0) gameplayManager.regenText.text = "";
        else if (regen - degen < 0) gameplayManager.regenText.text = "<color=red> -" + (regen - degen).ToString() + "<color=white> HP/s";
        else gameplayManager.regenText.text = "<color=green> +" + (regen - degen).ToString() + "<color=white> HP/s";
    }
    public IEnumerator<float> DamageFlash()
    {
        spriteRenderer.material = damageFlashMaterial;
        yield return Timing.WaitForSeconds(0.1f);
        spriteRenderer.material = defaultMaterial;
    }
}
